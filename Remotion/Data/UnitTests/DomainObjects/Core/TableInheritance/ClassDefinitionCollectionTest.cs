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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
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
      ClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", null, TableInheritanceTestDomainProviderID, typeof (Person), false);
      _classDefinitions.Add (personClass);

      try
      {
        _classDefinitions.Validate ();
        Assert.Fail ("MappingException was expected.");
      }
      catch (MappingException ex)
      {
        string expectedMessage = string.Format (
            "Neither class 'Person' nor its base classes specify an entity name. Make "
            + "class '{0}' abstract or apply a DBTable attribute to it or one of its base classes.", 
            typeof (Person).AssemblyQualifiedName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void ValidateAbstractClassHandlesNullEntityNameWithInherited ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (customerClass);      

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateAbstractClassHandlesSameInheritedEntityName ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);

      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", personClass.MyEntityName, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

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
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);

      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "DifferentEntityNameThanBaseClass", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

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
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (customerClass);
      _classDefinitions.Validate ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Validate cannot be invoked, because class 'Customer' is a derived class of 'Person', but is not part of the collection itself.")]
    public void ValidateWithDerivedClassNotInCollection ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      _classDefinitions.Add (personClass);
      _classDefinitions.Validate ();
    }

    [Test]
    public void ValidateEntityNameWithAbstractBaseClass ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    public void ValidateWithoutBaseClass ()
    {
      ReflectionBasedClassDefinition addressClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Address", "TableInheritance_Address", TableInheritanceTestDomainProviderID, typeof (Address), false);

      _classDefinitions.Add (addressClass);

      _classDefinitions.Validate ();

      // Expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.OrganizationalUnit.Name' of class 'OrganizationalUnit' must not define storage specific name 'NameColumn', because class 'Person' "
        + "in same inheritance hierarchy already defines property 'Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Person.FirstName' with the same storage specific name.")]
    public void ValidateWithSameColumnNameInDifferentInheritanceBranches ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);

      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

      personClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (personClass, "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Person.FirstName", "NameColumn", typeof (string), false, 100));

      ReflectionBasedClassDefinition organizationalUnitClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      organizationalUnitClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.Create (organizationalUnitClass, "Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.OrganizationalUnit.Name", "NameColumn", typeof (string), false, 100));

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
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), true);

      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);

      ReflectionBasedClassDefinition organizationalUnitClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass);

      _classDefinitions.Add (domainBaseClass);
      _classDefinitions.Add (personClass);
      _classDefinitions.Add (organizationalUnitClass);

      _classDefinitions.Validate ();
    }
  }
}
