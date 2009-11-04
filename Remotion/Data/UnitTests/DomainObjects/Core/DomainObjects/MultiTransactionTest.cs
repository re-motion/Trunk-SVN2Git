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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class MultiTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      Assert.IsTrue (order.CanBeUsedInTransaction);
      Assert.IsFalse (order.TransactionContext[ClientTransaction.CreateRootTransaction()].CanBeUsedInTransaction);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the given transaction "
        + "as it was loaded or created in another transaction. Enter a scope for the transaction, or enlist the object in "
        + "the transaction. \\(If no transaction was explicitly given, ClientTransaction.Current was used.\\)", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction);
        Dev.Null = order.OrderNumber;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the given transaction "
        + "as it was loaded or created in another transaction. Enter a scope for the transaction, or enlist the object in "
        + "the transaction. \\(If no transaction was explicitly given, ClientTransaction.Current was used.\\)", 
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnDeleteWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction);
        order.Delete();
      }
    }

    [Test]
    public void LoadedObjectCanBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction);
      Assert.IsFalse (order.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void NewObjectCanBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = Order.NewObject ();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Order|fbab57e5-ba54-4d61-8bca-e8b9badc253a|System.Guid' could not be found.", MatchType = MessageMatch.Regex)]
    public void NewObjectCannotBeUsedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      Order order = Order.NewObject ();
      newTransaction.EnlistDomainObject (order);
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreEqual (1, order.OrderNumber);
      }
    }

    [Test]
    public void NewObjectCanBeEnlistedAndUsedInTransactionWhenCommitted ()
    {
      SetDatabaseModifyable ();
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = Order.NewObject ();
      order.OrderNumber = 5;
      order.DeliveryDate = DateTime.Now;
      order.OrderItems.Add (OrderItem.NewObject ());
      order.OrderTicket = OrderTicket.NewObject ();
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      newTransaction.EnlistDomainObject (order);

      ClientTransactionScope.CurrentTransaction.Commit ();

      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreEqual (5, order.OrderNumber);
      }
      order.OrderTicket.Delete ();
      order.OrderItems[0].Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void EnlistedObjectCanBeUsedInTwoTransactions ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction);

      Assert.AreNotEqual (5, order.OrderNumber);
      order.OrderNumber = 5;
      Assert.AreEqual (5, order.OrderNumber);

      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreNotEqual (5, order.OrderNumber);
        order.OrderNumber = 6;
        Assert.AreEqual (6, order.OrderNumber);
      }

      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    public void DeletedObjectCanBeEnlistedInTransaction ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();

      order.OrderTicket.Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException), ExpectedMessage = "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' could "
        + "not be found.", MatchType = MessageMatch.Regex)]
    public void DeletedObjectCannotBeUsedInTransaction ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();

      order.OrderTicket.Delete ();
      order.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      newTransaction.EnlistDomainObject (order);

      using (newTransaction.EnterDiscardingScope ())
      {
        ++order.OrderNumber;
      }
    }

    [Test]
    public void DeletedObjectCanBeEnlistedAndUsedInTransactionIfNotCommitted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;
      order.Delete ();
      Assert.AreEqual (StateType.Deleted, order.State);
      
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (order);
      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreNotEqual (StateType.Deleted, order.State);
        Assert.AreEqual (orderNumber, order.OrderNumber);
      }
    }

    [Test]
    public void EnlistedObjectsAreLoadedOnFirstAccess ()
    {
      SetDatabaseModifyable ();

      ClientTransaction newTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction newTransaction2 = ClientTransaction.CreateRootTransaction();
      
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      newTransaction1.EnlistDomainObject (order);

      int oldOrderNumber = order.OrderNumber;
      order.OrderNumber = 5;
      ClientTransactionScope.CurrentTransaction.Commit ();

      newTransaction2.EnlistDomainObject (order);

      using (newTransaction1.EnterNonDiscardingScope ())
      {
        Assert.AreNotEqual (oldOrderNumber, order.OrderNumber);
        Assert.AreEqual (5, order.OrderNumber);
      }

      using (newTransaction2.EnterNonDiscardingScope ())
      {
        Assert.AreNotEqual (oldOrderNumber, order.OrderNumber);
        Assert.AreEqual (5, order.OrderNumber);
      }

      order.OrderNumber = 3;
      ClientTransactionScope.CurrentTransaction.Commit ();

      using (newTransaction2.EnterNonDiscardingScope ())
      {
        Assert.AreEqual (5, order.OrderNumber);
      }

      order.OrderNumber = oldOrderNumber;
      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void GetObjectAfterEnlistingReturnsEnlistedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        Assert.IsTrue (order.CanBeUsedInTransaction);
        Assert.AreSame (order, Order.GetObject (order.ID));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistingAlthoughObjectHasAlreadyBeenLoadedThrows ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.AreNotSame (order, Order.GetObject (order.ID));
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
      }
    }

    [Test]
    public void MultipleEnlistmentsAreIgnored()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);

        using (ClientTransactionScope.CurrentTransaction.CreateSubTransaction ().EnterDiscardingScope ())
        {
          ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
          ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        }
      }
    }

    [Test]
    public void EnlistDomainObjectInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction ();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistDomainObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem1 = order.OrderItems[0];
      OrderItem orderItem2 = order.OrderItems[1];

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      newTransaction.EnlistDomainObjects (order, orderItem2);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsFalse (orderItem1.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem2.TransactionContext[newTransaction].CanBeUsedInTransaction);

      using (newTransaction.EnterDiscardingScope ())
      {
        Assert.AreEqual (1, order.OrderNumber);
        Assert.AreSame (order, orderItem2.Order);
        newTransaction.EnlistDomainObject (orderItem2);
        Assert.AreSame (order, orderItem2.Order);
      }
    }

    [Test]
    public void EnlistDomainObjectsInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction();

      Assert.IsFalse (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsFalse (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistDomainObjects (order, orderItem);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      Assert.IsTrue (order.TransactionContext[newTransaction.ParentTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction.ParentTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistDomainObjectsAlsoDoesDiscardedAndDeletedObjects ()
    {
      Order order = Order.NewObject ();
      order.Delete ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (order.IsDiscarded);

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete ();
        SetDatabaseModifyable ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObjects (order, cwadt);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (cwadt.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistDomainObjectsIgnoresObjectsAlreadyEnlisted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistDomainObjects (order);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistDomainObjectsWorksWithObjectsDeletedInDatabase ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObjects (cwadt);

      Assert.IsTrue (cwadt.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' could not be found.")]
    public void UsingEnlistedObjectsDeletedInDatabaseThrowsObjectNotFoundException ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      newTransaction.EnlistDomainObjects (cwadt);

      Assert.IsTrue (cwadt.TransactionContext[newTransaction].CanBeUsedInTransaction);
      using (newTransaction.EnterDiscardingScope ())
      {
        cwadt.StringProperty = "FoO";
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistDomainObjectsThrowsOnObjectsAlreadyEnlistedWithDifferentReferences ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      using (newTransaction.EnterDiscardingScope ())
      {
        Order order2 = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotSame (order, order2);
      }

      newTransaction.EnlistDomainObjects (order);
    }

    [Test]
    public void OnLoadedCanAccessValuePropertiesInEnlistDomainObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      order.ProtectedLoaded += ((sender, e) => Assert.AreEqual (1, ((Order) sender).OrderNumber));

      newTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void EnlistSameDomainObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      Assert.IsFalse (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsFalse (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistSameDomainObjects_WithDiscardedSource ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

      ClientTransactionMock.Discard ();
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();

      Assert.IsFalse (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsFalse (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistSameDomainObjectsInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ().CreateSubTransaction ();

      Assert.IsFalse (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsFalse (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);

      Assert.IsTrue (order.TransactionContext[newTransaction.ParentTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction.ParentTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void EnlistSameObjectsDoesntThrowOnNew ()
    {
      Order.NewObject ();
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
    }

    [Test]
    public void EnlistSameDomainObjectsWorksTwiceWithObjectsDeletedInDatabase ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      ClientTransaction newTransaction2 = ClientTransaction.CreateRootTransaction ();
      newTransaction2.EnlistSameDomainObjects (newTransaction, false);

      Assert.IsTrue (cwadt.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (cwadt.TransactionContext[newTransaction2].CanBeUsedInTransaction);
    }

    [Test]
    public void OnLoadedCanReliablyAccessRelatedObjectPropertiesInEnlistSameDomainObjectsFromOrder ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      order.ProtectedLoaded += ((sender, e) => Assert.AreSame (sender, ((Order) sender).OrderItems[0].Order));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.AreSame (orderItem, order.OrderItems[0]);
    }

    [Test]
    public void OnLoadedCanReliablyAccessRelatedObjectPropertiesInEnlistSameDomainObjectsFromOrderItem ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      Order order = orderItem.Order;

      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      orderItem.ProtectedLoaded += ((sender, e) => Assert.Contains (sender, ((OrderItem) sender).Order.OrderItems));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
      Assert.IsTrue (orderItem.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.IsTrue (order.TransactionContext[newTransaction].CanBeUsedInTransaction);
      Assert.AreSame (order, orderItem.Order);
    }

    [Test]
    public void EnlistWithNoCopyEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);

        ClientTransaction.Current.EnlistDomainObject (order);

        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsFalse (orderItemAdded);
      }
    }

    [Test]
    public void EnlistWithCopyEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);

        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.CopyCollectionEventHandlers (order, ClientTransactionMock);

        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsTrue (orderItemAdded);
      }
    }

    [Test]
    public void EnlistSameWithoutCopyEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, false);
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsFalse (orderItemAdded);
      }
    }

    [Test]
    public void EnlistSameWithCopyEventHandlers ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsTrue (orderItemAdded);
      }
    }

    [Test]
    public void EnlistSameWithCopyEventHandlers_SubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsTrue (orderItemAdded);
      }
    }

    [Test]
    public void EnlistSameWithCopyEventHandlers_WithDiscardedSource ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      ClientTransactionMock.Discard ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsTrue (orderItemAdded);
      }
    }
  }
}
