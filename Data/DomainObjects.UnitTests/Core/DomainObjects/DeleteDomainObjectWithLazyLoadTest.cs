using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class DeleteDomainObjectWithLazyLoadTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualProperty ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.Delete ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
			Assert.IsNull (orderTicket.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
			Assert.AreEqual (StateType.Unchanged, order.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualNullProperty ()
    {
      Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
      computerWithoutEmployee.Delete ();

      Assert.IsNull (computerWithoutEmployee.Employee);
			Assert.IsNull (computerWithoutEmployee.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"]);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualProperty ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();

      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
			Assert.IsNull (orderTicket.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
			Assert.AreEqual (StateType.Changed, orderTicket.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualNullProperty ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      employeeWithoutComputer.Delete ();

      Assert.IsNull (employeeWithoutComputer.Computer);
    }

    [Test]
    public void DomainObjectWithOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      supervisor.Delete ();

      Employee subordinate1 = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee subordinate2 = Employee.GetObject (DomainObjectIDs.Employee5);

      Assert.AreEqual (0, supervisor.Subordinates.Count);
      Assert.IsNull (subordinate1.Supervisor);
      Assert.IsNull (subordinate2.Supervisor);
			Assert.IsNull (subordinate1.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"]);
			Assert.IsNull (subordinate2.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"]);
			Assert.AreEqual (StateType.Changed, subordinate1.InternalDataContainer.State);
			Assert.AreEqual (StateType.Changed, subordinate2.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithEmptyOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee3);
      supervisor.Delete ();

      Assert.AreEqual (0, supervisor.Subordinates.Count);
    }

    [Test]
    public void DomainObjectWithManyToOneRelation ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem.Delete ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsNull (orderItem.Order);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsFalse (order.OrderItems.Contains (orderItem.ID));
			Assert.IsNull (orderItem.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
			Assert.AreEqual (StateType.Unchanged, order.InternalDataContainer.State);
    }
  }
}
