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
  public class GetClassDefinitionForRdbmsClassReflector: TestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttribute()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassHavingClassIDAttribute), Configuration.NameResolver);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassWithStorageSpecificIdentifierAttribute()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassHavingStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithStorageSpecificIdentifierAttribute ()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithStorageSpecificIdentifierAttribute), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForBaseClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      ClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
      ClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForClassWithOneSideRelationProperties()
    {
      ClassReflector classReflector = new RdbmsClassReflector (typeof (ClassWithOneSideRelationProperties), Configuration.NameResolver);
      ClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithOneSideRelationProperties)));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The 'Remotion.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n  "
        + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void GetClassDefinition_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute()
    {
      Type derivedClass = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      ClassReflector classReflector = new RdbmsClassReflector (derivedClass, Configuration.NameResolver);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    private ReflectionBasedClassDefinition CreateClassWithMixedPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithMixedProperties",
          "ClassWithMixedProperties",
          c_testDomainProviderID,
          typeof (ClassWithMixedProperties),
          false, new PersistentMixinFinderMock());

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "DerivedClassWithMixedProperties",
          null,
          c_testDomainProviderID,
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition(), new PersistentMixinFinderMock());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClassWithOneSideRelationProperties),
          false, new PersistentMixinFinderMock());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "BaseClassWithoutStorageSpecificIdentifierAttribute",
          null,
          c_testDomainProviderID,
          typeof (BaseClassWithoutStorageSpecificIdentifierAttribute),
          true, new PersistentMixinFinderMock());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "DerivedClassWithStorageSpecificIdentifierAttribute",
          "DerivedClassWithStorageSpecificIdentifierAttribute",
          c_testDomainProviderID,
          typeof (DerivedClassWithStorageSpecificIdentifierAttribute),
          false,
          CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition (), new PersistentMixinFinderMock());

      return classDefinition;
    }
  }
}
