// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Registers and unregisters end-points in/from a <see cref="RelationEndPointMap"/>.
  /// </summary>
  public class RelationEndPointRegistrationAgent : IRelationEndPointRegistrationAgent
  {
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly RelationEndPointMap2 _relationEndPoints;
    private readonly ClientTransaction _clientTransaction;

    public RelationEndPointRegistrationAgent (IRelationEndPointProvider endPointProvider, RelationEndPointMap2 relationEndPoints, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("relationEndPoints", relationEndPoints);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _endPointProvider = endPointProvider;
      _relationEndPoints = relationEndPoints;
      _clientTransaction = clientTransaction;
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public void RegisterEndPoint (IRelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      _relationEndPoints.AddEndPoint (endPoint);

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        RegisterOppositeForRealObjectEndPoint (realObjectEndPoint);
    }

    public void UnregisterEndPoint (IRelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      if (!IsUnregisterable (endPoint))
      {
        var message = String.Format (
            "Cannot remove end-point '{0}' because it has changed. End-points can only be unregistered when they are unchanged.",
            endPoint.ID);
        throw new InvalidOperationException (message);
      }

      _relationEndPoints.RemoveEndPoint (endPoint.ID);

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        UnregisterOppositeForRealObjectEndPoint (realObjectEndPoint);
    }

    public bool IsUnregisterable (IRelationEndPoint endPoint)
    {
      // An end-point must be unchanged to be unregisterable.
      if (endPoint.HasChanged)
        return false;

      // If it is a real object end-point pointing to a non-null object, and the opposite end-point is loaded, the opposite (virtual) end-point 
      // must be unchanged. Virtual end-points cannot exist in changed state without their opposite real end-points.
      // (This only affects 1:n relations: for those, the opposite virtual end-point can be changed although the (one of many) real end-point is 
      // unchanged. For 1:1 relations, the real and virtual end-points always have an equal HasChanged flag.)

      var maybeOppositeEndPoint =
          Maybe
            .ForValue (endPoint as IRealObjectEndPoint)
            .Select (GetOppositeVirtualEndPoint);
      if (maybeOppositeEndPoint.Where (ep => ep.HasChanged).HasValue)
        return false;

      return true;
    }

    private void RegisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPointID = GetOppositeVirtualEndPointID (realObjectEndPoint);
      if (oppositeVirtualEndPointID == null)
      {
        realObjectEndPoint.MarkSynchronized ();
        return;
      }

      var oppositeVirtualEndPoint = (IVirtualEndPoint) _endPointProvider.GetRelationEndPointWithMinimumLoading (oppositeVirtualEndPointID);
      oppositeVirtualEndPoint.RegisterOriginalOppositeEndPoint (realObjectEndPoint);

      // Optimization for 1:1 relations: to avoid a database query, we'll mark the virtual end-point complete when the first opposite foreign key
      // is registered with it. We can only do this in root transactions; in sub-transactions we need the query to occur so that we get the same
      // relation state in the sub-transaction as in the root transaction even in the case of unsynchronized end-points.

      var oppositeVirtualObjectEndPoint = oppositeVirtualEndPoint as IVirtualObjectEndPoint;
      if (_clientTransaction.ParentTransaction == null && oppositeVirtualObjectEndPoint != null && !oppositeVirtualObjectEndPoint.IsDataComplete)
        oppositeVirtualObjectEndPoint.MarkDataComplete (realObjectEndPoint.GetDomainObjectReference ());
    }

    private void UnregisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeEndPoint = GetOppositeVirtualEndPoint (realObjectEndPoint);
      if (oppositeEndPoint == null)
      {
        realObjectEndPoint.ResetSyncState();
        return;
      }

      oppositeEndPoint.UnregisterOriginalOppositeEndPoint (realObjectEndPoint);
      if (oppositeEndPoint.CanBeCollected)
      {
        Assertion.IsTrue (IsUnregisterable (oppositeEndPoint), "Caller checks that this end-point is unregisterable.");
        _relationEndPoints.RemoveEndPoint (oppositeEndPoint.ID);
      }
    }

    private RelationEndPointID GetOppositeVirtualEndPointID (IRealObjectEndPoint realObjectEndPoint)
    {
      if (realObjectEndPoint.OppositeObjectID == null)
        return null;

      var oppositeDefinition = realObjectEndPoint.Definition.GetOppositeEndPointDefinition ();
      if (oppositeDefinition.IsAnonymous)
        return null;

      return RelationEndPointID.Create (realObjectEndPoint.OppositeObjectID, oppositeDefinition);
    }

    private IVirtualEndPoint GetOppositeVirtualEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeID = GetOppositeVirtualEndPointID (realObjectEndPoint);
      if (oppositeID == null)
        return null;

      var oppositeEndPoint = (IVirtualEndPoint) _relationEndPoints[oppositeID];
      Assertion.IsNotNull (oppositeEndPoint, "RegisterEndPoint ensures that for every real end-point, the opposite virtual end-point exists.");
      return oppositeEndPoint;
    }
  }
}