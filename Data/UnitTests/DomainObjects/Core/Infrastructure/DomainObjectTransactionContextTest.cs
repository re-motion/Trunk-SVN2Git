/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectTransactionContextTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private Order _newOrder;
    
    private ClientTransaction _otherTransaction;
    
    private DomainObjectTransactionContext _order1Context;
    private DomainObjectTransactionContext _newOrderContext;
    private DomainObjectTransactionContext _invalidContext;

    public override void SetUp ()
    {
      base.SetUp ();
      _otherTransaction = ClientTransaction.CreateRootTransaction ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _otherTransaction.EnlistDomainObject (_order1);

      using (_otherTransaction.EnterNonDiscardingScope ())
      {
        _newOrder = Order.NewObject();
      }

      _order1Context = new DomainObjectTransactionContext (_order1, _otherTransaction);
      _newOrderContext = new DomainObjectTransactionContext (_newOrder, _otherTransaction);

      _invalidContext = new DomainObjectTransactionContext (_newOrder, ClientTransaction.Current);
    }

    [Test]
    public void Initialization()
    {
      Assert.That (_order1Context.DomainObject, Is.SameAs (_order1));
      Assert.That (_order1Context.AssociatedTransaction, Is.SameAs (_otherTransaction));
    }

    [Test]
    public void CanBeUsedInTransaction_True ()
    {
      Assert.That (_order1Context.CanBeUsedInTransaction, Is.True);
    }

    [Test]
    public void CanBeUsedInTransaction_False ()
    {
      Assert.That (_invalidContext.CanBeUsedInTransaction, Is.False);
    }

    [Test]
    public void CheckIfRightTransaction_Works ()
    {
      _order1Context.CheckIfRightTransaction ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object 'Order|.*|System.Guid' cannot be used in the "
        + "given transaction as it was loaded or created in another transaction. Enter a scope for the transaction, or call EnlistInTransaction to "
        + "enlist the object in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
          MatchType = MessageMatch.Regex)]
    public void CheckIfRightTransaction_Fails ()
    {
      _invalidContext.CheckIfRightTransaction ();
    }

    [Test]
    public void IsDiscarded_False()
    {
      Assert.That (_newOrderContext.IsDiscarded, Is.False);
    }

    [Test]
    public void IsDiscarded_False_DiscardedInCurrentTransaction ()
    {
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope())
      {
        while (_order1.OrderItems.Count > 0)
          _order1.OrderItems[0].Delete ();

        _order1.OrderTicket.Delete ();
        _order1.Delete ();
        ClientTransaction.Current.Commit ();
        Assert.That (_order1.IsDiscarded, Is.True);

        Assert.That (_order1Context.IsDiscarded, Is.False);
      }
    }

    [Test]
    public void IsDiscarded_True ()
    {
      using (_newOrderContext.AssociatedTransaction.EnterNonDiscardingScope ())
      {
        _newOrder.Delete();
      }
      Assert.That (_newOrderContext.IsDiscarded, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void IsDiscarded_InvalidTransaction ()
    {
      Dev.Null = _invalidContext.IsDiscarded;
    }

    [Test]
    public void State_IsDiscarded ()
    {
      using (_newOrderContext.AssociatedTransaction.EnterNonDiscardingScope ())
      {
        _newOrder.Delete ();
      }
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.Discarded));
    }

    [Test]
    public void State_FromDataContainer ()
    {
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.New));
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Unchanged));
      using (_newOrderContext.AssociatedTransaction.EnterNonDiscardingScope ())
      {
        _order1.OrderNumber = 2;
      }
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void State_FromDataContainer_WithChangedRelation ()
    {
      using (_newOrderContext.AssociatedTransaction.EnterNonDiscardingScope ())
      {
        _order1.OrderItems.Clear();
      }
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void State_InvalidTransaction ()
    {
      Dev.Null = _invalidContext.State;
    }
  }
}