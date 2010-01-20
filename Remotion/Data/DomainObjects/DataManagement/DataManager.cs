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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataManager : ISerializable, IDeserializationCallback
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private IClientTransactionListener _transactionEventSink;

  private DataContainerMap _dataContainerMap;
  private RelationEndPointMap _relationEndPointMap;
  private Dictionary<ObjectID, DataContainer> _discardedDataContainers;
  
  private object[] _deserializedData; // only used for deserialization

  // construction and disposing

  public DataManager (ClientTransaction clientTransaction, ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);

    _clientTransaction = clientTransaction;
    _transactionEventSink = clientTransaction.TransactionEventSink;
    _dataContainerMap = new DataContainerMap (clientTransaction);
    _relationEndPointMap = new RelationEndPointMap (clientTransaction, collectionEndPointChangeDetectionStrategy);
    _discardedDataContainers = new Dictionary<ObjectID, DataContainer> ();
  }

  // methods and properties

  public ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  public int DiscardedObjectCount
  {
    get { return _discardedDataContainers.Count; }
  }

  public IEnumerable<ObjectID> DiscardedObjectIDs
  {
    get { return _discardedDataContainers.Keys; }
  }

  public bool IsDiscarded (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _discardedDataContainers.ContainsKey (id);
  }

  public DataContainer GetDiscardedDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer discardedDataContainer;
    if (!_discardedDataContainers.TryGetValue (id, out discardedDataContainer))
      throw new ArgumentException (String.Format ("The object '{0}' has not been discarded.", id), "id");
    else
      return discardedDataContainer;
  }

  public DataContainerCollection GetChangedDataContainersForCommit ()
  {
    var changedDataContainers = new DataContainerCollection ();
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      Assertion.IsTrue (domainObject.TransactionContext[_clientTransaction].State != StateType.NotLoadedYet);
      
      if (domainObject.TransactionContext[_clientTransaction].State != StateType.Deleted)
        _relationEndPointMap.CheckMandatoryRelations (domainObject);

      DataContainer dataContainer = _clientTransaction.GetDataContainer(domainObject);
      if (dataContainer.State != StateType.Unchanged)
        changedDataContainers.Add (dataContainer);
    }

    return changedDataContainers;
  }

  public DomainObjectCollection GetChangedDomainObjects ()
  {
    return GetDomainObjects (StateType.Changed, StateType.Deleted, StateType.New);
  }

  // TODO: This only returns domain objects for which a DataContainer exists, not unloaded or discarded objects. This should be reflected in the name.
  public DomainObjectCollection GetDomainObjects (params StateType[] states)
  {
    var stateSet = new StateValueSet (states);

    var matchingObjects = from dataContainer in DataContainerMap.Cast<DataContainer>()
                          let domainObject = dataContainer.DomainObject
                          let state = domainObject.TransactionContext[_clientTransaction].State
                          where stateSet.Matches (state)
                          select domainObject;
    return new DomainObjectCollection (matchingObjects, null);
  }

  public IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ()
  {
    foreach (RelationEndPoint endPoint in _relationEndPointMap)
    {
      if (endPoint.HasChanged)
        yield return endPoint;
    }
  }

  public void RegisterDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterEndPointsForDataContainer (dataContainer);
  }

  public void Unregister (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    
    Unregister (new[] { objectID });
  }

  public void Unregister (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    var loadedDataContainers = UnregisterAffectedDataFinder.GetAndCheckAffectedDataContainers (DataContainerMap, objectIDs);
    var endPointIDsToBeUnloaded = UnregisterAffectedDataFinder.GetAndCheckAffectedEndPointIDs (RelationEndPointMap, loadedDataContainers);

    UnregisterEndPoints (endPointIDsToBeUnloaded);
    UnregisterDataContainers(loadedDataContainers);
  }

  public void Commit ()
  {
    var deletedDataContainers = _dataContainerMap.GetByState (StateType.Deleted).ToList();

    _relationEndPointMap.CommitAllEndPoints ();
    
    foreach (var deletedDataContainer in deletedDataContainers)
      Discard (deletedDataContainer);

    _dataContainerMap.CommitAllDataContainers ();
  }

  public void Rollback ()
  {
    var newDataContainers = _dataContainerMap.GetByState (StateType.New).ToList();

    // roll back end point state before discarding objects because Discard checks that no dangling end points are created
    _relationEndPointMap.RollbackAllEndPoints ();

    // discard new data containers before rolling back data container state - new data containers cannot be rolled back
    foreach (var newDataContainer in newDataContainers)
      Discard (newDataContainer);

    _dataContainerMap.RollbackAllDataContainers ();
  }

  // This method might leave dangling end points, so it must only be used from scenarios where it is guaranteed that nothing points back to the
  // discarded object.
  private void Discard (DataContainer dataContainer)
  {
    foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
    {
      var endPoint = _relationEndPointMap[endPointID];
      if (endPoint != null)
      {
        Assertion.IsTrue (
            (endPoint.Definition.Cardinality == CardinalityType.One && ((ObjectEndPoint) endPoint).OppositeObjectID == null)
            || (endPoint.Definition.Cardinality == CardinalityType.Many && ((CollectionEndPoint) endPoint).OppositeDomainObjects.Count == 0),
            "Discard can only be used from scenarios where no dangling end points would be created.");
        _relationEndPointMap.RemoveEndPoint (endPointID);
      }
    }

    _dataContainerMap.Remove (dataContainer.ID);

    dataContainer.Discard ();
    MarkDiscarded (dataContainer);
  }

  public DataContainerMap DataContainerMap
  {
    get { return _dataContainerMap; }
  }

  public RelationEndPointMap RelationEndPointMap
  {
    get { return _relationEndPointMap; }
  }

  public void Delete (DomainObject deletedObject)
  {
    ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
    CheckClientTransactionForDeletion (deletedObject);

    DomainObjectCheckUtility.CheckIfObjectIsDiscarded (deletedObject, ClientTransaction);

    if (deletedObject.TransactionContext[_clientTransaction].State == StateType.Deleted)
      return;

    var oppositeEndPointRemoveModifications = _relationEndPointMap.GetRemoveModificationsForOppositeEndPoints (deletedObject);

    BeginDelete (deletedObject, oppositeEndPointRemoveModifications);
    PerformDelete (deletedObject, oppositeEndPointRemoveModifications);
    EndDelete (deletedObject, oppositeEndPointRemoveModifications);
  }

  // TODO: This will be rewritten as a command.
  internal void PerformDelete (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
  {
    ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
    ArgumentUtility.CheckNotNull ("oppositeEndPointRemoveModifications", oppositeEndPointRemoveModifications);

    var dataContainer = _clientTransaction.GetDataContainer (deletedObject);  // rescue dataContainer before the map deletes is
    Assertion.IsFalse (dataContainer.State == StateType.Deleted);
    Assertion.IsFalse (dataContainer.State == StateType.Discarded);

    _relationEndPointMap.PerformDelete (deletedObject, oppositeEndPointRemoveModifications);
    _dataContainerMap.PerformDelete (dataContainer);

    if (dataContainer.State == StateType.New)
    {
      dataContainer.Discard ();
      MarkDiscarded (dataContainer);
    }
    else
    {
      dataContainer.Delete();
    }
  }

  private void BeginDelete (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
  {
    _transactionEventSink.ObjectDeleting (deletedObject);
    oppositeEndPointRemoveModifications.NotifyClientTransactionOfBegin();

    deletedObject.OnDeleting (EventArgs.Empty);
    oppositeEndPointRemoveModifications.Begin();
  }

  private void EndDelete (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
  {
    oppositeEndPointRemoveModifications.NotifyClientTransactionOfEnd();
    _transactionEventSink.ObjectDeleted (deletedObject);

    oppositeEndPointRemoveModifications.End();
    deletedObject.OnDeleted (EventArgs.Empty);
  }

  private void CheckClientTransactionForDeletion (DomainObject domainObject)
  {
    if (!_clientTransaction.IsEnlisted (domainObject))
    {
      throw CreateClientTransactionsDifferException (
          "Cannot delete DomainObject '{0}', because it belongs to a different ClientTransaction.",
          domainObject.ID);
    }
  }

  private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
  {
    return new ClientTransactionsDifferException (String.Format (message, args));
  }

  private void MarkDiscarded (DataContainer discardedDataContainer)
  {
    _transactionEventSink.DataManagerMarkingObjectDiscarded (discardedDataContainer.ID);
    _discardedDataContainers.Add (discardedDataContainer.ID, discardedDataContainer);
  }

  private void UnregisterDataContainers (IEnumerable<DataContainer> dataContainers)
  {
    foreach (var dataContainer in dataContainers)
      _dataContainerMap.Remove (dataContainer.ID);
  }

  private void UnregisterEndPoints (IEnumerable<RelationEndPointID> unloadedEndPointIDs)
  {
    foreach (var unloadedEndPointID in unloadedEndPointIDs)
    {
      if (unloadedEndPointID.Definition.Cardinality == CardinalityType.One)
      {
        _relationEndPointMap.RemoveEndPoint (unloadedEndPointID);
      }
      else
      {
        var unloadedCollectionEndPoint = (CollectionEndPoint) _relationEndPointMap[unloadedEndPointID];
        unloadedCollectionEndPoint.Unload ();
      }
    }
  }

  #region Serialization
  protected DataManager (SerializationInfo info, StreamingContext context)
  {
    _deserializedData = (object[]) info.GetValue ("doInfo.GetData", typeof (object[]));
  }

  void IDeserializationCallback.OnDeserialization (object sender)
  {
    var doInfo = new FlattenedDeserializationInfo (_deserializedData);
    _clientTransaction = doInfo.GetValueForHandle<ClientTransaction> ();
    _transactionEventSink = _clientTransaction.TransactionEventSink;
    _dataContainerMap = doInfo.GetValue<DataContainerMap> ();
    _relationEndPointMap = doInfo.GetValueForHandle<RelationEndPointMap> ();
    _discardedDataContainers = new Dictionary<ObjectID, DataContainer> ();

    ObjectID[] discardedIDs = doInfo.GetArray<ObjectID> ();
    DataContainer[] discardedContainers = doInfo.GetArray<DataContainer> ();

    if (discardedIDs.Length != discardedContainers.Length)
      throw new SerializationException ("Invalid serilization data: discarded ID and data container counts do not match.");

    for (int i = 0; i < discardedIDs.Length; ++i)
      _discardedDataContainers.Add (discardedIDs[i], discardedContainers[i]);

    _deserializedData = null;
    doInfo.SignalDeserializationFinished ();
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    var doInfo = new FlattenedSerializationInfo();
    doInfo.AddHandle (_clientTransaction);
    doInfo.AddValue (_dataContainerMap);
    doInfo.AddHandle (_relationEndPointMap);

    var discardedIDs = new ObjectID[_discardedDataContainers.Count];
    _discardedDataContainers.Keys.CopyTo (discardedIDs, 0);
    doInfo.AddArray (discardedIDs);

    var discardedContainers = new DataContainer[_discardedDataContainers.Count];
    _discardedDataContainers.Values.CopyTo (discardedContainers, 0);
    doInfo.AddArray (discardedContainers);

    info.AddValue ("doInfo.GetData", doInfo.GetData());
  }
  #endregion
}
}
