// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class VirtualRelationEndPointDefinitionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

    // construction and disposing

    public VirtualRelationEndPointDefinitionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      RelationDefinition customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"];

      _customerEndPoint = (VirtualRelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Customer", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");

      _orderEndPoint = (RelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Order", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      VirtualRelationEndPointDefinition endPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(ClassDefinitionFactory.CreateOrderDefinition (), "VirtualEndPoint", true, CardinalityType.One, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem, Remotion.Data.UnitTests", null);

      Assert.IsTrue (endPoint.IsPropertyTypeResolved);
      Assert.AreSame (typeof (OrderItem), endPoint.PropertyType);
      Assert.AreEqual (typeof (OrderItem).AssemblyQualifiedName, endPoint.PropertyTypeName);
    }

    [Test]
    public void IsNull ()
    {
      Assert.IsNotNull (_customerEndPoint as INullObject);
      Assert.IsFalse (_customerEndPoint.IsNull);
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      VirtualRelationEndPointDefinition definition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(TestMappingConfiguration.Current.ClassDefinitions["Order"], "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      Assert.IsNotNull (_customerEndPoint.RelationDefinition);
    }
  }
}
