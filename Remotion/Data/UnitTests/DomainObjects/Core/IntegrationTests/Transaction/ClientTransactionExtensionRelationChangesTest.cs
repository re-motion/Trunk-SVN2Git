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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.MockConstraints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
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
            ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock,
            _order1,
            _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition,
            _orderTicket1,
            null);

        _orderTicket1EventReceiver.RelationChanging (
            _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _order1EventReceiver.RelationChanging (_order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, _orderTicket1, null);

        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, order, order.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, null, orderTicket);
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket, orderTicket.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, null, order);

        var orderEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (order);
        var orderTicketEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);

        orderEventReceiver.RelationChanging (order, order.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, null, orderTicket);

        orderTicketEventReceiver.RelationChanging (orderTicket, orderTicket.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, null, order);

        orderTicketEventReceiver.RelationChanged (orderTicket, orderTicket.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);

        orderEventReceiver.RelationChanged (order, order.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, orderTicket, orderTicket.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, order, order.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, orderTicket3, null);
        _extension.RelationChanging (
            ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, _orderTicket1, orderTicket3);

        oldOrderOfOrderTicket3EventReceiver.RelationChanging (oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, orderTicket3, null);
        _orderTicket1EventReceiver.RelationChanging (_orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        orderTicket3EventReceiver.RelationChanging (orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfOrderTicket3, _order1);
        _order1EventReceiver.RelationChanging (_order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, _orderTicket1, orderTicket3);

        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        orderTicket3EventReceiver.RelationChanged (orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        oldOrderOfOrderTicket3EventReceiver.RelationChanged (oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, orderTicket3, null);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, _orderTicket1, orderTicket3);
        _extension.RelationChanging (
            ClientTransactionMock, orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfOrderTicket3, _order1);

        _orderTicket1EventReceiver.RelationChanging (
            _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        oldOrderOfOrderTicket3EventReceiver.RelationChanging (
            oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, orderTicket3, null);
        _order1EventReceiver.RelationChanging (
            _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, _orderTicket1, orderTicket3);
        orderTicket3EventReceiver.RelationChanging (
            orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfOrderTicket3, _order1);


        orderTicket3EventReceiver.RelationChanged (orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        oldOrderOfOrderTicket3EventReceiver.RelationChanged (oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, orderTicket3, orderTicket3.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfOrderTicket3, oldOrderOfOrderTicket3.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _orderTicket1, _orderTicket1.Properties[typeof (OrderTicket), "Order"].PropertyData.RelationEndPointDefinition);
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
        _extension.RelationChanging (ClientTransactionMock, _location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, _client1, null);

        _location1EventReceiver.RelationChanging (_location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, _client1, null);

        _location1EventReceiver.RelationChanged (_location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);
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
        _extension.RelationChanging (ClientTransactionMock, newLocation, newLocation.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, null, _client1);

        newLocationEventReceiver.RelationChanging (newLocation, newLocation.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, null, _client1);

        newLocationEventReceiver.RelationChanged (newLocation, newLocation.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, newLocation, newLocation.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, _client1, newClient);

        _location1EventReceiver.RelationChanging (_location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition, _client1, newClient);

        _location1EventReceiver.RelationChanged (_location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);


        _extension.RelationChanged (ClientTransactionMock, _location1, _location1.Properties[typeof (Location), "Client"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (ClientTransactionMock),
            Is.Same (_order1),
            Is.Equal (_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, orderItem, null);

        orderItemEventReceiver.RelationChanging (orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);

        _order1EventReceiver.RelationChanging (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, orderItem, null);

        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);

        orderItemEventReceiver.RelationChanged (orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (ClientTransactionMock),
            Is.Same (_order1),
            Is.Equal (_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (ClientTransactionMock, orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1);
        _extension.RelationChanging (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, orderItem);

        orderItemEventReceiver.RelationChanging (orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1);
        _order1EventReceiver.RelationChanging (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, orderItem);

        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        orderItemEventReceiver.RelationChanged (orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, orderItem, orderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (ClientTransactionMock),
            Is.Same (_order1),
            Is.Equal (_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition),
            Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, newOrderItem);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, newOrderItem, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, newOrderItem, null);

        
        oldOrderOfNewOrderItemEventReceiver.RelationChanged (oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        newOrderItemEventReceiver.RelationChanged (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (
            ClientTransactionMock, oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (ClientTransactionMock),
            Is.Same (_order1),
            Is.Equal (_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition),
            Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, oldOrderItem,oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, oldOrderItem, newOrderItem);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1);

        _order1EventReceiver.RelationChanging (
            _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, oldOrderItem, newOrderItem);

        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        
        newOrderItemEventReceiver.RelationChanged (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
        
        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extension.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Is.Same (ClientTransactionMock),
            Is.Same (_order1),
            Is.Equal (_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition),
            Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1),
            Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (
            ClientTransactionMock, oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);
        _extension.RelationChanging (
            ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (
            ClientTransactionMock, _order1,_order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, oldOrderItem, newOrderItem);
        _extension.RelationChanging (
            ClientTransactionMock, oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, newOrderItem, null);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (
            _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, oldOrderItem, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, newOrderItem, null);

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _order1EventReceiver.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        newOrderItemEventReceiver.RelationChanged (newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);

        _extension.RelationChanged (ClientTransactionMock, oldOrderOfNewOrderItem, oldOrderOfNewOrderItem.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, newOrderItem, newOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
        _extension.RelationChanged (ClientTransactionMock, oldOrderItem, oldOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition);
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
            ClientTransactionMock, removedOrderItem, removedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, addedOrderItem, addedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, removedOrderItem, null));
        _extension.Expect (mock => mock.RelationChanging (
            ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, addedOrderItem));

        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (removedOrderItem, removedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, _order1, null));
        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanging (
            addedOrderItem, addedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition, null, _order1));
        _order1EventReceiver.Expect (mock => mock.RelationChanging (
            _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, removedOrderItem, null));
        
        _order1EventReceiver.Expect (mock => mock.RelationChanging (
            _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, null, addedOrderItem));

        _order1EventReceiver.Expect (mock => mock.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition));

        _order1EventReceiver.Expect (mock => mock.RelationChanged (_order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition));

        addedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (addedOrderItem, addedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition));

        removedOrderItemEventReceiverMock.Expect (mock => mock.RelationChanged (removedOrderItem, removedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition));

        _extension.Expect (mock => mock.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition));
        _extension.Expect (mock => mock.RelationChanged (ClientTransactionMock, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition));

        _extension.Expect (mock => mock.RelationChanged (ClientTransactionMock, addedOrderItem, addedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition));
        _extension.Expect (mock => mock.RelationChanged (ClientTransactionMock, removedOrderItem, removedOrderItem.Properties[typeof (OrderItem), "Order"].PropertyData.RelationEndPointDefinition));

      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems = newCollection;

      _mockRepository.VerifyAll ();
    }
  }
}
