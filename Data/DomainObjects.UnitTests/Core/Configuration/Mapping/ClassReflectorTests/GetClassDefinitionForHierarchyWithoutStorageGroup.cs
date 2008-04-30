using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.ClassReflectorTests
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
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithoutStorageGroupWithMixedProperties));
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
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties));
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
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties));
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
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithoutStorageGroupWithMixedProperties));
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
          false, new List<Type> ());

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
          CreateClassWithoutStorageGroupWithMixedPropertiesClassDefinition (), new List<Type> ());

      CreatePropertyDefinitionsForDerivedClassWithoutStorageGroupWithMixedProperties (classDefinition);

      return classDefinition;
    }
  }
}