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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class RelationDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _customerClass;
    private VirtualRelationEndPointDefinition _customerEndPoint;
    private RelationEndPointDefinition _orderEndPoint;
    private RelationDefinition _customerToOrder;

    public override void SetUp ()
    {
      base.SetUp();

      _customerClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Customer)];
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
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order").RelationDefinition;

      Assert.IsFalse (
          relationDefinition.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
                  + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
                  + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[0]));
      Assert.IsFalse (
          relationDefinition.Contains (
              FakeMappingConfiguration.Current.RelationDefinitions[
                  "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:"
                  + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
                  + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems"].EndPointDefinitions[1]));

      Assert.IsTrue (relationDefinition.Contains (relationDefinition.EndPointDefinitions[0]));
      Assert.IsTrue (relationDefinition.Contains (relationDefinition.EndPointDefinitions[1]));
    }

    [Test]
    public void RelationType_OneToOne ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Computer));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition (typeof (Computer).FullName + ".Employee").RelationDefinition;
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.OneToOne));
    }

    [Test]
    public void RelationType_OneMany ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order").RelationDefinition;
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.OneToMany));
    }

    [Test]
    public void RelationType_Unidirectional ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Location));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition (typeof (Location).FullName + ".Client").RelationDefinition;
      Assert.That (relationDefinition.RelationKind, Is.EqualTo (RelationKindType.Unidirectional));
    }
  }
}