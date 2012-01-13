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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCreationStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void RootToSubUnchanged ()
    {
      DomainObject obj = GetUnchanged ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughPropertyValue ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        Assert.AreEqual (obj.OrderNumber, obj.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int> ());
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjects ()
    {
      Order obj = GetChangedThroughRelatedObjects ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        Assert.AreEqual (obj.OrderItems.Count, obj.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count);
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectRealSide ()
    {
      Computer obj = GetChangedThroughRelatedObjectRealSide ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        Assert.AreEqual (obj.Employee, obj.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> ());
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectVirtualSide ()
    {
      Employee obj = GetChangedThroughRelatedObjectVirtualSide ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        Assert.AreEqual (obj.Computer, obj.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> ());
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubNewUnchanged ()
    {
      DomainObject obj = GetNewUnchanged ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubNewChanged ()
    {
      DomainObject obj = GetNewChanged ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubDeleted ()
    {
      Order obj = GetDeleted ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsInvalid);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException),
        ExpectedMessage = "Object 'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' is invalid in this transaction.")]
    public void RootToSubDeletedThrowsWhenReloadingTheObject ()
    {
      Order obj = GetDeleted ();
      ObjectID id = obj.ID;
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsInvalid);
        Order.GetObject (id);
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeleted ()
    {
      Client deleted = Client.GetObject (DomainObjectIDs.Client1);
      Location obj = GetUnidirectionalWithDeleted ();
      Assert.AreEqual (StateType.Deleted, deleted.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.IsTrue (deleted.IsInvalid);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
      Assert.AreEqual (StateType.Deleted, deleted.State);
    }

    [Test]
    [Ignore ("TODO 4584")]
    public void RootToSubUnidirectional_WithDeleted_ReturnsInvalidObject ()
    {
      Location obj = GetUnidirectionalWithDeleted ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (obj.Client.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeletedNew ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    [Ignore ("TODO 4584")]
    public void RootToSubUnidirectional_WithDeletedNew_ReturnsInvalidObject ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (obj.Client.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void RootToSubDiscarded ()
    {
      DomainObject obj = GetInvalid ();
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsInvalid);
      }
      Assert.IsTrue (obj.IsInvalid);
    }
  }
}
