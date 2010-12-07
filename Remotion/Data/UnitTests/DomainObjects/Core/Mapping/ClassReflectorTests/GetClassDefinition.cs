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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ClassReflectorTests
{
  [TestFixture]
  public class GetClassDefinition : TestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker(false);
      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      var classReflector= new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition();
      expected.SetDerivedClasses (new ClassDefinitionCollection());
      expected.BaseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { expected }, true, true));

      ReflectionBasedClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);
      actual.SetDerivedClasses (new ClassDefinitionCollection());
      actual.BaseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { actual }, true, true));

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _classDefinitionChecker.Check (expected.BaseClass, actual.BaseClass);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (actual.BaseClass, _classDefinitions.GetMandatory (typeof (ClassWithMixedProperties)));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithBaseClassAlreadyInClassDefinitionCollection ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ClassDefinition expectedBaseClass = CreateClassWithMixedPropertiesClassDefinition ();
      _classDefinitions.Add (expectedBaseClass);

      ClassDefinition actual = classReflector.GetClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual (2, _classDefinitions.Count);
      Assert.AreSame (actual, _classDefinitions.GetMandatory (typeof (DerivedClassWithMixedProperties)));
      Assert.AreSame (expectedBaseClass, actual.BaseClass);
    }

    [Test]
    public void GetClassDefinition_ForDerivedClassWithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();

      var expectedBaseClass = expected.BaseClass;
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
    public void GetClassDefinition_ForClassesWithSameClassID ()
    {
      Type type1 = GetTypeFromDomainWithErrors ("ClassWithSameClassID");
      Type type2 = GetTypeFromDomainWithErrors ("OtherClassWithSameClassID");

      var classReflector1 = new ClassReflector (type1, Configuration.NameResolver);
      var classReflector2 = new ClassReflector (type2, Configuration.NameResolver);

      classReflector1.GetClassDefinition (_classDefinitions);
      try
      {
        classReflector2.GetClassDefinition (_classDefinitions);

        Assert.Fail ("exception expected");
      }
      catch (MappingException ex)
      {
        var expectedMessage = string.Format (
            "Class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithSameClassID' "
            + "and 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.OtherClassWithSameClassID' "
            + "both have the same class ID 'DefinedID'. Use the ClassIDAttribute to define unique IDs for these classes. "
            + "The assemblies involved are '{0}' and '{1}'.",
            GetType ().Assembly.GetName ().FullName,
            GetType ().Assembly.GetName ().FullName);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void CreateClassDefinition_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition ();
      expected.SetDerivedClasses (new ClassDefinitionCollection ());

      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition (_classDefinitions);
      actual.SetDerivedClasses (new ClassDefinitionCollection ());

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void CreateClassDefinitions_InheritanceRoot_SimpleDomainObject ()
    {
      var classReflector= new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition(_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreSame (actual, actual.GetInheritanceRootClass());
    }

    [Test]
    public void CreateClassDefinition_ForMixedClass ()
    {
      var classReflector= new ClassReflector (typeof (TargetClassA), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition (_classDefinitions);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD)}));
    }

    [Test]
    public void CreateClassDefinition_ForDerivedMixedClass ()
    {
      var classReflector= new ClassReflector (typeof (TargetClassB), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition (_classDefinitions);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void CreateClassDefinition_ForClassWithOneSideRelationProperties ()
    {
      var classReflector= new ClassReflector (typeof (ClassWithVirtualRelationEndPoints), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection());
      expected.SetDerivedClasses (new ClassDefinitionCollection());

      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition(_classDefinitions);
      actual.SetDerivedClasses (new ClassDefinitionCollection());

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void CreateClassDefinition_ForClassHavingClassIDAttribute ()
    {
      var classReflector= new ClassReflector (typeof (ClassHavingClassIDAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
    }
    
   [Test]
    public void CreateClassDefinition_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
      var classReflector= new ClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.CreateClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
    }

    [Test]
    public void CreateClassDefinition_ForClassWithHasStorageGroupAttributeDefinedItselfAndInBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithStorageGroupAttribute), Configuration.NameResolver);
      var actual = classReflector.CreateClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("DerivedClassWithStorageGroupAttribute", actual.ID);
    }
    
    [Test]
    public void CreateClassDefinition_ForClosedGenericClass ()
    {
      var classReflector= new ClassReflector (typeof (ClosedGenericClass), Configuration.NameResolver);

      Assert.IsNotNull (classReflector.CreateClassDefinition (_classDefinitions));
    }

    [Test]
    public void CreateClassDefinition_ForClassWithoutStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);

      var actual = classReflector.CreateClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.Null);
    }

    [Test]
    public void CreateClassDefinition_ForClassWithStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithStorageGroupAttribute), Configuration.NameResolver);

      var actual = classReflector.CreateClassDefinition (_classDefinitions);

      Assert.IsNotNull (actual);
      Assert.That(actual.StorageGroupType, Is.SameAs(typeof(DBStorageGroupAttribute)));
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
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithMixedProperties),
          false);
      
      CreatePropertyDefinitionsForClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateDerivedClassWithMixedPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("DerivedClassWithMixedProperties",
          "DerivedClassWithMixedProperties",
          UnitTestDomainStorageProviderDefinition,
          typeof (DerivedClassWithMixedProperties),
          false,
          CreateClassWithMixedPropertiesClassDefinition());
     CreatePropertyDefinitionsForDerivedClassWithMixedProperties (classDefinition);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOneSideRelationPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("ClassWithVirtualRelationEndPoints",
          "ClassWithVirtualRelationEndPoints",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithVirtualRelationEndPoints),
          false);

      return classDefinition;
    }

    private static Type GetTypeFromDomainWithErrors (string typename)
    {
      var type = Assembly.GetExecutingAssembly().GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors." + typename,
          true,
          false);

      Assert.That (type, Is.Not.Null, "Type '{0}' could not be found in domain with errors.", typename);

      return type;
    }
  }
}
