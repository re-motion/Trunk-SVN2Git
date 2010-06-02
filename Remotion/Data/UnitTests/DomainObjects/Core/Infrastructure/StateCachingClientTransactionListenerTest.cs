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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class StateCachingClientTransactionListenerTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private StateCachingClientTransactionListener _cachingListener;
    private Order _existingOrder;
    private Order _newOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ();
      _cachingListener = new StateCachingClientTransactionListener (_transaction);

      _existingOrder = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _newOrder = _transaction.Execute (() => Order.NewObject ());

      ClientTransactionTestHelper.AddListener (_transaction, _cachingListener);
    }
    
    [Test]
    public void GetState ()
    {
      var existingState = _cachingListener.GetState (_existingOrder.ID);
      var newState = _cachingListener.GetState (_newOrder.ID);

      Assert.That (existingState, Is.EqualTo (StateType.Unchanged));
      Assert.That (newState, Is.EqualTo (StateType.New));
    }

    [Test]
    public void GetState_Twice ()
    {
      var existingState1 = _cachingListener.GetState (_existingOrder.ID);
      var existingState2 = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (existingState1, Is.EqualTo (StateType.Unchanged));
      Assert.That (existingState2, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterPropertyChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Execute (() => _existingOrder.OrderNumber++);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterRealObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Execute (() => _existingOrder.Customer = null);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterVirtualObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Execute (() => _existingOrder.OrderTicket = null);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterCollectionEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Execute (() => _existingOrder.OrderItems.RemoveAt (0));
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterUnload ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      UnloadService.UnloadData (_transaction, _existingOrder.ID, UnloadTransactionMode.ThisTransactionOnly);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetState_Invalidated_AfterReload ()
    {
      UnloadService.UnloadData (_transaction, _existingOrder.ID, UnloadTransactionMode.ThisTransactionOnly);
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);
      
      _transaction.EnsureDataAvailable (_existingOrder.ID);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterDelete ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      LifetimeService.DeleteObject (_transaction, _existingOrder);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void GetState_Invalidated_AfterDiscard ()
    {
      var stateBeforeChange = _cachingListener.GetState (_newOrder.ID);

      LifetimeService.DeleteObject (_transaction, _newOrder);
      var stateAfterChange = _cachingListener.GetState (_newOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.New));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void GetState_Invalidated_AfterRollback ()
    {
      _transaction.Execute (() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Rollback ();
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Changed));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterCommit ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.EnsureDataAvailable (_existingOrder.ID);
      subTransaction.Execute (() => _existingOrder.OrderNumber++);

      var cachingListener = new StateCachingClientTransactionListener (subTransaction);
      ClientTransactionTestHelper.AddListener (subTransaction, cachingListener);
      var stateBeforeChange = cachingListener.GetState (_existingOrder.ID);

      subTransaction.Commit ();
      var stateAfterChange = cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Changed));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterCommitOfSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.EnsureDataAvailable (_existingOrder.ID);
      subTransaction.Execute (() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      subTransaction.Commit ();
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }
  }
}