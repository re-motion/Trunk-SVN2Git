using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DeleteDomainObjectWithManyToOneRelationTest : ClientTransactionBaseTest
  {
    private OrderItem _orderItem;
    private Order _order;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _order = _orderItem.Order;

      _eventReceiver = CreateEventReceiver ();
    }

    [Test]
    public void DeleteOrderItemEvents ()
    {
      _orderItem.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", _orderItem, null, "3. Relation changing event of order"),
      new CollectionChangeState (_order.OrderItems, _orderItem, "4. Removed event of order.OrderItems"),
      new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, null, "5. Relation changed event of order"),
      new ObjectDeletionState (_orderItem, "6. Deleted event of orderItem"),
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void OrderItemCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem") };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderItemCollectionCancelsRemoveEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            { 
              new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
              new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems") 
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _eventReceiver.CancelEventNumber = 3;

      try
      {
        _orderItem.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
              new CollectionChangeState (_order.OrderItems, _orderItem, "2. Removing event of order.OrderItems"),
              new RelationChangeState (_order, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", _orderItem, null, "3. Relation changing event of order")
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void Relations ()
    {
      int numberOfOrderItemsBeforeDelete = _order.OrderItems.Count;
      _orderItem.Delete ();

      Assert.IsNull (_orderItem.Order);
      Assert.AreEqual (numberOfOrderItemsBeforeDelete - 1, _order.OrderItems.Count);
      Assert.IsFalse (_order.OrderItems.Contains (_orderItem.ID));
			Assert.IsNull (_orderItem.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]);
      Assert.AreEqual (StateType.Changed, _order.State);
			Assert.AreEqual (StateType.Unchanged, _order.InternalDataContainer.State);
    }

    [Test]
    public void ChangePropertyBeforeDeletion ()
    {
      _orderItem.Order = null;
      _eventReceiver = CreateEventReceiver ();

      _orderItem.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
          {
            new ObjectDeletionState (_orderItem, "1. Deleting event of orderItem"),
            new ObjectDeletionState (_orderItem, "2. Deleted event of orderItem"),
          };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      _orderItem.Delete ();

      DomainObjectCollection originalOrderItems = _order.GetOriginalRelatedObjects ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      Assert.IsNotNull (originalOrderItems);
      Assert.AreEqual (2, originalOrderItems.Count);
      Assert.AreSame (_orderItem, originalOrderItems[_orderItem.ID]);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetRelatedObjectOfDeletedObject ()
    {
      _orderItem.Delete ();

      _orderItem.Order = _order;
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ReassignDeletedObject ()
    {
      _orderItem.Delete ();

      _order.OrderItems.Add (_orderItem);
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _orderItem, _order },
          new DomainObjectCollection[] { _order.OrderItems });
    }
  }
}
