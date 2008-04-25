using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ReadOnlyClientTransactionTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      //ConsoleAppender appender = new ConsoleAppender ();
      //appender.Layout = new PatternLayout ("%logger - %message%newline");
      //BasicConfigurator.Configure (appender);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: NewObjectCreating.")]
    public void ThrowsOnNewObject ()
    {
      ClientTransactionMock.IsReadOnly = true;

      Order.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectDeleting.")]
    public void ThrowsOnDeleteNew ()
    {
      Order newOrder = Order.NewObject ();

      ClientTransactionMock.IsReadOnly = true;

      newOrder.Delete ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectDeleting.")]
    public void ThrowsOnDeleteLoaded ()
    {
      ClassWithAllDataTypes loadedCwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      ClientTransactionMock.IsReadOnly = true;

      loadedCwadt.Delete ();
    }

    [Test]
    public void CanGetPropertyValue ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      
      ClientTransactionMock.IsReadOnly = true;

      int orderNumber = loadedOrder.OrderNumber;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: PropertyValueChanging.")]
    public void ThrowsOnSetPropertyValue ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionMock.IsReadOnly = true;

      loadedOrder.OrderNumber = 42;
    }

    [Test]
    public void CanGetObjectIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionMock.IsReadOnly = true;

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreSame (loadedOrder, order);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnGetObjectIfNotLoaded ()
    {
      ClientTransactionMock.IsReadOnly = true;

      Order order = Order.GetObject (DomainObjectIDs.Order1);
    }

    #region Get/SetRelatedObjects1Side

    [Test]
    public void CanGetRelatedObjects1SideIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem loadedOrderItem = loadedOrder.OrderItems[0];
      
      ClientTransactionMock.IsReadOnly = true;

      OrderItem orderItem = loadedOrder.OrderItems[0];
      Assert.AreSame (loadedOrderItem, orderItem);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnGetRelatedObjects1SideIfNotLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransactionMock.IsReadOnly = true;

      OrderItem orderItem = loadedOrder.OrderItems[0];
    }

    [Test]
    public void CanGetRelatedObjectsEmpty1SideIfAlreadyLoaded ()
    {
      Official loadedOfficial = Official.GetObject (DomainObjectIDs.Official2);
      ObjectList<Order> loadedOrders = loadedOfficial.Orders;
      Assert.AreEqual (0, loadedOrders.Count);

      ClientTransactionMock.IsReadOnly = true;

      ObjectList<Order> orders = loadedOfficial.Orders;
      Assert.AreSame (loadedOrders, orders);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetRelatedObjectsEmpty1SideIfNotLoaded ()
    {
      Official loadedOfficial = Official.GetObject (DomainObjectIDs.Official2);

      ClientTransactionMock.IsReadOnly = true;

      ObjectList<Order> orders = loadedOfficial.Orders;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnAddRelatedObjects1Side ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ObjectList<OrderItem> loadedOrderItems = loadedOrder.OrderItems;
      OrderItem newItem = OrderItem.NewObject ();

      ClientTransactionMock.IsReadOnly = true;

      loadedOrderItems.Add (newItem);
    }

    #endregion
    
    #region Get/SetRelatedObjectsNSide

    [Test]
    public void CanGetRelatedObjectsNSideIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem loadedOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      
      ClientTransactionMock.IsReadOnly = true;

      Order order = loadedOrderItem.Order;
      Assert.AreSame (loadedOrder, order);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnGetRelatedObjectsNSideIfNotLoaded ()
    {
      OrderItem loadedOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      ClientTransactionMock.IsReadOnly = true;

      Order order = loadedOrderItem.Order;
    }


    [Test]
    public void CanGetNullRelatedObjectsNSideIfAlreadyLoaded ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);
      Assert.IsNull (loadedClient.ParentClient);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (loadedClient.ParentClient);
    }

    [Test]
    public void CanGetNullRelatedObjectsNSideIfNotLoaded ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (loadedClient.ParentClient);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnSetRelatedObjectsNSide ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);
      Client newClient = Client.NewObject ();

      ClientTransactionMock.IsReadOnly = true;

      loadedClient.ParentClient = newClient;
    }

    #endregion

    #region Get/SetRelatedObject1To1RealSide

    [Test]
    public void CanGetRelatedObject1To1RealSideIfAlreadyLoaded ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = loadedComputer.Employee;
      
      ClientTransactionMock.IsReadOnly = true;

      Employee employee = loadedComputer.Employee;
      Assert.AreSame (loadedEmployee, employee);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnGetRelatedObject1To1RealSideIfNotLoaded ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);

      ClientTransactionMock.IsReadOnly = true;

      Employee employee = loadedComputer.Employee;
    }

    [Test]
    public void CanGetNullRelatedObject1To1RealSideIfAlreadyLoaded ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee employee = computer.Employee;
      Assert.IsNull (computer.Employee);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (computer.Employee);
    }

    [Test]
    public void CanGetNullRelatedObject1To1RealSideIfNotLoaded ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (computer.Employee);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnSetRelatedObejct1To1RealSide ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee newEmployee = Employee.NewObject ();

      ClientTransactionMock.IsReadOnly = true;

      computer.Employee = newEmployee;
    }

    #endregion

    #region GetRelatedObject1To1VirtualSide

    [Test]
    public void CanGetRelatedObject1To1VirtualSideIfAlreadyLoaded ()
    {
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer loadedComputer = loadedEmployee.Computer;

      ClientTransactionMock.IsReadOnly = true;

      Computer computer = loadedEmployee.Computer;
      Assert.AreSame (loadedComputer, computer);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnGetRelatedObject1To1VirtualSideIfNotLoaded ()
    {
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);

      ClientTransactionMock.IsReadOnly = true;

      Computer computer = loadedEmployee.Computer;
    }

    [Test]
    public void CanGetNullRelatedObject1To1VirtualSideIfAlreadyLoaded ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
      Assert.IsNull (employee.Computer);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (employee.Computer);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetNullRelatedObject1To1VirtualSideIfNotLoaded ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);

      ClientTransactionMock.IsReadOnly = true;

      Assert.IsNull (employee.Computer);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnSetRelatedObject1To1VirtualSide ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
      Assert.IsNull (employee.Computer); // ensure the relation end point is registered

      Computer newComputer = Computer.NewObject ();

      ClientTransactionMock.IsReadOnly = true;

      employee.Computer = newComputer;
    }

#endregion

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: TransactionCommitting.")]
    public void ThrowsOnCommit ()
    {
      ClientTransactionMock.IsReadOnly = true;
      ClientTransactionMock.Commit ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: TransactionRollingBack.")]
    public void ThrowsOnRollback ()
    {
      ClientTransactionMock.IsReadOnly = true;
      ClientTransactionMock.Rollback ();
    }

    [Test]
    public void CanExecuteQueryIfAlreadyLoaded ()
    {
      Query query = new Query ("StoredProcedureQuery");
      OrderCollection loadedOrders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query);

      ClientTransactionMock.IsReadOnly = true;
      
      OrderCollection orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query);
      Assert.AreEqual (loadedOrders.Count, orders.Count);
      Assert.AreSame (loadedOrders[0], orders[0]);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectLoading.")]
    public void ThrowsOnExecuteQueryIfNotLoaded ()
    {
      Query query = new Query ("StoredProcedureQuery");

      ClientTransactionMock.IsReadOnly = true;

      ClientTransactionMock.QueryManager.GetCollection (query);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating.")]
    public void ThrowsOnCreateSubTransaction ()
    {
      ClientTransactionMock.IsReadOnly = true;
      ClientTransactionMock.CreateSubTransaction();
    }
  }
}