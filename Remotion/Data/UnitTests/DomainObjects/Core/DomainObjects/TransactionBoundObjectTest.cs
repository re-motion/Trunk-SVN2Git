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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class TransactionBoundObjectTest : StandardMappingTest
  {
    private ClientTransaction _bindingTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      _bindingTransaction = ClientTransaction.CreateBindingTransaction ();
    }

    [Test]
    public void NewBoundObject ()
    {
      Order order = NewBound<Order>();
      Assert.IsTrue (order.HasBindingTransaction);
      Assert.AreSame (_bindingTransaction, order.GetBindingTransaction());
    }

    [Test]
    public void GetBoundObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsTrue (order.HasBindingTransaction);
      Assert.AreSame (_bindingTransaction, order.GetBindingTransaction());
    }

    [Test]
    public void GetBoundObject_WithCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.IsTrue (order.HasBindingTransaction);
        Assert.AreSame (_bindingTransaction, order.GetBindingTransaction());
        Assert.AreNotSame (order, Order.GetObject (DomainObjectIDs.Order1));
      }
    }

    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = GetBound<Order>(DomainObjectIDs.Order1);
      Assert.IsTrue (_bindingTransaction.IsEnlisted (order));
    }

    [Test]
    public void CanBeUsedInTransaction_WithCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.IsTrue (_bindingTransaction.IsEnlisted (order));
        Assert.IsFalse (ClientTransaction.Current.IsEnlisted (order));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot enlist the domain object 'Order|5682f032-2f0b-494b-a31c-"
        + "c97f02b89c36|System.Guid' in this transaction, because it is already bound to another transaction.")]
    public void Enlist_InDifferentTransaction ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      newTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void Enlist_InBindingTransaction ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      _bindingTransaction.EnlistDomainObject (order);
    }

    [Test]
    public void GetSetValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.OrderNumber = 12;
      Assert.AreEqual (12, order.OrderNumber);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsNotNull (order.OrderTicket);
      Assert.IsNotNull (order.OrderItems[0]);
    }

    [Test]
    public void LoadedRelatedObjects_AreBound ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.IsTrue (order.OrderTicket.HasBindingTransaction);
      Assert.AreSame (_bindingTransaction, order.OrderTicket.GetBindingTransaction());
      Assert.IsTrue (order.OrderItems[0].HasBindingTransaction);
      Assert.AreSame (_bindingTransaction, order.OrderItems[0].GetBindingTransaction());
    }

    [Test]
    public void SetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderTicket orderTicket = NewBound<OrderTicket> ();
      order.OrderTicket = orderTicket;
      Assert.AreSame (orderTicket, order.OrderTicket);
    }

    [Test]
    public void InsertRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderItem orderItem = NewBound<OrderItem> ();
      order.OrderItems.Add (orderItem);
      Assert.AreSame (orderItem, order.OrderItems[order.OrderItems.Count - 1]);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void SetRelatedObject_UnboundValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        order.OrderTicket = OrderTicket.NewObject();
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void InsertRelatedObject_UnboundValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        order.OrderItems.Add (OrderItem.NewObject ());
      }
    }

    [Test]
    public void State ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Unchanged, order.State);
      ++order.OrderNumber;
      Assert.AreEqual (StateType.Changed, order.State);
    }

    [Test]
    public void RegisterForCommit ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      order.RegisterForCommit();
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void IsDiscarded ()
    {
      Order order = NewBound<Order> ();
      Assert.AreEqual (StateType.New, order.State);
      Assert.IsFalse (order.IsInvalid);
      order.Delete ();
      Assert.AreEqual (StateType.Invalid, order.State);
      Assert.IsTrue (order.IsInvalid);
    }

    [Test]
    public void DataContainer ()
    {
      Order order = NewBound<Order> ();
      DataContainer dc = order.InternalDataContainer;
      Assert.AreSame (_bindingTransaction, dc.ClientTransaction);
    }

    [Test]
    public void Delete ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.Delete();
      Assert.AreEqual (StateType.Deleted, order.State);
    }

    private T NewBound<T> (params object[] args)
    where T : DomainObject
    {
      return (T) LifetimeService.NewObject (_bindingTransaction, typeof (T), ParamList.CreateDynamic (args));
    }

    private T GetBound<T> (ObjectID id)
        where T : DomainObject
    {
      return (T) LifetimeService.GetObject (_bindingTransaction, id, true);
    }
  }
}
