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
      if (_relationEndPoints[endPoint.ID] != null)
      {
        var message = string.Format ("A relation end-point with ID '{0}' has already been registered.", endPoint.ID);
        throw new InvalidOperationException (message);
      }
      
      _relationEndPoints.AddEndPoint (endPoint);

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        RegisterOppositeForRealObjectEndPoint (realObjectEndPoint);
    }

    public void UnregisterEndPoint (IRelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      if (_relationEndPoints[endPoint.ID] != endPoint)
      {
        var message = string.Format ("End-point '{0}' is not part of this map.\r\nParameter name: endPoint", endPoint.ID);
        throw new ArgumentException (message);
      }

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        UnregisterOppositeForRealObjectEndPoint (realObjectEndPoint);

      _relationEndPoints.RemoveEndPoint (endPoint.ID);
    }

    private void RegisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPointID = RelationEndPointID.CreateOpposite (realObjectEndPoint.Definition, realObjectEndPoint.OriginalOppositeObjectID);
      if (oppositeVirtualEndPointID.Definition.IsAnonymous)
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
      ArgumentUtility.CheckNotNull ("realObjectEndPoint", realObjectEndPoint);

      var oppositeEndPointID = RelationEndPointID.CreateOpposite (realObjectEndPoint.Definition, realObjectEndPoint.OriginalOppositeObjectID);
      if (oppositeEndPointID.Definition.IsAnonymous)
      {
        realObjectEndPoint.ResetSyncState ();
        return;
      }
      
      var oppositeEndPoint = (IVirtualEndPoint) _endPointProvider.GetRelationEndPointWithoutLoading (oppositeEndPointID);
      if (oppositeEndPoint == null)
      {
        var message = string.Format (
            "Opposite end-point of '{0}' not found. When unregistering a non-virtual bidirectional end-point, the opposite end-point must exist.", 
            realObjectEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      oppositeEndPoint.UnregisterOriginalOppositeEndPoint (realObjectEndPoint);
      if (oppositeEndPoint.CanBeCollected)
        _relationEndPoints.RemoveEndPoint (oppositeEndPoint.ID);
    }
  }
}