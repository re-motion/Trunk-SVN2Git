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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class PersistingSubTransactionTest : ClientTransactionBaseTest
  {
    private Order _loadedOrder;

    public override void SetUp ()
    {
      base.SetUp ();
      SetDatabaseModifyable ();

      _loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (_loadedOrder.OrderNumber, Is.EqualTo (1));
      Assert.That (_loadedOrder.OrderItems.Count, Is.EqualTo (2));
    }

    [Test]
    public void SubtransactionPersistsIntoParentTransaction ()
    {
      using (new PersistingSubTransaction (ClientTransactionMock).EnterDiscardingScope ())
      {
        _loadedOrder.OrderNumber = 15;
        _loadedOrder.OrderItems.Add (OrderItem.NewObject ());
        ClientTransaction.Current.Commit ();
      }

      Assert.That (_loadedOrder.OrderNumber, Is.EqualTo (15));
      Assert.That (_loadedOrder.OrderItems.Count, Is.EqualTo (3));
    }

    [Test]
    public void SubtransactionPersistsIntoDatabase ()
    {
      using (new PersistingSubTransaction (ClientTransactionMock).EnterDiscardingScope ())
      {
        _loadedOrder.OrderNumber = 15;
        _loadedOrder.OrderItems.Add (OrderItem.NewObject ());
        ClientTransaction.Current.Commit ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInDb = Order.GetObject (DomainObjectIDs.Order1);
        Assert.That (orderInDb.OrderNumber, Is.EqualTo (15));
        Assert.That (orderInDb.OrderItems.Count, Is.EqualTo (3));
      }
    }

    [Test]
    [Ignore ("TODO: COMMONS-950")]
    public void PersistedObjects_UnchangedInParentTransaction ()
    {
      using (new PersistingSubTransaction (ClientTransactionMock).EnterDiscardingScope ())
      {
        _loadedOrder.OrderNumber = 15;
        _loadedOrder.OrderItems.Add (OrderItem.NewObject ());
        ClientTransaction.Current.Commit ();
      }

      Assert.That (_loadedOrder.OrderNumber, Is.EqualTo (15));
      Assert.That (_loadedOrder.OrderItems.Count, Is.EqualTo (3));
      Assert.That (_loadedOrder.State, Is.EqualTo (StateType.Unchanged));
      
      Assert.That (_loadedOrder.OrderItems[0], Is.EqualTo (StateType.Unchanged));
      Assert.That (_loadedOrder.OrderItems[1], Is.EqualTo (StateType.Unchanged));
      Assert.That (_loadedOrder.OrderItems[2], Is.EqualTo (StateType.Unchanged));
      
      ClientTransaction.Current.Commit ();
    }

    [Test]
    public void Timestamps ()
    {
      object timestampAfterCommit;
      using (new PersistingSubTransaction (ClientTransactionMock).EnterDiscardingScope ())
      {
        _loadedOrder.OrderNumber = 15;
        _loadedOrder.OrderItems.Add (OrderItem.NewObject ());
        var timestampBeforeCommit = _loadedOrder.Timestamp;
        ClientTransaction.Current.Commit ();
        timestampAfterCommit = _loadedOrder.Timestamp;
        Assert.That (timestampAfterCommit, Is.Not.EqualTo (timestampBeforeCommit));
      }

      Assert.That (_loadedOrder.Timestamp, Is.EqualTo (timestampAfterCommit));
    }
  }
}