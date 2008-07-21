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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance
{
  [TestFixture]
  public class ClassDefinitionValidatorTest : TableInheritanceMappingTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInDifferentEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type> ());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type> ());
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type> ());

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (organizationalUnit, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'OtherName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithParallelDerivationInSameEntitiesAndMatchingColumnNames ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type> ());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type> ());
      ReflectionBasedClassDefinition organizationalUnit = new ReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type> ());

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (personClass, "Name", "NameColumn", typeof (string), 100));
      organizationalUnit.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (organizationalUnit, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type> ());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type> ());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (personClass, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Customer' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInSameEntityAndDuplicateColumnNameInBaseOfBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type> ());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type> ());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type> ());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      customerClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (customerClass, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
       "Property 'OtherName' of class 'Person' must not define storage specific name 'NameColumn', because class 'DomainBase' "
       + "in same inheritance hierarchy already defines property 'Name' with the same storage specific name.")]
    public void ValidateMappingWithDerivationInUndefinedEntityAndDuplicateColumnName ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type> ());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), true, domainBaseClass, new List<Type> ());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (personClass, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }
  }
}