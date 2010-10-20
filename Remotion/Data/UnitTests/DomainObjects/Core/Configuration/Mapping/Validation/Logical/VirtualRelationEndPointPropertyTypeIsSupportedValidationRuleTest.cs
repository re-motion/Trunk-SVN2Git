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
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private VirtualRelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule();
      var type = typeof (BaseOfBaseValidationDomainObjectClass);
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
    public void PropertyTypIsDomainObjectCollection ()
    {
      var propertyType = typeof (DomainObjectCollection);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObjectCollection ()
    {
      var propertyType = typeof (ObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObject ()
    {
      var propertyType = typeof (BaseOfBaseValidationDomainObjectClass);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, propertyType, null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeNoDomainObjectCellection_And_NotDerivedFromDomainObjectCollectionOrDomainObject ()
    {
      var propertyType = typeof (string);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", propertyType);

      var validationResult = _validationRule.Validate (endPointDefinition);

      var expectedMessage = "Relation definition error: Virtual property 'Property' of class 'BaseOfBaseValidationDomainObjectClass' is "
                           +"of type 'System.String', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                           +"'Remotion.Data.DomainObjects.DomainObjectCollection' or must be 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}