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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class DomainObjectInMultipleTransactionsTest : ClientTransactionBaseTest
  {
    [Test]
    public void EnlistedObjectState_NotLoadedYet ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      TestableClientTransaction.EnlistDomainObject (order);

      Assert.That (order.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void NewObject_CanBeEnlistedAndUsedInTransaction_WhenCommitted ()
    {
      SetDatabaseModifyable ();
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = Order.NewObject ();
      order.OrderNumber = 5;
      order.DeliveryDate = DateTime.Now;
      order.OrderItems.Add (OrderItem.NewObject ());
      order.OrderTicket = OrderTicket.NewObject ();
      order.Official = DomainObjectIDs.Official1.GetObject<Official>();
      order.Customer = DomainObjectIDs.Customer1.GetObject<Customer> ();

      newTransaction.EnlistDomainObject (order);

      ClientTransactionScope.CurrentTransaction.Commit ();

      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.That (order.OrderNumber, Is.EqualTo (5));
      }
      order.OrderTicket.Delete ();
      order.OrderItems[0].Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void EnlistedObject_CanBeUsedInTwoTransactions ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      newTransaction.EnlistDomainObject (order);
      Assert.That (newTransaction.IsEnlisted (order), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);

      Assert.That (order.OrderNumber, Is.Not.EqualTo (5));
      order.OrderNumber = 5;
      Assert.That (order.OrderNumber, Is.EqualTo (5));

      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.That (order.OrderNumber, Is.Not.EqualTo (5));
        order.OrderNumber = 6;
        Assert.That (order.OrderNumber, Is.EqualTo (6));
      }

      Assert.That (order.OrderNumber, Is.EqualTo (5));
    }

    [Test]
    public void EnlistedObjects_AreLoadedOnFirstAccess ()
    {
      SetDatabaseModifyable ();

      ClientTransaction newTransaction1 = ClientTransaction.CreateRootTransaction ();
      ClientTransaction newTransaction2 = ClientTransaction.CreateRootTransaction ();

      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      newTransaction1.EnlistDomainObject (order);

      int oldOrderNumber = order.OrderNumber;
      order.OrderNumber = 5;
      ClientTransactionScope.CurrentTransaction.Commit ();

      newTransaction2.EnlistDomainObject (order);

      using (newTransaction1.EnterNonDiscardingScope ())
      {
        Assert.That (order.OrderNumber, Is.Not.EqualTo (oldOrderNumber));
        Assert.That (order.OrderNumber, Is.EqualTo (5));
      }

      using (newTransaction2.EnterNonDiscardingScope ())
      {
        Assert.That (order.OrderNumber, Is.Not.EqualTo (oldOrderNumber));
        Assert.That (order.OrderNumber, Is.EqualTo (5));
      }

      order.OrderNumber = 3;
      ClientTransactionScope.CurrentTransaction.Commit ();

      using (newTransaction2.EnterNonDiscardingScope ())
      {
        Assert.That (order.OrderNumber, Is.EqualTo (5));
      }

      order.OrderNumber = oldOrderNumber;
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void DeletedObject_CanBeEnlistedInTransaction ()
    {
      var order = GetObjectDeleteCommit();

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (order);
      Assert.That (newTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ObjectsNotFoundException), ExpectedMessage = 
        "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' could not be found.", 
        MatchType = MessageMatch.Regex)]
    public void DeletedObject_NotFoundOnFirstAccess ()
    {
      var order = GetObjectDeleteCommit ();

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      newTransaction.EnlistDomainObject (order);

      using (newTransaction.EnterDiscardingScope ())
      {
        ++order.OrderNumber;
      }
    }

    [Test]
    public void DeletedObject_CanBeEnlistedAndUsedInTransaction_IfNotCommitted ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      int orderNumber = order.OrderNumber;
      order.Delete ();
      Assert.That (order.State, Is.EqualTo (StateType.Deleted));
      
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (order);
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.That (order.State, Is.Not.EqualTo (StateType.Deleted));
        Assert.That (order.OrderNumber, Is.EqualTo (orderNumber));
      }
    }

    [Test]
    public void GetObject_AfterEnlistingReturnsEnlistedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        Assert.That (order.ID.GetObject<Order> (), Is.SameAs (order));
      }
    }

    private Order GetObjectDeleteCommit ()
    {
      SetDatabaseModifyable ();
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();

      order.OrderTicket.Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();
      return order;
    }

  }
}