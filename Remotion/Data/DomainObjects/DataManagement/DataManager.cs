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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class DataManager : ISerializable, IDeserializationCallback, IDataManager
  {
    private ClientTransaction _clientTransaction;
    private IClientTransactionListener _transactionEventSink;

    private DataContainerMap _dataContainerMap;
    private RelationEndPointMap _relationEndPointMap;
    private DomainObjectStateCache _domainObjectStateCache;
    private Dictionary<ObjectID, DomainObject> _invalidObjects;

    private object[] _deserializedData; // only used for deserialization

    public DataManager (ClientTransaction clientTransaction, ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);

      _clientTransaction = clientTransaction;
      _transactionEventSink = clientTransaction.TransactionEventSink;
      _dataContainerMap = new DataContainerMap (clientTransaction);
      _relationEndPointMap = new RelationEndPointMap (clientTransaction, collectionEndPointChangeDetectionStrategy);
      _domainObjectStateCache = new DomainObjectStateCache (clientTransaction);
      _invalidObjects = new Dictionary<ObjectID, DomainObject>();
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public int InvalidObjectCount
    {
      get { return _invalidObjects.Count; }
    }

    public IEnumerable<ObjectID> InvalidObjectIDs
    {
      get { return _invalidObjects.Keys; }
    }

    public DomainObjectStateCache DomainObjectStateCache
    {
      get { return _domainObjectStateCache; }
    }

    public IDataContainerMapReadOnlyView DataContainerMap
    {
      get { return _dataContainerMap; }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPointMap
    {
      get { return _relationEndPointMap; }
    }

    public bool IsInvalid (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return _invalidObjects.ContainsKey (id);
    }

    public DomainObject GetInvalidObjectReference (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      DomainObject invalidDomainObject;
      if (!_invalidObjects.TryGetValue (id, out invalidDomainObject))
        throw new ArgumentException (String.Format ("The object '{0}' has not been marked invalid.", id), "id");
      else
        return invalidDomainObject;
    }

    public IEnumerable<Tuple<DomainObject, DataContainer>> GetLoadedData ()
    {
      return DataContainerMap.Select (dc => Tuple.Create (dc.DomainObject, dc));
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

    public IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ()
    {
      return _relationEndPointMap.Where (endPoint => endPoint.HasChanged);
    }

    public IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (DataContainer dataContainer)
    {
      return from endPointID in dataContainer.AssociatedRelationEndPointIDs
             let endPoint = RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID)
             let oppositeRelationEndPoints = endPoint.GetOppositeRelationEndPoints (this)
             from oppositeEndPoint in oppositeRelationEndPoints
             select oppositeEndPoint;
    }

    public bool HasRelationChanged (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      return dataContainer.AssociatedRelationEndPointIDs
          .Select (endPointID => _relationEndPointMap[endPointID])
          .Any (endPoint => endPoint != null && endPoint.HasChanged);
    }

    public void CheckMandatoryRelations (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (RelationEndPointID endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (endPointID.Definition.IsMandatory)
        {
          RelationEndPoint endPoint = _relationEndPointMap[endPointID];
          if (endPoint != null)
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
      _relationEndPointMap.RegisterEndPointsForDataContainer (dataContainer);
    }

    public void RegisterCollectionEndPoint (RelationEndPointID endPointID, IEnumerable<DomainObject> relatedObjects)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);

      if (endPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("EndPointID must specify a collection-valued end point.", "endPointID");

      if (_relationEndPointMap[endPointID] != null)
        throw new InvalidOperationException (string.Format ("An end point with ID '{0}' already exists in this transaction.", endPointID));

      _relationEndPointMap.RegisterCollectionEndPoint (endPointID, relatedObjects);
    }

    public void Commit ()
    {
      var deletedDataContainers = _dataContainerMap.Where (dc => dc.State == StateType.Deleted).ToList();

      _relationEndPointMap.CommitAllEndPoints();

      foreach (var deletedDataContainer in deletedDataContainers)
        Discard (deletedDataContainer);

      _dataContainerMap.CommitAllDataContainers();
    }

    public void Rollback ()
    {
      var newDataContainers = _dataContainerMap.Where (dc => dc.State == StateType.New).ToList();

      // roll back end point state before discarding data containers because Discard checks that no dangling end points are created
      _relationEndPointMap.RollbackAllEndPoints();

      // discard new data containers before rolling back data container state - new data containers cannot be rolled back
      foreach (var newDataContainer in newDataContainers)
        Discard (newDataContainer);

      _dataContainerMap.RollbackAllDataContainers();
    }

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      RelationEndPoint[] endPoints;
      try
      {
        endPoints = (from endPointID in dataContainer.AssociatedRelationEndPointIDs
                     let endPoint = _relationEndPointMap[endPointID]
                     where endPoint != null
                     where EnsureEndPointReferencesNothing (endPoint)
                     select endPoint).ToArray();
      }
      catch (InvalidOperationException ex)
      {
        var message = String.Format ("Cannot discard data container '{0}', it might leave dangling references: '{1}'", dataContainer.ID, ex.Message);
        throw new ArgumentException (message, "dataContainer", ex);
      }

      foreach (var endPoint in endPoints)
        _relationEndPointMap.RemoveEndPoint (endPoint.ID);

      _dataContainerMap.Remove (dataContainer.ID);

      dataContainer.Discard();

      var domainObject = dataContainer.DomainObject;
      MarkObjectInvalid (domainObject);
    }

    public void MarkObjectInvalid (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (IsInvalid (domainObject.ID))
      {
        if (GetInvalidObjectReference (domainObject.ID) != domainObject)
          throw new InvalidOperationException ("Cannot mark the given object invalid, another object with the same ID has already been marked.");

        return;
      }

      if (DataContainerMap[domainObject.ID] != null)
      {
        var message = String.Format (
            "Cannot mark object '{0}' as invalid; there is a DataContainer registered for that object. Discard the DataContainer instead.",
            domainObject.ID);
        throw new InvalidOperationException (message);
      }

      _transactionEventSink.DataManagerMarkingObjectInvalid (_clientTransaction, domainObject.ID);
      _invalidObjects.Add (domainObject.ID, domainObject);
    }

    public void ClearInvalidFlag (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _invalidObjects.Remove (objectID);
    }

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      ClientTransaction.EnsureDataAvailable (objectID);

      var dataContainer = GetDataContainerWithoutLoading (objectID);
      Assertion.IsNotNull (dataContainer);
      return dataContainer;
    }

    public DataContainer GetDataContainerWithoutLoading (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      if (IsInvalid (id))
        throw new ObjectInvalidException (id);

      return DataContainerMap[id];
    }

    private bool EnsureEndPointReferencesNothing (RelationEndPoint relationEndPoint)
    {
      Maybe.ForValue (relationEndPoint as IObjectEndPoint)
          .Where (endPoint => endPoint.OppositeObjectID != null)
          .Select (endPoint => String.Format ("End point '{0}' still references object '{1}'.", endPoint.ID, endPoint.OppositeObjectID))
          .Do (message => { throw new InvalidOperationException (message); });

      Maybe.ForValue (relationEndPoint as ICollectionEndPoint)
          .Where (endPoint => endPoint.OppositeDomainObjects.Count != 0)
          .Select (
              endPoint => String.Format (
                  "End point '{0}' still references objects '{1}'.",
                  endPoint.ID,
                  SeparatedStringBuilder.Build (", ", endPoint.OppositeDomainObjects, (DomainObject obj) => obj.ID.ToString())))
          .Do (message => { throw new InvalidOperationException (message); });

      return true;
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

      DomainObjectCheckUtility.CheckIfObjectIsInvalid (deletedObject, ClientTransaction);

      if (deletedObject.TransactionContext[_clientTransaction].State == StateType.Deleted)
        return new NopCommand();

      return new DeleteCommand (ClientTransaction, deletedObject);
    }

    public UnloadCommand CreateUnloadCommand (params ObjectID[] objectIDs)
    {
      return new UnloadCommand (objectIDs, ClientTransaction, _dataContainerMap, _relationEndPointMap);
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
      _relationEndPointMap = doInfo.GetValueForHandle<RelationEndPointMap>();
      _domainObjectStateCache = doInfo.GetValue<DomainObjectStateCache>();
      _invalidObjects = new Dictionary<ObjectID, DomainObject>();

      var invalidIDs = doInfo.GetArray<ObjectID>();
      var invalidObjects = doInfo.GetArray<DomainObject>();

      if (invalidIDs.Length != invalidObjects.Length)
        throw new SerializationException ("Invalid serialization data: invalid ID and object counts do not match.");

      for (int i = 0; i < invalidIDs.Length; ++i)
        _invalidObjects.Add (invalidIDs[i], invalidObjects[i]);

      _deserializedData = null;
      doInfo.SignalDeserializationFinished();
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      var doInfo = new FlattenedSerializationInfo();
      doInfo.AddHandle (_clientTransaction);
      doInfo.AddValue (_dataContainerMap);
      doInfo.AddHandle (_relationEndPointMap);
      doInfo.AddValue (_domainObjectStateCache);

      var invalidIDs = new ObjectID[_invalidObjects.Count];
      _invalidObjects.Keys.CopyTo (invalidIDs, 0);
      doInfo.AddArray (invalidIDs);

      var invalidObjects = new DomainObject[_invalidObjects.Count];
      _invalidObjects.Values.CopyTo (invalidObjects, 0);
      doInfo.AddArray (invalidObjects);

      info.AddValue ("doInfo.GetData", doInfo.GetData());
    }

    #endregion
  }
}