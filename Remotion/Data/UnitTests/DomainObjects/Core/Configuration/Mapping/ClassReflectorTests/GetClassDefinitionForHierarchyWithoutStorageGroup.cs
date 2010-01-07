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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetClassDefinitionForHierarchyWithoutStorageGroup: TestBaseForHierarchyWithoutStorageGroup
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void GetClassDefinition_ForBaseClass()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithoutStorageGroupWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithoutStorageGroupWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithoutStorageGroupWithMixedPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithoutStorageGroupWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithoutStorageGroupWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expectedBaseClass = CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition ();
      _classDefinitions.Add (expectedBaseClass);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithoutStorageGroupWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithoutStorageGroupWithMixedPropertiesClassDefinition ();
      var expectedBaseClass = expected.ReflectionBasedBaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithoutStorageGroupWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    private ReflectionBasedClassDefinition CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithoutStorageGroupWithMixedProperties",
          "ClassWithoutStorageGroupWithMixedProperties",
          DefaultStorageProviderID,
          typeof (ClassWithoutStorageGroupWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithoutStorageGroupWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithoutStorageGroupWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DerivedClassWithoutStorageGroupWithMixedProperties",
          "DerivedClassWithoutStorageGroupWithMixedProperties",
          DefaultStorageProviderID,
          typeof (DerivedClassWithoutStorageGroupWithMixedProperties),
          false,
          CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition ());

      CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (classDefinition);

      return classDefinition;
    }
  }
}
