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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Persistence.SortExpressionIsSupportedForCardinalityOfRelationPropertyValidationRule;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Persistence
{
  [TestFixture]
  public class SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRuleTest : ValidationRuleTestBase
  {
    private SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule();
      var type = typeof (SortExpressionIsSupportedTestClass);
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
    public void CardinalityIsMany ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CardinalityIsOne_And_EndPointDefinitionHasNoSortExpression ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
        _classDefinition, "Property", false, CardinalityType.One, typeof (SortExpressionIsSupportedTestClass), null);

      var validationResult = _validationRule.Validate (endPointDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CardinalityOne_And_EndPointDefinitionHasSortExpression ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
        _classDefinition, "Property", false, CardinalityType.One, typeof (SortExpressionIsSupportedTestClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_sortExpression", "SortExpression");

      var validationResult = _validationRule.Validate (endPointDefinition);

      var expectedMessage = "Property 'Property' of class 'SortExpressionIsSupportedTestClass' must not specify a SortExpression, because cardinality is equal to 'one'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}