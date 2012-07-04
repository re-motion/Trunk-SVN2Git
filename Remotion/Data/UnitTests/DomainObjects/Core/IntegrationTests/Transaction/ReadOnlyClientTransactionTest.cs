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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class ReadOnlyClientTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: NewObjectCreating.")]
    public void ThrowsOnNewObject ()
    {
      TestableClientTransaction.IsReadOnly = true;

      Order.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectDeleting.")]
    public void ThrowsOnDeleteNew ()
    {
      Order newOrder = Order.NewObject ();

      TestableClientTransaction.IsReadOnly = true;

      newOrder.Delete ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectDeleting.")]
    public void ThrowsOnDeleteLoaded ()
    {
      ClassWithAllDataTypes loadedCwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      TestableClientTransaction.IsReadOnly = true;

      loadedCwadt.Delete ();
    }

    [Test]
    public void CanGetPropertyValue ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      
      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedOrder.OrderNumber;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: PropertyValueChanging.")]
    public void ThrowsOnSetPropertyValue ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      TestableClientTransaction.IsReadOnly = true;

      loadedOrder.OrderNumber = 42;
    }

    [Test]
    public void CanGetObjectIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      TestableClientTransaction.IsReadOnly = true;

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreSame (loadedOrder, order);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectsLoading.")]
    public void ThrowsOnGetObjectIfNotLoaded ()
    {
      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = Order.GetObject (DomainObjectIDs.Order1);
    }

    #region Get/SetRelatedObjects1Side

    [Test]
    public void CanGetRelatedObjects1SideIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem loadedOrderItem = loadedOrder.OrderItems[0];
      
      TestableClientTransaction.IsReadOnly = true;

      OrderItem orderItem = loadedOrder.OrderItems[0];
      Assert.AreSame (loadedOrderItem, orderItem);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetRelatedObjects1SideIfNotLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedOrder.OrderItems[0];
    }

    [Test]
    public void CanGetRelatedObjectsEmpty1SideIfAlreadyLoaded ()
    {
      Official loadedOfficial = Official.GetObject (DomainObjectIDs.Official2);
      ObjectList<Order> loadedOrders = loadedOfficial.Orders;
      Assert.AreEqual (0, loadedOrders.Count);

      TestableClientTransaction.IsReadOnly = true;

      ObjectList<Order> orders = loadedOfficial.Orders;
      Assert.AreSame (loadedOrders, orders);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetRelatedObjectsEmpty1SideIfNotLoaded ()
    {
      Official loadedOfficial = Official.GetObject (DomainObjectIDs.Official2);

      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedOfficial.Orders;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnAddRelatedObjects1Side ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ObjectList<OrderItem> loadedOrderItems = loadedOrder.OrderItems;
      OrderItem newItem = OrderItem.NewObject ();

      TestableClientTransaction.IsReadOnly = true;

      loadedOrderItems.Add (newItem);
    }

    #endregion
    
    #region Get/SetRelatedObjectsNSide

    [Test]
    public void CanGetRelatedObjectsNSideIfAlreadyLoaded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem loadedOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      
      TestableClientTransaction.IsReadOnly = true;

      Order order = loadedOrderItem.Order;
      Assert.AreSame (loadedOrder, order);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: ObjectsLoading.")]
    public void ThrowsOnGetRelatedObjectsNSideIfNotLoaded ()
    {
      OrderItem loadedOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedOrderItem.Order;
    }


    [Test]
    public void CanGetNullRelatedObjectsNSideIfAlreadyLoaded ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);
      Assert.IsNull (loadedClient.ParentClient);

      TestableClientTransaction.IsReadOnly = true;

      Assert.IsNull (loadedClient.ParentClient);
    }

    [Test]
    public void CanGetNullRelatedObjectsNSideIfNotLoaded ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);

      TestableClientTransaction.IsReadOnly = true;

      Assert.IsNull (loadedClient.ParentClient);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnSetRelatedObjectsNSide ()
    {
      Client loadedClient = Client.GetObject (DomainObjectIDs.Client1);
      Client newClient = Client.NewObject ();

      TestableClientTransaction.IsReadOnly = true;

      loadedClient.ParentClient = newClient;
    }

    #endregion

    #region Get/SetRelatedObject1To1RealSide

    [Test]
    public void CanGetRelatedObject1To1RealSideIfAlreadyLoaded ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = loadedComputer.Employee;
      
      TestableClientTransaction.IsReadOnly = true;

      Employee employee = loadedComputer.Employee;
      Assert.AreSame (loadedEmployee, employee);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: ObjectsLoading.")]
    public void ThrowsOnGetRelatedObject1To1RealSideIfNotLoaded ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);

      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedComputer.Employee;
    }

    [Test]
    public void CanGetNullRelatedObject1To1RealSideIfAlreadyLoaded ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Dev.Null = computer.Employee;
      Assert.IsNull (computer.Employee);

      TestableClientTransaction.IsReadOnly = true;

      Assert.IsNull (computer.Employee);
    }

    [Test]
    public void CanGetNullRelatedObject1To1RealSideIfNotLoaded ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);

      TestableClientTransaction.IsReadOnly = true;

      Assert.IsNull (computer.Employee);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: RelationChanging.")]
    public void ThrowsOnSetRelatedObejct1To1RealSide ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Employee newEmployee = Employee.NewObject ();

      TestableClientTransaction.IsReadOnly = true;

      computer.Employee = newEmployee;
    }

    #endregion

    #region GetRelatedObject1To1VirtualSide

    [Test]
    public void CanGetRelatedObject1To1VirtualSideIfAlreadyLoaded ()
    {
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer loadedComputer = loadedEmployee.Computer;

      TestableClientTransaction.IsReadOnly = true;

      Computer computer = loadedEmployee.Computer;
      Assert.AreSame (loadedComputer, computer);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetRelatedObject1To1VirtualSideIfNotLoaded ()
    {
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);

      TestableClientTransaction.IsReadOnly = true;

      Dev.Null = loadedEmployee.Computer;
    }

    [Test]
    public void CanGetNullRelatedObject1To1VirtualSideIfAlreadyLoaded ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
      Assert.IsNull (employee.Computer);

      TestableClientTransaction.IsReadOnly = true;

      Assert.IsNull (employee.Computer);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
       + "ClientTransaction is read-only. Offending transaction modification: RelationEndPointMapRegistering.")]
    public void ThrowsOnGetNullRelatedObject1To1VirtualSideIfNotLoaded ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);

      TestableClientTransaction.IsReadOnly = true;

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

      TestableClientTransaction.IsReadOnly = true;

      employee.Computer = newComputer;
    }

#endregion

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: TransactionCommitting.")]
    public void ThrowsOnCommit ()
    {
      TestableClientTransaction.IsReadOnly = true;
      TestableClientTransaction.Commit ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: TransactionRollingBack.")]
    public void ThrowsOnRollback ()
    {
      TestableClientTransaction.IsReadOnly = true;
      TestableClientTransaction.Rollback ();
    }

    [Test]
    public void CanExecuteQueryIfAlreadyLoaded ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");
      var loadedOrders = (OrderCollection) TestableClientTransaction.QueryManager.GetCollection (query).ToCustomCollection();

      TestableClientTransaction.IsReadOnly = true;
      
      var orders = (OrderCollection) TestableClientTransaction.QueryManager.GetCollection (query).ToCustomCollection();
      Assert.AreEqual (loadedOrders.Count, orders.Count);
      Assert.AreSame (loadedOrders[0], orders[0]);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: ObjectsLoading.")]
    public void ThrowsOnExecuteQueryIfNotLoaded ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("StoredProcedureQuery");

      TestableClientTransaction.IsReadOnly = true;

      TestableClientTransaction.QueryManager.GetCollection (query);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating.")]
    public void ThrowsOnCreateSubTransaction ()
    {
      TestableClientTransaction.IsReadOnly = true;
      TestableClientTransaction.CreateSubTransaction();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage =
        "The operation cannot be executed because the ClientTransaction is read-only. Offending transaction modification: RelationEndPointUnloading.")]
    public void ThrowsOnUnloadCollectionEndPoint ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var endPointID = customer.Orders.AssociatedEndPointID;
      customer.Orders.EnsureDataComplete();

      TestableClientTransaction.IsReadOnly = true;
      ((ICollectionEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID)).MarkDataIncomplete();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage =
        "The operation cannot be executed because the ClientTransaction is read-only. Offending transaction modification: "
        + "DataContainerMapUnregistering.")]
    public void ThrowsOnUnloadData_WithEndPoints ()
    {
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      TestableClientTransaction.IsReadOnly = true;
      var command = TestableClientTransaction.DataManager.CreateUnloadCommand (DomainObjectIDs.Order1);
      command.NotifyAndPerform ();
    }
  }
}
