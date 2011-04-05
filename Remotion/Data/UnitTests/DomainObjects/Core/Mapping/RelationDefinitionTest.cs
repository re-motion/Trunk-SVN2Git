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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _orderClass;
    private ClassDefinition _customerClass;
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;
    private RelationDefinition _customerToOrder;

    public override void SetUp ()
    {
      base.SetUp();

      _customerClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Customer)];
      _orderClass = FakeMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      _customerToOrder =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.Customer->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Customer.Orders"];
      _customerEndPoint = (VirtualRelationEndPointDefinition) _customerToOrder.EndPointDefinitions[0];
      _orderEndPoint = (RelationEndPointDefinition) _customerToOrder.EndPointDefinitions[1];
    }

    [Test]
    public void EndPointDefinitionsAreReturnedAsReadOnlyCollection ()
    {
      RelationDefinition relation = new RelationDefinition ("RelationID", _customerEndPoint, _orderEndPoint);

      Assert.That (relation.EndPointDefinitions, Is.TypeOf (typeof (ReadOnlyCollection<IRelationEndPointDefinition>)));
      Assert.That (relation.EndPointDefinitions.Count, Is.EqualTo (2));
      Assert.That (relation.EndPointDefinitions[0], Is.SameAs (_customerEndPoint));
      Assert.That (relation.EndPointDefinitions[1], Is.SameAs (_orderEndPoint));
    }

    [Test]
    public void GetToString ()
    {
      RelationDefinition relation = new RelationDefinition ("RelationID", _customerEndPoint, _orderEndPoint);

      Assert.That (relation.ToString(), Is.EqualTo (typeof (RelationDefinition).FullName + ": RelationID"));
    }

    [Test]
    public void IsEndPoint ()
    {
      RelationDefinition relation = new RelationDefinition ("myRelation", _customerEndPoint, _orderEndPoint);

      Assert.IsTrue (relation.IsEndPoint ("Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"));
      Assert.IsTrue (relation.IsEndPoint ("Customer", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));
      Assert.IsFalse (relation.IsEndPoint ("Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));
      Assert.IsFalse (relation.IsEndPoint ("Customer", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"));
    }

    [Test]
    public void GetEndPointDefinition ()
    {
      Assert.AreSame (
          _orderEndPoint,
          _customerToOrder.GetEndPointDefinition ("Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"));
      Assert.AreSame (
          _customerEndPoint,
          _customerToOrder.GetEndPointDefinition (
              "Customer", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.AreSame (
          _customerEndPoint,
          _customerToOrder.GetOppositeEndPointDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"));
      Assert.AreSame (
          _orderEndPoint,
          _customerToOrder.GetOppositeEndPointDefinition (
              "Customer", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));
    }

    [Test]
    public void GetEndPointDefinitionForUndefinedProperty ()
    {
      Assert.IsNull (
          _customerToOrder.GetEndPointDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedProperty ()
    {
      Assert.IsNull (
          _customerToOrder.GetOppositeEndPointDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber"));
    }

    [Test]
    public void GetEndPointDefinitionForUndefinedClass ()
    {
      Assert.IsNull (
          _customerToOrder.GetEndPointDefinition (
              "OrderTicket", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Customer"));
    }

    [Test]
    public void GetOppositeEndPointDefinitionForUndefinedClass ()
    {
      Assert.IsNull (
          _customerToOrder.GetOppositeEndPointDefinition (
              "OrderTicket", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Customer"));
    }

    [Test]
    public void GetOppositeClassDefinition ()
    {
      Assert.AreSame (
          _customerClass,
          _customerToOrder.GetOppositeClassDefinition (
              "Order", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"));
      Assert.AreSame (
          _orderClass,
          _customerToOrder.GetOppositeClassDefinition (
              "Customer", "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
        + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems' has no association with class 'Customer' "
        + "and property 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders'.")]
    public void GetMandatoryOppositeRelationEndPointDefinitionWithNotAssociatedRelationDefinitionID ()
    {
      RelationDefinition orderToOrderItem =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"];

      IRelationEndPointDefinition wrongEndPointDefinition =
          orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (
              _customerClass.GetMandatoryRelationEndPointDefinition (
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders"));

      orderToOrderItem.GetMandatoryOppositeRelationEndPointDefinition (wrongEndPointDefinition);
    }

    [Test]
    public void Contains ()
    {
      RelationDefinition relationDefinition =
          MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"];

      Assert.IsFalse (
          relationDefinition.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core."
                  + "Mapping.TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"]
                  .EndPointDefinitions[0]));
      Assert.IsFalse (
          relationDefinition.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core."
                  + "Mapping.TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core."
                  + "Mapping.TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[1]));

      Assert.IsTrue (
          relationDefinition.Contains (
              MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[0]));
      Assert.IsTrue (
          relationDefinition.Contains (
              MappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
              + "TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[1]));
    }

    [Test]
    public void RelationType_OneToOne ()
    {
      RelationDefinition relationDefinition =
          MappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
          + "TestDomain.Integration.Computer.Employee->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Computer"];
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.OneToOne));
    }

    [Test]
    public void RelationType_OneMany ()
    {
      RelationDefinition relationDefinition =
          MappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
          + "TestDomain.Integration.OrderItem.Order->Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"];
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.OneToMany));
    }

    [Test]
    public void RelationType_Unidirectional ()
    {
      RelationDefinition relationDefinition =
          MappingConfiguration.Current.RelationDefinitions[
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location:Remotion.Data.UnitTests.DomainObjects.Core.Mapping."
          +"TestDomain.Integration.Location.Client"];
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.Unidirectional));
    }
  }
}