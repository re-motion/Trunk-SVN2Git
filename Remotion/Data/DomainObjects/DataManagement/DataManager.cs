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

  public void RegisterExistingDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterExistingDataContainer (dataContainer);
  }

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterNewDataContainer (dataContainer);
  }

  public void Commit ()
  {
    _relationEndPointMap.Commit (_dataContainerMap.GetByState (StateType.Deleted));
    _dataContainerMap.Commit ();
  }

  public void Rollback ()
  {
    _relationEndPointMap.Rollback (_dataContainerMap.GetByState (StateType.New));
    _dataContainerMap.Rollback ();
  }

  public DataContainerMap DataContainerMap
  {
    get { return _dataContainerMap; }
  }

  public RelationEndPointMap RelationEndPointMap
  {
    get { return _relationEndPointMap; }
  }

  public void Delete (DomainObject domainObject)
  {
    //TODO later: Start here when implementing oldRelatedObject and NewRelatedObject on IClientTransactionExtension.RelationChanged () 
    //      and RelationChanged events of ClientTransaction and DomainObject
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckClientTransactionForDeletion (domainObject);

    if (domainObject.TransactionContext[_clientTransaction].State == StateType.Deleted)
      return;

    NotifyingBidirectionalRelationModification oppositeEndPointModifications =
        _relationEndPointMap.GetOppositeEndPointModificationsForDelete (domainObject);

    BeginDelete (domainObject, oppositeEndPointModifications);
    PerformDelete (domainObject, oppositeEndPointModifications);
    EndDelete (domainObject, oppositeEndPointModifications);
  }

  internal void PerformDelete (DomainObject domainObject, NotifyingBidirectionalRelationModification oppositeEndPointModifications)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    ArgumentUtility.CheckNotNull ("oppositeEndPointModifications", oppositeEndPointModifications);

    DataContainer dataContainer = _clientTransaction.GetDataContainer(domainObject);  // rescue dataContainer before the map deletes is
    if (dataContainer.State == StateType.Deleted)
      return;

    _relationEndPointMap.PerformDelete (domainObject, oppositeEndPointModifications);
    _dataContainerMap.PerformDelete (dataContainer);
    
    dataContainer.Delete ();
  }

  private void BeginDelete (DomainObject domainObject, NotifyingBidirectionalRelationModification oppositeEndPointModifications)
  {
    _transactionEventSink.ObjectDeleting (domainObject);
    oppositeEndPointModifications.NotifyClientTransactionOfBegin();

    domainObject.OnDeleting (EventArgs.Empty);
    oppositeEndPointModifications.Begin();
  }

  private void EndDelete (DomainObject domainObject, NotifyingBidirectionalRelationModification oppositeEndPointModifications)
  {
    oppositeEndPointModifications.NotifyClientTransactionOfEnd();
    _transactionEventSink.ObjectDeleted (domainObject);

    oppositeEndPointModifications.End ();
    domainObject.OnDeleted (EventArgs.Empty);
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

  internal void CopyDiscardedDataContainers (DataManager source)
  {
    ArgumentUtility.CheckNotNull ("source", source);

    foreach (KeyValuePair<ObjectID, DataContainer> discardedItem in source._discardedDataContainers)
    {
      ObjectID discardedObjectID = discardedItem.Key;
      DataContainer discardedDataContainer = discardedItem.Value;
      CopyDiscardedDataContainer (discardedObjectID, discardedDataContainer);
    }
  }

  internal void CopyDiscardedDataContainer (ObjectID discardedObjectID, DataContainer discardedDataContainer)
  {
    ArgumentUtility.CheckNotNull ("discardedObjectID", discardedObjectID);

    ArgumentUtility.CheckNotNull ("discardedDataContainer", discardedDataContainer);
    DataContainer newDiscardedContainer = DataContainer.CreateNew (discardedObjectID);

    newDiscardedContainer.SetClientTransaction (_clientTransaction);
    newDiscardedContainer.SetDomainObject (discardedDataContainer.DomainObject);
    newDiscardedContainer.Delete ();

    Assertion.IsTrue (IsDiscarded (newDiscardedContainer.ID),
        "newDiscardedContainer.Delete must have inserted the DataContainer into the list of discarded objects");
    Assertion.IsTrue (GetDiscardedDataContainer (newDiscardedContainer.ID) == newDiscardedContainer);
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
