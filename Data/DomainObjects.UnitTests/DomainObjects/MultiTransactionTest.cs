using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class MultiTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransaction.NewRootTransaction()));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the current transaction "
        + "as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call "
        + "EnlistInTransaction to enlist the object in the current transaction.", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        int i = order.OrderNumber;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the current transaction "
        + "as it was loaded or created in another transaction. Use a ClientTransactionScope to set the right transaction, or call "
        + "EnlistInTransaction to enlist the object in the current transaction.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnDeleteWhenCannotBeUsedInTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        order.Delete();
      }
    }

    [Test]
    public void LoadedObjectCanBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void NewObjectCanBeEnlistedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
      Order order = Order.NewObject ();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Order|fbab57e5-ba54-4d61-8bca-e8b9badc253a|System.Guid' could not be found.", MatchType = MessageMatch.Regex)]
    public void NewObjectCannotBeUsedInTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
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
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
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
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

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

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
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

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
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
      
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction();
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

      ClientTransaction newTransaction1 = ClientTransaction.NewRootTransaction();
      ClientTransaction newTransaction2 = ClientTransaction.NewRootTransaction();
      
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
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.EnlistDomainObject (order);
        Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreSame (order, Order.GetObject (order.ID));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A domain object instance for object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void EnlistingAlthoughObjectHasAlreadyBeenLoadedThrows ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
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
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
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
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ().CreateSubTransaction ();
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistDomainObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem1 = order.OrderItems[0];
      OrderItem orderItem2 = order.OrderItems[1];

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      newTransaction.EnlistDomainObjects (order, orderItem2);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem1.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem2.CanBeUsedInTransaction (newTransaction));

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

      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (ClientTransactionMock));

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ().CreateSubTransaction();

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistDomainObjects (order, orderItem);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction.ParentTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction.ParentTransaction));
    }

    [Test]
    public void EnlistDomainObjectsAlsoDoesDiscardedAndDeletedObjects ()
    {
      Order order = Order.NewObject ();
      order.Delete ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (order.IsDiscarded);

      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete ();
        SetDatabaseModifyable ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();

      newTransaction.EnlistDomainObjects (order, cwadt);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistDomainObjectsIgnoresObjectsAlreadyEnlisted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      
      newTransaction.EnlistDomainObject (order);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistDomainObjects (order);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistDomainObjectsWorksWithObjectsDeletedInDatabase ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();

      newTransaction.EnlistDomainObjects (cwadt);

      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' could not be found.")]
    public void UsingEnlistedObjectsDeletedInDatabaseThrowsObjectNotFoundException ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();

      newTransaction.EnlistDomainObjects (cwadt);

      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
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
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();

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
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      order.ProtectedLoaded += delegate (object sender, EventArgs e) { Assert.AreEqual (1, ((Order) sender).OrderNumber); };

      newTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void EnlistSameDomainObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (ClientTransactionMock));

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));
    }

    [Test]
    public void EnlistSameDomainObjectsInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (ClientTransactionMock));

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ().CreateSubTransaction ();

      Assert.IsFalse (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsFalse (orderItem.CanBeUsedInTransaction (newTransaction));

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));

      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction.ParentTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction.ParentTransaction));
    }

    [Test]
    public void EnlistSameObjectsDoesntThrowOnNew ()
    {
      Order.NewObject ();
      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
    }

    [Test]
    public void EnlistSameDomainObjectsWorksTwiceWithObjectsDeletedInDatabase ()
    {
      SetDatabaseModifyable ();
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (cwadt.ID).Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);

      ClientTransaction newTransaction2 = ClientTransaction.NewRootTransaction ();
      newTransaction2.EnlistSameDomainObjects (newTransaction, false);

      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (cwadt.CanBeUsedInTransaction (newTransaction2));
    }

    [Test]
    public void OnLoadedCanReliablyAccessRelatedObjectPropertiesInEnlistSameDomainObjectsFromOrder ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem orderItem = order.OrderItems[0];

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      order.ProtectedLoaded += delegate (object sender, EventArgs e) { Assert.AreSame (sender, ((Order) sender).OrderItems[0].Order); };

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));
      Assert.AreSame (orderItem, order.OrderItems[0]);
    }

    [Test]
    public void OnLoadedCanReliablyAccessRelatedObjectPropertiesInEnlistSameDomainObjectsFromOrderItem ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      Order order = orderItem.Order;

      ClientTransaction newTransaction = ClientTransaction.NewRootTransaction ();
      orderItem.ProtectedLoaded += delegate (object sender, EventArgs e) { Assert.Contains (sender, ((OrderItem) sender).Order.OrderItems); };

      newTransaction.EnlistSameDomainObjects (ClientTransactionMock, false);
      Assert.IsTrue (orderItem.CanBeUsedInTransaction (newTransaction));
      Assert.IsTrue (order.CanBeUsedInTransaction (newTransaction));
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

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
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

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
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

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
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

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);
        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsTrue (orderItemAdded);
      }
    }

    [Test]
    public void EnlistSameWithCopyEventHandlers_CopiesOnlyOnEnlist ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      bool orderItemAdded = false;
      order.OrderItems.Added += delegate { orderItemAdded = true; };
      Assert.IsFalse (orderItemAdded);
      order.OrderItems.Add (OrderItem.NewObject ());
      Assert.IsTrue (orderItemAdded);

      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (order);
        ClientTransaction.Current.EnlistSameDomainObjects (ClientTransactionMock, true);

        orderItemAdded = false;
        Assert.IsFalse (orderItemAdded);
        order.OrderItems.Add (OrderItem.NewObject ());
        Assert.IsFalse (orderItemAdded);
      }
    }
  }
}
