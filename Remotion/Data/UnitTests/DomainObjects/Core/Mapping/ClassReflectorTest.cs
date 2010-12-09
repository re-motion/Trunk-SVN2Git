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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassReflectorTest : MappingReflectionTestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    
    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
    }

    [Test]
    public void GetMetadata_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithMixedPropertiesClassDefinition ();

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateDerivedClassWithMixedPropertiesClassDefinition ();

      var baseClassDefinition = CreateClassWithMixedPropertiesClassDefinition();
      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
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
      var classReflectorForBaseClass = new ClassReflector (typeof (TargetClassA), Configuration.NameResolver);
      ReflectionBasedClassDefinition baseClass = classReflectorForBaseClass.GetMetadata (null);

      var classReflector = new ClassReflector (typeof (TargetClassB), Configuration.NameResolver);
      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (baseClass);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void GetMetadata_ForClassWithOneSideRelationProperties ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithVirtualRelationEndPoints), Configuration.NameResolver);
      ReflectionBasedClassDefinition expected = CreateClassWithOneSideRelationPropertiesClassDefinition ();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection ());

      ReflectionBasedClassDefinition actual = classReflector.GetMetadata (null);

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
    public void GetMetadata_PersistentMixinFinder_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithMixedProperties), Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithMixedProperties), Configuration.NameResolver);
      var baseClassDefinition = ClassDefinitionFactory.CreateFinishedOrderDefinition ();

      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.False);
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

    private void CreatePropertyDefinitionsForClassWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition> ();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedPropertiesNotInMapping), "BaseString", "BaseString", typeof (string), null, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedPropertiesNotInMapping), "BaseUnidirectionalOneToOne", "BaseUnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne", "BasePrivateUnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedProperties), "Int32", "Int32", typeof (int), null, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedProperties), "String", "String", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedProperties), "PrivateString", "PrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (ClassWithMixedProperties), "UnidirectionalOneToOne", "UnidirectionalOneToOneID", typeof (ObjectID), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private void CreatePropertyDefinitionsForDerivedClassWithMixedProperties (ReflectionBasedClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition> ();
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (DerivedClassWithMixedProperties), "String", "NewString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (DerivedClassWithMixedProperties), "PrivateString", "DerivedPrivateString", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (ReflectionBasedPropertyDefinitionFactory.Create (classDefinition, typeof (DerivedClassWithMixedProperties), "OtherString", "OtherString", typeof (string), true, null, StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }
    
  }
}
