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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class RelationEndPointCombinationIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointCombinationIsSupportedValidationRule _validationRule;

    private ClassDefinition _orderClass;
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;
    private RelationDefinition _customerToOrder;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointCombinationIsSupportedValidationRule();

      _orderClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"];
      _customerEndPoint = (VirtualRelationEndPointDefinition) _customerToOrder.EndPointDefinitions[0];
      _orderEndPoint = (RelationEndPointDefinition) _customerToOrder.EndPointDefinitions[1];
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicField (
          _customerToOrder, "_endPointDefinitions", new IRelationEndPointDefinition[] { _customerEndPoint, _orderEndPoint });
    }

    [Test]
    public void ValidRelationDefinition ()
    {
      var mappingValidationResult = _validationRule.Validate (_customerToOrder);

      AssertMappingValidationResult (mappingValidationResult, true, null);
    }

    [Test]
    public void TwoAnonymousRelationEndPoints ()
    {
      var anonymousEndPointDefinition = new AnonymousRelationEndPointDefinition (_orderClass);
      PrivateInvoke.SetNonPublicField (
          _customerToOrder, "_endPointDefinitions", new IRelationEndPointDefinition[] { anonymousEndPointDefinition, anonymousEndPointDefinition });

      var mappingValidationResult = _validationRule.Validate (_customerToOrder);

      var expectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order"
        +"->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer' cannot have two anonymous end points.";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoVirtualRelationEndPoints ()
    {
      PrivateInvoke.SetNonPublicField (
          _customerToOrder, "_endPointDefinitions", new IRelationEndPointDefinition[] { _customerEndPoint, _customerEndPoint });

      var mappingValidationResult = _validationRule.Validate (_customerToOrder);

      var expectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order"
        +"->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer' cannot have two virtual end points.";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }

    [Test]
    public void TwoNonVirtualRelationEndPoints ()
    {
      PrivateInvoke.SetNonPublicField (
          _customerToOrder, "_endPointDefinitions", new IRelationEndPointDefinition[] { _orderEndPoint, _orderEndPoint });

      var mappingValidationResult = _validationRule.Validate (_customerToOrder);

      var expectedMessage = "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order"
        +"->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer' cannot have two non-virtual end points.";
      AssertMappingValidationResult (mappingValidationResult, false, expectedMessage);
    }
  }
}