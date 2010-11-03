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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationReflectorTest : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _classWithRealRelationEndPoints;
    private ReflectionBasedClassDefinition _classWithVirtualRelationEndPoints;
    private ReflectionBasedClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp ();
      _classWithRealRelationEndPoints = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithRealRelationEndPoints));
      _classWithVirtualRelationEndPoints = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithVirtualRelationEndPoints));
      _classWithBothEndPointsOnSameClassClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));

      _classDefinitions = new ClassDefinitionCollection
                            {
                                _classWithRealRelationEndPoints,
                                _classWithVirtualRelationEndPoints,
                                _classWithBothEndPointsOnSameClassClassDefinition
                            };

      _relationDefinitions = new RelationDefinitionCollection ();
    }

    [Test]
    public void IsMixedProperty_False ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");
      Assert.That (relationReflector.IsMixedProperty, Is.False);
    }

    [Test]
    public void DeclaringMixin_Null ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");
      Assert.That (relationReflector.DeclaringMixin, Is.Null);
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (ClassWithRealRelationEndPoints)));
    }

    [Test]
    public void GetMetadata_RealSide_ID ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.ID, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints"
          +"->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.Unidirectional"));
    }

    [Test]
    public void GetMetadata_Unidirectional_AddedToRightClassDefinition ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithRealRelationEndPoints.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithVirtualRelationEndPoints.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "Unidirectional");

      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.That (actualRelationDefinition, Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions.GetMandatory (expectedRelationDefinition.ID), Is.SameAs (actualRelationDefinition));
      Assert.That (actualRelationDefinition, Is.SameAs (expectedRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_AddedToBothClassDefinitions ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithRealRelationEndPoints.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithVirtualRelationEndPoints.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (oppositeEndPointDefinition.PropertyName, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithRealRelationEndPoints)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToOne");

      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.That (actualRelationDefinition, Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions.GetMandatory (expectedRelationDefinition.ID), Is.SameAs (actualRelationDefinition));
      Assert.That (actualRelationDefinition, Is.SameAs (expectedRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne");
      var relationReflector = new RelationReflector (_classWithVirtualRelationEndPoints, propertyInfo, Configuration.NameResolver);

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Null);
      Assert.That (_relationDefinitions, Is.Empty);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_AddedToBothClassDefinitions ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithRealRelationEndPoints.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithVirtualRelationEndPoints.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithRealRelationEndPoints.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithRealRelationEndPoints));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithVirtualRelationEndPoints));
      Assert.That (oppositeEndPointDefinition.PropertyName, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithRealRelationEndPoints,
          typeof (ClassWithRealRelationEndPoints), "BidirectionalOneToMany");

      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.That (actualRelationDefinition, Is.Not.Null);
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions.GetMandatory (expectedRelationDefinition.ID), Is.SameAs (actualRelationDefinition));
      Assert.That (actualRelationDefinition, Is.SameAs (expectedRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany");
      var relationReflector = new RelationReflector (_classWithVirtualRelationEndPoints, propertyInfo, Configuration.NameResolver);

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Null);
      Assert.That (_relationDefinitions, Is.Empty);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_AddedToClassDefinition ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass), "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.That (_classWithBothEndPointsOnSameClassClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass), "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];

      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithBothEndPointsOnSameClassClassDefinition,
          typeof (ClassWithBothEndPointsOnSameClass), "Parent");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithBothEndPointsOnSameClassClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Relation definition error for end point: Class 'ClassWithInvalidUnidirectionalRelation' has no property "
        +"'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation.LeftSide'.")]
    public void GetMetadata_WithUnidirectionalCollectionProperty ()
    {
      var type = typeof (ClassWithInvalidUnidirectionalRelation);

      var propertyInfo = type.GetProperty ("LeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'Invalid' could not be found on type "
        +"'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        +"property: InvalidOppositePropertyNameLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();

      var propertyInfo = type.GetProperty ("InvalidOppositePropertyNameLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The declaring type 'TargetClass2ForMixinAddingBidirectionalRelationTwice' does not match the type of the opposite relation "
        + "propery 'VirtualSide' declared on type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors."
        + "RelationTargetForMixinAddingBidirectionalRelationTwice'.\r\nDeclaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
        + "TestDomain.Errors.MixinAddingBidirectionalRelationTwice, property: RealSide")]
    public void GetMetadata_WithInvalidOppositePropertyType_CausedByMixin ()
    {
      Type type = GetTargetClass2ForMixinAddingBidirectionalRelationTwice ();
      Type mixinType = GetMixinAddingBidirectionalRelationTwice ();

      var propertyInfo = mixinType.GetProperty ("RealSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type, mixinType);

      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata ());

      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetRelationTargetForMixinAddingBidirectionalRelationTwice ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }
    
    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "The classDefinition's class type 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.BaseClass' is not assignable "
        + "to the property's declaring type.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      var classType = typeof (BaseClass);
      var declaringType = typeof (DerivedClassHavingAnOverriddenPropertyWithMappingAttribute);

      var propertyInfo = declaringType.GetProperty ("Int32");

      new RelationReflector (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (classType), propertyInfo, Configuration.NameResolver);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationLeftSide);

    }

    private Type GetClassWithInvalidBidirectionalRelationRightSide ()
    {
      return typeof (ClassWithInvalidBidirectionalRelationRightSide);

    }

    private Type GetRelationTargetForMixinAddingBidirectionalRelationTwice ()
    {
      return typeof (RelationTargetForMixinAddingBidirectionalRelationTwice);

    }

    private Type GetTargetClass2ForMixinAddingBidirectionalRelationTwice ()
    {
      return typeof (TargetClass2ForMixinAddingBidirectionalRelationTwice);

    }

    private Type GetMixinAddingBidirectionalRelationTwice ()
    {
      return typeof (MixinAddingBidirectionalRelationTwice);

    }

    private RelationReflector CreateRelationReflectorForProperty (ReflectionBasedClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      var propertyReflector = new PropertyReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
      var propertyDefinition = propertyReflector.GetMetadata ();

      if (!classDefinition.MyPropertyDefinitions.Contains (propertyDefinition.PropertyName))
        classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      return new RelationReflector (classDefinition, propertyInfo, new ReflectionBasedNameResolver ());
    }
  }
}
