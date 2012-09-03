// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class VirtualRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _customerClassDefinition;
    private VirtualRelationEndPointDefinition _customerOrdersEndPoint;

    private ClassDefinition _orderClassDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Customer));
      _customerOrdersEndPoint = VirtualRelationEndPointDefinitionFactory.Create (
          _customerClassDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          typeof (OrderCollection),
          "OrderNumber desc");

      _orderClassDefinition = CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ();
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var endPoint = VirtualRelationEndPointDefinitionFactory.Create(
          _orderClassDefinition,
          "VirtualEndPoint",
          true,
          CardinalityType.One,
          typeof(OrderItem),
          null);

      Assert.AreSame (typeof (OrderItem), endPoint.PropertyInfo.PropertyType);
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
          _customerClassDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          typeof (OrderCollection),
          "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpressionText);
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.IsFalse (_customerOrdersEndPoint.IsAnonymous);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      Assert.IsNull (_customerOrdersEndPoint.RelationDefinition);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      _customerOrdersEndPoint.SetRelationDefinition (new RelationDefinition ("Test", _customerOrdersEndPoint, _customerOrdersEndPoint));
      Assert.IsNotNull (_customerOrdersEndPoint.RelationDefinition);
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var endPoint = VirtualRelationEndPointDefinitionFactory.Create (
          _orderClassDefinition,
          "OrderItems",
          false,
          CardinalityType.Many,
          typeof (ObjectList<OrderItem>),
          null);
      Assert.That (endPoint.SortExpressionText, Is.Null);

      Assert.That (endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("Product asc");

      Assert.That (endPoint.GetSortExpression(), Is.Not.Null);
      Assert.That (
          endPoint.GetSortExpression().ToString(),
          Is.EqualTo ("Product ASC"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "SortExpression 'Product asc asc' cannot be parsed: Expected one or two parts (a property name and an optional identifier), found 3 parts instead.\r\n\r\n"+
        "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order\r\nProperty: OrderItems")]
    public void GetSortExpression_Error ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("Product asc asc");
      Dev.Null = endPoint.GetSortExpression();
    }

    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition employeeClassDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Employee));
      VirtualRelationEndPointDefinition relationEndPointDefinition =
          (VirtualRelationEndPointDefinition) employeeClassDefinition.GetRelationEndPointDefinition (typeof (Employee) + ".Computer");
      Assert.AreEqual (PropertyInfoAdapter.Create(typeof (Employee).GetProperty ("Computer")), relationEndPointDefinition.PropertyInfo);
    }

    private VirtualRelationEndPointDefinition CreateFullVirtualEndPointAndClassDefinition_WithProductProperty (string sortExpressionString)
    {
      var endPoint = VirtualRelationEndPointDefinitionFactory.Create (
          _orderClassDefinition,
          "OrderItems",
          false,
          CardinalityType.Many,
          typeof (ObjectList<OrderItem>),
          sortExpressionString);
      var orderItemClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (OrderItem));
      var oppositeProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (orderItemClassDefinition, "Order");
      var productProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (orderItemClassDefinition, "Product");
      orderItemClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{oppositeProperty, productProperty}, true));
      orderItemClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeEndPoint = new RelationEndPointDefinition (oppositeProperty, false);
      var relationDefinition = new RelationDefinition ("test", endPoint, oppositeEndPoint);
      orderItemClassDefinition.SetReadOnly ();
      endPoint.SetRelationDefinition (relationDefinition);
      return endPoint;
    }

    private static ClassDefinition CreateOrderDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses ("Order", classType: typeof (Order));
    }
  }
}
