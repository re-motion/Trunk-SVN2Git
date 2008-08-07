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
      ReflectionBasedClassDefinition expectedBaseClass = expected.BaseClass;
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
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithoutStorageGroupWithMixedProperties",
          "ClassWithoutStorageGroupWithMixedProperties",
          DefaultStorageProviderID,
          typeof (ClassWithoutStorageGroupWithMixedProperties),
          false, new PersistentMixinFinderMock());

      CreatePropertyDefinitionsForClassWithoutStorageGroupWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithoutStorageGroupWithMixedPropertiesClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "DerivedClassWithoutStorageGroupWithMixedProperties",
          "DerivedClassWithoutStorageGroupWithMixedProperties",
          DefaultStorageProviderID,
          typeof (DerivedClassWithoutStorageGroupWithMixedProperties),
          false,
          CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition (), new PersistentMixinFinderMock());

      CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (classDefinition);

      return classDefinition;
    }
  }
}
