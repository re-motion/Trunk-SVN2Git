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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ClientTransactionEventBrokerTest : ClientTransactionBaseTest
  {
    private ClientTransaction _clientTransaction;
    private IClientTransactionEventDistributor _eventDistributor;

    private ClientTransactionEventBroker _eventBroker;

    private IClientTransactionListener _fakeListener;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _eventDistributor = MockRepository.GenerateStrictMock<IClientTransactionEventDistributor>();

      _eventBroker = new ClientTransactionEventBroker (_clientTransaction, _eventDistributor);

      _fakeListener = MockRepository.GenerateStub<IClientTransactionListener>();
    }

    public override void TearDown ()
    {
      _clientTransaction.Discard();

      base.TearDown ();
    }

    [Test]
    public void Listeners ()
    {
      _eventDistributor.Stub (stub => stub.Listeners).Return (new[] { _fakeListener });

      Assert.That (_eventBroker.Listeners, Is.EqualTo (new[] { _fakeListener }));
    }

    [Test]
    public void AddListener()
    {
      _eventDistributor.Expect (mock => mock.AddListener (_fakeListener));
      _eventDistributor.Replay();
      
      _eventBroker.AddListener (_fakeListener);

      _eventDistributor.VerifyAllExpectations();
    }

    [Test]
    public void RemoveListener ()
    {
      _eventDistributor.Expect (mock => mock.RemoveListener (_fakeListener));
      _eventDistributor.Replay ();

      _eventBroker.RemoveListener (_fakeListener);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionInitializeEvent ()
    {
      _eventDistributor.Expect (
          mock =>
          mock.TransactionInitialize (
              _clientTransaction));

      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionInitializeEvent ();

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionDiscardEvent ()
    {
      _eventDistributor.Expect (
          mock =>
          mock.TransactionDiscard (
              _clientTransaction));

      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionDiscardEvent ();

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseSubTransactionCreatingEvent ()
    {
      _eventDistributor.Expect (
          mock =>
          mock.SubTransactionCreating (
              _clientTransaction));
      _eventDistributor.Replay ();

      _eventBroker.RaiseSubTransactionCreatingEvent ();

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseSubTransactionInitializeEvent ()
    {
      var testableClientTransaction = new TestableClientTransaction ();

      _eventDistributor.Expect (
          mock =>
          mock.SubTransactionInitialize (
              _clientTransaction,
              testableClientTransaction));
      _eventDistributor.Replay ();

      _eventBroker.RaiseSubTransactionInitializeEvent (testableClientTransaction);
      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseSubTransactionCreatedEvent ()
    {
      var testableClientTransaction = new TestableClientTransaction ();

      _eventDistributor.Expect (
          mock =>
          mock.SubTransactionCreated (
              _clientTransaction,
              testableClientTransaction));
      _eventDistributor.Replay ();

      _eventBroker.RaiseSubTransactionCreatedEvent (testableClientTransaction);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseNewObjectCreatingEvent ()
    {
      var type = GetType ();

      _eventDistributor.Expect (
          mock =>
          mock.NewObjectCreating (
              _clientTransaction,
              type));
      _eventDistributor.Replay ();

      _eventBroker.RaiseNewObjectCreatingEvent (type);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectsLoadingEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<ObjectID> (new List<ObjectID> ());

      _eventDistributor.Expect (
          mock =>
          mock.ObjectsLoading (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectsLoadingEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectsLoadedEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());

      _eventDistributor.Expect (
          mock =>
          mock.ObjectsLoaded (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectsLoadedEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectsNotFoundEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<ObjectID> (new List<ObjectID> ());

      _eventDistributor.Expect (
          mock =>
          mock.ObjectsNotFound (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectsNotFoundEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectsUnloadingEvent ()
    {
      var domainObjects = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());

      _eventDistributor.Expect (
          mock =>
          mock.ObjectsUnloading (
              _clientTransaction, domainObjects));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectsUnloadingEvent (domainObjects);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectsUnloadedEvent ()
    {
      var domainObjects = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());

      _eventDistributor.Expect (
          mock =>
          mock.ObjectsUnloaded (
              _clientTransaction,
              domainObjects));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectsUnloadedEvent (domainObjects);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectDeletingEvent ()
    {
      var domainObject = Order.NewObject ();

      _eventDistributor.Expect (
          mock =>
          mock.ObjectDeleting (
              _clientTransaction,
              domainObject));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectDeletingEvent (domainObject);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectDeletedEvent ()
    {
      var domainObject = Order.NewObject ();

      _eventDistributor.Expect (
          mock =>
          mock.ObjectDeleted (
              _clientTransaction,
              domainObject));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectDeletedEvent (domainObject);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaisePropertyValueReadingEvent ()
    {
      var domainObject = Order.NewObject ();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      var valueAccess = ValueAccess.Original;

      _eventDistributor.Expect (
          mock =>
          mock.PropertyValueReading (
              _clientTransaction,
              domainObject,
              propertyDefinition,
              valueAccess));
      _eventDistributor.Replay ();

      _eventBroker.RaisePropertyValueReadingEvent (domainObject, propertyDefinition, valueAccess);
      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaisePropertyValueReadEvent ()
    {
      var domainObject = Order.NewObject ();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      var value = new object ();
      var valueAccess = ValueAccess.Original;


      _eventDistributor.Expect (
          mock =>
          mock.PropertyValueRead (
              _clientTransaction, domainObject, propertyDefinition, value, valueAccess));
      _eventDistributor.Replay ();

      _eventBroker.RaisePropertyValueReadEvent (domainObject, propertyDefinition, value, valueAccess);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaisePropertyValueChangingEvent ()
    {
      var domainObject = Order.NewObject ();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      var newValue = new object ();
      var oldValue = new object ();

      _eventDistributor.Expect (
          mock =>
          mock.PropertyValueChanging (
              _clientTransaction,
              domainObject,
              propertyDefinition,
              oldValue,
              newValue));
      _eventDistributor.Replay ();

      _eventBroker.RaisePropertyValueChangingEvent (domainObject, propertyDefinition, oldValue, newValue);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaisePropertyValueChangedEvent ()
    {
      var domainObject = Order.NewObject ();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      var newValue = new object ();
      var oldValue = new object ();

      _eventDistributor.Expect (
          mock =>
          mock.PropertyValueChanged (
              _clientTransaction,
              domainObject,
              propertyDefinition,
              oldValue,
              newValue));
      _eventDistributor.Replay ();

      _eventBroker.RaisePropertyValueChangedEvent (domainObject, propertyDefinition, oldValue, newValue);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationReadingEvent ()
    {
      var domainObject = Order.NewObject ();
      var valueAccess = ValueAccess.Original;
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition> ();

      _eventDistributor.Expect (
          mock =>
          mock.RelationReading (
              _clientTransaction,
              domainObject,
              relationEndPointDefinition,
              valueAccess));

      _eventDistributor.Replay ();
      _eventBroker.RaiseRelationReadingEvent (domainObject, relationEndPointDefinition, valueAccess);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationReadEvent ()
    {
      var domainObject = Order.NewObject ();
      var relatedObject = Order.NewObject ();
      var valueAccess = ValueAccess.Original;
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition> ();

      _eventDistributor.Expect (
          mock =>
          mock.RelationRead (
              _clientTransaction,
              domainObject,
              relationEndPointDefinition,
              relatedObject,
              valueAccess));

      _eventDistributor.Replay ();
      _eventBroker.RaiseRelationReadEvent (domainObject, relationEndPointDefinition, relatedObject, valueAccess);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationReadEvent_WithRelatedObjectsCollection ()
    {
      var domainObject = Order.NewObject ();
      var valueAccess = ValueAccess.Original;
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition> ();
      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection ());

      _eventDistributor.Expect (
          mock =>
          mock.RelationRead (
              _clientTransaction,
              domainObject,
              relationEndPointDefinition,
              relatedObjects,
              valueAccess));

      _eventDistributor.Replay ();
      _eventBroker.RaiseRelationReadEvent (domainObject, relationEndPointDefinition, relatedObjects, valueAccess);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationChangingEvent ()
    {
      var relatedObject = Order.NewObject();
      var oldRelatedObject = Order.NewObject();
      var newRelatedObject = Order.NewObject();
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();

      _eventDistributor.Expect (
          mock =>
          mock.RelationChanging (
              _clientTransaction,
              relatedObject,
              relationEndPointDefinition,
              oldRelatedObject,
              newRelatedObject));
      _eventDistributor.Replay();

      _eventBroker.RaiseRelationChangingEvent (relatedObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);

      _eventDistributor.VerifyAllExpectations();
    }

    [Test]
    public void RaiseRelationChangedEvent ()
    {
      var relatedObject = Order.NewObject();
      var oldRelatedObject = Order.NewObject();
      var newRelatedObject = Order.NewObject();
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();

      _eventDistributor.Expect (
          mock =>
          mock.RelationChanged (
              _clientTransaction,
              relatedObject,
              relationEndPointDefinition,
              oldRelatedObject,
              newRelatedObject));
      _eventDistributor.Replay();

      _eventBroker.RaiseRelationChangedEvent (relatedObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);

      _eventDistributor.VerifyAllExpectations();
    }

    [Test]
    public void RaiseFilterQueryResultEvent ()
    {
      var queryResult = new QueryResult<Order> (MockRepository.GenerateStub<IQuery> (), new Order[0]);
      var expected = new QueryResult<Order> (MockRepository.GenerateStub<IQuery> (), new Order[0]);

      _eventDistributor.Expect (
          mock =>
          mock.FilterQueryResult (
              _clientTransaction,
              queryResult))
          .Return (expected);

      _eventDistributor.Replay ();

      var actual = _eventBroker.RaiseFilterQueryResultEvent (queryResult);

      _eventDistributor.VerifyAllExpectations ();

      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void RaiseFilterCustomQueryResultEvent ()
    {
      var query = MockRepository.GenerateStub<IQuery> ();
      var results = new List<Order> ();
      var expected = new List<Order> ();

      _eventDistributor.Expect (
          mock =>
          mock.FilterCustomQueryResult (
              _clientTransaction,
              query, results))
          .Return (expected);

      _eventDistributor.Replay ();

      var actual = _eventBroker.RaiseFilterCustomQueryResultEvent (query, results);

      _eventDistributor.VerifyAllExpectations ();

      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void RaiseTransactionCommittingEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());
      var committingEventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar> ();

      _eventDistributor.Expect (
          mock =>
          mock.TransactionCommitting (
              _clientTransaction,
              readOnlyCollection,
              committingEventRegistrar));
      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionCommittingEvent (readOnlyCollection, committingEventRegistrar);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionCommitValidateEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<PersistableData> (new List<PersistableData> ());

      _eventDistributor.Expect (
          mock =>
          mock.TransactionCommitValidate (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionCommitValidateEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionCommittedEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());

      _eventDistributor.Expect (
          mock =>
          mock.TransactionCommitted (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionCommittedEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionRollingBackEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());


      _eventDistributor.Expect (
          mock =>
          mock.TransactionRollingBack (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionRollingBackEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseTransactionRolledBackEvent ()
    {
      var readOnlyCollection = new ReadOnlyCollection<DomainObject> (new List<DomainObject> ());

      _eventDistributor.Expect (
          mock =>
          mock.TransactionRolledBack (
              _clientTransaction,
              readOnlyCollection));
      _eventDistributor.Replay ();

      _eventBroker.RaiseTransactionRolledBackEvent (readOnlyCollection);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationEndPointMapRegisteringEvent ()
    {
      var relationEndPoint = MockRepository.GenerateStub<IRelationEndPoint> ();

      _eventDistributor.Expect (
          mock =>
          mock.RelationEndPointMapRegistering (
              _clientTransaction,
              relationEndPoint));
      _eventDistributor.Replay ();

      _eventBroker.RaiseRelationEndPointMapRegisteringEvent (relationEndPoint);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationEndPointMapUnregisteringEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));

      _eventDistributor.Expect (
          mock =>
          mock.RelationEndPointMapUnregistering (
              _clientTransaction,
              endPointID));
      _eventDistributor.Replay ();

      _eventBroker.RaiseRelationEndPointMapUnregisteringEvent (endPointID);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseRelationEndPointBecomingIncompleteEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));

      _eventDistributor.Expect (
          mock =>
          mock.RelationEndPointBecomingIncomplete (
              _clientTransaction,
              endPointID));
      _eventDistributor.Replay ();

      _eventBroker.RaiseRelationEndPointBecomingIncompleteEvent (endPointID);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectMarkedInvalidEvent ()
    {
      var domainObject = Order.NewObject ();

      _eventDistributor.Expect (
          mock =>
          mock.ObjectMarkedInvalid (
              _clientTransaction,
              domainObject));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectMarkedInvalidEvent (domainObject);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseObjectMarkedNotInvalidEvent ()
    {
      var relatedObject = Order.NewObject ();

      _eventDistributor.Expect (
          mock =>
          mock.ObjectMarkedNotInvalid (
              _clientTransaction,
              relatedObject));
      _eventDistributor.Replay ();

      _eventBroker.RaiseObjectMarkedNotInvalidEvent (relatedObject);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseDataContainerMapRegisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject ());

      _eventDistributor.Expect (
          mock =>
          mock.DataContainerMapRegistering (
              _clientTransaction,
              dataContainer
              ));
      _eventDistributor.Replay ();

      _eventBroker.RaiseDataContainerMapRegisteringEvent (dataContainer);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseDataContainerMapUnregisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject ());

      _eventDistributor.Expect (
          mock =>
          mock.DataContainerMapUnregistering (
              _clientTransaction,
              dataContainer));
      _eventDistributor.Replay ();

      _eventBroker.RaiseDataContainerMapUnregisteringEvent (dataContainer);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void RaiseDataContainerStateUpdatedEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject());
      var newDataContainerState = StateType.New;

      _eventDistributor.Expect (
          mock =>
          mock.DataContainerStateUpdated (
              _clientTransaction,
              dataContainer,
              newDataContainerState));
      _eventDistributor.Replay();

      _eventBroker.RaiseDataContainerStateUpdatedEvent (dataContainer, newDataContainerState);

      _eventDistributor.VerifyAllExpectations();
    }

    [Test]
    public void RaiseVirtualRelationEndPointStateUpdatedEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      var newEndPointChangeState = BooleanObjectMother.GetRandomBoolean();

      _eventDistributor.Expect (
          mock =>
          mock.VirtualRelationEndPointStateUpdated (
              _clientTransaction,
              endPointID,
              newEndPointChangeState));
      _eventDistributor.Replay ();

      _eventBroker.RaiseVirtualRelationEndPointStateUpdatedEvent (endPointID, newEndPointChangeState);

      _eventDistributor.VerifyAllExpectations ();
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var rootListener = new SerializableClientTransactionEventDistributorFake();
      var instance = new ClientTransactionEventBroker (clientTransaction, rootListener);

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EventDistributor, Is.Not.Null);
    }
  }
}