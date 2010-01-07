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
      Assert.That (_order1Context.ClientTransaction, Is.SameAs (_otherTransaction));
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
      DeleteNewOrder ();
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
      DeleteNewOrder();
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.Discarded));
    }

    [Test]
    public void State_FromDataContainer ()
    {
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.New));
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Unchanged));
      using (_order1Context.ClientTransaction.EnterNonDiscardingScope ())
      {
        _order1.OrderNumber = 2;
      }
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void State_FromDataContainer_WithChangedRelation ()
    {
      using (_order1Context.ClientTransaction.EnterNonDiscardingScope ())
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

    [Test]
    public void MarkAsChanged_Unchanged()
    {
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Unchanged));
      _order1Context.MarkAsChanged ();
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void MarkAsChanged_Changed ()
    {
      using (_order1Context.ClientTransaction.EnterNonDiscardingScope ())
      {
        _order1.OrderNumber = 2;
      }

      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
      _order1Context.MarkAsChanged ();
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChanged_New ()
    {
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.New));
      _newOrderContext.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChanged_Deleted ()
    {
      DeleteOrder1 ();
      Assert.That (_order1Context.State, Is.EqualTo (StateType.Deleted));
      _order1Context.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void MarkAsChanged_Discarded ()
    {
      DeleteNewOrder ();
      _newOrderContext.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void MarkAsChanged_InvalidTransaction ()
    {
      _invalidContext.MarkAsChanged ();
    }

    [Test]
    public void Timestamp_LoadedObject()
    {
      var timestamp = _order1Context.Timestamp;
      Assert.That (timestamp, Is.Not.Null);
      Assert.That (timestamp, Is.SameAs (_order1.GetInternalDataContainerForTransaction (_otherTransaction).Timestamp));
    }

    [Test]
    public void Timestamp_NewObject ()
    {
      var timestamp = _newOrderContext.Timestamp;
      Assert.That (timestamp, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void Timestamp_Discarded ()
    {
      DeleteNewOrder ();
      Dev.Null = _newOrderContext.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void Timestamp_InvalidTransaction ()
    {
      Dev.Null = _invalidContext.Timestamp;
    }

    private void DeleteNewOrder ()
    {
      using (_newOrderContext.ClientTransaction.EnterNonDiscardingScope ())
      {
        _newOrder.Delete ();
      }
    }

    private void DeleteOrder1 ()
    {
      using (_order1Context.ClientTransaction.EnterNonDiscardingScope ())
      {
        _order1.Delete ();
      }
    }
  }
}
