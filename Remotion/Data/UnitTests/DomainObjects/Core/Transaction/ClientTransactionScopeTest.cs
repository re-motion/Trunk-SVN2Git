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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionScopeTest
  {
    private ClientTransactionScope _outermostScope;

    [SetUp]
    public void SetUp ()
    {
      ClientTransactionScope.ResetActiveScope();
      _outermostScope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [TearDown]
    public void TearDown ()
    {
      if (ClientTransactionScope.ActiveScope != null)
      {
        Assert.AreSame (_outermostScope, ClientTransactionScope.ActiveScope);
        _outermostScope.Leave();
      }
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      using (clientTransaction.EnterNonDiscardingScope())
      {
        Assert.AreSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void EnterNullScopeSetsNullTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
        using (ClientTransactionScope.EnterNullScope())
        {
          Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        }
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      }
    }

    [Test]
    public void ActiveScope ()
    {
      _outermostScope.Leave();
      Assert.IsNull (ClientTransactionScope.ActiveScope);
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.IsNotNull (ClientTransactionScope.ActiveScope);
        Assert.AreSame (scope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();
      ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;

      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (ClientTransactionScope scope1 = clientTransaction1.EnterNonDiscardingScope())
      {
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
        Assert.AreSame (scope1, ClientTransactionScope.ActiveScope);

        using (ClientTransactionScope scope2 = clientTransaction2.EnterNonDiscardingScope())
        {
          Assert.AreSame (scope2, ClientTransactionScope.ActiveScope);
          Assert.AreSame (clientTransaction2, ClientTransactionScope.CurrentTransaction);
        }
        Assert.AreSame (scope1, ClientTransactionScope.ActiveScope);
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
        {
          Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
        }
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      }
    }

    [Test]
    public void ScopeCreatesTransactionWithDefaultCtor ()
    {
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.IsNotNull (ClientTransactionScope.CurrentTransaction);
        Assert.AreNotSame (original, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void ScopeHasTransactionProperty ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope())
        {
          Assert.AreSame (innerTransaction, inner.ScopedTransaction);
          Assert.AreSame (outerTransaction, outer.ScopedTransaction);
        }
      }
    }

    [Test]
    public void ScopeHasTransactionProperty_FromITransactionScopeInterface ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope())
        {
          Assert.AreSame (innerTransaction, ((ITransactionScope) inner).ScopedTransaction.To<ClientTransaction>());
          Assert.AreSame (outerTransaction, ((ITransactionScope) outer).ScopedTransaction.To<ClientTransaction> ());
        }
      }
    }

    [Test]
    public void ScopeHasAutoRollbackBehavior ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (AutoRollbackBehavior.Discard, scope.AutoRollbackBehavior);
        scope.AutoRollbackBehavior = AutoRollbackBehavior.None;
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (AutoRollbackBehavior.Discard, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.None))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.AreEqual (AutoRollbackBehavior.Rollback, scope.AutoRollbackBehavior);
      }
    }

    private class TransactionEventCounter
    {
      public int Rollbacks = 0;
      public int Commits = 0;

      public TransactionEventCounter (ClientTransaction clientTransaction)
      {
        clientTransaction.RolledBack += ClientTransaction_RolledBack;
        clientTransaction.Committed += ClientTransaction_Committed;
      }

      private void ClientTransaction_RolledBack (object sender, ClientTransactionEventArgs args)
      {
        ++Rollbacks;
      }

      private void ClientTransaction_Committed (object sender, ClientTransactionEventArgs args)
      {
        ++Commits;
      }
    }

    [Test]
    public void NoAutoRollbackWhenNoneBehavior ()
    {
      var mock = new ClientTransactionMock();
      var eventCounter = new TransactionEventCounter (mock);

      using (mock.EnterScope (AutoRollbackBehavior.None))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderNumber = 0xbadf00d;
        order.OrderTicket = OrderTicket.NewObject();
        order.OrderItems.Add (OrderItem.NewObject());
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (mock.EnterScope (AutoRollbackBehavior.None))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.None))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderNumber = 0xbadf00d;

        scope.ScopedTransaction.Rollback();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
    }

    [Test]
    public void AutoRollbackWhenRollbackBehavior ()
    {
      var mock = new ClientTransactionMock();
      var eventCounter = new TransactionEventCounter (mock);

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderNumber = 0xbadf00d;
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderTicket = OrderTicket.NewObject();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderItems.Add (OrderItem.NewObject());
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs().Order1);
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback();

        order.OrderNumber = 0xbadf00d;
      }

      Assert.AreEqual (2, eventCounter.Rollbacks);
    }

    [Test]
    public void CommitAndRollbackOnScope ()
    {
      ClientTransaction transaction = ClientTransaction.CreateRootTransaction();
      var eventCounter = new TransactionEventCounter (transaction);
      using (ClientTransactionScope scope = transaction.EnterNonDiscardingScope())
      {
        Assert.AreEqual (0, eventCounter.Commits);
        Assert.AreEqual (0, eventCounter.Rollbacks);

        scope.Commit();

        Assert.AreEqual (1, eventCounter.Commits);
        Assert.AreEqual (0, eventCounter.Rollbacks);

        scope.Rollback();

        Assert.AreEqual (1, eventCounter.Commits);
        Assert.AreEqual (1, eventCounter.Rollbacks);

        transaction.Commit();

        Assert.AreEqual (2, eventCounter.Commits);
        Assert.AreEqual (1, eventCounter.Rollbacks);

        transaction.Rollback();

        Assert.AreEqual (2, eventCounter.Commits);
        Assert.AreEqual (2, eventCounter.Rollbacks);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveTwiceThrows ()
    {
      ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
      scope.Leave();
      scope.Leave();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveAndDisposeThrows ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        scope.Leave();
      }
    }

    [Test]
    public void NoAutoEnlisting ()
    {
      Order order = Order.GetObject (new DomainObjectIDs().Order1);
      Assert.IsTrue (ClientTransaction.Current.IsEnlisted (order));
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.IsFalse (ClientTransaction.Current.IsEnlisted (order));
      }
    }

    [Test]
    public void ResetScope ()
    {
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
      Assert.IsNotNull (ClientTransactionScope.ActiveScope);
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      ClientTransactionScope.ResetActiveScope();
      Assert.IsNull (ClientTransactionScope.ActiveScope);
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void AutoDiscardBehavior ()
    {
      var mockRepository = new MockRepository();

      var transaction = mockRepository.StrictMock<ClientTransaction> (
          new Dictionary<Enum, object> (), 
          new ClientTransactionExtensionCollection (), 
          new RootCollectionEndPointChangeDetectionStrategy (),
          new DictionaryBasedEnlistedDomainObjectManager());

      Expect.Call (transaction.EnterScope (AutoRollbackBehavior.Discard))
          .Return (
          (ClientTransactionScope)
          PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ClientTransactionScope), transaction, AutoRollbackBehavior.Discard));
      Expect.Call (transaction.Discard()).Return (true);

      mockRepository.ReplayAll();

      using (transaction.EnterScope (AutoRollbackBehavior.Discard))
      {
      }

      mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This ClientTransactionScope is not the active scope. Leave the active scope before leaving this one.")]
    public void LeaveNonActiveScopeThrows ()
    {
      try
      {
        using (ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.Rollback))
        {
          ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
        }
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope(); // for TearDown
      }
    }

    [Test]
    public void ITransactionScope_IsActiveScope ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      try
      {
        ITransactionScope outerScope = clientTransaction.EnterDiscardingScope();
        Assert.That (outerScope.IsActiveScope, Is.True);

        using (ClientTransactionScope innerScope = clientTransaction.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That (outerScope.IsActiveScope, Is.False);
          Assert.That (((ITransactionScope) innerScope).IsActiveScope, Is.True);
        }

        Assert.That (outerScope.IsActiveScope, Is.True);
        outerScope.Leave ();
        Assert.That (outerScope.IsActiveScope, Is.False);
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope(); // for TearDown
      }
    }
  }
}
