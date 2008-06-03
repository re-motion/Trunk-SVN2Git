/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
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

      RelationDefinition customerToOrder = TestMappingConfiguration.Current.RelationDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];

      _customerEndPoint = (VirtualRelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Customer", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      _orderEndPoint = (RelationEndPointDefinition) customerToOrder.GetEndPointDefinition (
          "Order", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      VirtualRelationEndPointDefinition endPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(ClassDefinitionFactory.CreateOrderDefinition (), "VirtualEndPoint", true, CardinalityType.One, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem, Remotion.Data.DomainObjects.UnitTests", null);

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
