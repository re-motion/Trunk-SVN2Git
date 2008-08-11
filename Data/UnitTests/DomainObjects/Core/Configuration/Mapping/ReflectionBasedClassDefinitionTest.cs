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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _orderClass;
    private ReflectionBasedClassDefinition _distributorClass;

    public override void SetUp()
    {
      base.SetUp();

      _orderClass = (ReflectionBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _distributorClass = (ReflectionBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)];
    }

    [Test]
    public void Initialize()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.MyEntityName, Is.EqualTo ("OrderTable"));
      Assert.That (actual.GetViewName(), Is.EqualTo ("OrderView"));
      Assert.That (actual.StorageProviderID, Is.EqualTo ("StorageProvider"));
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
    }

    [Test]
    public void GetToString()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrderID", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ReflectionBasedClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (Order), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProvider", typeof (AbstractClass), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentFalse()
    {
      ReflectionBasedClassDefinition actual = 
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (AbstractClass), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentTrue()
    {
      ReflectionBasedClassDefinition actual = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassID", "Table", "StorageProvider", typeof (Order), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetRelationDefinition()
    {
      RelationDefinition relation = _orderClass.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", relation.ID);
    }

    [Test]
    public void GetUndefinedRelationDefinition()
    {
      Assert.IsNull (_orderClass.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void GetAllRelationDefinitions()
    {
      RelationDefinitionCollection relations = _distributorClass.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (5, relations.Count);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company"]);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"]);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor"]);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company"]);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyRelationDefinition()
    {
      _orderClass.GetRelationDefinition (string.Empty);
    }

    [Test]
    public void GetRelationDefinitionWithInheritance()
    {
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    public void GetRelatedClassDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelatedClassDefinitionWithEmtpyPropertyName()
    {
      _distributorClass.GetOppositeClassDefinition (string.Empty);
    }

    [Test]
    public void GetRelationEndPointDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo"));
      Assert.IsNotNull (_distributorClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetRelationEndPointDefinitionFromEmptyPropertyName()
    {
      _orderClass.GetRelationEndPointDefinition (string.Empty);
    }

    [Test]
    public void IsRelationEndPointTrue()
    {
      RelationDefinition orderToOrderItem =
          TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"];
      IRelationEndPointDefinition endPointDefinition =
          orderToOrderItem.GetEndPointDefinition ("Order", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

      Assert.IsTrue (_orderClass.IsRelationEndPoint (endPointDefinition));
    }

    [Test]
    public void IsRelationEndPointFalse()
    {
      RelationDefinition partnerToPerson =
          TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition ("Partner", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson");

      Assert.IsFalse (_orderClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsRelationEndPointWithNull()
    {
      _orderClass.IsRelationEndPoint (null);
    }

    [Test]
    public void IsRelationEndPointWithInheritance()
    {
      RelationDefinition partnerToPerson =
          TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition ("Partner", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson");

      Assert.IsTrue (_distributorClass.IsRelationEndPoint (partnerEndPoint));
    }

    [Test]
    public void GetPropertyDefinition()
    {
      Assert.IsNotNull (_orderClass.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void GetEmptyPropertyDefinition()
    {
      _orderClass.GetPropertyDefinition (string.Empty);
    }

    [Test]
    public void GetInheritedPropertyDefinition()
    {
      Assert.IsNotNull (_distributorClass.GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassNotDerivedFromDomainObject' of class 'Company' is not derived from "
        + "'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeWithInvalidDerivation()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (ClassNotDerivedFromDomainObject), false);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DomainObject' of class 'Company' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void ClassTypeDomainObject()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (DomainObject), false);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "Cannot derive class 'Customer' from base class 'Company' handled by different StorageProviders.")]
    public void BaseClassWithDifferentStorageProvider()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "Provider 1", typeof (Company), false);
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Company", "Provider 2", typeof (Customer), false, companyClass);
    }

    [Test]
    public void ClassTypeIsNotDerivedFromBaseClassType()
    {
      ReflectionBasedClassDefinition orderClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order",c_testDomainProviderID, typeof (Order), false);

      try
      {
        ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Distributor", "Company",c_testDomainProviderID, typeof (Distributor), false, orderClass);
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.",
            typeof (Distributor).AssemblyQualifiedName,
            "Distributor",
            orderClass.ClassType.AssemblyQualifiedName,
            orderClass.ID);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Company', because it already defines a property with the same name.")]
    public void AddDuplicateProperty()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);
      ReflectionBasedClassDefinition orderClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      ReflectionBasedPropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100);
      orderClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Company', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClassWithSameID ()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);
      ReflectionBasedClassDefinition otherCompanyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      ReflectionBasedPropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100);
      otherCompanyClass.MyPropertyDefinitions.Add (propertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Property 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Company", "TestDomain", typeof (Customer), false, companyClass);
      customerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(customerClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Property 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass()
    {
      ReflectionBasedClassDefinition companyClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);
      companyClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(companyClass, "Name", "Name", typeof (string), 100));

      ReflectionBasedClassDefinition partnerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Partner", "Company", "TestDomain", typeof (Partner), false, companyClass);

      ReflectionBasedClassDefinition supplierClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Supplier", "Company", "TestDomain", typeof (Supplier), false, partnerClass);

      supplierClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(supplierClass, "Name", "Name", typeof (string), 100));
    }

    [Test]
    public void ConstructorWithoutBaseClass()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company), false);

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "No relation found for class 'Order' and property 'UndefinedProperty'.")]
    public void GetMandatoryRelationEndPointDefinitionForUndefinedProperty()
    {
      _orderClass.GetMandatoryRelationEndPointDefinition ("UndefinedProperty");
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinition()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition =
          _orderClass.GetMandatoryOppositeEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      Assert.IsNotNull (oppositeEndPointDefinition);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition customerEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");
      IRelationEndPointDefinition orderTicketEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      IRelationEndPointDefinition officialEndPoint =
          _orderClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official");

      Assert.AreEqual (4, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, customerEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderTicketEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, orderItemsEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, officialEndPoint) >= 0);
    }

    [Test]
    public void GetRelationEndPointDefinitions()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetMyRelationEndPointDefinitions();

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (1, relationEndPointDefinitions.Length);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn", relationEndPointDefinitions[0].PropertyName);
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance()
    {
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition classWithoutRelatedClassIDColumnEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn");
      IRelationEndPointDefinition contactPersonEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Partner.ContactPerson");
      IRelationEndPointDefinition ceoEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo");
      IRelationEndPointDefinition classWithoutRelatedClassIDColumnAndDerivationEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation");
      IRelationEndPointDefinition industrialSectorEndPoint =
          _distributorClass.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector");

      Assert.IsNotNull (relationEndPointDefinitions);
      Assert.AreEqual (5, relationEndPointDefinitions.Length);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, contactPersonEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, ceoEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, classWithoutRelatedClassIDColumnAndDerivationEndPoint) >= 0);
      Assert.IsTrue (Array.IndexOf (relationEndPointDefinitions, industrialSectorEndPoint) >= 0);
    }

    [Test]
    public void GetDerivedClassesWithoutInheritance()
    {
      Assert.IsNotNull (_orderClass.DerivedClasses);
      Assert.AreEqual (0, _orderClass.DerivedClasses.Count);
      Assert.IsTrue (_orderClass.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void GetDerivedClassesWithInheritance()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Contains ("Partner"));
      Assert.IsTrue (companyDefinition.DerivedClasses.IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Company));

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void GetRelationDefinitions()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinitionCollection clientRelations = clientDefinition.GetRelationDefinitions();

      Assert.AreEqual (1, clientRelations.Count);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient", clientRelations[0].ID);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      RelationDefinition parentClient =
          MappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"];
      AnonymousRelationEndPointDefinition clientAnonymousEndPointDefinition =
          (AnonymousRelationEndPointDefinition) parentClient.GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void GetMyRelationEndPointDefinitions()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client));

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition folderEndPoint =
          folderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint =
          folderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder");

      Assert.IsTrue (folderDefinition.IsMyRelationEndPoint (folderEndPoint));
      Assert.IsFalse (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint));
    }

    [Test]
    public void GetMyRelationEndPointDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetMyRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions();

      Assert.AreEqual (2, endPointDefinitions.Length);
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
      Assert.IsTrue (Contains (endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetRelationDefinitionsCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      RelationDefinitionCollection relations = fileSystemItemDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"]);
    }

    [Test]
    public void GetRelationDefinitionsCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      RelationDefinitionCollection relations = folderDefinition.GetRelationDefinitions();

      Assert.IsNotNull (relations);
      Assert.AreEqual (1, relations.Count);
      Assert.IsNotNull (relations["Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"]);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (folderDefinition.GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (fileSystemItemDefinition.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.AreSame (
          folderDefinition,
          folderDefinition.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.AreSame (
          fileSystemItemDefinition,
          folderDefinition.GetOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNull (
          fileSystemItemDefinition.GetOppositeEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetMandatoryOppositeEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder.FileSystemItems"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeEndPointDefinitionWithInvalidPropertyName()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));
      ClassDefinition folderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Folder));

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (FileSystemItem));

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryRelationDefinition()
    {
      RelationDefinition relation = _orderClass.GetMandatoryRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");

      Assert.IsNotNull (relation);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", relation.ID);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'Order' and property 'InvalidProperty'.")]
    public void GetMandatoryRelationDefinitionWithInvalidPropertyName()
    {
      _orderClass.GetMandatoryRelationDefinition ("InvalidProperty");
    }

    [Test]
    public void GetMandatoryPropertyDefinition()
    {
      Assert.IsNotNull (_orderClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Order' does not contain the property 'InvalidProperty'.")]
    public void GetMandatoryPropertyDefinitionWithInvalidPropertName()
    {
      _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
    }

    [Test]
    public void SetClassDefinitionOfPropertyDefinition()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
     
      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Test", "Test", typeof (int));
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void CancelAddingOfPropertyDefinition()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      PropertyDefinition propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Test", "Test", typeof (int));
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
    public void Contains()
    {
      Assert.IsFalse (_orderClass.Contains (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_orderClass, "PropertyName", "ColumnName", typeof (int))));
      Assert.IsTrue (_orderClass.Contains (_orderClass["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void PropertyDefinitionCollectionBackLink()
    {
      Assert.AreSame (_orderClass, _orderClass.MyPropertyDefinitions.ClassDefinition);
    }

    [Test]
    public void GetInheritanceRootClass()
    {
      ClassDefinition expected = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass());
    }

    [Test]
    public void GetAllDerivedClasses()
    {
      ClassDefinition companyClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      ClassDefinitionCollection allDerivedClasses = companyClass.GetAllDerivedClasses();
      Assert.IsNotNull (allDerivedClasses);
      Assert.AreEqual (4, allDerivedClasses.Count);

      Assert.IsTrue (allDerivedClasses.Contains (typeof (Customer)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Partner)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Supplier)));
      Assert.IsTrue (allDerivedClasses.Contains (typeof (Distributor)));
    }

    [Test]
    public void IsSameOrBaseClassOfFalse()
    {
      Assert.IsFalse (_orderClass.IsSameOrBaseClassOf (_distributorClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithSameClass()
    {
      Assert.IsTrue (_orderClass.IsSameOrBaseClassOf (_orderClass));
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithBaseClass()
    {
      ClassDefinition companyClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Company)];

      Assert.IsTrue (companyClass.IsSameOrBaseClassOf (_distributorClass));
    }

    private bool Contains (IRelationEndPointDefinition[] endPointDefinitions, string propertyName)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in endPointDefinitions)
      {
        if (endPointDefinition.PropertyName == propertyName)
          return true;
      }

      return false;
    }

    [Test]
    public void PropertyInfoWithSimpleProperty ()
    {
      PropertyInfo property = typeof (Order).GetProperty ("OrderNumber");
      ReflectionBasedPropertyDefinition propertyDefinition =
          (ReflectionBasedPropertyDefinition) _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.AreEqual (property, propertyDefinition.PropertyInfo);
    }

    [Instantiable]
    [DBTable]
    public abstract class Base : DomainObject
    {
      public int Name
      {
        get { return Properties[typeof (Base), "Name"].GetValue<int> (); }
      }
    }

    [Instantiable]
    public abstract class Shadower : Base
    {
      [DBColumn ("NewName")]
      public new int Name
      {
        get { return Properties[typeof (Shadower), "Name"].GetValue<int> (); }
      }
    }

    [Test]
    public void PropertyInfoWithShadowedProperty ()
    {
      var property1 = typeof (Shadower).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      var property2 = typeof (Base).GetProperty ("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      ClassDefinition classDefinition1 = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Shadower));
      ClassDefinition classDefinition2 = classDefinition1.BaseClass;

      var propertyDefinition1 =
          (ReflectionBasedPropertyDefinition) classDefinition1.GetMandatoryPropertyDefinition (property1.DeclaringType.FullName + "." + property1.Name);
      var propertyDefinition2 =
          (ReflectionBasedPropertyDefinition) classDefinition2.GetMandatoryPropertyDefinition (property2.DeclaringType.FullName + "." + property2.Name);
      
      Assert.AreEqual (property1, propertyDefinition1.PropertyInfo);
      Assert.AreEqual (property2, propertyDefinition2.PropertyInfo);
    }

    [Test]
    public void CreatorIsFactoryBasedCreator ()
    {
      object creatorInstance = PrivateInvoke.GetPublicStaticField (
          typeof (ReflectionBasedClassDefinition).Assembly.GetType ("Remotion.Data.DomainObjects.Infrastructure.FactoryBasedDomainObjectCreator", true),
          "Instance");
      Assert.AreEqual (creatorInstance, PrivateInvoke.InvokeNonPublicMethod (_orderClass, "GetDomainObjectCreator"));
    }

    [Test]
    public void PersistentMixinFinder ()
    {
      var mixinFinder = new PersistentMixinFinderMock (typeof (Order));
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition("x", "xx", "xxx", typeof (Order), false, mixinFinder);
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
  }
}
