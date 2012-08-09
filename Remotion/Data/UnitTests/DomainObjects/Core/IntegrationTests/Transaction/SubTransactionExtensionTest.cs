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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = NUnit.Framework.Is;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionExtensionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extensionMock;
    private ClientTransaction _subTransaction;
    private ClientTransactionScope _subTransactionScope;

    private Order _order1;
    private DataManager _parentTransactionDataManager;
    private DataManager _subTransactionDataManager;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _extensionMock = _mockRepository.StrictMock<IClientTransactionExtension>();

      _subTransaction = TestableClientTransaction.CreateSubTransaction();
      _subTransactionScope = _subTransaction.EnterDiscardingScope();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);

      _extensionMock.Stub (stub => stub.Key).Return ("TestExtension");
      _extensionMock.Replay();
      TestableClientTransaction.Extensions.Add (_extensionMock);
      _subTransaction.Extensions.Add (_extensionMock);
      _extensionMock.BackToRecord();

      _mockRepository.BackToRecordAll();

      _parentTransactionDataManager = ClientTransactionTestHelper.GetDataManager (_subTransaction.ParentTransaction);
      _subTransactionDataManager = ClientTransactionTestHelper.GetDataManager (_subTransaction);
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove ("TestExtension");
      _subTransaction.Extensions.Remove ("TestExtension");
      _subTransactionScope.Leave ();

      base.TearDown();
    }

    [Test]
    public void NewObjectCreation ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.NewObjectCreating (_subTransaction, typeof (Order));
      }

      _mockRepository.ReplayAll();

      Order.NewObject();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoading ()
    {
      _mockRepository.BackToRecordAll();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));
      }

      _mockRepository.ReplayAll();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll();
    }

    private void RecordObjectLoadingCalls (
        ClientTransaction transaction,
        ObjectID expectedMainObjectID,
        bool expectingCollection,
        bool expectLoadedEvent,
        IEnumerable<ObjectID> expectedRelatedObjectIDs)
    {
      using (_mockRepository.Ordered())
      {
        // loading of main object
        _extensionMock.ObjectsLoading (
            Arg.Is (transaction.ParentTransaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedMainObjectID }));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.NotNull());

        _extensionMock.ObjectsLoading (Arg.Is (transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedMainObjectID }));
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.NotNull());

        // accessing relation property

        _extensionMock.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything());

        if (expectedRelatedObjectIDs.Any())
        {
          _extensionMock.ObjectsLoading (
              Arg.Is (transaction.ParentTransaction), Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (expectedRelatedObjectIDs));
        }

        if (expectLoadedEvent)
        {
          _extensionMock.ObjectsLoaded (transaction.ParentTransaction, null);
          LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.Anything());
        }

        if (expectedRelatedObjectIDs.Any())
        {
          _extensionMock.ObjectsLoading (Arg.Is (transaction), Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (expectedRelatedObjectIDs));
        }

        if (expectLoadedEvent)
        {
          _extensionMock.ObjectsLoaded (transaction, null);
          LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything());
        }

        if (expectingCollection)
          _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        else
          _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);

        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything());

        // loading of main object a second time

        // accessing relation property a second time

        _extensionMock.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything());


        if (expectingCollection)
          _extensionMock.RelationRead (transaction, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        else
          _extensionMock.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything(), Mocks_Is.Anything());
      }
    }

    private void TestObjectLoadingWithRelatedObjects (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectCollection,
        bool expectLoadedEvent,
        IEnumerable<ObjectID> expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll();
      RecordObjectLoadingCalls (_subTransaction, expectedMainObjectID, expectCollection, expectLoadedEvent, expectedRelatedIDs);

      _mockRepository.ReplayAll();

      accessCode();
      accessCode();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Order order = Order.GetObject (DomainObjectIDs.Order2);
            int orderItemCount = order.OrderItems.Count;
            Assert.That (orderItemCount, Is.EqualTo (1));
          },
          DomainObjectIDs.Order2,
          true,
          true,
          new[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
            Order order = orderItem.Order;
            Assert.That (order, Is.Not.Null);
          },
          DomainObjectIDs.OrderItem3,
          false,
          true,
          new[] { DomainObjectIDs.Order2 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
            Employee employee = computer.Employee;
            Assert.That (employee, Is.Not.Null);
          },
          DomainObjectIDs.Computer1,
          false,
          true,
          new[] { DomainObjectIDs.Employee3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
            Computer computer = employee.Computer;
            Assert.That (computer, Is.Not.Null);
          },
          DomainObjectIDs.Employee3,
          false,
          true,
          new[] { DomainObjectIDs.Computer1 });
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Official official = Official.GetObject (DomainObjectIDs.Official2);
            int count = official.Orders.Count;
            Assert.That (count, Is.EqualTo (0));
          },
          DomainObjectIDs.Official2,
          true,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Client client = Client.GetObject (DomainObjectIDs.Client1);
            Client parent = client.ParentClient;
            Assert.That (parent, Is.Null);
          },
          DomainObjectIDs.Client1,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
            Employee employee = computer.Employee;
            Assert.That (employee, Is.Null);
          },
          DomainObjectIDs.Computer4,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
            Computer computer = employee.Computer;
            Assert.That (computer, Is.Null);
          },
          DomainObjectIDs.Employee7,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void ObjectsLoaded ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));
      }

      _mockRepository.ReplayAll();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction.ParentTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));
      }

      _mockRepository.ReplayAll();

      Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_subTransaction);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock => mock.ObjectsLoading (
                Arg.Is (_subTransaction.ParentTransaction),
                Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));

        _extensionMock.Expect (
            mock => mock.ObjectsLoading (
                        Arg.Is (_subTransaction),
                        Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        
        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Property.ValueConstraint ("DomainObjects", Property.Value ("Count", 1)));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1)));
      }

      _mockRepository.ReplayAll();

      ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);

      var computerEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_subTransaction, computer);
        computerEventReceiver.Deleting (computer, EventArgs.Empty);
        computerEventReceiver.Deleted (computer, EventArgs.Empty);
        _extensionMock.ObjectDeleted (_subTransaction, computer);
      }

      _mockRepository.ReplayAll();

      computer.Delete();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1 = _order1.OrderItems[0];
      OrderItem orderItem2 = _order1.OrderItems[1];
      OrderTicket orderTicket = _order1.OrderTicket;
      Official official = _order1.Official;
      Customer customer = _order1.Customer;
      OrderCollection customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete ();
      ObjectList<Order> officialOrders = official.Orders;
      officialOrders.EnsureDataComplete ();
      Dev.Null = orderTicket.Order; // preload

      var order1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var orderItem1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem1);
      var orderItem2MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem2);
      var orderTicketMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);
      var officialMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (official);
      var customerMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (customer);

      var customerOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (customerOrders);

      var officialOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (officialOrders);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_subTransaction, _order1);
        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered ())
        {
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          _extensionMock.RelationChanging (_subTransaction, customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          customerMockEventReceiver.RelationChanging (customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);

          _extensionMock.RelationChanging (_subTransaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
          orderTicketMockEventReceiver.RelationChanging (orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

          _extensionMock.RelationChanging (_subTransaction, orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          orderItem1MockEventReceiver.RelationChanging (orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          _extensionMock.RelationChanging (_subTransaction, orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          orderItem2MockEventReceiver.RelationChanging (orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          _extensionMock.RelationChanging (_subTransaction, official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
          officialMockEventReceiver.RelationChanging (official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
        }

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanged (customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          _extensionMock.RelationChanged (_subTransaction, customer, GetEndPointDefinition (typeof (Customer), "Orders"), _order1, null);
          customerOrdersMockEventReceiver.Removed (customerOrders, _order1);

          orderTicketMockEventReceiver.RelationChanged (orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);
          _extensionMock.RelationChanged (_subTransaction, orderTicket, GetEndPointDefinition (typeof (OrderTicket), "Order"), _order1, null);

          orderItem1MockEventReceiver.RelationChanged (orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          _extensionMock.RelationChanged (_subTransaction, orderItem1, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          orderItem2MockEventReceiver.RelationChanged (orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);
          _extensionMock.RelationChanged (_subTransaction, orderItem2, GetEndPointDefinition (typeof (OrderItem), "Order"), _order1, null);

          officialOrdersMockEventReceiver.Removed (officialOrders, _order1);
          officialMockEventReceiver.RelationChanged (official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
          _extensionMock.RelationChanged (_subTransaction, official, GetEndPointDefinition (typeof (Official), "Orders"), _order1, null);
        }

        order1MockEventReceiver.Deleted (_order1, EventArgs.Empty);
        _extensionMock.ObjectDeleted (_subTransaction, _order1);
      }

      _mockRepository.ReplayAll();

      _order1.Delete();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingDeletedLoaded ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      Client deletedClient = location.Client;
      deletedClient.Delete();

      Client newClient = Client.NewObject();

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_subTransaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
        _extensionMock.RelationChanged (_subTransaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
      }

      _mockRepository.ReplayAll();

      location.Client = newClient;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject();

      Client deletedClient = location.Client;
      location.Client.Delete();

      Client newClient = Client.NewObject();

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_subTransaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
        _extensionMock.RelationChanged (_subTransaction, location, GetEndPointDefinition (typeof (Location), "Client"), deletedClient, newClient);
      }

      _mockRepository.ReplayAll();

      location.Client = newClient;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_subTransaction, computer);
        _extensionMock.ObjectDeleted (_subTransaction, computer);
      }

      _mockRepository.ReplayAll();

      computer.Delete();
      computer.Delete();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Current);
        _extensionMock.PropertyValueRead (
            _subTransaction,
            _order1,
            propertyDefinition,
            orderNumber,
            ValueAccess.Current);
        _extensionMock.PropertyValueReading (
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Original);
        _extensionMock.PropertyValueRead (
            _subTransaction,
            _order1,
            propertyDefinition,
            orderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetOriginalValueWithoutTypeCheck();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      var propertyDefinition = GetPropertyDefinition (typeof (Order), "Customer");
      var customerID = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID();

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (_subTransaction, _order1, propertyDefinition, ValueAccess.Current);
        _extensionMock.PropertyValueRead (_subTransaction, _order1, propertyDefinition, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.Properties[propertyDefinition.PropertyName].GetRelatedObjectID ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extensionMock);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll();

      _order1.OrderNumber = orderNumber;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            newOrderNumber);
        _extensionMock.PropertyValueChanged (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            newOrderNumber);

        _extensionMock.PropertyValueReading (
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Current);
        _extensionMock.PropertyValueRead (
            _subTransaction,
            _order1,
            propertyDefinition,
            newOrderNumber,
            ValueAccess.Current);
        _extensionMock.PropertyValueReading (
            _subTransaction,
            _order1,
            propertyDefinition,
            ValueAccess.Original);
        _extensionMock.PropertyValueRead (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      _order1.OrderNumber = newOrderNumber;
      Dev.Null = _order1.OrderNumber;
      Dev.Null = _order1.Properties[typeof (Order), "OrderNumber"].GetOriginalValueWithoutTypeCheck ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
        _extensionMock.PropertyValueChanged (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      var domainObjectMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      using (_mockRepository.Ordered())
      {
        // "Changing" notifications

        _extensionMock.PropertyValueChanging (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();



        // "Changed" notifications


        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        _extensionMock.PropertyValueChanged (
            _subTransaction,
            _order1,
            propertyDefinition,
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket = _order1.OrderTicket;

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (_subTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderTicket"), ValueAccess.Current);
        _extensionMock.RelationRead (_subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.OrderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      var originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (_subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), ValueAccess.Original);
        _extensionMock.RelationRead (_subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderTicket"), originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Current);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0]) & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.OrderItems;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems =
          _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Original);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0]) & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll();

      Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction,
            _order1,
            GetEndPointDefinition (typeof(Order), "OrderTicket"),
            ValueAccess.Current);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
          Arg.Is (_subTransaction.ParentTransaction),
          Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Property.Value ("Count", 1));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Property.Value ("Count", 1));

        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      Dev.Null = _order1.OrderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction, 
            _order1,
            GetEndPointDefinition (typeof (Order), "OrderItems"),
            ValueAccess.Current);

        _extensionMock.ObjectsLoading (_subTransaction.ParentTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Property.Value ("Count", 2));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Property.Value ("Count", 2));

        _extensionMock.ObjectsLoading (_subTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Property.Value ("Count", 2));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Property.Value ("Count", 2));

        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      Dev.Null = _order1.OrderItems;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction, _order1, GetEndPointDefinition (typeof(Order), "OrderTicket"), ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Property.Value ("Count", 1));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Property.Value ("Count", 1));
        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Original);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderTicket")),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _subTransaction, _order1, GetEndPointDefinition (typeof (Order), "OrderItems"), ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));
        
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Property.Value ("Count", 2));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Property.Value ("Count", 2));
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Original);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal (GetEndPointDefinition (typeof (Order), "OrderItems")),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      // preload query results to avoid Load notifications later on
      LifetimeService.GetObject (_subTransaction, DomainObjectIDs.Order1, true);
      LifetimeService.GetObject (_subTransaction, DomainObjectIDs.OrderWithoutOrderItem, true);

      _mockRepository.BackToRecord (_extensionMock);

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject> (new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject> ();

      _extensionMock
          .Expect (mock => mock.FilterQueryResult (
              Arg.Is (_subTransaction.ParentTransaction), 
              Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
          .Return (parentFilteredQueryResult);
      _extensionMock
          .Expect (mock => mock.FilterQueryResult (
            Arg.Is (_subTransaction),
            Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 1 && qr.Query == query)))
          .Return (subFilteredQueryResult);

      _mockRepository.ReplayAll();

      QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      Assert.That (finalResult, NUnit.Framework.Is.SameAs (subFilteredQueryResult));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4); // yields Order3, Order4

      QueryResult<DomainObject> parentFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject> (new[] { _order1 });
      QueryResult<DomainObject> subFilteredQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject> ();

      UnloadService.UnloadData (_subTransaction, _order1.ID); // unload _order1 to force Load events
      ClientTransactionTestHelper.SetIsActive (TestableClientTransaction, true);
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1); // we only want Load events in the sub-transaction
      ClientTransactionTestHelper.SetIsActive (TestableClientTransaction, false);

      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.SetEquals (new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 }))));
        _extensionMock.ObjectsLoaded (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 2));
        _extensionMock
            .Expect (mock => mock.FilterQueryResult (
                Arg.Is (_subTransaction.ParentTransaction), 
                Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
            .Return (parentFilteredQueryResult);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.SetEquals (new[] { DomainObjectIDs.Order1 }))));
        _extensionMock.ObjectsLoaded (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1));
        _extensionMock
            .Expect (
            mock =>
            mock.FilterQueryResult (
                Arg.Is (_subTransaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 1 && qr.Query == query)))
            .Return (subFilteredQueryResult);
      }

      _mockRepository.ReplayAll();

      QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      Assert.That (finalQueryResult, NUnit.Framework.Is.SameAs (subFilteredQueryResult));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_subTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (mock => mock.CommitValidate (
            Arg.Is (_subTransaction), 
            Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer }))));
        _extensionMock.Expect (mock => mock.Committed (Arg.Is (_subTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer })));
      }

      _mockRepository.ReplayAll();

      _subTransaction.Commit ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      computer.Employee = employee;

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_subTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (
            mock =>
            mock.CommitValidate (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { computer, employee }))));
        _extensionMock.Expect (
            mock =>
            mock.Committed (Arg.Is (_subTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { computer, employee })));
      }

      _mockRepository.ReplayAll();

      _subTransaction.Commit();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer = _order1.Customer;
      Customer newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
      _order1.Customer = newCustomer;

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_subTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        _extensionMock.Expect (
            mock =>
            mock.CommitValidate (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new DomainObject[] { _order1, newCustomer, oldCustomer }))));
        _extensionMock.Expect (
            mock =>
            mock.Committed (
                Arg.Is (_subTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new DomainObject[] { _order1, newCustomer, oldCustomer })));
      }

      _mockRepository.ReplayAll();
      
      _subTransactionScope.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extensionMock);

      var clientTransactionMockEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_subTransaction);

      var computerEventReveiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.Committing (
                Arg.Is (_subTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer }),
                Arg<CommittingEventRegistrar>.Is.TypeOf));
        clientTransactionMockEventReceiver.Expect (mock => mock.Committing (_subTransaction, computer));
        computerEventReveiver.Expect (mock => mock.Committing (computer));

        _extensionMock.Expect (mock => mock.CommitValidate (
            Arg.Is (_subTransaction), 
            Arg<ReadOnlyCollection<PersistableData>>.Matches (c => c.Select (d => d.DomainObject).SetEquals (new[] { computer }))));

        computerEventReveiver.Expect (mock => mock.Committed (computer));
        clientTransactionMockEventReceiver.Expect (mock => mock.Committed (_subTransaction, computer));
        _extensionMock.Expect (mock => mock.Committed (Arg.Is (_subTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { computer })));
      }

      _mockRepository.ReplayAll();

      _subTransaction.Commit();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RollingBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.IsIn (computer));

        _extensionMock.RolledBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      ClientTransactionScope.CurrentTransaction.Rollback();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetObjects ()
    {
      using (_mockRepository.Ordered())
      {
        // parent transaction first, just like persistence manager comes first in root transactions (ie. persistence manager loads data containers 
        // before any events are raised)

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction.ParentTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2, DomainObjectIDs.Order3 })));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_subTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2, DomainObjectIDs.Order3 })));
        
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));
      }

      _mockRepository.ReplayAll();

      using (_subTransaction.EnterNonDiscardingScope())
      {
        _subTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnloadData ()
    {
      using (_mockRepository.Ordered ())
      {
        _extensionMock
            .Expect (mock => mock.ObjectsUnloading (
                        Arg.Is (_subTransaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_subTransactionDataManager.DataContainers[_order1.ID] != null));
        _extensionMock
            .Expect (mock => mock.ObjectsUnloading (
                        Arg.Is (_subTransaction.ParentTransaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_parentTransactionDataManager.DataContainers[_order1.ID] != null));

        _extensionMock
            .Expect (mock => mock.ObjectsUnloaded (
                        Arg.Is (_subTransaction.ParentTransaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_parentTransactionDataManager.DataContainers[_order1.ID] == null));
        _extensionMock
            .Expect (mock => mock.ObjectsUnloaded (
                        Arg.Is (_subTransaction),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (_subTransactionDataManager.DataContainers[_order1.ID] == null));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadData (_subTransaction, _order1.ID);

      _mockRepository.VerifyAll ();
    }

  }
}
