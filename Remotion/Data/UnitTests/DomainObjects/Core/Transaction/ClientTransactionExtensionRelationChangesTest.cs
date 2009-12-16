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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.MockConstraints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionRelationChangesTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private OrderTicket _orderTicket1;
    private Location _location1;
    private Client _client1;

    private DomainObjectMockEventReceiver _order1EventReceiver;
    private DomainObjectMockEventReceiver _orderTicket1EventReceiver;
    private DomainObjectMockEventReceiver _location1EventReceiver;
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderTicket1 = _order1.OrderTicket;
      _location1 = Location.GetObject (DomainObjectIDs.Location1);
      _client1 = _location1.Client;

      _mockRepository = new MockRepository();

      _extension = _mockRepository.StrictMock<IClientTransactionExtension>();
      _order1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      _orderTicket1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_orderTicket1);
      _location1EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_location1);
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_client1); // no events must be signalled for _client1

      ClientTransactionScope.CurrentTransaction.Extensions.Add ("Name", _extension);
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _order1.OrderTicket = _orderTicket1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithNewNull ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock,
            _order1,
            "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket",
            _orderTicket1,
            null);
        _extension.RelationChanging (
            ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);

        _order1EventReceiver.RelationChanging (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderTicket1, null);

        _orderTicket1EventReceiver.RelationChanging (
            _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);

        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      }

      _mockRepository.ReplayAll();

      _order1.OrderTicket = null;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithOldNull ()
    {
      Order order = Order.NewObject();
      OrderTicket orderTicket = OrderTicket.NewObject();

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, order, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", null, orderTicket);
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", null, order);

        var orderEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (order);
        var orderTicketEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);

        orderEventReceiver.RelationChanging (order, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", null, orderTicket);

        orderTicketEventReceiver.RelationChanging (orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", null, order);

        _extension.RelationChanged (ClientTransactionMock, order, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        _extension.RelationChanged (ClientTransactionMock, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        orderEventReceiver.RelationChanged (order, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        orderTicketEventReceiver.RelationChanged (orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      }

      _mockRepository.ReplayAll();

      order.OrderTicket = orderTicket;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;

      var orderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket3, null);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderTicket1, orderTicket3);
        _extension.RelationChanging (
            ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);

        orderTicket3EventReceiver.RelationChanging (
            orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", oldOrderOfOrderTicket3, _order1);

        oldOrderOfOrderTicket3EventReceiver.RelationChanging (
            oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket3, null);

        _order1EventReceiver.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderTicket1, orderTicket3);

        _orderTicket1EventReceiver.RelationChanging (
            _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);

        _extension.RelationChanged (ClientTransactionMock, orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        orderTicket3EventReceiver.RelationChanged (orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        oldOrderOfOrderTicket3EventReceiver.RelationChanged (
            oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
      }

      _mockRepository.ReplayAll();

      orderTicket3.Order = _order1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _orderTicket1.Order = _order1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;

      var orderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket3);
      var oldOrderOfOrderTicket3EventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderTicket1, orderTicket3);
        _extension.RelationChanging (
            ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket3, null);

        _order1EventReceiver.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderTicket1, orderTicket3);

        _orderTicket1EventReceiver.RelationChanging (
            _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);

        orderTicket3EventReceiver.RelationChanging (
            orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", oldOrderOfOrderTicket3, _order1);

        oldOrderOfOrderTicket3EventReceiver.RelationChanging (
            oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket3, null);

        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        _extension.RelationChanged (ClientTransactionMock, orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        orderTicket3EventReceiver.RelationChanged (orderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");

        oldOrderOfOrderTicket3EventReceiver.RelationChanged (
            oldOrderOfOrderTicket3, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }

      _mockRepository.ReplayAll();

      _order1.OrderTicket = orderTicket3;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      _location1.Client = _client1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithNewNull ()
    {
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, _location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", _client1, null);

        _location1EventReceiver.RelationChanging (_location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", _client1, null);

        _extension.RelationChanged (ClientTransactionMock, _location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");

        _location1EventReceiver.RelationChanged (_location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      }

      _mockRepository.ReplayAll();

      _location1.Client = null;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithOldNull ()
    {
      Location newLocation = Location.NewObject();

      var newLocationEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newLocation);

      _mockRepository.BackToRecord (_extension);
      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, newLocation, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", null, _client1);

        newLocationEventReceiver.RelationChanging (newLocation, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", null, _client1);

        _extension.RelationChanged (ClientTransactionMock, newLocation, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");

        newLocationEventReceiver.RelationChanged (newLocation, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      }

      _mockRepository.ReplayAll();

      newLocation.Client = _client1;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnidirectionalRelationWithOldRelatedObject ()
    {
      Client newClient = Client.NewObject();
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newClient); // no events for newClient

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered())
      {
        _extension.RelationChanging (
            ClientTransactionMock, _location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", _client1, newClient);

        _location1EventReceiver.RelationChanging (_location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", _client1, newClient);

        _extension.RelationChanged (ClientTransactionMock, _location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");

        _location1EventReceiver.RelationChanged (_location1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      }

      _mockRepository.ReplayAll();

      _location1.Client = newClient;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RemoveFromOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;

      Assert.Greater (preloadedOrderItems.Count, 0);
      var orderItem = (OrderItem) preloadedOrderItems[0];

      _mockRepository.BackToRecord (_extension);
      var orderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyCollection<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionMock),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", orderItem, null);

        orderItemEventReceiver.RelationChanging (orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);

        _order1EventReceiver.RelationChanging (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", orderItem, null);

        _extension.RelationChanged (ClientTransactionMock, orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        orderItemEventReceiver.RelationChanged (orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Remove (orderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void AddToOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem orderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord (_extension);
      var orderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyCollection<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionMock),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, orderItem);

        orderItemEventReceiver.RelationChanging (orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1);

        _order1EventReceiver.RelationChanging (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, orderItem);

        _extension.RelationChanged (ClientTransactionMock, orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        orderItemEventReceiver.RelationChanged (orderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Add (orderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void AddToOneToManyRelationWithOldRelatedObject ()
    {
      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      Order oldOrderOfNewOrderItem = newOrderItem.Order;

      _mockRepository.BackToRecord (_extension);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyCollection<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionMock),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, newOrderItem);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", newOrderItem, null);

        newOrderItemEventReceiver.RelationChanging (
            newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (
            oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", newOrderItem, null);

        _extension.RelationChanged (ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (
            oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems.Add (newOrderItem);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithSameObject ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      OrderItem oldOrderItem = _order1.OrderItems[0];

      _mockRepository.BackToRecord (_extension);

      // no calls on the extension are expected

      _mockRepository.ReplayAll();

      orderItems[0] = oldOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.NewObject();

      _mockRepository.BackToRecord (_extension);
      var oldOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderItem);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyCollection<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionMock),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oldOrderItem, newOrderItem);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1);

        _order1EventReceiver.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oldOrderItem, newOrderItem);

        _extension.RelationChanged (ClientTransactionMock, oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithOldRelatedObject ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      Dev.Null = oldOrderOfNewOrderItem.OrderItems; // preload

      _mockRepository.BackToRecord (_extension);
      var oldOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderItem);
      var newOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (newOrderItem);
      var oldOrderOfNewOrderItemEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered())
      {
        _extension.RelationReading (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyCollection<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionMock),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oldOrderItem, newOrderItem);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", newOrderItem, null);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);

        newOrderItemEventReceiver.RelationChanging (
            newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oldOrderItem, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (
            oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", newOrderItem, null);

        _extension.RelationChanged (ClientTransactionMock, oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
        _extension.RelationChanged (ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");

        _order1EventReceiver.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (
            oldOrderOfNewOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.ReplayAll();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ReplaceWholeCollectionInOneToManyRelation ()
    {
      var oldCollection = _order1.OrderItems;
      var removedOrderItem = oldCollection[0];
      var stayingOrderItem = oldCollection[1];
      var addedOrderItem = OrderItem.NewObject ();

      var newCollection = new ObjectList<OrderItem> { stayingOrderItem, addedOrderItem };

      _mockRepository.BackToRecord (_extension);
      var removedOrderItemEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (removedOrderItem);
      _mockRepository.StrictMock<DomainObjectMockEventReceiver> (stayingOrderItem);
      var addedOrderItemEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (addedOrderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, removedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, addedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", removedOrderItem, null));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, addedOrderItem));

        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (
            removedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null));
        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (
            addedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", null, _order1));
        _order1EventReceiver.Expect (mock => mock.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", removedOrderItem, null));
        _order1EventReceiver.Expect (mock => mock.RelationChanging (
            _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", null, addedOrderItem));

        _extension.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, removedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"));
        _extension.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, addedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"));
        _extension.Expect (mock => mock.RelationChanged (
            ClientTransactionMock, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));

        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (
            removedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"));
        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (
            addedOrderItem, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"));
        _order1EventReceiver.Expect (mock => mock.RelationChanged (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems = newCollection;

      _mockRepository.VerifyAll ();
    }
  }
}
