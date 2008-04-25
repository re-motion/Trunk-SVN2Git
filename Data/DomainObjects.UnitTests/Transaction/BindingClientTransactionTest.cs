using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class BindingClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _bindingTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bindingTransaction = ClientTransaction.NewBindingTransaction ();
    }

    private T NewBound<T> (params object[] args)
        where T : DomainObject
    {
      using (_bindingTransaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T)).Invoke (args);
      }
    }

    private T GetBound<T> (ObjectID id)
        where T : DomainObject
    {
      using (_bindingTransaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.GetObject (id, true);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Binding transactions cannot have subtransactions.")]
    public void BindingTransactionCannotCreateSubtransaction ()
    {
      _bindingTransaction.CreateSubTransaction();
    }

    [Test]
    public void RollbackTransaction ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      OrderTicket oldTicket = order.OrderTicket;
      OrderItem oldItem = order.OrderItems[0];
      Assert.AreEqual (1, order.OrderNumber);

      OrderTicket newTicket = NewBound<OrderTicket>();
      OrderItem newItem = NewBound<OrderItem>();

      order.OrderNumber = 12;
      order.OrderTicket = newTicket;
      order.OrderItems[0] = newItem;
      Assert.AreNotEqual (1, order.OrderNumber);

      _bindingTransaction.Rollback();
      Assert.AreEqual (1, order.OrderNumber);
      Assert.AreSame (oldTicket, order.OrderTicket);
      Assert.AreSame (oldItem, order.OrderItems[0]);
    }

    [Test]
    public void CommitTransaction ()
    {
      SetDatabaseModifyable();

      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      OrderTicket oldTicket = order.OrderTicket;
      OrderItem oldItem = order.OrderItems[0];
      Assert.AreEqual (1, order.OrderNumber);

      OrderTicket newTicket = NewBound<OrderTicket>();
      OrderItem newItem = NewBound<OrderItem>();

      order.OrderNumber = 12;
      order.OrderTicket = newTicket;
      order.OrderItems[0] = newItem;
      Assert.AreNotEqual (1, order.OrderNumber);

      oldTicket.Delete();
      oldItem.Delete();

      _bindingTransaction.Commit ();

      Assert.AreEqual (12, order.OrderNumber);
      Assert.AreSame (newTicket, order.OrderTicket);
      Assert.AreSame (newItem, order.OrderItems[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot enlist the domain object Order|5682f032-2f0b-494b-a31c-"
        + "c97f02b89c36|System.Guid in this binding transaction, because it has originally been loaded in another transaction.")]
    public void EnlistingFails_ForObjectFromOther ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        _bindingTransaction.EnlistDomainObject (order);
      }
    }

    [Test]
    public void EnlistingSucceeds_ForSameTransaction ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      _bindingTransaction.EnlistDomainObject (order);
    }
  }
}