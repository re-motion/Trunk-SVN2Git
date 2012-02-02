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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement
{
  // TODO 3658: Inject event sink and use instead of ListenerManager
  /// <summary>
  /// Manages the data (<see cref="DataContainer"/> instances, <see cref="IRelationEndPoint"/> instances, and invalid objects) for a 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class DataManager : ISerializable, IDeserializationCallback, IDataManager
  {
    private ClientTransaction _clientTransaction;

    private DataContainerMap _dataContainerMap;
    
    private IInvalidDomainObjectManager _invalidDomainObjectManager;
    private IObjectLoader _objectLoader;
    private DomainObjectStateCache _domainObjectStateCache;
    private IRelationEndPointManager _relationEndPointManager;

    private object[] _deserializedData; // only used for deserialization

    public DataManager (
        ClientTransaction clientTransaction, 
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IObjectLoader objectLoader,
        IRelationEndPointManager relationEndPointManager)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("relationEndPointManager", relationEndPointManager);

      _clientTransaction = clientTransaction;
      _dataContainerMap = new DataContainerMap (clientTransaction);

      _invalidDomainObjectManager = invalidDomainObjectManager;
      _objectLoader = objectLoader;
      _domainObjectStateCache = new DomainObjectStateCache (clientTransaction);

      _relationEndPointManager = relationEndPointManager;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IDataContainerMapReadOnlyView DataContainers
    {
      get { return _dataContainerMap; }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { return _relationEndPointManager.RelationEndPoints; }
    }

    // TODO 4499: Remove
    public DomainObjectStateCache DomainObjectStateCache
    {
      get { return _domainObjectStateCache; }
    }

    public IEnumerable<PersistableData> GetLoadedDataByObjectState (params StateType[] domainObjectStates)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("domainObjectStates", domainObjectStates);

      var stateSet = new StateValueSet (domainObjectStates);

      var matchingObjects = from dataContainer in DataContainers
                            let domainObject = dataContainer.DomainObject
                            let state = domainObject.TransactionContext[_clientTransaction].State
                            where stateSet.Matches (state)
                            let associatedEndPointSequence = 
                                dataContainer.AssociatedRelationEndPointIDs.Select (GetRelationEndPointWithoutLoading).Where (ep => ep != null)
                            select new PersistableData (domainObject, state, dataContainer, associatedEndPointSequence);
      return matchingObjects;
    }

    // TODO 4498: Remove
    public IEnumerable<PersistableData> GetNewChangedDeletedData ()
    {
      return GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New);
    }

    // TODO 4411: Remove
    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return from endPointID in dataContainer.AssociatedRelationEndPointIDs
             let endPoint = GetRelationEndPointWithLazyLoad (endPointID)
             let oppositeRelationEndPointIDs = endPoint.GetOppositeRelationEndPointIDs ()
             from oppositeEndPointID in oppositeRelationEndPointIDs
             select GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    // TODO 4498: Remove
    public bool HasRelationChanged (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return dataContainer.AssociatedRelationEndPointIDs
          .Select (GetRelationEndPointWithoutLoading)
          .Any (endPoint => endPoint != null && endPoint.HasChanged);
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      if (!dataContainer.HasDomainObject)
        throw new InvalidOperationException ("The DomainObject of a DataContainer must be set before it can be registered with a transaction.");

      if (_dataContainerMap[dataContainer.ID] != null)
        throw new InvalidOperationException (string.Format ("A DataContainer with ID '{0}' already exists in this transaction.", dataContainer.ID));

      dataContainer.SetClientTransaction (_clientTransaction);

      _dataContainerMap.Register (dataContainer);
      _relationEndPointManager.RegisterEndPointsForDataContainer (dataContainer);
    }

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      var unregisterEndPointsCommand = _relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer);
      var unregisterDataContainerCommand = CreateUnregisterDataContainerCommand (dataContainer.ID);
      var compositeCommand = new CompositeCommand (unregisterEndPointsCommand, unregisterDataContainerCommand);

      try
      {
        compositeCommand.NotifyAndPerform ();
      }
      catch (Exception ex)
      {
        var message = string.Format ("Cannot discard data for object '{0}': {1}", dataContainer.ID, ex.Message);
        throw new InvalidOperationException (message, ex);
      }

      dataContainer.Discard ();

      var domainObject = dataContainer.DomainObject;
      _invalidDomainObjectManager.MarkInvalid (domainObject);
    }

    public void MarkInvalid (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (!_clientTransaction.IsEnlisted (domainObject))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot mark DomainObject '{0}' invalid, because it belongs to a different ClientTransaction.",
            domainObject.ID);
      }

      if (DataContainers[domainObject.ID] != null)
      {
        var message = string.Format ("Cannot mark DomainObject '{0}' invalid because there is data registered for the object.", domainObject.ID);
        throw new InvalidOperationException (message);
      }

      if (RelationEndPointID.GetAllRelationEndPointIDs (domainObject.ID).Any (id => _relationEndPointManager.GetRelationEndPointWithoutLoading (id) != null))
      {
        var message = string.Format (
            "Cannot mark DomainObject '{0}' invalid because there are relation end-points registered for the object.", domainObject.ID);
        throw new InvalidOperationException (message);
      }

      _invalidDomainObjectManager.MarkInvalid (domainObject);
    }

    public void Commit ()
    {
      var deletedDataContainers = _dataContainerMap.Where (dc => dc.State == StateType.Deleted).ToList();

      _relationEndPointManager.CommitAllEndPoints();

      foreach (var deletedDataContainer in deletedDataContainers)
        Discard (deletedDataContainer);

      _dataContainerMap.CommitAllDataContainers();
    }

    public void Rollback ()
    {
      var newDataContainers = _dataContainerMap.Where (dc => dc.State == StateType.New).ToList();

      // roll back end point state before discarding data containers because Discard checks that no dangling end points are created
      _relationEndPointManager.RollbackAllEndPoints();

      // discard new data containers before rolling back data container state - new data containers cannot be rolled back
      foreach (var newDataContainer in newDataContainers)
        Discard (newDataContainer);

      _dataContainerMap.RollbackAllDataContainers();
    }

    public DataContainer GetDataContainerWithoutLoading (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid (objectID))
        throw new ObjectInvalidException (objectID);

      return DataContainers[objectID];
    }

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var dataContainer = GetDataContainerWithoutLoading (objectID);
      return dataContainer ?? LoadLazyDataContainer (objectID);
    }

    public IEnumerable<DataContainer> GetDataContainersWithLazyLoad (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var objectIDsAsCollection = objectIDs.ConvertToCollection();

      var idsToBeLoaded = objectIDsAsCollection.Where (id => GetDataContainerWithoutLoading (id) == null);
      _objectLoader.LoadObjects (idsToBeLoaded, throwOnNotFound);
      return objectIDsAsCollection.Select (GetDataContainerWithoutLoading);
    }

    public void LoadLazyCollectionEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var collectionEndPoint = GetRelationEndPointWithoutLoading (endPointID) as ICollectionEndPoint;

      if (collectionEndPoint == null)
        throw new ArgumentException ("The given ID does not identify an ICollectionEndPoint managed by this DataManager.", "endPointID");

      if (collectionEndPoint.IsDataComplete)
        throw new InvalidOperationException ("The given end-point cannot be loaded, its data is already complete.");

      var loadedData = _objectLoader.GetOrLoadRelatedObjects (endPointID);
      var domainObjects = loadedData.Select (data => data.GetDomainObjectReference()).ToArray();
      collectionEndPoint.MarkDataComplete (domainObjects);
    }

    public void LoadLazyVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var virtualObjectEndPoint = GetRelationEndPointWithoutLoading (endPointID) as IVirtualObjectEndPoint;

      if (virtualObjectEndPoint == null)
        throw new ArgumentException ("The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.", "endPointID");

      if (virtualObjectEndPoint.IsDataComplete)
        throw new InvalidOperationException ("The given end-point cannot be loaded, its data is already complete.");

      var loadedObjectData = _objectLoader.GetOrLoadRelatedObject (endPointID);
      var domainObject = loadedObjectData.GetDomainObjectReference();

      // Since RelationEndPointManager.RegisterEndPoint contains a query optimization for 1:1 relations, it is possible that
      // loading the related object has already marked the end-point complete. In that case, we won't call it again (to avoid an exception).
      if (!virtualObjectEndPoint.IsDataComplete)
        virtualObjectEndPoint.MarkDataComplete (domainObject);
    }

    public DataContainer LoadLazyDataContainer (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (_dataContainerMap[objectID] != null)
        throw new InvalidOperationException ("The given DataContainer cannot be loaded, its data is already available.");

      _objectLoader.LoadObject (objectID);

      var dataContainer = _dataContainerMap[objectID];
      Assertion.IsNotNull (dataContainer);
      return dataContainer;
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      return _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      return _relationEndPointManager.GetRelationEndPointWithoutLoading (endPointID);
    }

    public IVirtualEndPoint GetOrCreateVirtualEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      return _relationEndPointManager.GetOrCreateVirtualEndPoint (endPointID);
    }

    public IDataManagementCommand CreateDeleteCommand (DomainObject deletedObject)
    {
      ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);

      if (!_clientTransaction.IsEnlisted (deletedObject))
      {
        throw CreateClientTransactionsDifferException (
            "Cannot delete DomainObject '{0}', because it belongs to a different ClientTransaction.",
            deletedObject.ID);
      }

      DomainObjectCheckUtility.EnsureNotInvalid (deletedObject, ClientTransaction);

      if (deletedObject.TransactionContext[_clientTransaction].State == StateType.Deleted)
        return new NopCommand();

      return new DeleteCommand (ClientTransaction, deletedObject);
    }

    public IDataManagementCommand CreateUnloadCommand (params ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var domainObjects = new List<DomainObject>();
      var problematicDataContainers = new List<DataContainer>();
      var commands = new List<IDataManagementCommand>();

      foreach (var objectID in objectIDs)
      {
        var dataContainer = GetDataContainerWithoutLoading (objectID);
        if (dataContainer != null)
        {
          domainObjects.Add (dataContainer.DomainObject);

          if (dataContainer.State != StateType.Unchanged)
          {
            problematicDataContainers.Add (dataContainer);
          }
          else
          {
            commands.Add (CreateUnregisterDataContainerCommand (objectID));
            commands.Add (_relationEndPointManager.CreateUnregisterCommandForDataContainer (dataContainer));
          }
        }
      }

      if (problematicDataContainers.Count != 0)
      {
        var itemList = SeparatedStringBuilder.Build (", ", problematicDataContainers.Select (dc => string.Format ("'{0}' ({1})", dc.ID, dc.State)));
        var message = string.Format (
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: {0}.",
            itemList);
        return new ExceptionCommand (new InvalidOperationException (message));
      }

      if (domainObjects.Count == 0)
      {
        Assertion.IsTrue (commands.Count == 0);
        return new NopCommand();
      }
      else
      {
        var compositeCommand = new CompositeCommand (commands);
        return new UnloadCommand (_clientTransaction, domainObjects, compositeCommand);
      }
    }

    public IDataManagementCommand CreateUnloadVirtualEndPointsCommand (params RelationEndPointID[] endPointIDs)
    {
      ArgumentUtility.CheckNotNull ("endPointIDs", endPointIDs);

      var endPointsOfNewOrDeletedObjects = (from endPointID in endPointIDs
                                            let owningDataContainer = GetDataContainerWithoutLoading (endPointID.ObjectID)
                                            where owningDataContainer != null && (owningDataContainer.State == StateType.Deleted || owningDataContainer.State == StateType.New)
                                            select endPointID).ConvertToCollection();
      if (endPointsOfNewOrDeletedObjects.Count > 0)
      {
        var message = "Cannot unload the following relation end-points because they belong to new or deleted objects: "
            + SeparatedStringBuilder.Build (", ", endPointsOfNewOrDeletedObjects) + ".";
        var exception = new InvalidOperationException (message);
        return new ExceptionCommand (exception);
      }

      return _relationEndPointManager.CreateUnloadVirtualEndPointsCommand (endPointIDs);
    }

    public IDataManagementCommand CreateUnloadAllCommand ()
    {
      var domainObjects = DataContainers.Select (dc => dc.DomainObject).ConvertToCollection();
      if (domainObjects.Count == 0)
        return new NopCommand();
      else
        return new UnloadAllCommand (
            _relationEndPointManager, _dataContainerMap, _invalidDomainObjectManager, _clientTransaction, _clientTransaction.ListenerManager);
    }

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
    {
      return new ClientTransactionsDifferException (String.Format (message, args));
    }

    private UnregisterDataContainerCommand CreateUnregisterDataContainerCommand (ObjectID objectID)
    {
      return new UnregisterDataContainerCommand (objectID, _dataContainerMap);
    }

    #region Serialization

    protected DataManager (SerializationInfo info, StreamingContext context)
    {
      _deserializedData = (object[]) info.GetValue ("doInfo.GetData", typeof (object[]));
    }

    void IDeserializationCallback.OnDeserialization (object sender)
    {
      var doInfo = new FlattenedDeserializationInfo (_deserializedData);
      _clientTransaction = doInfo.GetValueForHandle<ClientTransaction>();
      _dataContainerMap = doInfo.GetValue<DataContainerMap>();
      _relationEndPointManager = doInfo.GetValueForHandle<RelationEndPointManager>();
      _domainObjectStateCache = doInfo.GetValue<DomainObjectStateCache>();
      _invalidDomainObjectManager = doInfo.GetValue<IInvalidDomainObjectManager> ();
      _objectLoader = doInfo.GetValueForHandle<IObjectLoader>();

      _deserializedData = null;
      doInfo.SignalDeserializationFinished();
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      var doInfo = new FlattenedSerializationInfo();
      doInfo.AddHandle (_clientTransaction);
      doInfo.AddValue (_dataContainerMap);
      doInfo.AddHandle (_relationEndPointManager);
      doInfo.AddValue (_domainObjectStateCache);
      doInfo.AddValue (_invalidDomainObjectManager);
      doInfo.AddHandle (_objectLoader);
      
      info.AddValue ("doInfo.GetData", doInfo.GetData());
    }

    #endregion
  }
}