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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where all of its data is available (ie., the end-point has been (lazily) loaded).
  /// </summary>
  public class CompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

    private readonly ICollectionEndPointDataKeeper _dataKeeper;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ClientTransaction _clientTransaction;

    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;

    public CompleteCollectionEndPointLoadState (
        ICollectionEndPointDataKeeper dataKeeper,
        IRelationEndPointProvider endPointProvider,
        ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _dataKeeper = dataKeeper;
      _endPointProvider = endPointProvider;
      _clientTransaction = clientTransaction;

      _unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint>();
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IRealObjectEndPoint[] UnsynchronizedOppositeEndPoints
    {
      get { return _unsynchronizedOppositeEndPoints.Values.ToArray (); }
    }

    public bool IsDataComplete ()
    {
      return true;
    }

    public void EnsureDataComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      // Data is already complete
    }

    public void MarkDataComplete (ICollectionEndPoint collectionEndPoint, DomainObject[] items, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already complete.");
    }

    public void MarkDataIncomplete (ICollectionEndPoint collectionEndPoint, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      _clientTransaction.TransactionEventSink.RelationEndPointUnloading (_clientTransaction, collectionEndPoint);

      stateSetter (_dataKeeper);

      foreach (var oppositeEndPoint in _unsynchronizedOppositeEndPoints.Values)
        collectionEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public ReadOnlyCollectionDataDecorator GetCollectionData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return new ReadOnlyCollectionDataDecorator(_dataKeeper.CollectionData, true);
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      var collectionType = collectionEndPoint.Definition.PropertyType;
      return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _dataKeeper.OriginalCollectionData);
    }

    public void RegisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_dataKeeper.OriginalCollectionData.ContainsObjectID (oppositeEndPoint.ObjectID))
      {
        if (s_log.IsInfoEnabled)
        {
          s_log.InfoFormat (
              "ObjectEndPoint '{0}' is registered for already loaded CollectionEndPoint '{1}'. "
              + "The collection query result contained the item, so the ObjectEndPoint is marked as synchronzed.",
              oppositeEndPoint.ID,
              collectionEndPoint.ID);
        }

        _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
        oppositeEndPoint.MarkSynchronized();
      }
      else
      {
        if (s_log.IsWarnEnabled)
        {
          s_log.WarnFormat (
              "ObjectEndPoint '{0}' is registered for already loaded CollectionEndPoint '{1}'. "
              + "The collection query result did not contain the item, so the ObjectEndPoint is out-of-sync.",
              oppositeEndPoint.ID,
              collectionEndPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
        oppositeEndPoint.MarkUnsynchronized();
      }
    }

    public void UnregisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_unsynchronizedOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      {
        if (s_log.IsDebugEnabled)
        {
          s_log.DebugFormat (
              "Unsynchronized ObjectEndPoint '{0}' is unregistered from CollectionEndPoint '{1}'.", 
              oppositeEndPoint.ID, 
              collectionEndPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
      }
      else
      {
        if (s_log.IsInfoEnabled)
        {
          s_log.InfoFormat (
              "ObjectEndPoint '{0}' is unregistered from CollectionEndPoint '{1}'. The CollectionEndPoint is transitioned to incomplete state.",
              oppositeEndPoint.ID,
              collectionEndPoint.ID);
        }

        collectionEndPoint.MarkDataIncomplete();
        collectionEndPoint.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!oppositeEndPoint.IsSynchronized)
        throw new InvalidOperationException ("Cannot register end-points that are out-of-sync.");
      
      _dataKeeper.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public bool IsSynchronized (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      return !_dataKeeper.OriginalItemsWithoutEndPoints.Any();
    }

    public void Synchronize (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (s_log.IsDebugEnabled)
      {
        s_log.DebugFormat ("CollectionEndPoint '{0}' is synchronized.", collectionEndPoint.ID);
      }

      foreach (var item in _dataKeeper.OriginalItemsWithoutEndPoints)
        _dataKeeper.UnregisterOriginalItemWithoutEndPoint (item);
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectEndPoint '{0}' is marked as synchronized.", oppositeEndPoint.ID);
      
      if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID))
      {
        var message = string.Format (
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized();
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        ICollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);
      ArgumentUtility.CheckNotNull ("collectionSetter", collectionSetter);

      if (_unsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format (
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the opposite object property '{2}' of domain "
            + "object '{3}' is out of sync. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            _unsynchronizedOppositeEndPoints.Values.First().ObjectID);
        throw new InvalidOperationException (message);
      }

      if (_dataKeeper.OriginalItemsWithoutEndPoints.Length != 0)
      {
        var message = string.Format (
            "The collection of relation property '{0}' of domain object '{1}' cannot be replaced because the relation property is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{0}' property.",
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            _dataKeeper.OriginalItemsWithoutEndPoints.First().ID);
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
      return new CollectionEndPointRemoveCommand (collectionEndPoint, removedRelatedObject, _dataKeeper.CollectionData, _endPointProvider);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (_unsynchronizedOppositeEndPoints.Count != 0)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because the opposite object property '{2}' of domain object '{3}' is out of sync with the "
            + "collection property '{1}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{2}' property.",
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            _unsynchronizedOppositeEndPoints.Values.First().ObjectID);
        throw new InvalidOperationException (message);
      }

      if (_dataKeeper.OriginalItemsWithoutEndPoints.Length != 0)
      {
        var message = string.Format (
            "The domain object '{0}' cannot be deleted because its collection property '{1}' is out of sync with "
            + "the opposite object property '{2}' of domain object '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.Definition.GetMandatoryOppositeEndPointDefinition().PropertyName,
            _dataKeeper.OriginalItemsWithoutEndPoints.First().ID);
        throw new InvalidOperationException (message);
      }

      return new CollectionEndPointDeleteCommand (collectionEndPoint, _dataKeeper.CollectionData);
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);

      CheckAddedObject (insertedRelatedObject);
      return new CollectionEndPointInsertCommand (collectionEndPoint, index, insertedRelatedObject, _dataKeeper.CollectionData, _endPointProvider);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      return CreateInsertCommand (collectionEndPoint, addedRelatedObject, _dataKeeper.CollectionData.Count);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      CheckAddedObject (replacementObject);
      CheckRemovedObject (_dataKeeper.CollectionData.GetObject (index));

      var replacedObject = _dataKeeper.CollectionData.GetObject (index);
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (collectionEndPoint, replacedObject);
      else
        return new CollectionEndPointReplaceCommand (collectionEndPoint, replacedObject, index, replacementObject, _dataKeeper.CollectionData);
    }

    public void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      _dataKeeper.CollectionData.ReplaceContents (sourceEndPoint.Collection.Cast<DomainObject>());

      if (sourceEndPoint.HasBeenTouched || collectionEndPoint.HasChanged)
        collectionEndPoint.Touch();
    }

    public bool HasChanged ()
    {
      return _dataKeeper.HasDataChanged();
    }

    public void Commit ()
    {
      Assertion.IsTrue (
          _dataKeeper.CurrentOppositeEndPoints.All (ep => ep.IsSynchronized), 
          "We assume that it is not possible to register opposite end-points that are out-of-sync.");
      _dataKeeper.Commit ();
    }

    public void Rollback ()
    {
      _dataKeeper.Rollback();
    }

    private void CheckAddedObject (DomainObject domainObject)
    {
      if (_unsynchronizedOppositeEndPoints.ContainsKey (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      if (_dataKeeper.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be added to collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    private void CheckRemovedObject (DomainObject domainObject)
    {
      if (_unsynchronizedOppositeEndPoints.ContainsKey (domainObject.ID))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because its object property "
            + "'{3}' is out of sync with the collection property. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{3}' property.",
            domainObject.ID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }

      if (_dataKeeper.ContainsOriginalItemWithoutEndPoint (domainObject))
      {
        var message = string.Format (
            "The domain object with ID '{0}' cannot be replaced or removed from collection property '{1}' of object '{2}' because the property is "
            + "out of sync with the opposite object property '{3}'. To make this change, synchronize the two properties by calling the "
            + "'BidirectionalRelationSyncService.Synchronize' method on the '{1}' property.",
            domainObject.ID,
            _dataKeeper.EndPointID.Definition.PropertyName,
            _dataKeeper.EndPointID.ObjectID,
            _dataKeeper.EndPointID.Definition.GetOppositeEndPointDefinition().PropertyName);
        throw new InvalidOperationException (message);
      }
    }

    #region Serialization

    public CompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      var unsynchronizedOppositeEndPoints = new List<IRealObjectEndPoint>();
      info.FillCollection (unsynchronizedOppositeEndPoints);
      _unsynchronizedOppositeEndPoints = unsynchronizedOppositeEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_dataKeeper);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_clientTransaction);
      info.AddCollection (_unsynchronizedOppositeEndPoints.Values);
    }

    #endregion
  }
}