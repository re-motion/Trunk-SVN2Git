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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadVirtualEndPointTest : UnloadTestBase
  {
    [Test]
    public void UnloadVirtualEndPoint_Collection ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderItem1, true);
      CheckDataContainerExists (orderItem2, true);

      CheckEndPointExists (orderItem1, "Order", true);
      CheckEndPointExists (orderItem2, "Order", true);
      CheckVirtualEndPoint (order, "OrderItems", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_AccessingEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (
          TestableClientTransaction,
          orderItems.AssociatedEndPointID);

      Assert.That (orderItems.IsDataComplete, Is.False);

      Dev.Null = order.OrderItems;

      Assert.That (orderItems.IsDataComplete, Is.False, "Reaccessing the end point does not load data");

      var orderItemArray = order.OrderItems.ToArray ();

      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemArray, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_EnsureDataComplete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (
          TestableClientTransaction,
          orderItems.AssociatedEndPointID);

      Assert.That (orderItems.IsDataComplete, Is.False);

      orderItems.EnsureDataComplete ();

      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_Reload ()
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

      UnloadService.UnloadVirtualEndPoint (
          TestableClientTransaction, 
          orderItems.AssociatedEndPointID);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2, OrderItem.GetObject (newOrderItemID) }));
    }
    
    [Test]
    public void UnloadVirtualEndPoint_Collection_AlreadyUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var endPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPoint.ID);
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);
      Assert.That (endPoint.IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPoint.ID);
    }

    [Test]
    public void UnloadVirtualEndPoint_Object ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPoint (order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderTicket, true);

      CheckEndPointExists (orderTicket, "Order", true);
      CheckVirtualEndPoint (order, "OrderTicket", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_AccessingEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", false);

      Dev.Null = order.OrderTicket;

      CheckVirtualEndPoint (order, "OrderTicket", true);
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_EnsureDataComplete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPoint (order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", false);

      TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", true);
      Assert.That (order.OrderTicket, Is.SameAs (orderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_Reload ()
    {
      SetDatabaseModifyable ();

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var oldOrderTicket = order.OrderTicket;

      ObjectID newOrderTicketID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (DomainObjectIDs.Order1);
        orderInOtherTx.OrderTicket.Delete();

        orderInOtherTx.OrderTicket = OrderTicket.NewObject ();
        newOrderTicketID = orderInOtherTx.OrderTicket.ID;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.OrderTicket, Is.SameAs (oldOrderTicket));

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      Assert.That (order.OrderTicket, Is.SameAs (OrderTicket.GetObject (newOrderTicketID)));
    }

    [Test]
    public void UnloadVirtualEndPoint_Object_AlreadyUnloaded ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (order, "OrderTicket", false);
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_EndPointsOfNewObject_CannotBeUnloaded ()
    {
      var order = Order.NewObject ();
      var endPointID1 = RelationEndPointID.Create (order, o => o.OrderTicket);
      var endPointID2 = RelationEndPointID.Create (order, o => o.OrderItems);

      CheckEndPointExists (endPointID1, true);
      CheckEndPointExists (endPointID2, true);

      Assert.That (
          () => UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPointID1),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID1 + "."));
      Assert.That (
          () => UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPointID2),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID2 + "."));

      Assert.That (UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, endPointID1), Is.False);
      Assert.That (UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, endPointID2), Is.False);

      CheckEndPointExists (endPointID1, true);
      CheckEndPointExists (endPointID2, true);
    }

    [Test]
    public void UnloadVirtualEndPoint_EndPointsOfDeletedObject_CannotBeUnloaded ()
    {
      var customerWithoutOrders = Customer.GetObject (DomainObjectIDs.Customer2);
      var employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      var endPointID1 = RelationEndPointID.Create (customerWithoutOrders, o => o.Orders);
      var endPointID2 = RelationEndPointID.Create (employeeWithoutComputer, o => o.Computer);

      customerWithoutOrders.Delete ();
      employeeWithoutComputer.Delete ();

      CheckEndPointExists (endPointID1, true);
      CheckEndPointExists (endPointID2, true);

      Assert.That (
          () => UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPointID1),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID1 + "."));
      Assert.That (
          () => UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, endPointID2),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot unload the following relation end-points because they belong to new or deleted objects: " + endPointID2 + "."));

      Assert.That (UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, endPointID1), Is.False);
      Assert.That (UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, endPointID2), Is.False);

      CheckEndPointExists (endPointID1, true);
      CheckEndPointExists (endPointID2, true);
    }
  }
}