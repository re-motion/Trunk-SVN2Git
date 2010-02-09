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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class UnloadTest : ClientTransactionBaseTest
  {
    [Test]
    public void UnloadCollectionEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataAvailable, Is.True);

      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock, 
          orderItems.AssociatedEndPoint.ID, 
          UnloadTransactionMode.ThisTransactionOnly);

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderItem1, true);
      CheckDataContainerExists (orderItem2, true);

      CheckEndPointExists (orderItem1, "Order", true);
      CheckEndPointExists (orderItem2, "Order", true);
      CheckCollectionEndPoint (order, "OrderItems", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadCollectionEndPoint_AccessingEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataAvailable, Is.True);

      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock,
          orderItems.AssociatedEndPoint.ID,
          UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (orderItems.IsDataAvailable, Is.False);

      Dev.Null = order.OrderItems;

      Assert.That (orderItems.IsDataAvailable, Is.False, "Reaccessing the end point does not load data");

      var orderItemArray = order.OrderItems.ToArray ();

      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItemArray, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadCollectionEndPoint_EnsureDataAvailable ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataAvailable, Is.True);

      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock,
          orderItems.AssociatedEndPoint.ID,
          UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (orderItems.IsDataAvailable, Is.False);

      orderItems.EnsureDataAvailable ();

      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadCollectionEndPoint_Reload ()
    {
      SetDatabaseModifyable ();

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      ObjectID newOrderItemID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (DomainObjectIDs.Order1);
        var newOrderItem = OrderItem.NewObject ();
        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add (newOrderItem);
        ClientTransaction.Current.Commit ();
      }

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadCollectionEndPoint (
          ClientTransactionMock, 
          orderItems.AssociatedEndPoint.ID, 
          UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2, OrderItem.GetObject (newOrderItemID) }));
    }
    
    [Test]
    public void UnloadCollectionEndPoint_AlreadyUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var endPoint = customer.Orders.AssociatedEndPoint;

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, endPoint.ID, UnloadTransactionMode.ThisTransactionOnly);
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      Assert.That (endPoint.IsDataAvailable, Is.False);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, endPoint.ID, UnloadTransactionMode.ThisTransactionOnly);
    }

    [Test]
    public void UnloadData_OrderTicket ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order = orderTicket1.Order;
      order.EnsureDataAvailable ();

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (ClientTransactionMock, orderTicket1.ID, UnloadTransactionMode.ThisTransactionOnly);

      CheckDataContainerExists (orderTicket1, false);
      CheckDataContainerExists (order, true);

      CheckEndPointExists (orderTicket1, "Order", false);
      CheckEndPointExists (order, "OrderTicket", false);

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadData_Order ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;
      var customer = order1.Customer;
      var customerOrders = customer.Orders;

      customer.EnsureDataAvailable ();

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataAvailable, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      CheckDataContainerExists (order1, false);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);
      CheckDataContainerExists (orderTicket, true);
      CheckDataContainerExists (customer, true);

      CheckEndPointExists (orderTicket, "Order", true);
      CheckEndPointExists (order1, "OrderTicket", true);
      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckCollectionEndPoint (order1, "OrderItems", true);
      CheckEndPointExists (order1, "Customer", false);
      CheckCollectionEndPoint (customer, "Orders", false);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataAvailable, Is.False);
    }

    [Test]
    public void UnloadData_OrderItem ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (ClientTransactionMock, orderItemA.ID, UnloadTransactionMode.ThisTransactionOnly);

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, false);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", false);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckCollectionEndPoint (order1, "OrderItems", false);

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataAvailable, Is.False);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadData_Reload_PropertyValue ()
    {
      SetDatabaseModifyable();
      var order1 = Order.GetObject (DomainObjectIDs.Order1);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (order1.ID);
        orderInOtherTx.OrderNumber = 4711;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order1.OrderNumber, Is.EqualTo (1));

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order1.OrderNumber, Is.EqualTo (4711));
    }

    [Test]
    public void UnloadData_Reload_ForeignKey ()
    {
      SetDatabaseModifyable ();
      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      ObjectID newEmployeeID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTx = Computer.GetObject (computer1.ID);
        computerInOtherTx.Employee = Employee.NewObject ();
        newEmployeeID = computerInOtherTx.Employee.ID;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (computer1.Employee, Is.SameAs (Employee.GetObject (DomainObjectIDs.Employee3)));

      UnloadService.UnloadData (ClientTransactionMock, computer1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (computer1.Employee, Is.SameAs (Employee.GetObject (newEmployeeID)));
    }

    [Test]
    public void UnloadData_AlreadyUnloaded ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (ClientTransactionMock.GetEnlistedDomainObject (DomainObjectIDs.Order1), Is.SameAs (order1));

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadData_NonLoadedObject ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      UnloadService.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (ClientTransactionMock.GetEnlistedDomainObject (DomainObjectIDs.Order1), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void UnloadData_Changed ()
    {
      ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
      UnloadService.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1, UnloadTransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'.")]
    public void UnloadData_ChangedVirtualEndPoint ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);
      domainObject.OrderTicket = OrderTicket.NewObject ();
      UnloadService.UnloadData (ClientTransactionMock, domainObject.ID, UnloadTransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer'.")]
    public void UnloadData_ChangedVirtualNullEndPoint ()
    {
      var domainObject = Employee.GetObject (DomainObjectIDs.Employee3);

      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (domainObject.ID, "Computer");
      var virtualEndPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (virtualEndPointID);
      Assert.That (virtualEndPoint.OppositeObjectID, Is.Not.Null);
      virtualEndPoint.OppositeObjectID = null;

      Assert.That (virtualEndPoint.HasChanged, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, domainObject.ID, UnloadTransactionMode.ThisTransactionOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because one of its relations has been changed. Only "
        + "unchanged objects can be unloaded. Changed end point: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'.")]
    public void UnloadData_ChangedCollection ()
    {
      OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order.OrderItems.Add (OrderItem.NewObject ());
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID].HasChanged, Is.True);

      UnloadService.UnloadData (ClientTransactionMock, DomainObjectIDs.OrderItem1, UnloadTransactionMode.ThisTransactionOnly);
    }

    [Test]
    public void UnloadCollectionEndPointAndData ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataAvailable, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (
          ClientTransactionMock,
          orderItems.AssociatedEndPoint.ID,
          UnloadTransactionMode.ThisTransactionOnly);

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderItem1, false);
      CheckDataContainerExists (orderItem2, false);

      CheckCollectionEndPoint (order, "OrderItems", false);
      CheckEndPointExists (orderItem1, "Order", false);
      CheckEndPointExists (orderItem2, "Order", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (orderItems.IsDataAvailable, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_EnsureDataAvailable ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataAvailable, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (
          ClientTransactionMock,
          orderItems.AssociatedEndPoint.ID,
          UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataAvailable, Is.False);

      orderItem1.EnsureDataAvailable ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataAvailable, Is.False);

      orderItems.EnsureDataAvailable ();

      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataAvailable, Is.True);
      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_Reload ()
    {
      SetDatabaseModifyable ();

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      ObjectID newOrderItemID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (DomainObjectIDs.Order1);
        var orderItem1InOtherTx = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        var newOrderItem = OrderItem.NewObject ();
        newOrderItemID = newOrderItem.ID;
        orderInOtherTx.OrderItems.Add (newOrderItem);
        orderInOtherTx.OrderItems.Remove (orderItem1InOtherTx);

        orderItem1InOtherTx.Order = Order.GetObject (DomainObjectIDs.Order2);

        ClientTransaction.Current.Commit ();
      }

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));

      UnloadService.UnloadCollectionEndPointAndData (
          ClientTransactionMock,
          orderItems.AssociatedEndPoint.ID,
          UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem2, OrderItem.GetObject (newOrderItemID) }));
      Assert.That (orderItem1.Order, Is.SameAs (Order.GetObject (DomainObjectIDs.Order2)));
    }

    [Test]
    public void Events ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);
      using (listenerMock.GetMockRepository ().Ordered ())
      {
        listenerMock
            .Expect (mock => mock.ObjectsUnloading (Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.UnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.UnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemA.UnloadedCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderItemB.UnloadedCalled, Is.False, "items unloaded after this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
            });
        listenerMock
            .Expect (mock => mock.ObjectsUnloaded (Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.UnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.UnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemA.UnloadedCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderItemB.UnloadedCalled, Is.True, "items unloaded before this method is called");

              Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      listenerMock.Replay ();

      UnloadService.UnloadCollectionEndPointAndData (
          ClientTransactionMock, 
          order1.OrderItems.AssociatedEndPoint.ID, 
          UnloadTransactionMode.ThisTransactionOnly);

      listenerMock.VerifyAllExpectations ();

      Assert.That (orderItemA.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemB.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderItemA.UnloadingDateTime, Is.LessThan (orderItemB.UnloadingDateTime), "orderItemA.OnUnloading before orderItemB.OnUnloading");

      Assert.That (orderItemA.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemB.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
      Assert.That (orderItemA.UnloadedDateTime, Is.GreaterThan (orderItemB.UnloadedDateTime), "orderItemA.OnUnloaded after orderItemB.OnUnloaded");
    }

    [Test]
    public void ReadingValueProperties_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      Dev.Null = order1.OrderNumber;

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void WritingValueProperties_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      order1.OrderNumber = 4711;

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ReadingStateProperties_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      Dev.Null = order1.ID;
      Dev.Null = order1.IsDiscarded;
      Dev.Null = order1.State;
      try
      {
        Dev.Null = order1.GetBindingTransaction ();
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingTimestamp_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      Dev.Null = order1.Timestamp;

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void MarkAsChanged_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      order1.MarkAsChanged ();

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void EnsureDataAvailable_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      order1.EnsureDataAvailable ();

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }
    
    [Test]
    public void ReadingPropertyAccessor_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      order1.PreparePropertyAccess (typeof (Order).FullName + ".OrderNumber");
      Dev.Null = order1.CurrentProperty;
      order1.PropertyAccessFinished ();

      Dev.Null = order1.Properties;

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingTransactionContext_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      Dev.Null = order1.DefaultTransactionContext;
      Dev.Null = order1.TransactionContext;

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingCollectionEndPoint_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      var customerOrders = customer.Orders;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);

      Assert.That (customer.Orders, Is.SameAs (customerOrders)); // does not reload the object or the relation

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);
    }

    [Test]
    public void ChangingCollectionEndPoint_ReloadsCollectionAndObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      var customerOrders = customer.Orders;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      customer.Orders.Add (Order.NewObject ()); // reloads the relation contents and thus the object

      AssertObjectWasLoadedAmongOthers(listenerMock, order1);
      
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataAvailable, Is.True);
    }

    [Test]
    [Ignore ("TODO 2264")]
    public void ReadingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket)); // does not reload the object
      Assert.That (orderTicket.Order, Is.SameAs (order1)); // does not reload the object
      Assert.That (order1.OrderItems, Is.SameAs (orderItems)); // does not reload the object
      Assert.That (order1.OrderItems, Is.EquivalentTo (new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That (orderItemA.Order, Is.SameAs (order1)); // does not reload the object
      Assert.That (orderItemB.Order, Is.SameAs (order1)); // does not reload the object

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2264")]
    public void ReadingOriginalVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.Properties.Find ("OrderTicket").GetOriginalValueWithoutTypeCheck(), Is.SameAs (orderTicket)); // does not reload the object
      Assert.That (orderTicket.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object
      Assert.That (order1.Properties.Find ("OrderItems").GetOriginalValueWithoutTypeCheck (), Is.EquivalentTo (new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That (orderItemA.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object
      Assert.That (orderItemB.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void ChangingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      order1.OrderTicket = OrderTicket.NewObject(); // does not reload the object
      Assert.That (orderTicket.Order, Is.Null);

      order1.OrderItems.Add (OrderItem.NewObject ()); // does not reload the object
      order1.OrderItems = new ObjectList<OrderItem> (new[] { orderItemA }); // does not reload the object
      Assert.That (orderItemA.Order, Is.SameAs (order1));
      Assert.That (orderItemB.Order, Is.Null);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void ChangingRealRelationEndPoints_DoesNotReloadOppositeObjects ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      EnsureTransactionThrowsOnLoad ();

      orderTicket.Order = Order.NewObject ();
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      orderItemA.Order = Order.NewObject ();
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.Customer, Is.SameAs (customer)); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded (listenerMock, order1);

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void ChangingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      order1.Customer = Customer.NewObject (); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ReadingOppositeCollectionEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (customer.Orders, List.Contains (order1)); // enumerating reloads the relation contents because the foreign key is stored in order1

      AssertObjectWasLoadedAmongOthers (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void AddingToCollectionEndPoint_DoesntReloadOtherItems ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      Console.WriteLine (customer.State);

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Console.WriteLine (customer.State);
      EnsureTransactionThrowsOnLoad ();
      
      customer.Orders.Add (Order.NewObject()); // does not reload order1 because that object's foreign key is not involved

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }
    
    [Test]
    public void AddingToCollectionEndPoint_ReloadsObjectBeingAdded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);

      UnloadService.UnloadData (ClientTransactionMock, order2.ID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order2.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      customer.Orders.Add (order2); // reloads order2 because order2's foreign key is changed

      AssertObjectWasLoaded (listenerMock, order2);
      Assert.That (order2.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void Commit_DoesNotReloadObjectOrCollection ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);

      EnsureTransactionThrowsOnLoad();

      ClientTransactionMock.Commit();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);
    }

    [Test]
    public void Rollback_DoesNotReloadObjectOrCollection ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData (ClientTransactionMock, order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);

      EnsureTransactionThrowsOnLoad ();

      ClientTransactionMock.Rollback();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataAvailable, Is.False);
    }

    private void CheckDataContainerExists (DomainObject domainObject, bool dataContainerShouldExist)
    {
      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID];
      if (dataContainerShouldExist)
        Assert.That (dataContainer, Is.Not.Null, "Data container '{0}' does not exist.", domainObject.ID);
      else
        Assert.That (dataContainer, Is.Null, "Data container '{0}' should not exist.", domainObject.ID);
    }

    private void CheckEndPointExists (DomainObject owningObject, string shortPropertyName, bool endPointShouldExist)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionMock.DataManager.RelationEndPointMap[endPointID];
      if (endPointShouldExist)
        Assert.That (endPoint, Is.Not.Null, "End point '{0}' does not exist.", endPointID);
      else
        Assert.That (endPoint, Is.Null, "End point '{0}' should not exist.", endPointID);
    }

    private void CheckCollectionEndPoint (DomainObject owningObject, string shortPropertyName, bool shouldDataBeAvailable)
    {
      CheckEndPointExists (owningObject, shortPropertyName, true);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionMock.DataManager.RelationEndPointMap[endPointID];
      if (shouldDataBeAvailable)
        Assert.That (endPoint.IsDataAvailable, Is.True, "End point '{0}' does not have any data.", endPoint.ID);
      else
        Assert.That (endPoint.IsDataAvailable, Is.False, "End point '{0}' should not have any data.", endPoint.ID);
    }

    private void EnsureTransactionThrowsOnLoad ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvent (
          ClientTransactionMock,
          mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    private void AssertObjectWasLoaded (IClientTransactionListener listenerMock, DomainObject loadedObject)
    {
      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { loadedObject })));
    }

    private void AssertObjectWasLoadedAmongOthers (IClientTransactionListener listenerMock, DomainObject loadedObject)
    {
      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (Arg<ReadOnlyCollection<DomainObject>>.List.ContainsAll (new[] { loadedObject })));
    }
  }
}