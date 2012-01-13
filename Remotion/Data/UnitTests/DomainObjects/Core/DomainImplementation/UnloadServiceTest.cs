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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class UnloadServiceTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _collectionEndPointID;
    private RelationEndPointID _virtualObjectEndPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
    }

    [Test]
    public void UnloadVirtualEndPoint_CollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete (_collectionEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint ()
    {
      EnsureEndPointLoadedAndComplete (_virtualObjectEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_NotLoadedYet ()
    {
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Null);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete (_collectionEndPointID);
      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a virtual end-point.\r\nParameter name: endPointID")]
    public void UnloadVirtualEndPoint_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, objectEndPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The end point with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' has been changed. Changed end points cannot be unloaded.")]
    public void UnloadVirtualEndPoint_Changed ()
    {
      var orders = Customer.GetObject (_collectionEndPointID.ObjectID).Orders;
      orders.Clear ();

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.True);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);
    }

    [Test]
    public void UnloadVirtualEndPoint_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager,_collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (subTransaction, _collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_CollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete (_collectionEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_VirtualObject ()
    {
      EnsureEndPointLoadedAndComplete (_virtualObjectEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_virtualObjectEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete (_collectionEndPointID);
      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);

      var result = UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_Failure ()
    {
      var orders = Customer.GetObject (_collectionEndPointID.ObjectID).Orders;
      orders.Clear ();

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (result, Is.False);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint (TestableClientTransaction, _collectionEndPointID);

      Assert.That (result, Is.True);
      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_Failure_InHigherTransaction ()
    {
      var orders = Customer.GetObject (_collectionEndPointID.ObjectID).Orders;
      orders.Clear ();

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _collectionEndPointID);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.False);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint (subTransaction, _collectionEndPointID);

      Assert.That (result, Is.False);

      Assert.That (subDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Null);
      
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID), Is.Not.Null);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).IsDataComplete, Is.True);
      Assert.That (parentDataManager.GetRelationEndPointWithoutLoading (_collectionEndPointID).HasChanged, Is.True);
    }

    [Test]
    public void UnloadData ()
    {
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (subTransaction, DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Success ()
    {
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Failure ()
    {
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].MarkAsChanged ();
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));

      var result = UnloadService.TryUnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.False);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void TryUnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.True);
      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Failure_InHigherTransaction ()
    {
      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].MarkAsChanged ();
      
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
      
      var result = UnloadService.TryUnloadData (subTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.False);
      Assert.That (subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainers[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection_UnloadsEndPointAndItems ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (orderItemsEndPoint.ID);

      var orderItem1 = orderItemsEndPoint.Collection[0];
      var orderItem2 = orderItemsEndPoint.Collection[1];

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, orderItemsEndPoint.ID);

      Assert.That (orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Collection_EmptyCollection_UnloadsEndPoint ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (ordersEndPoint.ID);

      Assert.That (ordersEndPoint.Collection, Is.Empty);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_UnloadsEndPointAndItem ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointID.Create (order, o => o.OrderTicket);
      EnsureEndPointLoadedAndComplete (endPointID);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      var orderTicket = order.OrderTicket;

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
      Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_NullEndPoint_UnloadsEndPoint ()
    {
      var employee = Employee.GetObject (DomainObjectIDs.Employee1);
      var endPointID = RelationEndPointID.Create (employee, e => e.Computer);
      EnsureEndPointLoadedAndComplete (endPointID);
      Assert.That (employee.Computer, Is.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a virtual end-point.\r\nParameter name: endPointID")]
    public void UnloadVirtualEndPointAndItemData_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, objectEndPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid/' denotes an anonymous end-point, which cannot be unloaded.\r\n"
        + "Parameter name: endPointID")]
    public void UnloadVirtualEndPointAndItemData_AnonymousEndPoint ()
    {
      var anonymousEndPointDefinition = GetEndPointDefinition (typeof (Location), "Client").GetOppositeEndPointDefinition();
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, anonymousEndPointDefinition);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DoesNothing_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DoesNothing_IfDataNotComplete ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_ThrowsAndDoesNothing_IfItemCannotBeUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      var orderA = (Order) ordersEndPoint.Collection[0];
      var orderB = (Order) ordersEndPoint.Collection[1];
      
      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderWithoutOrderItem
      ++orderB.OrderNumber;
      
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));

      Assert.That (() => UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID), Throws.InvalidOperationException);

      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadVirtualEndPointAndItemData (subTransaction, parentOrderItemsEndPoint.ID);

      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_AppliedToParentTransaction_UnloadsFromWholeHierarchy()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, parentOrderItemsEndPoint.ID);

      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Collection ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (orderItemsEndPoint.ID);

      var orderItem1 = orderItemsEndPoint.Collection[0];
      var orderItem2 = orderItemsEndPoint.Collection[1];

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, orderItemsEndPoint.ID);

      Assert.That (result, Is.True);

      Assert.That (orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Collection_Empty ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (ordersEndPoint.ID);

      Assert.That (ordersEndPoint.Collection, Is.Empty);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Object ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointID.Create (order, o => o.OrderTicket);
      EnsureEndPointLoadedAndComplete (endPointID);
      var orderTicket = order.OrderTicket;
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
      Assert.That (orderTicket, Is.Not.Null);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (result, Is.True);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
      Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Object_Null ()
    {
      var employee = Employee.GetObject (DomainObjectIDs.Employee1);
      var endPointID = RelationEndPointID.Create (employee, e => e.Computer);
      EnsureEndPointLoadedAndComplete (endPointID);
      Assert.That (employee.Computer, Is.Null);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (result, Is.True);

      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a virtual end-point.\r\nParameter name: endPointID")]
    public void TryUnloadVirtualEndPointAndItemData_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, objectEndPointID);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, endPointID);

      Assert.That (result, Is.True);
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_IfDataNotComplete ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, ordersEndPoint.ID);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_EndPointChanged_EndPointAndItemsStillLoaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);

      var orderA = customer.Orders[0];
      var orderB = customer.Orders[1];
      customer.Orders.Remove (orderB);

      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      Assert.That (ordersEndPoint.HasChanged, Is.True);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      
      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_ItemCannotBeUnloaded_EndPointAndItemStillLoaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      var orderA = (Order) ordersEndPoint.Collection[0];
      var orderB = (Order) ordersEndPoint.Collection[1];

      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderWithoutOrderItem
      ++orderB.OrderNumber;

      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, ordersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (TestableClientTransaction, parentOrderItemsEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_InHigherTransaction ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var parentOrdersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (parentOrdersEndPoint.ID);

      customer.Orders[0].MarkAsChanged ();

      Assert.That (parentOrdersEndPoint.Collection[0].State, Is.EqualTo (StateType.Changed));

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var subOrdersEndPoint = (ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (subTransaction).GetRelationEndPointWithLazyLoad (
          parentOrdersEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrdersEndPoint.ID);

      Assert.That (subOrdersEndPoint.Collection[0].TransactionContext[subTransaction].State, Is.EqualTo (StateType.Unchanged));

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData (subTransaction, parentOrdersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (subOrdersEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrdersEndPoint.IsDataComplete, Is.True);
    }

    private void EnsureEndPointLoadedAndComplete (RelationEndPointID endPointID)
    {
      var dataManager = TestableClientTransaction.DataManager;
      EnsureEndPointLoadedAndComplete (dataManager, endPointID);
    }

    private void EnsureEndPointLoadedAndComplete (IDataManager dataManager, RelationEndPointID endPointID)
    {
      var endPoint = dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      endPoint.EnsureDataComplete();
    }
  }
}