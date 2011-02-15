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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public abstract class ObjectEndPoint : RelationEndPoint, IObjectEndPoint
  {
    private IObjectEndPointSyncState _syncState; // keeps track of whether this end-point is synchronised with the opposite end point

    protected ObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
       _syncState = new UnsynchronizedObjectEndPointSyncState();
    }

    public abstract ObjectID OppositeObjectID { get; protected set; }
    public abstract ObjectID OriginalOppositeObjectID { get; }

    public void MarkSynchronized ()
    {
      _syncState = new SynchronizedObjectEndPointSyncState();
    }

    public void MarkUnsynchronized ()
    {
      _syncState = new UnsynchronizedObjectEndPointSyncState();
    }

    public DomainObject GetOppositeObject (bool includeDeleted)
    {
      if (OppositeObjectID == null)
        return null;
      else if (includeDeleted && ClientTransaction.IsInvalid (OppositeObjectID))
        return ClientTransaction.GetInvalidObjectReference (OppositeObjectID);
      else
        return ClientTransaction.GetObject (OppositeObjectID, includeDeleted);
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      if (OriginalOppositeObjectID == null)
        return null;

      return ClientTransaction.GetObject (OriginalOppositeObjectID, true);
    }

    public override bool IsDataComplete
    {
      get { return true; }
    }

    public override void EnsureDataComplete ()
    {
      // nothing to do, ObjectEndPoints' data is always complete
      Assertion.IsTrue (IsDataComplete);
    }

    public override void CheckMandatory ()
    {
      if (OppositeObjectID == null)
      {
        throw CreateMandatoryRelationNotSetException (
            GetDomainObjectReference(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' cannot be null.",
            PropertyName,
            ObjectID);
      }
    }

    public override sealed IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var currentRelatedObject = GetOppositeObject (true);
      if (removedRelatedObject != currentRelatedObject)
      {
        string removedID = removedRelatedObject.ID.ToString();
        string currentID = currentRelatedObject != null ? currentRelatedObject.ID.ToString() : "<null>";

        var message = string.Format (
            "Cannot remove object '{0}' from object end point '{1}' - it currently holds object '{2}'.",
            removedID,
            PropertyName,
            currentID);
        throw new InvalidOperationException (message);
      }

      return CreateSetCommand (null);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _syncState.CreateDeleteCommand (this, id => OppositeObjectID = id);
    }

    public virtual IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return _syncState.CreateSetCommand (this, newRelatedObject, id => OppositeObjectID = id);
    }

    public override void SetValueFrom (IRelationEndPoint source)
    {
      var sourceObjectEndPoint = ArgumentUtility.CheckNotNullAndType<ObjectEndPoint> ("source", source);

      if (Definition != sourceObjectEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      // ReSharper disable RedundantCheckBeforeAssignment
      if (OppositeObjectID != sourceObjectEndPoint.OppositeObjectID)
        OppositeObjectID = sourceObjectEndPoint.OppositeObjectID;
      // ReSharper restore RedundantCheckBeforeAssignment

      if (sourceObjectEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }

    public override IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();
      if (oppositeEndPointDefinition.IsAnonymous || OppositeObjectID == null)
        return Enumerable.Empty<IRelationEndPoint>();
      else
      {
        var oppositeEndPointID = new RelationEndPointID (OppositeObjectID, oppositeEndPointDefinition);
        return new[] { dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID) };
      }
    }

    #region Serialization

    protected ObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _syncState = info.GetValue<IObjectEndPointSyncState>();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (_syncState);
    }

    #endregion
  }
}