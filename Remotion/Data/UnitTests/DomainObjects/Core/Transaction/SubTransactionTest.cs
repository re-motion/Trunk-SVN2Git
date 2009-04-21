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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void NullParentTransaction ()
    {
      Assert.IsNull (ClientTransactionMock.ParentTransaction);
    }

    [Test]
    public void SameRootTransaction ()
    {
      Assert.AreSame (ClientTransactionMock, ClientTransactionMock.RootTransaction);
    }

    [Test]
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.AreSame (ClientTransactionMock, subTransaction.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction.RootTransaction);
    }

    [Test]
    public void CreateSubTransactionInSubTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction();
      Assert.AreSame (subTransaction1, subTransaction2.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction2.RootTransaction);
    }

    [Test]
    public void CreatingSubTransactionSetsParentReadonly ()
    {
      Assert.IsFalse (ClientTransactionMock.IsReadOnly);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.IsTrue (ClientTransactionMock.IsReadOnly);
      Assert.IsFalse (subTransaction.IsReadOnly);

      ClientTransaction subTransaction2 = subTransaction.CreateSubTransaction();
      Assert.IsTrue (subTransaction.IsReadOnly);
      Assert.IsFalse (subTransaction2.IsReadOnly);
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      subTransaction.Discard();
      ClientTransaction newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType();
      Assert.AreSame (ClientTransactionMock, subTransaction.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction.RootTransaction);
      Assert.AreNotSame (subTransaction, newSubTransaction);
      Assert.AreEqual (subTransaction.GetType (), newSubTransaction.GetType ());
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForRootTransaction ()
    {
      ClientTransaction newRootTransaction = ClientTransactionMock.CreateEmptyTransactionOfSameType ();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.AreSame (ClientTransactionMock, subTransaction.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction.RootTransaction);
      Assert.AreNotSame (ClientTransactionMock, newRootTransaction);
      Assert.AreEqual (ClientTransactionMock.GetType (), newRootTransaction.GetType ());
    }

    [Test]
    public void EnterDiscardingScopeEnablesDiscardBehavior ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (AutoRollbackBehavior.Discard, ClientTransactionScope.ActiveScope.AutoRollbackBehavior);
      }
    }

    [Test]
    public void SubTransactionHasSameExtensions ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.AreSame (ClientTransactionMock.Extensions, subTransaction.Extensions);
    }

    [Test]
    public void SubTransactionHasSameApplicationData ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.AreSame (ClientTransactionMock.ApplicationData, subTransaction.ApplicationData);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating.")]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction();
      ClientTransaction subTransaction2 = ClientTransactionMock.CreateSubTransaction();
    }

    [Test]
    public void SubTransactionCanBeUsedToCreateAndLoadNewObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.AreSame (subTransaction, ClientTransactionScope.CurrentTransaction);
        Order order = Order.NewObject ();
        Assert.IsTrue (order.TransactionContext[subTransaction].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

        order.OrderNumber = 4711;
        Assert.AreEqual (4711, order.OrderNumber);

        OrderItem item = OrderItem.NewObject ();
        order.OrderItems.Add (item);
        Assert.IsTrue (order.OrderItems.Contains (item.ID));

        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        Assert.IsNotNull (ceo);
        Assert.IsTrue (ceo.TransactionContext[subTransaction].CanBeUsedInTransaction);
        Assert.IsTrue (ceo.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);

        Assert.AreSame (ceo.Company, Company.GetObject (DomainObjectIDs.Company1));
      }
    }

    [Test]
    public void DomainObjectsCreatedInParentCanBeUsedInSubTransactions ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (order.TransactionContext[subTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void DomainObjectsCreatedInSubTransactionCanBeUsedInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject();
        Assert.IsTrue (order.TransactionContext[subTransaction].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      }
    }

    [Test]
    public void DomainObjectsLoadedInParentCanBeUsedInSubTransactions ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      Assert.IsTrue (order.TransactionContext[subTransaction].CanBeUsedInTransaction);
    }

    [Test]
    public void DomainObjectsLoadedInSubTransactionCanBeUsedInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsTrue (order.TransactionContext[subTransaction].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
      }
    }

    [Test]
    public void SubTransactionCanAccessObjectNewedInParent ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        order.OrderNumber = 5;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void ParentCannotAccessObjectNewedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.NewObject ();
      }
      Dev.Null = order.OrderNumber;
    }

    [Test]
    public void SubTransactionCanAccessObjectLoadedInParent ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        ++order.OrderNumber;
        OrderTicket oldTicket = order.OrderTicket;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    public void ParentCanAccessObjectLoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void ParentCanReloadObjectLoadedInSubTransactionAndGetTheSameReference ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Order order;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.AreSame (order, Order.GetObject (DomainObjectIDs.Order1));
    }

    [Test]
    public void ParentCanReloadRelatedObjectLoadedInSubTransactionAndGetTheSameReference ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Order order;
      OrderTicket orderTicket;
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        orderTicket = order.OrderTicket;
      }
      Assert.AreSame (order, Order.GetObject (DomainObjectIDs.Order1));
      Assert.AreSame (orderTicket, OrderTicket.GetObject (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void ParentCanReloadNullRelatedObjectLoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Computer computer;
      Employee employee;
      using (subTransaction.EnterDiscardingScope ())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        Assert.IsNull (computer.Employee);
        employee = Employee.GetObject (DomainObjectIDs.Employee1);
        Assert.IsNull (employee.Computer);
      }
      Assert.IsNull (Computer.GetObject (DomainObjectIDs.Computer4).Employee);
      Assert.IsNull (computer.Employee);
      Assert.IsNull (Employee.GetObject (DomainObjectIDs.Employee1).Computer);
      Assert.IsNull (employee.Computer);
    }

    [Test]
    public void ParentCanReloadRelatedObjectCollectionLoadedInSubTransactionAndGetTheSameReferences ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Order order;
      Set<OrderItem> orderItems = new Set<OrderItem> ();
      using (subTransaction.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        orderItems.Add (order.OrderItems[0]);
        orderItems.Add (order.OrderItems[1]);
      }
      Assert.AreSame (order, Order.GetObject (DomainObjectIDs.Order1));
      Assert.IsTrue (orderItems.Contains (OrderItem.GetObject (DomainObjectIDs.OrderItem1)));
      Assert.IsTrue (orderItems.Contains (OrderItem.GetObject (DomainObjectIDs.OrderItem1)));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void UnidirectionalDeleteInRootTransactionCausesThrowOnAccess ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client1);
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Assert.AreSame (client, location.Client);
      client.Delete ();
      Client clientAfterDelete = location.Client;
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void IndirectAccessToDeletedObjectInSubTransactionThrows ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client1);
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Assert.AreSame (client, location.Client);

      client.Delete ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope())
      {
        Client clientAfterDelete = location.Client;
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void NewUnidirectionalDeleteInRootTransactionCausesThrowOnAccess ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject ();
      location.Client.Delete ();

      Client clientAfterDelete = location.Client;
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void IndirectAccessToDeletedNewObjectInSubTransactionThrows ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject ();
      location.Client.Delete ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Client clientAfterDelete = location.Client;
      }
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInRootTransactionWorks ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.NewObject ();
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.NewObject ();
      location.Client.Delete ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        location.Client = Client.NewObject ();
      }
    }

    [Test]
    public void ResettingDeletedLoadedUnidirectionalInRootTransactionWorks ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }

    [Test]
    public void ResettingDeletedLoadedUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        location.Client = Client.NewObject();
      }
    }

    [Test]
    public void StateChangesInsideSubTransaction ()
    {
      Order newOrder = Order.NewObject ();

      Assert.AreEqual (StateType.New, newOrder.State);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, newOrder.State);

        newOrder.OrderNumber = 7;

        Assert.AreEqual (StateType.Changed, newOrder.State);
        Assert.AreNotEqual (newOrder.Properties[typeof (Order) + ".OrderNumber"].GetValue<int>(),
            newOrder.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int> ());
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForPersistentProperties ()
    {
      Order newUnchangedOrder = Order.NewObject ();
      int newUnchangedOrderNumber = newUnchangedOrder.OrderNumber;

      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedUnchangedOrder = Order.GetObject (DomainObjectIDs.Order1);
      int loadedUnchangedOrderNumber = loadedUnchangedOrder.OrderNumber;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        Assert.AreSame (loadedUnchangedOrder, Order.GetObject (DomainObjectIDs.Order1));
        Assert.AreSame (loadedChangedOrder, Order.GetObject (DomainObjectIDs.Order2));

        Assert.AreEqual (newUnchangedOrderNumber, newUnchangedOrder.OrderNumber);
        Assert.AreEqual (4711, newChangedOrder.OrderNumber);
        Assert.AreEqual (loadedUnchangedOrderNumber, loadedUnchangedOrder.OrderNumber);
        Assert.AreEqual (13, loadedChangedOrder.OrderNumber);
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForTransactionProperties ()
    {
      OrderTicket newUnchangedOrderTicket = OrderTicket.NewObject ();
      int newUnchangedInt32TransactionProperty = newUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket newChangedOrderTicket = OrderTicket.NewObject ();
      newChangedOrderTicket.Int32TransactionProperty = 4711;

      OrderTicket loadedUnchangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      int loadedUnchangedInt32TransactionProperty = loadedUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket loadedChangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      loadedChangedOrderTicket.Int32TransactionProperty = 13;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreSame (loadedUnchangedOrderTicket, OrderTicket.GetObject (DomainObjectIDs.OrderTicket1));
        Assert.AreSame (loadedChangedOrderTicket, OrderTicket.GetObject (DomainObjectIDs.OrderTicket2));

        Assert.AreEqual (newUnchangedInt32TransactionProperty, newUnchangedOrderTicket.Int32TransactionProperty);
        Assert.AreEqual (4711, newChangedOrderTicket.Int32TransactionProperty);
        Assert.AreEqual (loadedUnchangedInt32TransactionProperty, loadedUnchangedOrderTicket.Int32TransactionProperty);
        Assert.AreEqual (13, loadedChangedOrderTicket.Int32TransactionProperty);
      }
    }

    [Test]
    public void PropertyValueChangedAreNotPropagatedToParent ()
    {
      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        newChangedOrder.OrderNumber = 17;
        loadedChangedOrder.OrderNumber = 4;

        using (ClientTransactionMock.EnterDiscardingScope ())
        {
          Assert.AreEqual (4711, newChangedOrder.OrderNumber);
          Assert.AreEqual (13, loadedChangedOrder.OrderNumber);
        }
      }
    }

    [Test]
    public void SubTransactionHasRelatedObjectCollectionEquivalentToParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ObjectList<OrderItem> loadedItems = loadedOrder.OrderItems;

      Assert.AreSame (loadedItems, loadedOrder.OrderItems);

      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];
      OrderItem newItem1 = OrderItem.NewObject ();
      OrderItem newItem2 = OrderItem.NewObject ();
      newItem2.Product = "Baz, buy two get three for free";

      loadedOrder.OrderItems.Clear ();
      loadedOrder.OrderItems.Add (loadedItem2);
      loadedOrder.OrderItems.Add (newItem1);
      loadedOrder.OrderItems.Add (newItem2);

      Order newOrder = Order.NewObject ();
      OrderItem newItem3 = OrderItem.NewObject();
      newItem3.Product = "FooBar, the energy bar with extra Foo";
      newOrder.OrderItems.Add (newItem3);

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreSame (loadedOrder, Order.GetObject (DomainObjectIDs.Order1));
        Assert.AreNotSame (loadedItems, loadedOrder.OrderItems);

        Assert.AreEqual (3, loadedOrder.OrderItems.Count);

        Assert.AreSame (loadedItem2, loadedOrder.OrderItems[0]);
        Assert.AreSame (newItem1, loadedOrder.OrderItems[1]);
        Assert.AreSame (newItem2, loadedOrder.OrderItems[2]);

        Assert.AreSame (loadedOrder, loadedItem2.Order);
        Assert.AreSame (loadedOrder, newItem1.Order);
        Assert.AreSame (loadedOrder, newItem2.Order);

        Assert.AreEqual ("Baz, buy two get three for free", loadedOrder.OrderItems[2].Product);

        Assert.AreEqual (1, newOrder.OrderItems.Count);
        Assert.AreSame (newItem3, newOrder.OrderItems[0]);
        Assert.AreEqual ("FooBar, the energy bar with extra Foo", newOrder.OrderItems[0].Product);
        Assert.AreSame (newOrder, newItem3.Order);
      }
    }

    [Test]
    public void SubTransactionCanGetRelatedObjectCollectionEvenWhenObjectsHaveBeenDiscarded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        orderItem1.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsTrue (orderItem1.IsDiscarded);
        
        ObjectList<OrderItem> orderItems = loadedOrder.OrderItems;
        Assert.AreEqual (1, orderItems.Count);
        Assert.AreEqual (DomainObjectIDs.OrderItem2, orderItems[0].ID);
      }
    }

    [Test]
    public void RelatedObjectCollectionChangesAreNotPropagatedToParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      Assert.AreEqual (2, loadedOrder.OrderItems.Count);
      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];

      Order newOrder = Order.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        loadedOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());

        using (ClientTransactionMock.EnterDiscardingScope ())
        {
          Assert.AreEqual (2, loadedOrder.OrderItems.Count);
          Assert.AreSame (loadedItem1, loadedOrder.OrderItems[0]);
          Assert.AreSame (loadedItem2, loadedOrder.OrderItems[1]);
          Assert.AreEqual (0, newOrder.OrderItems.Count);
        }
      }
    }

    [Test]
    public void SubTransactionHasSameRelatedObjectAsParent1To1 ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee1);
      Assert.AreNotSame (loadedComputer.Employee, loadedEmployee);
      loadedComputer.Employee = loadedEmployee;

      Assert.AreSame (loadedEmployee, loadedComputer.Employee);
      Assert.AreSame (loadedComputer, loadedEmployee.Computer);

      Computer newComputer = Computer.NewObject ();
      Employee newEmployee = Employee.NewObject ();
      newEmployee.Computer = newComputer;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        Assert.AreSame (loadedComputer, Computer.GetObject (DomainObjectIDs.Computer1));
        Assert.AreSame (loadedEmployee, Employee.GetObject (DomainObjectIDs.Employee1));

        Assert.AreSame (loadedEmployee, loadedComputer.Employee);
        Assert.AreSame (loadedComputer, loadedEmployee.Computer);

        Assert.AreSame (newComputer, newEmployee.Computer);
        Assert.AreSame (newEmployee, newComputer.Employee);
      }
    }

    [Test]
    public void RelatedObjectChangesAreNotPropagatedToParent ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer newComputer = Computer.NewObject ();
      Employee newEmployee = Employee.NewObject ();
      newEmployee.Computer = newComputer;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope ())
      {
        loadedComputer.Employee = Employee.NewObject ();
        loadedEmployee.Computer = Computer.NewObject ();

        newComputer.Employee = Employee.NewObject ();
        newEmployee.Computer = Computer.NewObject ();

        using (ClientTransactionMock.EnterDiscardingScope ())
        {
          Assert.AreSame (loadedComputer, loadedEmployee.Computer);
          Assert.AreSame (loadedEmployee, loadedComputer.Employee);

          Assert.AreSame (newComputer, newEmployee.Computer);
          Assert.AreSame (newEmployee, newComputer.Employee);
        }
      }
    }

    [Test]
    public void SubTransactionCreatingEvent ()
    {
      ClientTransaction subTransactionFromEvent = null;

      ClientTransactionMock.SubTransactionCreated += delegate (object sender, SubTransactionCreatedEventArgs args)
      {
        Assert.AreSame (ClientTransactionMock, sender);
        Assert.IsNotNull (args.SubTransaction);
        subTransactionFromEvent = args.SubTransaction;
      };

      Assert.IsNull (subTransactionFromEvent);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.IsNotNull (subTransactionFromEvent);
      Assert.AreSame (subTransaction, subTransactionFromEvent);
    }

    [Test]
    public void GetObjects_UnloadedObjects_PropagatedToParent ()
    {
      MockRepository mockRepository = new MockRepository();
      ClientTransactionMock mockParent = mockRepository.PartialMock<ClientTransactionMock>();

      Expect.Call (mockParent.MockableGetObjects<DomainObject> (null, true)).Constraints (Mocks_List.Count (Mocks_Is.Equal (3)),
          Mocks_Is.Equal (true)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      mockRepository.ReplayAll ();

      ClientTransaction subTransaction = mockParent.CreateSubTransaction();

      using (subTransaction.EnterDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ObjectList<DomainObject> objects = subTransaction.GetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.ClassWithAllDataTypes1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);

        mockRepository.VerifyAll ();

        object[] expectedObjects = new object[]
            {
                Order.GetObject (DomainObjectIDs.Order1),
                ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1),
                Order.GetObject (DomainObjectIDs.Order2),
                OrderItem.GetObject (DomainObjectIDs.OrderItem1)
            };
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void TryObjects_UnloadedObjects_PropagatedToParent ()
    {
      MockRepository mockRepository = new MockRepository ();
      ClientTransactionMock mockParent = mockRepository.PartialMock<ClientTransactionMock> ();

      Expect.Call (mockParent.MockableGetObjects<DomainObject> (null, false)).Constraints (Mocks_List.Count (Mocks_Is.Equal (3)),
          Mocks_Is.Equal (false)).CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      mockRepository.ReplayAll ();

      ClientTransaction subTransaction = mockParent.CreateSubTransaction();

      using (subTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        subTransaction.TryGetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.ClassWithAllDataTypes1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);

        mockRepository.VerifyAll ();
      }
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        ObjectList<DomainObject> objects = subTransaction.GetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);

        Assert.AreEqual (1, eventReceiver.LoadedDomainObjects.Count);
        Assert.That (eventReceiver.LoadedDomainObjects[0], Is.EqualTo (objects));

        listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order1));
        listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.Order2));
        listenerMock.AssertWasCalled (mock => mock.ObjectLoading (DomainObjectIDs.OrderItem1));

        listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.List.Equal (objects)));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        object[] expectedObjects = new object[]
            {
                Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2),
                OrderItem.GetObject (DomainObjectIDs.OrderItem1)
            };
        ObjectList<DomainObject> objects = subTransaction.GetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        Order.GetObject (DomainObjectIDs.Order1);
        Order.GetObject (DomainObjectIDs.Order2);
        OrderItem.GetObject (DomainObjectIDs.OrderItem1);

        eventReceiver.Clear();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        subTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1);
        Assert.That (eventReceiver.LoadedDomainObjects, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.Is.Anything));
      }
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        DomainObject[] expectedObjects = new DomainObject[] {Order.NewObject(), OrderItem.NewObject()};
        ObjectList<DomainObject> objects = subTransaction.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        DomainObject[] expectedObjects = new DomainObject[] {Order.NewObject(), OrderItem.NewObject()};
        eventReceiver.Clear();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        subTransaction.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (eventReceiver.LoadedDomainObjects, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectLoading (Arg<ObjectID>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (Arg<DomainObjectCollection>.Is.Anything));
      }
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'Order|33333333-3333-3333-3333-333333333333|System.Guid' could not be found.\r\n")]
    public void GetObjects_NotFound ()
    {
      Guid guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        subTransaction.GetObjects<DomainObject> (new ObjectID (typeof (Order), guid));
      }
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      Guid guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order newObject = Order.NewObject();
        ObjectList<Order> objects = subTransaction.TryGetObjects<Order> (
            DomainObjectIDs.Order1,
            newObject.ID,
            new ObjectID (typeof (Order), guid),
            DomainObjectIDs.Order2);
        DomainObject[] expectedObjects =
            new DomainObject[] {Order.GetObject (DomainObjectIDs.Order1), newObject, Order.GetObject (DomainObjectIDs.Order2)};
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' "
      + "cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem' or derived from "
      + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'.\r\nParameter name: domainObject")]
    public void GetObjects_InvalidType ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        subTransaction.GetObjects<OrderItem> (DomainObjectIDs.Order1);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException),
        ExpectedMessage = "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted.")]
    public void GetObjects_Deleted ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order.GetObject (DomainObjectIDs.Order1).Delete();
        subTransaction.GetObjects<OrderItem> (DomainObjectIDs.Order1);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException),
       ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is already discarded.")]
    public void GetObjects_Discarded ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete ();
        subTransaction.Commit();
        subTransaction.GetObjects<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionMock cannot be made writeable twice. A common " 
        + ("reason for this error is that a subtransaction is accessed while its parent transaction is engaged in a load operation. During such an " 
        + "operation, the subtransaction cannot be used."))]
    public void Throws_WhenUsedWhileParentIsWriteable ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Type unlockerType = typeof (ClientTransaction).Assembly.GetType ("Remotion.Data.DomainObjects.Infrastructure.TransactionUnlocker");
        object unlocker =
            Activator.CreateInstance (unlockerType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {ClientTransactionMock}, null);
        using ((IDisposable) unlocker)
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionMock cannot be made writeable twice. A common "
    + ("reason for this error is that a subtransaction is accessed while its parent transaction is engaged in a load operation. During such an "
    + "operation, the subtransaction cannot be used."))]
    public void Throws_WhenUsedWhileParentIsWriteable_IntegrationTest ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        ClientTransactionMock.Loaded += delegate
        {
          subTransaction.GetObjects<Order> (DomainObjectIDs.Order1);
        };
        Order.GetObject (DomainObjectIDs.Order2);
      }
    }
  }
}
