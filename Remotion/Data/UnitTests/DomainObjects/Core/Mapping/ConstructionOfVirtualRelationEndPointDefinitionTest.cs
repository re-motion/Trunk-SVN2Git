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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ConstructionOfVirtualRelationEndPointDefinitionTest : MappingReflectionTestBase
  {

    //TODO 3421: move test to VirtualRelationEndPointDefinitionTest and remove this class
    [Test]
    public void InitializeWithSortExpression ()
    {
      ReflectionBasedClassDefinition customerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Property 'Orders' of class 'Customer' must not specify a SortExpression, because cardinality is equal to 'one'.")]
    public void InitializeWithSortExpressionAndCardinalityOfOne ()
    {
      ReflectionBasedClassDefinition customerDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer), false);

      VirtualRelationEndPointDefinition endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(customerDefinition, "Orders", false, CardinalityType.One, typeof (Order), "OrderNumber desc");
    }
  }
}
