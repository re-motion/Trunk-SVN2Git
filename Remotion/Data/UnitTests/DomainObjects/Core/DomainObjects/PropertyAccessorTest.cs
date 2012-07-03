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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorTest : ClientTransactionBaseTest
  {
    [Test]
    public void Construction()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject();
      }
      var data = new PropertyAccessorData (sector.ID.ClassDefinition, typeof (IndustrialSector).FullName + ".Name");

      var propertyAccessor = new PropertyAccessor (sector, data, transaction);
      Assert.That (propertyAccessor.DomainObject, Is.SameAs (sector));
      Assert.That (propertyAccessor.PropertyData, Is.SameAs (data));
      Assert.That (propertyAccessor.ClientTransaction, Is.SameAs (transaction));
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
    public void GetValue_SetValue_WithTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject ();
        sector.Name = "Foo";
      }
      var data = new PropertyAccessorData (sector.ID.ClassDefinition, typeof (IndustrialSector).FullName + ".Name");
      var accessor = new PropertyAccessor(sector, data, transaction);

      Assert.That (accessor.GetValue<string> (), Is.EqualTo ("Foo"));
      accessor.SetValue ("Bar");

      using (transaction.EnterNonDiscardingScope ())
      {
        Assert.That (sector.Name, Is.EqualTo ("Bar"));
      }
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
    public void SetValue_WithObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      var newCompanies = new ObjectList<Company> ();
      var oldCompanies = sector.Companies;

      CreateAccessor (sector, "Companies").SetValue (newCompanies);

      Assert.That (sector.Companies, Is.SameAs (newCompanies));
      Assert.That (sector.Companies.AssociatedEndPointID, Is.Not.Null);
      Assert.That (oldCompanies.AssociatedEndPointID, Is.Null);
    }

    [Test]
    public void SetValue_WithObjectList_PerformsBidirectionalChange ()
    {
      var sector = IndustrialSector.NewObject ();
      var company = Company.NewObject();

      var newCompanies = new ObjectList<Company> { company };

      CreateAccessor (sector, "Companies").SetValue (newCompanies);

      Assert.That (company.IndustrialSector, Is.SameAs (sector));
    }

    [Test]
    public void SetValue_WithObjectList_Notifies ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      IndustrialSector sector = IndustrialSector.NewObject ();
      var newCompanies = new ObjectList<Company> { Company.NewObject() };

      var propertyAccessor = CreateAccessor (sector, "Companies");
      propertyAccessor.SetValue (newCompanies);

      listenerMock.AssertWasCalled (
        mock => mock.RelationChanged (TestableClientTransaction, sector, propertyAccessor.PropertyData.RelationEndPointDefinition, null, newCompanies[0]));
    }

    [Test]
    public void SetValue_WithObjectList_SelfReplace ()
    {
      var sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      var previousEndPointID = sector.Companies.AssociatedEndPointID;

      sector.Companies = sector.Companies;

      Assert.That (sector.Companies.AssociatedEndPointID, Is.SameAs (previousEndPointID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given collection is already associated with an end point.\r\n"
        + "Parameter name: value")]
    public void SetValue_WithObjectList_NewCollectionAlreadyAssociated ()
    {
      var sector1 = IndustrialSector.NewObject ();
      var sector2 = IndustrialSector.NewObject ();

      CreateAccessor (sector1, "Companies").SetValue (sector2.Companies);
    }

    [Test]
    public void SetValue_WithObjectList_CollectionIsReadOnly ()
    {
      var customer1 = Customer.NewObject ();

      var newCollection = (OrderCollection) new OrderCollection ().Clone (true);
      CreateAccessor (customer1, "Orders").SetValue (newCollection);

      Assert.That (customer1.Orders, Is.SameAs (newCollection));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The given collection ('Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.TestDomain.Order]') is not of the same type "
        + "as the end point's current opposite collection ('Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection').")]
    public void SetValue_WithObjectList_DifferentCollectionTypes ()
    {
      var customer1 = Customer.NewObject ();

      var newCollection = new ObjectList<Order> ();
      CreateAccessor (customer1, "Orders").SetValueWithoutTypeCheck (newCollection);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The given collection has a different item type than the end point's current opposite collection.")]
    public void SetValue_WithObjectList_DifferentRequiredItemType ()
    {
      var customer1 = Customer.NewObject ();

      var newCollection = new DomainObjectCollection (typeof (Customer));
      CreateAccessor (customer1, "Orders").SetValueWithoutTypeCheck (newCollection);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetValue_WithObjectList_ObjectDeleted ()
    {
      var sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      sector.Delete ();

      CreateAccessor (sector, "Companies").SetValue (new ObjectList<Company> ());
    }

    [Test]
    public void SetValue_WithRelatedObject ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var newTicket = OrderTicket.NewObject();

      CreateAccessor (order, "OrderTicket").SetValue (newTicket);

      Assert.That (order.OrderTicket, Is.SameAs (newTicket));
    }

    [Test]
    public void SetValue_WithRelatedObject_PerformsBidirectionalChange ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var newTicket = OrderTicket.NewObject ();
      var oldTicket = order.OrderTicket;

      CreateAccessor (order, "OrderTicket").SetValue (newTicket);

      Assert.That (newTicket.Order, Is.SameAs (order));
      Assert.That (oldTicket.Order, Is.Null);
    }

    [Test]
    public void SetValue_WithRelatedObject_Notifies ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var oldTicket = order.OrderTicket;
      var newTicket = OrderTicket.NewObject ();

      var propertyAccessor = CreateAccessor (order, "OrderTicket");
      propertyAccessor.SetValue (newTicket);

      listenerMock.AssertWasCalled (
          mock => mock.RelationChanged (TestableClientTransaction, order, propertyAccessor.PropertyData.RelationEndPointDefinition, oldTicket, newTicket));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetValue_WithRelatedObject_ObjectDeleted ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete();

      CreateAccessor (order, "OrderTicket").SetValue (OrderTicket.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetValue_WithRelatedObject_NewObjectDeleted ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var newTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      newTicket.Delete();

      CreateAccessor (order, "OrderTicket").SetValue (newTicket);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetValue_WithRelatedObject_WithInvalidType ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var customer = Company.GetObject (DomainObjectIDs.Customer1);

      CreateAccessor (order, "OrderTicket").SetValueWithoutTypeCheck (customer);
    }

    [Test]
    public void SetValue_WithRelatedObject_WithCorrectDerivedType ()
    {
      var ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      var partnerCompany = Partner.GetObject (DomainObjectIDs.Partner1);

      CreateAccessor (ceo, "Company").SetValueWithoutTypeCheck (partnerCompany);

      Assert.That (ceo.Company, Is.SameAs (partnerCompany));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetValue_WithRelatedObject_WithInvalidBaseType ()
    {
      var person = Person.GetObject (DomainObjectIDs.Person1);
      var company = Company.GetObject (DomainObjectIDs.Company1);

      CreateAccessor (person, "AssociatedPartnerCompany").SetValueWithoutTypeCheck (company);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set to DomainObject "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. The objects do not belong to the same ClientTransaction.")]
    public void SetValue_WithRelatedObject_WithObjectNotEnlistedInThisTransaction ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketFromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<OrderTicket> (DomainObjectIDs.OrderTicket1);

      CreateAccessor (order, "OrderTicket").SetValueWithoutTypeCheck (orderTicketFromOtherTransaction);
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

      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull);
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].IsNull);

      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].SetValue (Color.Values.Green());
      Assert.IsFalse (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull);
      
      cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].SetValue<Color> (null);
      Assert.IsTrue (cwadt.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull);
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
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjectLists.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].IsNull);
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjectLists.Count, "The IsNull check did not cause the object to be loaded.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetValue<Customer> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjectLists.Count, "An ordinary check does cause the object to be loaded.");
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
      Assert.AreEqual (0, eventReceiver.LoadedDomainObjectLists.Count);

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].IsNull);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjectLists.Count, "For virtual end points, the IsNull unfortunately does cause a load.");

      Assert.IsFalse (existingOrder.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetValue<OrderTicket> () == null);
      Assert.AreEqual (1, eventReceiver.LoadedDomainObjectLists.Count, "An ordinary check does cause the object to be loaded.");
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
      Assert.AreEqual (items,
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDSimple ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetRelatedObjectID ();
    }

    [Test]
    public void GetRelatedObjectIDRelatedRealEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (order.Customer.ID, order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].GetRelatedObjectID ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "ObjectIDs only exist on the real side of a relation, not on the virtual side.")]
    public void GetRelatedObjectIDRelatedVirtualEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetRelatedObjectIDRelatedCollection ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDSimple ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectID ();
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedRealEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
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
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectID ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This operation can only be used on related object properties.")]
    public void GetOriginalRelatedObjectIDRelatedCollection ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"].GetOriginalRelatedObjectID ();
    }

    [Test]
    public void DiscardCheck ()
    {
      var order = Order.NewObject ();
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

    private void ExpectDiscarded (Action action)
    {
      try
      {
        action ();
        Assert.Fail ("Expected ObjectInvalidException.");
      }
      catch (ObjectInvalidException)
      {
        // ok
      }
    }

    private static PropertyAccessor CreateAccessor (DomainObject domainObject, string shortIdentifier)
    {
      string propertyIdentifier = domainObject.GetPublicDomainObjectType().FullName + "." + shortIdentifier;
      var data = new PropertyAccessorData (domainObject.ID.ClassDefinition, propertyIdentifier);
      return new PropertyAccessor (domainObject, data, ClientTransaction.Current);
    }
  }
}
