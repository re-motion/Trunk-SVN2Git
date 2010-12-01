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
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using Class1 =
    Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot.
        Class1;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationReflectorTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _classWithRealRelationEndPoints;
    private ReflectionBasedClassDefinition _classWithVirtualRelationEndPoints;
    private ReflectionBasedClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private ClassDefinitionCollection _classDefinitions;
    private ITypeDiscoveryService _typeDiscoverServiceStub;
    private ReflectionBasedNameResolver _nameResolver;

    public override void SetUp ()
    {
      base.SetUp();
      _nameResolver = new ReflectionBasedNameResolver();
      _classWithRealRelationEndPoints = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithRealRelationEndPoints));
      _classWithRealRelationEndPoints.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithRealRelationEndPoints.SetRelationDefinitions (new RelationDefinitionCollection());
      _classWithVirtualRelationEndPoints = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithVirtualRelationEndPoints));
      _classWithVirtualRelationEndPoints.SetPropertyDefinitions (new PropertyDefinitionCollection());
      _classWithVirtualRelationEndPoints.SetRelationDefinitions (new RelationDefinitionCollection());
      _classWithBothEndPointsOnSameClassClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));
      _classWithBothEndPointsOnSameClassClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      _classDefinitions = new ClassDefinitionCollection
                          {
                              _classWithRealRelationEndPoints,
                              _classWithVirtualRelationEndPoints,
                              _classWithBothEndPointsOnSameClassClassDefinition
                          };
      _typeDiscoverServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
    }

    [Test]
    public void IsMixedProperty_False ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");
      Assert.That (relationReflector.IsMixedProperty, Is.False);
    }

    [Test]
    public void DeclaringMixin_Null ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");
      Assert.That (relationReflector.DeclaringMixin, Is.Null);
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (ClassWithRealRelationEndPoints)));
    }

    [Test]
    public void GetMetadata_RealSide_ID ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (
          actualRelationDefinition.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints:"
              +
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.Unidirectional"));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToOne_CheckEndPoint0 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToOne_CheckEndPoint1 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToOne_CheckEndPoint0 ()
    {
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne");
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToOne_CheckEndPoint1 ()
    {
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne");
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToMany_CheckEndPoint0 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithRealRelationEndPoint_BidirectionalOneToMany_CheckEndPoint1 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints),
          "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToMany_CheckEndPoint0 ()
    {
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany");
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_WithVirtualRelationEndPoint_BidirectionalOneToMany_CheckEndPoint1 ()
    {
      EnsurePropertyDefinitionExisitsOnClassDefinition (
          _classWithRealRelationEndPoints, typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany");
      var relationReflector = CreateRelationReflector (_classWithVirtualRelationEndPoints, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass),
          "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflector (
          _classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass),
          "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      Assert.That (
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
            "Relation definition error for end point: Class 'ClassWithInvalidUnidirectionalRelation' has no property "
            + "'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation.LeftSide'.")]
    public void GetMetadata_WithUnidirectionalCollectionProperty ()
    {
      var type = typeof (ClassWithInvalidUnidirectionalRelation);

      var propertyInfo = type.GetProperty ("LeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      var relationReflector = CreateRelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide()));

      relationReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation definition error for end point: Class 'ClassWithInvalidBidirectionalRelationLeftSide' has no property 'Remotion.Data.UnitTests."
        + "DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide.InvalidOppositePropertyNameLeftSide'.")]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();

      var propertyInfo = type.GetProperty ("InvalidOppositePropertyNameLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      var relationReflector = CreateRelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide()));

      relationReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsDeclaringTypeOfOppositeProperty_NotReturnTypeOfThisProperty ()
    {
      var originatingProperty = typeof (Class1).GetProperty ("RelationProperty");
      var oppositeProperty = typeof (BaseClass2).GetProperty ("RelationPropertyOnBaseClass");
      var classDeclaringOppositeProperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (BaseClass2));
      var originatingClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Class1));
      var derivedOfClassDeclaringOppositeProperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedClass2), classDeclaringOppositeProperty);

      // This tests the scenario that the relation property's return type is a subclass of the opposite property's declaring type
      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classDeclaringOppositeProperty.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classDeclaringOppositeProperty.ClassType));

      var endPointFactoryMock = MockRepository.GenerateMock<IRelationEndPointDefinitionFactory>();

      var fakeEndPoint1 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var fakeEndPoint2 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      fakeEndPoint1.Stub (stub => stub.PropertyInfo).Return (originatingProperty);
      fakeEndPoint2.Stub (stub => stub.PropertyInfo).Return (oppositeProperty);
      fakeEndPoint1.Stub (stub => stub.ClassDefinition).Return (originatingClass);

      endPointFactoryMock.Expect (mock => mock.CreateEndPoint (originatingClass, originatingProperty, _nameResolver)).Return (fakeEndPoint1);
      endPointFactoryMock.Expect (
          mock =>
          mock.CreateEndPoint (
              Arg.Is (classDeclaringOppositeProperty),
              Arg<PropertyInfo>.Matches (pi => pi.Name == "RelationPropertyOnBaseClass"),
              Arg.Is (_nameResolver)))
          .Return (fakeEndPoint2);
      endPointFactoryMock.Replay();

      var classDefinitions = new ClassDefinitionCollection
                             { originatingClass, classDeclaringOppositeProperty, derivedOfClassDeclaringOppositeProperty };

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver, endPointFactoryMock);
      var result = relationReflector.GetMetadata (classDefinitions);

      endPointFactoryMock.VerifyAllExpectations();
      Assert.That (result.EndPointDefinitions, Is.EqualTo (new[] { fakeEndPoint1, fakeEndPoint2 }));
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsDeclaringTypeOfProperty_WithOverriddenProperty ()
    {
      var originatingProperty =
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.Class1).GetProperty (
              "RelationProperty");
      var oppositeProperty =
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.BaseClass2).GetProperty (
              "OverriddenProperty");
      var classDeclaringOppositeProperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.BaseClass2));
      var originatingClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.Class1));
      var derivedOfClassDeclaringOppositeProperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOverriddenOppositeProperty_BelowInheritanceRoot.DerivedClass2),
              classDeclaringOppositeProperty);
     
      // This tests the scenario that the relation property's return type is a subclass of the opposite property's declaring type
      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classDeclaringOppositeProperty.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classDeclaringOppositeProperty.ClassType));

      var endPointFactoryMock = MockRepository.GenerateMock<IRelationEndPointDefinitionFactory>();

      var fakeEndPoint1 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var fakeEndPoint2 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      fakeEndPoint1.Stub (stub => stub.PropertyInfo).Return (originatingProperty);
      fakeEndPoint2.Stub (stub => stub.PropertyInfo).Return (oppositeProperty);
      fakeEndPoint1.Stub (stub => stub.ClassDefinition).Return (originatingClass);

      endPointFactoryMock.Expect (mock => mock.CreateEndPoint (originatingClass, originatingProperty, _nameResolver)).Return (fakeEndPoint1);
      endPointFactoryMock.Expect (
          mock =>
          mock.CreateEndPoint (
              Arg.Is (classDeclaringOppositeProperty),
              Arg<PropertyInfo>.Matches (pi => pi.Name == "OverriddenProperty"),
              Arg.Is (_nameResolver)))
          .Return (fakeEndPoint2);
      endPointFactoryMock.Replay();

      var classDefinitions = new ClassDefinitionCollection
                             { originatingClass, classDeclaringOppositeProperty, derivedOfClassDeclaringOppositeProperty };

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver, endPointFactoryMock);
      var result = relationReflector.GetMetadata (classDefinitions);

      endPointFactoryMock.VerifyAllExpectations();
      Assert.That (result.EndPointDefinitions, Is.EqualTo (new[] { fakeEndPoint1, fakeEndPoint2 }));
    }

    [Test]
    public void GetMetadata_OppositeClassDefinition_IsInheritanceRoot_IfDeclaringTypeOfPropertyIsNotInMapping ()
    {
      var originatingProperty =
          typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot.Class1).GetProperty ("RelationProperty");
      var oppositeProperty = typeof (ClassAboveInheritanceRoot).GetProperty ("RelationPropertyOnClassAboveInheritanceRoot");
      var classNotInMapping = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassAboveInheritanceRoot));
      var originatingClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TestDomain.RelationReflector.RelatedTypeDoesNotMatchOppositeProperty_AboveInheritanceRoot.Class1));
      var derivedOfClassDeclaringOppositeProperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Class2));
      
      // This tests the scenario that the relation property's return type is a subclass of the opposite property's declaring type
      Assert.That (originatingProperty.PropertyType, Is.Not.SameAs (classNotInMapping.ClassType));
      Assert.That (originatingProperty.PropertyType.BaseType, Is.SameAs (classNotInMapping.ClassType));

      var endPointFactoryMock = MockRepository.GenerateMock<IRelationEndPointDefinitionFactory>();

      var fakeEndPoint1 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var fakeEndPoint2 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      fakeEndPoint1.Stub (stub => stub.PropertyInfo).Return (originatingProperty);
      fakeEndPoint2.Stub (stub => stub.PropertyInfo).Return (oppositeProperty);
      fakeEndPoint1.Stub (stub => stub.ClassDefinition).Return (originatingClass);

      endPointFactoryMock.Expect (mock => mock.CreateEndPoint (originatingClass, originatingProperty, _nameResolver)).Return (fakeEndPoint1);
      endPointFactoryMock.Expect (
          mock =>
          mock.CreateEndPoint (
              Arg.Is (derivedOfClassDeclaringOppositeProperty),
              Arg<PropertyInfo>.Matches (pi => pi.Name == "RelationPropertyOnClassAboveInheritanceRoot"),
              Arg.Is (_nameResolver)))
          .Return (fakeEndPoint2);
      endPointFactoryMock.Replay();

      var classDefinitions = new ClassDefinitionCollection { originatingClass, derivedOfClassDeclaringOppositeProperty };

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver, endPointFactoryMock);
      var result = relationReflector.GetMetadata (classDefinitions);

      endPointFactoryMock.VerifyAllExpectations();
      Assert.That (result.EndPointDefinitions, Is.EqualTo (new[] { fakeEndPoint1, fakeEndPoint2 }));
    }

    [Test]
    public void GetMetadata_RelatedPropertyTypeIsNotInMapping ()
    {
      var originatingProperty =
          typeof (TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.Class1).GetProperty ("BidirectionalRelationProperty");
      var oppositeProperty = typeof (ClassNotInMapping).GetProperty ("RelationProperty");
      var originatingClass =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping.Class1));
      var classDefinitions = new ClassDefinitionCollection { originatingClass };

      var endPointFactoryMock = MockRepository.GenerateMock<IRelationEndPointDefinitionFactory>();
      var fakeEndPoint1 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      var fakeEndPoint2 = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      fakeEndPoint1.Stub (stub => stub.PropertyInfo).Return (originatingProperty);
      fakeEndPoint2.Stub (stub => stub.PropertyInfo).Return (oppositeProperty);
      fakeEndPoint1.Stub (stub => stub.ClassDefinition).Return (originatingClass);

      endPointFactoryMock.Expect (mock => mock.CreateEndPoint (originatingClass, originatingProperty, _nameResolver)).Return (fakeEndPoint1);
      endPointFactoryMock.Expect (
          mock =>
          mock.CreateEndPoint (
              Arg<ReflectionBasedClassDefinition>.Matches (cd => cd.GetType() == typeof (TypeNotFoundClassDefinition)),
              Arg<PropertyInfo>.Matches (pi => pi.Name == "RelationProperty"),
              Arg.Is (_nameResolver)))
          .Return (fakeEndPoint2);
      endPointFactoryMock.Replay();

      var relationReflector = new RelationReflector (originatingClass, originatingProperty, _nameResolver, endPointFactoryMock);
      relationReflector.GetMetadata (classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
            "The classDefinition's class type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.BaseClass' is not assignable "
            + "to the property's declaring type.\r\n"
            +
            "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
            + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      var classType = typeof (BaseClass);
      var declaringType = typeof (DerivedClassHavingAnOverriddenPropertyWithMappingAttribute);

      var propertyInfo = declaringType.GetProperty ("Int32");

      CreateRelationReflector (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (classType), propertyInfo);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationLeftSide);
    }

    private Type GetClassWithInvalidBidirectionalRelationRightSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationRightSide);
    }

    private RelationReflector CreateRelationReflector (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      return new RelationReflector (classDefinition, propertyInfo, _nameResolver, new ReflectionBasedRelationEndPointDefinitionFactory());
    }

    private RelationReflector CreateRelationReflector (ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      PropertyInfo propertyInfo = EnsurePropertyDefinitionExisitsOnClassDefinition (classDefinition, declaringType, propertyName);
      return CreateRelationReflector (classDefinition, propertyInfo);
    }

    private PropertyInfo EnsurePropertyDefinitionExisitsOnClassDefinition (
        ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver());
      var propertyDefinition = propertyReflector.GetMetadata();

      if (!classDefinition.MyPropertyDefinitions.Contains (propertyDefinition.PropertyName))
      {
        PrivateInvoke.SetNonPublicField (classDefinition, "_propertyDefinitions", new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      }
      
      return propertyInfo;
    }
  }
}