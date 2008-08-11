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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", "TableInheritance_DomainBase", TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition organizationalUnit = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", null, TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), true, domainBaseClass);

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (domainBaseClass, "Name", "NameColumn", typeof (string), 100));
      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (personClass, "OtherName", "NameColumn", typeof (string), 100));

      new ClassDefinitionValidator (domainBaseClass).ValidateInheritanceHierarchy (new Dictionary<string, List<PropertyDefinition>> ());
    }
  }
}