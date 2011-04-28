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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;
using Client = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client;
using Customer = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer;
using FileSystemItem = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem;
using Folder = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder;
using Order = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order;
using Person = Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _orderClass;
    private ClassDefinition _distributorClass;

    private ClassDefinition _targetClassForPersistentMixinClass;
    private ClassDefinition _derivedTargetClassForPersistentMixinClass;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private ClassDefinition _domainBaseClass;
    private ClassDefinition _personClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider");

      _domainBaseClass = ClassDefinitionFactory.CreateClassDefinition (
          "DomainBase", null, UnitTestDomainStorageProviderDefinition, typeof (DomainBase), false);
      _personClass = ClassDefinitionFactory.CreateClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false, _domainBaseClass);
      _customerClass = ClassDefinitionFactory.CreateClassDefinition (
          "Customer", null, UnitTestDomainStorageProviderDefinition, typeof (Customer), false, _personClass);
      _organizationalUnitClass = ClassDefinitionFactory.CreateClassDefinition (
          "OrganizationalUnit",
          "TableInheritance_OrganizationalUnit",
          UnitTestDomainStorageProviderDefinition,
          typeof (OrganizationalUnit),
          false,
          _domainBaseClass);

      _domainBaseClass.SetDerivedClasses (new[] { _personClass, _organizationalUnitClass });

      _orderClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
      _distributorClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Distributor)];

      _targetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.TypeDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.TypeDefinitions[typeof (DerivedTargetClassForPersistentMixin)];
    }

    [Test]
    public void Initialize ()
    {
      var actual = new ClassDefinition ("Order", typeof (Order), false, null, null, new PersistentMixinFinder (typeof (Order)));
      actual.SetDerivedClasses (new ClassDefinition[0]);

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.StorageEntityDefinition, Is.Null);
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      //Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
      Assert.That (actual.IsReadOnly, Is.False);
    }

    [Test]
    public void InitializeWithNullStorageGroupType ()
    {
      Assert.That (_domainBaseClass.StorageGroupType, Is.Null);

      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "DomainBase",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DomainBase),
          false,
          null,
          null,
          new PersistentMixinFinder (typeof (DomainBase)));
      Assert.That (classDefinition.StorageGroupType, Is.Null);
    }

    [Test]
    public void InitializeWithStorageGroupType ()
    {
      Assert.That (_domainBaseClass.StorageGroupType, Is.Null);

      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "DomainBase",
          null,
          UnitTestDomainStorageProviderDefinition,
          typeof (DomainBase),
          false,
          null,
          typeof (DBStorageGroupAttribute),
          new PersistentMixinFinder (typeof (DomainBase)));
      Assert.That (classDefinition.StorageGroupType, Is.Not.Null);
      Assert.That (classDefinition.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void NullEntityNameWithDerivedClass ()
    {
      Assert.IsNull (StorageModelTestHelper.GetEntityName (_domainBaseClass));
      Assert.IsNotNull (StorageModelTestHelper.GetEntityName (_personClass));
      Assert.IsNull (StorageModelTestHelper.GetEntityName (_customerClass));
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName());
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingle ()
    {
      string[] entityNames = _customerClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void SetStorageEntityDefinition ()
    {
      var tableDefinition = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Tablename"),
          new EntityNameDefinition (null, "Viewname"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);

      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetStorageEntityDefinition_ClassIsReadOnly ()
    {
      var tableDefinition = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Tablename"),
          new EntityNameDefinition (null, "Viewname"),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (_domainBaseClass, "Test", "Test");

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));

      Assert.That (_domainBaseClass.MyPropertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyPropertyDefinitions[0], Is.SameAs (propertyDefinition));
      Assert.That (_domainBaseClass.MyPropertyDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The property-definitions for class 'DomainBase' have already been set."
        )]
    public void SetPropertyDefinitions_Twice_ThrowsException ()
    {
      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (_domainBaseClass, "Test", "Test");

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetPropertyDefinitions_ClassIsReadOnly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
    }

    [Test]
    public void SetRelationEndPointDefinitions ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));

      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions[0], Is.SameAs (endPointDefinition));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation end point for property 'Test' cannot be added to class 'DomainBase', because it was initialized for class 'Distributor'.")]
    public void SetRelationEndPointDefinitions_DifferentClassDefinition_ThrowsException ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _distributorClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation end point for property 'Test' cannot be added to class 'Person', because base class 'DomainBase' already defines a relation end point "
        + "with the same property name.")]
    public void SetRelationEndPointDefinitions_EndPointWithSamePropertyNameWasAlreadyAdded_ThrowsException ()
    {
      var baseEndPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      var derivedEndPointDefinition = new VirtualRelationEndPointDefinition (
          _personClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { baseEndPointDefinition }, true));
      _personClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { derivedEndPointDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation end point definitions for class 'DomainBase' have already been set.")]
    public void SetRelationEndPointDefinitions_Twice_ThrowsException ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetRelationEndPointDefinitions_ClassIsReadonly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[0], true));
    }

    [Test]
    public void SetDerivedClasses ()
    {
      _personClass.SetDerivedClasses (new[] { _customerClass });

      Assert.That (_personClass.DerivedClasses.Count, Is.EqualTo (1));
      Assert.That (_personClass.DerivedClasses[0], Is.SameAs (_customerClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The derived-classes for class 'Person' have already been set.")]
    public void SetDerivedClasses_Twice_ThrowsException ()
    {
      _personClass.SetDerivedClasses (new[] { _customerClass });
      _personClass.SetDerivedClasses (new[] { _customerClass });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'Person' is read-only.")]
    public void SetDerivedClasses_ClasssIsReadOnly ()
    {
      _personClass.SetReadOnly();
      _personClass.SetDerivedClasses (new[] { _orderClass });
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Derived class 'Order' cannot be added to class 'Person', because it has no base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasNoBaseClassDefined ()
    {
      _personClass.SetDerivedClasses (new[] { _orderClass });
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Derived class 'Person' cannot be added to class 'Customer', because it has class 'DomainBase' as its base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasWrongBaseClassDefined ()
    {
      _customerClass.SetDerivedClasses (new[] { _personClass });
    }

    [Test]
    public void GetAllConcreteEntityNamesForConcrete ()
    {
      string[] entityNames = _personClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingleWithEntityName ()
    {
      ClassDefinition personClass = ClassDefinitionFactory.CreateClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false);
      ClassDefinition customerClass = ClassDefinitionFactory.CreateClassDefinition (
          "Customer", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, personClass);

      string[] entityNames = customerClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClass ()
    {
      // ensure both classes derived from DomainBase are loaded
      Dev.Null = _personClass;
      Dev.Null = _organizationalUnitClass;

      string[] entityNames = _domainBaseClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (2, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
      Assert.AreEqual ("TableInheritance_OrganizationalUnit", entityNames[1]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClassWithSameEntityNameInInheritanceHierarchy ()
    {
      ClassDefinition domainBaseClass = ClassDefinitionFactory.CreateClassDefinition (
          "DomainBase", null, UnitTestDomainStorageProviderDefinition, typeof (DomainBase), false);
      ClassDefinition personClass = ClassDefinitionFactory.CreateClassDefinition (
          "Person", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Person), false, domainBaseClass);
      ClassDefinitionFactory.CreateClassDefinition (
          "Customer", "TableInheritance_Person", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, personClass);

      domainBaseClass.SetDerivedClasses (new[] { personClass });

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void SetReadOnly ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      actual.SetDerivedClasses (new ClassDefinition[0]);
      Assert.That (actual.IsReadOnly, Is.False);

      actual.SetReadOnly();

      Assert.That (actual.IsReadOnly, Is.True);
      Assert.That (((ICollection<ClassDefinition>)actual.DerivedClasses).IsReadOnly, Is.True);
    }

    [Test]
    public void GetToString ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateClassDefinition (
          "OrderID", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_FromNonAbstractType ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromAbstractType ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (AbstractClass), true);

      Assert.IsTrue (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentFalse ()
    {
      ClassDefinition actual =
          ClassDefinitionFactory.CreateClassDefinition (
              "ClassID", "Table", UnitTestDomainStorageProviderDefinition, typeof (AbstractClass), false);

      Assert.IsFalse (actual.IsAbstract);
    }

    [Test]
    public void GetIsAbstract_FromArgumentTrue ()
    {
      ClassDefinition actual = ClassDefinitionFactory.CreateClassDefinition (
          "ClassID", "Table", UnitTestDomainStorageProviderDefinition, typeof (Order), true);

      Assert.IsTrue (actual.IsAbstract);
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
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"];
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
              +
              "TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
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
              +
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetPropertyDefinition_NoPropertyDefinitionsHaveBeenSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      classDefinition.GetPropertyDefinition ("dummy");
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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetAllPropertyDefinitions_ThrowsWhenPropertiesNotSet ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order",
          "OrderTable",
          new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider"),
          typeof (Order),
          false);
      classDefinition.GetPropertyDefinitions();
    }

    [Test]
    public void GetAllPropertyDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (classDefinition, "Test", "Test");
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetPropertyDefinitions();
      var result2 = classDefinition.GetPropertyDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllPropertyDefinitions_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var orderClass = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      var propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      orderClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var companyPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var customerClass = ClassDefinitionFactory.CreateClassDefinition (
          "Customer", "Company", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, companyClass);
      var customerPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (customerClass, "Name", "Name");
      customerClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { customerPropertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);
      var companyPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (companyClass, "Name", "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var partnerClass = ClassDefinitionFactory.CreateClassDefinition (
          "Partner", "Company", UnitTestDomainStorageProviderDefinition, typeof (Partner), false, companyClass);
      partnerClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      var supplierClass = ClassDefinitionFactory.CreateClassDefinition (
          "Supplier", "Company", UnitTestDomainStorageProviderDefinition, typeof (Supplier), false, partnerClass);
      var supplierPropertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (supplierClass, "Name", "Name");
      supplierClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { supplierPropertyDefinition }, true));
    }

    [Test]
    public void ConstructorWithoutBaseClass ()
    {
      ClassDefinitionFactory.CreateClassDefinition (
          "Company", "Company", UnitTestDomainStorageProviderDefinition, typeof (Company), false);

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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "No relation end point definitions have been set for class 'Order'.")]
    public void GetAllRelationEndPointDefinitions_ThrowsWhenRelationsNotSet ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.GetRelationEndPointDefinitions();
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetRelationEndPointDefinitions();
      var result2 = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllRelationEndPointDefinitionss_ReadOnly ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "OrderTable", UnitTestDomainStorageProviderDefinition, typeof (Order), false);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, typeof (DomainObject), null, typeof (Order).GetProperty ("OrderNumber"));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
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
      IRelationEndPointDefinition[] relationEndPointDefinitions = _distributorClass.MyRelationEndPointDefinitions.ToArray();

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
      Assert.IsTrue (((ICollection<ClassDefinition>) _orderClass.DerivedClasses).IsReadOnly);
    }

    [Test]
    public void GetDerivedClassesWithInheritance ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Company)];

      Assert.IsNotNull (companyDefinition.DerivedClasses);
      Assert.AreEqual (2, companyDefinition.DerivedClasses.Count);
      Assert.IsTrue (companyDefinition.DerivedClasses.Any (cd => cd.ID == "Customer"));
      Assert.IsTrue (companyDefinition.DerivedClasses.Any (cd => cd.ID == "Partner"));
      Assert.IsTrue (((ICollection<ClassDefinition>) companyDefinition.DerivedClasses).IsReadOnly);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Company)];

      Assert.IsTrue (companyDefinition.IsPartOfInheritanceHierarchy);
      Assert.IsTrue (_distributorClass.IsPartOfInheritanceHierarchy);
      Assert.IsFalse (_orderClass.IsPartOfInheritanceHierarchy);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition ()
    {
      ClassDefinition clientDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Client)];

      RelationDefinition parentClient =
          MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Client.ParentClient"];
      var clientAnonymousEndPointDefinition = (AnonymousRelationEndPointDefinition) parentClient.GetEndPointDefinition ("Client", null);

      Assert.IsFalse (clientDefinition.IsRelationEndPoint (clientAnonymousEndPointDefinition));
    }

    [Test]
    public void MyPropertyDefinitions ()
    {
      var clientDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Client)];

      var propertyDefinitions = clientDefinition.MyPropertyDefinitions.ToArray();

      Assert.That (propertyDefinitions.Length, Is.EqualTo (1));
      Assert.That (
          propertyDefinitions[0].PropertyName,
          Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void MyPropertyDefinitions_NoPropertiesSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));

      classDefinition.MyPropertyDefinitions.ToArray();
    }

    [Test]
    public void MyRelationEndPointDefinitions ()
    {
      var clientDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Client)];

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No relation end point definitions have been set for class 'Order'.")]
    public void MyRelationEndPointDefinitions_NoRelationEndPointDefinitionsSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));

      classDefinition.MyRelationEndPointDefinitions.ToArray();
    }

    [Test]
    public void MyRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    public void IsMyRelationEndPoint ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

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
    public void MyRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.AreEqual (1, endPointDefinitions.Length);
      Assert.IsTrue (
          Contains (
              endPointDefinitions, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

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
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

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
    public void GetRelationEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

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
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
      Assert.IsNotNull (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems"));
    }

    [Test]
    public void GetOppositeClassDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

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
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

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
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      Assert.IsNotNull (
          fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'Invalid'.")]
    public void GetMandatoryOppositeEndPointDefinition_InvalidProperty ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("Invalid");
    }

    [Test]
    public void GetOppositeEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

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
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      Assert.IsNull (fileSystemItemDefinition.GetOppositeEndPointDefinition ("Invalid"));
    }

    [Test]
    public void GetMandatoryOppositeEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

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
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      Assert.IsNotNull (fileSystemItemDefinition.GetMandatoryOppositeEndPointDefinition ("InvalidProperty"));
    }

    [Test]
    public void GetMandatoryOppositeClassDefinition ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];
      ClassDefinition folderDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (Folder)];

      Assert.AreSame (
          folderDefinition,
          fileSystemItemDefinition.GetMandatoryOppositeClassDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'FileSystemItem' and property 'InvalidProperty'.")]
    public void GetMandatoryOppositeClassDefinitionWithInvalidPropertyName ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.TypeDefinitions[typeof (FileSystemItem)];

      fileSystemItemDefinition.GetMandatoryOppositeClassDefinition ("InvalidProperty");
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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "Order", "Order", UnitTestDomainStorageProviderDefinition, typeof (Order), false);

      PropertyDefinition propertyDefinition = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "Test", "Test");
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      Assert.AreSame (classDefinition, propertyDefinition.ClassDefinition);
    }

    [Test]
    public void Contains ()
    {
      Assert.IsFalse (
          _orderClass.Contains (
              PropertyDefinitionFactory.CreateForFakePropertyInfo (
                  _orderClass, "PropertyName", "ColumnName")));
      Assert.IsTrue (
          _orderClass.Contains (
              _orderClass["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"]));
    }

    [Test]
    public void GetInheritanceRootClass ()
    {
      ClassDefinition expected = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];
      Assert.AreSame (expected, _distributorClass.GetInheritanceRootClass());
    }

    [Test]
    public void GetAllDerivedClasses ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];
      var allDerivedClasses = companyClass.GetAllDerivedClasses();
      Assert.IsNotNull (allDerivedClasses);
      Assert.AreEqual (4, allDerivedClasses.Length);

      Assert.IsTrue (allDerivedClasses.Any (cd => cd.ClassType == typeof (Customer)));
      Assert.IsTrue (allDerivedClasses.Any (cd => cd.ClassType == typeof (Partner)));
      Assert.IsTrue (allDerivedClasses.Any (cd => cd.ClassType == typeof (Supplier)));
      Assert.IsTrue (allDerivedClasses.Any (cd => cd.ClassType == typeof (Distributor)));
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
      ClassDefinition companyClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];

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
      var propertyDefinition = _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.AreEqual (property, propertyDefinition.PropertyInfo);
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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixinFinder);
      Assert.That (classDefinition.PersistentMixinFinder, Is.SameAs (mixinFinder));
    }

    [Test]
    public void PersistentMixins_Empty ()
    {
      var mixins = new Type[0];
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }

    [Test]
    public void PersistentMixins_NonEmpty ()
    {
      var mixins = new[] { typeof (MixinA), typeof (MixinB) };
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }


    [Test]
    public void ResolveProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_Twice_ReturnsSamePropertyDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result1 = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));
      var result2 = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveProperty_StorageClassNoneProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var result = _orderClass.ResolveProperty (new PropertyInfoAdapter (property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveProperty_MixinProperty ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _targetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter (property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_MixinPropertyOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveProperty (new PropertyInfoAdapter (property));

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

    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoChanges ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinA)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration(); // ok, no changes
      }
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkOnInheritanceRootInheritingMixin ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x",
          "xx",
          UnitTestDomainStorageProviderDefinition,
          typeof (InheritanceRootInheritingPersistentMixin),
          false,
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot));

      using (MixinConfiguration
          .BuildNew()
          .ForClass (typeof (TargetClassAboveInheritanceRoot))
          .AddMixins (typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot))
          .EnterScope())
      {
        // ok, no changes, even though the mixins stem from a base class
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.\r\n"
        + "Original configuration: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.MixinA.\r\n"
        + "Active configuration: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.NonDomainObjectMixin + "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain.MixinA")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenAnyChanges_EvenToNonPersistentMixins ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));

      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA)).EnterScope
              ())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinMissing ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass<Order>().Clear().EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Order), false, typeof (MixinA));
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsChangeOnParentClass ()
    {
      ClassDefinition baseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "xbase", "xx", UnitTestDomainStorageProviderDefinition, typeof (Company), false, typeof (MixinA));
      ClassDefinition classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "x", "xx", UnitTestDomainStorageProviderDefinition, typeof (Customer), false, baseClassDefinition);
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Company)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }
  }
}