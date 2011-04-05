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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.
        StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ClassReflectorTest : MappingReflectionTestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private RelationEndPointDefinitionChecker _endPointDefinitionChecker;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _endPointDefinitionChecker = new RelationEndPointDefinitionChecker();
    }

    [Test]
    public void GetMetadata_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithDifferentProperties), MappingObjectFactory, Configuration.NameResolver);
      var expected = CreateClassWithDifferentPropertiesClassDefinition();

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithDifferentProperties), MappingObjectFactory, Configuration.NameResolver);
      var expected = CreateDerivedClassWithDifferentPropertiesClassDefinition();

      var baseClassDefinition = CreateClassWithDifferentPropertiesClassDefinition();
      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForMixedClass ()
    {
      var classReflector = new ClassReflector (typeof (TargetClassA), MappingObjectFactory, Configuration.NameResolver);
      var actual = classReflector.GetMetadata (null);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD) }));
    }

    [Test]
    public void GetMetadata_ForDerivedMixedClass ()
    {
      var classReflectorForBaseClass = new ClassReflector (typeof (TargetClassA), MappingObjectFactory, Configuration.NameResolver);
      var baseClass = classReflectorForBaseClass.GetMetadata (null);

      var classReflector = new ClassReflector (typeof (TargetClassB), MappingObjectFactory, Configuration.NameResolver);
      var actual = classReflector.GetMetadata (baseClass);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void GetMetadata_ForClassWithVirtualRelationEndPoints ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithVirtualRelationEndPoints), MappingObjectFactory, Configuration.NameResolver);
      var expected = CreateClassWithVirtualRelationEndPointsClassDefinition();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection());
      CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (expected);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForClassHavingClassIDAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassHavingClassIDAttribute), MappingObjectFactory, Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
    }

    [Test]
    public void GetMetadata_ForClosedGenericClass ()
    {
      var classReflector = new ClassReflector (typeof (ClosedGenericClass), MappingObjectFactory, Configuration.NameResolver);

      Assert.IsNotNull (classReflector.GetMetadata (null));
    }

    [Test]
    public void GetMetadata_ForClassWithoutStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (ClassDerivedFromSimpleDomainObject), MappingObjectFactory, Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttribute ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithStorageGroupAttribute), MappingObjectFactory, Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForBaseClass ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithDifferentProperties), MappingObjectFactory, Configuration.NameResolver);

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForDerivedClass ()
    {
      var classReflector = new ClassReflector (typeof (DerivedClassWithDifferentProperties), MappingObjectFactory, Configuration.NameResolver);
      var baseClassDefinition = ClassDefinitionFactory.CreateFinishedOrderDefinition();

      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.False);
    }

    private ClassDefinition CreateClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithDifferentProperties",
          "ClassWithDifferentProperties",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithDifferentProperties),
          false);

      CreatePropertyDefinitionsForClassWithDifferentProperties (classDefinition);
      CreateEndPointDefinitionsForClassWithDifferentProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateDerivedClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedClassWithDifferentProperties",
          "DerivedClassWithDifferentProperties",
          UnitTestDomainStorageProviderDefinition,
          typeof (DerivedClassWithDifferentProperties),
          false,
          CreateClassWithDifferentPropertiesClassDefinition());
      CreatePropertyDefinitionsForDerivedClassWithDifferentProperties (classDefinition);
      CreateEndPointDefinitionsForDerivedClassWithDifferentProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithVirtualRelationEndPointsClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithVirtualRelationEndPoints",
          "ClassWithVirtualRelationEndPoints",
          UnitTestDomainStorageProviderDefinition,
          typeof (ClassWithVirtualRelationEndPoints),
          false);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithDifferentPropertiesNotInMapping),
              "BaseString",
              "BaseString",
              typeof (string),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithDifferentPropertiesNotInMapping),
              "BaseUnidirectionalOneToOne",
              "BaseUnidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithDifferentPropertiesNotInMapping),
              "BasePrivateUnidirectionalOneToOne",
              "BasePrivateUnidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition, typeof (ClassWithDifferentProperties), "Int32", "Int32", typeof (int), false, null, StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition, typeof (ClassWithDifferentProperties), "String", "String", typeof (string), true, null, StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithDifferentProperties),
              "PrivateString",
              "PrivateString",
              typeof (string),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithDifferentProperties),
              "UnidirectionalOneToOne",
              "UnidirectionalOneToOneID",
              typeof (ObjectID),
              true,
              null,
              StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      endPoints.Add (CreateRelationEndPointDefinition (classDefinition, typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne", false));
      endPoints.Add (
          CreateRelationEndPointDefinition (classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne", false));
      endPoints.Add (
          CreateRelationEndPointDefinition (
              classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne", false));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private void CreatePropertyDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "String",
              "NewString",
              typeof (string),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "PrivateString",
              "DerivedPrivateString",
              typeof (string),
              true,
              null,
              StorageClass.Persistent));
      properties.Add (
          PropertyDefinitionFactory.Create (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "OtherString",
              "OtherString",
              typeof (string),
              true,
              null,
              StorageClass.Persistent));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();

      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "NoAttribute", false, CardinalityType.Many, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "NotNullable", true, CardinalityType.Many, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne", false, CardinalityType.One, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany", false, CardinalityType.Many, "NoAttribute"));

      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne", false, CardinalityType.One, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BaseBidirectionalOneToMany",
              false,
              CardinalityType.Many,
              "NoAttribute"));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToOne",
              false,
              CardinalityType.One,
              null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToMany",
              false,
              CardinalityType.Many,
              "NoAttribute"));

      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private void CreateEndPointDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (
        ClassDefinition classDefinition, Type declaringType, string shortPropertyName, bool isMandatory)
    {
      var propertyInfo = GetPropertyInfo (declaringType, shortPropertyName);

      return
          new RelationEndPointDefinition (
              classDefinition[MappingConfiguration.Current.NameResolver.GetPropertyName (new PropertyInfoAdapter (propertyInfo))],
              isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string shortPropertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string sortExpressionText)
    {
      var propertyInfo = GetPropertyInfo (declaringType, shortPropertyName);

      return new ReflectionBasedVirtualRelationEndPointDefinition (
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName (new PropertyInfoAdapter (propertyInfo)),
          isMandatory,
          cardinality,
          propertyInfo.PropertyType,
          sortExpressionText,
          propertyInfo);
    }

    private PropertyInfo GetPropertyInfo (Type declaringType, string shortPropertyName)
    {
      var propertyInfo = declaringType.GetProperty (
          shortPropertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '" + shortPropertyName + "' not found on type '" + declaringType + "'.");
      return propertyInfo;
    }
  }
}