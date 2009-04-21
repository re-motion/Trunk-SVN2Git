// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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