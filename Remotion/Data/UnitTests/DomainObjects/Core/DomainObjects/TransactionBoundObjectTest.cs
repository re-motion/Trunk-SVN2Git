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
      Assert.That (order.HasBindingTransaction, Is.True);
      Assert.That (order.GetBindingTransaction(), Is.SameAs (_bindingTransaction));
    }

    [Test]
    public void GetBoundObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.That (order.HasBindingTransaction, Is.True);
      Assert.That (order.GetBindingTransaction(), Is.SameAs (_bindingTransaction));
    }

    [Test]
    public void GetBoundObject_WithCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.That (order.HasBindingTransaction, Is.True);
        Assert.That (order.GetBindingTransaction(), Is.SameAs (_bindingTransaction));
        Assert.That (DomainObjectIDs.Order1.GetObject<Order> (), Is.Not.SameAs (order));
      }
    }

    [Test]
    public void CanBeUsedInTransaction ()
    {
      Order order = GetBound<Order>(DomainObjectIDs.Order1);
      Assert.That (_bindingTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void CanBeUsedInTransaction_WithCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Order order = GetBound<Order> (DomainObjectIDs.Order1);
        Assert.That (_bindingTransaction.IsEnlisted (order), Is.True);
        Assert.That (ClientTransaction.Current.IsEnlisted (order), Is.False);
      }
    }

    [Test]
    public void GetSetValue ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.OrderNumber = 12;
      Assert.That (order.OrderNumber, Is.EqualTo (12));
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.That (order.OrderTicket, Is.Not.Null);
      Assert.That (order.OrderItems[0], Is.Not.Null);
    }

    [Test]
    public void LoadedRelatedObjects_AreBound ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      Assert.That (order.OrderTicket.HasBindingTransaction, Is.True);
      Assert.That (order.OrderTicket.GetBindingTransaction(), Is.SameAs (_bindingTransaction));
      Assert.That (order.OrderItems[0].HasBindingTransaction, Is.True);
      Assert.That (order.OrderItems[0].GetBindingTransaction(), Is.SameAs (_bindingTransaction));
    }

    [Test]
    public void SetRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderTicket orderTicket = NewBound<OrderTicket> ();
      order.OrderTicket = orderTicket;
      Assert.That (order.OrderTicket, Is.SameAs (orderTicket));
    }

    [Test]
    public void InsertRelatedObject ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);

      OrderItem orderItem = NewBound<OrderItem> ();
      order.OrderItems.Add (orderItem);
      Assert.That (order.OrderItems[order.OrderItems.Count - 1], Is.SameAs (orderItem));
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
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      ++order.OrderNumber;
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
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
      Assert.That (order.State, Is.EqualTo (StateType.New));
      Assert.That (order.IsInvalid, Is.False);
      order.Delete ();
      Assert.That (order.State, Is.EqualTo (StateType.Invalid));
      Assert.That (order.IsInvalid, Is.True);
    }

    [Test]
    public void DataContainer ()
    {
      Order order = NewBound<Order> ();
      DataContainer dc = order.InternalDataContainer;
      Assert.That (dc.ClientTransaction, Is.SameAs (_bindingTransaction));
    }

    [Test]
    public void Delete ()
    {
      Order order = GetBound<Order> (DomainObjectIDs.Order1);
      order.Delete();
      Assert.That (order.State, Is.EqualTo (StateType.Deleted));
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
