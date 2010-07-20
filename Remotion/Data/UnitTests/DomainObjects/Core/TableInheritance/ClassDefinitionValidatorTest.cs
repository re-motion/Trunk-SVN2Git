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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class ClassDefinitionValidatorTest : TableInheritanceMappingTest
  {
    public string DummyProperty { get; set; }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInDifferentEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (personClass, "Name", "NameColumn"));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (organizationalUnit, "OtherName", "NameColumn"));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();
      organizationalUnit.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithParallelDerivationInSameEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (personClass, "Name", "NameColumn"));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (organizationalUnit, "OtherName", "NameColumn"));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();
      organizationalUnit.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (domainBaseClass, "Name", "NameColumn"));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (personClass, "OtherName", "NameColumn"));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (domainBaseClass, "Name", "NameColumn"));
      customerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (customerClass, "OtherName", "NameColumn"));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();
      customerClass.SetReadOnly ();
      
      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInUndefinedEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), true, domainBaseClass);

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (domainBaseClass, "Name", "NameColumn"));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (personClass, "OtherName", "NameColumn"));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    public void ValidateMappingWithDerivationInDifferentEntitiesAndMatchingColumnNames_SameStorageProperty ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      var propertyInfo = GetType ().GetProperty ("DummyProperty");

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (personClass, typeof (Person), "FirstName", typeof (string), null, 100, StorageClass.Persistent, propertyInfo, new ColumnDefinition ("NameColumn", propertyInfo)));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (organizationalUnit, "OtherName", typeof (string), null, 100, StorageClass.Persistent, propertyInfo, new ColumnDefinition ("NameColumn", propertyInfo)));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();
      organizationalUnit.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    public void ValidateMappingWithParallelDerivationInSameEntitiesAndMatchingColumnNames_SameStorageProperty ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      var propertyInfo = GetType ().GetProperty ("DummyProperty");

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (personClass, typeof (Person), "FirstName", typeof (string), null, 100, StorageClass.Persistent, propertyInfo, new ColumnDefinition ("NameColumn", propertyInfo)));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (organizationalUnit, "OtherName", typeof (string), null, 100, StorageClass.Persistent, propertyInfo, new ColumnDefinition ("NameColumn", propertyInfo)));

      domainBaseClass.SetReadOnly ();
      personClass.SetReadOnly ();
      organizationalUnit.SetReadOnly ();

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

  }
}
