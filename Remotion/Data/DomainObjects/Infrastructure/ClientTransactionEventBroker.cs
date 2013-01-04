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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Manages the <see cref="IClientTransactionListener"/> instances attached to a <see cref="DomainObjects.ClientTransaction"/> instance and
  /// allows clients to raise events for the <see cref="ClientTransaction"/>. This class delegates the actual event distribution to an implementation 
  /// of <see cref="IClientTransactionEventDistributor"/>.
  /// </summary>
  [Serializable]
  public class ClientTransactionEventBroker : IClientTransactionEventBroker
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionEventDistributor _eventDistributor;

    public ClientTransactionEventBroker (ClientTransaction clientTransaction, IClientTransactionEventDistributor eventDistributor)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("eventDistributor", eventDistributor);

      _clientTransaction = clientTransaction;
      _eventDistributor = eventDistributor;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionEventDistributor EventDistributor
    {
      get { return _eventDistributor; }
    }

    public IEnumerable<IClientTransactionListener> Listeners
    {
      get { return _eventDistributor.Listeners; }
    }

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);
      _eventDistributor.AddListener (listener);
    }

    public void RemoveListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);
      _eventDistributor.RemoveListener (listener);
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _eventDistributor.Extensions; }
    }

    public void RaiseTransactionInitializeEvent ()
    {
      _eventDistributor.TransactionInitialize (_clientTransaction);
    }

    public void RaiseTransactionDiscardEvent ()
    {
      _eventDistributor.TransactionDiscard (_clientTransaction);
    }

    public void RaiseSubTransactionCreatingEvent ()
    {
      _eventDistributor.SubTransactionCreating (_clientTransaction);
    }

    public void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction)
    {
      _eventDistributor.SubTransactionInitialize (_clientTransaction, subTransaction);
    }

    public void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction)
    {
      _eventDistributor.SubTransactionCreated (_clientTransaction, subTransaction);
    }

    public void RaiseNewObjectCreatingEvent (Type type)
    {
      _eventDistributor.NewObjectCreating (_clientTransaction, type);
    }

    public void RaiseObjectsLoadingEvent (ReadOnlyCollection<ObjectID> objectIDs)
    {
      _eventDistributor.ObjectsLoading (_clientTransaction, objectIDs);
    }

    public void RaiseObjectsLoadedEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _eventDistributor.ObjectsLoaded (_clientTransaction, domainObjects);
    }

    public void RaiseObjectsNotFoundEvent (ReadOnlyCollection<ObjectID> objectIDs)
    {
      _eventDistributor.ObjectsNotFound (_clientTransaction, objectIDs);
    }

    public void RaiseObjectsUnloadingEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _eventDistributor.ObjectsUnloading (_clientTransaction, unloadedDomainObjects);
    }

    public void RaiseObjectsUnloadedEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _eventDistributor.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects);
    }

    public void RaiseObjectDeletingEvent (DomainObject domainObject)
    {
      _eventDistributor.ObjectDeleting (_clientTransaction, domainObject);
    }

    public void RaiseObjectDeletedEvent (DomainObject domainObject)
    {
      _eventDistributor.ObjectDeleted (_clientTransaction, domainObject);
    }

    public void RaisePropertyValueReadingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      _eventDistributor.PropertyValueReading (_clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public void RaisePropertyValueReadEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      _eventDistributor.PropertyValueRead (_clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public void RaisePropertyValueChangingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _eventDistributor.PropertyValueChanging (_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public void RaisePropertyValueChangedEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _eventDistributor.PropertyValueChanged (_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public void RaiseRelationReadingEvent (DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      _eventDistributor.RelationReading (_clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public void RaiseRelationReadEvent (
        DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      _eventDistributor.RelationRead (_clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public void RaiseRelationReadEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      _eventDistributor.RelationRead (_clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public void RaiseRelationChangingEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      _eventDistributor.RelationChanging (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public void RaiseRelationChangedEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      _eventDistributor.RelationChanged (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      return _eventDistributor.FilterQueryResult (_clientTransaction, queryResult);
    }

    public IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results)
    {
      return _eventDistributor.FilterCustomQueryResult (_clientTransaction, query, results);
    }

    public void RaiseTransactionCommittingEvent (ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      _eventDistributor.TransactionCommitting (_clientTransaction, domainObjects, eventRegistrar);
    }

    public void RaiseTransactionCommitValidateEvent (ReadOnlyCollection<PersistableData> committedData)
    {
      _eventDistributor.TransactionCommitValidate (_clientTransaction, committedData);
    }

    public void RaiseTransactionCommittedEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _eventDistributor.TransactionCommitted (_clientTransaction, domainObjects);
    }

    public void RaiseTransactionRollingBackEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _eventDistributor.TransactionRollingBack (_clientTransaction, domainObjects);
    }

    public void RaiseTransactionRolledBackEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _eventDistributor.TransactionRolledBack (_clientTransaction, domainObjects);
    }

    public void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint)
    {
      _eventDistributor.RelationEndPointMapRegistering (_clientTransaction, endPoint);
    }

    public void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID)
    {
      _eventDistributor.RelationEndPointMapUnregistering (_clientTransaction, endPointID);
    }

    public void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID)
    {
      _eventDistributor.RelationEndPointBecomingIncomplete (_clientTransaction, endPointID);
    }

    public void RaiseObjectMarkedInvalidEvent (DomainObject domainObject)
    {
      _eventDistributor.ObjectMarkedInvalid (_clientTransaction, domainObject);
    }

    public void RaiseObjectMarkedNotInvalidEvent (DomainObject domainObject)
    {
      _eventDistributor.ObjectMarkedNotInvalid (_clientTransaction, domainObject);
    }

    public void RaiseDataContainerMapRegisteringEvent (DataContainer container)
    {
      _eventDistributor.DataContainerMapRegistering (_clientTransaction, container);
    }

    public void RaiseDataContainerMapUnregisteringEvent (DataContainer container)
    {
      _eventDistributor.DataContainerMapUnregistering (_clientTransaction, container);
    }

    public void RaiseDataContainerStateUpdatedEvent (DataContainer container, StateType newDataContainerState)
    {
      _eventDistributor.DataContainerStateUpdated (_clientTransaction, container, newDataContainerState);
    }

    public void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      _eventDistributor.VirtualRelationEndPointStateUpdated (_clientTransaction, endPointID, newEndPointChangeState);
    }
  }
}