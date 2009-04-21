// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
    private ReflectionBasedClassDefinition _addressClass;
    private ReflectionBasedClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      _personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, _domainBaseClass);
      _customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", null, TableInheritanceTestDomainProviderID, typeof (Customer), false, _personClass);
      _addressClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Address", "TableInheritance_Address", TableInheritanceTestDomainProviderID, typeof (Address), false);

      _organizationalUnitClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, _domainBaseClass);
    }

    [Test]
    public void InitializeWithNullEntityName ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);

      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      Assert.IsNull (classDefinition.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException))]
    public void EntityNameMustNotBeEmptyWithClassType ()
    {
      ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", string.Empty, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
    }

    [Test]
    public void NullEntityNameWithDerivedClass ()
    {
      Assert.IsNull (_domainBaseClass.MyEntityName);
      Assert.IsNotNull (_personClass.MyEntityName);
      Assert.IsNull (_customerClass.MyEntityName);
    }

    [Test]
    public void GetEntityName ()
    {
      Assert.IsNull (_domainBaseClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _personClass.GetEntityName ());
      Assert.AreEqual ("TableInheritance_Person", _customerClass.GetEntityName ());
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingle ()
    {
      string[] entityNames = _customerClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConcrete ()
    {
      string[] entityNames = _personClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForConreteSingleWithEntityName ()
    {
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      string[] entityNames = customerClass.GetAllConcreteEntityNames ();
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

      string[] entityNames = _domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (2, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
      Assert.AreEqual ("TableInheritance_OrganizationalUnit", entityNames[1]);
    }

    [Test]
    public void GetAllConcreteEntityNamesForAbstractClassWithSameEntityNameInInheritanceHierarchy ()
    {
      ReflectionBasedClassDefinition domainBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false);
      ReflectionBasedClassDefinition personClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass);
      ReflectionBasedClassDefinition customerClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Customer), false, personClass);

      string[] entityNames = domainBaseClass.GetAllConcreteEntityNames ();
      Assert.IsNotNull (entityNames);
      Assert.AreEqual (1, entityNames.Length);
      Assert.AreEqual ("TableInheritance_Person", entityNames[0]);
    }
  }
}
