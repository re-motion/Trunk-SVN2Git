// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class UnloadServiceTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
    }

    [Test]
    public void UnloadCollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete (_endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPoint_NotLoadedYet ()
    {
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Null);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Null);
    }

    [Test]
    public void UnloadCollectionEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete (_endPointID);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void UnloadCollectionEndPoint_ObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, objectEndPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The end point with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders' has been changed. Changed end points cannot be unloaded.")]
    public void UnloadCollectionEndPoint_Changed ()
    {
      var orders = Customer.GetObject (_endPointID.ObjectID).Orders;
      orders.Clear ();

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);
    }

    [Test]
    public void UnloadCollectionEndPoint_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager,_endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPoint (subTransaction, _endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPoint_Success ()
    {
      EnsureEndPointLoadedAndComplete (_endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (result, Is.True);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete (_endPointID);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, _endPointID);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);

      UnloadService.TryUnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPoint_Failure ()
    {
      var orders = Customer.GetObject (_endPointID.ObjectID).Orders;
      orders.Clear ();

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);

      var result = UnloadService.TryUnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (result, Is.False);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);
    }

    [Test]
    public void TryUnloadCollectionEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();

      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadCollectionEndPoint (ClientTransactionMock, _endPointID);

      Assert.That (result, Is.True);
      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPoint_Failure_InHigherTransaction ()
    {
      var orders = Customer.GetObject (_endPointID.ObjectID).Orders;
      orders.Clear ();

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      EnsureEndPointLoadedAndComplete (subDataManager, _endPointID);

      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (subDataManager.RelationEndPointMap[_endPointID].HasChanged, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);

      var result = UnloadService.TryUnloadCollectionEndPoint (subTransaction, _endPointID);

      Assert.That (result, Is.False);
      Assert.That (subDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].IsDataComplete, Is.True);
      Assert.That (subDataManager.RelationEndPointMap[_endPointID].HasChanged, Is.False);
      Assert.That (parentDataManager.RelationEndPointMap[_endPointID].HasChanged, Is.True);
    }

    [Test]
    public void UnloadData ()
    {
      ClientTransactionMock.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (subTransaction, DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Success ()
    {
      ClientTransactionMock.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (result, Is.True);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Failure ()
    {
      ClientTransactionMock.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1].MarkAsChanged ();
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));

      var result = UnloadService.TryUnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (result, Is.False);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void TryUnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData (ClientTransactionMock, DomainObjectIDs.Order1);

      Assert.That (result, Is.True);
      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Failure_InHigherTransaction ()
    {
      ClientTransactionMock.EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1].MarkAsChanged ();
      
      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subDataManager = ClientTransactionTestHelper.GetDataManager (subTransaction);
      var parentDataManager = ClientTransactionMock.DataManager;

      subTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Unchanged));
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
      
      var result = UnloadService.TryUnloadData (subTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.False);
      Assert.That (subDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That (parentDataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_UnloadsEndPointAndItems ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (orderItemsEndPoint.ID);

      var orderItem1 = orderItemsEndPoint.Collection[0];
      var orderItem2 = orderItemsEndPoint.Collection[1];

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, orderItemsEndPoint.ID);

      Assert.That (orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_UnloadsEndPoint_EmptyCollection ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (ordersEndPoint.ID);

      Assert.That (ordersEndPoint.Collection, Is.Empty);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void UnloadCollectionEndPointAndData_ObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, objectEndPointID);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_DoesNothing_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, endPointID);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_DoesNothing_IfDataNotComplete ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_ThrowsAndDoesNothing_IfItemCannotBeUnloaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      var orderA = (Order) ordersEndPoint.Collection[0];
      var orderB = (Order) ordersEndPoint.Collection[1];
      
      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderWithoutOrderItem
      ++orderB.OrderNumber;
      
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));

      try
      {
        UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadCollectionEndPointAndData (subTransaction, parentOrderItemsEndPoint.ID);

      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_AppliedToParentTransaction_UnloadsFromWholeHierarchy()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, parentOrderItemsEndPoint.ID);

      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadCollectionEndPointAndData_EmptyCollection ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var parentOrdersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (parentOrdersEndPoint.ID);

      Assert.That (parentOrdersEndPoint.Collection, Is.Empty);

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subOrdersEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          parentOrdersEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrdersEndPoint.ID);

      UnloadService.UnloadCollectionEndPointAndData (subTransaction, parentOrdersEndPoint.ID);

      Assert.That (subOrdersEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrdersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Success ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (orderItemsEndPoint.ID);

      var orderItem1 = orderItemsEndPoint.Collection[0];
      var orderItem2 = orderItemsEndPoint.Collection[1];

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, orderItemsEndPoint.ID);

      Assert.That (result, Is.True);

      Assert.That (orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem2.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Success_UnloadsEndPoint_EmptyCollection ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer2);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (ordersEndPoint.ID);

      Assert.That (ordersEndPoint.Collection, Is.Empty);

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' does not denote a CollectionEndPoint.\r\nParameter name: endPointID")]
    public void TryUnloadCollectionEndPointAndData_ObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete (objectEndPointID);

      UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, objectEndPointID);
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Success_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, endPointID);

      Assert.That (result, Is.True);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void UnloadCollectionEndPointAndData_Success_IfDataNotComplete ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, ordersEndPoint.ID);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Failure_EndPointChanged_EndPointAndItemsStillLoaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);

      var orderA = customer.Orders[0];
      var orderB = customer.Orders[1];
      customer.Orders.Remove (orderB);

      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      Assert.That (ordersEndPoint.HasChanged, Is.True);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      
      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Failure_ItemCannotBeUnloaded_EndPointAndItemStillLoaded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);

      var orderA = (Order) ordersEndPoint.Collection[0];
      var orderB = (Order) ordersEndPoint.Collection[1];

      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderWithoutOrderItem
      ++orderB.OrderNumber;

      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, ordersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (orderA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderB.State, Is.EqualTo (StateType.Changed));
      Assert.That (ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (order.OrderItems);
      EnsureEndPointLoadedAndComplete (parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrderItemsEndPoint.ID);

      var result = UnloadService.TryUnloadCollectionEndPointAndData (ClientTransactionMock, parentOrderItemsEndPoint.ID);

      Assert.That (result, Is.True);
      Assert.That (subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That (orderItem1.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItem1.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void TryUnloadCollectionEndPointAndData_Failure_InHigherTransaction ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var parentOrdersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (customer.Orders);
      EnsureEndPointLoadedAndComplete (parentOrdersEndPoint.ID);

      customer.Orders[0].MarkAsChanged ();

      Assert.That (parentOrdersEndPoint.Collection[0].State, Is.EqualTo (StateType.Changed));

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      var subOrdersEndPoint = (ICollectionEndPoint) ClientTransactionTestHelper.GetDataManager (subTransaction).RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          parentOrdersEndPoint.ID);
      EnsureEndPointLoadedAndComplete (ClientTransactionTestHelper.GetDataManager (subTransaction), subOrdersEndPoint.ID);

      Assert.That (subOrdersEndPoint.Collection[0].TransactionContext[subTransaction].State, Is.EqualTo (StateType.Unchanged));

      var result = UnloadService.TryUnloadCollectionEndPointAndData (subTransaction, parentOrdersEndPoint.ID);

      Assert.That (result, Is.False);
      Assert.That (subOrdersEndPoint.IsDataComplete, Is.False);
      Assert.That (parentOrdersEndPoint.IsDataComplete, Is.True);
    }

    private void EnsureEndPointLoadedAndComplete (RelationEndPointID endPointID)
    {
      var dataManager = ClientTransactionMock.DataManager;
      EnsureEndPointLoadedAndComplete (dataManager, endPointID);
    }

    private void EnsureEndPointLoadedAndComplete (IDataManager dataManager, RelationEndPointID endPointID)
    {
      dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (dataManager.RelationEndPointMap[endPointID], Is.Not.Null);
      dataManager.RelationEndPointMap[endPointID].EnsureDataComplete();
    }
  }
}