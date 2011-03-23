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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Keeps the data of a <see cref="VirtualObjectEndPoint"/>, managing current and original values as well as opposite end-points.
  /// </summary>
  public class VirtualObjectEndPointDataKeeper : IVirtualObjectEndPointDataKeeper
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IVirtualEndPointStateUpdateListener _updateListener;

    private ObjectID _currentOppositeObjectID;
    private ObjectID _originalOppositeObjectID;

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

    public ObjectID CurrentOppositeObjectID
    {
      get { return _currentOppositeObjectID; }
      set
      {
        _currentOppositeObjectID = value;
        _updateListener.StateUpdated (HasDataChanged());
      }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { return _originalOppositeObjectID; }
    }

    public IRealObjectEndPoint CurrentOppositeEndPoint
    {
      get { return _currentOppositeEndPoint; }
    }

    public IRealObjectEndPoint OriginalOppositeEndPoint
    {
      get { return _originalOppositeEndPoint; }
    }

    public bool ContainsOriginalObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return Equals (_originalOppositeObjectID, objectID);
    }

    public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      return _originalOppositeEndPoint == oppositeEndPoint;
    }

    public bool ContainsOriginalItemsWithoutEndPoints ()
    {
      return _originalOppositeObjectID != null && _originalOppositeEndPoint == null;
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_originalOppositeEndPoint != null)
        throw new InvalidOperationException ("The original opposite end-point has already been registered.");

      // Only set current end-point/value if they haven't already been set to a different value
      if (!HasDataChanged ())
      {
        _currentOppositeEndPoint = oppositeEndPoint;
        _currentOppositeObjectID = oppositeEndPoint.ObjectID;
      }

      _originalOppositeEndPoint = oppositeEndPoint;
      _originalOppositeObjectID = oppositeEndPoint.ObjectID;
      _updateListener.StateUpdated (HasDataChanged ());
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_originalOppositeEndPoint != oppositeEndPoint)
        throw new InvalidOperationException ("The original opposite end-point has not been registered.");

      _originalOppositeEndPoint = null;
      _originalOppositeObjectID = null;

      _currentOppositeEndPoint = null;
      _currentOppositeObjectID = null;

      _updateListener.StateUpdated (false);
    }

    public void RegisterOriginalItemWithoutEndPoint (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (_originalOppositeObjectID != null)
        throw new InvalidOperationException ("An original opposite item has already been registered.");

      // Only set current value if it hasn't already been set to a different value
      if (!HasDataChanged())
        _currentOppositeObjectID = objectID;

      _originalOppositeObjectID = objectID;
      _updateListener.StateUpdated (HasDataChanged());
    }

    void IVirtualEndPointDataKeeper.RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      RegisterOriginalItemWithoutEndPoint (domainObject.ID);
    }

    public void UnregisterOriginalItemWithoutEndPoint (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (objectID != _originalOppositeObjectID)
        throw new InvalidOperationException ("Cannot unregister original item, it has not been registered.");

      if (_originalOppositeEndPoint != null)
        throw new InvalidOperationException ("Cannot unregister original item, an end-point has been registered for it.");

      // Only set current value if it hasn't already been set to a different value
      if (!HasDataChanged ())
        _currentOppositeObjectID = null;

      _originalOppositeObjectID = null;
      _updateListener.StateUpdated (HasDataChanged ());
    }
    
    void IVirtualEndPointDataKeeper.UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      UnregisterOriginalItemWithoutEndPoint (domainObject.ID);
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
      return !Equals (CurrentOppositeObjectID, OriginalOppositeObjectID);
    }

    public void Commit ()
    {
      _originalOppositeObjectID = _currentOppositeObjectID;
      _originalOppositeEndPoint = _currentOppositeEndPoint;

      _updateListener.StateUpdated (false);
    }

    public void Rollback ()
    {
      _currentOppositeObjectID = _originalOppositeObjectID;
      _currentOppositeEndPoint = _originalOppositeEndPoint;

      _updateListener.StateUpdated (false);
    }

    #region Serialization

    public VirtualObjectEndPointDataKeeper (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _endPointID = info.GetValueForHandle<RelationEndPointID>();
      _updateListener = info.GetValueForHandle<IVirtualEndPointStateUpdateListener>();
      _originalOppositeEndPoint = info.GetValue<IRealObjectEndPoint>();
      _originalOppositeObjectID = info.GetValueForHandle<ObjectID>();
      _currentOppositeEndPoint = info.GetValue<IRealObjectEndPoint>();
      _currentOppositeObjectID = info.GetValueForHandle<ObjectID> ();
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_endPointID);
      info.AddHandle (_updateListener);
      info.AddValue (_originalOppositeEndPoint);
      info.AddHandle (_originalOppositeObjectID);
      info.AddValue (_currentOppositeEndPoint);
      info.AddHandle (_currentOppositeObjectID);
    }

    #endregion
  }
}