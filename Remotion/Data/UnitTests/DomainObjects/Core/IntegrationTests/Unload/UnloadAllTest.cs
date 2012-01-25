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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Unload
{
  [Ignore ("TODO 4600")]
  [Obsolete ("Not yet implemented.", false)]
  public class UnloadAllTest : UnloadTestBase
  {
    [Test]
    public void TransactionIsCompletelyCleared_BothFromData_AndFromEndPoints ()
    {
      var unchangedObject = LoadOrderWithRelations (DomainObjectIDs.Order1);
      
      var changedObjectDueToDataState = LoadOrderWithRelations (DomainObjectIDs.Order2);
      ++changedObjectDueToDataState.OrderNumber;

      var changedObjectDueToVirtualRelationState = LoadOrderWithRelations (DomainObjectIDs.Order3);
      changedObjectDueToVirtualRelationState.OrderTicket = null;
      
      var newObject = Order.NewObject();
      
      var deletedObject = LoadOrderWithRelations (DomainObjectIDs.Order4);
      deletedObject.Delete();

      var invalidObject = Order.NewObject();
      invalidObject.Delete();

      CheckDataAndEndPoints (unchangedObject, true);
      CheckDataAndEndPoints (changedObjectDueToDataState, true);
      CheckDataAndEndPoints (changedObjectDueToVirtualRelationState, true);
      CheckDataAndEndPoints (newObject, true);
      CheckDataAndEndPoints (deletedObject, true);
      CheckDataAndEndPoints (invalidObject, false);

      UnloadService.UnloadAll (TestableClientTransaction);

      CheckDataAndEndPoints (unchangedObject, false);
      CheckDataAndEndPoints (changedObjectDueToDataState, false);
      CheckDataAndEndPoints (changedObjectDueToVirtualRelationState, false);
      CheckDataAndEndPoints (newObject, false);
      CheckDataAndEndPoints (deletedObject, false);
      CheckDataAndEndPoints (invalidObject, false);

      CheckTransactionIsEmpty();
    }

    [Test]
    public void EnlistedObjects_AreKept ()
    {
      var unchangedObject = LoadOrderWithRelations (DomainObjectIDs.Order1);

      var changedObjectDueToDataState = LoadOrderWithRelations (DomainObjectIDs.Order2);
      ++changedObjectDueToDataState.OrderNumber;

      var changedObjectDueToVirtualRelationState = LoadOrderWithRelations (DomainObjectIDs.Order3);
      changedObjectDueToVirtualRelationState.OrderTicket = null;

      var newObject = Order.NewObject ();

      var deletedObject = LoadOrderWithRelations (DomainObjectIDs.Order4);
      deletedObject.Delete ();

      var invalidObject = Order.NewObject ();
      invalidObject.Delete ();

      UnloadService.UnloadAll (TestableClientTransaction);

      Assert.That (TestableClientTransaction.IsEnlisted (unchangedObject), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (changedObjectDueToDataState), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (changedObjectDueToVirtualRelationState), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (newObject), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (deletedObject), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (invalidObject), Is.True);
    }

    [Test]
    public void StatesOfUnloadedObjects_AreSetToNotLoadedYet_OrInvalid ()
    {
      var unchangedObject = LoadOrderWithRelations (DomainObjectIDs.Order1);

      var changedObjectDueToDataState = LoadOrderWithRelations (DomainObjectIDs.Order2);
      ++changedObjectDueToDataState.OrderNumber;

      var changedObjectDueToVirtualRelationState = LoadOrderWithRelations (DomainObjectIDs.Order3);
      changedObjectDueToVirtualRelationState.OrderTicket = null;

      var newObject = Order.NewObject ();

      var deletedObject = LoadOrderWithRelations (DomainObjectIDs.Order4);
      deletedObject.Delete ();

      var invalidObject = Order.NewObject ();
      invalidObject.Delete ();

      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (changedObjectDueToDataState.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedObjectDueToVirtualRelationState.State, Is.EqualTo (StateType.Changed));
      Assert.That (deletedObject.State, Is.EqualTo (StateType.Deleted));
      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      Assert.That (invalidObject.State, Is.EqualTo (StateType.Invalid));

      UnloadService.UnloadAll (TestableClientTransaction);

      Assert.That (unchangedObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (changedObjectDueToDataState.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (changedObjectDueToVirtualRelationState.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (deletedObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (newObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (invalidObject.State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void ModifiedData_AndEndPoints_AreRolledBack_AndReloadedOnAccess ()
    {
      var order = LoadOrderWithRelations (DomainObjectIDs.Order1);
      order.OrderTicket = null;
      order.OrderItems.Clear ();
      order.OrderNumber = 0;
      CheckDataAndEndPoints (order, true);

      UnloadService.UnloadAll (TestableClientTransaction);

      CheckDataAndEndPoints (order, false);

      Assert.That (order.OrderTicket, Is.Not.Null.And.Property ("ID").EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (order.OrderItems, Is.Not.Empty.And.Count.EqualTo (2));
      Assert.That (order.OrderNumber, Is.EqualTo (1));

      CheckDataAndEndPoints (order, true);
    }

    [Test]
    public void CollectionReferences_AreKeptValid_AndCanBeAccessed_ViaRelation_OrViaCollection ()
    {
      var object1 = LoadOrderWithRelations (DomainObjectIDs.Order1);
      var collection1 = object1.OrderItems;

      var object2 = LoadOrderWithRelations (DomainObjectIDs.Order2);
      var collection2 = object1.OrderItems;
      collection2.Clear();

      UnloadService.UnloadAll (TestableClientTransaction);

      Assert.That (object1.OrderItems, Is.SameAs (collection1));
      Assert.That (collection2, Is.Not.Empty);
      Assert.That (collection2, Is.EqualTo (object2.OrderItems));
    }

    [Test]
    public void ChangedCollectionReferences_AreRolledBack_AndReloadedOnAccess ()
    {
      var order = LoadOrderWithRelations (DomainObjectIDs.Order1);
      var oldCollection = order.OrderItems;
      order.OrderItems = new ObjectList<OrderItem>();
      Assert.That (order.OrderItems, Is.Not.SameAs (oldCollection));
      Assert.That (order.OrderItems, Is.Not.Count.EqualTo (2));

      UnloadService.UnloadAll (TestableClientTransaction);

      Assert.That (order.OrderItems, Is.SameAs (oldCollection));
      Assert.That (order.OrderItems, Has.Count.EqualTo (2));
    }

    [Test]
    public void Unload_AffectsWholeHierarchy ()
    {
      var orderChangedInRoot = LoadOrderWithRelations (DomainObjectIDs.Order1);
      orderChangedInRoot.OrderTicket = null;
      orderChangedInRoot.OrderItems.Clear();
      orderChangedInRoot.OrderNumber = 0;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var middleTransaction = ClientTransaction.Current;
        var orderChangedInMiddle = LoadOrderWithRelations (DomainObjectIDs.Order2);
        orderChangedInMiddle.OrderTicket = null;
        orderChangedInMiddle.OrderItems.Clear();
        orderChangedInMiddle.OrderNumber = 0;

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var orderChangedInSubSub = LoadOrderWithRelations (DomainObjectIDs.Order3);
          orderChangedInSubSub.OrderTicket = null;
          orderChangedInSubSub.OrderItems.Clear();
          orderChangedInSubSub.OrderNumber = 0;

          UnloadService.UnloadAll (middleTransaction);

          CheckTransactionIsEmpty ();
          Assert.That (orderChangedInSubSub.OrderNumber, Is.EqualTo (4));
        }

        CheckTransactionIsEmpty ();
        Assert.That (orderChangedInMiddle.OrderNumber, Is.EqualTo (3));
      }

      CheckTransactionIsEmpty ();
      Assert.That (orderChangedInRoot.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void UnloadFromHierarchy_WithObjectLoadedinMultipleTransactions_UnloadsTheObjectFromWholeHierarchy ()
    {
      var orderChangedInRootAndSubSub = LoadOrderWithRelations (DomainObjectIDs.Order1);
      orderChangedInRootAndSubSub.OrderNumber = 0;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        orderChangedInRootAndSubSub.OrderNumber = 1001;

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          UnloadService.UnloadAll (ClientTransaction.Current);
          CheckTransactionIsEmpty ();
        }

        CheckTransactionIsEmpty ();
      }

      CheckTransactionIsEmpty ();
      Assert.That (orderChangedInRootAndSubSub.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void UnloadFromHierarchy_WithNewObjects_MarksObjectsInvalidInWholeHierarchy ()
    {
      Order newObject;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var middleTopTransaction = ClientTransaction.Current;
        newObject = Order.NewObject ();

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          var middleBottomTransaction = ClientTransaction.Current;
          newObject.EnsureDataAvailable();

          using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
          {
            Assert.That (newObject.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.Invalid));
            Assert.That (newObject.TransactionContext[middleTopTransaction].State, Is.EqualTo (StateType.New));
            Assert.That (newObject.TransactionContext[middleBottomTransaction].State, Is.EqualTo (StateType.Unchanged));
            Assert.That (newObject.TransactionContext[ClientTransaction.Current].State, Is.EqualTo (StateType.NotLoadedYet));

            UnloadService.UnloadAll (ClientTransaction.Current);

            Assert.That (newObject.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.Invalid));
            Assert.That (newObject.TransactionContext[middleTopTransaction].State, Is.EqualTo (StateType.Invalid));
            Assert.That (newObject.TransactionContext[middleBottomTransaction].State, Is.EqualTo (StateType.Invalid));
            Assert.That (newObject.TransactionContext[ClientTransaction.Current].State, Is.EqualTo (StateType.Invalid));
          }
        }
      }
    }

    [Test]
    public void Events ()
    {
      var order1 = LoadOrderWithRelations (DomainObjectIDs.Order1);
      var order2 = LoadOrderWithRelations (DomainObjectIDs.Order2);
      
      var mockRepository = new MockRepository();
      var clientTransactionListener = mockRepository.StrictMock<IClientTransactionListener>();
      var unloadEventReceiver = mockRepository.StrictMock<IUnloadEventReceiver>();

      TestableClientTransaction.AddListener (clientTransactionListener);
      order1.SetUnloadEventReceiver (unloadEventReceiver);
      order2.SetUnloadEventReceiver (unloadEventReceiver);
      
      using (mockRepository.Ordered ())
      {
        clientTransactionListener
            .Expect (
                mock =>
                mock.ObjectsUnloading (
                    Arg.Is (TestableClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order1, order2 })));
        unloadEventReceiver.Expect (mock => mock.OnUnloading (order1));
        unloadEventReceiver.Expect (mock => mock.OnUnloading (order2));

        using (mockRepository.Unordered ())
        {
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order1, o => o.Customer)));
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order1, o => o.Official)));
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order1, o => o.OrderTicket)));
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order2, o => o.Customer)));
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order2, o => o.Official)));
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (TestableClientTransaction, RelationEndPointID.Resolve (order2, o => o.OrderTicket)));

          clientTransactionListener.Expect (mock => mock.DataContainerMapUnregistering (TestableClientTransaction, order1.InternalDataContainer));
          clientTransactionListener.Expect (mock => mock.DataContainerMapUnregistering (TestableClientTransaction, order2.InternalDataContainer));
        }

        unloadEventReceiver.Expect (mock => mock.OnUnloaded (order2));
        unloadEventReceiver.Expect (mock => mock.OnUnloaded (order1));
        clientTransactionListener
            .Expect (
                mock =>
                mock.ObjectsUnloaded (
                    Arg.Is (TestableClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order1, order2 })));
      }
      mockRepository.ReplayAll();

      UnloadService.UnloadAll (TestableClientTransaction);

      mockRepository.VerifyAll();
    }

    [Test]
    public void Events_New ()
    {
      var order = Order.NewObject();

      var mockRepository = new MockRepository ();
      var clientTransactionListener = mockRepository.StrictMock<IClientTransactionListener> ();
      var unloadEventReceiver = mockRepository.StrictMock<IUnloadEventReceiver> ();

      TestableClientTransaction.AddListener (clientTransactionListener);
      order.SetUnloadEventReceiver (unloadEventReceiver);

      using (mockRepository.Ordered ())
      {
        clientTransactionListener
            .Expect (
                mock =>
                mock.ObjectsUnloading (
                    Arg.Is (TestableClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order })));
        unloadEventReceiver.Expect (mock => mock.OnUnloading (order));

        using (mockRepository.Unordered ())
        {
          clientTransactionListener.Expect (
              mock => mock.RelationEndPointMapUnregistering (Arg.Is (TestableClientTransaction), Arg<RelationEndPointID>.Is.Anything)).Repeat.AtLeastOnce();

          clientTransactionListener.Expect (mock => mock.DataContainerMapUnregistering (TestableClientTransaction, order.InternalDataContainer));
          clientTransactionListener.Expect (mock => mock.DataManagerDiscardingObject (TestableClientTransaction, order.ID));
        }

        unloadEventReceiver.Expect (mock => mock.OnUnloaded (order));
        clientTransactionListener
            .Expect (
                mock =>
                mock.ObjectsUnloaded (
                    Arg.Is (TestableClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order })));
      }
      mockRepository.ReplayAll ();

      UnloadService.UnloadAll (TestableClientTransaction);

      mockRepository.VerifyAll ();
    }

    [Test]
    public void Events_New_WithHierarchy ()
    {
      Order newObject;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var middleTopTransaction = ClientTransaction.Current;
        newObject = Order.NewObject ();

        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          var middleBottomTransaction = ClientTransaction.Current;
          newObject.EnsureDataAvailable ();

          using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
          {
            var subSubTransaction = ClientTransaction.Current;

            Assert.That (newObject.TransactionContext[TestableClientTransaction].State, Is.EqualTo (StateType.Invalid));
            Assert.That (newObject.TransactionContext[middleTopTransaction].State, Is.EqualTo (StateType.New));
            Assert.That (newObject.TransactionContext[middleBottomTransaction].State, Is.EqualTo (StateType.Unchanged));
            Assert.That (newObject.TransactionContext[subSubTransaction].State, Is.EqualTo (StateType.NotLoadedYet));

            var clientTransactionListener = MockRepository.GenerateMock<IClientTransactionListener> ();
            TestableClientTransaction.AddListener (clientTransactionListener);
            ClientTransactionTestHelper.AddListener (middleTopTransaction, clientTransactionListener);
            ClientTransactionTestHelper.AddListener (middleBottomTransaction, clientTransactionListener);
            ClientTransactionTestHelper.AddListener (subSubTransaction, clientTransactionListener);

            UnloadService.UnloadAll (ClientTransaction.Current);

            clientTransactionListener.AssertWasCalled (mock => mock.DataManagerDiscardingObject (middleTopTransaction, newObject.ID));
            clientTransactionListener.AssertWasCalled (
                mock => mock.DataContainerMapUnregistering (Arg.Is (middleTopTransaction), Arg<DataContainer>.Matches (dc => dc.ID == newObject.ID)));
            
            clientTransactionListener.AssertWasCalled (
                mock => mock.DataContainerMapUnregistering (Arg.Is (middleBottomTransaction), Arg<DataContainer>.Matches (dc => dc.ID == newObject.ID)));

            //// TODO 4610: Add ObjectMarkedInvalid
            //clientTransactionListener.AssertWasCalled (mock => mock.ObjectMarkedInvalid (subSubTransaction, newObject.ID));
          }
        }
      }
    }

    [Test]
    public void Events_Cancellation ()
    {
      var order = LoadOrderWithRelations (DomainObjectIDs.Order1);

      var mockRepository = new MockRepository ();
      var clientTransactionListener = mockRepository.StrictMock<IClientTransactionListener> ();
      var unloadEventReceiver = mockRepository.StrictMock<IUnloadEventReceiver> ();

      var exception = new Exception ("Test");

      order.SetUnloadEventReceiver (unloadEventReceiver);

      using (mockRepository.Ordered ())
      {
        clientTransactionListener
            .Expect (
                mock =>
                mock.ObjectsUnloading (
                    Arg.Is (TestableClientTransaction), Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { order })));
        unloadEventReceiver.Expect (mock => mock.OnUnloading (order)).Throw (exception);
      }
      mockRepository.ReplayAll ();

      Assert.That (() => UnloadService.UnloadAll (TestableClientTransaction), Throws.Exception.SameAs (exception));

      mockRepository.VerifyAll ();

      CheckDataAndEndPoints (order, true);
    }

    private void CheckDataAndEndPoints (Order order, bool shouldBePresent)
    {
      CheckDataContainerExists (order, shouldBePresent);
      CheckVirtualEndPointExistsAndComplete (order, "OrderItems", shouldBePresent, shouldBePresent);
      CheckVirtualEndPointExistsAndComplete (order, "OrderTicket", shouldBePresent, shouldBePresent);
      CheckEndPointExists (order, "Customer", shouldBePresent);
    }

    private Order LoadOrderWithRelations (ObjectID objectID)
    {
      var order = Order.GetObject (objectID);
      order.EnsureDataAvailable ();
      ClientTransaction.Current.EnsureDataComplete (RelationEndPointID.Resolve (order, o => o.OrderTicket));
      ClientTransaction.Current.EnsureDataComplete (RelationEndPointID.Resolve (order, o => o.OrderItems));
      ClientTransaction.Current.EnsureDataComplete (RelationEndPointID.Resolve (order, o => o.Customer));
      return order;
    }

    private void CheckTransactionIsEmpty ()
    {
      Assert.That (TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That (TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }
  }
}