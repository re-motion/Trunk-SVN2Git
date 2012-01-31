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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionListener _strictListenerMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      _strictListenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
    }

    [TearDown]
    public override void TearDown ()
    {
      _strictListenerMock.BackToRecord();
      base.TearDown();
    }

    [Test]
    public void TransactionInitialize ()
    {
      ClientTransaction inititalizedTransaction = null;

      _strictListenerMock
          .Expect (mock => mock.TransactionInitialize (Arg<ClientTransaction>.Is.Anything))
          .WhenCalled (mi => inititalizedTransaction = (ClientTransaction) mi.Arguments[0]);
      _strictListenerMock.Replay ();

      var result = ClientTransactionObjectMother.CreateWithCustomListeners (_strictListenerMock);

      _strictListenerMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (inititalizedTransaction));
    }

    [Test]
    public void TransactionDiscard ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      _strictListenerMock.Expect (mock => mock.TransactionDiscard (TestableClientTransaction));
      
      _mockRepository.ReplayAll ();

      TestableClientTransaction.Discard();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionDiscard_OnlyFiresIfTransactionIsNotYetDiscarded ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      _strictListenerMock.Expect (mock => mock.TransactionDiscard (TestableClientTransaction));

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Discard ();
      TestableClientTransaction.Discard ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.NewObjectCreating (
                Arg.Is (TestableClientTransaction), 
                Arg.Is (typeof (ClassWithAllDataTypes)), 
                Arg<DomainObject>.Matches (obj => obj != null && obj.ID == null)));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg.Is (TestableClientTransaction), Arg<DataContainer>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.NewObject ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
          Arg.Is (TestableClientTransaction), 
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg.Is (TestableClientTransaction), Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeleted ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
          _strictListenerMock.Expect (mock => mock.ObjectDeleting (TestableClientTransaction, cwadt));
          _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, cwadt.InternalDataContainer, StateType.Deleted));
          _strictListenerMock.Expect (mock => mock.ObjectDeleted (TestableClientTransaction, cwadt));
      }

      _mockRepository.ReplayAll ();

      cwadt.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueReading (TestableClientTransaction, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueRead (TestableClientTransaction, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = order.OrderNumber;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (TestableClientTransaction, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        43));
        _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, order.InternalDataContainer, StateType.Changed));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (TestableClientTransaction, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        43));
      }

      _mockRepository.ReplayAll ();

      order.OrderNumber = 43;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;

      TestableClientTransaction.AddListener (_strictListenerMock);

      IRelationEndPointDefinition customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      IRelationEndPointDefinition orderItemsEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.RelationReading (TestableClientTransaction, order, customerEndPointDefinition, ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (TestableClientTransaction, order, customerEndPointDefinition, customer, ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationReading (TestableClientTransaction, order, orderItemsEndPointDefinition, ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (TestableClientTransaction), 
                Arg.Is (order), 
                Arg.Is (orderItemsEndPointDefinition),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Matches (domainObjects => domainObjects.SequenceEqual (orderItems.Cast<DomainObject> ())),
                Arg.Is (ValueAccess.Current)));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = order.Customer;
      Dev.Null = order.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChangingRelationChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer oldCustomer = order.Customer;
      Customer newCustomer = Customer.NewObject();
      
      // preload all related objects
      oldCustomer.Orders.EnsureDataComplete ();

      var oldCustomerEndPointID = oldCustomer.Orders.AssociatedEndPointID;
      var newCustomerEndPointID = newCustomer.Orders.AssociatedEndPointID;

      IRelationEndPointDefinition customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      
      TestableClientTransaction.AddListener (_strictListenerMock);
      
      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            TestableClientTransaction,
            order, 
            customerEndPointDefinition,
            oldCustomer, 
            newCustomer));
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, 
            newCustomer, 
            newCustomerEndPointID.Definition,
            null, 
            order));
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            TestableClientTransaction, 
            oldCustomer,
            oldCustomerEndPointID.Definition,
            order, 
            null));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (
                TestableClientTransaction, 
                order.InternalDataContainer,
                order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                oldCustomer.ID,
                newCustomer.ID));
        _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, order.InternalDataContainer, StateType.Changed));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (
                TestableClientTransaction, 
                order.InternalDataContainer,
                order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                oldCustomer.ID,
                newCustomer.ID));
        _strictListenerMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (TestableClientTransaction, newCustomerEndPointID, null));
        _strictListenerMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (TestableClientTransaction, oldCustomerEndPointID, null));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            TestableClientTransaction, 
            oldCustomer, oldCustomerEndPointID.Definition, order, null));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            TestableClientTransaction, 
            newCustomer, newCustomerEndPointID.Definition, null, order));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            TestableClientTransaction, 
            order, customerEndPointDefinition, oldCustomer, newCustomer));
      }

      _mockRepository.ReplayAll ();

      order.Customer = newCustomer;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");
      var orders = (OrderCollection) TestableClientTransaction.QueryManager.GetCollection (query).ToCustomCollection ();

      TestableClientTransaction.AddListener (_strictListenerMock);

      var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _strictListenerMock
          .Expect (mock => mock.FilterQueryResult (
              Arg.Is (TestableClientTransaction), 
              Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == orders.Count)))
          .Return (newQueryResult);

      _mockRepository.ReplayAll ();

      var result = TestableClientTransaction.QueryManager.GetCollection (query);
      Assert.That (result, Is.SameAs (newQueryResult));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionCommitting (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { order })));
        _strictListenerMock.Expect (mock => mock.TransactionCommitValidate (
            Arg.Is (TestableClientTransaction),
            Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new[] { order }))));
        _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, order.InternalDataContainer, StateType.Unchanged));
        _strictListenerMock.Expect (mock => mock.TransactionCommitted (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { order })));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionRollingBack (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
        _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, order.InternalDataContainer, StateType.Unchanged));
        _strictListenerMock.Expect (mock => mock.TransactionRolledBack (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg.Is (TestableClientTransaction), Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (
            mock => mock.RelationEndPointMapRegistering (
                Arg.Is (TestableClientTransaction), 
                Arg<IRelationEndPoint>.Matches (
                    rep => rep.Definition.PropertyName == typeof (Company).FullName + ".IndustrialSector" && rep.ObjectID == DomainObjectIDs.Customer1)));
        _strictListenerMock.Expect (
            mock => mock.RelationEndPointMapRegistering (
                Arg.Is (TestableClientTransaction),
                Arg<IRelationEndPoint>.Matches (
                    rep => rep.Definition.PropertyName == typeof (IndustrialSector).FullName + ".Companies" && rep.ObjectID == DomainObjectIDs.IndustrialSector1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      Customer.GetObject (DomainObjectIDs.Customer1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject ();
      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (order.ID, "OrderTicket");
      var orderItemEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (order.ID, "OrderItems");

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectDeleting (TestableClientTransaction, order));
        _strictListenerMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (TestableClientTransaction, orderTicketEndPointID, false));
        _strictListenerMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (TestableClientTransaction, orderItemEndPointID, false));

        _strictListenerMock
            .Expect (mock => mock.RelationEndPointMapUnregistering (
                Arg.Is (TestableClientTransaction), 
                Arg<RelationEndPointID>.Matches (id => id.ObjectID == order.ID)))
            .Repeat.Times (4); // four related objects/object collections in Order

        _strictListenerMock.Expect (mock => mock.DataContainerMapUnregistering (TestableClientTransaction, order.InternalDataContainer));
        _strictListenerMock.Expect (mock => mock.DataContainerStateUpdated (TestableClientTransaction, order.InternalDataContainer, StateType.Invalid));
        _strictListenerMock.Expect (mock => mock.ObjectMarkedInvalid (TestableClientTransaction, order));
        _strictListenerMock.Expect (mock => mock.ObjectDeleted (TestableClientTransaction, order));
      }

      _mockRepository.ReplayAll ();

      order.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _strictListenerMock.Expect (
            mock => mock.DataContainerMapRegistering (
                Arg.Is (TestableClientTransaction), 
                Arg<DataContainer>.Matches (dc => dc.ID == DomainObjectIDs.ClassWithAllDataTypes1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (TestableClientTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionCreating_AndSubTransactionCreated ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      ClientTransaction initializedTransaction = null;

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.SubTransactionCreating (TestableClientTransaction));
        _strictListenerMock
            .Expect (mock => mock.SubTransactionInitialize (
                Arg.Is (TestableClientTransaction),
                Arg<ClientTransaction>.Matches (tx => tx != null && tx != TestableClientTransaction && tx.ParentTransaction == TestableClientTransaction)))
            .WhenCalled (mi => initializedTransaction = (ClientTransaction) mi.Arguments[1]);
        _strictListenerMock.Expect (mock => mock.SubTransactionCreated (
            Arg.Is (TestableClientTransaction), 
            Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
      }

      _mockRepository.ReplayAll ();

      var result = TestableClientTransaction.CreateSubTransaction();

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (initializedTransaction));
    }

    [Test]
    public void SubTransactionInitialize_AndTransactionInitialize_AndSubTransactionCreated ()
    {
      TestableClientTransaction.AddListener (_strictListenerMock);

      ClientTransaction initializedTransaction = null;

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.SubTransactionCreating (TestableClientTransaction));
        _strictListenerMock
            .Expect (mock => mock.SubTransactionInitialize (
                Arg.Is (TestableClientTransaction),
                Arg<ClientTransaction>.Matches (tx => tx != null && tx != TestableClientTransaction && tx.ParentTransaction == TestableClientTransaction)))
            .WhenCalled (mi =>
            {
              initializedTransaction = (ClientTransaction) mi.Arguments[1];
              ClientTransactionTestHelper.AddListener (initializedTransaction, _strictListenerMock);
            });
        _strictListenerMock.Expect (mock => mock.TransactionInitialize (Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
        _strictListenerMock.Expect (mock => mock.SubTransactionCreated (
            Arg.Is (TestableClientTransaction),
            Arg<ClientTransaction>.Matches (tx => tx == initializedTransaction)));
      }

      _mockRepository.ReplayAll ();

      var result = TestableClientTransaction.CreateSubTransaction ();

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (initializedTransaction));
    }

    [Test]
    public void ObjectsUnloadingObjectsUnloaded ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      var orderEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (orderTicket1.ID, "Order");
      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (orderTicket1.Order.ID, "OrderTicket");

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket1 })))
            .WhenCalled (mi => Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged)));
        using (_mockRepository.Unordered ())
        {
          _strictListenerMock
              .Expect (mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, orderEndPointID));
          _strictListenerMock
              .Expect (mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, orderTicketEndPointID));
          _strictListenerMock
              .Expect (mock => mock.RelationEndPointUnloading (TestableClientTransaction, orderTicketEndPointID));
          _strictListenerMock
              .Expect (mock => mock.DataContainerMapUnregistering (TestableClientTransaction, orderTicket1.InternalDataContainer));
        }
        _strictListenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (TestableClientTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket1 })))
            .WhenCalled (mi => Assert.That (orderTicket1.State, Is.EqualTo (StateType.NotLoadedYet)));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadData (TestableClientTransaction, orderTicket1.ID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointUnload ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order1.OrderItems);

      Dev.Null = orderItemsEndPoint.HasChanged; // warm up has changed cache

      TestableClientTransaction.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.RelationEndPointUnloading (TestableClientTransaction, orderItemsEndPoint.ID));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, orderItemsEndPoint.ID);

      _mockRepository.VerifyAll ();
    }
  }
}
