using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionRollbackDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void RollbackResetsPropertyValuesToThoseOfParentTransaction ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      Order newOrder = Order.NewObject ();

      loadedOrder.OrderNumber = 5;
      newOrder.OrderNumber = 7;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        loadedOrder.OrderNumber = 13;
        newOrder.OrderNumber = 47;

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreEqual (StateType.Unchanged, loadedOrder.State);
        Assert.AreEqual (StateType.Unchanged, newOrder.State);

        Assert.AreEqual (5, loadedOrder.OrderNumber);
        Assert.AreEqual (7, newOrder.OrderNumber);
      }

      Assert.AreEqual (5, loadedOrder.OrderNumber);
      Assert.AreEqual (7, newOrder.OrderNumber);
    }

    [Test]
    public void RollbackResetsRelatedObjectsToThoseOfParentTransaction ()
    {
      Order newOrder = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (orderItem);

      Assert.AreEqual (1, newOrder.OrderItems.Count);
      Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        newOrder.OrderItems.Add (OrderItem.NewObject ());

        Assert.AreEqual (2, newOrder.OrderItems.Count);
        Assert.IsFalse (newOrder.OrderItems.ContainsObject (orderItem));

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreEqual (StateType.Unchanged, newOrder.State);

        Assert.AreEqual (1, newOrder.OrderItems.Count);
        Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));
      }

      Assert.AreEqual (1, newOrder.OrderItems.Count);
      Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));
    }

    [Test]
    public void RollbackResetsRelatedObjectToThatOfParentTransaction ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;
      Location location = Location.NewObject ();
      Client client = Client.NewObject ();
      location.Client = client;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        computer.Employee = Employee.NewObject ();
        location.Client = null;
        Assert.IsNull (employee.Computer);

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreEqual (StateType.Unchanged, computer.State);
        Assert.AreEqual (StateType.Unchanged, employee.State);
        Assert.AreEqual (StateType.Unchanged, location.State);
        Assert.AreEqual (StateType.Unchanged, client.State);

        Assert.AreSame (employee, computer.Employee);
        Assert.AreSame (computer, employee.Computer);
        Assert.AreSame (client, location.Client);
      }

      Assert.AreSame (employee, computer.Employee);
      Assert.AreSame (computer, employee.Computer);
      Assert.AreSame (client, location.Client);
    }

    [Test]
    public void SubCommitDoesNotRollbackParent ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderNumber = 5;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderNumber = 3;
        ClientTransactionScope.CurrentTransaction.Rollback ();
      }
      Assert.AreEqual (5, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }
  }
}