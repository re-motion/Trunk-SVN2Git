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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetClassDefinition : TestBase
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
    public void GetClassDefinition_ForBaseClass ()
    {
      var classReflector= new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      var classReflector= new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void InheritanceRoot_SimpleDomainObject ()
    {
      var classReflector= new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreSame (actual, actual.GetInheritanceRootClass());
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      var classReflector= new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
      ReflectionBasedClassDefinition expectedBaseClass = expected.BaseClass;
      _classDefinitions.Add (expectedBaseClass);
      _classDefinitions.Add (expected);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expected, actual);
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForMixedClass ()
    {
      var classReflector= new ClassReflector (typeof (TargetClassA), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD)}));
    }

    [Test]
    public void GetClassDefinition_ForDerivedMixedClass ()
    {
      var classReflector= new ClassReflector (typeof (TargetClassB), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void GetClassDefinition_ForClassWithOneSideRelationProperties ()
    {
      var classReflector= new ClassReflector (typeof (ClassWithOneSideRelationProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      Assert.AreEqual (1, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (ClassWithOneSideRelationProperties)));
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttribute ()
    {
      var classReflector= new ClassReflector (typeof (ClassHavingClassIDAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForClassWithStorageSpecificIdentifierAttribute ()
    {
      var classReflector= new ClassReflector (typeof (ClassHavingStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithStorageSpecificIdentifierAttribute ()
    {
      var classReflector= new ClassReflector (typeof (DerivedClassWithStorageSpecificIdentifierAttribute), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition();

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
    }

    [Test]
    public void GetClassDefinition_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
      var classReflector= new ClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
      Assert.AreEqual ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable", actual.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type cannot redefine the 'Remotion.Data.DomainObjects.StorageGroupAttribute' already defined on base type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.BaseClassWithStorageGroupAttribute'.\r\n"
        + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.Derived2ClassWithStorageGroupAttribute")]
    public void GetClassDefinition_ForDerivedClassWithRedefinedStorageGroupAttribute ()
    {
      Type type = GetTypeFromDomainWithErrors ("Derived2ClassWithStorageGroupAttribute");

      var classReflector= new ClassReflector (type, Configuration.NameResolver);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer "
        + "argument). The reflection-based mapping does not use this constructor any longer and requires it to be removed.\r\n"
        + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithLegacyLoadConstructor")]
    public void GetClassDefinition_ForClassWithLegacyLoadConstructor ()
    {
      Type type = GetTypeFromDomainWithErrors ("ClassWithLegacyLoadConstructor");

      var classReflector= new ClassReflector (type, Configuration.NameResolver);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    [Test]
    public void GetClassDefinition_ForClosedGenericClass ()
    {
      var classReflector= new ClassReflector (typeof (ClosedGenericClass), Configuration.NameResolver);

      Assert.IsNotNull (classReflector.GetClassDefinition (_classDefinitions));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Generic domain objects are not supported.\r\n"
        + "Type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.GenericClass`1")]
    public void GetClassDefinition_ForOpenGenericClass ()
    {
      Type type = GetTypeFromDomainWithErrors ("GenericClass`1");
      Type closedGenericType = type.MakeGenericType (typeof (int));

      var classReflector= new ClassReflector (closedGenericType, Configuration.NameResolver);

      classReflector.GetClassDefinition (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping."
        + "TestDomain.Errors.ClassWithSameClassID' and 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors."
        + "OtherClassWithSameClassID' both have the same class ID 'DefinedID'. Use the ClassIDAttribute to define unique IDs for these "
        + "classes. The assemblies involved are 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors, "
        + "Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' and 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain."
        + "Errors, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.")]
    public void GetClassDefinition_ForClassesWithSameClassID ()
    {
      Type type1 = GetTypeFromDomainWithErrors ("ClassWithSameClassID");
      Type type2 = GetTypeFromDomainWithErrors ("OtherClassWithSameClassID");

      var classReflector1 = new ClassReflector (type1, Configuration.NameResolver);
      var classReflector2 = new ClassReflector (type2, Configuration.NameResolver);

      classReflector1.GetClassDefinition (_classDefinitions);
      classReflector2.GetClassDefinition (_classDefinitions);
    }

    [Test]
    public void PersistentMixinFinder_ForBaseClass ()
    {
      var classReflector= new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      Assert.That (classReflector.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void PersistentMixinFinder_ForDerivedClass ()
    {
      var classReflector= new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      Assert.That (classReflector.PersistentMixinFinder.IncludeInherited, Is.False);
    }

    [Test]
    public void PersistentMixinFinder_SimpleDomainObject ()
    {
      var classReflector = new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);
      Assert.That (classReflector.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void PersistentMixinFinder_InheritanceRoot ()
    {
      var classReflector = new ClassReflector (typeof (InheritanceRootInheritingPersistentMixin), Configuration.NameResolver);
      Assert.That (classReflector.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    private ReflectionBasedClassDefinition CreateClassWithMixedPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithMixedProperties",
          "ClassWithMixedProperties",
          c_testDomainProviderID,
          typeof (ClassWithMixedProperties),
          false);

      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DerivedClassWithMixedProperties",
          "DerivedClassWithMixedProperties",
          c_testDomainProviderID,
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition());

      CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithOneSideRelationProperties",
          "ClassWithOneSideRelationProperties",
          c_testDomainProviderID,
          typeof (ClassWithOneSideRelationProperties),
          false);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("BaseClassWithoutStorageSpecificIdentifierAttribute",
          "BaseClassWithoutStorageSpecificIdentifierAttribute",
          c_testDomainProviderID,
          typeof (BaseClassWithoutStorageSpecificIdentifierAttribute),
          true);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithStorageSpecificIdentifierAttributeClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DerivedClassWithStorageSpecificIdentifierAttribute",
          "DerivedClassWithStorageSpecificIdentifierAttribute",
          c_testDomainProviderID,
          typeof (DerivedClassWithStorageSpecificIdentifierAttribute),
          false,
          CreateBaseClassWithoutStorageSpecificIdentifierAttributeDefinition ());

      return classDefinition;
    }

    private static Type GetTypeFromDomainWithErrors (string typename)
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors." + typename,
          true,
          false);

      Assert.That (type, Is.Not.Null, "Type '{0}' could not be found in domain with errors.", typename);

      return type;
    }
  }
}
