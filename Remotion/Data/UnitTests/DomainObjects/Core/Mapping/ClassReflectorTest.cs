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
using Rhino.Mocks;

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
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Int32")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "UnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseString")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "UnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);
      DomainModelConstraintProviderMock.Replay();
      
      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClassWithDifferentProperties))).Return ("ClassWithDifferentProperties");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (ClassWithDifferentProperties),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);
      var expected = CreateClassWithDifferentPropertiesClassDefinition();

      var actual = classReflector.GetMetadata (null);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OtherString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OtherString")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (DerivedClassWithDifferentProperties))).Return ("DerivedClassWithDifferentProperties");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (DerivedClassWithDifferentProperties),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);
      var expected = CreateDerivedClassWithDifferentPropertiesClassDefinition();

      var baseClassDefinition = CreateClassWithDifferentPropertiesClassDefinition();
      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadata_ForMixedClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P0")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P1")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P2")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P5")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P7")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P8")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P0a")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (TargetClassA))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (TargetClassA), MappingObjectFactory, Configuration.NameResolver, ClassIDProviderMock, DomainModelConstraintProviderMock);
      var actual = classReflector.GetMetadata (null);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD) }));
      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadata_ForDerivedMixedClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P0")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P1")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P2")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P7")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P8")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P5")))
          .Return (null);
      DomainModelConstraintProviderMock
         .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P0a")))
         .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (TargetClassA))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflectorForBaseClass = new ClassReflector (
          typeof (TargetClassA), MappingObjectFactory, Configuration.NameResolver, ClassIDProviderMock, DomainModelConstraintProviderMock);
      var baseClass = classReflectorForBaseClass.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();

      ClassIDProviderMock.BackToRecord();
      DomainModelConstraintProviderMock.BackToRecord();

      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P3")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P4")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P9")))
        .Return (null);
      DomainModelConstraintProviderMock
        .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "P6")))
        .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (TargetClassB))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (TargetClassB), MappingObjectFactory, Configuration.NameResolver, ClassIDProviderMock, DomainModelConstraintProviderMock);
      var actual = classReflector.GetMetadata (baseClass);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadata_ForClassWithVirtualRelationEndPoints ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateBidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateBidirectionalOneToMany")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NoAttribute")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NotNullable")))
          .Return (false);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Return (true);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClassWithVirtualRelationEndPoints))).Return ("ClassWithVirtualRelationEndPoints");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (ClassWithVirtualRelationEndPoints),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);
      var expected = CreateClassWithVirtualRelationEndPointsClassDefinition();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection());
      CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (expected);

      var actual = classReflector.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_GetClassID ()
    {
      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClassHavingClassIDAttribute))).Return ("ClassIDForClassHavingClassIDAttribute");
      ClassIDProviderMock.Replay();

      var classReflector = new ClassReflector (
          typeof (ClassHavingClassIDAttribute),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);

      var actual = classReflector.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations();
      DomainModelConstraintProviderMock.VerifyAllExpectations();
      Assert.IsNotNull (actual);
      Assert.AreEqual ("ClassIDForClassHavingClassIDAttribute", actual.ID);
    }

    [Test]
    public void GetMetadata_ForClosedGenericClass ()
    {
      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClosedGenericClass))).Return ("ClassID");
      ClassIDProviderMock.Replay();
     
      var classReflector = new ClassReflector (
          typeof (ClosedGenericClass), MappingObjectFactory, Configuration.NameResolver, ClassIDProviderMock, DomainModelConstraintProviderMock);

      Assert.IsNotNull (classReflector.GetMetadata (null));
      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadata_ForClassWithoutStorageGroupAttribute ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "IntProperty")))
          .Return (null);
      DomainModelConstraintProviderMock.Replay ();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClassDerivedFromSimpleDomainObject))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (ClassDerivedFromSimpleDomainObject),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);

      var actual = classReflector.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttribute ()
    {
      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (DerivedClassWithStorageGroupAttribute))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (DerivedClassWithStorageGroupAttribute),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);

      var actual = classReflector.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
      Assert.IsNotNull (actual);
      Assert.That (actual.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForBaseClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "Int32")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "UnidirectionalOneToOne")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseString")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "UnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);
      DomainModelConstraintProviderMock.Replay();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (ClassWithDifferentProperties))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (ClassWithDifferentProperties),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);

      var actual = classReflector.GetMetadata (null);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations ();
      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForDerivedClass ()
    {
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OtherString")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.GetMaxLength (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (null);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OtherString")))
          .Return (true);
      DomainModelConstraintProviderMock
          .Expect (mock => mock.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderMock.Replay();

      ClassIDProviderMock.Expect (mock => mock.GetClassID (typeof (DerivedClassWithDifferentProperties))).Return ("ClassID");
      ClassIDProviderMock.Replay ();

      var classReflector = new ClassReflector (
          typeof (DerivedClassWithDifferentProperties),
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderMock,
          DomainModelConstraintProviderMock);
      var baseClassDefinition = ClassDefinitionFactory.CreateFinishedOrderDefinition();

      var actual = classReflector.GetMetadata (baseClassDefinition);

      ClassIDProviderMock.VerifyAllExpectations ();
      DomainModelConstraintProviderMock.VerifyAllExpectations();
      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.False);
    }

    private ClassDefinition CreateClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
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
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
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

      return new RelationEndPointDefinition (
          classDefinition[MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo)],
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

      return new VirtualRelationEndPointDefinition (
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo),
          isMandatory,
          cardinality,
          propertyInfo.PropertyType,
          sortExpressionText,
          propertyInfo);
    }

    private IPropertyInformation GetPropertyInfo (Type declaringType, string shortPropertyName)
    {
      var propertyInfo = declaringType.GetProperty (
          shortPropertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '" + shortPropertyName + "' not found on type '" + declaringType + "'.");
      return PropertyInfoAdapter.Create (propertyInfo);
    }
  }
}