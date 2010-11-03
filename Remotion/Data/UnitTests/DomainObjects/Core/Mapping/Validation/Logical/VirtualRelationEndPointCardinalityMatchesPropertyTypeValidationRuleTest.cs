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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule _validationRule;
    private ClassDefinition _classDefinition;
    private RelationDefinition _relationDefinition;
    private VirtualRelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule();
      _classDefinition = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _relationDefinition =
         FakeMappingConfiguration.Current.RelationDefinitions[
             "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"];
      _endPoint1 = (VirtualRelationEndPointDefinition) _relationDefinition.EndPointDefinitions[0];
      _endPoint2 = (RelationEndPointDefinition) _relationDefinition.EndPointDefinitions[1];
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, _endPoint2 });
    }

    [Test]
    public void NoVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeNotDerivedFromDomainObject ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", typeof (DomainObject));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "The property type of a virtual end point of a one-to-one relation must be derived from 'Remotion.Data.DomainObjects.DomainObject'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeDerivedFromDomainObject ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDerivedFromDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (ObjectList<BaseOfBaseValidationDomainObjectClass>), null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsNotDerivedFromDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", typeof (BaseOfBaseValidationDomainObjectClass));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "The property type of a virtual end point of a one-to-many relation must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void OnePropertyEndPointTypeWhichIsNotDerivedFromDomainObjectCollection_And_OnePropertyEndPointTypeWhicIsNotDerivedFromDomainObject ()
    {
      var endPointDefinition1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);
      var endPointDefinition2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition1, "_propertyType", typeof (BaseOfBaseValidationDomainObjectClass));
      PrivateInvoke.SetNonPublicField (endPointDefinition2, "_propertyType", typeof (DomainObject));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition1, endPointDefinition2 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "The property type of a virtual end point of a one-to-many relation must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'."
        +"\r\nThe property type of a virtual end point of a one-to-one relation must be derived from 'Remotion.Data.DomainObjects.DomainObject'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);

    }
  }
}