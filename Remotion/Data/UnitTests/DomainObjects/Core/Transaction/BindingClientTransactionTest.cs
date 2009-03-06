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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

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

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot insert DomainObject 'OrderItem[^']*' at position 0 into " 
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be inserted is bound to a BindingClientTransaction.", 
        MatchType = MessageMatch.Regex)]
    public void InsertBoundObject_IntoCollectionOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var orderItem = NewBound<OrderItem>();
        var order = Order.NewObject();
        order.OrderItems.Add (orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot insert DomainObject 'OrderItem[^']*' at position 0 into "
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The Order object owning the collection is bound to a BindingClientTransaction.", 
        MatchType = MessageMatch.Regex)]
    public void InsertUnboundObject_IntoCollectionOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = NewBound<Order> ();
        var orderItem = OrderItem.NewObject ();
        order.OrderItems.Add (orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot insert DomainObject 'OrderItem[^']*' at position 0 into "
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be inserted is bound to a BindingClientTransaction. The Order object "
        + "owning the collection is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void InsertBoundObject_IntoCollectionOfBoundObject_InOtherTx ()
    {
      var order = NewBound<Order> ();
      var orderItem = NewObject<OrderItem> (ClientTransaction.CreateBindingTransaction ());
      order.OrderItems.Add (orderItem);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot remove DomainObject 'OrderItem[^']*' from "
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be removed is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void RemoveBoundObject_FromCollectionOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var orderItem = NewBound<OrderItem> ();
        var order = Order.NewObject ();
        order.OrderItems.ChangeDelegate.PerformRemove (order.OrderItems, orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot remove DomainObject 'OrderItem[^']*' from "
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The Order object owning the collection is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void RemoveUnboundObject_FromCollectionOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = NewBound<Order> ();
        var orderItem = OrderItem.NewObject ();
        order.OrderItems.ChangeDelegate.PerformRemove (order.OrderItems, orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot remove DomainObject 'OrderItem[^']*' from "
        + "collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be removed is bound to a BindingClientTransaction. The Order object "
        + "owning the collection is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void RemoveBoundObject_FromCollectionOfBoundObject_InOtherTx ()
    {
      var order = NewBound<Order> ();
      var orderItem = NewObject<OrderItem> (ClientTransaction.CreateBindingTransaction ());
      order.OrderItems.ChangeDelegate.PerformRemove (order.OrderItems, orderItem);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot replace DomainObject at position 0 with DomainObject "
        + "'OrderItem[^']*' in collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be inserted is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceBoundObject_IntoCollectionOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var orderItem = NewBound<OrderItem> ();
        var order = Order.NewObject ();
        order.OrderItems.Add (NewObject<OrderItem> (ClientTransaction.Current));
        order.OrderItems[0] = orderItem;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot replace DomainObject at position 0 with DomainObject "
        + "'OrderItem[^']*' in collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The Order object owning the collection is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceUnboundObject_IntoCollectionOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = NewBound<Order> ();
        order.OrderItems.Add (NewObject<OrderItem> (order.GetBindingTransaction()));
        var orderItem = OrderItem.NewObject ();
        order.OrderItems[0] = orderItem;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Cannot replace DomainObject at position 0 with DomainObject "
        + "'OrderItem[^']*' in collection of property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order[^']*'. The objects do not "
        + "belong to the same ClientTransaction. The OrderItem object to be inserted is bound to a BindingClientTransaction. The Order object "
        + "owning the collection is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceBoundObject_IntoCollectionOfBoundObject_InOtherTx ()
    {
      var order = NewBound<Order> ();
      order.OrderItems.Add (NewObject<OrderItem> (order.GetBindingTransaction()));
      var orderItem = NewObject<OrderItem> (ClientTransaction.CreateBindingTransaction ());
      order.OrderItems[0] = orderItem;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order.Official' of DomainObject 'Order[^']*' cannot be set to DomainObject 'Official[^']*'. The objects do not belong to "
        + "the same ClientTransaction. The Official object to be set into the property is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void SetBoundObject_IntoRelationOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var official = NewBound<Official> ();
        var order = Order.NewObject ();
        order.Official = official;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order.Official' of DomainObject 'Order[^']*' cannot be set to DomainObject 'Official[^']*'. The objects do not belong to "
        + "the same ClientTransaction. The Order object owning the property is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void SetUnboundObject_IntoRelationOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var official = Official.NewObject ();
        var order = NewBound<Order> ();
        order.Official = official;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order.Official' of DomainObject 'Order[^']*' cannot be set to DomainObject 'Official[^']*'. The objects do not belong to "
        + "the same ClientTransaction. The Official object to be set into the property is bound to a BindingClientTransaction. The Order object "
        + "owning the property is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void SetBoundObject_IntoRelationOfBoundObject_InOtherTx ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var official = NewBound<Official>();
        var order = NewObject<Order> (ClientTransaction.CreateBindingTransaction());
        order.Official = official;
      }
    }

    private T NewBound<T> (params object[] args)
        where T : DomainObject
    {
      return NewObject<T> (_bindingTransaction, args);
    }

    private T NewObject<T> (ClientTransaction creatingTransaction, params object[] args)
        where T : DomainObject
    {
      using (creatingTransaction.EnterNonDiscardingScope ())
      {
        return (T) RepositoryAccessor.NewObject (typeof (T), ParamList.CreateDynamic (args));
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
  }
}
