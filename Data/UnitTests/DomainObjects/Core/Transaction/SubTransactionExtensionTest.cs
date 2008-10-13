/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionExtensionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;
    private ClientTransaction _subTransaction;
    private ClientTransactionScope _subTransactionScope;

    private Order _order1;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _extension = _mockRepository.StrictMock<IClientTransactionExtension> ();

      _subTransaction = ClientTransactionMock.CreateSubTransaction ();
      _subTransactionScope = _subTransaction.EnterDiscardingScope ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);

      _subTransaction.Extensions.Add ("TestExtension", _extension);

      _mockRepository.BackToRecordAll ();
    }

    [Test]
    public void ExtensionsAreSameAcrossHierarchy ()
    {
      Assert.IsNotNull (_subTransaction.Extensions);
      Assert.AreSame (_subTransaction.Extensions, _subTransaction.ParentTransaction.Extensions);
    }

    [Test]
    public void NewObjectCreation ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.NewObjectCreating (_subTransaction, typeof (Order));
      }

      _mockRepository.ReplayAll ();

      Order.NewObject ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectLoading ()
    {
      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.Order2);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.Order2);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.NotNull ());
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.NotNull ());
      }

      _mockRepository.ReplayAll ();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll ();
    }

    private void RecordObjectLoadingCalls (ClientTransaction transaction, ObjectID expectedMainObjectID, bool expectingCollection,
          bool expectLoadedEvent, bool expectParentRead, IEnumerable<ObjectID> expectedRelatedObjectIDs)
    {
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extension.ObjectLoading (transaction, expectedMainObjectID);
        _extension.ObjectLoading (transaction.ParentTransaction, expectedMainObjectID);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.NotNull ());
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.NotNull ());

        // accessing relation property

        _extension.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());

        if (expectParentRead)
        {
          _extension.RelationReading (null, null, null, ValueAccess.Current);
          LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());

          foreach (ObjectID relatedID in expectedRelatedObjectIDs)
            _extension.ObjectLoading (transaction.ParentTransaction, relatedID);

          if (expectLoadedEvent)
          {
            _extension.ObjectsLoaded (transaction.ParentTransaction, null);
            LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.Anything ());
          }

          if (expectingCollection)
            _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
          else
            _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);

          LastCall.Constraints (
              Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());
        }

        foreach (ObjectID relatedID in expectedRelatedObjectIDs)
          _extension.ObjectLoading (transaction, relatedID);

        if (!expectParentRead)
        {
          foreach (ObjectID relatedID in expectedRelatedObjectIDs)
            _extension.ObjectLoading (transaction.ParentTransaction, relatedID);

          if (expectLoadedEvent)
          {
            _extension.ObjectsLoaded (transaction.ParentTransaction, null);
            LastCall.Constraints (Mocks_Is.Same (transaction.ParentTransaction), Mocks_Is.Anything ());
          }
        }

        if (expectLoadedEvent)
        {
          _extension.ObjectsLoaded (transaction, null);
          LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything ());
        }

        if (expectingCollection)
          _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);

        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());

        // loading of main object a second time

        // accessing relation property a second time

        _extension.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());


        if (expectingCollection)
          _extension.RelationRead (transaction, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything (), Mocks_Is.Anything ());

      }
    }

    private void TestObjectLoadingWithRelatedObjects (Action accessCode, ObjectID expectedMainObjectID, bool expectCollection, bool expectLoadedEvent,
        bool expectParentRead, IEnumerable<ObjectID> expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll ();
      RecordObjectLoadingCalls (_subTransaction, expectedMainObjectID, expectCollection, expectLoadedEvent, expectParentRead, expectedRelatedIDs);

      _mockRepository.ReplayAll ();

      accessCode ();
      accessCode ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Order order = Order.GetObject (DomainObjectIDs.Order2);
            int orderItemCount = order.OrderItems.Count;
            Assert.AreEqual (1, orderItemCount);
          }, DomainObjectIDs.Order2, true, true, true, new ObjectID[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
            Order order = orderItem.Order;
            Assert.IsNotNull (order);
          }, DomainObjectIDs.OrderItem3, false, true, false, new ObjectID[] { DomainObjectIDs.Order2 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
            Employee employee = computer.Employee;
            Assert.IsNotNull (employee);
          }, DomainObjectIDs.Computer1, false, true, false, new ObjectID[] { DomainObjectIDs.Employee3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
            Computer computer = employee.Computer;
            Assert.IsNotNull (computer);
          }, DomainObjectIDs.Employee3, false, true, true, new ObjectID[] { DomainObjectIDs.Computer1 });
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Official official = Official.GetObject (DomainObjectIDs.Official2);
            int count = official.Orders.Count;
            Assert.AreEqual (0, count);
          }, DomainObjectIDs.Official2, true, false, true, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Client client = Client.GetObject (DomainObjectIDs.Client1);
            Client parent = client.ParentClient;
            Assert.IsNull (parent);
          }, DomainObjectIDs.Client1, false, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
            Employee employee = computer.Employee;
            Assert.IsNull (employee);
          }, DomainObjectIDs.Computer4, false, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
            Computer computer = employee.Computer;
            Assert.IsNull (computer);
          }, DomainObjectIDs.Employee7, false, false, true, new ObjectID[] { });
    }

    [Test]
    public void ObjectsLoaded ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1));
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.Order2);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.Order2);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1));
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));
      }

      _mockRepository.ReplayAll ();

      Order order = Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      ClientTransactionMockEventReceiver clientTransactionEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_subTransaction);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1));
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));

        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));
      }

      _mockRepository.ReplayAll ();

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer;

      computer = Computer.GetObject (DomainObjectIDs.Computer4);

      DomainObjectMockEventReceiver computerEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (_subTransaction, computer);
        computerEventReceiver.Deleting (computer, EventArgs.Empty);
        _extension.ObjectDeleted (_subTransaction, computer);
        computerEventReceiver.Deleted (computer, EventArgs.Empty);
      }

      _mockRepository.ReplayAll ();

      computer.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1;
      OrderItem orderItem2;
      OrderTicket orderTicket;
      Official official;
      Customer customer;
      OrderCollection customerOrders;
      DomainObjectCollection officialOrders;
      Order preloadedOrder1;

      orderItem1 = (OrderItem) _order1.OrderItems[0];
      orderItem2 = (OrderItem) _order1.OrderItems[1];
      orderTicket = _order1.OrderTicket;
      official = _order1.Official;
      customer = _order1.Customer;
      customerOrders = customer.Orders;
      officialOrders = official.Orders;
      preloadedOrder1 = orderTicket.Order;

      DomainObjectMockEventReceiver order1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      DomainObjectMockEventReceiver orderItem1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem1);
      DomainObjectMockEventReceiver orderItem2MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem2);
      DomainObjectMockEventReceiver orderTicketMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);
      DomainObjectMockEventReceiver officialMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (official);
      DomainObjectMockEventReceiver customerMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (customer);

      DomainObjectCollectionMockEventReceiver customerOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (customerOrders);

      DomainObjectCollectionMockEventReceiver officialOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (officialOrders);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (_subTransaction, _order1);

        using (_mockRepository.Unordered ())
        {
          _extension.RelationChanging (_subTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null);
          _extension.RelationChanging (_subTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
          _extension.RelationChanging (_subTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (_subTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (_subTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders", _order1, null);
        }

        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanging (customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null);
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanging (
              orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
          orderItem1MockEventReceiver.RelationChanging (orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          orderItem2MockEventReceiver.RelationChanging (orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          officialMockEventReceiver.RelationChanging (official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders", _order1, null);
          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          LastCall.IgnoreArguments ().Constraints (Mocks_Is.Same (officialOrders), Mocks_Property.Value ("DomainObject", _order1));
        }

        using (_mockRepository.Unordered ())
        {
          _extension.RelationChanged (_subTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
          _extension.RelationChanged (_subTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
          _extension.RelationChanged (_subTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extension.RelationChanged (_subTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extension.RelationChanged (_subTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
        }

        _extension.ObjectDeleted (_subTransaction, _order1);

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanged (customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
          customerOrdersMockEventReceiver.Removed (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanged (orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
          orderItem1MockEventReceiver.RelationChanged (orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          orderItem2MockEventReceiver.RelationChanged (orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          officialMockEventReceiver.RelationChanged (official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
          officialOrdersMockEventReceiver.Removed (officialOrders, _order1);
        }

        order1MockEventReceiver.Deleted (_order1, EventArgs.Empty);
      }

      _mockRepository.ReplayAll ();

      _order1.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingDeletedLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;

      location = Location.GetObject (DomainObjectIDs.Location1);
      deletedClient = location.Client;
      deletedClient.Delete ();
      newClient = Client.NewObject ();

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_subTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extension.RelationChanged (_subTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll ();

      location.Client = newClient;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;

      location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject ();

      deletedClient = location.Client;
      location.Client.Delete ();

      newClient = Client.NewObject ();

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_subTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extension.RelationChanged (_subTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll ();

      location.Client = newClient;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer;
      computer = Computer.GetObject (DomainObjectIDs.Computer4);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectDeleting (_subTransaction, computer);
        _extension.ObjectDeleted (_subTransaction, computer);
      }

      _mockRepository.ReplayAll ();

      computer.Delete ();
      computer.Delete ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_subTransaction);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_subTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (_subTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (_subTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (_subTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      Dev.Null = _order1.OrderNumber;
      Dev.Null =
          (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue =
          _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"];
      ObjectID customerID =
          (ObjectID) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_subTransaction);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueReading (_subTransaction, order1DC, customerPropertyValue, ValueAccess.Current);
        _extension.PropertyValueRead (_subTransaction, order1DC, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      Dev.Null = (ObjectID) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extension);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll ();

      _order1.OrderNumber = orderNumber;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_subTransaction);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueChanging (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);
        _extension.PropertyValueChanged (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);

        _extension.PropertyValueReading (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            newOrderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();
      using (_subTransaction.EnterDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;

        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null =
            (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_subTransaction);

      using (_mockRepository.Ordered ())
      {
        _extension.PropertyValueChanging (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
        _extension.PropertyValueChanged (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();

      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_subTransaction);

      DomainObjectMockEventReceiver domainObjectMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      DataContainerMockEventReceiver dataContainerMockEventReceiver =
          _mockRepository.StrictMock<DataContainerMockEventReceiver> (order1DC);
      PropertyValueCollectionMockEventReceiver propertyValueCollectionMockEventReceiver =
          _mockRepository.StrictMock<PropertyValueCollectionMockEventReceiver> (order1DC.PropertyValues);
      
      using (_mockRepository.Ordered ())
      {
        // "Changing" notifications

        _extension.PropertyValueChanging (_subTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();

        dataContainerMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();

        propertyValueCollectionMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();


        // "Changed" notifications


        propertyValueCollectionMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        dataContainerMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();

        _extension.PropertyValueChanged (_subTransaction, order1DC, order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll ();

      _order1.OrderNumber = oldOrderNumber + 1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extension);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll ();

      using (PersistenceManager persistanceManager = new PersistenceManager ())
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition =
            orderTicketDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        persistanceManager.LoadRelatedDataContainer (
            orderTicket.InternalDataContainer, new RelationEndPointID (orderTicket.ID, orderEndPointDefinition));
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll ();

      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (
            _order1.InternalDataContainer, new RelationEndPointID (_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      orderTicket = _order1.OrderTicket;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Current);
        _extension.RelationRead (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      Dev.Null = _order1.OrderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket;

      originalOrderTicket = (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Original);
        _extension.RelationRead (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      Dev.Null = (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems;
      orderItems = _order1.OrderItems;
      
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0])
            & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = _order1.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems;
      originalOrderItems = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Original);
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0])
            & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll ();

      Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket",
            ValueAccess.Current);
        _extension.RelationReading (_subTransaction.ParentTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket",
            ValueAccess.Current);

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1));

        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
          Mocks_Is.Same (_subTransaction.ParentTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));

        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
          Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll ();

      Dev.Null = _order1.OrderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationReading (_subTransaction.ParentTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction.ParentTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.ObjectLoading (_subTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());
        _extension.ObjectLoading (_subTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());
        
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 2));
        
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll ();

      Dev.Null = _order1.OrderItems;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (
            _subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Original);
        
        _extension.RelationReading (
            _subTransaction.ParentTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Current);

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything());
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1));

        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction.ParentTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.ObjectLoading (_subTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Original);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll ();

      Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (
            _subTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Original);

        _extension.RelationReading (
            _subTransaction.ParentTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything());
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));

        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction.ParentTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Original);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull (),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll ();

      Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);

      _mockRepository.BackToRecord (_extension);

      _extension.FilterQueryResult (null, null, null);
      LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));

      _mockRepository.ReplayAll ();

      ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything());
        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));
        _extension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));

        _extension.ObjectLoading (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Is.Anything ());
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1));
      }

      _mockRepository.ReplayAll ();

      DomainObjectCollection loadedObjects = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      Dev.Null = ((Order) loadedObjects[0]).InternalDataContainer;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);
      IClientTransactionExtension filteringExtension = _mockRepository.StrictMock<ClientTransactionExtensionWithQueryFiltering> ();
      _subTransaction.Extensions.Add ("FilteringExtension", filteringExtension);
      IClientTransactionExtension lastExtension = _mockRepository.StrictMock<IClientTransactionExtension> ();
      _subTransaction.Extensions.Add ("LastExtension", lastExtension);

      using (_mockRepository.Ordered ())
      {
        for (int i = 0; i < 2; ++i)
        {
          _extension.ObjectLoading (null, null);
          LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());
          filteringExtension.ObjectLoading (null, null);
          LastCall.IgnoreArguments ().Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());
          lastExtension.ObjectLoading (null, null);
          LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Is.Anything ());
          }

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));
        filteringExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));
        lastExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2));

        _extension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
        filteringExtension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        lastExtension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_Property.Value ("Count", 1), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll ();

      DomainObjectCollection queryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      Assert.AreEqual (1, queryResult.Count);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer;
      computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionScope.CurrentTransaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer;
      Employee employee;

      computer = Computer.GetObject (DomainObjectIDs.Computer4);
      employee = Employee.GetObject (DomainObjectIDs.Employee1);
      computer.Employee = employee;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionScope.CurrentTransaction.Commit ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer;
      Customer newCustomer;

      oldCustomer = _order1.Customer;
      newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
      _order1.Customer = newCustomer;

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction),
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction),
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll ();


      ClientTransactionScope.CurrentTransaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable ();

      Computer computer;
      
      computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extension);

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_subTransaction);

      DomainObjectMockEventReceiver computerEventReveiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered ())
      {
        computerEventReveiver.Committing (computer, EventArgs.Empty);

        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));

        clientTransactionMockEventReceiver.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        computerEventReveiver.Committed (computer, EventArgs.Empty);

        clientTransactionMockEventReceiver.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_subTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();


      ClientTransactionScope.CurrentTransaction.Commit ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback ()
    {
      Computer computer;

      computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "newSerialNumber";

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RollingBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.IsIn (computer));

        _extension.RolledBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionScope.CurrentTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SubTransactions ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.SubTransactionCreating (_subTransaction);
        _extension.SubTransactionCreated (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_Property.Value ("ParentTransaction", _subTransaction));
      }

      _mockRepository.ReplayAll ();

      _subTransaction.CreateSubTransaction ();
    }

    [Test]
    public void GetObjects ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.Order2);
        _extension.ObjectLoading (_subTransaction, DomainObjectIDs.Order3);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.Order2);
        _extension.ObjectLoading (_subTransaction.ParentTransaction, DomainObjectIDs.Order3);

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction.ParentTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_subTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));
      }

      _mockRepository.ReplayAll ();

      using (_subTransaction.EnterNonDiscardingScope ())
      {
        _subTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      }
    }
  }
}
