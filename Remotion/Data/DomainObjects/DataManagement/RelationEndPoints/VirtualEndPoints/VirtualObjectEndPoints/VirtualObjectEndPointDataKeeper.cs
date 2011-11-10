// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Keeps the data of a <see cref="VirtualObjectEndPoint"/>, managing current and original values as well as opposite end-points.
  /// </summary>
  public class VirtualObjectEndPointDataKeeper : IVirtualObjectEndPointDataKeeper
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IVirtualEndPointStateUpdateListener _updateListener;

    private DomainObject _currentOppositeObject;
    private DomainObject _originalOppositeObject;

    private IRealObjectEndPoint _currentOppositeEndPoint;
    private IRealObjectEndPoint _originalOppositeEndPoint;

    public VirtualObjectEndPointDataKeeper (RelationEndPointID endPointID, IVirtualEndPointStateUpdateListener updateListener)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("updateListener", updateListener);

      _endPointID = endPointID;
      _updateListener = updateListener;
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IVirtualEndPointStateUpdateListener UpdateListener
    {
      get { return _updateListener; }
    }

    public DomainObject CurrentOppositeObject
    {
      get { return _currentOppositeObject; }
      set
      {
        _currentOppositeObject = value;
        _updateListener.StateUpdated (HasDataChanged());
      }
    }

    public DomainObject OriginalOppositeObject
    {
      get { return _originalOppositeObject; }
    }

    public IRealObjectEndPoint CurrentOppositeEndPoint
    {
      get { return _currentOppositeEndPoint; }
    }

    public IRealObjectEndPoint OriginalOppositeEndPoint
    {
      get { return _originalOppositeEndPoint; }
    }

    public DomainObject OriginalItemWithoutEndPoint
    {
      get { return OriginalOppositeEndPoint == null ? OriginalOppositeObject : null; }
    }

    public bool ContainsOriginalObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return Equals (_originalOppositeObject.GetSafeID(), objectID);
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_originalOppositeEndPoint != null)
        throw new InvalidOperationException ("The original opposite end-point has already been registered.");

      var oppositeObject = oppositeEndPoint.GetDomainObjectReference();
      if (_originalOppositeObject != null && _originalOppositeObject != oppositeObject)
        throw new InvalidOperationException ("A different original opposite item has already been registered.");

      // Only set current end-point/value if they haven't already been set to a different value
      if (!HasDataChanged ())
      {
        _currentOppositeEndPoint = oppositeEndPoint;
        _currentOppositeObject = oppositeObject;
      }

      _originalOppositeEndPoint = oppositeEndPoint;
      _originalOppositeObject = oppositeObject;
      _updateListener.StateUpdated (HasDataChanged ());
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_originalOppositeEndPoint != oppositeEndPoint)
        throw new InvalidOperationException ("The original opposite end-point has not been registered.");

      _originalOppositeEndPoint = null;
      _originalOppositeObject = null;

      _currentOppositeEndPoint = null;
      _currentOppositeObject = null;

      _updateListener.StateUpdated (false);
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (_originalOppositeObject != null)
        throw new InvalidOperationException ("An original opposite item has already been registered.");

      Assertion.IsTrue (_originalOppositeEndPoint == null, "if _originalOppositeObject is null, _originalOppositeEndPoint must be null, too");

      // Only set current value if it hasn't already been set to a different value
      if (!HasDataChanged())
        _currentOppositeObject = domainObject;

      _originalOppositeObject = domainObject;
      _updateListener.StateUpdated (HasDataChanged());
    }

    public void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (domainObject != _originalOppositeObject)
        throw new InvalidOperationException ("Cannot unregister original item, it has not been registered.");

      if (_originalOppositeEndPoint != null)
        throw new InvalidOperationException ("Cannot unregister original item, an end-point has been registered for it.");

      // Only set current value if it hasn't already been set to a different value
      if (!HasDataChanged ())
        _currentOppositeObject = null;

      _originalOppositeObject = null;
      _updateListener.StateUpdated (HasDataChanged ());
    }
    
    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_currentOppositeEndPoint != null)
        throw new InvalidOperationException ("An opposite end-point has already been registered.");

      _currentOppositeEndPoint = oppositeEndPoint;
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_currentOppositeEndPoint != oppositeEndPoint)
        throw new InvalidOperationException ("The opposite end-point has not been registered.");

      _currentOppositeEndPoint = null;
    }

    public bool HasDataChanged ()
    {
      return !Equals (CurrentOppositeObject, OriginalOppositeObject);
    }

    public void Commit ()
    {
      _originalOppositeObject = _currentOppositeObject;
      _originalOppositeEndPoint = _currentOppositeEndPoint;

      _updateListener.StateUpdated (false);
    }

    public void Rollback ()
    {
      _currentOppositeObject = _originalOppositeObject;
      _currentOppositeEndPoint = _originalOppositeEndPoint;

      _updateListener.StateUpdated (false);
    }

    public void SetDataFromSubTransaction (IVirtualObjectEndPointDataKeeper sourceDataKeeper, IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("sourceDataKeeper", sourceDataKeeper);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _currentOppositeObject = sourceDataKeeper.CurrentOppositeObject;
      if (sourceDataKeeper.CurrentOppositeEndPoint == null)
      {
        _currentOppositeEndPoint = null;
      }
      else
      {
        _currentOppositeEndPoint =
            (IRealObjectEndPoint) endPointProvider.GetRelationEndPointWithoutLoading (sourceDataKeeper.CurrentOppositeEndPoint.ID);
        Assertion.IsNotNull (
            _currentOppositeEndPoint,
            "When committing a current virtual relation value from a sub-transaction, the opposite end-point is guaranteed to exist.");
      }

      _updateListener.StateUpdated (HasDataChanged ());
    }

    #region Serialization

    public VirtualObjectEndPointDataKeeper (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _endPointID = info.GetValueForHandle<RelationEndPointID>();
      _updateListener = info.GetValueForHandle<IVirtualEndPointStateUpdateListener>();
      _originalOppositeEndPoint = info.GetValue<IRealObjectEndPoint>();
      _originalOppositeObject = info.GetValueForHandle<DomainObject>();
      _currentOppositeEndPoint = info.GetValue<IRealObjectEndPoint>();
      _currentOppositeObject = info.GetValueForHandle<DomainObject> ();
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_endPointID);
      info.AddHandle (_updateListener);
      info.AddValue (_originalOppositeEndPoint);
      info.AddHandle (_originalOppositeObject);
      info.AddValue (_currentOppositeEndPoint);
      info.AddHandle (_currentOppositeObject);
    }

    #endregion
  }
}