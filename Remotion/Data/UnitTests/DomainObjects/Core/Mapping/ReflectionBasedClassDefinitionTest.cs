// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _orderClass;
    private ReflectionBasedClassDefinition _distributorClass;
    
    private ReflectionBasedClassDefinition _targetClassForPersistentMixinClass;
    private ReflectionBasedClassDefinition _derivedTargetClassForPersistentMixinClass;

    public override void SetUp ()
    {
      base.SetUp();

      _orderClass = (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _distributorClass = (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];

      _targetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          (ReflectionBasedClassDefinition) FakeMappingConfiguration.Current.ClassDefinitions[typeof (DerivedTargetClassForPersistentMixin)];
    }

    [Test]
    public void Initialize ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.MyEntityName, Is.EqualTo ("OrderTable"));
      Assert.That (actual.GetViewName(), Is.EqualTo ("OrderView"));
      Assert.That (actual.StorageProviderID, Is.EqualTo ("StorageProvider"));
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
      Assert.That (actual.IsReadOnly, Is.False);
    }

    [Test]
    public void SetReadOnly ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      Assert.That (actual.IsReadOnly, Is.False);

      actual.SetReadOnly();

      Assert.That (actual.IsReadOnly, Is.True);
    }

    [Test]
    public void SetReadOnly_SetsCollectionsReadOnly ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      Assert.That (actual.MyPropertyDefinitions.IsReadOnly, Is.False);
      Assert.That (actual.MyRelationDefinitions.IsReadOnly, Is.False);
      
      actual.SetReadOnly();

      Assert.That (actual.MyPropertyDefinitions.IsReadOnly, Is.True);
      Assert.That (actual.MyRelationDefinitions.IsReadOnly, Is.True);
      Assert.That (actual.DerivedClasses.IsReadOnly, Is.True);
    }

    [Test]
    public void GetToString ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderID", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ReflectionBasedClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (AbstractClass), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentFalse ()
    {
      ReflectionBasedClassDefinition actual =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (AbstractClass), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentTrue ()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassID", "Table", "StorageProvider", typeof (Order), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetRelationDefinition ()
    {
      RelationDefinition relation =
          _orderClass.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:Remotion.Data.UnitTests.DomainObjects.Core."
        +"Mapping.TestDomain.Integration.Order.Customer->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders", 
        relation.ID);
    }

    [Test]
    public void GetUndefinedRelationDefinition ()
    {
      Assert.IsNull (
          _orderClass.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    public void GetAllRelationDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "ClassDefinition must be read-only when retrieving data that spans the inheritance hierarchy.")]
    public void GetAllRelationDefinitions_ThrowsWhenNotReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.GetRelationDefinitions();
    }

    [Test]
    public void GetAllRelationDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetRelationDefinitions();
      var result2 = classDefinition.GetRelationDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllRelationDefinitions_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    public void GetAllRelationDefinitions_Contents ()
    {
      RelationDefinitionCollection relations = _distributorClass.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (5, relations.Count);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo:Remotion.Data.UnitTests.DomainObjects."
        +"Core.Mapping.TestDomain.Integration.Ceo.Company->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"]);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner:Remotion.Data.UnitTests.DomainObjects."
        +"Core.Mapping.TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"]);
      Assert.IsNotNull (
          relations[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn:Remotion.Data.UnitTests."
              +"DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn.Distributor->Remotion.Data.UnitTests.DomainObjects."
              +"Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn"]);
      Assert.IsNotNull (
          relations[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation.Company->"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation"]);
      Assert.IsNotNull (
        relations["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company:"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector->"
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.IndustrialSector.Companies"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyRelationDefinition ()
    {
      _orderClass.GetRelationDefinition (string.Empty);
    }

    [Test]
    public void GetRelationDefinitionWithInheritance ()
    {
      Assert.IsNotNull (
          _distributorClass.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"));
      Assert.IsNotNull (
          _distributorClass.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
    }

    [Test]
    public void GetRelatedClassDefinition ()
    {
      Assert.IsNotNull (
          _distributorClass.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"));
      Assert.IsNotNull (
          _distributorClass.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelatedClassDefinitionWithEmtpyPropertyName ()
    {
      _distributorClass.GetOppositeClassDefinition (string.Empty);
    }

    [Test]
    public void GetRelationEndPointDefinition ()
    {
      Assert.IsNotNull (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo"));
      Assert.IsNotNull (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelationEndPointDefinitionFromEmptyPropertyName ()
    {
      _orderClass.GetRelationEndPointDefinition (string.Empty);
    }

    [Test]
    public void IsRelationEndPointTrue ()
    {
      RelationDefinition orderToOrderItem =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              +"TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"];
      IRelationEndPointDefinition endPointDefinition =
          orderToOrderItem.GetEndPointDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems");

      Assert.IsTrue (_orderClass.IsRelationEndPoint (endPointDefinition));
    }

    [Test]
    public void IsRelationEndPointFalse ()
    {
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              +"TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");

      Assert.IsFalse (_orderClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsRelationEndPointWithNull ()
    {
      _orderClass.IsRelationEndPoint (null);
    }

    [Test]
    public void IsRelationEndPointWithInheritance ()
    {
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");

      Assert.IsTrue (_distributorClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      Assert.IsNotNull (
          _orderClass.GetPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyPropertyDefinition ()
    {
      _orderClass.GetPropertyDefinition (string.Empty);
    }

    [Test]
    public void GetInheritedPropertyDefinition ()
    {
      Assert.IsNotNull (
          _distributorClass.GetPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson"));
    }

    [Test]
    public void GetAllPropertyDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "ClassDefinition must be read-only when retrieving data that spans the inheritance hierarchy.")]
    public void GetAllPropertyDefinitions_ThrowsWhenNotReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.GetPropertyDefinitions();
    }

    [Test]
    public void GetAllPropertyDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetPropertyDefinitions();
      var result2 = classDefinition.GetPropertyDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllPropertyDefinitions_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassNotDerivedFromDomainObject' of class 'Company' is not derived from "
        + "'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeWithInvalidDerivation ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (ClassNotDerivedFromDomainObject), false);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Type 'Remotion.Data.DomainObjects.DomainObject' of class 'Company' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeDomainObject ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (DomainObject), false);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Company', because it already defines a property with the same name.")]
    public void AddDuplicateProperty ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);

      companyClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));
      companyClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);
      ReflectionBasedClassDefinition orderClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", "TestDomain", typeof (Order), false);

      ReflectionBasedPropertyDefinition propertyDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      orderClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Company', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClassWithSameID ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);
      ReflectionBasedClassDefinition otherCompanyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedPropertyDefinition propertyDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      otherCompanyClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);
      companyClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));

      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "Company", "TestDomain", typeof (Customer), false, companyClass);
      customerClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (customerClass, "Name", "Name"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", "TestDomain", typeof (Company), false);
      companyClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name"));

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      ReflectionBasedClassDefinition supplierClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass);

      supplierClass.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (supplierClass, "Name", "Name"));
    }

    [Test]
    public void ConstructorWithoutBaseClass ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "No relation found for class 'Order' and property 'UndefinedProperty'.")]
    public void GetMandatoryRelationEndPointDefinitionForUndefinedProperty ()
    {
      _orderClass.GetMandatoryRelationEndPointDefinition ("UndefinedProperty");
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition ()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition =
          _orderClass.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket");
      Assert.IsNotNull (oppositeEndPointDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order",
          oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "ClassDefinition must be read-only when retrieving data that spans the inheritance hierarchy.")]
    public void GetAllRelationEndPointDefinitions_ThrowsWhenNotReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.GetRelationEndPointDefinitions();
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetRelationEndPointDefinitions();
      var result2 = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllRelationEndPointDefinitionss_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", "StorageProvider", typeof (Order), false);
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Content ()
    {
      var relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition customerEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer");
      IRelationEndPointDefinition orderTicketEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems");
      IRelationEndPointDefinition officialEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official");

      Assert.That (
          relationEndPointDefinitions, Is.EquivalentTo (new[] { customerEndPoint, orderTicketEndPoint, orderItemsEndPoint, officialEndPoint }));
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetMyRelationEndPointDefinitions();

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (1, relationEndPointDefinitions.Length);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn",
          relationEndPointDefinitions[0].PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance ()
    {
      var relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition classWithoutRelatedClassIDColumnEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn");
      IRelationEndPointDefinition contactPersonEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson");
      IRelationEndPointDefinition ceoEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo");
      IRelationEndPointDefinition classWithoutRelatedClassIDColumnAndDerivationEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation");
      IRelationEndPointDefinition industrialSectorEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector");

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.That (
          relationEndPointDefinitions,
          Is.EquivalentTo (
              new[]
              {
                  classWithoutRelatedClassIDColumnEndPoint,
                  contactPersonEndPoint,
                  ceoEndPoint,
                  classWithoutRelatedClassIDColumnAndDerivationEndPoint,
                  industrialSectorEndPoint
              }));
    }

    [Test]
    public void GetDerivedClassesWithoutInheritance ()
    {
      Assert.IsNotNull (_orderClass.DerivedClasses);
      Assert.AreEqual (0, _orderClass.DerivedClasses.Count);
      Assert.IsTrue (_orderClass.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void GetDerivedClassesWithInheritance ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Partner"));
      Assert.IsTrue (companyDefinition.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinitionCollection clientRelations = clientDefinition.GetRelationDefinitions();

      Assert.AreEqual (1, clientRelations.Count);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient", clientRelations[0].ID);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinition parentClient =
          MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              +"TestDomain.Integration.Client.ParentClient"];
      var clientAnonymousEndPointDefinition = (AnonymousRelationEndPointDefinition) parentClient.GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient"));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition folderEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder");

      Assert.IsTrue (folderDefinition.IsMyRelationEndPoint (folderEndPoint));
      Assert.IsFalse (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      var endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Count());
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      var endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (2, endPointDefinitions.Count());
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      RelationDefinitionCollection relations = fileSystemItemDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (
          relations["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem:Remotion.Data.UnitTests.DomainObjects."
          + "Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder->Remotion.Data.UnitTests.DomainObjects."
          + "Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"]);
    }

    [Test]
    public void GetRelationDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      RelationDefinitionCollection relations = folderDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (
          relations["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem:Remotion.Data.UnitTests.DomainObjects."
          + "Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder->Remotion.Data.UnitTests.DomainObjects."
          + "Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"]);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.AreSame (
          folderDefinition,
          folderDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.AreSame (
          fileSystemItemDefinition,
          folderDefinition.GetOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'Invalid'.")]
    public void GetMandatoryOppositeEndPointDefinition_InvalidProperty ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("Invalid");
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeEndPointDefinition_InvalidProperty ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNull (fileSystemItemDefinition.GetOppositeEndPointDefinition ("Invalid"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeEndPointDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetMandatoryOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryRelationDefinition ()
    {
      RelationDefinition relation =
          _orderClass.GetMandatoryRelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:Remotion.Data.UnitTests.DomainObjects.Core."
        +"Mapping.TestDomain.Integration.Order.Customer->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders", 
        relation.ID);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'Order' and property 'InvalidProperty'.")]
    public void GetMandatoryRelationDefinitionWithInvalidPropertyName ()
    {
      _orderClass.GetMandatoryRelationDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryPropertyDefinition ()
    {
      Assert.IsNotNull (
          _orderClass.GetMandatoryPropertyDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Order' does not contain the property 'InvalidProperty'.")]
    public void GetMandatoryPropertyDefinitionWithInvalidPropertName ()
    {
      _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
    }

    [Test]
    public void SetClassDefinitionOfPropertyDefinition ()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", "TestDomain", typeof (Order), false);

      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "Test", "Test");
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void CancelAddingOfPropertyDefinition ()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", "TestDomain", typeof (Order), false);

      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Test", "Test");
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      new PropertyDefinitionCollectionEventReceiver (classDefinition.MyPropertyDefinitions, true);
      try
      {
        classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
        Assert.Fail ("Expected an EventReceiverCancelException.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
      }
    }

    [Test]
    public void Contains ()
    {
      Assert.IsFalse (
          _orderClass.Contains (
              ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
                  _orderClass, "PropertyName", "ColumnName")));
      Assert.IsTrue (
          _orderClass.Contains (
              _orderClass["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
    }

    [Test]
    public void PropertyDefinitionCollectionBackLink ()
    {
      Assert.AreSame (_orderClass, _orderClass.MyPropertyDefinitions.ClassDefinition);
    }

    [Test]
    public void GetInheritanceRootClass ()
    {
      ClassDefinition expected = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass());
    }

    [Test]
    public void GetAllDerivedClasses ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      ClassDefinitionCollection allDerivedClasses = companyClass.GetAllDerivedClasses();
      Assert.IsNotNull (allDerivedClasses);
      Assert.AreEqual (4, allDerivedClasses.Count);

      Assert.IsTrue (allDerivedClasses.Contains (typeof (Customer)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Partner)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Supplier)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Distributor)));
    }

    [Test]
    public void IsSameOrBaseClassOfFalse ()
    {
      Assert.IsFalse (_orderClass.IsSameOrBaseClassOf (_distributorClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithSameClass ()
    {
      Assert.IsTrue (_orderClass.IsSameOrBaseClassOf (_orderClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithBaseClass ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      Assert.IsTrue (companyClass.IsSameOrBaseClassOf (_distributorClass));
    }

    private bool Contains (IEnumerable<IRelationEndPointDefinition> endPointDefinitions, string propertyName)
    {
      return endPointDefinitions.Any (endPointDefinition => endPointDefinition.PropertyName == propertyName);
    }

    [Test]
    public void PropertyInfoWithSimpleProperty ()
    {
      PropertyInfo property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition =
          (ReflectionBasedPropertyDefinition) _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.AreEqual (property, propertyDefinition.PropertyInfo);
    }

    [Instantiable]
    [DBTable]
    public abstract class Base : DomainObject
    {
      public int Name
      {
        get { return Properties[typeof (Base), "Name"].GetValue<int>(); }
      }
    }

    [Instantiable]
    public abstract class Shadower : Base
    {
      [DBColumn ("NewName")]
      public new int Name
      {
        get { return Properties[typeof (Shadower), "Name"].GetValue<int>(); }
      }
    }

    [Test]
    public void PropertyInfoWithShadowedProperty ()
    {
      var property1 = typeof (Shadower).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var property2 = typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (Arg<Type>.Is.Anything, Arg<bool>.Is.Anything))
          .Return (new[] { typeof (Base), typeof (Shadower) });

      var mappingReflector = new MappingReflector (typeDiscoveryServiceStub);
      ClassDefinition classDefinition1 = new ClassDefinitionCollection(mappingReflector.GetClassDefinitions(), true, true)[typeof (Shadower)];
      ClassDefinition classDefinition2 = classDefinition1.BaseClass;

      var propertyDefinition1 =
          (ReflectionBasedPropertyDefinition)
          classDefinition1.GetMandatoryPropertyDefinition (property1.DeclaringType.FullName + "." + property1.Name);
      var propertyDefinition2 =
          (ReflectionBasedPropertyDefinition)
          classDefinition2.GetMandatoryPropertyDefinition (property2.DeclaringType.FullName + "." + property2.Name);

      Assert.AreEqual (property1, propertyDefinition1.PropertyInfo);
      Assert.AreEqual (property2, propertyDefinition2.PropertyInfo);
    }

    [Test]
    public void CreatorIsFactoryBasedCreator ()
    {
      Assert.AreEqual (InterceptedDomainObjectCreator.Instance, _orderClass.GetDomainObjectCreator());
    }

    [Test]
    public void PersistentMixinFinder ()
    {
      var mixinFinder = new PersistentMixinFinderMock (typeof (Order));
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, mixinFinder);
      Assert.That (classDefinition.PersistentMixinFinder, Is.SameAs (mixinFinder));
    }

    [Test]
    public void PersistentMixins_Empty ()
    {
      var mixins = new Type[0];
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }

    [Test]
    public void PersistentMixins_NonEmpty ()
    {
      var mixins = new[] { typeof (MixinA), typeof (MixinB) };
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("x", "xx", "xxx", typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }

    

    [Test]
    public void ResolveProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter(property));

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_Twice_ReturnsSamePropertyDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result1 = _orderClass.ResolveProperty (new PropertyInfoAdapter(property));
      var result2 = _orderClass.ResolveProperty (new PropertyInfoAdapter(property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveProperty_StorageClassNoneProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter(property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveProperty_MixinProperty ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _targetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter(property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_MixinPropertyOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter(property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToOne ()
    {
      var property = typeof (Order).GetProperty ("OrderTicket");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToMany ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_NonEndPointProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveRelationEndPoint_Twice_ReturnsSameRelationDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result1 = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));
      var result2 = _orderClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      Assert.That (result1, Is.SameAs (result2));
    }
    
    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_VirtualEndPoint ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");

      var result = _targetClassForPersistentMixinClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_DefinedOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveRelationEndPoint (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }
  }
}