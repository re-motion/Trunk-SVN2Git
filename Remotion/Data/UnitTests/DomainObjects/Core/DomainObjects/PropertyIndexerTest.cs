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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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
      Assert.IsNotNull (accessor);
      Assert.AreSame (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"),
          accessor.PropertyData.PropertyDefinition);
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
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      IndustrialSector sector;
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        sector = IndustrialSector.NewObject ();
      }

      var indexer = new PropertyIndexer (sector);
      var accessor = indexer[typeof (IndustrialSector), "Name"];
      Assert.That (accessor.ClientTransaction, Is.SameAs (bindingTransaction));
      Assert.AreSame (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"),
          accessor.PropertyData.PropertyDefinition);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "IndustrialSector does not have a mapping property named 'Bla'.\r\nParameter name: propertyName")]
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
      Assert.AreEqual (6, order.Properties.GetPropertyCount());

      OrderItem orderItem = OrderItem.NewObject ();
      Assert.AreEqual (3, orderItem.Properties.GetPropertyCount());

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.AreEqual (44, cwadt.Properties.GetPropertyCount());
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
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"));
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("Bla"));
    }

    [Test]
    public void ShortNameAndType ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
          order.Properties[typeof (Order), "OrderNumber"]);
    }

    [Test]
    public void ShortNameAndTypeWithShadowedProperties ()
    {
      var classWithMixedProperties = (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties), ParamList.Empty);

      var indexer = new PropertyIndexer(classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (DerivedClassWithMixedProperties), "String"]);
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (ClassWithMixedProperties), "String"]);
    }

    [Test]
    public void Find ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
        order.Properties.Find (typeof (Order), "OrderNumber"));
    }

    [Test]
    public void FindFromDerivedType ()
    {
      Distributor distributor = Distributor.NewObject ();
      Assert.IsFalse (distributor.Properties.Contains (typeof (Distributor).FullName + ".ContactPerson"));
      Assert.AreEqual (distributor.Properties[typeof (Partner), "ContactPerson"], distributor.Properties.Find (typeof (Distributor), "ContactPerson"));
    }

    [Test]
    public void FindFromGenericType ()
    {
      var instance = (ClosedGenericClassWithManySideRelationProperties)
          RepositoryAccessor.NewObject (typeof (ClosedGenericClassWithManySideRelationProperties), ParamList.Empty);
      Assert.IsFalse (instance.Properties.Contains (typeof (ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional"));
      Assert.AreEqual (instance.Properties[typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>), "BaseUnidirectional"],
          instance.Properties.Find (typeof (ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional"));
    }

    [Test]
    public void FindFromDerivedType_WithoutExplicitType ()
    {
      Distributor distributor = Distributor.NewObject ();
      Assert.IsFalse (distributor.Properties.Contains (typeof (Distributor).FullName + ".ContactPerson"));
      Assert.AreEqual (distributor.Properties[typeof (Partner), "ContactPerson"], distributor.Properties.Find ("ContactPerson"));
    }

    [Test]
    public void FindWithShadowedProperty ()
    {
      var classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties), ParamList.Empty);
      
      var indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (DerivedClassWithMixedProperties), "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (ClassWithMixedProperties), "String"));
    }

    [Test]
    public void FindWithShadowedPropertyAndInferredType ()
    {
      var classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties), ParamList.Empty);

      var indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (classWithMixedProperties, "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find ((ClassWithMixedProperties) classWithMixedProperties, "String"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Distributor does not have or inherit a mapping property with the short name 'Frobbers'.", MatchType = MessageMatch.Contains)]
    public void FindNonExistingProperty ()
    {
      Distributor distributor = Distributor.NewObject ();
      distributor.Properties.Find (typeof (Distributor), "Frobbers");
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainRoot ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (order));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainIndirectRelatedObjects ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      var relatedObjects = new List<DomainObject> (ceo.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (ceo.Company.IndustrialSector));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainDuplicates ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Is.EquivalentTo (new Set<DomainObject> (relatedObjects)));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainNulls ()
    {
      Order order = Order.NewObject ();
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (null));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (order.Official));
      Assert.That (relatedObjects, List.Contains (order.OrderTicket));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectBothSides ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      var relatedObjects = new List<DomainObject> (computer.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (computer.Employee));

      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      relatedObjects = new List<DomainObject> (employee.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (employee.Computer));

    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectUnidirectional ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client2);
      var relatedObjects = new List<DomainObject> (client.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (client.ParentClient));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (order.OrderItems, Is.SubsetOf (relatedObjects));
    }

    [Test]
    public void PropertyIndexer_CachesPropertyData()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var indexer = new PropertyIndexer (order);
      Assert.That (indexer[typeof (Order), "OrderNumber"].PropertyData, Is.SameAs (indexer[typeof (Order), "OrderNumber"].PropertyData));
    }

    [Test]
    public void DomainObject_CachesPropertyIndexer()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (order.Properties, Is.SameAs (order.Properties));
    }
  }
}
