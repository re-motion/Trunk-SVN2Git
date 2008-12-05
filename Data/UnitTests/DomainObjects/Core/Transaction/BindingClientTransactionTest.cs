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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class BindingClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _bindingTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bindingTransaction = ClientTransaction.CreateBindingTransaction ();
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
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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
