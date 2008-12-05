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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class RollbackDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void RollbackPropertyChange ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Name = "Arthur Dent";

      Assert.AreEqual (StateType.Changed, customer.State);

      ClientTransactionMock.Rollback ();

      Assert.AreEqual (StateType.Unchanged, customer.State);
      Assert.AreEqual ("Kunde 1", customer.Name);
    }

    [Test]
    public void RollbackOneToOneRelationChange ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = order.OrderTicket;
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      Order oldOrderOfNewOrderTicket = newOrderTicket.Order;

      order.OrderTicket = newOrderTicket;
      oldOrderOfNewOrderTicket.OrderTicket = oldOrderTicket;

      ClientTransactionMock.Rollback ();

      Assert.AreSame (oldOrderTicket, order.OrderTicket);
      Assert.AreSame (order, oldOrderTicket.Order);
      Assert.AreSame (newOrderTicket, oldOrderOfNewOrderTicket.OrderTicket);
      Assert.AreSame (oldOrderOfNewOrderTicket, newOrderTicket.Order);
    }

    [Test]
    public void RollbackOneToManyRelationChange ()
    {
      Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

      Order order = customer1.Orders[DomainObjectIDs.Order1];

      order.Customer = customer2;

      ClientTransactionMock.Rollback ();

      Assert.IsNotNull (customer1.Orders[order.ID]);
      Assert.IsNull (customer2.Orders[order.ID]);
      Assert.AreSame (customer1, order.Customer);

      Assert.AreEqual (2, customer1.Orders.Count);
      Assert.AreEqual (0, customer2.Orders.Count);
    }

    [Test]
    public void RollbackDeletion ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.Delete ();

      ClientTransactionMock.Rollback ();

      Computer computerAfterRollback = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.AreSame (computer, computerAfterRollback);
      Assert.AreEqual (StateType.Unchanged, computer.State);
    }

    [Test]
    public void RollbackDeletionAndPropertyChange ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "1111111111111";

      Assert.AreEqual ("63457-kol-34", computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetOriginalValue<string>());
      Assert.AreEqual ("1111111111111", computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetValue<string>());

      computer.Delete ();
      ClientTransactionMock.Rollback ();

      Assert.AreEqual ("63457-kol-34", computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetOriginalValue<string>());
      Assert.AreEqual ("63457-kol-34", computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetValue<string>());
      Assert.IsFalse (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].HasChanged);
    }

    [Test]
    public void RollbackDeletionWithRelationChange ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      OrderTicket oldOrderTicket = order.OrderTicket;
      DomainObjectCollection oldOrderItems = order.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      Customer oldCustomer = order.Customer;
      Official oldOfficial = order.Official;

      order.Delete ();

      Assert.IsNull (order.OrderTicket);
      Assert.AreEqual (0, order.OrderItems.Count);
      Assert.IsNull (order.Customer);
      Assert.IsNull (order.Official);

      ClientTransactionMock.Rollback ();

      Assert.AreSame (oldOrderTicket, order.OrderTicket);
      Assert.AreEqual (oldOrderItems.Count, order.OrderItems.Count);
      Assert.AreSame (oldOrderItems[DomainObjectIDs.OrderItem1], order.OrderItems[DomainObjectIDs.OrderItem1]);
      Assert.AreSame (oldOrderItems[DomainObjectIDs.OrderItem2], order.OrderItems[DomainObjectIDs.OrderItem2]);
      Assert.AreSame (oldCustomer, order.Customer);
      Assert.AreSame (oldOfficial, order.Official);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void RollbackForNewObject ()
    {
      Order newOrder = Order.NewObject ();

      ClientTransactionMock.Rollback ();

      int number = newOrder.OrderNumber;
    }

    [Test]
    public void RollbackForNewObjectWithRelations ()
    {
      Order newOrder = Order.NewObject ();
      ObjectID newOrderID = newOrder.ID;

      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      newOrder.OrderTicket = orderTicket1;
      customer.Orders.Add (newOrder);
      orderItem1.Order = newOrder;

      ClientTransactionMock.Rollback ();

      Assert.AreEqual (StateType.Unchanged, order1.State);
      Assert.AreSame (orderTicket1, order1.OrderTicket);
      Assert.IsFalse (customer.Orders.Contains (newOrderID));
      Assert.AreSame (order1, orderItem1.Order);
    }

    [Test]
    public void SetOneToManyRelationForNewObjectAfterRollback ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = OrderItem.NewObject (order);
      ObjectID orderItemID = orderItem.ID;

      Assert.AreSame (order, orderItem.Order);
      Assert.IsTrue (order.OrderItems.Contains (orderItemID));

      ClientTransactionMock.Rollback ();

      Assert.IsFalse (order.OrderItems.Contains (orderItemID));

      orderItem = OrderItem.NewObject (order);

      Assert.AreSame (order, orderItem.Order);
      Assert.IsTrue (order.OrderItems.ContainsObject (orderItem));
    }

    [Test]
    public void DomainObjectCollectionIsSameAfterRollback ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection orderItems = order.OrderItems;
      OrderItem orderItem = OrderItem.NewObject (order);

      ClientTransactionMock.Rollback ();

      Assert.AreSame (orderItems, order.OrderItems);
      Assert.IsFalse (order.OrderItems.IsReadOnly);
    }
  }
}
