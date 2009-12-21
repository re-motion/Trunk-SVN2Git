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

  public DataContainerCollection GetChangedDataContainersForCommit ()
  {
    var changedDataContainers = new DataContainerCollection ();
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
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
    return GetDomainObjects (new[] { StateType.Changed, StateType.Deleted, StateType.New });
  }

  public DomainObjectCollection GetDomainObjects (StateType stateType)
  {
    ArgumentUtility.CheckValidEnumValue ("stateType", stateType);

    return GetDomainObjects (new[] { stateType });
  }

  public DomainObjectCollection GetDomainObjects (StateType[] states)
  {
    var domainObjects = new DomainObjectCollection ();

    bool includeChanged = ContainsState (states, StateType.Changed);
    bool includeDeleted = ContainsState (states, StateType.Deleted);
    bool includeNew = ContainsState (states, StateType.New);
    bool includeUnchanged = ContainsState (states, StateType.Unchanged);

    foreach (DataContainer dataContainer in _dataContainerMap)
    {
      StateType domainObjectState = dataContainer.DomainObject.TransactionContext[_clientTransaction].State;
      if (includeChanged && domainObjectState == StateType.Changed)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeDeleted && domainObjectState == StateType.Deleted)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeNew && domainObjectState == StateType.New)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeUnchanged && domainObjectState == StateType.Unchanged)
        domainObjects.Add (dataContainer.DomainObject);
    }

    return domainObjects;
  }

  public IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ()
  {
    foreach (RelationEndPoint endPoint in _relationEndPointMap)
    {
      if (endPoint.HasChanged)
        yield return endPoint;
    }
  }

  private bool ContainsState (StateType[] states, StateType state)
  {
    return (Array.IndexOf (states, state) >= 0);
  }

  public void RegisterDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterEndPointsForDataContainer (dataContainer);
  }

  public void Commit ()
  {
    var deletedObjects = _dataContainerMap.GetByState (StateType.Deleted).ToList();
    foreach (var deletedObject in deletedObjects)
      Discard (deletedObject);

    _relationEndPointMap.Commit ();
    _dataContainerMap.Commit ();
  }

  public void Rollback ()
  {
    var newObjects = _dataContainerMap.GetByState (StateType.New).ToList();
    foreach (var newObject in newObjects)
      Discard (newObject);

    _relationEndPointMap.Rollback ();
    _dataContainerMap.Rollback ();
  }

  private void Discard (DataContainer dataContainer)
  {
    foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
    {
      if (_relationEndPointMap[endPointID] != null)
        _relationEndPointMap.Discard (endPointID);
    }

    _dataContainerMap.Discard (dataContainer.ID);
    MarkDiscarded (dataContainer);
  }

  public void Commit2 ()
  {
    _relationEndPointMap.Commit2 (_dataContainerMap.GetByState2 (StateType.Deleted));
    _dataContainerMap.Commit2 ();
  }

  public void Rollback2 ()
  {
    _relationEndPointMap.Rollback2 (_dataContainerMap.GetByState2 (StateType.New));
    _dataContainerMap.Rollback2 ();
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

    // TODO: The next line implicitly loads the data container of the object before deleting it. This is necessary, but too implicit.
    // Explicitly ensure that the object has actually been loaded.
    if (deletedObject.TransactionContext[_clientTransaction].State == StateType.Deleted) // TODO 1914: | Discarded?
      return;

    var oppositeEndPointRemoveModifications = _relationEndPointMap.GetRemoveModificationsForOppositeEndPoints (deletedObject);

    BeginDelete (deletedObject, oppositeEndPointRemoveModifications);
    PerformDelete2 (deletedObject, oppositeEndPointRemoveModifications);
    EndDelete (deletedObject, oppositeEndPointRemoveModifications);
  }

  internal void PerformDelete2 (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
  {
    ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
    ArgumentUtility.CheckNotNull ("oppositeEndPointRemoveModifications", oppositeEndPointRemoveModifications);

    var dataContainer = _clientTransaction.GetDataContainer (deletedObject);  // rescue dataContainer before the map deletes is
    Assertion.IsFalse (dataContainer.State == StateType.Deleted);

    _relationEndPointMap.PerformDelete2 (deletedObject, oppositeEndPointRemoveModifications);
    _dataContainerMap.PerformDelete2 (dataContainer);
    
    dataContainer.Delete2 ();
  }

  internal void PerformDelete (DomainObject deletedObject, CompositeRelationModificationWithEvents oppositeEndPointRemoveModifications)
  {
    ArgumentUtility.CheckNotNull ("deletedObject", deletedObject);
    ArgumentUtility.CheckNotNull ("oppositeEndPointRemoveModifications", oppositeEndPointRemoveModifications);

    var dataContainer = _clientTransaction.GetDataContainer (deletedObject);  // rescue dataContainer before the map deletes is
    Assertion.IsFalse (dataContainer.State == StateType.Deleted);

    _relationEndPointMap.PerformDelete2 (deletedObject, oppositeEndPointRemoveModifications);
    _dataContainerMap.PerformDelete2 (dataContainer);

    dataContainer.Delete2 ();
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
    if (!domainObject.TransactionContext[_clientTransaction].CanBeUsedInTransaction)
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

  public void MarkDiscarded (DataContainer discardedDataContainer)
  {
    ArgumentUtility.CheckNotNull ("discardedDataContainer", discardedDataContainer);
    
    _transactionEventSink.DataManagerMarkingObjectDiscarded (discardedDataContainer.ID);
    _discardedDataContainers.Add (discardedDataContainer.ID, discardedDataContainer);
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

  public int DiscardedObjectCount
  {
    get { return _discardedDataContainers.Count; }
  }

  public IEnumerable<ObjectID> DiscardedObjectIDs
  {
    get { return _discardedDataContainers.Keys; }
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
