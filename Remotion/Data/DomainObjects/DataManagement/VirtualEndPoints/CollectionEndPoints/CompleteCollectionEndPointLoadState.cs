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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteCollectionEndPointLoadState
      : CompleteVirtualEndPointLoadStateBase<ICollectionEndPoint, ReadOnlyCollectionDataDecorator, ICollectionEndPointDataKeeper>,
        ICollectionEndPointLoadState
  {
    public CompleteCollectionEndPointLoadState (
        ICollectionEndPointDataKeeper dataKeeper,
        IRelationEndPointProvider endPointProvider,
        ClientTransaction clientTransaction)
        : base(dataKeeper, endPointProvider, clientTransaction)
    {
    }

    public override ReadOnlyCollectionDataDecorator GetData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return new ReadOnlyCollectionDataDecorator(DataKeeper.CollectionData, true);
    }

    public override ReadOnlyCollectionDataDecorator GetOriginalData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return DataKeeper.OriginalCollectionData;
    }

    public override void Synchronize (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      if (Log.IsDebugEnabled)
        Log.DebugFormat ("End-point '{0}' is synchronized.", endPoint.ID);

      foreach (var item in DataKeeper.OriginalItemsWithoutEndPoints)
        DataKeeper.UnregisterOriginalItemWithoutEndPoint (item);
    }

    public override void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      DataKeeper.CollectionData.ReplaceContents (sourceEndPoint.Collection.Cast<DomainObject> ());

      if (sourceEndPoint.HasBeenTouched || collectionEndPoint.HasChanged)
        collectionEndPoint.Touch ();
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        ICollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);
      ArgumentUtility.CheckNotNull ("collectionSetter", collectionSetter);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format (
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the opposite object property '{2}' of domain "
            + "object '{3}' is out of sync. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            UnsynchronizedOppositeEndPoints.First().ObjectID);
        throw new InvalidOperationException (message);
      }

      if (DataKeeper.OriginalItemsWithoutEndPoints.Length != 0)
      {
        var message = string.Format (
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the relation property is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{0}' property.",
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            DataKeeper.OriginalItemsWithoutEndPoints.First().ID);
        throw new InvalidOperationException (message);
      }

      return new CollectionEndPointSetCollectionCommand (
          collectionEndPoint,
          newCollection,
          collectionSetter,
          collectionEndPoint.Collection,
          newCollection);
    }

    public IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      CheckRemovedObject (removedRelatedObject);
      return new CollectionEndPointRemoveCommand (collectionEndPoint, removedRelatedObject, DataKeeper.CollectionData, EndPointProvider);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (UnsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
            + "collection property '{1}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            UnsynchronizedOppositeEndPoints.First().ObjectID);
        throw new InvalidOperationException (message);
      }

      if (DataKeeper.OriginalItemsWithoutEndPoints.Length != 0)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because its collection property '{1}' is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            DataKeeper.OriginalItemsWithoutEndPoints.First().ID);
        throw new InvalidOperationException (message);
      }

      return new CollectionEndPointDeleteCommand (collectionEndPoint, DataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);

      CheckAddedObject (insertedRelatedObject);
      return new CollectionEndPointInsertCommand (collectionEndPoint, index, insertedRelatedObject, DataKeeper.CollectionData, EndPointProvider);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return CreateInsertCommand (collectionEndPoint, addedRelatedObject, DataKeeper.CollectionData.Count);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      CheckAddedObject (replacementObject);
      CheckRemovedObject (DataKeeper.CollectionData.GetObject (index));

      var replacedObject = DataKeeper.CollectionData.GetObject (index);
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (collectionEndPoint, replacedObject);
      else
        return new CollectionEndPointReplaceCommand (collectionEndPoint, replacedObject, index, replacementObject, DataKeeper.CollectionData);
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return DataKeeper.OriginalOppositeEndPoints;
    }

    protected override bool HasUnsynchronizedCurrentOppositeEndPoints ()
    {
      return DataKeeper.CurrentOppositeEndPoints.Any (ep => !ep.IsSynchronized);
    }

    private void CheckAddedObject (DomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      if (DataKeeper.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    private void CheckRemovedObject (DomainObject domainObject)
    {
      if (ContainsUnsynchronizedOppositeEndPoint (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      if (DataKeeper.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            DataKeeper.EndPointID.Definition.PropertyName,
            DataKeeper.EndPointID.ObjectID,
            DataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    #region Serialization

    public CompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
        : base(info)
    {
    }

    #endregion
  }
}