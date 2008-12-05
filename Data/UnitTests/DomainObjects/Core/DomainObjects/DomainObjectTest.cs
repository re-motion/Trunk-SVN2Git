// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

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
      Assert.That (order.ToString (), Is.EqualTo (order.ID.ToString ()));
    }

    [Test]
    public void LoadingOfSimpleObject ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Assert.That (classWithAllDataTypes.ID.Value, Is.EqualTo (DomainObjectIDs.ClassWithAllDataTypes1.Value), "ID.Value");
      Assert.That (classWithAllDataTypes.ID.ClassID, Is.EqualTo (DomainObjectIDs.ClassWithAllDataTypes1.ClassID), "ID.ClassID");
      Assert.That (classWithAllDataTypes.ID.StorageProviderID, Is.EqualTo (DomainObjectIDs.ClassWithAllDataTypes1.StorageProviderID), "ID.StorageProviderID");

      Assert.That (classWithAllDataTypes.BooleanProperty, Is.EqualTo (false), "BooleanProperty");
      Assert.That (classWithAllDataTypes.ByteProperty, Is.EqualTo (85), "ByteProperty");
      Assert.That (classWithAllDataTypes.DateProperty, Is.EqualTo (new DateTime (2005, 1, 1)), "DateProperty");
      Assert.That (classWithAllDataTypes.DateTimeProperty, Is.EqualTo (new DateTime (2005, 1, 1, 17, 0, 0)), "DateTimeProperty");
      Assert.That (classWithAllDataTypes.DecimalProperty, Is.EqualTo (123456.789m), "DecimalProperty");
      Assert.That (classWithAllDataTypes.DoubleProperty, Is.EqualTo (987654.321d), "DoubleProperty");
      Assert.That (classWithAllDataTypes.EnumProperty, Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1), "EnumProperty");
      Assert.That (classWithAllDataTypes.FlagsProperty, Is.EqualTo (ClassWithAllDataTypes.FlagsType.Flag2), "FlagsProperty");
      Assert.That (classWithAllDataTypes.GuidProperty, Is.EqualTo (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")), "GuidProperty");
      Assert.That (classWithAllDataTypes.Int16Property, Is.EqualTo (32767), "Int16Property");
      Assert.That (classWithAllDataTypes.Int32Property, Is.EqualTo (2147483647), "Int32Property");
      Assert.That (classWithAllDataTypes.Int64Property, Is.EqualTo (9223372036854775807L), "Int64Property");
      Assert.That (classWithAllDataTypes.SingleProperty, Is.EqualTo (6789.321), "SingleProperty");
      Assert.That (classWithAllDataTypes.StringProperty, Is.EqualTo ("abcdef���"), "StringProperty");
      Assert.That (classWithAllDataTypes.StringPropertyWithoutMaxLength, Is.EqualTo ("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"), "StringPropertyWithoutMaxLength");
      ResourceManager.IsEqualToImage1 (classWithAllDataTypes.BinaryProperty, "BinaryProperty");

      Assert.That (classWithAllDataTypes.NaBooleanProperty, Is.EqualTo (true), "NaBooleanProperty");
      Assert.That (classWithAllDataTypes.NaByteProperty, Is.EqualTo ((byte) 78), "NaByteProperty");
      Assert.That (classWithAllDataTypes.NaDateProperty, Is.EqualTo (new DateTime (2005, 2, 1)), "NaDateProperty");
      Assert.That (classWithAllDataTypes.NaDateTimeProperty, Is.EqualTo (new DateTime (2005, 2, 1, 5, 0, 0)), "NaDateTimeProperty");
      Assert.That (classWithAllDataTypes.NaDecimalProperty, Is.EqualTo (765.098m), "NaDecimalProperty");
      Assert.That (classWithAllDataTypes.NaDoubleProperty, Is.EqualTo (654321.789d), "NaDoubleProperty");
      Assert.That (classWithAllDataTypes.NaEnumProperty, Is.EqualTo (ClassWithAllDataTypes.EnumType.Value2), "NaEnumProperty");
      Assert.That (classWithAllDataTypes.NaFlagsProperty, Is.EqualTo (ClassWithAllDataTypes.FlagsType.Flag1 | ClassWithAllDataTypes.FlagsType.Flag2), "NaFlagsProperty");
      Assert.That (classWithAllDataTypes.NaGuidProperty, Is.EqualTo (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), "NaGuidProperty");
      Assert.That (classWithAllDataTypes.NaInt16Property, Is.EqualTo ((short) 12000), "NaInt16Property");
      Assert.That (classWithAllDataTypes.NaInt32Property, Is.EqualTo (-2147483647), "NaInt32Property");
      Assert.That (classWithAllDataTypes.NaInt64Property, Is.EqualTo (3147483647L), "NaInt64Property");
      Assert.That (classWithAllDataTypes.NaSingleProperty, Is.EqualTo (12.456F), "NaSingleProperty");

      Assert.That (classWithAllDataTypes.NaBooleanWithNullValueProperty, Is.Null, "NaBooleanWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaByteWithNullValueProperty, Is.Null, "NaByteWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaDecimalWithNullValueProperty, Is.Null, "NaDecimalWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaDateWithNullValueProperty, Is.Null, "NaDateWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaDateTimeWithNullValueProperty, Is.Null, "NaDateTimeWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaDoubleWithNullValueProperty, Is.Null, "NaDoubleWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaEnumWithNullValueProperty, Is.Null, "NaEnumWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaFlagsWithNullValueProperty, Is.Null, "NaFlagsWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaGuidWithNullValueProperty, Is.Null, "NaGuidWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaInt16WithNullValueProperty, Is.Null, "NaInt16WithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaInt32WithNullValueProperty, Is.Null, "NaInt32WithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaInt64WithNullValueProperty, Is.Null, "NaInt64WithNullValueProperty");
      Assert.That (classWithAllDataTypes.NaSingleWithNullValueProperty, Is.Null, "NaSingleWithNullValueProperty");
      Assert.That (classWithAllDataTypes.StringWithNullValueProperty, Is.Null, "StringWithNullValueProperty");
      Assert.That (classWithAllDataTypes.NullableBinaryProperty, Is.Null, "NullableBinaryProperty");
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
      Assert.That (company, Is.Not.Null);

      var partner = company as Partner;
      Assert.That (partner, Is.Not.Null);

      Assert.That (partner.ID, Is.EqualTo (DomainObjectIDs.Partner2), "ID");
      Assert.That (partner.Name, Is.EqualTo ("Partner 2"), "Name");

      Assert.That (partner.ContactPerson.ID, Is.EqualTo (DomainObjectIDs.Person2), "ContactPerson");
    }

    [Test]
    public void LoadingOfTwiceDerivedObject ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Supplier1);
      Assert.That (company, Is.Not.Null);

      var supplier = company as Supplier;
      Assert.That (supplier, Is.Not.Null);

      Assert.That (supplier.ID, Is.EqualTo (DomainObjectIDs.Supplier1));
      Assert.That (supplier.Name, Is.EqualTo ("Lieferant 1"), "Name");
      Assert.That (supplier.ContactPerson.ID, Is.EqualTo (DomainObjectIDs.Person3), "ContactPerson");
      Assert.That (supplier.SupplierQuality, Is.EqualTo (1), "SupplierQuality");
    }

    [Test]
    public void OnLoaded ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);

      Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.True);
      Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (1));
      Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));
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

      Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.False);
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

      Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.True);
      Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (1));
      Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
    }

    [Test]
    public void OnLoadedInSubTransaction ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);

        Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.True);
        Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (2));
        Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void OnLoadedInParentAndSubTransaction ()
    {
      var id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (id);
      Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.True);
      Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (1));
      Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (id);

        Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (2));
        Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void OnLoadedWithNewInParentAndGetInSubTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.False);
      Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (0));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Dev.Null = classWithAllDataTypes.Int32Property;

        Assert.That (classWithAllDataTypes.OnLoadedHasBeenCalled, Is.True);
        Assert.That (classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo (1));
        Assert.That (classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (order.OrderTicket, Is.Not.Null);
      Assert.That (order.OrderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void GetRelatedObjectByInheritedRelationTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer4);

      Ceo ceoReference1 = customer.Ceo;

      Ceo ceoReference2 = customer.Ceo;

      Assert.That (ceoReference2, Is.SameAs (ceoReference1));
    }

    [Test]
    public void GetDerivedRelatedObject ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo10);

      Company company = ceo.Company;
      Assert.That (company, Is.Not.Null);

      var distributor = company as Distributor;
      Assert.That (distributor, Is.Not.Null);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.That (customer.Orders, Is.Not.Null);
      Assert.That (customer.Orders.Count, Is.EqualTo (2));
      Assert.That (customer.Orders[DomainObjectIDs.Order1].ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (customer.Orders[DomainObjectIDs.OrderWithoutOrderItem].ID, Is.EqualTo (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void GetRelatedObjectsWithDerivation ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
      DomainObjectCollection collection = industrialSector.Companies;

      Assert.That (collection.Count, Is.EqualTo (7));
      Assert.That (collection[DomainObjectIDs.Company1].GetPublicDomainObjectType (), Is.EqualTo (typeof (Company)));
      Assert.That (collection[DomainObjectIDs.Company2].GetPublicDomainObjectType (), Is.EqualTo (typeof (Company)));
      Assert.That (collection[DomainObjectIDs.Customer2].GetPublicDomainObjectType (), Is.EqualTo (typeof (Customer)));
      Assert.That (collection[DomainObjectIDs.Customer3].GetPublicDomainObjectType (), Is.EqualTo (typeof (Customer)));
      Assert.That (collection[DomainObjectIDs.Partner2].GetPublicDomainObjectType (), Is.EqualTo (typeof (Partner)));
      Assert.That (collection[DomainObjectIDs.Supplier2].GetPublicDomainObjectType (), Is.EqualTo (typeof (Supplier)));
      Assert.That (collection[DomainObjectIDs.Distributor1].GetPublicDomainObjectType (), Is.EqualTo (typeof (Distributor)));
    }

    [Test]
    public void ChangeTrackingEvents ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      var eventReceiver = new DomainObjectEventReceiver (customer, false);
      customer.Name = "New name";

      Assert.That (eventReceiver.HasChangingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasChangedEventBeenCalled, Is.EqualTo (true));
      Assert.That (customer.Name, Is.EqualTo ("New name"));
      Assert.That (eventReceiver.ChangingOldValue, Is.EqualTo ("Kunde 1"));
      Assert.That (eventReceiver.ChangingNewValue, Is.EqualTo ("New name"));
      Assert.That (eventReceiver.ChangedOldValue, Is.EqualTo ("Kunde 1"));
      Assert.That (eventReceiver.ChangedNewValue, Is.EqualTo ("New name"));
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
        Assert.That (eventReceiver.HasChangingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasChangedEventBeenCalled, Is.EqualTo (false));
        Assert.That (customer.Name, Is.EqualTo ("Kunde 1"));
        Assert.That (eventReceiver.ChangingOldValue, Is.EqualTo ("Kunde 1"));
        Assert.That (eventReceiver.ChangingNewValue, Is.EqualTo ("New name"));
      }
    }

    [Test]
    public void StateProperty ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      customer.Name = "New name";
      Assert.That (customer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void StateInDifferentTransactions ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Name = "New name";

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (customer);
        Assert.That (customer.TransactionContext[ClientTransaction.Current].State, Is.EqualTo (StateType.Unchanged));
        Assert.That (customer.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.Changed));

        using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
        {
          Assert.That (customer.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.Changed)); // must not throw a ClientTransactionDiffersException
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

      Assert.That (newObject.IsDiscarded, Is.True);
      Assert.That (newObject.State, Is.EqualTo (StateType.Discarded));
      Assert.That (newObjectDataContainer.IsDiscarded, Is.True);
      Assert.That (newObjectDataContainer.State, Is.EqualTo (StateType.Discarded));

      loadedObject.Delete ();
      ClientTransactionMock.Commit ();

      Assert.That (loadedObject.IsDiscarded, Is.True);
      Assert.That (loadedObject.State, Is.EqualTo (StateType.Discarded));
      Assert.That (loadedObjectDataContainer.IsDiscarded, Is.True);
      Assert.That (loadedObjectDataContainer.State, Is.EqualTo (StateType.Discarded));
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
        Assert.That (loadedObject.IsDiscarded, Is.True);
      }
      Assert.That (loadedObject.IsDiscarded, Is.False);

      Assert.That (loadedObject.TransactionContext[otherTransaction].IsDiscarded, Is.True);
      Assert.That (loadedObject.TransactionContext[ClientTransaction.Current].IsDiscarded, Is.False);
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      order.MarkAsChanged ();
      Assert.That (order.State, Is.EqualTo (StateType.Changed));

      ClientTransactionMock.Rollback ();
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      SetDatabaseModifyable ();

      order.MarkAsChanged ();
      Assert.That (order.State, Is.EqualTo (StateType.Changed));

      ClientTransactionMock.Commit ();
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
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
      Assert.That (order3.OrderNumber, Is.EqualTo (7));

      newOrder = Order.GetObject (newOrderID);
      Assert.That (newOrder, Is.Not.Null);

      official1 = Official.GetObject (DomainObjectIDs.Official1);
      Assert.That (official1.Orders[newOrderID], Is.Not.Null);
      Assert.That (newOrder.Official, Is.SameAs (official1));
      Assert.That (official1.Orders[DomainObjectIDs.Order1], Is.Null);

      orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Assert.That (orderTicket1.FileName, Is.EqualTo (@"C:\NewFile.tif"));
      Assert.That (orderTicket1.Order, Is.SameAs (newOrder));
      Assert.That (newOrder.OrderTicket, Is.SameAs (orderTicket1));

      newOrderItem1 = OrderItem.GetObject (newOrderItem1ID);
      Assert.That (newOrderItem1, Is.Not.Null);
      Assert.That (newOrderItem1.Position, Is.EqualTo (1));
      Assert.That (newOrderItem1.Order, Is.SameAs (newOrder));
      Assert.That (newOrder.OrderItems[newOrderItem1ID], Is.Not.Null);

      newOrderItem2 = OrderItem.GetObject (newOrderItem2ID);
      Assert.That (newOrderItem2, Is.Not.Null);
      Assert.That (newOrderItem2.Position, Is.EqualTo (2));
      Assert.That (newOrderItem2.Order, Is.SameAs (order3));
      Assert.That (order3.OrderItems[newOrderItem2ID], Is.Not.Null);

      newCustomer = Customer.GetObject (newCustomerID);
      newCeo = Ceo.GetObject (newCeoID);

      Assert.That (newCeo.Company, Is.SameAs (newCustomer));
      Assert.That (newCustomer.Ceo, Is.SameAs (newCeo));
      Assert.That (newCustomer.Orders.Contains (DomainObjectIDs.Order2), Is.True);
      Assert.That (newCustomer.Orders[DomainObjectIDs.Order2].Customer, Is.SameAs (newCustomer));

      orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Assert.That (orderTicket3.FileName, Is.EqualTo (@"C:\NewFile.gif"));
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

      Assert.That (newSubordinate1.Supervisor, Is.SameAs (newSupervisor1));
      Assert.That (newSupervisor1.Subordinates.Contains (newSubordinate1ID), Is.True);

      supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Assert.That (supervisor2.Supervisor, Is.Null);
      Assert.That (supervisor2.Name, Is.EqualTo ("New name of supervisor"));

      subordinate3 = Employee.GetObject (DomainObjectIDs.Employee3);

      Assert.That (subordinate3.Supervisor, Is.SameAs (supervisor2));
      Assert.That (supervisor2.Subordinates.Contains (DomainObjectIDs.Employee3), Is.True);
      Assert.That (subordinate3.Name, Is.EqualTo ("New name of subordinate"));

      Assert.That (newSupervisor1.Supervisor, Is.SameAs (supervisor2));
      Assert.That (supervisor2.Subordinates.Contains (newSupervisor1ID), Is.True);

      newSubordinate2 = Employee.GetObject (newSubordinate2ID);

      Assert.That (newSubordinate2.Supervisor, Is.Null);

      supervisor6 = Employee.GetObject (DomainObjectIDs.Employee6);
      newSubordinate3 = Employee.GetObject (newSubordinate3ID);

      Assert.That (newSubordinate3.Supervisor, Is.SameAs (supervisor6));
      Assert.That (supervisor6.Subordinates.Contains (newSubordinate3ID), Is.True);

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
      Assert.That (computer4.Employee, Is.Null);
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
      Assert.That (partner.ContactPerson.ID, Is.EqualTo (newPerson.ID));
      Assert.That (partner.IndustrialSector, Is.Null);
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
      Assert.That (order.TransactionContext[clientTransactionMock].CanBeUsedInTransaction, Is.True);
      Assert.That (order.TransactionContext[ClientTransactionScope.CurrentTransaction].CanBeUsedInTransaction, Is.False);
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

        Assert.That (order.State, Is.EqualTo (StateType.Deleted));
      }
      Assert.That (order.TransactionContext[clientTransactionMock].CanBeUsedInTransaction, Is.True);
      Assert.That (order.TransactionContext[ClientTransactionScope.CurrentTransaction].CanBeUsedInTransaction, Is.False);
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
      Assert.That (order.TransactionContext[clientTransactionMock].CanBeUsedInTransaction, Is.True);
      Assert.That (order.TransactionContext[ClientTransactionScope.CurrentTransaction].CanBeUsedInTransaction, Is.False);
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrder ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Assert.That (customer.Orders[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (customer.Orders[1].ID, Is.EqualTo (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrderWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      Dev.Null = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      Assert.That (customer.Orders[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (customer.Orders[1].ID, Is.EqualTo (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void GetDomainObjectType ()
    {
      Customer customer = Customer.NewObject ();
      Assert.That (customer.InternalDataContainer.DomainObjectType, Is.EqualTo (typeof (Customer)));
    }

    [Test]
    public void MultiplePropertiesWithSameShortName ()
    {
      var derivedClass = (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();
      ClassWithMixedProperties baseClass = derivedClass;

      derivedClass.String = "Derived";
      baseClass.String = "Base";

      Assert.That (derivedClass.String, Is.EqualTo ("Derived"));
      Assert.That (baseClass.String, Is.EqualTo ("Base"));

      baseClass.String = "NewBase";
      derivedClass.String = "NewDerived";

      Assert.That (derivedClass.String, Is.EqualTo ("NewDerived"));
      Assert.That (baseClass.String, Is.EqualTo ("NewBase"));
    }

    [Test]
    public void EventManager ()
    {
      var order = Order.NewObject ();
      var eventManager = order.EventManager;
      Assert.That (eventManager, Is.Not.Null);
      Assert.That (eventManager.DomainObject, Is.SameAs (order));

      var eventManager2 = order.EventManager;
      Assert.That (eventManager, Is.SameAs (eventManager2));
    }

    [Test]
    public void EventManager_ConstructedTrue ()
    {
      var order = Order.NewObject ();
      var eventManager = order.EventManager;
      Assert.That (eventManager.IsConstructedDomainObject, Is.True);
    }

    [Test]
    public void EventManager_ConstructedFalse ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var eventManager = order.EventManager;
      Assert.That (eventManager.IsConstructedDomainObject, Is.False);
    }

    [Test]
    public void EventManager_Serialization ()
    {
      var order = Order.NewObject ();
      var eventManager = order.EventManager;
      Assert.That (eventManager, Is.Not.Null);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newEventManager = deserializedOrder.EventManager;
      Assert.That (newEventManager, Is.Not.Null);
      Assert.That (newEventManager.DomainObject, Is.SameAs (deserializedOrder));
    }

    [Test]
    public void EventManager_Serialization_ConstructedTrue ()
    {
      var order = Order.NewObject ();
      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newEventManager = deserializedOrder.EventManager;
      Assert.That (newEventManager.IsConstructedDomainObject, Is.True);
    }

    [Test]
    public void EventManager_Serialization_ConstructedFalse ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newEventManager = deserializedOrder.EventManager;
      Assert.That (newEventManager.IsConstructedDomainObject, Is.False);
    }

    [Test]
    public void EventManager_Serialization_ISerializable_ConstructedTrue ()
    {
      var classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      var newEventManager = deserializedClassWithAllDataTypes.EventManager;
      Assert.That (newEventManager.IsConstructedDomainObject, Is.True);
    }

    [Test]
    public void EventManager_Serialization_ISerializable_ConstructedFalse ()
    {
      var classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      var newEventManager = deserializedClassWithAllDataTypes.EventManager;
      Assert.That (newEventManager.IsConstructedDomainObject, Is.False);
    }

    [Test]
    public void Properties ()
    {
      var order = Order.NewObject ();
      var propertyIndexer = order.Properties;
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var propertyIndexer2 = order.Properties;
      Assert.That (propertyIndexer, Is.SameAs (propertyIndexer2));
    }

    [Test]
    public void Properties_Serialization ()
    {
      var order = Order.NewObject ();
      var propertyIndexer = order.Properties;
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newPropertyIndexer = deserializedOrder.Properties;
      Assert.That (newPropertyIndexer, Is.Not.Null);
      Assert.That (newPropertyIndexer.DomainObject, Is.SameAs (deserializedOrder));
    }

    [Test]
    public void TransactionContext()
    {
      var order = Order.NewObject ();
      var transactionContextIndexer = order.TransactionContext;

      Assert.That (transactionContextIndexer, Is.InstanceOfType (typeof (DomainObjectTransactionContextIndexer)));
      Assert.That (((DomainObjectTransactionContext) transactionContextIndexer[ClientTransaction.Current]).DomainObject, Is.SameAs (order));
    }

    [Test]
    public void ConstructorWithVirtualCall()
    {
      var orderItem = OrderItem.NewObject ("Test Toast");
      Assert.That (orderItem.Product, Is.EqualTo ("Test Toast"));
    }

  }
}
