using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  [TestFixture]
  public class SubTransactionCreationStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void RootToSubUnchanged ()
    {
      DomainObject obj = GetUnchanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughPropertyValue ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.OrderNumber,
            obj.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjects ()
    {
      Order obj = GetChangedThroughRelatedObjects ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.OrderItems.Count,
           obj.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectRealSide ()
    {
      Computer obj = GetChangedThroughRelatedObjectRealSide ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.Employee,
            obj.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectVirtualSide ()
    {
      Employee obj = GetChangedThroughRelatedObjectVirtualSide ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.Computer,
            obj.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubNewUnchanged ()
    {
      DomainObject obj = GetNewUnchanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubNewChanged ()
    {
      DomainObject obj = GetNewChanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubDeleted ()
    {
      Order obj = GetDeleted ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' could not be found.")]
    public void RootToSubDeletedThrowsWhenReloadingTheObject ()
    {
      Order obj = GetDeleted ();
      ObjectID id = obj.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
        Order.GetObject (id);
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeleted ()
    {
      Client deleted = Client.GetObject (DomainObjectIDs.Client1);
      Location obj = GetUnidirectionalWithDeleted ();
      Assert.AreEqual (StateType.Deleted, deleted.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.IsTrue (deleted.IsDiscarded);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
      Assert.AreEqual (StateType.Deleted, deleted.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid' could not be found.")]
    public void RootToSubUnidirectionalWithDeletedThrowsWhenAccessingTheObject ()
    {
      Location obj = GetUnidirectionalWithDeleted ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Client client = obj.Client;
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeletedNew ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Client|.*|System.Guid' could not be found.", MatchType = MessageMatch.Regex)]
    public void RootToSubUnidirectionalWithDeletedNewThrowsWhenAccessingTheObject ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Client client = obj.Client;
      }
    }

    [Test]
    public void RootToSubDiscarded ()
    {
      DomainObject obj = GetDiscarded ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.IsTrue (obj.IsDiscarded);
    }
  }
}