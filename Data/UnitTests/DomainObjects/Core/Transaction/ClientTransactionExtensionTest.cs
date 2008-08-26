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
  public class ClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;
    private ClientTransactionMock _newTransaction;

    private Order _order1;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _mockRepository = new MockRepository ();
      _extension = _mockRepository.StrictMock<IClientTransactionExtension> ();

      ClientTransactionScope.CurrentTransaction.Extensions.Add ("TestExtension", _extension);

      _newTransaction = new ClientTransactionMock ();
      _newTransaction.Extensions.Add ("TestExtension", _extension);

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.OrderNumber; // preload _order1
      }
      _mockRepository.BackToRecordAll ();
    }

    [Test]
    public void Extensions ()
    {
      Assert.IsNotNull (ClientTransactionScope.CurrentTransaction.Extensions);
    }

    [Test]
    public void NewObjectCreation ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.NewObjectCreating (_newTransaction, typeof (Order));
      }

      _mockRepository.ReplayAll ();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        Order.NewObject();
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectLoading ()
    {
      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered())
      {
        _extension.ObjectLoading (_newTransaction, DomainObjectIDs.Order2);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Is.NotNull());
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        Dev.Null = Order.GetObject (DomainObjectIDs.Order2);
        Dev.Null = Order.GetObject (DomainObjectIDs.Order2);
      }

      _mockRepository.VerifyAll ();
    }

    private void RecordObjectLoadingCalls (ClientTransaction transaction, ObjectID expectedMainObjectID, bool expectingCollection,
          bool expectLoadedEvent, IEnumerable<ObjectID> expectedRelatedObjectIDs)
    {
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extension.ObjectLoading (transaction, expectedMainObjectID);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.NotNull ());

        // accessing relation property

        _extension.RelationReading (null, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        foreach (ObjectID relatedID in expectedRelatedObjectIDs)
          _extension.ObjectLoading (transaction, relatedID);

        if (expectLoadedEvent)
        {
          _extension.ObjectsLoaded (transaction, null);
          LastCall.IgnoreArguments();
        }

        if (expectingCollection)
          _extension.RelationRead (transaction, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);

        LastCall.IgnoreArguments();

        // loading of main object a second time

        // accessing relation property a second time

        _extension.RelationReading (transaction, null, null, ValueAccess.Current);
        LastCall.IgnoreArguments();

        if (expectingCollection)
          _extension.RelationRead (transaction, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        else
          _extension.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments();
      }
    }

    private void TestObjectLoadingWithRelatedObjects (Action accessCode, ObjectID expectedMainObjectID, bool expectCollection, bool expectLoadedEvent,
        IEnumerable<ObjectID> expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll ();
      RecordObjectLoadingCalls (_newTransaction, expectedMainObjectID, expectCollection, expectLoadedEvent, expectedRelatedIDs);

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        _mockRepository.ReplayAll ();

        accessCode ();
        accessCode ();

        _mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Order order = Order.GetObject (DomainObjectIDs.Order2);
            int orderItemCount = order.OrderItems.Count;
            Assert.AreEqual (1, orderItemCount);
          }, DomainObjectIDs.Order2, true, true, new ObjectID[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
            Order order = orderItem.Order;
            Assert.IsNotNull (order);
          }, DomainObjectIDs.OrderItem3, false, true, new ObjectID[] { DomainObjectIDs.Order2 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
            Employee employee = computer.Employee;
            Assert.IsNotNull (employee);
          }, DomainObjectIDs.Computer1, false, true, new ObjectID[] { DomainObjectIDs.Employee3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
            Computer computer = employee.Computer;
            Assert.IsNotNull (computer);
          }, DomainObjectIDs.Employee3, false, true, new ObjectID[] { DomainObjectIDs.Computer1 });
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Official official = Official.GetObject (DomainObjectIDs.Official2);
            int count = official.Orders.Count;
            Assert.AreEqual (0, count);
          }, DomainObjectIDs.Official2, true, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Client client = Client.GetObject (DomainObjectIDs.Client1);
            Client parent = client.ParentClient;
            Assert.IsNull (parent);
          }, DomainObjectIDs.Client1, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
            Employee employee = computer.Employee;
            Assert.IsNull (employee);
          }, DomainObjectIDs.Computer4, false, false, new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
            Computer computer = employee.Computer;
            Assert.IsNull (computer);
          }, DomainObjectIDs.Employee7, false, false, new ObjectID[] { });
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _extension.ObjectLoading (_newTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      _extension.ObjectsLoaded (null, null);
      LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extension.ObjectLoading (_newTransaction, DomainObjectIDs.Order2);
      _extension.ObjectsLoaded (null, null);
      LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order2);
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      ClientTransactionMockEventReceiver clientTransactionEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_newTransaction);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectLoading (_newTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));

        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDelete ()
    {
      Computer computer;

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
      }

      DomainObjectMockEventReceiver computerEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (_newTransaction, computer);
        computerEventReceiver.Deleting (computer, EventArgs.Empty);
        _extension.ObjectDeleted (_newTransaction, computer);
        computerEventReceiver.Deleted (computer, EventArgs.Empty);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        computer.Delete();
      }

    _mockRepository.VerifyAll();
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
      using (_newTransaction.EnterNonDiscardingScope())
      {
        orderItem1 = (OrderItem) _order1.OrderItems[0];
        orderItem2 = (OrderItem) _order1.OrderItems[1];
        orderTicket = _order1.OrderTicket;
        official = _order1.Official;
        customer = _order1.Customer;
        customerOrders = customer.Orders;
        officialOrders = official.Orders;
        preloadedOrder1 = orderTicket.Order;
      }

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

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (_newTransaction, _order1);

        using (_mockRepository.Unordered())
        {
          _extension.RelationChanging (_newTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null);
          _extension.RelationChanging (_newTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
          _extension.RelationChanging (_newTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (_newTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extension.RelationChanging (_newTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders", _order1, null);
        }

        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
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

        using (_mockRepository.Unordered())
        {
          _extension.RelationChanged (_newTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
          _extension.RelationChanged (_newTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
          _extension.RelationChanged (_newTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extension.RelationChanged (_newTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extension.RelationChanged (_newTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
        }

        _extension.ObjectDeleted (_newTransaction, _order1);

        using (_mockRepository.Unordered())
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

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        _order1.Delete();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingDeletedLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;

      

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        location = Location.GetObject (DomainObjectIDs.Location1);
        deletedClient = location.Client;
        deletedClient.Delete ();
        newClient = Client.NewObject ();
      }

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_newTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extension.RelationChanged (_newTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll ();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        location.Client = newClient;
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;

      

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        location = Location.GetObject (DomainObjectIDs.Location1);
        location.Client = Client.NewObject();

        deletedClient = location.Client;
        location.Client.Delete();

        newClient = Client.NewObject();
      }

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_newTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extension.RelationChanged (_newTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll ();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        location.Client = newClient;
      }

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer;

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectDeleting (_newTransaction, computer);
        _extension.ObjectDeleted (_newTransaction, computer);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        computer.Delete();
        computer.Delete();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);
      

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueReading (_newTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (_newTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (_newTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (_newTransaction, order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;

        Dev.Null = _order1.OrderNumber;
        Dev.Null =
            (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyReadWithoutDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      PropertyDefinition orderNumberDefinition = orderClass.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];

      PropertyValue propertyValue = new PropertyValue (orderNumberDefinition);
      PropertyValueCollection propertyValueCollection = new PropertyValueCollection();
      propertyValueCollection.Add (propertyValue);

      Dev.Null = (int) propertyValue.Value;

      // Expectation: no exception
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue =
          _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"];
      ObjectID customerID =
          (ObjectID) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueReading (_newTransaction, order1DC, customerPropertyValue, ValueAccess.Current);
        _extension.PropertyValueRead (_newTransaction, order1DC, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;

        Dev.Null = (ObjectID) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extension);
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

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueChanging (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);
        _extension.PropertyValueChanged (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);

        _extension.PropertyValueReading (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extension.PropertyValueRead (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            newOrderNumber,
            ValueAccess.Current);
        _extension.PropertyValueReading (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extension.PropertyValueRead (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;

        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null =
            (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);
      

      using (_mockRepository.Ordered())
      {
        _extension.PropertyValueChanging (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
        _extension.PropertyValueChanged (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        _order1.OrderNumber = oldOrderNumber + 1;
      }
      
      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extension);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      DomainObjectMockEventReceiver domainObjectMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      DataContainerMockEventReceiver dataContainerMockEventReceiver =
          _mockRepository.StrictMock<DataContainerMockEventReceiver> (order1DC);
      PropertyValueCollectionMockEventReceiver propertyValueCollectionMockEventReceiver =
          _mockRepository.StrictMock<PropertyValueCollectionMockEventReceiver> (order1DC.PropertyValues);


      using (_mockRepository.Ordered())
      {
        // "Changing" notifications

        _extension.PropertyValueChanging (_newTransaction, order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        dataContainerMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        propertyValueCollectionMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();


        // "Changed" notifications

        propertyValueCollectionMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        dataContainerMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        _extension.PropertyValueChanged (_newTransaction, order1DC, order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extension);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll();

      using (PersistenceManager persistanceManager = new PersistenceManager())
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition =
            orderTicketDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        persistanceManager.LoadRelatedDataContainer (
            orderTicket.InternalDataContainer, new RelationEndPointID (orderTicket.ID, orderEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll();

      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (
            _order1.InternalDataContainer, new RelationEndPointID (_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        orderTicket = _order1.OrderTicket;
      }

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Current);
        _extension.RelationRead (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.OrderTicket;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        originalOrderTicket =
            (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Original);
        _extension.RelationRead (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        orderItems = _order1.OrderItems;
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0])
            & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.OrderItems;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        originalOrderItems = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Original);
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("IsReadOnly", true) & Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0])
            & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Current);

        _extension.ObjectLoading (null, null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));

        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
          Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.OrderTicket;
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);

        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction), 
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.OrderItems;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", ValueAccess.Original);

        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));
        _extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Original);

        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResult ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      _mockRepository.BackToRecord (_extension);

      _extension.FilterQueryResult (null, null, null);
      LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      using (_mockRepository.Ordered())
      {
        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();
        _extension.ObjectLoading (_newTransaction, null);
        LastCall.IgnoreArguments ();

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);
      IClientTransactionExtension filteringExtension = _mockRepository.StrictMock<ClientTransactionExtensionWithQueryFiltering>();
      _newTransaction.Extensions.Add ("FilteringExtension", filteringExtension);
      IClientTransactionExtension lastExtension = _mockRepository.StrictMock<IClientTransactionExtension>();
      _newTransaction.Extensions.Add ("LastExtension", lastExtension);

      using (_mockRepository.Ordered())
      {

        for (int i = 0; i < 2; ++i)
        {
          _extension.ObjectLoading (_newTransaction, null);
          LastCall.IgnoreArguments();
          filteringExtension.ObjectLoading (_newTransaction, null);
          LastCall.IgnoreArguments();
          lastExtension.ObjectLoading (_newTransaction, null);
          LastCall.IgnoreArguments();
        }

        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        filteringExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        lastExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));

        _extension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query));
        filteringExtension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2), Mocks_Is.Same (query)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        lastExtension.FilterQueryResult (null, null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1), Mocks_Is.Same (query));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        DomainObjectCollection queryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.AreEqual (1, queryResult.Count);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer;
      Employee employee ;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        employee = Employee.GetObject (DomainObjectIDs.Employee1);
        computer.Employee = employee;
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer;
      Customer newCustomer;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects = true;
        oldCustomer = _order1.Customer;
        newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
        _order1.Customer = newCustomer;
      }
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), 
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), 
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }
      _mockRepository.BackToRecord (_extension);

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_newTransaction);

      DomainObjectMockEventReceiver computerEventReveiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered())
      {
        computerEventReveiver.Committing (computer, EventArgs.Empty);

        _extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));

        clientTransactionMockEventReceiver.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        computerEventReveiver.Committed (computer, EventArgs.Empty);

        clientTransactionMockEventReceiver.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        _extension.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void StorageProviderGetFieldValue ()
    {
      int originalOrderNumber = _order1.OrderNumber;
      _order1.OrderNumber = originalOrderNumber + 1;

      _mockRepository.BackToRecord (_extension);

      using (StorageProviderManager storageProviderManager = new StorageProviderManager())
      {
        using (UnitTestStorageProviderStub storageProvider =
            (UnitTestStorageProviderStub) storageProviderManager.GetMandatory (c_unitTestStorageProviderStubID))
        {
          _mockRepository.ReplayAll();

          Assert.AreEqual (
              originalOrderNumber + 1,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber", ValueAccess.Current));
          Assert.AreEqual (
              originalOrderNumber,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber", ValueAccess.Original));

          _mockRepository.VerifyAll();
        }
      }
    }

    [Test]
    public void Rollback ()
    {
      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RollingBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.IsIn (computer));

        _extension.RolledBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Rollback();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SubTransactions ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.SubTransactionCreating (_newTransaction);
        _extension.SubTransactionCreated (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("ParentTransaction", _newTransaction));
      }

      _mockRepository.ReplayAll ();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.CreateSubTransaction ();
      }
    }

    [Test]
    public void GetObjects ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.ObjectLoading (_newTransaction, DomainObjectIDs.Order2);
        _extension.ObjectLoading (_newTransaction, DomainObjectIDs.Order3);
        _extension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));
      }

      _mockRepository.ReplayAll ();

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        _newTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      }

      _mockRepository.VerifyAll ();
    }
  }
}
