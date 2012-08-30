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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
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
    public void GetObjectReference ()
    {
      var boundResult = LifetimeService.GetObjectReference (_bindingTransaction, DomainObjectIDs.Order1);
      
      Assert.That (boundResult.HasBindingTransaction, Is.True);
      Assert.That (boundResult.GetBindingTransaction (), Is.SameAs (_bindingTransaction));
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
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot insert DomainObject 'OrderItem|90a931d6-d7e9-4a6c-9109-53f5e3b481ee|System.Guid' into collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|442d21bd-ff55-4168-b75f-cc0a29ebf8c5|System.Guid'. The objects do not belong to the same ClientTransaction. The OrderItem object is "
        + "bound to a BindingClientTransaction.", 
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
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        "Cannot insert DomainObject 'OrderItem|657828f9-fcc4-4b6f-bbb6-86054f437105|System.Guid' into collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|8da27c17-d110-4ecb-92e4-e7984830a2ed|System.Guid'. The objects do not belong to the same ClientTransaction. The Order object "
        + "owning the property is bound to a BindingClientTransaction.",
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
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot insert DomainObject 'OrderItem|5a0e9087-de57-49f4-a0d5-5253b687ea35|System.Guid' into collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|fcfa221d-7919-4241-bce2-0f0f9d701891|System.Guid'. The objects do not belong to the same ClientTransaction. The OrderItem object "
        + "is bound to a BindingClientTransaction. The Order object owning the property is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void InsertBoundObject_IntoCollectionOfBoundObject_InOtherTx ()
    {
      var order = NewBound<Order> ();
      var orderItem = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateBindingTransaction ());
      order.OrderItems.Add (orderItem);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\n"
        + "Parameter name: domainObject",
        MatchType = MessageMatch.Regex)]
    public void RemoveBoundObject_FromCollectionOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);
        var orderItem = GetBound<OrderItem> (order.OrderItems[0].ID);
        order.OrderItems.Remove (orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\n"
        + "Parameter name: domainObject",
        MatchType = MessageMatch.Regex)]
    public void RemoveUnboundObject_FromCollectionOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = GetBound<Order> (DomainObjectIDs.Order1);
        var orderItem = OrderItem.GetObject (order.OrderItems[0].ID);
        order.OrderItems.Remove (orderItem);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The object to be removed has the same ID as an object in this collection, but is a different object reference.\r\n"
        + "Parameter name: domainObject",
        MatchType = MessageMatch.Regex)]
    public void RemoveBoundObject_FromCollectionOfBoundObject_InOtherTx ()
    {
      var order = GetBound<Order> (DomainObjectIDs.Order1);
      var orderItem = DomainObjectMother.GetObjectInTransaction<OrderItem> (ClientTransaction.CreateBindingTransaction (), order.OrderItems[0].ID);
      order.OrderItems.Remove (orderItem);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot put DomainObject 'OrderItem|6f4fb909-5fee-4332-9bb1-c302041927b0|System.Guid' into the collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|e5f2b5ea-4979-4403-9388-6de10ea08a71|System.Guid'. The objects do not belong to the same ClientTransaction. The OrderItem object is "
        + "bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceBoundObject_IntoCollectionOfUnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var orderItem = NewBound<OrderItem> ();
        var order = Order.NewObject ();
        order.OrderItems.Add (DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.Current));
        order.OrderItems[0] = orderItem;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot put DomainObject 'OrderItem|1a89a208-f70f-4d0c-910d-0be05a63a24e|System.Guid' into the collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|57041610-340f-4cfb-b318-5cfd75226613|System.Guid'. The objects do not belong to the same ClientTransaction. The Order object "
        + "owning the property is bound to a BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceUnboundObject_IntoCollectionOfBoundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var order = NewBound<Order> ();
        order.OrderItems.Add (DomainObjectMother.CreateObjectInTransaction<OrderItem> (order.GetBindingTransaction()));
        var orderItem = OrderItem.NewObject ();
        order.OrderItems[0] = orderItem;
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        "Cannot put DomainObject 'OrderItem|067c0750-8985-4419-a76e-7e4db6bf68e2|System.Guid' into the collection of property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject "
        + "'Order|a50f8e9f-0444-4f23-80fd-10012dcb10c3|System.Guid'. The objects do not belong to the same ClientTransaction. The OrderItem object is "
        + "bound to a BindingClientTransaction. The Order object owning the property is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void ReplaceBoundObject_IntoCollectionOfBoundObject_InOtherTx ()
    {
      var order = NewBound<Order> ();
      order.OrderItems.Add (DomainObjectMother.CreateObjectInTransaction<OrderItem> (order.GetBindingTransaction()));
      var orderItem = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateBindingTransaction ());
      order.OrderItems[0] = orderItem;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order.Official' of DomainObject 'Order[^']*' cannot be set to DomainObject 'Official[^']*'. The objects do not belong to "
        + "the same ClientTransaction. The Official object is bound to a BindingClientTransaction.",
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
        + "the same ClientTransaction. The Official object is bound to a BindingClientTransaction. The Order object "
        + "owning the property is also bound, but to a different BindingClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void SetBoundObject_IntoRelationOfBoundObject_InOtherTx ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var official = NewBound<Official>();
        var order = DomainObjectMother.CreateObjectInTransaction<Order> (ClientTransaction.CreateBindingTransaction());
        order.Official = official;
      }
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType ()
    {
      var newTransaction = _bindingTransaction.CreateEmptyTransactionOfSameType (false);
      Assert.That (newTransaction, Is.Not.SameAs (_bindingTransaction));
      Assert.That (newTransaction.GetType (), Is.EqualTo (_bindingTransaction.GetType ()));
      Assert.That (
          ClientTransactionTestHelper.GetPersistenceStrategy (newTransaction).GetType (),
          Is.EqualTo (ClientTransactionTestHelper.GetPersistenceStrategy (_bindingTransaction).GetType ()));
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType_CopyInvalidObjectInformation_False ()
    {
      var order = _bindingTransaction.Execute (() => Order.NewObject());
      _bindingTransaction.Execute (order.Delete);

      var newTransaction = _bindingTransaction.CreateEmptyTransactionOfSameType (false);
      Assert.That (newTransaction.IsEnlisted (order), Is.False);
      Assert.That (newTransaction.IsInvalid (order.ID), Is.False);
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType_CopyInvalidObjectInformation_True ()
    {
      var order = _bindingTransaction.Execute (() => Order.NewObject ());
      _bindingTransaction.Execute (order.Delete);

      Assert.That (
          () => _bindingTransaction.CreateEmptyTransactionOfSameType (true),
          Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
              string.Format (
                  "Cannot enlist the domain object {0} in this binding transaction, because it has originally been loaded in another transaction.",
                  order.ID)));
    }

    private T NewBound<T> ()
        where T : DomainObject
    {
      return DomainObjectMother.CreateObjectInTransaction<T> (_bindingTransaction);
    }

    private T GetBound<T> (ObjectID id)
        where T : DomainObject
    {
      return DomainObjectMother.GetObjectInTransaction<T> (_bindingTransaction, id);
    }
  }
}
