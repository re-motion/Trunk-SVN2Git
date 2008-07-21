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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DomainObjects
{
  [TestFixture]
  public class PropertyIndexerTest : ClientTransactionBaseTest
  {
    [Test]
    public void WorksForExistingProperty()
    {
      PropertyIndexer indexer = new PropertyIndexer (IndustrialSector.NewObject());
      Assert.IsNotNull (indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"]);
      Assert.AreSame (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
              .GetPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"].PropertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Remotion.Data.DomainObjects.UnitTests.TestDomain."
        + "IndustrialSector does not have a mapping property named 'Bla'.\r\nParameter name: propertyName")]
    public void ThrowsForNonExistingProperty ()
    {
      PropertyIndexer indexer = new PropertyIndexer (IndustrialSector.NewObject ());
      object o = indexer["Bla"];
    }

    [Test]
    public void Count ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (6, order.Properties.GetPropertyCount());

      OrderItem orderItem = OrderItem.NewObject ();
      Assert.AreEqual (3, orderItem.Properties.GetPropertyCount());

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.AreEqual (41, cwadt.Properties.GetPropertyCount());
    }

    [Test]
    public void GetEnumeratorGeneric ()
    {
      Order order = Order.NewObject();
      List<string> propertyNames = new List<string> ();
      foreach (PropertyAccessor propertyAccessor in (IEnumerable<PropertyAccessor>)order.Properties)
      {
        propertyNames.Add (propertyAccessor.PropertyIdentifier);
      }

      Assert.That (propertyNames, Is.EquivalentTo (new string[] {
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void GetEnumeratorNonGeneric ()
    {
      Order order = Order.NewObject ();
      List<string> propertyNames = new List<string> ();
      foreach (PropertyAccessor propertyAccessor in (IEnumerable)order.Properties)
      {
        propertyNames.Add (propertyAccessor.PropertyIdentifier);
      }

      Assert.That (propertyNames, Is.EquivalentTo (new string[] {
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void Contains ()
    {
      Order order = Order.NewObject ();
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"));
      Assert.IsTrue (order.Properties.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("Bla"));
    }

    [Test]
    public void ShortNameAndType ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
          order.Properties[typeof (Order), "OrderNumber"]);
    }

    [Test]
    public void ShortNameAndTypeWithShadowedProperties ()
    {
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();

      PropertyIndexer indexer = new PropertyIndexer(classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (DerivedClassWithMixedProperties), "String"]);
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (ClassWithMixedProperties), "String"]);
    }

    [Test]
    public void Find ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
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
      ClosedGenericClassWithManySideRelationProperties instance = (ClosedGenericClassWithManySideRelationProperties)
          RepositoryAccessor.NewObject (typeof (ClosedGenericClassWithManySideRelationProperties)).With();
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
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();
      
      PropertyIndexer indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (DerivedClassWithMixedProperties), "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (ClassWithMixedProperties), "String"));
    }

    [Test]
    public void FindWithShadowedPropertyAndInferredType ()
    {
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();

      PropertyIndexer indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (classWithMixedProperties, "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find ((ClassWithMixedProperties) classWithMixedProperties, "String"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Remotion.Data.DomainObjects.UnitTests.TestDomain."
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
      List<DomainObject> relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (order));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainIndirectRelatedObjects ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      List<DomainObject> relatedObjects = new List<DomainObject> (ceo.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (ceo.Company.IndustrialSector));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainDuplicates ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Is.EquivalentTo (new Set<DomainObject> (relatedObjects)));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainNulls ()
    {
      Order order = Order.NewObject ();
      List<DomainObject> relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (null));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (order.Official));
      Assert.That (relatedObjects, List.Contains (order.OrderTicket));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectBothSides ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      List<DomainObject> relatedObjects = new List<DomainObject> (computer.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (computer.Employee));

      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      relatedObjects = new List<DomainObject> (employee.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (employee.Computer));

    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectUnidirectional ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client2);
      List<DomainObject> relatedObjects = new List<DomainObject> (client.Properties.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (client.ParentClient));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.Properties.GetAllRelatedObjects ());
      Assert.That (order.OrderItems, Is.SubsetOf (relatedObjects));
    }
  }
}
