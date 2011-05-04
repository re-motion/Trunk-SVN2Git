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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadInSubTransactionsTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _subTransaction = ClientTransactionMock.CreateSubTransaction ();
      _subTransaction.EnterDiscardingScope ();
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ActiveScope.Leave();

      base.TearDown ();
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (_subTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists (_subTransaction, order, true);
      CheckDataContainerExists (_subTransaction, orderItem1, true);
      CheckDataContainerExists (_subTransaction, orderItem2, true);

      CheckEndPointExists (_subTransaction, orderItem1, "Order", true);
      CheckEndPointExists (_subTransaction, orderItem2, "Order", true);
      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", false);

      CheckDataContainerExists (_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItem1, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItem2, true);

      CheckEndPointExists (_subTransaction.ParentTransaction, orderItem1, "Order", true);
      CheckEndPointExists (_subTransaction.ParentTransaction, orderItem2, "Order", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (order.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem2.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_EnsureDataComplete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (_subTransaction, orderItems.AssociatedEndPointID);

      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", false);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", false);

      orderItems.EnsureDataComplete ();

      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", true);
    }

    [Test]
    public void UnloadVirtualEndPoint_Collection_ChangedInParent_ButNotInSubTx ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;

      Assert.That (orderItems.Count, Is.EqualTo (2));
      orderItems.Add (OrderItem.NewObject());
      Assert.That (orderItems.Count, Is.EqualTo (3));
      _subTransaction.Commit();

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Changed));

      try
      {
        UnloadService.UnloadVirtualEndPoint (_subTransaction, orderItems.AssociatedEndPointID);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo (
            "The end point with ID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' "
            + "has been changed. Changed end points cannot be unloaded."));
      }

      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", false);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", true);

      orderItems.EnsureDataComplete ();

      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", true);
      Assert.That (orderItems.Count, Is.EqualTo (3));
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

      UnloadService.UnloadVirtualEndPoint (_subTransaction, orderItems.AssociatedEndPointID);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem1, orderItem2, OrderItem.GetObject (newOrderItemID) }));
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualEndPoint ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicket = order.OrderTicket;

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", true);
      
      UnloadService.UnloadVirtualEndPoint (_subTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckDataContainerExists (_subTransaction, order, true);
      CheckDataContainerExists (_subTransaction, orderTicket, true);

      CheckEndPointExists (_subTransaction, orderTicket, "Order", true);
      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", false);

      CheckDataContainerExists (_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderTicket, true);

      CheckEndPointExists (_subTransaction.ParentTransaction, orderTicket, "Order", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderTicket", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (order.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_EnsureDataComplete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      _subTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", true);

      UnloadService.UnloadVirtualEndPoint (_subTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", false);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderTicket", false);

      _subTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderTicket", true);
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_ChangedInParent_ButNotInSubTx ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var oldOrderTicket = order.OrderTicket;

      oldOrderTicket.Delete();
      var newOrderTicket = OrderTicket.NewObject();
      order.OrderTicket = newOrderTicket;
      
      _subTransaction.Commit ();

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Changed));

      try
      {
        UnloadService.UnloadVirtualEndPoint (_subTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo (
            "The end point with ID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' "
            + "has been changed. Changed end points cannot be unloaded."));
      }

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", false);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderTicket", true);

      _subTransaction.EnsureDataComplete (RelationEndPointID.Create (order, o => o.OrderTicket));

      CheckVirtualEndPoint (_subTransaction, order, "OrderTicket", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderTicket", true);
      Assert.That (order.OrderTicket, Is.SameAs (newOrderTicket));
    }

    [Test]
    public void UnloadVirtualEndPoint_ObjectEndPoint_Reload ()
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

      UnloadService.UnloadVirtualEndPoint (_subTransaction, RelationEndPointID.Create (order, o => o.OrderTicket));

      Assert.That (order.OrderTicket, Is.SameAs (OrderTicket.GetObject (newOrderTicketID)));
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
      customerOrders.EnsureDataComplete ();

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataComplete, Is.True);

      UnloadService.UnloadData (_subTransaction, order1.ID);

      CheckDataContainerExists (_subTransaction, order1, false);
      CheckDataContainerExists (_subTransaction, orderItemA, true);
      CheckDataContainerExists (_subTransaction, orderItemB, true);
      CheckDataContainerExists (_subTransaction, orderTicket, true);
      CheckDataContainerExists (_subTransaction, customer, true);

      CheckEndPointExists (_subTransaction, orderTicket, "Order", true);
      CheckEndPointExists (_subTransaction, order1, "OrderTicket", true);
      CheckEndPointExists (_subTransaction, orderItemA, "Order", true);
      CheckEndPointExists (_subTransaction, orderItemB, "Order", true);
      CheckVirtualEndPoint (_subTransaction, order1, "OrderItems", true);
      CheckEndPointExists (_subTransaction, order1, "Customer", false);
      CheckVirtualEndPoint (_subTransaction, customer, "Orders", false);

      CheckDataContainerExists (_subTransaction.ParentTransaction, order1, false);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItemA, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItemB, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderTicket, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, customer, true);

      CheckEndPointExists (_subTransaction.ParentTransaction, orderTicket, "Order", true);
      CheckEndPointExists (_subTransaction.ParentTransaction, order1, "OrderTicket", true);
      CheckEndPointExists (_subTransaction.ParentTransaction, orderItemA, "Order", true);
      CheckEndPointExists (_subTransaction.ParentTransaction, orderItemB, "Order", true);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order1, "OrderItems", true);
      CheckEndPointExists (_subTransaction.ParentTransaction, order1, "Customer", false);
      CheckVirtualEndPoint (_subTransaction.ParentTransaction, customer, "Orders", false);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataComplete, Is.False);

      Assert.That (order1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItemA.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
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

      UnloadService.UnloadData (_subTransaction, order1.ID);

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

      UnloadService.UnloadData (_subTransaction, computer1.ID);

      Assert.That (computer1.Employee, Is.SameAs (Employee.GetObject (newEmployeeID)));
    }

    [Test]
    public void UnloadData_Changed_InParentTransaction_ButNotInSubTransaction ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderNumber = 4711;
      _subTransaction.Commit ();

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Changed));

      try
      {
        UnloadService.UnloadData (_subTransaction, DomainObjectIDs.Order1);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo (
            "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed)."));
      }

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Changed));

      Assert.That (order1.OrderNumber, Is.EqualTo (4711));

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void UnloadCollectionEndPointAndData ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);

      Assert.That (orderItems.IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPointAndData (_subTransaction, orderItems.AssociatedEndPointID);

      CheckDataContainerExists (_subTransaction, order, true);
      CheckDataContainerExists (_subTransaction, orderItem1, false);
      CheckDataContainerExists (_subTransaction, orderItem2, false);

      CheckVirtualEndPoint (_subTransaction, order, "OrderItems", false);
      CheckEndPointExists (_subTransaction, orderItem1, "Order", false);
      CheckEndPointExists (_subTransaction, orderItem2, "Order", false);

      CheckDataContainerExists (_subTransaction.ParentTransaction, order, true);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItem1, false);
      CheckDataContainerExists (_subTransaction.ParentTransaction, orderItem2, false);

      CheckVirtualEndPoint (_subTransaction.ParentTransaction, order, "OrderItems", false);
      CheckEndPointExists (_subTransaction.ParentTransaction, orderItem1, "Order", false);
      CheckEndPointExists (_subTransaction.ParentTransaction, orderItem2, "Order", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItem1.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
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

      UnloadService.UnloadCollectionEndPointAndData (_subTransaction, orderItems.AssociatedEndPointID);

      Assert.That (orderItems, Is.EquivalentTo (new[] { orderItem2, OrderItem.GetObject (newOrderItemID) }));
      Assert.That (orderItem1.Order, Is.SameAs (Order.GetObject (DomainObjectIDs.Order2)));
    }

    [Test]
    public void Events ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      var subListenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_subTransaction);
      var rootListenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_subTransaction.ParentTransaction);
      using (subListenerMock.GetMockRepository ().Ordered ())
      {
        subListenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
            });
        subListenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (orderItemB.State, Is.EqualTo (StateType.NotLoadedYet));
            });

        rootListenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
              Assert.That (orderItemB.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.Unchanged));
            });
        rootListenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (_subTransaction), 
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderItemA, orderItemB })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderItemA.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (orderItemB.TransactionContext[_subTransaction.ParentTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      subListenerMock.Replay ();

      UnloadService.UnloadCollectionEndPointAndData (_subTransaction, order1.OrderItems.AssociatedEndPointID);

      subListenerMock.VerifyAllExpectations ();
      subListenerMock.BackToRecord (); // For Discarding
    }

    private void CheckDataContainerExists (ClientTransaction clientTransaction, DomainObject domainObject, bool dataContainerShouldExist)
    {
      var dataContainer = ClientTransactionTestHelper.GetDataManager (clientTransaction).DataContainers[domainObject.ID];
      if (dataContainerShouldExist)
        Assert.That (dataContainer, Is.Not.Null, "Data container '{0}' does not exist.", domainObject.ID);
      else
        Assert.That (dataContainer, Is.Null, "Data container '{0}' should not exist.", domainObject.ID);
    }

    private void CheckEndPointExists (ClientTransaction clientTransaction, DomainObject owningObject, string shortPropertyName, bool endPointShouldExist)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionTestHelper.GetDataManager (clientTransaction).GetRelationEndPointWithoutLoading (endPointID);
      if (endPointShouldExist)
        Assert.That (endPoint, Is.Not.Null, "End point '{0}' does not exist.", endPointID);
      else
        Assert.That (endPoint, Is.Null, "End point '{0}' should not exist.", endPointID);
    }

    private void CheckVirtualEndPoint (
        ClientTransaction clientTransaction, 
        DomainObject owningObject, 
        string shortPropertyName, 
        bool shouldDataBeComplete)
    {
      CheckEndPointExists (clientTransaction, owningObject, shortPropertyName, true);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, shortPropertyName);
      var endPoint = ClientTransactionTestHelper.GetDataManager (clientTransaction).GetRelationEndPointWithoutLoading (endPointID);
      if (shouldDataBeComplete)
        Assert.That (endPoint.IsDataComplete, Is.True, "End point '{0}' does not have any data.", endPoint.ID);
      else
        Assert.That (endPoint.IsDataComplete, Is.False, "End point '{0}' should not have any data.", endPoint.ID);
    }
  }
}