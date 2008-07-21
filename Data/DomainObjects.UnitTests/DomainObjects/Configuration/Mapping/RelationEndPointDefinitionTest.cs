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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionTest : StandardMappingTest
  {
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

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
    public void InitializeWithResolvedPropertyType ()
    {
      RelationEndPointDefinition endPoint = new RelationEndPointDefinition (
          ClassDefinitionFactory.CreateOrderDefinitionWithResolvedCustomerProperty (), 
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", 
          true);

      Assert.IsTrue (endPoint.IsPropertyTypeResolved);
      Assert.AreSame (typeof (ObjectID), endPoint.PropertyType);
      Assert.AreEqual (typeof (ObjectID).AssemblyQualifiedName, endPoint.PropertyTypeName);
    }

    [Test]
    public void IsNull ()
    {
      Assert.IsNotNull (_orderEndPoint as INullObject);
      Assert.IsFalse (_orderEndPoint.IsNull);
    }

    [Test]
    public void CorrespondsToForVirtualEndPoint ()
    {
      Assert.IsTrue (_customerEndPoint.CorrespondsTo ("Customer", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("Customer", "NonExistingProperty"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("OrderTicket", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));
    }

    [Test]
    public void CorrespondsTo ()
    {
      Assert.IsTrue (_orderEndPoint.CorrespondsTo ("Order", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Order", "NonExistingProperty"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Partner", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      RelationEndPointDefinition definition = new RelationEndPointDefinition (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)], 
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", 
          true);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      Assert.IsNotNull (_orderEndPoint.RelationDefinition);
    }
  }
}
