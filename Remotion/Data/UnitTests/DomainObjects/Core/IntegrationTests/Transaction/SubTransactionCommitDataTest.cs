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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitPropagatesChangesToLoadedObjectsToParentTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (0, order.OrderNumber);
      }

      Assert.AreEqual (0, order.OrderNumber);
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToMany ()
    {
      var originalOrder = Order.GetObject (DomainObjectIDs.Order2);
      Assert.That (originalOrder.OrderItems.Count, Is.EqualTo (1));

      var originalTicket = originalOrder.OrderTicket;
      var originalOfficial = originalOrder.Official;

      var orderItem = originalOrder.OrderItems[0];

      Order newOrder;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder = Order.GetObject (DomainObjectIDs.Order3);
        newOrder.OrderItems.Add (orderItem);
        originalTicket.Delete (); // dependent object
        originalOrder.Delete ();
        ClientTransaction.Current.Commit ();
        Assert.That (orderItem.Order, Is.SameAs (newOrder));
        Assert.That (orderItem.Properties.Find ("Order").GetRelatedObjectID (), Is.EqualTo (newOrder.ID));
      }
      Assert.That (orderItem.Order, Is.SameAs (newOrder));
      Assert.That (orderItem.Properties.Find ("Order").GetRelatedObjectID (), Is.EqualTo (newOrder.ID));

      Assert.That (newOrder.OrderItems.ContainsObject (orderItem));
      Assert.That (originalOrder.State, Is.EqualTo (StateType.Deleted));
      Assert.That (originalOrder.OrderItems, Is.Empty);
      Assert.That (originalOrder.OrderTicket, Is.Null);
      Assert.That (originalOrder.Official, Is.Null);

      Assert.That (originalTicket.Order, Is.Null);
      Assert.That (originalOfficial.Orders, Has.No.Member(originalOrder));
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToOne ()
    {
      var originalOrder = Order.GetObject (DomainObjectIDs.Order2);
      Assert.That (originalOrder.OrderItems.Count, Is.EqualTo (1));
      var originalItem = originalOrder.OrderItems[0];
      var originalOfficial = originalOrder.Official;

      var orderTicket = originalOrder.OrderTicket;

      Order newOrder;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newOrder = Order.GetObject (DomainObjectIDs.Order3);
        newOrder.OrderTicket.Delete (); // delete old ticket
        newOrder.OrderTicket = orderTicket;
        
        originalItem.Delete (); // delete old item
        originalOrder.Delete ();
        ClientTransaction.Current.Commit ();
        Assert.That (orderTicket.Order, Is.SameAs (newOrder));
        Assert.That (orderTicket.Properties.Find ("Order").GetRelatedObjectID (), Is.EqualTo (newOrder.ID));
      }
      Assert.That (orderTicket.Order, Is.SameAs (newOrder));
      Assert.That (orderTicket.Properties.Find ("Order").GetRelatedObjectID (), Is.EqualTo (newOrder.ID));

      Assert.That (newOrder.OrderTicket, Is.SameAs (orderTicket));
      Assert.That (originalOrder.State, Is.EqualTo (StateType.Deleted));
      Assert.That (originalOrder.OrderItems, Is.Empty);
      Assert.That (originalOrder.OrderTicket, Is.Null);
      Assert.That (originalOrder.Official, Is.Null);

      Assert.That (originalItem.Order, Is.Null);
      Assert.That (originalOfficial.Orders, Has.No.Member(originalOrder));
    }

    [Test]
    public void CommitSavesPropertyValuesToParentTransaction ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ClassWithAllDataTypes newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject ();

      loadedOrder.OrderNumber = 5;
      newClassWithAllDataTypes.Int16Property = 7;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotEqual (4711, orderInSub.OrderNumber);
        orderInSub.OrderNumber = 4711;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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
      TestableClientTransaction.Commit ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (cwadt);
        Assert.AreEqual (7, cwadt.Int32Property);
        Assert.AreEqual (8, cwadt.Int16Property);
      }
    }

    [Test]
    public void PropertyValue_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.AreEqual (32767, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        cwadt.Int32Property = 7;
        Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.AreEqual (32767, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
          Assert.AreEqual (32767, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
          Assert.AreEqual (7, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));

          cwadt.Int16Property = 8;

          Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
          Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
          Assert.AreEqual (8, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
          Assert.AreEqual (7, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));
        }

        Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.AreEqual (32767, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));

        ClientTransaction.Current.Commit ();

        Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
        Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
        Assert.AreEqual (8, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
        Assert.AreEqual (7, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));
      }

      Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsTrue (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsTrue (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.AreEqual (32767, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));

      TestableClientTransaction.Commit ();

      Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueChanged (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property")));
      Assert.IsFalse (cwadt.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property")));
      Assert.AreEqual (8, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original));
      Assert.AreEqual (7, cwadt.InternalDataContainer.GetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original));
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


      RelationEndPointID propertyID = RelationEndPointID.Create(orderTicket.ID, typeof (OrderTicket).FullName + ".Order");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        orderTicket.Order = newOrder;
        oldOrder.OrderTicket = oldOrderTicket;
        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (oldOrder.ID, ((IObjectEndPoint)GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (newOrder, orderTicket.Order);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (newOrder.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

          orderTicket.Order = newOrder2;
          oldOrderTicket2.Order = newOrder;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (newOrder.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (newOrder2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (oldOrder.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (newOrder2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.AreEqual (oldOrder.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.AreEqual (newOrder2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
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

      RelationEndPointID propertyID = RelationEndPointID.Create(order3.ID, typeof (Order).FullName + ".OrderTicket");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order3.OrderTicket = orderTicket1;
        orderTicket3.Order = order1;

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (orderTicket3.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.AreEqual (orderTicket1, order3.OrderTicket);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (orderTicket1.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

          order3.OrderTicket = orderTicket2;
          orderTicket1.Order = order2;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (orderTicket1.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.AreEqual (orderTicket2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (orderTicket3.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.AreEqual (orderTicket2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.AreEqual (orderTicket3.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.AreEqual (orderTicket2.ID, ((IObjectEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).OriginalOppositeObjectID);
    }

    [Test]
    public void CollectionEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem newItem = OrderItem.NewObject ();
      OrderItem firstItem = order.OrderItems[0];

      RelationEndPointID propertyID = RelationEndPointID.Create(order.ID, typeof (Order).FullName + ".OrderItems");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderItems.Add (newItem);

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
        Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
          Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));

          order.OrderItems[0].Delete ();
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
          Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
          Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
          Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
        Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
        Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
        Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
      Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID).HasChanged);
      Assert.IsTrue (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (newItem));
      Assert.IsFalse (((ICollectionEndPoint) GetDataManager (ClientTransaction.Current).GetRelationEndPointWithoutLoading (propertyID)).GetCollectionWithOriginalData().ContainsObject (firstItem));
    }

    [Test]
    public void Committing_TwoDeletedNewObjects_RelatedInParentTransaction ()
    {
      var order = Order.NewObject ();
      var orderTicket = OrderTicket.NewObject ();

      orderTicket.Order = order;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.Delete ();
        orderTicket.Delete ();

        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.IsInvalid, Is.True);
      Assert.That (orderTicket.IsInvalid, Is.True);

      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }


    private DataManager GetDataManager (ClientTransaction transaction)
    {
      return (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
    }
  }
}
