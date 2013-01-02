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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class ClientTransactionEventSinkWithMock : IClientTransactionEventSink
  {
    public static ClientTransactionEventSinkWithMock CreateWithStrictMock (
        ClientTransaction clientTransaction = null, MockRepository mockRepository = null)
    {
      var mock = mockRepository != null
                     ? mockRepository.StrictMock<IClientTransactionListener>()
                     : MockRepository.GenerateStrictMock<IClientTransactionListener>();
      return new ClientTransactionEventSinkWithMock (mock, clientTransaction);
    }

    public static ClientTransactionEventSinkWithMock CreateWithDynamicMock (
        ClientTransaction clientTransaction = null, MockRepository mockRepository = null)
    {
      var mock = mockRepository != null
                     ? mockRepository.DynamicMock<IClientTransactionListener>()
                     : MockRepository.GenerateMock<IClientTransactionListener>();
      return new ClientTransactionEventSinkWithMock (mock, clientTransaction);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _mock;

    public ClientTransactionEventSinkWithMock (IClientTransactionListener mock, ClientTransaction clientTransaction = null)
    {
      ArgumentUtility.CheckNotNull ("mock", mock);

      _clientTransaction = clientTransaction ?? ClientTransactionObjectMother.Create();
      _mock = mock;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionListener Mock
    {
      get { return _mock; }
    }

    public void RaiseRelationChangingEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      _mock.RelationChanging (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public void RaiseRelationChangedEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      _mock.RelationChanged (_clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public void RaiseObjectDeletingEvent (DomainObject domainObject)
    {
      _mock.ObjectDeleting (_clientTransaction, domainObject);
    }

    public void RaiseObjectDeletedEvent (DomainObject domainObject)
    {
      _mock.ObjectDeleted (_clientTransaction, domainObject);
    }

    public void RaiseObjectsUnloadingEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _mock.ObjectsUnloading (_clientTransaction, unloadedDomainObjects);
    }

    public void RaiseObjectsUnloadedEvent (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _mock.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects);
    }

    public void RaiseRelationEndPointBecomingIncompleteEvent (RelationEndPointID endPointID)
    {
      _mock.RelationEndPointBecomingIncomplete (_clientTransaction, endPointID);
    }

    public void RaiseRelationEndPointMapRegisteringEvent (IRelationEndPoint endPoint)
    {
      _mock.RelationEndPointMapRegistering (_clientTransaction, endPoint);
    }

    public void RaiseRelationEndPointMapUnregisteringEvent (RelationEndPointID endPointID)
    {
      _mock.RelationEndPointMapUnregistering (_clientTransaction, endPointID);
    }

    public void RaiseVirtualRelationEndPointStateUpdatedEvent (RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      _mock.VirtualRelationEndPointStateUpdated (_clientTransaction, endPointID, newEndPointChangeState);
    }

    public void RaisePropertyValueReadingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      _mock.PropertyValueReading (_clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public void RaisePropertyValueReadEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      _mock.PropertyValueRead (_clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public void RaisePropertyValueChangingEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _mock.PropertyValueChanging (_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public void RaisePropertyValueChangedEvent (DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _mock.PropertyValueChanged (_clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public void RaiseDataContainerStateUpdatedEvent (DataContainer container, StateType newDataContainerState)
    {
      _mock.DataContainerStateUpdated (_clientTransaction, container, newDataContainerState);
    }

    public void RaiseDataContainerMapRegisteringEvent (DataContainer container)
    {
      _mock.DataContainerMapRegistering (_clientTransaction, container);
    }

    public void RaiseDataContainerMapUnregisteringEvent (DataContainer container)
    {
      _mock.DataContainerMapUnregistering (_clientTransaction, container);
    }

    public void RaiseSubTransactionCreatingEvent ()
    {
      _mock.SubTransactionCreating (_clientTransaction);
    }

    public void RaiseSubTransactionInitializeEvent (ClientTransaction subTransaction)
    {
      _mock.SubTransactionInitialize (_clientTransaction, subTransaction);
    }

    public void RaiseSubTransactionCreatedEvent (ClientTransaction subTransaction)
    {
      _mock.SubTransactionCreated (_clientTransaction, subTransaction);
    }

    public void RaiseObjectMarkedInvalidEvent (DomainObject domainObject)
    {
      _mock.ObjectMarkedInvalid (_clientTransaction, domainObject);
    }

    public void RaiseObjectMarkedNotInvalidEvent (DomainObject domainObject)
    {
      _mock.ObjectMarkedNotInvalid (_clientTransaction, domainObject);
    }

    public void RaiseNewObjectCreatingEvent (Type type)
    {
      _mock.NewObjectCreating (_clientTransaction, type);
    }

    public void RaiseObjectsLoadingEvent (ReadOnlyCollection<ObjectID> objectIDs)
    {
      _mock.ObjectsLoading (_clientTransaction, objectIDs);
    }

    public void RaiseObjectsLoadedEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _mock.ObjectsLoaded (_clientTransaction, domainObjects);
    }

    public void RaiseObjectsNotFoundEvent (ReadOnlyCollection<ObjectID> objectIDs)
    {
      _mock.ObjectsNotFound (_clientTransaction, objectIDs);
    }

    public void RaiseTransactionCommittingEvent (ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      _mock.TransactionCommitting (_clientTransaction, domainObjects, eventRegistrar);
    }

    public void RaiseTransactionCommitValidateEvent (ReadOnlyCollection<PersistableData> committedData)
    {
      _mock.TransactionCommitValidate (_clientTransaction, committedData);
    }

    public void RaiseTransactionCommittedEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _mock.TransactionCommitted (_clientTransaction, domainObjects);
    }

    public void RaiseTransactionRollingBackEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _mock.TransactionRollingBack (_clientTransaction, domainObjects);
    }

    public void RaiseTransactionRolledBackEvent (ReadOnlyCollection<DomainObject> domainObjects)
    {
      _mock.TransactionRolledBack (_clientTransaction, domainObjects);
    }

    public QueryResult<T> RaiseFilterQueryResultEvent<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      return _mock.FilterQueryResult (_clientTransaction, queryResult);
    }

    public IEnumerable<T> RaiseFilterCustomQueryResultEvent<T> (IQuery query, IEnumerable<T> results)
    {
      return _mock.FilterCustomQueryResult (_clientTransaction, query, results);
    }

    public void RaiseTransactionInitializeEvent ()
    {
      _mock.TransactionInitialize (_clientTransaction);
    }

    public void RaiseTransactionDiscardEvent ()
    {
      _mock.TransactionDiscard (_clientTransaction);
    }

    public void RaiseRelationReadingEvent (DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      _mock.RelationReading (_clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public void RaiseRelationReadEvent (
        DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      _mock.RelationRead (_clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public void RaiseRelationReadEvent (
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      _mock.RelationRead (_clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public void ReplayMock ()
    {
      _mock.Replay();
    }

    public void VerifyMock ()
    {
      _mock.VerifyAllExpectations();
    }

    public void BackToRecordMock ()
    {
      _mock.BackToRecord();
    }

    public MockRepository GetMockRepository ()
    {
      return _mock.GetMockRepository();
    }

    public IMethodOptions<R> ExpectMock<R> (Function<IClientTransactionListener, R> func)
    {
      return _mock.Expect (func);
    }

    public IMethodOptions<RhinoMocksExtensions.VoidType> ExpectMock (Action<IClientTransactionListener> action)
    {
      return _mock.Expect (action);
    }

    public IMethodOptions<R> StubMock<R> (Function<IClientTransactionListener, R> func)
    {
      return _mock.Stub (func);
    }

    public IMethodOptions<object> StubMock (Action<IClientTransactionListener> action)
    {
      return _mock.Stub (action);
    }

    public void AssertWasCalledMock (Func<IClientTransactionListener, object> action)
    {
      _mock.AssertWasCalled (action);
    }

    public void AssertWasCalledMock (Action<IClientTransactionListener> action)
    {
      _mock.AssertWasCalled (action);
    }

    public void AssertWasNotCalledMock (Func<IClientTransactionListener, object> action)
    {
      _mock.AssertWasNotCalled (action);
    }

    public void AssertWasNotCalledMock (Action<IClientTransactionListener> action)
    {
      _mock.AssertWasNotCalled (action);
    }
  }
}