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
    
    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker(false);
    }

    [Test]
    public void GetMetadata_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition ();
      expected.SetDerivedClasses (new ClassDefinitionCollection ());

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);
      actual.SetDerivedClasses (new ClassDefinitionCollection ());

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();
      expected.SetDerivedClasses (new ClassDefinitionCollection ());

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (CreateClassWithMixedPropertiesClassDefinition());
      actual.SetDerivedClasses (new ClassDefinitionCollection ());

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void GetMetadata_InheritanceRoot_SimpleDomainObject ()
    {
      var classReflector = new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.AreSame (actual, actual.GetInheritanceRootClass ());
    }

    [Test]
    public void GetMetadata_ForMixedClass ()
    {
      var classReflector = new ClassReflector (typeof (TargetClassA), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD) }));
    }

    [Test]
    public void GetMetadata_ForDerivedMixedClass ()
    {
      var classReflector = new ClassReflector (typeof (TargetClassB), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void GetMetadata_ForClassWithOneSideRelationProperties ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithVirtualRelationEndPoints), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition ();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection ());
      expected.SetDerivedClasses (new ClassDefinitionCollection ());

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);
      actual.SetDerivedClasses (new ClassDefinitionCollection ());

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void GetMetadata_ForClassHavingClassIDAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassHavingClassIDAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
    }

    [Test]
    public void GetMetadata_ForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute), Configuration.NameResolver);

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute", actual.ID);
    }

    [Test]
    public void GetMetadata_ForClassWithHasStorageGroupAttributeDefinedItselfAndInBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithStorageGroupAttribute), Configuration.NameResolver);
      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("DerivedClassWithStorageGroupAttribute", actual.ID);
    }

    [Test]
    public void GetMetadata_ForClosedGenericClass ()
    {
      var classReflector = new ClassReflector (typeof (ClosedGenericClass), Configuration.NameResolver);

      Assert.IsNotNull (classReflector.GetMetadata (null));
    }

    [Test]
    public void GetMetadata_ForClassWithoutStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithStorageGroupAttribute), Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
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
    
  }
}
