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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorTest : ClientTransactionBaseTest
  {
    [Test]
    public void Construction()
    {
      var sector = IndustrialSector.NewObject ();
      var data = new PropertyAccessorData (sector.ID.ClassDefinition, typeof (IndustrialSector).FullName + ".Name");

      var propertyAccessor = new PropertyAccessor (sector, data);
      Assert.That (propertyAccessor.DomainObject, Is.SameAs (sector));
      Assert.That (propertyAccessor.PropertyData, Is.SameAs (data));
    }

    [Test]
    public void GetValue_SetValue ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();

      Company company = Company.NewObject ();
      company.IndustrialSector = sector;
      Assert.AreSame (sector, company.IndustrialSector, "related object");

      Assert.IsTrue (sector.Companies.ContainsObject (company), "related objects");
      var newCompanies = new ObjectList<Company> ();
      sector.Companies = newCompanies;
      Assert.That (sector.Companies, Is.SameAs (newCompanies));

      sector.Name = "Foo";
      Assert.AreEqual ("Foo", sector.Name, "property value");
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void GetValue_ThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Name").GetValue<int>();
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void SetValue_ThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Name").SetValue (5);
    }

   [Test]
    public void HasChangedTx ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (sector.Properties[typeof (IndustrialSector), "Name"].HasChangedTx (ClientTransactionMock), Is.False);
      }
      sector.Name = "Foo";
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (sector.Properties[typeof (IndustrialSector), "Name"].HasChangedTx (ClientTransactionMock), Is.True);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Domain object 'IndustrialSector|3bb7bee9-2af9-4a85-998e-618bebbe5a6b|System.Guid' cannot be used in the current " 
        + "transaction as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call EnlistInTransaction to enlist the object in the current transaction.")]
    public void HasChangedTx_InvalidTransaction ()
    {
      IndustrialSector sector = IndustrialSector.GetObject(DomainObjectIDs.IndustrialSector1);
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction ();
      sector.Properties[typeof (IndustrialSector), "Name"].HasChangedTx (clientTransaction);
    }

    [Test]
    public void HasBeenTouchedTx ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (sector.Properties[typeof (IndustrialSector), "Name"].HasBeenTouchedTx (ClientTransactionMock), Is.False);
      }
      sector.Name = sector.Name;
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.That (sector.Properties[typeof (IndustrialSector), "Name"].HasBeenTouchedTx (ClientTransactionMock), Is.True);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Domain object 'IndustrialSector|3bb7bee9-2af9-4a85-998e-618bebbe5a6b|System.Guid' cannot be used in the current "
        + "transaction as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call EnlistInTransaction to enlist the object in the current transaction.")]
    public void HasBeenTouchedTx_InvalidTransaction ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction ();
      sector.Properties[typeof (IndustrialSector), "Name"].HasBeenTouchedTx (clientTransaction);
    }

    [Test]
    public void HasChangedAndOriginalValueSimple()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Name");
      Assert.IsFalse (property.HasChanged);
      var originalValue = property.GetValue<string>();
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());

      property.SetValue ("Foo");
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual ("Foo", property.GetValue<string>());
      Assert.AreNotEqual (property.GetValue<string>(), property.GetOriginalValue<string> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());
    }

    [Test]
    public void HasChangedAndOriginalValueRelated()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      PropertyAccessor property = CreateAccessor (computer, "Employee");
      Assert.IsFalse (property.HasChanged);
      var originalValue = property.GetOriginalValue<Employee>();

      property.GetValue<Employee> ().Name = "FooBarBazFred";
      Assert.IsFalse (property.HasChanged);

      Employee newValue = Employee.NewObject ();
      property.SetValue (newValue);
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual (newValue, property.GetValue<Employee> ());
      Assert.AreNotEqual (property.GetValue<Employee> (), property.GetOriginalValue<Employee> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<Employee> ());

    }

    [Test]
    public void HasChangedAndOriginalValueRelatedCollection()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Companies");

      Assert.IsFalse (property.HasChanged);
      var originalValue = property.GetValue<ObjectList<Company>> ();
      int originalCount = originalValue.Count;
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<ObjectList<Company>> ());

      property.GetValue<ObjectList<Company>> ().Add (Company.NewObject ());
      Assert.AreNotEqual (originalCount, property.GetValue<ObjectList<Company>> ().Count);
      Assert.IsTrue (property.HasChanged);

      Assert.AreSame (originalValue, property.GetValue<ObjectList<Company>> ()); // !!
      Assert.AreNotSame (property.GetValue<ObjectList<Company>> (), property.GetOriginalValue<ObjectList<Company>> ());
      Assert.AreEqual (originalCount, property.GetOriginalValue<ObjectList<Company>>().Count);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage =  "Actual type .* of property .* does not match expected type 'System.Int32'",
        MatchType = MessageMatch.Regex)]
    public void GetOriginalValueThrowsWithWrongType()
    {
      CreateAccessor (IndustrialSector.NewObject(), "Companies").GetOriginalValue<int>();
    }

    [Test]
    public void HasBeenTouchedSimple ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Name");

      Assert.IsFalse (property.HasBeenTouched);
      property.SetValueWithoutTypeCheck (property.GetValueWithoutTypeCheck ());
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRelated ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      PropertyAccessor property = CreateAccessor (computer, "Employee");

      Assert.IsFalse (property.HasBeenTouched);
      property.SetValueWithoutTypeCheck (property.GetValueWithoutTypeCheck ());
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedRelatedCollection ()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Companies");

      Assert.IsFalse (property.HasBeenTouched);
      ((DomainObjectCollection) property.GetValueWithoutTypeCheck ())[0] = ((DomainObjectCollection) property.GetValueWithoutTypeCheck ())[0];
      Assert.IsTrue (property.HasBeenTouched);
    }

    [Test]
    public void IsNullPropertyValue ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (true);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull);

      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue ("");
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue<string> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull);
    }

    [Test]
    public void IsNullRelatedObjectCollection ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].IsNull);
    }

    [Test]
    public void IsNullRelatedObjectNonVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsTrue (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].IsNull);

      newOrder.Customer = Customer.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].IsNull);

      newOrder.Customer = null;
      Assert.IsTrue (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].IsNull);

      var eventReceiver = new ClientTransactionEventReceiver (ClientTransactionScope.CurrentTransaction);
      Order existingOrder = Order.GetObject (DomainObjectIDs.Order1);

      eventReceiver.Clear ();
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].IsNull);
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count, "The IsNull check did not cause the object to be loaded.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetValue<Customer> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "An ordinary check does cause the object to be loaded.");
    }

    [Test]
    public void IsNullRelatedObjectVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsTrue (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].IsNull);

      newOrder.OrderTicket = OrderTicket.NewObject ();
      Assert.IsFalse (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].IsNull);

      newOrder.OrderTicket = null;
      Assert.IsTrue (newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].IsNull);

      var eventReceiver = new ClientTransactionEventReceiver (ClientTransactionScope.CurrentTransaction);
      Order existingOrder = Order.GetObject (DomainObjectIDs.Order1);

      eventReceiver.Clear ();
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjects.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].IsNull);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "For virtual end points, the IsNull unfortunately does cause a load.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValue<OrderTicket> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count, "An ordinary check does cause the object to be loaded.");
    }

    [Test]
    public void GetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();

      object ticket = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheck();
      Assert.AreSame (ticket, newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValue<OrderTicket>());

      object items = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheck ();
      Assert.AreSame (items,
          newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValue<ObjectList<OrderItem>> ());

      object number = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheck ();
      Assert.AreEqual (number,
          newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValue<int> ());
    }

    [Test]
    public void GetOriginalValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.OrderTicket = OrderTicket.NewObject ();

      object ticket = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreSame (ticket, newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValue<OrderTicket> ());

      newOrder.OrderItems.Add (OrderItem.NewObject ());

      object items = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreSame (items,
          newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ());

      ++newOrder.OrderNumber;

      object number = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheck ();
      Assert.AreEqual (number,
          newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValue<int> ());
    }

    [Test]
    public void SetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck (7);
      Assert.AreEqual (7, newOrder.OrderNumber);

      OrderTicket orderTicket = OrderTicket.NewObject ();
      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheck (orderTicket);
      Assert.AreSame (orderTicket, newOrder.OrderTicket);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber' does not match expected type 'System.Int32'.")]
    public void SetValueWithoutTypeCheckThrowsOnWrongType ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck ("7");
    }

    [Test]
    public void GetValueTx ()
    {
      Order newOrder = Order.NewObject ();
      
      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValue(9);
      newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false));
      newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValue (10);
        newOrder.OrderItems.Clear();
        newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false));
        newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

        Assert.AreEqual (
            10,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValueTx<int> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValueTx<int> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction)[0]);
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction)[0]);

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket2, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetValueWithoutTypeCheckTx ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValue (9);
      newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false));
      newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValue (10);
        newOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false));
        newOrder.OrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

        Assert.AreEqual (
            10,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
            ((DomainObjectCollection)newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction))[0]);
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
            ((DomainObjectCollection)newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction))[0]);

        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket2, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetOriginalValueTx ()
    {
      Order newOrder = Order.NewObject ();
      
      newOrder.OrderNumber = 9;
      OrderItem newOrderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (newOrderItem);
      OrderTicket newOrderTicket = OrderTicket.NewObject ();
      newOrder.OrderTicket = newOrderTicket;

      newOrder.Official = Official.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
        
        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValueTx<int> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValueTx<int> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            newOrderItem,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction)[0]);
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValueTx<ObjectList<OrderItem>> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction).Count);

        Assert.AreEqual (
            newOrderTicket,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            null,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValueTx<OrderTicket> (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueTx<int> (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void GetOriginalValueWithoutTypeCheckTx ()
    {
      Order newOrder = Order.NewObject ();

      newOrder.OrderNumber = 9;
      OrderItem newOrderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (newOrderItem);
      OrderTicket newOrderTicket = OrderTicket.NewObject ();
      newOrder.OrderTicket = newOrderTicket;

      newOrder.Official = Official.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (
            9,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            0,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));

        Assert.AreEqual (
            newOrderItem,
            ((ObjectList<OrderItem>) newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction))[0]);
        Assert.AreEqual (
            0,
            ((ObjectList<OrderItem>) newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction)).Count);

        Assert.AreEqual (
            newOrderTicket,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (
            null,
            newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheckTx (
                ClientTransactionScope.CurrentTransaction.ParentTransaction));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void GetOriginalValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.GetOriginalValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction);
      }
    }

    [Test]
    public void SetValueTx ()
    {
      Order order = Order.GetObject(DomainObjectIDs.Order1);
      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueTx (
            ClientTransactionScope.CurrentTransaction, 1);
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueTx (ClientTransactionMock, 2);

        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].SetValueTx (
            ClientTransactionScope.CurrentTransaction, orderTicket1);
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].SetValueTx (
            ClientTransactionMock, orderTicket2);

        Assert.AreEqual (1, order.OrderNumber);
        Assert.AreSame (orderTicket1, order.OrderTicket);
      }
      Assert.AreEqual (2, order.OrderNumber);
      Assert.AreSame (orderTicket2, order.OrderTicket);
    }

    [Test]
    public void SetValueTx_WithObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      var newCompanies = new ObjectList<Company> ();
      CreateAccessor (sector, "Companies").SetValue (newCompanies);
      Assert.That (sector.Companies, Is.SameAs (newCompanies));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueTx (ClientTransactionScope.CurrentTransaction, 1);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueTx (ClientTransactionScope.CurrentTransaction, 2);
      }
    }

    [Test]
    public void SetValueWithoutTypeCheckTx ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, 1);
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (ClientTransactionMock, 2);

        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, orderTicket1);
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheckTx (
            ClientTransactionMock, orderTicket2);

        Assert.AreEqual (1, order.OrderNumber);
        Assert.AreSame (orderTicket1, order.OrderTicket);
      }
      Assert.AreEqual (2, order.OrderNumber);
      Assert.AreSame (orderTicket2, order.OrderTicket);
    }

    [Test]
    public void SetValueWithoutTypeCheckTx_WithObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      var newCompanies = new ObjectList<Company> ();
      CreateAccessor (sector, "Companies").SetValueWithoutTypeCheckTx (ClientTransactionMock, newCompanies);
      Assert.That (sector.Companies, Is.SameAs (newCompanies));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueWithoutTypeCheckTxWithInvalidTransactionNew ()
    {
      Order newOrder = Order.NewObject ();
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction, 1);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetValueWithoutTypeCheckTxWithInvalidTransactionLoaded ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor accessor = newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        accessor.SetValueWithoutTypeCheckTx (ClientTransactionScope.CurrentTransaction, 2);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void SetValueWithoutTypeCheckTxForWrongType ()
    {
      Order newOrder = Order.NewObject ();
      newOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheckTx (
            ClientTransactionScope.CurrentTransaction, "1");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetRelatedObjectID ();
    }

    [Test]
    public void GetRelatedObjectIDRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (order.Customer.ID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetRelatedObjectID ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDRelatedCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDTxSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetRelatedObjectIDTxRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID customerID = order.Customer.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (customerID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetRelatedObjectIDTx (ClientTransactionMock));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectIDTxRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectTxIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectID ();
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID originalID = order.Customer.ID;
      order.Customer = Customer.NewObject ();
      Assert.AreNotEqual (order.Customer.ID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetOriginalRelatedObjectID ());
      Assert.AreEqual (originalID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetOriginalRelatedObjectID ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDRelatedCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDTxSimple ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetOriginalRelatedObjectIDTxRelatedRealEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ObjectID originalCustomerID = order.Customer.ID;
      order.Customer = Customer.NewObject ();
      ObjectID customerID = order.Customer.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreNotEqual (customerID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetOriginalRelatedObjectIDTx (ClientTransactionMock));
        Assert.AreEqual (originalCustomerID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetOriginalRelatedObjectIDTx (ClientTransactionMock));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectIDTxRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetOriginalRelatedObjectTxIDRelatedVirtualEndPoint ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectIDTx (ClientTransactionMock);
      }
    }

    [Test]
    public void GetSetValueTx_WithNoScope ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionScope.EnterNullScope ())
      {
        var orderNumber = order.Properties[typeof (Order), "OrderNumber"].GetValueTx<int> (ClientTransactionMock);
        order.Properties[typeof (Order), "OrderNumber"].SetValueTx (ClientTransactionMock, orderNumber + 1);
        Assert.AreEqual (orderNumber + 1, order.Properties[typeof (Order), "OrderNumber"].GetValueTx<int> (ClientTransactionMock));
      }
    }

    [Test]
    public void DiscardCheck ()
    {
      Order order = Order.NewObject ();
      order.Delete ();

      PropertyAccessor property = order.Properties[typeof (Order), "OrderNumber"];

      ExpectDiscarded (() => Dev.Null = property.HasChanged);
      ExpectDiscarded (() => Dev.Null = property.HasBeenTouched);
      ExpectDiscarded (() => Dev.Null = property.IsNull);
      ExpectDiscarded (() => Dev.Null = property.GetOriginalRelatedObjectID());
      ExpectDiscarded (() => Dev.Null = property.GetOriginalValue<int>());
      ExpectDiscarded (() => Dev.Null = property.GetOriginalValueWithoutTypeCheck());
      ExpectDiscarded (() => Dev.Null = property.GetRelatedObjectID());
      ExpectDiscarded (() => Dev.Null = property.GetValue<int>());
      ExpectDiscarded (() => Dev.Null = property.GetValueWithoutTypeCheck());
      ExpectDiscarded (() => property.SetValue (0));
      ExpectDiscarded (() => property.SetValueWithoutTypeCheck (0));
      
      // no exceptions
      Dev.Null = property.PropertyData;
      Dev.Null = property.DomainObject;
    }

    [Test]
    public void DiscardCheck_Tx ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ClientTransaction otherTransaction = ClientTransaction.CreateRootTransaction ();
      using (otherTransaction.EnterNonDiscardingScope ())
      {
        otherTransaction.EnlistDomainObject (instance);
        instance.Delete();
        SetDatabaseModifyable ();
        otherTransaction.Commit ();
        Assert.IsTrue (instance.IsDiscarded);
      }

      Assert.IsFalse (instance.IsDiscarded);

      PropertyAccessor property = instance.Properties[typeof (ClassWithAllDataTypes), "Int32Property"];

      ExpectDiscarded (() => Dev.Null = property.GetOriginalRelatedObjectIDTx (otherTransaction));
      ExpectDiscarded (() => Dev.Null = property.GetOriginalValueTx<int> (otherTransaction));
      ExpectDiscarded (() => Dev.Null = property.GetOriginalValueWithoutTypeCheckTx (otherTransaction));
      ExpectDiscarded (() => Dev.Null = property.GetRelatedObjectIDTx (otherTransaction));
      ExpectDiscarded (() => Dev.Null = property.GetValueTx<int> (otherTransaction));
      ExpectDiscarded (() => Dev.Null = property.GetValueWithoutTypeCheckTx (otherTransaction));
      ExpectDiscarded (() => property.SetValueTx (otherTransaction, 0));
      ExpectDiscarded (() => property.SetValueWithoutTypeCheckTx (otherTransaction, 0));

      try { Dev.Null = property.GetOriginalRelatedObjectIDTx (ClientTransaction.Current); } catch (InvalidOperationException) { }
      Dev.Null = property.GetOriginalValueTx<int> (ClientTransaction.Current);
      Dev.Null = property.GetOriginalValueWithoutTypeCheckTx (ClientTransaction.Current);
      try { Dev.Null = property.GetRelatedObjectIDTx (ClientTransaction.Current); } catch (InvalidOperationException) { }
      Dev.Null = property.GetValueTx<int> (ClientTransaction.Current);
      Dev.Null = property.GetValueWithoutTypeCheckTx (ClientTransaction.Current);
      property.SetValueTx (ClientTransaction.Current, 0);
      property.SetValueWithoutTypeCheckTx (ClientTransaction.Current, 0);
    }


    private void ExpectDiscarded (Action action)
    {
      try
      {
        action ();
        Assert.Fail ("Expected ObjectDiscardedException.");
      }
      catch (ObjectDiscardedException)
      {
        // ok
      }
    }

    private static PropertyAccessor CreateAccessor (DomainObject domainObject, string shortIdentifier)
    {
      string propertyIdentifier = domainObject.GetPublicDomainObjectType().FullName + "." + shortIdentifier;
      var data = new PropertyAccessorData (domainObject.ID.ClassDefinition, propertyIdentifier);
      return new PropertyAccessor (domainObject, data);
    }
  }
}
