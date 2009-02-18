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
  public class RelationEndPointDefinitionTest : StandardMappingTest
  {
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;

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
    public void InitializeWithResolvedPropertyType ()
    {
      RelationEndPointDefinition endPoint = new RelationEndPointDefinition (
          ClassDefinitionFactory.CreateOrderDefinitionWithResolvedCustomerProperty (), 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", 
          true);

      Assert.IsTrue (endPoint.IsPropertyTypeResolved);
      Assert.AreSame (typeof (ObjectID), endPoint.PropertyType);
      Assert.AreEqual (typeof (ObjectID).AssemblyQualifiedName, endPoint.PropertyTypeName);
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.IsFalse (_orderEndPoint.IsAnonymous);
    }

    [Test]
    public void CorrespondsToForVirtualEndPoint ()
    {
      Assert.IsTrue (_customerEndPoint.CorrespondsTo ("Customer", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("Customer", "NonExistingProperty"));
      Assert.IsFalse (_customerEndPoint.CorrespondsTo ("OrderTicket", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders"));
    }

    [Test]
    public void CorrespondsTo ()
    {
      Assert.IsTrue (_orderEndPoint.CorrespondsTo ("Order", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Order", "NonExistingProperty"));
      Assert.IsFalse (_orderEndPoint.CorrespondsTo ("Partner", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      RelationEndPointDefinition definition = new RelationEndPointDefinition (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)], 
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", 
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
