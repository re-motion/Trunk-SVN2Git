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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject.GetType should not be used.")]
    public void GetTypeThrows ()
    {
      try
      {
        Order order = Order.NewObject ();
        typeof (DomainObject).GetMethod ("GetType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke (
            order, new object[0]);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public new void ToString ()
    {
      Order order = Order.NewObject();
      Assert.AreEqual (order.ID.ToString(), order.ToString());
    }

    [Test]
    public void LoadingOfSimpleObject ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.Value, classWithAllDataTypes.ID.Value, "ID.Value");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.ClassID, classWithAllDataTypes.ID.ClassID, "ID.ClassID");
      Assert.AreEqual (DomainObjectIDs.ClassWithAllDataTypes1.StorageProviderID, classWithAllDataTypes.ID.StorageProviderID, "ID.StorageProviderID");

      Assert.AreEqual (false, classWithAllDataTypes.BooleanProperty, "BooleanProperty");
      Assert.AreEqual (85, classWithAllDataTypes.ByteProperty, "ByteProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes.DateProperty, "DateProperty");
      Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes.DateTimeProperty, "DateTimeProperty");
      Assert.AreEqual (123456.789m, classWithAllDataTypes.DecimalProperty, "DecimalProperty");
      Assert.AreEqual (987654.321d, classWithAllDataTypes.DoubleProperty, "DoubleProperty");
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes.EnumProperty, "EnumProperty");
      Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes.GuidProperty, "GuidProperty");
      Assert.AreEqual (32767, classWithAllDataTypes.Int16Property, "Int16Property");
      Assert.AreEqual (2147483647, classWithAllDataTypes.Int32Property, "Int32Property");
      Assert.AreEqual (9223372036854775807L, classWithAllDataTypes.Int64Property, "Int64Property");
      Assert.AreEqual (6789.321, classWithAllDataTypes.SingleProperty, "SingleProperty");
      Assert.AreEqual ("abcdeföäü", classWithAllDataTypes.StringProperty, "StringProperty");
      Assert.AreEqual ("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", classWithAllDataTypes.StringPropertyWithoutMaxLength, "StringPropertyWithoutMaxLength");
      ResourceManager.IsEqualToImage1 (classWithAllDataTypes.BinaryProperty, "BinaryProperty");

      Assert.AreEqual (true, classWithAllDataTypes.NaBooleanProperty, "NaBooleanProperty");
      Assert.AreEqual ((byte) 78, classWithAllDataTypes.NaByteProperty, "NaByteProperty");
      Assert.AreEqual (new DateTime (2005, 2, 1), classWithAllDataTypes.NaDateProperty, "NaDateProperty");
      Assert.AreEqual (new DateTime (2005, 2, 1, 5, 0, 0), classWithAllDataTypes.NaDateTimeProperty, "NaDateTimeProperty");
      Assert.AreEqual (765.098m, classWithAllDataTypes.NaDecimalProperty, "NaDecimalProperty");
      Assert.AreEqual (654321.789d, classWithAllDataTypes.NaDoubleProperty, "NaDoubleProperty");
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes.NaEnumProperty, "NaEnumProperty");
      Assert.AreEqual (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"), classWithAllDataTypes.NaGuidProperty, "NaGuidProperty");
      Assert.AreEqual ((short) 12000, classWithAllDataTypes.NaInt16Property, "NaInt16Property");
      Assert.AreEqual (-2147483647, classWithAllDataTypes.NaInt32Property, "NaInt32Property");
      Assert.AreEqual (3147483647L, classWithAllDataTypes.NaInt64Property, "NaInt64Property");
      Assert.AreEqual (12.456F, classWithAllDataTypes.NaSingleProperty, "NaSingleProperty");

      Assert.IsNull (classWithAllDataTypes.NaBooleanWithNullValueProperty, "NaBooleanWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaByteWithNullValueProperty, "NaByteWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaDecimalWithNullValueProperty, "NaDecimalWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaDateWithNullValueProperty, "NaDateWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaDateTimeWithNullValueProperty, "NaDateTimeWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaDoubleWithNullValueProperty, "NaDoubleWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaEnumWithNullValueProperty, "NaEnumWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaGuidWithNullValueProperty, "NaGuidWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaInt16WithNullValueProperty, "NaInt16WithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaInt32WithNullValueProperty, "NaInt32WithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaInt64WithNullValueProperty, "NaInt64WithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NaSingleWithNullValueProperty, "NaSingleWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.StringWithNullValueProperty, "StringWithNullValueProperty");
      Assert.IsNull (classWithAllDataTypes.NullableBinaryProperty, "NullableBinaryProperty");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty' does not allow null values.")]
    public void GetNullFromNonNullableValueType()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
			classWithAllDataTypes.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = null;
      Dev.Null = classWithAllDataTypes.BooleanProperty;
    }

    [Test]
    public void LoadingOfDerivedObject ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Partner2);
      Assert.IsNotNull (company);

      var partner = company as Partner;
      Assert.IsNotNull (partner);

      Assert.AreEqual (DomainObjectIDs.Partner2, partner.ID, "ID");
      Assert.AreEqual ("Partner 2", partner.Name, "Name");

      Assert.AreEqual (DomainObjectIDs.Person2, partner.ContactPerson.ID, "ContactPerson");
    }

    [Test]
    public void LoadingOfTwiceDerivedObject ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Supplier1);
      Assert.IsNotNull (company);

      var supplier = company as Supplier;
      Assert.IsNotNull (supplier);

      Assert.AreEqual (DomainObjectIDs.Supplier1, supplier.ID);
      Assert.AreEqual ("Lieferant 1", supplier.Name, "Name");
      Assert.AreEqual (DomainObjectIDs.Person3, supplier.ContactPerson.ID, "ContactPerson");
      Assert.AreEqual (1, supplier.SupplierQuality, "SupplierQuality");
    }

    [Test]
    public void OnLoaded ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);

      Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
      Assert.AreEqual (1, classWithAllDataTypes.OnLoadedCallCount);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, classWithAllDataTypes.OnLoadedLoadMode);
    }

    [Test]
    public void NoOnLoadedInReactionToEnlist ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
      classWithAllDataTypes.OnLoadedHasBeenCalled = false;
      classWithAllDataTypes.OnLoadedCallCount = 0;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObject (classWithAllDataTypes);

      Assert.IsFalse (classWithAllDataTypes.OnLoadedHasBeenCalled);
    }

    [Test]
    public void OnLoadedInReactionToEnlistOnFirstAccess ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
      classWithAllDataTypes.OnLoadedHasBeenCalled = false;
      classWithAllDataTypes.OnLoadedCallCount = 0;
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObject (classWithAllDataTypes);

      using (newTransaction.EnterDiscardingScope ())
      {
        classWithAllDataTypes.Int32Property = 5;
      }

      Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
      Assert.AreEqual (1, classWithAllDataTypes.OnLoadedCallCount);
      Assert.AreEqual (LoadMode.DataContainerLoadedOnly, classWithAllDataTypes.OnLoadedLoadMode);
    }

    [Test]
    public void OnLoadedInSubTransaction ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);

        Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
        Assert.AreEqual (2, classWithAllDataTypes.OnLoadedCallCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, classWithAllDataTypes.OnLoadedLoadMode);
      }
    }

    [Test]
    public void OnLoadedInParentAndSubTransaction ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
      Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
      Assert.AreEqual (1, classWithAllDataTypes.OnLoadedCallCount);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, classWithAllDataTypes.OnLoadedLoadMode);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (id);

        Assert.AreEqual (2, classWithAllDataTypes.OnLoadedCallCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, classWithAllDataTypes.OnLoadedLoadMode);
      }
    }

    [Test]
    public void OnLoadedWithNewInParentAndGetInSubTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      Assert.IsFalse (classWithAllDataTypes.OnLoadedHasBeenCalled);
      Assert.AreEqual (0, classWithAllDataTypes.OnLoadedCallCount);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Dev.Null = classWithAllDataTypes.Int32Property;

        Assert.IsTrue (classWithAllDataTypes.OnLoadedHasBeenCalled);
        Assert.AreEqual (1, classWithAllDataTypes.OnLoadedCallCount);
        Assert.AreEqual (LoadMode.DataContainerLoadedOnly, classWithAllDataTypes.OnLoadedLoadMode);
      }
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsNotNull (order.OrderTicket);
      Assert.AreEqual (DomainObjectIDs.OrderTicket1, order.OrderTicket.ID);
    }

    [Test]
    public void GetRelatedObjectByInheritedRelationTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer4);

      Ceo ceoReference1 = customer.Ceo;

      Ceo ceoReference2 = customer.Ceo;

      Assert.AreSame (ceoReference1, ceoReference2);
    }

    [Test]
    public void GetDerivedRelatedObject ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo10);

      Company company = ceo.Company;
      Assert.IsNotNull (company);

      var distributor = company as Distributor;
      Assert.IsNotNull (distributor);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.IsNotNull (customer.Orders);
      Assert.AreEqual (2, customer.Orders.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[DomainObjectIDs.Order1].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[DomainObjectIDs.OrderWithoutOrderItem].ID);
    }

    [Test]
    public void GetRelatedObjectsWithDerivation ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
      DomainObjectCollection collection = industrialSector.Companies;

      Assert.AreEqual (7, collection.Count);
      Assert.AreEqual (typeof (Company), collection[DomainObjectIDs.Company1].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Company), collection[DomainObjectIDs.Company2].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Customer), collection[DomainObjectIDs.Customer2].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Customer), collection[DomainObjectIDs.Customer3].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Partner), collection[DomainObjectIDs.Partner2].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Supplier), collection[DomainObjectIDs.Supplier2].GetPublicDomainObjectType ());
      Assert.AreEqual (typeof (Distributor), collection[DomainObjectIDs.Distributor1].GetPublicDomainObjectType ());
    }

    [Test]
    public void ChangeTrackingEvents ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      var eventReceiver = new DomainObjectEventReceiver (customer, false);
      customer.Name = "New name";

      Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled);
      Assert.AreEqual (true, eventReceiver.HasChangedEventBeenCalled);
      Assert.AreEqual ("New name", customer.Name);
      Assert.AreEqual ("Kunde 1", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("New name", eventReceiver.ChangingNewValue);
      Assert.AreEqual ("Kunde 1", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("New name", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void CancelChangeTrackingEvents ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      var eventReceiver = new DomainObjectEventReceiver (customer, true);

      try
      {
        customer.Name = "New name";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, eventReceiver.HasChangingEventBeenCalled);
        Assert.AreEqual (false, eventReceiver.HasChangedEventBeenCalled);
        Assert.AreEqual ("Kunde 1", customer.Name);
        Assert.AreEqual ("Kunde 1", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("New name", eventReceiver.ChangingNewValue);
      }
    }

    [Test]
    public void StateProperty ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.AreEqual (StateType.Unchanged, customer.State);
      customer.Name = "New name";
      Assert.AreEqual (StateType.Changed, customer.State);
    }

    [Test]
    public void StateInDifferentTransactions ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Name = "New name";

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (customer);
        Assert.AreEqual (StateType.Unchanged, customer.GetStateForTransaction (ClientTransaction.Current));
        Assert.AreEqual (StateType.Changed, customer.GetStateForTransaction (ClientTransactionMock));

        using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (StateType.Changed, customer.GetStateForTransaction (ClientTransactionMock)); // must not throw a ClientTransactionDiffersException
        }
      }
    }

    [Test]
    public void DiscardedStateType ()
    {
      ClassWithAllDataTypes newObject = ClassWithAllDataTypes.NewObject ();
      DataContainer newObjectDataContainer = newObject.InternalDataContainer;
      ClassWithAllDataTypes loadedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      DataContainer loadedObjectDataContainer = newObject.InternalDataContainer;

      newObject.Delete ();

      Assert.IsTrue (newObject.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, newObject.State);
      Assert.IsTrue (newObjectDataContainer.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, newObjectDataContainer.State);

      loadedObject.Delete ();
      ClientTransactionMock.Commit ();

      Assert.IsTrue (loadedObject.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, loadedObject.State);
      Assert.IsTrue (loadedObjectDataContainer.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, loadedObjectDataContainer.State);
    }

    [Test]
    public void IsDiscardedInTransaction ()
    {
      ClientTransaction otherTransaction = ClientTransaction.CreateRootTransaction ();
      ClassWithAllDataTypes loadedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      using (otherTransaction.EnterNonDiscardingScope ())
      {
        otherTransaction.EnlistDomainObject (loadedObject);
        loadedObject.Delete ();
        otherTransaction.Commit ();
        Assert.IsTrue (loadedObject.IsDiscarded);
      }
      Assert.IsFalse (loadedObject.IsDiscarded);

      Assert.IsTrue (loadedObject.IsDiscardedInTransaction (otherTransaction));
      Assert.IsFalse (loadedObject.IsDiscardedInTransaction (ClientTransaction.Current));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Unchanged, order.State);
      order.MarkAsChanged ();
      Assert.AreEqual (StateType.Changed, order.State);

      ClientTransactionMock.Rollback ();
      Assert.AreEqual (StateType.Unchanged, order.State);

      SetDatabaseModifyable ();

      order.MarkAsChanged ();
      Assert.AreEqual (StateType.Changed, order.State);

      ClientTransactionMock.Commit ();
      Assert.AreEqual (StateType.Unchanged, order.State);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject ();
      order.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();
      order.MarkAsChanged ();
    }

    [Test]
    public void PrivateConstructor ()
    {
      Dev.Null = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void ProtectedConstructor ()
    {
      Dev.Null = Company.GetObject (DomainObjectIDs.Company1);
    }

    [Test]
    public void PublicConstructor ()
    {
      Dev.Null = Customer.GetObject (DomainObjectIDs.Customer1);
    }

    [Test]
    public void InternalConstructor ()
    {
      Dev.Null = Ceo.GetObject (DomainObjectIDs.Ceo1);
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException))]
    public void MaxLengthCheck ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      const string tooLongName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";
      customer.Name = tooLongName;
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void TypeCheck ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      const int invalidName = 123;
      customer.NamePropertyOfInvalidType = invalidName;
    }

    [Test]
    public void TestAllOperations ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      Customer customer1 = order1.Customer;
      Customer customer4 = Customer.GetObject (DomainObjectIDs.Customer4);

      Order order3 = customer4.Orders[DomainObjectIDs.Order3];
      Dev.Null = customer4.Orders[DomainObjectIDs.Order4];

      OrderTicket orderTicket1 = order1.OrderTicket;
      OrderTicket orderTicket3 = order2.OrderTicket;

      Official official1 = order1.Official;

      var orderItem1 = order1.OrderItems[DomainObjectIDs.OrderItem1];
      var orderItem2 = order1.OrderItems[DomainObjectIDs.OrderItem2];
      Dev.Null = order3.OrderItems[DomainObjectIDs.OrderItem4];

      order1.Delete ();
      orderItem1.Delete ();
      orderItem2.Delete ();

      order3.OrderNumber = 7;

      Order newOrder = Order.NewObject ();
      ObjectID newOrderID = newOrder.ID;
      newOrder.DeliveryDate = DateTime.Now;
      newOrder.Official = official1;
      customer1.Orders.Add (newOrder);

      newOrder.OrderTicket = orderTicket1;
      orderTicket1.FileName = @"C:\NewFile.tif";

      OrderItem newOrderItem1 = OrderItem.NewObject ();
      ObjectID newOrderItem1ID = newOrderItem1.ID;

      newOrderItem1.Position = 1;
      newOrder.OrderItems.Add (newOrderItem1);

      OrderItem newOrderItem2 = OrderItem.NewObject ();
      ObjectID newOrderItem2ID = newOrderItem2.ID;
      newOrderItem2.Position = 2;
      order3.OrderItems.Add (newOrderItem2);

      Customer newCustomer = Customer.NewObject ();
      ObjectID newCustomerID = newCustomer.ID;

      Ceo newCeo = Ceo.NewObject ();
      ObjectID newCeoID = newCeo.ID;
      newCustomer.Ceo = newCeo;
      order2.Customer = newCustomer;

      orderTicket3.FileName = @"C:\NewFile.gif";

      Order deletedNewOrder = Order.NewObject ();
      deletedNewOrder.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      CheckIfObjectIsDeleted (DomainObjectIDs.Order1);
      CheckIfObjectIsDeleted (DomainObjectIDs.OrderItem1);
      CheckIfObjectIsDeleted (DomainObjectIDs.OrderItem2);

      order3 = Order.GetObject (DomainObjectIDs.Order3);
      Assert.AreEqual (7, order3.OrderNumber);

      newOrder = Order.GetObject (newOrderID);
      Assert.IsNotNull (newOrder);

      official1 = Official.GetObject (DomainObjectIDs.Official1);
      Assert.IsNotNull (official1.Orders[newOrderID]);
      Assert.AreSame (official1, newOrder.Official);
      Assert.IsNull (official1.Orders[DomainObjectIDs.Order1]);

      orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Assert.AreEqual (@"C:\NewFile.tif", orderTicket1.FileName);
      Assert.AreSame (newOrder, orderTicket1.Order);
      Assert.AreSame (orderTicket1, newOrder.OrderTicket);

      newOrderItem1 = OrderItem.GetObject (newOrderItem1ID);
      Assert.IsNotNull (newOrderItem1);
      Assert.AreEqual (1, newOrderItem1.Position);
      Assert.AreSame (newOrder, newOrderItem1.Order);
      Assert.IsNotNull (newOrder.OrderItems[newOrderItem1ID]);

      newOrderItem2 = OrderItem.GetObject (newOrderItem2ID);
      Assert.IsNotNull (newOrderItem2);
      Assert.AreEqual (2, newOrderItem2.Position);
      Assert.AreSame (order3, newOrderItem2.Order);
      Assert.IsNotNull (order3.OrderItems[newOrderItem2ID]);

      newCustomer = Customer.GetObject (newCustomerID);
      newCeo = Ceo.GetObject (newCeoID);

      Assert.AreSame (newCustomer, newCeo.Company);
      Assert.AreSame (newCeo, newCustomer.Ceo);
      Assert.IsTrue (newCustomer.Orders.Contains (DomainObjectIDs.Order2));
      Assert.AreSame (newCustomer, newCustomer.Orders[DomainObjectIDs.Order2].Customer);

      orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Assert.AreEqual (@"C:\NewFile.gif", orderTicket3.FileName);
    }

    [Test]
    public void TestAllOperationsWithHierarchy ()
    {
      Employee newSupervisor1 = Employee.NewObject ();
      ObjectID newSupervisor1ID = newSupervisor1.ID;

      Employee newSubordinate1 = Employee.NewObject ();
      ObjectID newSubordinate1ID = newSubordinate1.ID;
      newSubordinate1.Supervisor = newSupervisor1;

      Employee supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee subordinate4 = Employee.GetObject (DomainObjectIDs.Employee4);

      Employee supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);
      Employee subordinate3 = Employee.GetObject (DomainObjectIDs.Employee3);
      supervisor2.Supervisor = supervisor1;
      supervisor2.Name = "New name of supervisor";
      subordinate3.Name = "New name of subordinate";

      Employee supervisor6 = Employee.GetObject (DomainObjectIDs.Employee6);
      Dev.Null = Employee.GetObject (DomainObjectIDs.Employee7);

      Employee newSubordinate2 = Employee.NewObject ();
      ObjectID newSubordinate2ID = newSubordinate2.ID;
      Employee newSubordinate3 = Employee.NewObject ();
      ObjectID newSubordinate3ID = newSubordinate3.ID;

      newSupervisor1.Supervisor = supervisor2;
      newSubordinate2.Supervisor = supervisor1;
      newSubordinate3.Supervisor = supervisor6;

      supervisor1.Delete ();
      subordinate4.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      newSupervisor1 = Employee.GetObject (newSupervisor1ID);
      newSubordinate1 = Employee.GetObject (newSubordinate1ID);

      Assert.AreSame (newSupervisor1, newSubordinate1.Supervisor);
      Assert.IsTrue (newSupervisor1.Subordinates.Contains (newSubordinate1ID));

      supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Assert.IsNull (supervisor2.Supervisor);
      Assert.AreEqual ("New name of supervisor", supervisor2.Name);

      subordinate3 = Employee.GetObject (DomainObjectIDs.Employee3);

      Assert.AreSame (supervisor2, subordinate3.Supervisor);
      Assert.IsTrue (supervisor2.Subordinates.Contains (DomainObjectIDs.Employee3));
      Assert.AreEqual ("New name of subordinate", subordinate3.Name);

      Assert.AreSame (supervisor2, newSupervisor1.Supervisor);
      Assert.IsTrue (supervisor2.Subordinates.Contains (newSupervisor1ID));

      newSubordinate2 = Employee.GetObject (newSubordinate2ID);

      Assert.IsNull (newSubordinate2.Supervisor);

      supervisor6 = Employee.GetObject (DomainObjectIDs.Employee6);
      newSubordinate3 = Employee.GetObject (newSubordinate3ID);

      Assert.AreSame (supervisor6, newSubordinate3.Supervisor);
      Assert.IsTrue (supervisor6.Subordinates.Contains (newSubordinate3ID));

      CheckIfObjectIsDeleted (DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee4);
    }

    [Test]
    public void DeleteNewObjectWithExistingRelated ()
    {
      Computer computer4 = Computer.GetObject (DomainObjectIDs.Computer4);

      Employee newDeletedEmployee = Employee.NewObject ();
      computer4.Employee = newDeletedEmployee;

      newDeletedEmployee.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      computer4 = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.IsNull (computer4.Employee);
    }

    [Test]
    public void ExistingObjectRelatesToNewAndDeleted ()
    {
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner2);

      Person newPerson = Person.NewObject ();
      partner.ContactPerson = newPerson;
      partner.IndustrialSector.Delete ();

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      partner = Partner.GetObject (DomainObjectIDs.Partner2);
      Assert.AreEqual (newPerson.ID, partner.ContactPerson.ID);
      Assert.IsNull (partner.IndustrialSector);
    }

    [Test]
    public void GetObjectWithTransaction ()
    {
      Order order;
      var clientTransactionMock = new ClientTransactionMock ();
      using (clientTransactionMock.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.IsTrue (order.CanBeUsedInTransaction (clientTransactionMock));
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
    }

    [Test]
    public void GetDeletedObjectWithTransaction ()
    {
      Order order;
      var clientTransactionMock = new ClientTransactionMock ();
      using (clientTransactionMock.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);

        order.Delete();

        order = Order.GetObject (DomainObjectIDs.Order1, true);

        Assert.AreEqual (StateType.Deleted, order.State);
      }
      Assert.IsTrue (order.CanBeUsedInTransaction (clientTransactionMock));
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
    }

    [Test]
    public void CreateNewObjectWithTransaction ()
    {
      var clientTransactionMock = new ClientTransactionMock ();
      Order order;
      using (clientTransactionMock.EnterDiscardingScope())
      {
        order = Order.NewObject ();
      }
      Assert.IsTrue (order.CanBeUsedInTransaction (clientTransactionMock));
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrder ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[1].ID);
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrderWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Dev.Null = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      Assert.AreEqual (DomainObjectIDs.Order1, customer.Orders[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, customer.Orders[1].ID);
    }

    [Test]
    public void GetDomainObjectType ()
    {
      Customer customer = Customer.NewObject ();
      Assert.AreEqual (typeof (Customer), customer.InternalDataContainer.DomainObjectType);
    }

    [Test]
    public void MultiplePropertiesWithSameShortName ()
    {
      var derivedClass = (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();
      ClassWithMixedProperties baseClass = derivedClass;

      derivedClass.String = "Derived";
      baseClass.String = "Base";
      
      Assert.AreEqual ("Derived", derivedClass.String);
      Assert.AreEqual ("Base", baseClass.String);

      baseClass.String = "NewBase";
      derivedClass.String = "NewDerived";
      
      Assert.AreEqual ("NewDerived", derivedClass.String);
      Assert.AreEqual ("NewBase", baseClass.String);
    }
  }
}
