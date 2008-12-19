// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionCommitDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitPropagatesChangesToLoadedObjectsToParentTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        order.OrderNumber = 5;

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (5, order.OrderNumber);
      }

      Assert.IsNotNull (order);
      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    public void CommitPropagatesChangesToNewObjectsToParentTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        classWithAllDataTypes.Int32Property = 7;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInSub ()
    {
      Order order;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (1, order.OrderNumber);
      }

      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInRoot ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (1, order.OrderNumber);
      }

      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void CommitLeavesUnchangedNewObjects ()
    {
      Order order = Order.NewObject ();
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (0, order.OrderNumber);
      }

      Assert.AreEqual (0, order.OrderNumber);
    }

    [Test]
    public void CommitSavesPropertyValuesToParentTransaction ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ClassWithAllDataTypes newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject ();

      loadedOrder.OrderNumber = 5;
      newClassWithAllDataTypes.Int16Property = 7;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        loadedOrder.OrderNumber = 13;
        newClassWithAllDataTypes.Int16Property = 47;

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, loadedOrder.State);
        Assert.AreEqual (StateType.Unchanged, newClassWithAllDataTypes.State);

        Assert.AreEqual (13, loadedOrder.OrderNumber);
        Assert.AreEqual (47, newClassWithAllDataTypes.Int16Property);
      }

      Assert.AreEqual (13, loadedOrder.OrderNumber);
      Assert.AreEqual (47, newClassWithAllDataTypes.Int16Property);

      Assert.AreEqual (StateType.Changed, loadedOrder.State);
      Assert.AreEqual (StateType.New, newClassWithAllDataTypes.State);
    }

    [Test]
    public void CommitSavesRelatedObjectsToParentTransaction ()
    {
      Order order = Order.NewObject ();
      Official official = Official.GetObject (DomainObjectIDs.Official1);
      order.Official = official;
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      OrderItem orderItem = OrderItem.NewObject ();
      order.OrderItems.Add (orderItem);

      Assert.AreSame (official, order.Official);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsTrue (order.OrderItems.ContainsObject (orderItem));
      Assert.IsNull (order.OrderTicket);

      OrderItem newOrderItem;
      OrderTicket newOrderTicket;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrderItem = OrderItem.NewObject ();

        orderItem.Delete ();
        order.OrderItems.Add (newOrderItem);
        order.OrderItems.Add (OrderItem.NewObject ());

        newOrderTicket = OrderTicket.NewObject ();
        order.OrderTicket = newOrderTicket;

        Assert.AreSame (official, order.Official);
        Assert.AreEqual (2, order.OrderItems.Count);
        Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
        Assert.IsNotNull (order.OrderTicket);
        Assert.AreSame (newOrderTicket, order.OrderTicket);

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, order.State);

        Assert.AreSame (official, order.Official);
        Assert.AreEqual (2, order.OrderItems.Count);
        Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
        Assert.IsNotNull (order.OrderTicket);
        Assert.AreSame (newOrderTicket, order.OrderTicket);
      }

      Assert.AreSame (official, order.Official);
      Assert.AreEqual (2, order.OrderItems.Count);
      Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
      Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
      Assert.IsNotNull (order.OrderTicket);
      Assert.AreSame (newOrderTicket, order.OrderTicket);
    }

    [Test]
    public void CommittedRelatedObjectCollectionOrder ()
    {
      Order order = Order.NewObject ();
      Official official = Official.GetObject (DomainObjectIDs.Official1);
      order.Official = official;
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      OrderItem orderItem1 = OrderItem.NewObject ();
      OrderItem orderItem2 = OrderItem.NewObject ();
      OrderItem orderItem3 = OrderItem.NewObject ();
      order.OrderItems.Add (orderItem1);
      order.OrderItems.Add (orderItem2);
      order.OrderItems.Add (orderItem3);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (order.OrderItems, Is.EqualTo (new object[] {orderItem1, orderItem2, orderItem3}));
        order.OrderItems.Clear ();
        order.OrderItems.Add (orderItem2);
        order.OrderItems.Add (orderItem3);
        order.OrderItems.Add (orderItem1);
        Assert.That (order.OrderItems, Is.EqualTo (new object[] { orderItem2, orderItem3, orderItem1 }));
        ClientTransaction.Current.Commit();
        Assert.That (order.OrderItems, Is.EqualTo (new object[] { orderItem2, orderItem3, orderItem1 }));
      }
      Assert.That (order.OrderItems, Is.EqualTo (new object[] { orderItem2, orderItem3, orderItem1 }));
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (order.OrderItems, Is.EqualTo (new object[] {orderItem2, orderItem3, orderItem1}));
      }
    }

    [Test]
    public void CommitSavesRelatedObjectToParentTransaction ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;
      Location location1 = Location.NewObject ();
      Location location2 = Location.NewObject ();

      Client client = Client.NewObject ();
      location1.Client = client;

      Employee newEmployee;
      Client newClient1 = Client.NewObject ();
      Client newClient2;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newEmployee = Employee.NewObject ();
        computer.Employee = newEmployee;

        location1.Client = newClient1;

        newClient2 = Client.NewObject ();
        location2.Client = newClient2;

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.AreSame (newClient1, location1.Client);
        Assert.AreSame (newClient2, location2.Client);

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.AreSame (newClient1, location1.Client);
        Assert.AreSame (newClient2, location2.Client);
      }

      Assert.IsNull (employee.Computer);
      Assert.AreSame (newEmployee, computer.Employee);
      Assert.AreSame (newClient1, location1.Client);
      Assert.AreSame (newClient2, location2.Client);
    }

    [Test]
    public void EndPointsAreCorrectFromBothSidesForCompletelyNewObjectGraphs ()
    {
      Order order;
      OrderItem newOrderItem;
      OrderTicket newOrderTicket;
      Official newOfficial;
      Customer newCustomer;
      Ceo newCeo;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order = Order.NewObject ();
        
        newOrderTicket = OrderTicket.NewObject ();
        order.OrderTicket = newOrderTicket;
        
        newOrderItem = OrderItem.NewObject ();
        order.OrderItems.Add (newOrderItem);
        
        newOfficial = Official.NewObject ();
        order.Official = newOfficial;
        
        newCustomer = Customer.NewObject ();
        order.Customer = newCustomer;

        newCeo = Ceo.NewObject ();
        newCustomer.Ceo = newCeo;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.AreSame (order, newOrderTicket.Order);
      Assert.AreSame (newOrderTicket, order.OrderTicket);

      Assert.AreSame (newOrderItem, order.OrderItems[0]);
      Assert.AreSame (order, newOrderItem.Order);

      Assert.AreSame (order, order.Official.Orders[0]);
      Assert.AreSame (newOfficial, order.Official);

      Assert.AreSame (order, order.Customer.Orders[0]);
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (newCeo, newCustomer.Ceo);
      Assert.AreSame (newCustomer, newCeo.Company);
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInParent ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotEqual (4711, orderInSub.OrderNumber);
        orderInSub.OrderNumber = 4711;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Order orderInParent = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (4711, orderInParent.OrderNumber);
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInNewSub ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotEqual (4711, orderInSub.OrderNumber);
        orderInSub.OrderNumber = 4711;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreEqual (4711, orderInSub.OrderNumber);
      }
    }

    [Test]
    public void ObjectValuesCanBeChangedInParentAndChildSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreNotEqual (7, cwadt.Int32Property);
      Assert.AreNotEqual (8, cwadt.Int16Property);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        cwadt.Int32Property = 7;
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (7, cwadt.Int32Property);
          cwadt.Int16Property = 8;
          ClientTransaction.Current.Commit ();
        }
        Assert.AreEqual (7, cwadt.Int32Property);
        Assert.AreEqual (8, cwadt.Int16Property);
        ClientTransaction.Current.Commit ();
      }
      Assert.AreEqual (7, cwadt.Int32Property);
      Assert.AreEqual (8, cwadt.Int16Property);
      ClientTransactionMock.Commit ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (cwadt);
        Assert.AreEqual (7, cwadt.Int32Property);
        Assert.AreEqual (8, cwadt.Int16Property);
      }
    }

    [Test]
    public void HasBeenMarkedChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          cwadt.MarkAsChanged ();
          Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);

          using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
          {
            Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
            ++cwadt.Int32Property;
            Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);
            ClientTransaction.Current.Commit ();
            Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
          }

          ClientTransaction.Current.Commit ();

          Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
        }

        Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);
        ClientTransaction.Current.Commit ();
        Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
      }

      Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);

      ClientTransactionMock.Commit ();

      Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
    }

    [Test]
    public void PropertyValue_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
      Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        cwadt.Int32Property = 7;
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
        Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
          Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
          Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

          cwadt.Int16Property = 8;

          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
          Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
          Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
          Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
        }

        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
        Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

        ClientTransaction.Current.Commit ();

        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
        Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
      }

      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
      Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

      ClientTransactionMock.Commit ();

      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasBeenTouched);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasBeenTouched);
      Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
    }

    [Test]
    public void ObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Order oldOrder = orderTicket.Order;
      
      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      OrderTicket oldOrderTicket = newOrder.OrderTicket;

      Order newOrder2 = Order.GetObject (DomainObjectIDs.Order3);
      OrderTicket oldOrderTicket2 = newOrder2.OrderTicket;


      RelationEndPointID propertyID = new RelationEndPointID (orderTicket.ID, typeof (OrderTicket).FullName + ".Order");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        orderTicket.Order = newOrder;
        oldOrder.OrderTicket = oldOrderTicket;
        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint)GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (newOrder, orderTicket.Order);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          orderTicket.Order = newOrder2;
          oldOrderTicket2.Order = newOrder;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (newOrder2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (newOrder2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
    }

    [Test]
    public void VirtualObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      OrderTicket orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Order order1 = orderTicket1.Order;

      OrderTicket orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      Order order2 = orderTicket2.Order;

      Order order3 = Order.GetObject (DomainObjectIDs.Order2);
      OrderTicket orderTicket3 = order3.OrderTicket;

      RelationEndPointID propertyID = new RelationEndPointID (order3.ID, typeof (Order).FullName + ".OrderTicket");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order3.OrderTicket = orderTicket1;
        orderTicket3.Order = order1;

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (orderTicket3.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (orderTicket1, order3.OrderTicket);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (orderTicket1.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          order3.OrderTicket = orderTicket2;
          orderTicket1.Order = order2;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (orderTicket1.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (orderTicket2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (orderTicket3.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (orderTicket2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (orderTicket3.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (orderTicket2.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
    }

    [Test]
    public void CollectionEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem newItem = OrderItem.NewObject ();
      OrderItem firstItem = order.OrderItems[0];

      RelationEndPointID propertyID = new RelationEndPointID (order.ID, typeof (Order).FullName + ".OrderItems");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderItems.Add (newItem);

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

          order.OrderItems[0].Delete ();
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
      Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
      Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
    }

    private DataManager GetDataManager (ClientTransaction transaction)
    {
      return (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
    }
  }
}
