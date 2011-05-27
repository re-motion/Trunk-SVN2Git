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
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class VirtualRelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private class DerivedObjectList<T> : ObjectList<T> where T : DomainObject
    {
    }

    private VirtualRelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new VirtualRelationEndPointPropertyTypeIsSupportedValidationRule();
      _classDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
    }

    [Test]
    public void NoVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypIsObjectList()
    {
      var propertyType = typeof (ObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsDerivedFromObjectList ()
    {
      var propertyType = typeof (DerivedObjectList<BaseOfBaseValidationDomainObjectClass>);
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, propertyType, null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsDomainObject ()
    {
      var propertyType = typeof (DomainObject);
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, propertyType, null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyTypeIsSubclassOfDomainObject ()
    {
      var propertyType = typeof (BaseOfBaseValidationDomainObjectClass);
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.One, propertyType, null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void LeftEndpointPropertyType_NotAssignableToObjectListOrDomainObject ()
    {
      var leftEndPointDefinition = CreateVirtualRelationEndPointDefinition ("Left", CardinalityType.One, typeof (string));
      var rightEndPointDefinition = CreateVirtualRelationEndPointDefinition ("Right", CardinalityType.One, typeof (DomainObject));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "Virtual property 'Left' of class 'Order' is of type 'String', but must be assignable to 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: Left";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void RightEndpointPropertyType_NotAssignableToObjectListOrDomainObject ()
    {
      var leftEndPointDefinition = CreateVirtualRelationEndPointDefinition ("Left", CardinalityType.Many, typeof (ObjectList<>));
      var rightEndPointDefinition = CreateVirtualRelationEndPointDefinition ("Right", CardinalityType.One, typeof (string));
      var relationDefinition = new RelationDefinition ("Test", leftEndPointDefinition, rightEndPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "Virtual property 'Right' of class 'Order' is of type 'String', but must be assignable to 'DomainObject' or 'ObjectList`1'.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order\r\n"
          + "Property: Right";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (string propertyName, CardinalityType cardinality, Type propertyType)
    {
      return new VirtualRelationEndPointDefinition (
          _classDefinition,
          "Definition." + propertyName,
          false,
          cardinality,
          propertyType,
          null,
          CreatePropertyInformationStub (propertyName));
    }

    private IPropertyInformation CreatePropertyInformationStub (string name)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub.Stub (stub => stub.Name).Return (name);
      propertyInformationStub.Stub (stub => stub.PropertyType).Throw (new NotSupportedException());
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (typeof (Order)));

      return propertyInformationStub;
    }
  }
}