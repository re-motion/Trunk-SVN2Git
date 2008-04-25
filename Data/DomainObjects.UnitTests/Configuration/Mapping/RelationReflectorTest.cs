using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
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
      _classWithManySideRelationPropertiesClassDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithManySideRelationProperties));
      _classWithOneSideRelationPropertiesClassDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
      _classWithBothEndPointsOnSameClassClassDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithBothEndPointsOnSameClass));

      _classDefinitions = new ClassDefinitionCollection();
      _classDefinitions.Add (_classWithManySideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithOneSideRelationPropertiesClassDefinition);
      _classDefinitions.Add (_classWithBothEndPointsOnSameClassClassDefinition);

      _relationDefinitions = new RelationDefinitionCollection();
    }

    [Test]
    public void IsMixedProperty_False ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("Unidirectional");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      Assert.IsFalse (relationReflector.IsMixedProperty);
    }

    [Test]
    public void DomainObjectTypeDeclaringProperty ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("Unidirectional");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      Assert.AreEqual (typeof (ClassWithManySideRelationProperties), relationReflector.DomainObjectTypeDeclaringProperty);
    }

    [Test]
    public void GetMetadata_Unidirectional ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("Unidirectional");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.Unidirectional",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (AnonymousRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      AnonymousRelationEndPointDefinition oppositeEndPointDefinition =
          (AnonymousRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Not.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_UnidirectionalAndRelationAlreadyInRelationDefinitionCollection ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("Unidirectional");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector expectedRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      RelationReflector actualRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actualRelationDefinition);
      Assert.AreEqual (1, _relationDefinitions.Count);
      Assert.AreSame (actualRelationDefinition, _relationDefinitions.GetMandatory (expectedRelationDefinition.ID));
      Assert.AreSame (expectedRelationDefinition, actualRelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToOne",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToOne",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOneAndRelationAlreadyInRelationDefinitionCollection ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToOne");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector expectedRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      RelationReflector actualRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actualRelationDefinition);
      Assert.AreEqual (1, _relationDefinitions.Count);
      Assert.AreSame (actualRelationDefinition, _relationDefinitions.GetMandatory (expectedRelationDefinition.ID));
      Assert.AreSame (expectedRelationDefinition, actualRelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToOne");
      RelationReflector relationReflector = new RelationReflector (_classWithOneSideRelationPropertiesClassDefinition, propertyInfo);

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithManySideRelationProperties.BidirectionalOneToMany",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);
      Assert.That (_classWithManySideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithOneSideRelationPropertiesClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationProperties.BidirectionalOneToMany",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);
      Assert.That (_classWithOneSideRelationPropertiesClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToManyAndRelationAlreadyInRelationDefinitionCollection ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("BidirectionalOneToMany");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata ();
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector expectedRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition expectedRelationDefinition = expectedRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      RelationReflector actualRelationReflector = new RelationReflector (_classWithManySideRelationPropertiesClassDefinition, propertyInfo);
      RelationDefinition actualRelationDefinition = actualRelationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.IsNotNull (actualRelationDefinition);
      Assert.AreEqual (1, _relationDefinitions.Count);
      Assert.AreSame (actualRelationDefinition, _relationDefinitions.GetMandatory (expectedRelationDefinition.ID));
      Assert.AreSame (expectedRelationDefinition, actualRelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_VirtualEndPoint ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BidirectionalOneToMany");
      RelationReflector relationReflector = new RelationReflector (_classWithOneSideRelationPropertiesClassDefinition, propertyInfo);

      Assert.IsNull (relationReflector.GetMetadata (_classDefinitions, _relationDefinitions));
      Assert.That (_relationDefinitions.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany_WithBothEndPointsOnSameClass ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithBothEndPointsOnSameClass).GetProperty ("Parent");
      PropertyReflector propertyReflector = new PropertyReflector (_classWithBothEndPointsOnSameClassClassDefinition, propertyInfo);
      PropertyDefinition propertyDefinition = propertyReflector.GetMetadata();
      _classWithBothEndPointsOnSameClassClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      RelationReflector relationReflector = new RelationReflector (_classWithBothEndPointsOnSameClassClassDefinition, propertyInfo);

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Parent",
          actualRelationDefinition.ID);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[0]);
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[0];
      Assert.AreEqual (propertyDefinition, endPointDefinition.PropertyDefinition);
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, endPointDefinition.ClassDefinition);
      Assert.AreSame (actualRelationDefinition, endPointDefinition.RelationDefinition);

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actualRelationDefinition.EndPointDefinitions[1]);
      VirtualRelationEndPointDefinition oppositeEndPointDefinition =
          (VirtualRelationEndPointDefinition) actualRelationDefinition.EndPointDefinitions[1];
      Assert.AreSame (_classWithBothEndPointsOnSameClassClassDefinition, oppositeEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithBothEndPointsOnSameClass.Children",
          oppositeEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithBothEndPointsOnSameClass>), oppositeEndPointDefinition.PropertyType);
      Assert.AreSame (actualRelationDefinition, oppositeEndPointDefinition.RelationDefinition);

      Assert.That (_classWithBothEndPointsOnSameClassClassDefinition.MyRelationDefinitions, List.Contains (actualRelationDefinition));

      Assert.That (_relationDefinitions.Count, Is.EqualTo (1));
      Assert.That (_relationDefinitions, List.Contains (actualRelationDefinition));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "The property type of an uni-directional relation property must be assignable to Remotion.Data.DomainObjects.DomainObject.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation, "
        + "property: LeftSide")]
    public void GetMetadata_WithUnidirectionalCollectionProperty ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidUnidirectionalRelation", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("LeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'Invalid' could not be found on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyNameLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyName ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositePropertyNameLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "defines a 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute' whose opposite property does not match.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide")]
    public void GetMetadata_WithInvalidPropertyNameInBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSide' does not match the type of the opposite relation propery 'InvalidOppositePropertyTypeRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositePropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyType ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositePropertyTypeLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide()));
      
      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The declaring type 'TargetClass2ForMixinAddingBidirectionalRelationTwice' does not match the type of the opposite relation "
        + "propery 'VirtualSide' declared on type 'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors." 
        + "RelationTargetForMixinAddingBidirectionalRelationTwice'.\r\nDeclaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping." 
        + "TestDomain.Errors.MixinAddingBidirectionalRelationTwice, property: RealSide")]
    public void GetMetadata_WithInvalidOppositePropertyType_CausedByMixin ()
    {
      Type type = GetTargetClass2ForMixinAddingBidirectionalRelationTwice ();
      Type mixinType = GetMixinAddingBidirectionalRelationTwice();
      PropertyInfo propertyInfo = mixinType.GetProperty ("RealSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type, mixinType);

      PropertyReflector propertyReflector = new PropertyReflector (classDefinition, propertyInfo);
      classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata ());

      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetRelationTargetForMixinAddingBidirectionalRelationTwice ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
      Assert.Fail("Expected exception");
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSideNotInMapping' cannot be assigned to the type of the opposite relation propery 'BaseInvalidOppositePropertyTypeRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSideNotInMapping, "
        + "property: BaseInvalidOppositePropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeOnBaseClassNotInMapping ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("BaseInvalidOppositePropertyTypeLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSide' does not match the type of the opposite relation propery 'InvalidOppositeCollectionPropertyTypeRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: InvalidOppositeCollectionPropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeForCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("InvalidOppositeCollectionPropertyTypeLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
       "The declaring type 'ClassWithInvalidBidirectionalRelationLeftSideNotInMapping' cannot be assigned to the type of the opposite relation propery 'BaseInvalidOppositeCollectionPropertyTypeRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSideNotInMapping, "
        + "property: BaseInvalidOppositeCollectionPropertyTypeLeftSide")]
    public void GetMetadata_WithInvalidOppositePropertyTypeForCollectionPropertyOnBaseClassNotInMapping ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide ();
      PropertyInfo propertyInfo = type.GetProperty ("BaseInvalidOppositeCollectionPropertyTypeLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "Opposite relation property 'MissingBidirectionalRelationAttributeForCollectionPropertyRightSide' declared on type "
        + "'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide' "
        + "does not define a matching 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute'.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public void GetMetadata_WithMissingBidirectionalRelationAttributeOnOppositeCollectionProperty ()
    {
      Type type = GetClassWithInvalidBidirectionalRelationLeftSide();
      PropertyInfo propertyInfo = type.GetProperty ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage =
        "A bidirectional relation can only have one virtual relation end point.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NoContainsKeyLeftSide")]
    public void GetMetadata_WithTwoVirtualEndPoints ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
      PropertyInfo propertyInfo = type.GetProperty ("NoContainsKeyLeftSide");
      ReflectionBasedClassDefinition classDefinition = CreateReflectionBasedClassDefinition (type);
      RelationReflector relationReflector = new RelationReflector (classDefinition, propertyInfo);
      _classDefinitions.Add (classDefinition);
      _classDefinitions.Add (CreateReflectionBasedClassDefinition (GetClassWithInvalidBidirectionalRelationRightSide ()));

      relationReflector.GetMetadata (_classDefinitions, _relationDefinitions);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "The classDefinition's class type 'Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.BaseClass' is not assignable "
        + "to the property's declaring type.\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void Initialize_WithPropertyInfoNotAssignableToTheClassDefinitionsType ()
    {
      Type classType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.BaseClass", true, false);
      Type declaringType = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute", true, false);
      PropertyInfo propertyInfo = declaringType.GetProperty ("Int32");

      new RelationReflector (CreateReflectionBasedClassDefinition (classType), propertyInfo);
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type, params Type[] persistentMixins)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, persistentMixins);
    }

    private Type GetClassWithInvalidBidirectionalRelationLeftSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
    }

    private Type GetClassWithInvalidBidirectionalRelationRightSide ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationRightSide", true, false);
    }

    private Type GetRelationTargetForMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.RelationTargetForMixinAddingBidirectionalRelationTwice", true, false);
    }

    private Type GetTargetClass1ForMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.TargetClass1ForMixinAddingBidirectionalRelationTwice", true, false);
    }

    private Type GetTargetClass2ForMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.TargetClass2ForMixinAddingBidirectionalRelationTwice", true, false);
    }

    private Type GetMixinAddingBidirectionalRelationTwice ()
    {
      return TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.MixinAddingBidirectionalRelationTwice", true, false);
    }
  }
}