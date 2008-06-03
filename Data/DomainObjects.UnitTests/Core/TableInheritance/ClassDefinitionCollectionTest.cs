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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class ClassDefinitionCollectionTest : TableInheritanceMappingTest
  {
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinitions = new ClassDefinitionCollection ();
    }

    [Test]
    public void ValidateAbstractClass ()
    {
      ClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());
      _classDefinitions.Add (personClass);

      try
      {
        _classDefinitions.Validate ();
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Type '{0}' must be abstract, because neither class 'Person' nor its base classes specify an entity name.", 
            typeof (Person).AssemblyQualifiedName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);      

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          "Customer", personClass.MyEntityName, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "Class 'Customer' must not specify an entity name 'DifferentEntityNameThanBaseClass'"
        + " which is different from inherited entity name 'TableInheritance_Person'.")]
    public void ValidateWithDifferentEntityNames ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());

      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition (
          "Customer", "DifferentEntityNameThanBaseClass", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Person' is a base class of a class in the collection,"
        + " but the base class is not part of the collection itself.")]
    public void ValidateWithBaseClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());
      ReflectionBasedClassDefinition customerClass = new ReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, new List<Type>());
      new ReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass, new List<Type>());

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());
      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      ReflectionBasedClassDefinition addressClass = new ReflectionBasedClassDefinition ("Address", "TableInheritance_Address", TableInheritanceTestDomainProviderID, typeof (Address), false, new List<Type>());

      _classDefinitions.Add (addressClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Property 'OrganizationalUnitName' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'PersonName' with the same storage specific name.")]
    public void ValidateWithSameColumnNameInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(personClass, "PersonName", "NameColumn", typeof (string), false, 100));

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type>());

      organizationalUnitClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(organizationalUnitClass, "OrganizationalUnitName", "NameColumn", typeof (string), false, 100));

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), 
        ExpectedMessage = "At least two classes in different inheritance branches derived from abstract class 'DomainBase'"
        + " specify the same entity name 'TableInheritance_Person', which is not allowed.")]
    public void ValidateWithSameEntityNamesInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true, new List<Type>());

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type>());

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type>());

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }
  }
}
