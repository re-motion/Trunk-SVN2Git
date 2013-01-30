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
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class PropertyIndexerTest : ClientTransactionBaseTest
  {
    [Test]
    public void Item()
    {
      var indexer = new PropertyIndexer (IndustrialSector.NewObject());
      var accessor = indexer["Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"];
      Assert.That (accessor, Is.Not.Null);
      Assert.That (
          accessor.PropertyData.PropertyDefinition,
          Is.SameAs (
              MappingConfiguration.Current.GetTypeDefinition (typeof (IndustrialSector))
                  .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name")));
    }

    [Test]
    public void Item_UsesCurrentTransaction()
    {
      var indexer = new PropertyIndexer (IndustrialSector.NewObject ());
      var accessor = indexer["Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"];
      Assert.That (accessor.ClientTransaction, Is.SameAs (ClientTransaction.Current));
    }

    [Test]
    public void Item_UsesBindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      IndustrialSector sector;
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject ();
      }

      var indexer = new PropertyIndexer (sector);
      var accessor = indexer["Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"];
      Assert.That (accessor.ClientTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    public void Item_WithSpecificTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject ();
      }

      var indexer = new PropertyIndexer (sector);
      var accessor1 = indexer["Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"];
      Assert.That (accessor1.ClientTransaction, Is.Not.SameAs (transaction));

      var accessor2 = indexer["Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name", transaction];
      Assert.That (accessor2.ClientTransaction, Is.SameAs (transaction));
    }

    [Test]
    public void Item_WithShortNotation ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction();
      IndustrialSector sector;
      using (bindingTransaction.EnterNonDiscardingScope())
      {
        sector = IndustrialSector.NewObject();
      }

      var indexer = new PropertyIndexer (sector);
      var accessor = indexer[typeof (IndustrialSector), "Name"];
      Assert.That (accessor.ClientTransaction, Is.SameAs (bindingTransaction));
      Assert.That (
          accessor.PropertyData.PropertyDefinition,
          Is.SameAs (
              MappingConfiguration.Current.GetTypeDefinition (typeof (IndustrialSector))
                  .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name")));
    }

    [Test]
    public void Item_WithShortNotation_WithSpecificTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject ();
      }

      var indexer = new PropertyIndexer (sector);
      var accessor1 = indexer[typeof (IndustrialSector), "Name"];
      Assert.That (accessor1.ClientTransaction, Is.Not.SameAs (transaction));

      var accessor2 = indexer[typeof (IndustrialSector), "Name", transaction];
      Assert.That (accessor2.ClientTransaction, Is.SameAs (transaction));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector' does not have a mapping property named 'Bla'.")]
    public void Item_ThrowsForNonExistingProperty ()
    {
      var indexer = new PropertyIndexer (IndustrialSector.NewObject ());
      Dev.Null = indexer["Bla"];
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void Item_ThrowsForNullCurrentTransaction ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      var indexer = new PropertyIndexer (sector);

      using (ClientTransactionScope.EnterNullScope ())
      {
        Dev.Null = indexer[typeof (IndustrialSector), "Name"];
      }
    }

    [Test]
    public void Count ()
    {
      Order order = Order.NewObject ();
      Assert.That (order.Properties.GetPropertyCount (), Is.EqualTo (6));

      OrderItem orderItem = OrderItem.NewObject ();
      Assert.That (orderItem.Properties.GetPropertyCount (), Is.EqualTo (3));

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.That (cwadt.Properties.GetPropertyCount (), Is.EqualTo (46));
    }

    [Test]
    public void AsEnumerable_GetsAllProperties ()
    {
      Order order = Order.NewObject ();
      var propertyNames = (from propertyAccessor in order.Properties.AsEnumerable()
                           select propertyAccessor.PropertyData.PropertyIdentifier).ToArray ();

      Assert.That (propertyNames, Is.EquivalentTo (new[] {
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber",
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate",
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official",
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket",
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer",
        "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void AsEnumerable_DefaultTransaction_Current ()
    {
      Order order = Order.NewObject();

      var transactions = (from propertyAccessor in order.Properties.AsEnumerable ()
                          select propertyAccessor.ClientTransaction).Distinct ().ToArray ();
      Assert.That (transactions, Is.EqualTo (new[] { ClientTransaction.Current }));
    }

    [Test]
    public void AsEnumerable_DefaultTransaction_Binding ()
    {
      Order order;
      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        order = Order.NewObject();
      }

      var transactions = (from propertyAccessor in order.Properties.AsEnumerable ()
                          select propertyAccessor.ClientTransaction).Distinct ().ToArray ();
      Assert.That (transactions, Is.EqualTo (new[] { order.GetBindingTransaction() }));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "cannot be used in the given transaction ")]
    public void AsEnumerable_DefaultTransaction_WrongTransaction ()
    {
      Order order;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        order = Order.NewObject ();
      }

      (from propertyAccessor in order.Properties.AsEnumerable ()
       select propertyAccessor.ClientTransaction).ToArray ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void AsEnumerable_DefaultTransaction_NullTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransactionScope.EnterNullScope())
      {
        (from propertyAccessor in order.Properties.AsEnumerable ()
         select propertyAccessor.ClientTransaction).ToArray ();
      }
    }

    [Test]
    public void AsEnumerable_SpecificTransaction ()
    {
      Order order;
      ClientTransaction transaction = ClientTransaction.CreateRootTransaction ();
      using (transaction.EnterNonDiscardingScope ())
      {
        order = Order.NewObject();
      }

      var transactions = (from propertyAccessor in order.Properties.AsEnumerable (transaction)
                          select propertyAccessor.ClientTransaction).Distinct ().ToArray ();
      Assert.That (transactions, Is.EqualTo (new[] { transaction }));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "cannot be used in the given transaction ")]
    public void AsEnumerable_InvalidTransaction ()
    {
      Order order = Order.NewObject ();

      (from propertyAccessor in order.Properties.AsEnumerable (ClientTransaction.CreateRootTransaction ())
       select propertyAccessor.ClientTransaction).ToArray ();
    }

    [Test]
    public void Contains ()
    {
      Order order = Order.NewObject ();
      Assert.That (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"), Is.True);
      Assert.That (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"), Is.True);
      Assert.That (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"), Is.True);
      Assert.That (order.Properties.Contains ("OrderTicket"), Is.False);
      Assert.That (order.Properties.Contains ("Bla"), Is.False);
    }

    [Test]
    public void ShortNameAndType ()
    {
      Order order = Order.NewObject ();
      Assert.That (
                  order.Properties[typeof (Order), "OrderNumber"], Is.EqualTo (order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void ShortNameAndTypeWithShadowedProperties ()
    {
      var classWithMixedProperties = (DerivedClassWithDifferentProperties) 
          LifetimeService.NewObject (TestableClientTransaction, typeof (DerivedClassWithDifferentProperties), ParamList.Empty);

      var indexer = new PropertyIndexer(classWithMixedProperties);
      Assert.That (
                  indexer[typeof (DerivedClassWithDifferentProperties), "String"], Is.EqualTo (indexer[typeof (DerivedClassWithDifferentProperties).FullName + ".String"]));
      Assert.That (
                  indexer[typeof (ClassWithDifferentProperties), "String"], Is.EqualTo (indexer[typeof (ClassWithDifferentProperties).FullName + ".String"]));
    }

    [Test]
    public void Find_Property ()
    {
      Order order = Order.NewObject ();
      var result = order.Properties.Find (typeof (Order), "OrderNumber");
      Assert.That (result, Is.EqualTo (order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void Find_VirtualRelationEndPoint ()
    {
      Order order = Order.NewObject ();
      var result = order.Properties.Find (typeof (Order), "OrderItems");
      Assert.That (result, Is.EqualTo (order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"]));
    }

    [Test]
    public void Find_WithoutExplicitType ()
    {
      Distributor distributor = Distributor.NewObject ();
      Assert.That (distributor.Properties.Contains (typeof (Distributor).FullName + ".ContactPerson"), Is.False);
      Assert.That (distributor.Properties.Find ("ContactPerson"), Is.EqualTo (distributor.Properties[typeof (Partner), "ContactPerson"]));
    }

    [Test]
    public void Find_Generic_WithInferredType ()
    {
      var classWithMixedProperties =
          (DerivedClassWithDifferentProperties) LifetimeService.NewObject (TestableClientTransaction, typeof (DerivedClassWithDifferentProperties), ParamList.Empty);
      var indexer = new PropertyIndexer (classWithMixedProperties);
      
      var resultOnDerived = indexer.Find (classWithMixedProperties, "String");
      Assert.That (resultOnDerived, Is.EqualTo (indexer[typeof (DerivedClassWithDifferentProperties).FullName + ".String"]));

      var resultOnBase = indexer.Find ((ClassWithDifferentProperties) classWithMixedProperties, "String");
      Assert.That (resultOnBase, Is.EqualTo (indexer[typeof (ClassWithDifferentProperties).FullName + ".String"]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The domain object type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor' does not have or inherit a mapping property with the "
        + "short name 'Frobbers'.", MatchType = MessageMatch.Contains)]
    public void Find_NonExistingProperty ()
    {
      Distributor distributor = Distributor.NewObject ();
      distributor.Properties.Find (typeof (Distributor), "Frobbers");
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainRoot ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.No.Member(order));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainIndirectRelatedObjects ()
    {
      Ceo ceo = DomainObjectIDs.Ceo1.GetObject<Ceo> ();
      var relatedObjects = new List<DomainObject> (ceo.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.No.Member(ceo.Company.IndustrialSector));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainDuplicates ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Is.EquivalentTo (new Set<DomainObject> (relatedObjects)));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainNulls ()
    {
      Order order = Order.NewObject ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.No.Member(null));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.Member (order.Official));
      Assert.That (relatedObjects, Has.Member (order.OrderTicket));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectBothSides ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      var relatedObjects = new List<DomainObject> (computer.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.Member (computer.Employee));

      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      relatedObjects = new List<DomainObject> (employee.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.Member (employee.Computer));

    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectUnidirectional ()
    {
      Client client = DomainObjectIDs.Client2.GetObject<Client> ();
      var relatedObjects = new List<DomainObject> (client.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Has.Member (client.ParentClient));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjects ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (order.OrderItems, Is.SubsetOf (relatedObjects));
    }

    [Test]
    public void PropertyIndexer_CachesPropertyData()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      var indexer = new PropertyIndexer (order);
      Assert.That (indexer[typeof (Order), "OrderNumber"].PropertyData, Is.SameAs (indexer[typeof (Order), "OrderNumber"].PropertyData));
    }

    [Test]
    public void DomainObject_CachesPropertyIndexer()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.That (order.Properties, Is.SameAs (order.Properties));
    }
  }
}
