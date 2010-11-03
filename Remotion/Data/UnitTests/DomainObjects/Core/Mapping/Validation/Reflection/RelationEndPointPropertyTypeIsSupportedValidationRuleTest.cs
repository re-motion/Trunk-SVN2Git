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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class RelationEndPointPropertyTypeIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointPropertyTypeIsSupportedValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;
    private RelationDefinition _relationDefinition;
    private VirtualRelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointPropertyTypeIsSupportedValidationRule();
      _classDefinition = new ReflectionBasedClassDefinition (
          "ID",
          "EntityName",
          "SPID",
          typeof (RelationEndPointPropertyClass),
          false,
          null,
          new PersistentMixinFinderMock (typeof (RelationEndPointPropertyClass), new Type[0]));
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
    public void NoReflectionBasedVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NoBidirectionalRelation_PropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithoutBidirectionalAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass).GetProperty ("PropertyWithoutBidirectionalAttribute"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "The property type of an uni-directional relation property must be assignable to Remotion.Data.DomainObjects.DomainObject.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void NoBidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "DomainObjectPropertyWithoutBidirectionalAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass).GetProperty ("DomainObjectPropertyWithoutBidirectionalAttribute"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeDomainObject ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "DomainObjectPropertyWithBidirectionalAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass).GetProperty ("DomainObjectPropertyWithBidirectionalAttribute"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void BidirectionalRelation_PropertyTypeNoDomainObject ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition,
          "PropertyWithBidirectionalAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass).GetProperty ("PropertyWithBidirectionalAttribute"));
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, endPointDefinition });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }
  }
}