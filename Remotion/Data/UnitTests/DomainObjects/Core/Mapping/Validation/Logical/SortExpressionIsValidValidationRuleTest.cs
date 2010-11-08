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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class SortExpressionIsValidValidationRuleTest : ValidationRuleTestBase
  {
    private SortExpressionIsValidValidationRule _validationRule;
    private RelationDefinition _relationDefinition;
    private VirtualRelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new SortExpressionIsValidValidationRule();
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
    public void ValidSortExpressionWithSortingDirection ()
    {
       var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition, "Orders", false, CardinalityType.Many, typeof (Order), "OrderNumber desc", typeof (Customer).GetProperty ("Orders"));
      endPointDefinition.SetRelationDefinition (_relationDefinition);
       PrivateInvoke.SetNonPublicField (
           _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint2 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ValidSortExpressionWithoutSortingDirection ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Orders", false, CardinalityType.Many, typeof (Order), "OrderNumber", typeof (Customer).GetProperty ("Orders"));
      endPointDefinition.SetRelationDefinition (_relationDefinition);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint2 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidSortExpression ()
    {
      var endPointDefinition = new ReflectionBasedVirtualRelationEndPointDefinition (
         _classDefinition, "Orders", false, CardinalityType.Many, typeof (Order), "Test", typeof (Customer).GetProperty ("Orders"));
      endPointDefinition.SetRelationDefinition (_relationDefinition);
      PrivateInvoke.SetNonPublicField (
          _relationDefinition, "_endPointDefinitions", new IRelationEndPointDefinition[] { endPointDefinition, _endPoint2 });

      var validationResult = _validationRule.Validate (_relationDefinition);

      var expectedMessage = "SortExpression 'Test' cannot be parsed: 'Test' is not a valid mapped property name. Expected the .NET property name of a "+
        "property declared by the 'Order' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin or "
        +"a derived class of 'Order', the full unique re-store property identifier can be specified.\r\n\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order'\r\nProperty: 'Orders'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}