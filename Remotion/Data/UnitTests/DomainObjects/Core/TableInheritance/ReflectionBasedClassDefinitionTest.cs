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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class ReflectionBasedClassDefinitionTest : TableInheritanceMappingTest
  {
    private ReflectionBasedClassDefinition _domainBaseClass;
    private ReflectionBasedClassDefinition _personClass;
    private ReflectionBasedClassDefinition _customerClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp();

      _domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      _personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, _domainBaseClass);
      _customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, _personClass);

      _organizationalUnitClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrganizationalUnit",
          "TableInheritance_OrganizationalUnit",
          TableInheritanceTestDomainProviderID,
          typeof (OrganizationalUnit),
          false,
          _domainBaseClass);
    }

    [Test]
    public void InitializeWithNullStorageGroupType ()
    {
      Assert.That(_domainBaseClass.StorageGroupType, Is.Null);

      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase",
          null,
          TableInheritanceTestDomainProviderID,
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

      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase",
          null,
          TableInheritanceTestDomainProviderID,
          typeof (DomainBase),
          false,
          null,
          typeof(DBStorageGroupAttribute),
          new PersistentMixinFinder (typeof (DomainBase)));
      Assert.That (classDefinition.StorageGroupType, Is.Not.Null);
      Assert.That (classDefinition.StorageGroupType, Is.SameAs(typeof(DBStorageGroupAttribute)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase", string.Empty, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
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
      var tableDefinition = new TableDefinition ("DefaultStorageProvider", "Tablename", "Viewname", new ColumnDefinition[0]);
      
      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    public void SetStorageProviderDefinition ()
    {
      var providerDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));

      _domainBaseClass.SetStorageProviderDefinition (providerDefinition);

      Assert.That (_domainBaseClass.StorageProviderDefinition, Is.SameAs (providerDefinition));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_domainBaseClass, "Test", "Test");

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      Assert.That (_domainBaseClass.MyPropertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyPropertyDefinitions[0], Is.SameAs (propertyDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetPropertyDefinitions_ClassIsReadOnly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
    }

    [Test]
    public void SetRelationDefinitions ()
    {
      var relationDefinition = new RelationDefinition (
          "Test", new AnonymousRelationEndPointDefinition (_domainBaseClass), new AnonymousRelationEndPointDefinition (_domainBaseClass));

      _domainBaseClass.SetRelationDefinitions (new RelationDefinitionCollection (new[] { relationDefinition }, true));

      Assert.That (_domainBaseClass.MyRelationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyRelationDefinitions[0], Is.SameAs (relationDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetRelationDefinitions_ClassIsReadOnly ()
    {
      _domainBaseClass.SetReadOnly ();

      _domainBaseClass.SetRelationDefinitions (new RelationDefinitionCollection (new RelationDefinition[0], true));
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
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }
  }
}