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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using NUnit.Framework.SyntaxHelpers;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
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
    [Ignore ("TODO: COMMONS-1973")]
    public void TransactionInitializing ()
    {
      
    }

    [Test]
    public void TransactionDiscarding ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      _strictListenerMock.Expect (mock => mock.TransactionDiscarding (ClientTransactionMock));
      
      _mockRepository.ReplayAll ();

      ClientTransactionMock.Discard();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionDiscarding_OnlyFiresIfTransactionIsNotYetDiscarded ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      _strictListenerMock.Expect (mock => mock.TransactionDiscarding (ClientTransactionMock));

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Discard ();
      ClientTransactionMock.Discard ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.NewObjectCreating (
                Arg.Is (ClientTransactionMock), 
                Arg.Is (typeof (ClassWithAllDataTypes)), 
                Arg<DomainObject>.Matches (obj => obj != null && obj.ID == null)));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.NewObject ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadingInitializedObjectsLoaded ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
          Arg.Is (ClientTransactionMock), 
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (ClientTransactionMock), 
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
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
          _strictListenerMock.Expect (mock => mock.ObjectDeleting (ClientTransactionMock, cwadt));
          _strictListenerMock.Expect (mock => mock.ObjectDeleted (ClientTransactionMock, cwadt));
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

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueReading (ClientTransactionMock, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueRead (ClientTransactionMock, order.InternalDataContainer,
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

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (ClientTransactionMock, order.InternalDataContainer,
                        order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"],
                        orderNumber,
                        43));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (ClientTransactionMock, order.InternalDataContainer,
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

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (
            mock => mock.RelationReading (ClientTransactionMock, order, typeof (Order).FullName + ".Customer", ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (ClientTransactionMock, order, typeof (Order).FullName + ".Customer", customer, ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationReading (ClientTransactionMock, order, typeof (Order).FullName + ".OrderItems", ValueAccess.Current));
        _strictListenerMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (ClientTransactionMock), 
                Arg.Is (order), 
                Arg.Is (typeof (Order).FullName + ".OrderItems"),
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
      foreach (var otherOrder in oldCustomer.Orders)
        Dev.Null = otherOrder;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, 
            order, 
            typeof (Order).FullName + ".Customer", 
            oldCustomer, 
            newCustomer));
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, 
            newCustomer, 
            typeof (Customer).FullName + ".Orders", 
            null, 
            order));
        _strictListenerMock.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, 
            oldCustomer, 
            typeof (Customer).FullName + ".Orders", 
            order, 
            null));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanging (
                ClientTransactionMock, 
                order.InternalDataContainer,
                order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                oldCustomer.ID,
                newCustomer.ID));
        _strictListenerMock.Expect (
            mock => mock.PropertyValueChanged (
                ClientTransactionMock, 
                order.InternalDataContainer,
                order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".Customer"],
                oldCustomer.ID,
                newCustomer.ID));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, 
            oldCustomer, 
            typeof (Customer).FullName + ".Orders"));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, 
            newCustomer, 
            typeof (Customer).FullName + ".Orders"));
        _strictListenerMock.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, 
            order, 
            typeof (Order).FullName + ".Customer"));
      }

      _mockRepository.ReplayAll ();

      order.Customer = newCustomer;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");
      var orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query).ToCustomCollection ();

      ClientTransactionMock.AddListener (_strictListenerMock);

      var newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _strictListenerMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == orders.Count)))
          .Return (newQueryResult);

      _mockRepository.ReplayAll ();

      var result = ClientTransactionMock.QueryManager.GetCollection (query);
      Assert.That (result, Is.SameAs (newQueryResult));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionCommitting (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
        _strictListenerMock.Expect (mock => mock.TransactionCommitted (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.TransactionRollingBack (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
        _strictListenerMock.Expect (mock => mock.TransactionRolledBack (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (doc => doc.Count == 1)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        _strictListenerMock.Expect (mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Is.Anything));
        _strictListenerMock.Expect (
            mock => mock.RelationEndPointMapRegistering (
                        Arg<RelationEndPoint>.Matches (
                            rep => rep.PropertyName == typeof (Company).FullName + ".IndustrialSector" && rep.ObjectID == DomainObjectIDs.Customer1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (ClientTransactionMock), 
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

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectDeleting (ClientTransactionMock, order));

        _strictListenerMock
            .Expect (mock => mock.RelationEndPointMapUnregistering (Arg<RelationEndPointID>.Matches (id => id.ObjectID == order.ID)))
            .Repeat.Times (4); // four related objects/object collections in Order

        _strictListenerMock.Expect (mock => mock.DataContainerMapUnregistering (order.InternalDataContainer));
        _strictListenerMock.Expect (mock => mock.DataManagerMarkingObjectInvalid (order.ID));
        _strictListenerMock.Expect (mock => mock.ObjectDeleted (ClientTransactionMock, order));
      }

      _mockRepository.ReplayAll ();

      order.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _strictListenerMock.Expect (
            mock => mock.DataContainerMapRegistering (Arg<DataContainer>.Matches (dc => dc.ID == DomainObjectIDs.ClassWithAllDataTypes1)));

        _strictListenerMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactionCreatingSubTransactionCreated ()
    {
      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.SubTransactionCreating (ClientTransactionMock));
        _strictListenerMock.Expect (
            mock =>
            mock.SubTransactionCreated (
              Arg.Is (ClientTransactionMock), 
              Arg<ClientTransaction>.Matches (tx => tx != null && tx != ClientTransactionMock && tx.ParentTransaction == ClientTransactionMock)));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.CreateSubTransaction();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsUnloadingObjectsUnloaded ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      var orderEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (orderTicket1.ID, "Order");
      var orderTicketEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (orderTicket1.Order.ID, "OrderTicket");

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (ClientTransactionMock), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket1 })))
            .WhenCalled (mi => Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged)));
        using (_mockRepository.Unordered ())
        {
          _strictListenerMock
              .Expect (mock => mock.RelationEndPointMapUnregistering (orderEndPointID));
          _strictListenerMock
              .Expect (mock => mock.RelationEndPointMapUnregistering (orderTicketEndPointID));
          _strictListenerMock
              .Expect (mock => mock.DataContainerMapUnregistering (orderTicket1.InternalDataContainer));
        }
        _strictListenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (ClientTransactionMock), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket1 })))
            .WhenCalled (mi => Assert.That (orderTicket1.State, Is.EqualTo (StateType.NotLoadedYet)));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadData (ClientTransactionMock, orderTicket1.ID, UnloadTransactionMode.ThisTransactionOnly);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationEndPointUnload ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order1.OrderItems);

      ClientTransactionMock.AddListener (_strictListenerMock);

      using (_mockRepository.Ordered ())
      {
        _strictListenerMock.Expect (mock => mock.RelationEndPointUnloading ((RelationEndPoint) orderItemsEndPoint));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock, 
          orderItemsEndPoint.ID, 
          UnloadTransactionMode.ThisTransactionOnly);

      _mockRepository.VerifyAll ();
    }
  }
}
