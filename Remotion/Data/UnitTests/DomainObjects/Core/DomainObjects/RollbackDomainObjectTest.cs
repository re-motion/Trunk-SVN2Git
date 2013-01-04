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

      Assert.That (customer.State, Is.EqualTo (StateType.Changed));

      TestableClientTransaction.Rollback ();

      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.Name, Is.EqualTo ("Kunde 1"));
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

      TestableClientTransaction.Rollback ();

      Assert.That (order.OrderTicket, Is.SameAs (oldOrderTicket));
      Assert.That (oldOrderTicket.Order, Is.SameAs (order));
      Assert.That (oldOrderOfNewOrderTicket.OrderTicket, Is.SameAs (newOrderTicket));
      Assert.That (newOrderTicket.Order, Is.SameAs (oldOrderOfNewOrderTicket));
    }

    [Test]
    public void RollbackOneToManyRelationChange ()
    {
      Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

      Order order = customer1.Orders[DomainObjectIDs.Order1];

      order.Customer = customer2;

      TestableClientTransaction.Rollback ();

      Assert.That (customer1.Orders[order.ID], Is.Not.Null);
      Assert.That (customer2.Orders[order.ID], Is.Null);
      Assert.That (order.Customer, Is.SameAs (customer1));

      Assert.That (customer1.Orders.Count, Is.EqualTo (2));
      Assert.That (customer2.Orders.Count, Is.EqualTo (0));
    }

    [Test]
    public void RollbackDeletion ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.Delete ();

      TestableClientTransaction.Rollback ();

      Computer computerAfterRollback = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.That (computerAfterRollback, Is.SameAs (computer));
      Assert.That (computer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void RollbackDeletionAndPropertyChange ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      computer.SerialNumber = "1111111111111";

      Assert.That (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetOriginalValue<string>(), Is.EqualTo ("63457-kol-34"));
      Assert.That (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetValue<string>(), Is.EqualTo ("1111111111111"));

      computer.Delete ();
      TestableClientTransaction.Rollback ();

      Assert.That (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetOriginalValue<string>(), Is.EqualTo ("63457-kol-34"));
      Assert.That (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].GetValue<string>(), Is.EqualTo ("63457-kol-34"));
      Assert.That (computer.Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"].HasChanged, Is.False);
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

      Assert.That (order.OrderTicket, Is.Null);
      Assert.That (order.OrderItems.Count, Is.EqualTo (0));
      Assert.That (order.Customer, Is.Null);
      Assert.That (order.Official, Is.Null);

      TestableClientTransaction.Rollback ();

      Assert.That (order.OrderTicket, Is.SameAs (oldOrderTicket));
      Assert.That (order.OrderItems.Count, Is.EqualTo (oldOrderItems.Count));
      Assert.That (order.OrderItems[DomainObjectIDs.OrderItem1], Is.SameAs (oldOrderItems[DomainObjectIDs.OrderItem1]));
      Assert.That (order.OrderItems[DomainObjectIDs.OrderItem2], Is.SameAs (oldOrderItems[DomainObjectIDs.OrderItem2]));
      Assert.That (order.Customer, Is.SameAs (oldCustomer));
      Assert.That (order.Official, Is.SameAs (oldOfficial));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void RollbackForNewObject ()
    {
      Order newOrder = Order.NewObject ();

      TestableClientTransaction.Rollback ();

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

      TestableClientTransaction.Rollback ();

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));
      Assert.That (customer.Orders.Contains (newOrderID), Is.False);
      Assert.That (orderItem1.Order, Is.SameAs (order1));
    }

    [Test]
    public void SetOneToManyRelationForNewObjectAfterRollback ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = OrderItem.NewObject (order);
      ObjectID orderItemID = orderItem.ID;

      Assert.That (orderItem.Order, Is.SameAs (order));
      Assert.That (order.OrderItems.Contains (orderItemID), Is.True);

      TestableClientTransaction.Rollback ();

      Assert.That (order.OrderItems.Contains (orderItemID), Is.False);

      orderItem = OrderItem.NewObject (order);

      Assert.That (orderItem.Order, Is.SameAs (order));
      Assert.That (order.OrderItems.ContainsObject (orderItem), Is.True);
    }

    [Test]
    public void DomainObjectCollectionIsSameAfterRollback ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection orderItems = order.OrderItems;
      OrderItem orderItem = OrderItem.NewObject (order);

      TestableClientTransaction.Rollback ();

      Assert.That (order.OrderItems, Is.SameAs (orderItems));
      Assert.That (order.OrderItems.IsReadOnly, Is.False);
    }
  }
}
