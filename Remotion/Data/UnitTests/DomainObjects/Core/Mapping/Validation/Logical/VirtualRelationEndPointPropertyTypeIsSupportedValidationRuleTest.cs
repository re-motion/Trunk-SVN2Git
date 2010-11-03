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
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private VirtualRelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private ClassDefinition _classDefinition;
    private RelationDefinition _relationDefinition;
    private VirtualRelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule();
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
    public void PropertyTypIsDomainObjectCollection ()
    {
      var propertyType = typeof (DomainObjectCollection);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObjectCollection ()
    {
      var propertyType = typeof (ObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObject ()
    {
      var propertyType = typeof (BaseOfBaseValidationDomainObjectClass);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, propertyType, null);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void LeftEndpointPropertyTypeIsNoDomainObjectCellection_And_NotDerivedFromDomainObjectCollectionOrDomainObject ()
    {
      var propertyType = typeof (string);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", propertyType);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint1 });
      
      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "Relation definition error: Virtual property 'Property' of class 'Order' is "
                           +"of type 'System.String', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                           +"'Remotion.Data.DomainObjects.DomainObjectCollection' or must be 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RightEndpointPropertyTypeIsNoDomainObjectCellection_And_NotDerivedFromDomainObjectCollectionOrDomainObject ()
    {
      var propertyType = typeof (string);
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_propertyType", propertyType);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { _endPoint1, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "Relation definition error: Virtual property 'Property' of class 'Order' is "
                           + "of type 'System.String', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                           + "'Remotion.Data.DomainObjects.DomainObjectCollection' or must be 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void BothEndpointPropertyTypesAreNoDomainObjectCellection_And_AreNotDerivedFromDomainObjectCollectionOrDomainObject ()
    {
      var propertyType = typeof (string);
      var endPointDefinition1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      var endPointDefinition2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "PropertyWithStorageClassNone", false, CardinalityType.One, typeof (BaseOfBaseValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition1, "_propertyType", propertyType);
      PrivateInvoke.SetNonPublicField (endPointDefinition2, "_propertyType", propertyType);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition1, endPointDefinition2 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "Relation definition error: Virtual property 'Property' of class 'Order' is "
                           + "of type 'System.String', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                           + "'Remotion.Data.DomainObjects.DomainObjectCollection' or must be 'Remotion.Data.DomainObjects.DomainObjectCollection'.\r\n"
                           + "Relation definition error: Virtual property 'PropertyWithStorageClassNone' of class 'Order' is "
                           + "of type 'System.String', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                           + "'Remotion.Data.DomainObjects.DomainObjectCollection' or must be 'Remotion.Data.DomainObjects.DomainObjectCollection'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}