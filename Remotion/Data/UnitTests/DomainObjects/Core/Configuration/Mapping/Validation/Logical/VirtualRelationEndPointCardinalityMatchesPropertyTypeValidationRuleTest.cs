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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Logical.VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRuleTest : ValidationRuleTestBase
  {
    private VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule();
      var type = typeof (CardinalityMatchesPropertyTypeCheckClass);
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
    }

    [Test]
    public void NoVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeNotDerivedFromDomainObject ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.One, typeof (CardinalityMatchesPropertyTypeCheckClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", typeof (DomainObject));

      var validationResult = _validationRule.Validate (endPointDefinition);

      var expectedMessage = "The property type of a virtual end point of a one-to-one relation must be derived from 'Remotion.Data.DomainObjects.DomainObject'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityOne_And_PropertyTypeDerivedFromDomainObject ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.One, typeof (CardinalityMatchesPropertyTypeCheckClass), null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsDerivedFromDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (ObjectList<CardinalityMatchesPropertyTypeCheckClass>), null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithCardinalityMany_And_PropertyTypeIsNotDerivedFromDomainObjectCollection ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", typeof (CardinalityMatchesPropertyTypeCheckClass));

      var validationResult = _validationRule.Validate (endPointDefinition);

      var expectedMessage = "The property type of a virtual end point of a one-to-many relation must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }


  }
}