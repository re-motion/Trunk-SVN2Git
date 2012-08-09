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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Reflection;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private DataManager _dataManager;

    private MockRepository _mockRepository;
    private Dictionary<Enum, object> _fakeApplicationData;
    private IDataManager _dataManagerMock;
    private IEnlistedDomainObjectManager _enlistedObjectManagerMock;
    private ClientTransactionExtensionCollection _fakeExtensions;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IClientTransactionEventBroker _eventBrokerMock;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IQueryManager _queryManagerMock;
    private ICommitRollbackAgent _commitRollbackAgentMock;

    private TestableClientTransaction _transactionWithMocks;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction();
      _dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);

      _mockRepository = new MockRepository();
      _fakeApplicationData = new Dictionary<Enum, object>();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _enlistedObjectManagerMock = _mockRepository.StrictMock<IEnlistedDomainObjectManager> ();
      _fakeExtensions = new ClientTransactionExtensionCollection("test");
      _invalidDomainObjectManagerMock = _mockRepository.StrictMock<IInvalidDomainObjectManager> ();
      _eventBrokerMock = _mockRepository.StrictMock<IClientTransactionEventBroker>();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _queryManagerMock = _mockRepository.StrictMock<IQueryManager> ();
      _commitRollbackAgentMock = _mockRepository.StrictMock<ICommitRollbackAgent>();

      _transactionWithMocks = ClientTransactionObjectMother.CreateWithComponents<TestableClientTransaction> (
          null,
          _fakeApplicationData,
          tx => { throw new NotImplementedException(); },
          _dataManagerMock,
          _enlistedObjectManagerMock,
          _fakeExtensions,
          _invalidDomainObjectManagerMock,
          _eventBrokerMock,
          _persistenceStrategyMock,
          _queryManagerMock,
          _commitRollbackAgentMock);
      // Ignore calls made by ctor
      _eventBrokerMock.BackToRecord();
    }

    [Test]
    public void Initialization_OrderOfFactoryCalls ()
    {
      var fakeParentTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransactionTestHelper.SetIsActive (fakeParentTransaction, false);

      var componentFactoryMock = _mockRepository.StrictMock<IClientTransactionComponentFactory>();
      var listenerManagerMock = _mockRepository.StrictMock<IClientTransactionEventBroker>();
      
      var fakeExtensionCollection = new ClientTransactionExtensionCollection ("test");

      var parentListenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      ClientTransactionTestHelper.AddListener (fakeParentTransaction, parentListenerMock);

      ClientTransaction constructedTransaction = null;

      using (_mockRepository.Ordered ())
      {
        componentFactoryMock
            .Expect (mock => mock.GetParentTransaction (Arg<ClientTransaction>.Is.Anything))
            .Return (fakeParentTransaction)
            .WhenCalled (
                mi =>
                {
                  constructedTransaction = (ClientTransaction) mi.Arguments[0];
                  Assert.That (constructedTransaction.ID, Is.Not.EqualTo (Guid.Empty));
                });
        componentFactoryMock
            .Expect (mock => mock.CreateApplicationData (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_fakeApplicationData)
            .WhenCalled (mi => Assert.That (constructedTransaction.ParentTransaction == fakeParentTransaction));
        componentFactoryMock
            .Expect (mock => mock.CreateEventBroker (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (listenerManagerMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.ApplicationData, Is.SameAs (_fakeApplicationData)));
        componentFactoryMock
            .Expect (mock => mock.CreateEnlistedObjectManager (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_enlistedObjectManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetEventBroker (constructedTransaction), Is.SameAs (listenerManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreateInvalidDomainObjectManager (
                Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), 
                Arg.Is (listenerManagerMock)))
            .Return (_invalidDomainObjectManagerMock)
            .WhenCalled (
                mi => Assert.That (
                    ClientTransactionTestHelper.GetEnlistedDomainObjectManager (constructedTransaction), Is.SameAs (_enlistedObjectManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreatePersistenceStrategy (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_persistenceStrategyMock)
            .WhenCalled (
                mi => Assert.That (
                    ClientTransactionTestHelper.GetInvalidDomainObjectManager (constructedTransaction), Is.SameAs (_invalidDomainObjectManagerMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateDataManager (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), 
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == listenerManagerMock),
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock)))
            .Return (_dataManagerMock)
            .WhenCalled (
                mi => Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (constructedTransaction), Is.SameAs (_persistenceStrategyMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateQueryManager (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == listenerManagerMock), 
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock), 
                    Arg.Is (_dataManagerMock)))
            .Return (_queryManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetIDataManager (constructedTransaction), Is.SameAs (_dataManagerMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateCommitRollbackAgent (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == listenerManagerMock),
                    Arg.Is (_persistenceStrategyMock),
                    Arg.Is (_dataManagerMock)))
            .Return (_commitRollbackAgentMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.QueryManager, Is.SameAs (_queryManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreateExtensionCollection (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (fakeExtensionCollection)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetCommitRollbackAgent (constructedTransaction), Is.SameAs (_commitRollbackAgentMock)));
        listenerManagerMock
            .Expect (mock => mock.AddListener (Arg<ExtensionClientTransactionListener>.Is.TypeOf))
            .WhenCalled (
                mi =>
                {
                  Assert.That (constructedTransaction.Extensions, Is.SameAs (fakeExtensionCollection));
                  Assert.That (((ExtensionClientTransactionListener) mi.Arguments[0]).Extension, Is.SameAs (constructedTransaction.Extensions)); 
                });

        parentListenerMock.Expect (
            mock => mock.SubTransactionInitialize (
                Arg.Is (fakeParentTransaction), 
                Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)));
        listenerManagerMock
            .Expect (mock => mock.RaiseEvent (Arg<Action<ClientTransaction, IClientTransactionListener>>.Is.Anything))
            .WhenCalled (
                mi => CheckRaisedEvent (
                    (Action<ClientTransaction, IClientTransactionListener>)mi.Arguments[0], 
                    (tx, l) => l.TransactionInitialize (tx)));
      }

      _mockRepository.ReplayAll();

      var result = Activator.CreateInstance (
          typeof (ClientTransaction),
          BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { componentFactoryMock },
          null);

      _mockRepository.VerifyAll();

      Assert.That (result, Is.SameAs (constructedTransaction));
    }

    [Test]
    public void ParentTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();

      Assert.That (subTransaction.ParentTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void ParentTransaction_Null ()
    {
      Assert.That (_transaction.ParentTransaction, Is.Null);
    }

    [Test]
    public void ActiveSubTransaction_Null ()
    {
      Assert.That (_transaction.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction_Same ()
    {
      Assert.That (_transaction.RootTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void RootTransaction_NotSame ()
    {
      var grandParentTransaction = _transaction;
      var parentTransaction = grandParentTransaction.CreateSubTransaction();
      var transaction = parentTransaction.CreateSubTransaction();

      Assert.That (transaction.RootTransaction, Is.SameAs (grandParentTransaction));
    }

    [Test]
    public void LeafTransaction_Same ()
    {
      Assert.That (_transaction.LeafTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void LeafTransaction_NotSame ()
    {
      var subTransaction1 = _transaction.CreateSubTransaction ();
      var subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (_transaction.LeafTransaction, Is.SameAs (subTransaction2));
    }

    [Test]
    public void ToString_LeafRootTransaction ()
    {
      var expected = string.Format ("ClientTransaction (root, leaf) {0}", _transaction.ID);
      Assert.That (_transaction.ToString(), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_NonLeafRootTransaction ()
    {
      _transaction.CreateSubTransaction ();
      var expected = string.Format ("ClientTransaction (root, parent) {0}", _transaction.ID);
      Assert.That (_transaction.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_LeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var expected = string.Format ("ClientTransaction (sub, leaf) {0}", subTransaction.ID);
      Assert.That (subTransaction.ToString(), Is.EqualTo (expected));
    }

    [Test]
    public void ToString_NonLeafSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.CreateSubTransaction ();
      var expected = string.Format ("ClientTransaction (sub, parent) {0}", subTransaction.ID);
      Assert.That (subTransaction.ToString (), Is.EqualTo (expected));
    }

    [Test]
    public void AddListener ()
    {
      var listenerStub = MockRepository.GenerateStub<IClientTransactionListener>();
      _eventBrokerMock.Expect (mock => mock.AddListener (listenerStub));
      _eventBrokerMock.Replay();

      _transactionWithMocks.AddListener (listenerStub);

      _eventBrokerMock.VerifyAllExpectations();
    }

    [Test]
    public void RemoveListener ()
    {
      var listenerStub = MockRepository.GenerateStub<IClientTransactionListener> ();
      _eventBrokerMock.Expect (mock => mock.RemoveListener (listenerStub));
      _eventBrokerMock.Replay ();

      _transactionWithMocks.RemoveListener (listenerStub);

      _eventBrokerMock.VerifyAllExpectations ();
    }

    [Test]
    public void HasChanged ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.HasDataChanged ()).Return (true).Repeat.Once ();
      _commitRollbackAgentMock.Expect (mock => mock.HasDataChanged ()).Return (false).Repeat.Once ();
      _mockRepository.ReplayAll ();

      var result1 = _transactionWithMocks.HasChanged ();
      var result2 = _transactionWithMocks.HasChanged ();

      _mockRepository.VerifyAll ();

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void Commit ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.CommitData());
      _mockRepository.ReplayAll();

      _transactionWithMocks.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      _commitRollbackAgentMock.Expect (mock => mock.RollbackData ());
      _mockRepository.ReplayAll ();

      _transactionWithMocks.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetObject_UnknownObject_IsLoaded ()
    {
      var result = ClientTransactionTestHelper.CallGetObject (_transaction, DomainObjectIDs.Order1, false);

      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetObject_KnownObject_IsReturned ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, order.ID, false);

      Assert.That (result, Is.SameAs (order));
    }

    [Test]
    public void GetObject_EnlistedButNotLoadedObject_LoadsObject ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      _transaction.EnlistDomainObject (order);

      Assert.That (order.TransactionContext[_transaction].State, Is.EqualTo (StateType.NotLoadedYet));

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, order.ID, false);

      Assert.That (result, Is.SameAs (order));
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetObject_InvalidObject_ThrowsWithoutGoingToDatabase ()
    {
      var invalidObject = _transaction.Execute (() => Official.NewObject ());
      LifetimeService.DeleteObject (_transaction, invalidObject);

      Assert.That (invalidObject.TransactionContext[_transaction].IsInvalid, Is.True);

      var storageProviderMock = UnitTestStorageProviderStub.CreateStorageProviderMockForOfficial ();
      try
      {
        UnitTestStorageProviderStub.ExecuteWithMock (storageProviderMock, () => 
            ClientTransactionTestHelper.CallGetObject (_transaction, invalidObject.ID, false));
        Assert.Fail ("Expected ObjectsNotFoundException.");
      }
      catch (ObjectInvalidException)
      {
        // ok
      }

      storageProviderMock.AssertWasNotCalled (mock => mock.LoadDataContainer (invalidObject.ID));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject_DeletedObject_IncludeDeletedFalse_Throws ()
    {
      var deletedObject = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      LifetimeService.DeleteObject (_transaction, deletedObject);

      Assert.That (deletedObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Deleted));

      ClientTransactionTestHelper.CallGetObject (_transaction, deletedObject.ID, false);
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedTrue_Returns ()
    {
      var deletedObject = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      LifetimeService.DeleteObject (_transaction, deletedObject);

      Assert.That (deletedObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Deleted));

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, deletedObject.ID, true);

      Assert.That (result, Is.SameAs (deletedObject));
    }

    [Test]
    public void TryGetObject ()
    {
      var fakeOrder = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainer.SetDomainObject (fakeOrder);

      var counter = new OrderedExpectationCounter ();

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (false);
      _enlistedObjectManagerMock.Stub (stub => stub.GetEnlistedDomainObject (DomainObjectIDs.Order1)).Return (fakeOrder);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1, false))
          .Return (dataContainer)
          .Ordered (counter);

      _mockRepository.ReplayAll ();

      var result = ClientTransactionTestHelper.CallTryGetObject (_transactionWithMocks, DomainObjectIDs.Order1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (fakeOrder));
    }

    [Test]
    public void TryGetObject_WithNotFoundObject ()
    {
      var fakeOrder = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      var counter = new OrderedExpectationCounter ();

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (false);
      _enlistedObjectManagerMock.Stub (stub => stub.GetEnlistedDomainObject (DomainObjectIDs.Order1)).Return (fakeOrder);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1, false))
          .Return (null)
          .Ordered (counter);

      _mockRepository.ReplayAll ();

      var result = ClientTransactionTestHelper.CallTryGetObject (_transactionWithMocks, DomainObjectIDs.Order1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetObject_WithInvalidObjects ()
    {
      var fakeOrder = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (true);
      _invalidDomainObjectManagerMock.Stub (stub => stub.GetInvalidObjectReference (DomainObjectIDs.Order1)).Return (fakeOrder);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (Arg<ICollection<ObjectID>>.List.Equal (new ObjectID[0]), Arg.Is (false)))
          .Return (new DataContainer[0]);
      _mockRepository.ReplayAll ();

      var result = ClientTransactionTestHelper.CallTryGetObject (_transactionWithMocks, DomainObjectIDs.Order1);

      Assert.That (result, Is.SameAs (fakeOrder));
    }

    [Test]
    public void GetObjectReference_KnownObject_ReturnedWithoutLoading ()
    {
      var instance = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      _transaction.EnlistDomainObject (instance);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvent (
          _transaction,
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));

      var result = ClientTransactionTestHelper.CallGetObjectReference (_transaction, instance.ID);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    public void GetObjectReference_KnownObject_Invalid_Works ()
    {
      var instance = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      LifetimeService.DeleteObject (_transaction, instance);
      Assert.That (instance.TransactionContext[_transaction].IsInvalid, Is.True);

      var result = ClientTransactionTestHelper.CallGetObjectReference (_transaction, instance.ID);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    public void GetObjectReference_UnknownObject_ReturnsUnloadedObject ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvent (
        _transaction,
        mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      
      var result = ClientTransactionTestHelper.CallGetObjectReference (_transaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (InterceptedDomainObjectCreator.Instance.Factory.WasCreatedByFactory (((object) result).GetType()), Is.True);
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetObjectReference_UnknownObject_BindsToBindingClientTransactionOnly ()
    {
      var unboundResult = ClientTransactionTestHelper.CallGetObjectReference (_transaction, DomainObjectIDs.Order1);
      Assert.That (unboundResult.HasBindingTransaction, Is.False);

      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundResult = ClientTransactionTestHelper.CallGetObjectReference (bindingTransaction, DomainObjectIDs.Order1);
      Assert.That (boundResult.HasBindingTransaction, Is.True);
      Assert.That (boundResult.GetBindingTransaction(), Is.SameAs (bindingTransaction));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      var invalidObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      _transaction.Execute (invalidObject.Delete);

      Assert.That (invalidObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Invalid));

      var invalidObjectReference = ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, invalidObject.ID);

      Assert.That (invalidObjectReference, Is.SameAs (invalidObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been marked invalid.\r\nParameter name: id")]
    public void GetInvalidObjectReference_ThrowsWhenNotInvalid ()
    {
      ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, DomainObjectIDs.Order1);
    }

    [Test]
    public void IsInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.False);

      _transaction.Execute (domainObject.Delete);

      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.True);
    }

    [Test]
    public void NewObject ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);
      listenerMock
          .Expect (mock => mock.NewObjectCreating (_transaction, typeof (OrderItem)))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));

      var result = ClientTransactionTestHelper.CallNewObject (_transaction, typeof (OrderItem), ParamList.Create ("Some Product"));

      listenerMock.VerifyAllExpectations();
      ClientTransactionTestHelper.RemoveListener (_transaction, listenerMock);
      Assert.That (result, Is.Not.Null);

      Assert.That (result, Is.AssignableTo<OrderItem> ());
      Assert.That (result, Is.Not.TypeOf<OrderItem> ());
      var typeDefinition = GetTypeDefinition (typeof (OrderItem));
      var interceptedDomainObjectCreator = ((InterceptedDomainObjectCreator) typeDefinition.GetDomainObjectCreator ());
      var interceptedDomainObjectTypeFactory = interceptedDomainObjectCreator.Factory;
      Assert.That (interceptedDomainObjectTypeFactory.WasCreatedByFactory (((object) result).GetType()), Is.True);

      Assert.That (((OrderItem) result).CtorCalled, Is.True);
      Assert.That (((OrderItem) result).CtorTx, Is.SameAs (_transaction));
      Assert.That (_transaction.Execute (() => ((OrderItem) result).Product), Is.EqualTo ("Some Product"));

      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
    }

    [Test]
    public void NewObject_InitializesMixins ()
    {
      Assert.That (ClientTransaction.Current, Is.Not.SameAs (_transaction));
      var result = ClientTransactionTestHelper.CallNewObject (_transaction, typeof (ClassWithAllDataTypes), ParamList.Empty);

      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (result);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That (mixin.OnDomainObjectCreatedTx, Is.SameAs (_transaction));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1, true))
          .Return (DataContainerObjectMother.CreateDataContainer());
      _mockRepository.ReplayAll();
      
      _transactionWithMocks.EnsureDataAvailable (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EnsureDataAvailable_Many ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainer[0]);
      _mockRepository.ReplayAll ();

      _transactionWithMocks.EnsureDataAvailable (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryEnsureDataAvailable_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1, false))
          .Return (DataContainerObjectMother.CreateDataContainer ());
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1, false))
          .Return (null);
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, false))
          .Return (new[] { DataContainerObjectMother.CreateDataContainer(), DataContainerObjectMother.CreateDataContainer() });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, false))
          .Return (new[] { DataContainerObjectMother.CreateDataContainer (), null });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Complete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1).Orders);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1).Orders);
      
      var endPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint, Is.Not.Null);
      endPoint.MarkDataIncomplete ();
      Assert.That (endPoint.IsDataComplete, Is.False);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      var order1 = _transaction.Execute(() => Order.NewObject ());
      var order2 = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObjects ().ToArray (), Is.EquivalentTo (new[] { order1, order2 }));
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.EnlistedDomainObjectCount, Is.EqualTo (2));
    }

    [Test]
    public void IsEnlisted ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.IsEnlisted (order), Is.True);
      Assert.That (ClientTransaction.CreateRootTransaction().IsEnlisted (order), Is.False);
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObject (order.ID), Is.SameAs (order));
    }

    [Test]
    public void EnlistDomainObject ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (_transaction.IsEnlisted (order), Is.False);

      _transaction.EnlistDomainObject (order);

      Assert.That (_transaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_DoesntLoad ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (_transaction.IsEnlisted (order), Is.False);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnlistDomainObject (order);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    [Test]
    public void EnlistDomainObject_InvalidObjects ()
    {
      var invalidObject = _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (invalidObject.Delete);
      Assert.That (invalidObject.TransactionContext[_transaction].IsInvalid, Is.True);

      var newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (invalidObject);

      Assert.That (newTransaction.IsEnlisted (invalidObject), Is.True);
    }

    [Test]
    public void CopyCollectionEventHandlers ()
    {
      var order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      
      bool orderItemAdded = false;
      _transaction.Execute (() => order.OrderItems.Added += delegate { orderItemAdded = true; });

      var otherTransaction = ClientTransaction.CreateRootTransaction ();
      Assert.That (orderItemAdded, Is.False);

      otherTransaction.EnlistDomainObject (order);
      otherTransaction.CopyCollectionEventHandlers (order, _transaction);

      otherTransaction.Execute (() => order.OrderItems.Add (OrderItem.NewObject ()));
      Assert.That (orderItemAdded, Is.True);
    }

    [Test]
    public void CreateSubTransaction_WithDefaultComponentFactory ()
    {
      Assert.That (_transaction.IsActive, Is.True);
      
      var subTransaction = _transaction.CreateSubTransaction ();
      Assert.That (subTransaction, Is.TypeOf (typeof (ClientTransaction)));
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (_transaction));
      Assert.That (_transaction.IsActive, Is.False);
      Assert.That (_transaction.SubTransaction, Is.SameAs (subTransaction));

      Assert.That (subTransaction.Extensions, Is.Empty);
      Assert.That (subTransaction.ApplicationData, Is.SameAs (_transaction.ApplicationData));
      
      var enlistedObjectManager = ClientTransactionTestHelper.GetEnlistedDomainObjectManager (subTransaction);
      Assert.That (enlistedObjectManager, Is.SameAs (ClientTransactionTestHelper.GetEnlistedDomainObjectManager (_transaction)));

      var invalidDomainObjectManager = ClientTransactionTestHelper.GetInvalidDomainObjectManager (subTransaction);
      Assert.That (invalidDomainObjectManager, Is.TypeOf (typeof (InvalidDomainObjectManager)));
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction);
      Assert.That (persistenceStrategy, Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_WithCustomFactory ()
    {
      ClientTransaction fakeSubTransaction = null;
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ClientTransaction> factoryMock =
          (tx, invalidDomainObjectManager, enlistedDomainObjectManager) =>
          {
            fakeSubTransaction = CreateTransactionInHierarchy (_transaction);
            Assert.That (tx, Is.SameAs (_transaction));
            Assert.That (invalidDomainObjectManager, Is.SameAs (ClientTransactionTestHelper.GetInvalidDomainObjectManager (_transaction)));
            Assert.That (enlistedDomainObjectManager, Is.SameAs (ClientTransactionTestHelper.GetEnlistedDomainObjectManager (_transaction)));
            return fakeSubTransaction;
          };

      Assert.That (_transaction.IsActive, Is.True);

      var subTransaction = _transaction.CreateSubTransaction (factoryMock);

      Assert.That (_transaction.IsActive, Is.False);
      Assert.That (subTransaction, Is.SameAs (fakeSubTransaction));
      Assert.That (_transaction.SubTransaction, Is.SameAs (subTransaction));
    }

    [Test]
    public void CreateSubTransaction_Events ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);
      var mockRepository = listenerMock.GetMockRepository();
      var componentFactoryPartialMock = mockRepository.PartialMock<SubClientTransactionComponentFactory> (
          _transaction,
          ClientTransactionTestHelper.GetInvalidDomainObjectManager (_transaction),
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager (_transaction));
      var eventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);

      using (mockRepository.Ordered ())
      {
        listenerMock
            .Expect (mock => mock.SubTransactionCreating (_transaction))
            .WhenCalled (mi => Assert.That (_transaction.IsActive, Is.True));
        componentFactoryPartialMock
            .Expect (mock => mock.GetParentTransaction (Arg<ClientTransaction>.Is.Anything))
            .Return (_transaction);
        eventReceiverMock
            .Expect (mock => mock.SubTransactionCreated (
                Arg.Is (_transaction), 
                Arg<SubTransactionCreatedEventArgs>.Matches (arg => arg.SubTransaction.ParentTransaction == _transaction)))
            .WhenCalled (mi =>
            {
              Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
              Assert.That (_transaction.IsActive, Is.False);
            });
        listenerMock
            .Expect (mock => mock.SubTransactionCreated (
                Arg.Is (_transaction), 
                Arg<ClientTransaction>.Matches (subTx => subTx.ParentTransaction == _transaction)));
      }

      mockRepository.ReplayAll ();

      _transaction.CreateSubTransaction ((tx, invalidDomainObjectManager, enlistedDomainObjectManager) => 
          ClientTransactionObjectMother.CreateWithComponents<ClientTransaction> (componentFactoryPartialMock));
      mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateSubTransaction_CancellationInCreatingNotification ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);
      var eventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (_transaction);
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ClientTransaction> factoryMock = 
          (tx, invalidDomainObjectManager, enlistedDomainobjectManager) =>
          {
            Assert.Fail ("Must not be called.");
            return null;
          };

      var exception = new InvalidOperationException ("Canceled");
      listenerMock
          .Expect (mock => mock.SubTransactionCreating (_transaction))
          .Throw (exception);
      listenerMock.Replay ();
      eventReceiverMock.Replay ();

      try
      {
        _transaction.CreateSubTransaction (factoryMock);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }

      listenerMock.AssertWasNotCalled (mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<ClientTransaction>.Is.Anything));

      eventReceiverMock.AssertWasNotCalled (mock => mock.SubTransactionCreated (Arg<object>.Is.Anything, Arg<SubTransactionCreatedEventArgs>.Is.Anything));

      Assert.That (_transaction.IsActive, Is.True);
    }

    [Test]
    public void CreateSubTransaction_ExceptionInFactory ()
    {
      var exception = new InvalidOperationException ("Canceled");
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ClientTransaction> factoryMock = 
          (tx, invalidDomainObjectManager, enlistedDomainObjectManager) => { throw (exception); };

      try
      {
        _transaction.CreateSubTransaction (factoryMock);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }

      Assert.That (_transaction.IsActive, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The given component factory did not create a sub-transaction for this transaction.")]
    public void CreateSubTransaction_Throws_WhenParentTransactionDoesNotMatch ()
    {
      try
      {
        _transaction.CreateSubTransaction ((tx, invalidDomainObjectManager, enlistedDomainObjectManager) => ClientTransaction.CreateRootTransaction());
      }
      catch (InvalidOperationException)
      {
        Assert.That (_transaction.IsActive, Is.True);
        throw;
      }
    }

    [Test]
    public void Discard ()
    {
      var listenerMock = SetupEventForwardingToListenerMock (_eventBrokerMock, _transactionWithMocks);
      listenerMock.Expect (mock => mock.TransactionDiscard (_transactionWithMocks));
      _eventBrokerMock.Expect (mock => mock.AddListener (Arg<InvalidatedTransactionListener>.Is.TypeOf));
      _mockRepository.ReplayAll();

      Assert.That (_transactionWithMocks.IsDiscarded, Is.False);

      _transactionWithMocks.Discard();

      _mockRepository.VerifyAll();
      Assert.That (_transactionWithMocks.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_WithParentTransaction ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsActive (parentTransaction, false);

      var subTransaction = CreateTransactionInHierarchy (parentTransaction);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, subTransaction);

      _eventBrokerMock.Stub (stub => stub.RaiseEvent (Arg<Action<ClientTransaction, IClientTransactionListener>>.Is.Anything));
      _eventBrokerMock.Stub (stub => stub.AddListener (Arg<IClientTransactionListener>.Is.Anything));
      _mockRepository.ReplayAll ();

      Assert.That (parentTransaction.IsActive, Is.False);
      Assert.That (parentTransaction.SubTransaction, Is.Not.Null);

      subTransaction.Discard ();

      Assert.That (parentTransaction.IsActive, Is.True);
      Assert.That (parentTransaction.SubTransaction, Is.Null);
    }

    [Test]
    public void Discard_Twice ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsActive (parentTransaction, false);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, _transactionWithMocks);

      var subTransaction = CreateTransactionInHierarchy (parentTransaction);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, subTransaction);
      _eventBrokerMock.Stub (stub => stub.RaiseEvent (Arg<Action<ClientTransaction, IClientTransactionListener>>.Is.Anything));

      subTransaction.Discard();

      ClientTransactionTestHelper.SetIsActive (parentTransaction, false);
      var otherSubTransaction = CreateTransactionInHierarchy (parentTransaction);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, otherSubTransaction);

      _eventBrokerMock.BackToRecord();
      _eventBrokerMock.Replay();

      subTransaction.Discard ();

      _eventBrokerMock.AssertWasNotCalled (mock => mock.RaiseEvent (Arg<Action<ClientTransaction, IClientTransactionListener>>.Is.Anything));
      Assert.That (parentTransaction.IsActive, Is.False);
      Assert.That (parentTransaction.SubTransaction, Is.SameAs (otherSubTransaction));
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      _transaction.Execute (() => order.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2));
      
      DomainObject orderTicket = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket, Is.SameAs (_transaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket2))));
    }

    [Test]
    public void GetRelatedObject_Deleted ()
    {
      Location location = _transaction.Execute (() => Location.GetObject (DomainObjectIDs.Location1));

      var client = _transaction.Execute (() => location.Client);
      _transaction.Execute (() => location.Client.Delete());

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);

      Assert.That (result, Is.SameAs (client));
      Assert.That (_transaction.Execute (() => result.State), Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void GetRelatedObject_Invalid ()
    {
      Location location = _transaction.Execute (() => Location.GetObject (DomainObjectIDs.Location1));
      Client newClient = _transaction.Execute (() => Client.NewObject ());
      _transaction.Execute (() => location.Client = newClient);
      _transaction.Execute (() => location.Client.Delete ());

      var endPointID = RelationEndPointID.Create (location.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      var result = ClientTransactionTestHelper.CallGetRelatedObject (_transaction, endPointID);
      Assert.That (result, Is.SameAs (newClient));
      Assert.That (_transaction.Execute (() => result.State), Is.EqualTo (StateType.Invalid));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object (cardinality one).\r\nParameter name: relationEndPointID")]
    public void GetRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      ClientTransactionTestHelper.CallGetRelatedObject (
          _transaction, 
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _transaction.Execute (() => order.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2));

      DomainObject orderTicket = ClientTransactionTestHelper.CallGetOriginalRelatedObject (_transaction, endPointID);

      Assert.That (orderTicket, Is.Not.Null);
      Assert.That (orderTicket, Is.SameAs (_transaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket1))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object (cardinality one).\r\nParameter name: relationEndPointID")]
    public void GetOriginalRelatedObject_WrongCardinality ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      ClientTransactionTestHelper.CallGetOriginalRelatedObject (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      
      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      var endPoint = ((ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (_transaction).GetRelationEndPointWithLazyLoad (endPointID));
      endPoint.CreateAddCommand (_transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem3))).ExpandToAllRelatedObjects ().Perform ();

      var orderItems = ClientTransactionTestHelper.CallGetRelatedObjects (_transaction, endPointID);

      Assert.That (orderItems, Is.TypeOf<ObjectList<OrderItem>>());
      Assert.That (
          orderItems,
          Is.EquivalentTo (
              new[]
              {
                  _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1)),
                  _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem2)),
                  _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem3))
              }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object collection (cardinality many).\r\nParameter name: relationEndPointID")]
    public void GetRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      ClientTransactionTestHelper.CallGetRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      var endPointID = RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      var endPoint = ((ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (_transaction).GetRelationEndPointWithLazyLoad (endPointID));
      endPoint.CreateAddCommand (_transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem3))).ExpandToAllRelatedObjects ().Perform ();

      var orderItems = ClientTransactionTestHelper.CallGetOriginalRelatedObjects (_transaction, endPointID);

      Assert.That (orderItems, Is.TypeOf<ObjectList<OrderItem>> ());
      Assert.That (
        orderItems,
        Is.EquivalentTo (
            new[]
              {
                  _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1)),
                  _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem2))
              }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The given end-point ID does not denote a related object collection (cardinality many).\r\nParameter name: relationEndPointID")]
    public void GetOriginalRelatedObjects_WrongCardinality ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      ClientTransactionTestHelper.CallGetOriginalRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void TryGetObjects ()
    {
      var fakeOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainer1.SetDomainObject (fakeOrder1);

      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order2);
      dataContainer2.SetDomainObject (fakeOrder2);

      var counter = new OrderedExpectationCounter();
      
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order2)).Return (false);

      _dataManagerMock
          .Expect (
              mock => mock.GetDataContainersWithLazyLoad (
                  Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }),
                  Arg.Is (false)))
          .Return (new[] { dataContainer1, dataContainer2 })
          .Ordered (counter);

      _mockRepository.ReplayAll();

      var result = _transactionWithMocks.TryGetObjects<Order> (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { fakeOrder1, fakeOrder2 }));
    }

    [Test]
    public void TryGetObjects_WithNotFoundObjects ()
    {
      var fakeOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order2);
      dataContainer.SetDomainObject (fakeOrder2);

      var counter = new OrderedExpectationCounter ();

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order2)).Return (false);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }),
              Arg.Is (false)))
          .Return (new[] { null, dataContainer })
          .Ordered (counter);

      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryGetObjects<Order> (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { null, fakeOrder2 }));
    }

    [Test]
    public void TryGetObjects_WithInvalidObjects ()
    {
      var fakeOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order2);
      dataContainer.SetDomainObject (fakeOrder2);

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (true);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order2)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.GetInvalidObjectReference (DomainObjectIDs.Order1)).Return (fakeOrder1);

      _dataManagerMock
            .Stub (mock => mock.GetDataContainersWithLazyLoad (
                Arg<ICollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 }),
                Arg.Is (false)))
            .Return (new[] { dataContainer });

      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryGetObjects<Order> (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      Assert.That (result, Is.EqualTo (new[] { fakeOrder1, fakeOrder2 }));
    }

    [Test]
    public void GetObjects ()
    {
      var fakeOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeOrder2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainer1.SetDomainObject (fakeOrder1);

      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order2);
      dataContainer2.SetDomainObject (fakeOrder2);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }),
              Arg.Is (true)))
          .Return (new[] { dataContainer1, dataContainer2 });

      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.GetObjects<Order> (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { fakeOrder1, fakeOrder2 }));
    }

    [Test]
    public void GetObjects_InvalidType ()
    {
      var fakeOrder = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainer.SetDomainObject (fakeOrder);

      _dataManagerMock
          .Stub (mock => mock.GetDataContainersWithLazyLoad (Arg<ICollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1 }), Arg.Is (true)))
          .Return (new[] { dataContainer });
      _mockRepository.ReplayAll ();

      Assert.That (() => _transactionWithMocks.GetObjects<Customer> (DomainObjectIDs.Order1), Throws.TypeOf<InvalidCastException>());

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serialization ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var subTransaction = clientTransaction.CreateSubTransaction ();

      var deserializedClientTransaction = Serializer.SerializeAndDeserialize (clientTransaction);

      Assert.That (deserializedClientTransaction, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetComponentFactory (deserializedClientTransaction), Is.Not.Null);
      Assert.That (deserializedClientTransaction.ParentTransaction, Is.Null);
      Assert.That (deserializedClientTransaction.ApplicationData, Is.Not.Null);
      Assert.That (deserializedClientTransaction.Extensions, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetEventBroker (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetEnlistedDomainObjectManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetInvalidDomainObjectManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetIDataManager (deserializedClientTransaction), Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (deserializedClientTransaction), Is.Not.Null);
      Assert.That (deserializedClientTransaction.QueryManager, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetCommitRollbackAgent (deserializedClientTransaction), Is.Not.Null);
      Assert.That (deserializedClientTransaction.SubTransaction, Is.Not.Null);
      Assert.That (deserializedClientTransaction.IsDiscarded, Is.False);
      Assert.That (deserializedClientTransaction.ID, Is.EqualTo (clientTransaction.ID));

      var deserializedSubTransaction = Serializer.SerializeAndDeserialize (subTransaction);

      Assert.That (deserializedSubTransaction.ParentTransaction, Is.Not.Null);
      Assert.That (deserializedSubTransaction.SubTransaction, Is.Null);

      clientTransaction.Discard();

      var deserializedDiscardedTransaction = Serializer.SerializeAndDeserialize (clientTransaction);

      Assert.That (deserializedDiscardedTransaction.IsDiscarded, Is.True);
    }

    private ClientTransaction CreateTransactionInHierarchy (ClientTransaction parent)
    {
      return ClientTransactionObjectMother.CreateWithComponents<ClientTransaction> (
          parent,
          _fakeApplicationData,
          tx => { throw new NotImplementedException (); },
          _dataManagerMock,
          _enlistedObjectManagerMock,
          _fakeExtensions,
          _invalidDomainObjectManagerMock,
          _eventBrokerMock,
          _persistenceStrategyMock,
          _queryManagerMock);
    }

    private void CheckRaisedEvent (Action<ClientTransaction, IClientTransactionListener> raiseAction, Action<ClientTransaction, IClientTransactionListener> expectedEvent)
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();

      var listenerMock = MockRepository.GenerateStrictMock<IClientTransactionListener>();
      listenerMock.Expect (mock => expectedEvent (clientTransaction, mock));
      listenerMock.Replay();

      raiseAction (clientTransaction, listenerMock);

      listenerMock.VerifyAllExpectations();
    }

    private IClientTransactionListener SetupEventForwardingToListenerMock (IClientTransactionEventBroker eventBrokerMock, TestableClientTransaction expectedClientTransaction)
    {
      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      eventBrokerMock
          .Stub (stub => stub.RaiseEvent (Arg<Action<ClientTransaction, IClientTransactionListener>>.Is.Anything))
          .WhenCalled (mi => ((Action<ClientTransaction, IClientTransactionListener>) mi.Arguments[0]) (expectedClientTransaction, listenerMock));
      return listenerMock;
    }
  }
}