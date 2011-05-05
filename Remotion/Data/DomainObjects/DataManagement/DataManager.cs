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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Manages the data (<see cref="DataContainer"/> instances, <see cref="IRelationEndPoint"/> instances, and invalid objects) for a 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class DataManager : ISerializable, IDeserializationCallback, IDataManager, ILazyLoader
  {
    private ClientTransaction _clientTransaction;
    private IClientTransactionListener _transactionEventSink;

    private DataContainerMap _dataContainerMap;
    private IRelationEndPointManager _relationEndPointManager;
    private IInvalidDomainObjectManager _invalidDomainObjectManager;
    private IObjectLoader _objectLoader;
    private DomainObjectStateCache _domainObjectStateCache;

    private object[] _deserializedData; // only used for deserialization

    public DataManager (
        ClientTransaction clientTransaction, 
        ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy, 
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IObjectLoader objectLoader)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);

      _clientTransaction = clientTransaction;
      _transactionEventSink = clientTransaction.TransactionEventSink;
      _dataContainerMap = new DataContainerMap (clientTransaction);

      var collectionEndPointDataKeeperFactory = new CollectionEndPointDataKeeperFactory (clientTransaction, collectionEndPointChangeDetectionStrategy);
      var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (clientTransaction);
      var relationEndPointRegistrationAgent = new RelationEndPointRegistrationAgent (this, _clientTransaction);
      _relationEndPointManager = new RelationEndPointManager (
          clientTransaction, 
          this, 
          this, 
          collectionEndPointDataKeeperFactory,
          virtualObjectEndPointDataKeeperFactory,
          relationEndPointRegistrationAgent);

      _invalidDomainObjectManager = invalidDomainObjectManager;
      _objectLoader = objectLoader;
      _domainObjectStateCache = new DomainObjectStateCache (clientTransaction);
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

    public DomainObjectStateCache DomainObjectStateCache
    {
      get { return _domainObjectStateCache; }
    }

    public IEnumerable<Tuple<DomainObject, DataContainer>> GetLoadedData ()
    {
      return DataContainers.Select (dc => Tuple.Create (dc.DomainObject, dc));
    }

    public IEnumerable<Tuple<DomainObject, DataContainer, StateType>> GetLoadedDataByObjectState (params StateType[] domainObjectStates)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("domainObjectStates", domainObjectStates);

      var stateSet = new StateValueSet (domainObjectStates);

      var matchingObjects = from tuple in GetLoadedData()
                            let domainObject = tuple.Item1
                            let state = domainObject.TransactionContext[_clientTransaction].State
                            where stateSet.Matches (state)
                            select Tuple.Create (domainObject, tuple.Item2, state);
      return matchingObjects;
    }

    public IEnumerable<Tuple<DomainObject, DataContainer, StateType>> GetChangedDataByObjectState ()
    {
      return GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New);
    }

    public IEnumerable<DataContainer> GetChangedDataContainersForCommit ()
    {
      foreach (var tuple in GetChangedDataByObjectState())
      {
        var dataContainer = tuple.Item2;
        var state = tuple.Item3;

        Assertion.IsTrue (state != StateType.NotLoadedYet);

        if (state != StateType.Deleted)
          CheckMandatoryRelations (dataContainer);

        if (dataContainer.State != StateType.Unchanged) // filter out those items whose state is only Changed due to relation changes
          yield return dataContainer;
      }
    }

    public IEnumerable<IRelationEndPoint> GetChangedRelationEndPoints ()
    {
      return _relationEndPointManager.RelationEndPoints.Where (endPoint => endPoint.HasChanged);
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (DataContainer dataContainer)
    {
      return from endPointID in dataContainer.AssociatedRelationEndPointIDs
             let endPoint = _relationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID)
             let oppositeRelationEndPointIDs = endPoint.GetOppositeRelationEndPointIDs ()
             from oppositeEndPointID in oppositeRelationEndPointIDs
             select _relationEndPointManager.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    public bool HasRelationChanged (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return dataContainer.AssociatedRelationEndPointIDs
          .Select (GetRelationEndPointWithoutLoading)
          .Any (endPoint => endPoint != null && endPoint.HasChanged);
    }

    public void CheckMandatoryRelations (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (RelationEndPointID endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (endPointID.Definition.IsMandatory)
        {
          IRelationEndPoint endPoint = GetRelationEndPointWithoutLoading (endPointID);
          if (endPoint != null && endPoint.IsDataComplete)
            endPoint.CheckMandatory();
        }
      }
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

    public bool TrySetCollectionEndPointData (RelationEndPointID relationEndPointID, DomainObject[] items)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("items", items);
      
      return _relationEndPointManager.TrySetCollectionEndPointData (relationEndPointID, items);
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

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      IRelationEndPoint[] endPoints;
      try
      {
        endPoints = (from endPointID in dataContainer.AssociatedRelationEndPointIDs
                     let endPoint = GetRelationEndPointWithoutLoading (endPointID)
                     where endPoint != null
                     select EnsureEndPointReferencesNothing (endPoint)).ToArray();
      }
      catch (InvalidOperationException ex)
      {
        var message = String.Format ("Cannot discard data container '{0}', it might leave dangling references: '{1}'", dataContainer.ID, ex.Message);
        throw new ArgumentException (message, "dataContainer", ex);
      }

      foreach (var endPoint in endPoints)
        _relationEndPointManager.RemoveEndPoint (endPoint.ID);

      _dataContainerMap.Remove (dataContainer.ID);

      dataContainer.Discard();

      var domainObject = dataContainer.DomainObject;
      if (_invalidDomainObjectManager.MarkInvalid (domainObject))
        _transactionEventSink.DataManagerDiscardingObject (_clientTransaction, domainObject.ID);
    }

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var dataContainer = GetDataContainerWithoutLoading (objectID);
      return dataContainer ?? LoadLazyDataContainer (objectID);
    }

    public DataContainer GetDataContainerWithoutLoading (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      if (_invalidDomainObjectManager.IsInvalid (id))
        throw new ObjectInvalidException (id);

      return DataContainers[id];
    }

    public void LoadLazyCollectionEndPoint (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      if (collectionEndPoint != GetRelationEndPointWithoutLoading (collectionEndPoint.ID))
        throw new ArgumentException ("The given end-point is not managed by this DataManager.", "collectionEndPoint");

      if (collectionEndPoint.IsDataComplete)
        throw new InvalidOperationException ("The given end-point cannot be loaded, its data is already complete.");

      var domainObjects = _objectLoader.LoadRelatedObjects (collectionEndPoint.ID, this);
      collectionEndPoint.MarkDataComplete (domainObjects);
    }

    public void LoadLazyVirtualObjectEndPoint (IVirtualObjectEndPoint virtualObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull ("virtualObjectEndPoint", virtualObjectEndPoint);

      if (virtualObjectEndPoint != GetRelationEndPointWithoutLoading (virtualObjectEndPoint.ID))
        throw new ArgumentException ("The given end-point is not managed by this DataManager.", "virtualObjectEndPoint");

      if (virtualObjectEndPoint.IsDataComplete)
        throw new InvalidOperationException ("The given end-point cannot be loaded, its data is already complete.");

      var domainObject = _objectLoader.LoadRelatedObject (virtualObjectEndPoint.ID, this);
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

      _objectLoader.LoadObject (objectID, this);

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

    public IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      return _relationEndPointManager.GetRelationEndPointWithMinimumLoading (endPointID);
    }

    private IRelationEndPoint EnsureEndPointReferencesNothing (IRelationEndPoint relationEndPoint)
    {
      Maybe.ForValue (relationEndPoint as IObjectEndPoint)
          .Where (endPoint => endPoint.OppositeObjectID != null)
          .Select (endPoint => String.Format ("End point '{0}' still references object '{1}'.", endPoint.ID, endPoint.OppositeObjectID))
          .Do (message => { throw new InvalidOperationException (message); });

      Maybe.ForValue (relationEndPoint as ICollectionEndPoint)
          .Where (endPoint => endPoint.Collection.Count != 0)
          .Select (
              endPoint => String.Format (
                  "End point '{0}' still references objects '{1}'.",
                  endPoint.ID,
                  SeparatedStringBuilder.Build (", ", endPoint.Collection, (DomainObject obj) => obj.ID.ToString())))
          .Do (message => { throw new InvalidOperationException (message); });

      return relationEndPoint;
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
            commands.Add (new UnregisterDataContainerCommand (objectID, _dataContainerMap));
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

    private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
    {
      return new ClientTransactionsDifferException (String.Format (message, args));
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
      _transactionEventSink = _clientTransaction.TransactionEventSink;
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