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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
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
    private IClientTransactionEventBroker _eventBrokerMock;
    private ITransactionHierarchyManager _hierarchyManagerMock;
    private IEnlistedDomainObjectManager _enlistedObjectManagerMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IDataManager _dataManagerMock;
    private IObjectLifetimeAgent _objectLifetimeAgentMock;
    private IQueryManager _queryManagerMock;
    private ICommitRollbackAgent _commitRollbackAgentMock;
    private ClientTransactionExtensionCollection _fakeExtensions;

    private TestableClientTransaction _transactionWithMocks;

    private ObjectID _objectID1;
    private DomainObject _fakeDomainObject1;
    private ObjectID _objectID2;
    private DomainObject _fakeDomainObject2;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction();
      _dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);

      _mockRepository = new MockRepository();
      _fakeApplicationData = new Dictionary<Enum, object> ();
      _eventBrokerMock = _mockRepository.StrictMock<IClientTransactionEventBroker> ();
      _hierarchyManagerMock = _mockRepository.StrictMock<ITransactionHierarchyManager> ();
      _enlistedObjectManagerMock = _mockRepository.StrictMock<IEnlistedDomainObjectManager> ();
      _invalidDomainObjectManagerMock = _mockRepository.StrictMock<IInvalidDomainObjectManager> ();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _objectLifetimeAgentMock = _mockRepository.StrictMock<IObjectLifetimeAgent> ();
      _queryManagerMock = _mockRepository.StrictMock<IQueryManager> ();
      _commitRollbackAgentMock = _mockRepository.StrictMock<ICommitRollbackAgent> ();
      _fakeExtensions = new ClientTransactionExtensionCollection("test");

      _transactionWithMocks = ClientTransactionObjectMother.CreateWithComponents<TestableClientTransaction> (
          _fakeApplicationData,
          _eventBrokerMock,
          _hierarchyManagerMock,
          _enlistedObjectManagerMock,
          _invalidDomainObjectManagerMock,
          _persistenceStrategyMock,
          _dataManagerMock,
          _objectLifetimeAgentMock,
          _queryManagerMock,
          _commitRollbackAgentMock,
          _fakeExtensions,
          tx => { throw new NotImplementedException(); });
      // Ignore calls made by ctor
      _hierarchyManagerMock.BackToRecord ();
      _eventBrokerMock.BackToRecord ();

      _objectID1 = DomainObjectIDs.Order1;
      _fakeDomainObject1 = DomainObjectMother.CreateFakeObject (_objectID1);
      _objectID2 = DomainObjectIDs.Order2;
      _fakeDomainObject2 = DomainObjectMother.CreateFakeObject (_objectID2);
    }

    [Test]
    public void Initialization_OrderOfFactoryCalls ()
    {
      var componentFactoryMock = _mockRepository.StrictMock<IClientTransactionComponentFactory>();

      var fakeExtensionCollection = new ClientTransactionExtensionCollection ("test");

      ClientTransaction constructedTransaction = null;

      using (_mockRepository.Ordered ())
      {
        componentFactoryMock
            .Expect (mock => mock.CreateApplicationData (Arg<ClientTransaction>.Is.Anything))
            .Return (_fakeApplicationData)
            .WhenCalled (
                mi =>
                {
                  constructedTransaction = (ClientTransaction) mi.Arguments[0];
                  Assert.That (constructedTransaction.ID, Is.Not.EqualTo (Guid.Empty));
                });
        componentFactoryMock
            .Expect (mock => mock.CreateEventBroker (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_eventBrokerMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.ApplicationData, Is.SameAs (_fakeApplicationData)));
        componentFactoryMock
            .Expect (mock => mock.CreateTransactionHierarchyManager (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), Arg.Is (_eventBrokerMock)))
            .Return (_hierarchyManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetEventBroker (constructedTransaction), Is.SameAs (_eventBrokerMock)));
        _hierarchyManagerMock
            .Expect (mock => mock.InstallListeners (_eventBrokerMock));
        componentFactoryMock
            .Expect (mock => mock.CreateEnlistedObjectManager (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (_enlistedObjectManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetHierarchyManager (constructedTransaction) == _hierarchyManagerMock));
        componentFactoryMock
            .Expect (mock => mock.CreateInvalidDomainObjectManager (
                Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction), 
                Arg.Is (_eventBrokerMock)))
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
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock),
                    Arg.Is (_hierarchyManagerMock)))
            .Return (_dataManagerMock)
            .WhenCalled (
                mi => Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (constructedTransaction), Is.SameAs (_persistenceStrategyMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateObjectLifetimeAgent (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_invalidDomainObjectManagerMock),
                    Arg.Is (_dataManagerMock),
                    Arg.Is (_enlistedObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock)))
            .Return (_objectLifetimeAgentMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetIDataManager (constructedTransaction), Is.SameAs (_dataManagerMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateQueryManager (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock), 
                    Arg.Is (_invalidDomainObjectManagerMock), 
                    Arg.Is (_persistenceStrategyMock), 
                    Arg.Is (_dataManagerMock),
                    Arg.Is (_hierarchyManagerMock)))
            .Return (_queryManagerMock)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetObjectLifetimeAgent (constructedTransaction), Is.SameAs (_objectLifetimeAgentMock)));
        componentFactoryMock
            .Expect (
                mock =>
                mock.CreateCommitRollbackAgent (
                    Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction),
                    Arg<IClientTransactionEventSink>.Matches (eventSink => eventSink == _eventBrokerMock),
                    Arg.Is (_persistenceStrategyMock),
                    Arg.Is (_dataManagerMock)))
            .Return (_commitRollbackAgentMock)
            .WhenCalled (mi => Assert.That (constructedTransaction.QueryManager, Is.SameAs (_queryManagerMock)));
        componentFactoryMock
            .Expect (mock => mock.CreateExtensionCollection (Arg<ClientTransaction>.Matches (tx => tx == constructedTransaction)))
            .Return (fakeExtensionCollection)
            .WhenCalled (mi => Assert.That (ClientTransactionTestHelper.GetCommitRollbackAgent (constructedTransaction), Is.SameAs (_commitRollbackAgentMock)));
        _eventBrokerMock
            .Expect (mock => mock.AddListener (Arg<ExtensionClientTransactionListener>.Is.TypeOf))
            .WhenCalled (
                mi =>
                {
                  Assert.That (constructedTransaction.Extensions, Is.SameAs (fakeExtensionCollection));
                  Assert.That (((ExtensionClientTransactionListener) mi.Arguments[0]).Extension, Is.SameAs (constructedTransaction.Extensions)); 
                });

        _hierarchyManagerMock.Expect (mock => mock.OnBeforeTransactionInitialize());
        _eventBrokerMock
            .Expect (mock => mock.RaiseTransactionInitializeEvent());
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
      var fakeParent = ClientTransactionObjectMother.Create();
      _hierarchyManagerMock.Stub (mock => mock.ParentTransaction).Return (fakeParent);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.ParentTransaction, Is.SameAs (fakeParent));
    }

    [Test]
    public void SubTransaction ()
    {
      var fakeSub = ClientTransactionObjectMother.Create ();
      _hierarchyManagerMock.Stub (mock => mock.SubTransaction).Return (fakeSub);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.SubTransaction, Is.SameAs (fakeSub));
    }

    [Test]
    public void RootTransaction_Same ()
    {
      _hierarchyManagerMock.Stub (mock => mock.ParentTransaction).Return (null);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.RootTransaction, Is.SameAs (_transactionWithMocks));
    }

    [Test]
    public void RootTransaction_MultipleSteps ()
    {
      var fakeParent1 = ClientTransactionObjectMother.Create ();
      var fakeParent2 = ClientTransactionObjectMother.CreateWithParent (fakeParent1);
      _hierarchyManagerMock.Stub (mock => mock.ParentTransaction).Return (fakeParent2);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.RootTransaction, Is.SameAs (fakeParent1));
    }

    [Test]
    public void LeafTransaction_Same ()
    {
      _hierarchyManagerMock.Stub (mock => mock.SubTransaction).Return (null);
      _hierarchyManagerMock.Replay ();

      Assert.That (_transactionWithMocks.LeafTransaction, Is.SameAs (_transactionWithMocks));
    }

    [Test]
    public void LeafTransaction_MultipleSteps ()
    {
      var fakeSub1 = ClientTransactionObjectMother.Create ();
      var fakeSub2 = ClientTransactionObjectMother.CreateWithSub (fakeSub1);
      _hierarchyManagerMock.Stub (mock => mock.SubTransaction).Return (fakeSub2);
      _hierarchyManagerMock.Replay();

      Assert.That (_transactionWithMocks.LeafTransaction, Is.SameAs (fakeSub1));
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
    public void CurrentObjectInitializationContext()
    {
      var fakeInitializationContext = MockRepository.GenerateStub<IObjectInitializationContext>();
      _objectLifetimeAgentMock.Expect (mock => mock.CurrentInitializationContext).Return (fakeInitializationContext);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.GetCurrentObjectInitializationContext (_transactionWithMocks);

      _objectLifetimeAgentMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeInitializationContext));
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
    public void GetObject ()
    {
      var includeDeleted = BooleanObjectMother.GetRandomBoolean();
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObject (_objectID1, includeDeleted))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay();

      var result = ClientTransactionTestHelper.CallGetObject (_transactionWithMocks, _objectID1, includeDeleted);

      _objectLifetimeAgentMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void TryGetObject ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.TryGetObject (_objectID1))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallTryGetObject (_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void GetObjectReference ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObjectReference (_objectID1))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallGetObjectReference (_transactionWithMocks, _objectID1);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
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
      ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, _objectID1);
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
      var typeDefinition = GetTypeDefinition (typeof (Order));
      var constructorParameters = ParamList.Create (_fakeDomainObject1);
      _objectLifetimeAgentMock
          .Expect (mock => mock.NewObject (typeDefinition, constructorParameters))
          .Return (_fakeDomainObject1);
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallNewObject (_transactionWithMocks, typeof (Order), constructorParameters);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeDomainObject1));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, true))
          .Return (DataContainerObjectMother.Create());
      _mockRepository.ReplayAll();
      
      _transactionWithMocks.EnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EnsureDataAvailable_Many ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new DataContainer[0]);
      _mockRepository.ReplayAll ();

      _transactionWithMocks.EnsureDataAvailable (new[] { _objectID1, _objectID2 });

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryEnsureDataAvailable_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false))
          .Return (DataContainerObjectMother.Create ());
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false))
          .Return (null);
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (_objectID1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_True ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { DataContainerObjectMother.Create(), DataContainerObjectMother.Create() });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { _objectID1, _objectID2 });

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_False ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { DataContainerObjectMother.Create (), null });
      _mockRepository.ReplayAll ();

      var result = _transactionWithMocks.TryEnsureDataAvailable (new[] { _objectID1, _objectID2 });

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
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_objectID1, "Customer");
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
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (_objectID1);
      Assert.That (_transaction.IsEnlisted (order), Is.False);

      _transaction.EnlistDomainObject (order);

      Assert.That (_transaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_DoesntLoad ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (_objectID1);
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
    [Obsolete ("TODO 2072 - Remove")]
    public void CopyCollectionEventHandlers ()
    {
      var order = _transaction.Execute (() => Order.GetObject (_objectID1));
      
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
    public void CreateSubTransaction_WithDefaultFactory ()
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
      ClientTransaction fakeSubTransaction = ClientTransactionObjectMother.Create();
      Func<ClientTransaction, ClientTransaction> actualFactoryFunc = null;
      _hierarchyManagerMock
          .Expect (mock => mock.CreateSubTransaction (Arg<Func<ClientTransaction, ClientTransaction>>.Is.Anything))
          .WhenCalled (mi => actualFactoryFunc = (Func<ClientTransaction, ClientTransaction>) mi.Arguments[0])
          .Return (fakeSubTransaction);

      ClientTransaction fakeSubTransaction2 = ClientTransactionObjectMother.Create ();
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ITransactionHierarchyManager, IClientTransactionEventSink, ClientTransaction> factoryMock =
        (tx, invalidDomainObjectManager, enlistedDomainObjectManager, hierarchyManager, eventSink) =>
        {
          Assert.That (tx, Is.SameAs (_transactionWithMocks));
          Assert.That (invalidDomainObjectManager, Is.SameAs (_invalidDomainObjectManagerMock));
          Assert.That (enlistedDomainObjectManager, Is.SameAs (_enlistedObjectManagerMock));
          Assert.That (hierarchyManager, Is.SameAs (_hierarchyManagerMock));
          Assert.That (eventSink, Is.SameAs (_eventBrokerMock));
          return fakeSubTransaction2;
        };

      _mockRepository.ReplayAll();
      
      var result = _transactionWithMocks.CreateSubTransaction (factoryMock);

      _hierarchyManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeSubTransaction));

      // Check the actualfactoryFunc that was passed from CreateSubTransaction to _hierarchyManagerMock by invoking it.
      var actualFactoryFuncResult = actualFactoryFunc (_transactionWithMocks);
      Assert.That (actualFactoryFuncResult, Is.SameAs (fakeSubTransaction2));
    }

    [Test]
    public void Discard ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventBrokerMock.RaiseTransactionDiscardEvent();
        _hierarchyManagerMock.Expect (mock => mock.OnTransactionDiscard());
        _eventBrokerMock.Expect (mock => mock.AddListener (Arg<InvalidatedTransactionListener>.Is.TypeOf));
      }
      _mockRepository.ReplayAll();

      Assert.That (_transactionWithMocks.IsDiscarded, Is.False);

      _transactionWithMocks.Discard();

      _mockRepository.VerifyAll();
      Assert.That (_transactionWithMocks.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_Twice ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsActive (parentTransaction, false);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, _transactionWithMocks);

      _eventBrokerMock.Stub (stub => stub.RaiseTransactionDiscardEvent());
      _hierarchyManagerMock.Stub (mock => mock.OnTransactionDiscard ());
      
      _transactionWithMocks.Discard();

      _mockRepository.BackToRecordAll();
      _mockRepository.ReplayAll ();

      _transactionWithMocks.Discard ();

      _eventBrokerMock.AssertWasNotCalled (mock => mock.RaiseTransactionDiscardEvent());
      _hierarchyManagerMock.AssertWasNotCalled (mock => mock.OnTransactionDiscard ());
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

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
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

      ClientTransactionTestHelper.CallGetRelatedObject (
          _transaction, 
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));
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
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

      ClientTransactionTestHelper.CallGetOriginalRelatedObject (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));
      
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
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

      ClientTransactionTestHelper.CallGetRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

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
      Order order = _transaction.Execute (() => Order.GetObject (_objectID1));

      ClientTransactionTestHelper.CallGetOriginalRelatedObjects (
          _transaction,
          RelationEndPointID.Create (order.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetObjects ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.GetObjects<DomainObject> (new[] { _objectID1, _objectID2 }))
          .Return (new[] { _fakeDomainObject1, _fakeDomainObject2 });
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallGetObjects<DomainObject> (_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _fakeDomainObject1, _fakeDomainObject2 }));
    }

    [Test]
    public void TryGetObjects ()
    {
      _objectLifetimeAgentMock
          .Expect (mock => mock.TryGetObjects<DomainObject> (new[] { _objectID1, _objectID2 }))
          .Return (new[] { _fakeDomainObject1, _fakeDomainObject2 });
      _objectLifetimeAgentMock.Replay ();

      var result = ClientTransactionTestHelper.CallTryGetObjects<DomainObject> (_transactionWithMocks, _objectID1, _objectID2);

      _objectLifetimeAgentMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { _fakeDomainObject1, _fakeDomainObject2 }));
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
  }
}