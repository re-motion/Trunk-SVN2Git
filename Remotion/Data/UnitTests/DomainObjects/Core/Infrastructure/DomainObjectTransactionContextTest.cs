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
    private Order _loadedOrder1;
    private Order _notYetLoadedOrder2;
    private Order _newOrder;
    
    private DomainObjectTransactionContext _loadedOrder1Context;
    private DomainObjectTransactionContext _notYetLoadedOrder2Context;
    private DomainObjectTransactionContext _newOrderContext;
    private DomainObjectTransactionContext _invalidContext;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedOrder1 = Order.GetObject (DomainObjectIDs.Order1);
      _notYetLoadedOrder2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      ClientTransactionMock.EnlistDomainObject (_notYetLoadedOrder2);
      _newOrder = Order.NewObject();

      _loadedOrder1Context = new DomainObjectTransactionContext (_loadedOrder1, ClientTransactionMock);
      _notYetLoadedOrder2Context = new DomainObjectTransactionContext (_notYetLoadedOrder2, ClientTransactionMock);
      _newOrderContext = new DomainObjectTransactionContext (_newOrder, ClientTransactionMock);

      _invalidContext = new DomainObjectTransactionContext (_newOrder, ClientTransaction.CreateRootTransaction());
    }

    [Test]
    public void Initialization()
    {
      Assert.That (_loadedOrder1Context.DomainObject, Is.SameAs (_loadedOrder1));
      Assert.That (_loadedOrder1Context.ClientTransaction, Is.SameAs (ClientTransactionMock));
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
        DeleteOrder(_loadedOrder1);
        ClientTransaction.Current.Commit ();

        Assert.That (_loadedOrder1.IsDiscarded, Is.True);
        Assert.That (_loadedOrder1Context.IsDiscarded, Is.False);
      }
    }

    [Test]
    public void IsDiscarded_True ()
    {
      _newOrder.Delete ();
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
      _newOrder.Delete ();
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.Discarded));
    }

    [Test]
    public void State_NotYetLoaded ()
    {
      Assert.That (_notYetLoadedOrder2Context.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void State_FromDataContainer ()
    {
      Assert.That (_newOrderContext.State, Is.EqualTo (StateType.New));
      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Unchanged));

      _loadedOrder1.OrderNumber = 2;

      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void State_FromDataContainer_WithChangedRelation ()
    {
      _loadedOrder1.OrderItems.Clear();

      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Changed));
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
      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Unchanged));
      _loadedOrder1Context.MarkAsChanged ();
      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void MarkAsChanged_Changed ()
    {
      _loadedOrder1.OrderNumber = 2;

      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Changed));
      _loadedOrder1Context.MarkAsChanged ();
      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Changed));
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
      DeleteOrder (_loadedOrder1);

      Assert.That (_loadedOrder1Context.State, Is.EqualTo (StateType.Deleted));
      _loadedOrder1Context.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void MarkAsChanged_Discarded ()
    {
      _newOrder.Delete ();

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
      var timestamp = _loadedOrder1Context.Timestamp;
      Assert.That (timestamp, Is.Not.Null);
      Assert.That (timestamp, Is.SameAs (_loadedOrder1.Timestamp));
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
      _newOrder.Delete ();
      Dev.Null = _newOrderContext.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void Timestamp_InvalidTransaction ()
    {
      Dev.Null = _invalidContext.Timestamp;
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[_notYetLoadedOrder2.ID], Is.Null);

      _notYetLoadedOrder2Context.EnsureDataAvailable ();

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[_notYetLoadedOrder2.ID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[_notYetLoadedOrder2.ID].DomainObject, Is.SameAs (_notYetLoadedOrder2));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void EnsureDataAvailable_Discarded ()
    {
      _newOrder.Delete ();
      _newOrderContext.EnsureDataAvailable ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException))]
    public void EnsureDataAvailable_InvalidTransaction ()
    {
      _invalidContext.EnsureDataAvailable();
    }

    [Test]
    public void Execute_Action_RunsDelegate ()
    {
      bool delegateRun = false;
      DomainObject objParameter = null;
      ClientTransaction txParameter = null;

      Action<DomainObject, ClientTransaction> action = (obj, tx) =>
      {
        delegateRun = true;
        objParameter = obj;
        txParameter = tx;
      };

      _loadedOrder1Context.Execute (action);

      Assert.That (delegateRun, Is.True);
      Assert.That (objParameter, Is.SameAs (_loadedOrder1Context.DomainObject));
      Assert.That (txParameter, Is.SameAs (_loadedOrder1Context.ClientTransaction));
    }

    [Test]
    public void Execute_Action_SetsCurrentTx ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        Assert.That (ClientTransaction.Current, Is.Null);

        ClientTransaction currentInDelegate = null;
        Action<DomainObject, ClientTransaction> action = (obj, tx) => currentInDelegate = ClientTransaction.Current;

        _loadedOrder1Context.Execute (action);

        Assert.That (currentInDelegate, Is.SameAs (_loadedOrder1Context.ClientTransaction));
      }
    }

    [Test]
    public void Execute_Func_RunsDelegate ()
    {
      DomainObject objParameter = null;
      ClientTransaction txParameter = null;

      Func<DomainObject, ClientTransaction, int> func = (obj, tx) =>
      {
        objParameter = obj;
        txParameter = tx;
        return 17;
      };

      var result = _loadedOrder1Context.Execute (func);

      Assert.That (result, Is.EqualTo (17));
      Assert.That (objParameter, Is.SameAs (_loadedOrder1Context.DomainObject));
      Assert.That (txParameter, Is.SameAs (_loadedOrder1Context.ClientTransaction));
    }

    [Test]
    public void Execute_Func_SetsCurrentTx ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransaction.Current, Is.Null);

        ClientTransaction currentInDelegate = null;
        Func<DomainObject, ClientTransaction, int> func = (obj, tx) =>
        {
          currentInDelegate = ClientTransaction.Current;
          return 4;
        };

        _loadedOrder1Context.Execute (func);

        Assert.That (currentInDelegate, Is.SameAs (_loadedOrder1Context.ClientTransaction));
      }
    }

    private void DeleteOrder (Order order)
    {
      while (order.OrderItems.Count > 0)
        order.OrderItems[0].Delete ();

      order.OrderTicket.Delete ();
      order.Delete ();
    }
  }
}
