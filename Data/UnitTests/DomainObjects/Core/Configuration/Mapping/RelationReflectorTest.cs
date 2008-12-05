// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class RelationReflectorTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classWithManySideRelationPropertiesClassDefinition;
    private ReflectionBasedClassDefinition _classWithOneSideRelationPropertiesClassDefinition;
    private ReflectionBasedClassDefinition _classWithBothEndPointsOnSameClassClassDefinition;
    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    public override void SetUp ()
    {
      base.SetUp();
      _classWithManySideRelationPropertiesClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithManySideRelationProperties));
      _classWithOneSideRelationPropertiesClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
      _classWithBothEndPointsOnSameClassClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));

      _classDefinitions = new ClassDefinitionCollection
                            {
                                _classWithManySideRelationPropertiesClassDefinition,
                                _classWithOneSideRelationPropertiesClassDefinition,
                                _classWithBothEndPointsOnSameClassClassDefinition
                            };

      _relationDefinitions = new RelationDefinitionCollection();
    }

    [Test]
    public void IsMixedProperty_False ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition, 
          typeof (ClassWithManySideRelationProperties), "Unidirectional");
      Assert.That (relationReflector.IsMixedProperty, Is.False);
    }

    [Test]
    public void DeclaringMixin_Null ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");
      Assert.That (relationReflector.DeclaringMixin, Is.Null);
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");
      Assert.That (relationReflector.DeclaringDomainObjectTypeForProperty, Is.EqualTo (typeof (ClassWithManySideRelationProperties)));
    }

    [Test]
    public void GetMetadata_RealSide_ID ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.ID, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional"));
    }

    [Test]
    public void GetMetadata_Unidirectional_AddedToRightClassDefinition ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithManySideRelationPropertiesClassDefinition.GetPropertyDefinitions ()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithManySideRelationPropertiesClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithOneSideRelationPropertiesClassDefinition));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_Unidirectional_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "Unidirectional");

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
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      
      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithManySideRelationPropertiesClassDefinition.GetPropertyDefinitions ()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithManySideRelationPropertiesClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithOneSideRelationPropertiesClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ClassWithManySideRelationProperties)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToOne");

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
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      var relationReflector = new RelationReflector (_classWithOneSideRelationPropertiesClassDefinition, propertyInfo, Configuration.NameResolver);

      Assert.That (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions), Is.Null);
      Assert.That (_relationDefinitions, Is.Empty);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_AddedToBothClassDefinitions ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_EndPoint0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOfType (typeof (RelationEndPointDefinition)));
      var endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithManySideRelationPropertiesClassDefinition.GetPropertyDefinitions ()[0]));
      Assert.That (endPointDefinition.ClassDefinition, Is.SameAs (_classWithManySideRelationPropertiesClassDefinition));
      Assert.That (endPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_EndPoint1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToMany");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.That (actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOfType (typeof (VirtualRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.That (oppositeEndPointDefinition.ClassDefinition, Is.SameAs (_classWithOneSideRelationPropertiesClassDefinition));
      Assert.That (oppositeEndPointDefinition.PropertyName, Is.EqualTo (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithManySideRelationProperties>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_RelationAlreadyInRelationDefinitionCollection ()
    {
      var expectedRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToMany");
      var actualRelationReflector = CreateRelationReflectorForProperty (_classWithManySideRelationPropertiesClassDefinition,
          typeof (ClassWithManySideRelationProperties), "BidirectionalOneToMany");

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
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      var relationReflector = new RelationReflector (_classWithOneSideRelationPropertiesClassDefinition, propertyInfo, Configuration.NameResolver);

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
      Assert.That (endPointDefinition.PropertyDefinition, Is.EqualTo (_classWithBothEndPointsOnSameClassClassDefinition.GetPropertyDefinitions ()[0]));
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
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children"));
      Assert.That (oppositeEndPointDefinition.PropertyType, Is.SameAs (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>)));
      Assert.That (oppositeEndPointDefinition.RelationDefinition, Is.SameAs (actualRelationDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "The property type of an uni-directional relation property must be assignable to Remotion.Data.DomainObjects.DomainObject.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation, "
        + "property: LeftSide")]
    public void GetMetadata_WithUnidirectionalCollectionProperty ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation", true, false);

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
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyNameLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();

      var propertyInfo = type.GetProperty ("InvalidOppositePropertyNameLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "defines a 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute' whose opposite property does not match.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide")]
    public void GetMetadata_WithInvalidPropertyNameInBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      
      var propertyInfo = type.GetProperty ("InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSide' does not match the type of the opposite relation propery 'InvalidOppositePropertyTypeRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyType ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      
      var propertyInfo = type.GetProperty ("InvalidOppositePropertyTypeLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide()));
      
      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The declaring type 'TargetClass2ForMixinAddingBidirectionalRelationTwice' does not match the type of the opposite relation "
        + "propery 'VirtualSide' declared on type 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors." 
        + "RelationTargetForMixinAddingBidirectionalRelationTwice'.\r\nDeclaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping." 
        + "TestDomain.Errors.MixinAddingBidirectionalRelationTwice, property: RealSide")]
    public void GetMetadata_WithInvalidOppositePropertyType_CausedByMixin ()
    {
      Type type = GetTargetClass2ForMixinAddingBidirectionalRelationTwice ();
      Type mixinType = GetMixinAddingBidirectionalRelationTwice();

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
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSideNotInMapping' cannot be assigned to the type of the opposite relation propery 'BaseInvalidOppositePropertyTypeRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSideNotInMapping, "
        + "property: BaseInvalidOppositePropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeOnBaseClassNotInMapping ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();

      var propertyInfo = type.GetProperty ("BaseInvalidOppositePropertyTypeLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSide' does not match the type of the opposite relation propery 'InvalidOppositeCollectionPropertyTypeRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositeCollectionPropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeForCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      
      var propertyInfo = type.GetProperty ("InvalidOppositeCollectionPropertyTypeLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSideNotInMapping' cannot be assigned to the type of the opposite relation propery 'BaseInvalidOppositeCollectionPropertyTypeRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSideNotInMapping, "
        + "property: BaseInvalidOppositeCollectionPropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeForCollectionPropertyOnBaseClassNotInMapping ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();

      var propertyInfo = type.GetProperty ("BaseInvalidOppositeCollectionPropertyTypeLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      
      var propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeForCollectionPropertyRightSide' declared on type "
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();

      var propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "A bidirectional relation can only have one virtual relation end point.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NoContainsKeyLeftSide")]
    public void GetMetadata_WithTwoVirtualEndPoints ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);

      var propertyInfo = type.GetProperty ("NoContainsKeyLeftSide");
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type);
      var relationReflector = new RelationReflector (classDefinition, propertyInfo, Configuration.NameResolver);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "The classDefinition's class type 'Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.BaseClass' is not assignable "
        + "to the property's declaring type.\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      Type classType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.BaseClass", true, false);
      Type declaringType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute", true, false);
      var propertyInfo = declaringType.GetProperty ("Int32");

      new RelationReflector (ClassDefinitionFactory.CreateReflectionBasedClassDefinition (classType), propertyInfo, Configuration.NameResolver);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
    }

    private Type GetClassWithInvalidBidirectionalRelationRightSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide", true, false);
    }

    private Type GetRelationTargetForMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.RelationTargetForMixinAddingBidirectionalRelationTwice", true, false);
    }

    private Type GetTargetClass2ForMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.TargetClass2ForMixinAddingBidirectionalRelationTwice", true, false);
    }

    private Type GetMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors.MixinAddingBidirectionalRelationTwice", true, false);
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
