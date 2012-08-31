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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Reflection;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectLifetimeAgentTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private ClientTransactionEventSinkWithMock _eventSinkWithMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IDataManager _dataManagerMock;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private IPersistenceStrategy _persistenceStrategyMock;

    private ObjectLifetimeAgent _agent;

    private ObjectID _objectID1;
    private DomainObject _domainObject1;
    private DataContainer _dataContainer1;

    private ObjectID _objectID2;
    private DomainObject _domainObject2;
    private DataContainer _dataContainer2;
    private IDomainObjectCreator _domainObjectCreatorMock;
    private ClassDefinition _typeDefinitionWithCreatorMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransactionObjectMother.Create();
      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock();
      _invalidDomainObjectManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager> ();
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _persistenceStrategyMock = MockRepository.GenerateStrictMock<IPersistenceStrategy>();

      _agent = new ObjectLifetimeAgent (
          _transaction, 
          _eventSinkWithMock, 
          _invalidDomainObjectManagerMock, 
          _dataManagerMock, 
          _enlistedDomainObjectManagerMock,
          _persistenceStrategyMock);

      _objectID1 = DomainObjectIDs.Order1;
      _domainObject1 = DomainObjectMother.CreateFakeObject (_objectID1);
      _dataContainer1 = DataContainerObjectMother.CreateExisting (_domainObject1);

      _objectID2 = DomainObjectIDs.Order2;
      _domainObject2 = DomainObjectMother.CreateFakeObject (_objectID2);
      _dataContainer2 = DataContainerObjectMother.CreateExisting (_domainObject2);

      _domainObjectCreatorMock = MockRepository.GenerateStrictMock<IDomainObjectCreator>();
      _typeDefinitionWithCreatorMock = ClassDefinitionObjectMother.CreateClassDefinition (
          classType: typeof (OrderItem), instanceCreator: _domainObjectCreatorMock);
    }

    [Test]
    public void NewObject ()
    {
      var constructorParameters = ParamList.Create ("Some Product");

      _eventSinkWithMock.ExpectMock (mock => mock.NewObjectCreating (_eventSinkWithMock.ClientTransaction, _typeDefinitionWithCreatorMock.ClassType));
      _persistenceStrategyMock.Expect (mock => mock.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (_typeDefinitionWithCreatorMock.ClassType, constructorParameters))
          .WhenCalled (
              mi =>
              {
                Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
                Assert.That (_agent.CurrentInitializationContext, Is.Not.Null);
                Assert.That (_agent.CurrentInitializationContext, Is.TypeOf<ObjectInitializationContext>());
                Assert.That (_agent.CurrentInitializationContext.ObjectID, Is.SameAs (_objectID1));
                Assert.That (
                    ((ObjectInitializationContext) _agent.CurrentInitializationContext).EnlistedDomainObjectManager,
                    Is.SameAs (_enlistedDomainObjectManagerMock));
                Assert.That (((ObjectInitializationContext) _agent.CurrentInitializationContext).DataManager, Is.SameAs (_dataManagerMock));
                Assert.That (_agent.CurrentInitializationContext.BindingClientTransaction, Is.Null);
              })
          .Return (_domainObject1);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      Assert.That (_agent.CurrentInitializationContext, Is.Null);

      var result = _agent.NewObject (_typeDefinitionWithCreatorMock, constructorParameters);

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      Assert.That (_agent.CurrentInitializationContext, Is.Null);

      _eventSinkWithMock.VerifyMock();
      _persistenceStrategyMock.VerifyAllExpectations();
      _domainObjectCreatorMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void NewObject_WithBindingClientTransaction ()
    {
      var bindingTrancation = ClientTransactionObjectMother.CreateBinding();

      var agent = new ObjectLifetimeAgent (
          bindingTrancation,
          _eventSinkWithMock,
          _invalidDomainObjectManagerMock,
          _dataManagerMock,
          _enlistedDomainObjectManagerMock,
          _persistenceStrategyMock);

      _eventSinkWithMock.StubMock (mock => mock.NewObjectCreating (Arg<ClientTransaction>.Is.Anything, Arg<Type>.Is.Anything));
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (Arg<ClassDefinition>.Is.Anything)).Return (_objectID1);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (_typeDefinitionWithCreatorMock.ClassType, ParamList.Empty))
          .WhenCalled (mi => 
          {
            Assert.That (agent.CurrentInitializationContext, Is.Not.Null);
            Assert.That (agent.CurrentInitializationContext.BindingClientTransaction, Is.SameAs (bindingTrancation)); 
          })
          .Return (_domainObject1);

      agent.NewObject (_typeDefinitionWithCreatorMock, ParamList.Empty);

      _domainObjectCreatorMock.VerifyAllExpectations();
    }

    [Test]
    public void NewObject_InitializationContextIsThreadLocal()
    {
      _eventSinkWithMock.StubMock (mock => mock.NewObjectCreating (Arg<ClientTransaction>.Is.Anything, Arg<Type>.Is.Anything));
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (Arg<ClassDefinition>.Is.Anything)).Return (_objectID1);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (_typeDefinitionWithCreatorMock.ClassType, ParamList.Empty))
          .WhenCalled (mi =>
          {
            Assert.That (_agent.CurrentInitializationContext, Is.Not.Null);
            ThreadRunner.Run (() => Assert.That (_agent.CurrentInitializationContext, Is.Null));
          })
          .Return (_domainObject1);

      _agent.NewObject (_typeDefinitionWithCreatorMock, ParamList.Empty);

      _domainObjectCreatorMock.VerifyAllExpectations ();
    }

    [Test]
    public void NewObject_CleansUpContext_WhenExceptionIsThrown ()
    {
      _eventSinkWithMock.StubMock (mock => mock.NewObjectCreating (Arg<ClientTransaction>.Is.Anything, Arg<Type>.Is.Anything));
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (Arg<ClassDefinition>.Is.Anything)).Return (_objectID1);

      var exception = new Exception ("Test");

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (_typeDefinitionWithCreatorMock.ClassType, ParamList.Empty))
          .WhenCalled (mi => Assert.That (_agent.CurrentInitializationContext, Is.Not.Null))
          .Throw (exception);

      Assert.That (_agent.CurrentInitializationContext, Is.Null);

      Assert.That (() => _agent.NewObject (_typeDefinitionWithCreatorMock, ParamList.Empty), Throws.Exception.SameAs (exception));

      _domainObjectCreatorMock.VerifyAllExpectations ();
      Assert.That (_agent.CurrentInitializationContext, Is.Null);
    }

    [Test]
    public void NewObject_RecursiveContexts ()
    {
      var typeDefinitionWithCreatorMock2 = ClassDefinitionObjectMother.CreateClassDefinition (instanceCreator: _domainObjectCreatorMock);
      _eventSinkWithMock.StubMock (mock => mock.NewObjectCreating (Arg<ClientTransaction>.Is.Anything, Arg<Type>.Is.Anything));
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (typeDefinitionWithCreatorMock2)).Return (_objectID2);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (_typeDefinitionWithCreatorMock.ClassType, ParamList.Empty))
          .WhenCalled (mi =>
          {
            Assert.That (_agent.CurrentInitializationContext, Is.Not.Null);
            Assert.That (_agent.CurrentInitializationContext.ObjectID, Is.EqualTo (_objectID1));

            _agent.NewObject (typeDefinitionWithCreatorMock2, ParamList.Empty);

            Assert.That (_agent.CurrentInitializationContext, Is.Not.Null);
            Assert.That (_agent.CurrentInitializationContext.ObjectID, Is.EqualTo (_objectID1));
          })
          .Return (_domainObject1);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (typeDefinitionWithCreatorMock2.ClassType, ParamList.Empty))
          .WhenCalled (mi =>
          {
            Assert.That (_agent.CurrentInitializationContext, Is.Not.Null);
            Assert.That (_agent.CurrentInitializationContext.ObjectID, Is.EqualTo (_objectID2));
          })
          .Return (_domainObject2);

      Assert.That (_agent.CurrentInitializationContext, Is.Null);

      _agent.NewObject (_typeDefinitionWithCreatorMock, ParamList.Empty);

      _domainObjectCreatorMock.VerifyAllExpectations ();
      Assert.That (_agent.CurrentInitializationContext, Is.Null);
    }

    [Test]
    public void NewObject_AbstractClass ()
    {
      var typeDefinition = GetTypeDefinition (typeof (AbstractClass));
      Assert.That (
          () => _agent.NewObject (typeDefinition, ParamList.Empty), 
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass' because it is abstract. "
              + "For classes with automatic properties, InstantiableAttribute must be used."));
    }
    
    [Test]
    public void GetObjectReference_KnownObject_Invalid_Works ()
    {
      _invalidDomainObjectManagerMock.Expect (mock => mock.IsInvalid (_objectID1)).Return (true);
      _invalidDomainObjectManagerMock.Expect (mock => mock.GetInvalidObjectReference (_objectID1)).Return (_domainObject1);

      var result = _agent.GetObjectReference (_objectID1);

      _invalidDomainObjectManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObjectReference_KnownObject_ReturnedWithoutLoading ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _enlistedDomainObjectManagerMock.Expect (mock => mock.GetEnlistedDomainObject (_objectID1)).Return (_domainObject1);

      var result = _agent.GetObjectReference (_objectID1);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObjectReference_UnknownObject_ReturnsUnloadedObject ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _enlistedDomainObjectManagerMock.Stub (stub => stub.GetEnlistedDomainObject (_objectID1)).Return (null);

      var result = _agent.GetObjectReference (_objectID1);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (InterceptedDomainObjectCreator.Instance.Factory.WasCreatedByFactory (((object) result).GetType ()), Is.True);
      Assert.That (result.ID, Is.EqualTo (_objectID1));
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetObject ()
    {
      Assert.That (_dataContainer1.State, Is.Not.EqualTo (StateType.Deleted));

      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, true)).Return (_dataContainer1);

      var result = _agent.GetObject (_objectID1, BooleanObjectMother.GetRandomBoolean());

      _dataManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_dataContainer1.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedTrue ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted (_domainObject1);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithLazyLoad (_objectID1, true)).Return (dataContainer);

      var result = _agent.GetObject (_objectID1, true);

      Assert.That (result, Is.SameAs (dataContainer.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedFalse ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted (_domainObject1);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithLazyLoad (_objectID1, true)).Return (dataContainer);

      Assert.That (() => _agent.GetObject (_objectID1, false), Throws.TypeOf<ObjectDeletedException>().With.Property<ObjectDeletedException>(e => e.ID).EqualTo (_objectID1));
    }

    [Test]
    public void TryGetObject_InvalidObject ()
    {
      _invalidDomainObjectManagerMock.Expect (stub => stub.IsInvalid (_objectID1)).Return (true);
      _invalidDomainObjectManagerMock.Expect (stub => stub.GetInvalidObjectReference (_objectID1)).Return (_domainObject1);

      var result = _agent.TryGetObject (_objectID1);
      
      _dataManagerMock.AssertWasNotCalled (mock => mock.GetDataContainerWithLazyLoad (Arg<ObjectID>.Is.Anything, Arg<bool>.Is.Anything));
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_Found ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false)).Return (_dataContainer1);

      var result = _agent.TryGetObject (_objectID1);

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_dataContainer1.DomainObject));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_NotFound ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false)).Return (null);

      var result = _agent.TryGetObject (_objectID1);

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetObjects ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      var result = _agent.GetObjects<Order> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<Order[]>().And.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetObjects_InvalidType ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (
          () => _agent.GetObjects<ClassWithAllDataTypes> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }), 
          Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void TryGetObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<Order[]>().And.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithNotFoundObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Stub (stub => stub.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { null, _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });

      Assert.That (result, Is.EqualTo (new[] { null, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithInvalidObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (true);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.GetInvalidObjectReference (_objectID1)).Return (_domainObject1);

      _dataManagerMock
            .Stub (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID2 }, false))
            .Return (new[] { _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_InvalidType ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (
          () => _agent.TryGetObjects<ClassWithAllDataTypes> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }),
          Throws.TypeOf<InvalidCastException> ());
    }

    [Test]
    public void Delete ()
    {
      var commandMock1 = MockRepository.GenerateStrictMock<IDataManagementCommand> ();
      var commandMock2 = MockRepository.GenerateStrictMock<IDataManagementCommand> ();

      var counter = new OrderedExpectationCounter();
      _dataManagerMock.Expect (mock => mock.CreateDeleteCommand (_domainObject1)).Return (commandMock1);
      commandMock2.Stub (stub => stub.GetAllExceptions()).Return (Enumerable.Empty<Exception>());
      commandMock1.Expect (mock => mock.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (commandMock2)).Ordered (counter);
      commandMock2.Expect (mock => mock.Begin ()).Ordered (counter);
      commandMock2.Expect (mock => mock.Perform ()).Ordered (counter);
      commandMock2.Expect (mock => mock.End ()).Ordered (counter);

      _agent.Delete (_domainObject1);

      _dataManagerMock.VerifyAllExpectations();
      commandMock1.VerifyAllExpectations();
      commandMock2.VerifyAllExpectations();
    }

    [Test]
    public void Serialization ()
    {
      var instance = new ObjectLifetimeAgent (
          _transaction,
          new SerializableClientTransactionEventSinkFake(),
          new SerializableInvalidDomainObjectManagerFake(),
          new SerializableDataManagerFake(),
          new SerializableEnlistedDomainObjectManagerFake(),
          new SerializablePersistenceStrategyFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
      Assert.That (deserializedInstance.InvalidDomainObjectManager, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
      Assert.That (deserializedInstance.EnlistedDomainObjectManager, Is.Not.Null);
      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
    }

  }
}