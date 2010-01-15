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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  /// <summary>
  /// 
  /// </summary>
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
    }

    [Test]
    public void GetEmptyDomainObjectsFromStateTypeOverload ()
    {
      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetUnchangedDomainObjectsFromStateTypeOverload ()
    {
      DataContainer container = CreateOrder1DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container.DomainObject));
    }

    [Test]
    public void GetUnchangedDomainObjects ()
    {
      DataContainer container = CreateOrder1DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container.DomainObject));
    }

    [Test]
    public void GetChangedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = CreateOrder1DataContainer ();
      DataContainer container2 = CreateOrder2DataContainer();

      container1.SetValue (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber",
          (int) container1.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber") + 1);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Changed);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container1.DomainObject));

      domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container2.DomainObject));    
    }

    [Test]
    public void GetDeletedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = CreateClassWithAllDataTypesDataContainer();
      DataContainer container2 = CreateOrder2DataContainer ();

      _dataManager.Delete (container1.DomainObject);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Deleted);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container1.DomainObject));

      domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container2.DomainObject));
    }

    [Test]
    public void GetNewAndUnchangedDomainObjects ()
    {
			DataContainer container1 = Order.NewObject ().InternalDataContainer;
      DataContainer container2 = CreateOrder2DataContainer ();

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.New);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container1.DomainObject));

      domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.That (domainObjects, Is.Not.Null);
      Assert.That (domainObjects.Count, Is.EqualTo (1));
      Assert.That (domainObjects[0], Is.SameAs (container2.DomainObject));
    }

    [Test]
    public void GetEmptyChangedDomainObjects ()
    {
      Assert.That (_dataManager.GetChangedDomainObjects ().Count, Is.EqualTo (0));
    }

    [Test]
    public void GetChangedDomainObjects ()
    {
      DataContainer container = CreateOrder1DataContainer ();
      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 42;

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.That (changedObjects.Count, Is.EqualTo (1));
      Assert.That (changedObjects[0].ID, Is.EqualTo (container.ID));
    }

    [Test]
    public void GetChangedDomainObjectsForMultipleObjects ()
    {
      CreateOrder1DataContainer ();
      DataContainer container2 = CreateOrderTicket1DataContainer ();

      container2["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.FileName"] = @"C:\NewFile.jpg";

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.That (changedObjects.Count, Is.EqualTo (1));
      Assert.That (changedObjects[0].ID, Is.EqualTo (container2.ID));
    }

    [Test]
    public void GetChangedDomainObjectsForRelationChange ()
    {
      DataContainer order1 = CreateOrder1DataContainer ();
      CreateOrderTicket1DataContainer ();
      DataContainer orderTicket2 = CreateOrderTicket2DataContainer ();
      CreateOrderWithoutOrderItemDataContainer();

      var order1EndPointID = new RelationEndPointID (order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      ((ObjectEndPoint) _dataManager.RelationEndPointMap[order1EndPointID]).SetOppositeObjectAndNotify (orderTicket2.DomainObject);

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.That (changedObjects.Count, Is.EqualTo (4));
    }

    [Test]
    public void GetChangedRelationEndPoints ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);

      OrderItem orderItem1 = order1.OrderItems[0];
      OrderTicket orderTicket = order1.OrderTicket;

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;

      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Dev.Null = location.Client;

      Assert.IsEmpty (new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ()));

      orderItem1.Order = null; // 2 endpoints
      orderTicket.Order = null; // 2 endpoints

      computer.Employee = Employee.NewObject (); // 3 endpoints
      employee.Computer = null; // (1 endpoint)

      location.Client = Client.NewObject (); // 1 endpoint

      var changedEndPoints = new List<RelationEndPoint> (_dataManager.GetChangedRelationEndPoints ());

      Assert.That (changedEndPoints.Count, Is.EqualTo (8));

      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderItems")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderItem1.ID, typeof (OrderItem) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (orderTicket.ID, typeof (OrderTicket) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (order1.ID, typeof (Order) + ".OrderTicket")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.ID, typeof (Computer) + ".Employee")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (computer.Employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[new RelationEndPointID (location.ID, typeof (Location) + ".Client")],
          changedEndPoints);
    }

    [Test]
    public void RegisterDataContainer_RegistersDataContainerInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dataContainer));
    }

    [Test]
    public void RegisterDataContainer_RegistersEndPointsInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);
    }

    [Test]
    public void Unregister_NotLoaded ()
    {
      var listenerMock = new MockRepository ().StrictMock<IClientTransactionListener> ();
      ClientTransactionTestHelper.AddListener (_dataManager.ClientTransaction, listenerMock);
      listenerMock.Replay (); // no events expected

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      _dataManager.Unregister (DomainObjectIDs.Order1);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Unregister_RemovesDataContainer ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Not.Null);

      _dataManager.Unregister (DomainObjectIDs.Order1);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Unregister_RemovesRelationsWithForeignKeys ()
    {
      var realEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var oppositeVirtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      EnsureDataAvailable (DomainObjectIDs.OrderTicket1);
      Assert.That (_dataManager.RelationEndPointMap[realEndPointID], Is.Not.Null);
      Assert.That (_dataManager.RelationEndPointMap[oppositeVirtualEndPointID], Is.Not.Null);

      _dataManager.Unregister (DomainObjectIDs.OrderTicket1);

      Assert.That (_dataManager.RelationEndPointMap[realEndPointID], Is.Null);
      Assert.That (_dataManager.RelationEndPointMap[oppositeVirtualEndPointID], Is.Null);
    }

    [Test]
    public void Unregister_KeepsRelationsWithoutForeignKeys ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var oppositeRealEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

      EnsureDataAvailable (DomainObjectIDs.OrderTicket1);
      Assert.That (_dataManager.RelationEndPointMap[oppositeRealEndPointID], Is.Not.Null);
      Assert.That (_dataManager.RelationEndPointMap[virtualEndPointID], Is.Not.Null);

      _dataManager.Unregister (DomainObjectIDs.Order1);

      Assert.That (_dataManager.RelationEndPointMap[oppositeRealEndPointID], Is.Not.Null);
      Assert.That (_dataManager.RelationEndPointMap[virtualEndPointID], Is.Not.Null);
    }

    [Test]
    public void Unregister_RemovesNullVirtualEndPoints ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee1, "Computer");
      EnsureDataAvailable (DomainObjectIDs.Employee1);

      _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (virtualEndPointID);

      Assert.That (_dataManager.RelationEndPointMap[virtualEndPointID], Is.Not.Null);
      Assert.That (((ObjectEndPoint) _dataManager.RelationEndPointMap[virtualEndPointID]).OppositeObjectID, Is.Null);

      _dataManager.Unregister (DomainObjectIDs.Employee1);

      Assert.That (_dataManager.RelationEndPointMap[virtualEndPointID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of DataContainer 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is 'Changed'. Only unchanged DataContainers can be "
        + "unloaded.")]
    public void Unregister_NotUnchanged ()
    {
      EnsureDataAvailable (DomainObjectIDs.Order1);
      _dataManager.DataContainerMap[DomainObjectIDs.Order1].MarkAsChanged ();
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1].State, Is.EqualTo (StateType.Changed));
      
      _dataManager.Unregister (DomainObjectIDs.Order1);
    }

    [Test]
    public void Unregister_UnloadsOwningCollections ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (collectionEndPointID);

      Assert.That (collectionEndPoint.HasChanged, Is.False);
      Assert.That (collectionEndPoint.IsDataAvailable, Is.True);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));

      _dataManager.Unregister (DomainObjectIDs.OrderItem1);

      Assert.That (collectionEndPoint.IsDataAvailable, Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The data of object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded because the following relation end "
        + "points have changed: "
        + "Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems")]
    public void Unregister_ChangedCollection ()
    {
      var collectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var collectionEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (collectionEndPointID);
      collectionEndPoint.OppositeDomainObjects.Add (OrderItem.NewObject ());

      Assert.That (collectionEndPoint.HasChanged, Is.True);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));

      _dataManager.Unregister (DomainObjectIDs.OrderItem1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The data of object 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid' cannot be unloaded because the following relation end points "
        + "have changed: Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer")]
    public void Unregister_ChangedVirtualNullEndPoint ()
    {
      var virtualEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Employee3, "Computer");
      
      var virtualEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (virtualEndPointID);
      Assert.That (virtualEndPoint.OppositeObjectID, Is.Not.Null);
      virtualEndPoint.OppositeObjectID = null;

      Assert.That (virtualEndPoint.HasChanged, Is.True);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Employee3].State, Is.EqualTo (StateType.Unchanged));

      _dataManager.Unregister (DomainObjectIDs.Employee3);
    }

    [Test]
    public void GetChangedDataContainersForCommitWithDeletedObject ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem1.Delete ();

      _dataManager.GetChangedDataContainersForCommit ();

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Cannot delete DomainObject '.*', because it belongs to a different ClientTransaction.",
       MatchType = MessageMatch.Regex)]
    public void DeleteWithOtherClientTransaction ()
    {
      Order order1;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
      }
      _dataManager.Delete (order1);
    }

    [Test]
    public void DeleteWithOtherClientTransaction_UsesStoredTransaction ()
    {
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        _dataManager.Delete (order1); // deletes in _dataManager's transaction, not in current transaction
      }
      Assert.That (Order.GetObject (DomainObjectIDs.Order1, true).State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void IsDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.False);
      Assert.That (_dataManager.DiscardedObjectCount, Is.EqualTo (0));

      _dataManager.Rollback ();

      Assert.That (_dataManager.IsDiscarded (dataContainer.ID), Is.True);
      Assert.That (_dataManager.DiscardedObjectCount, Is.EqualTo (1));
    }

    [Test]
    public void GetDiscardedDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);
      _dataManager.Rollback ();

      Assert.That (_dataManager.GetDiscardedDataContainer (dataContainer.ID), Is.SameAs (dataContainer));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been discarded.\r\nParameter name: id")]
    public void GetDiscardedDataContainerThrowsWhenNotDiscarded ()
    {
      _dataManager.GetDiscardedDataContainer (DomainObjectIDs.Order1);
    }

    [Test]
    public void Commit_CommitsRelationEndPointMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Order2;

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Commit ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Commit_CommitsDataContainerMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));
      
      _dataManager.Commit ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Commit_RemovesDeletedEndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Commit_DiscardsDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Commit_MarksDeletedDataContainersAsDiscarded ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.False);

      _dataManager.Commit ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void Rollback_RollsBackRelationEndPointStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.OppositeObjectID = DomainObjectIDs.Order2;

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Rollback ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void Rollback_RollsBackDataContainerStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      _dataManager.RegisterDataContainer (dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManager.Rollback ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Rollback_RemovesNewEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = new RelationEndPointID (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Rollback ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _dataManager.Rollback ();

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Rollback_DiscardsNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Rollback ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Rollback_MarksNewDataContainersAsDiscarded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.False);

      _dataManager.Rollback ();

      Assert.That (_dataManager.IsDiscarded (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void Delete_DeletedObject ()
    {
      var deletedObject = Order.GetObject (DomainObjectIDs.Order1);
      deletedObject.Delete ();
      Assert.That (deletedObject.State, Is.EqualTo (StateType.Deleted));

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _dataManager.Delete (deletedObject);
      Assert.That (deletedObject.State, Is.EqualTo (StateType.Deleted));

      listenerMock.AssertWasNotCalled (mock => mock.ObjectDeleting (deletedObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void Delete_DiscardedObject ()
    {
      var discardedObject = Order.NewObject ();
      discardedObject.Delete ();
      Assert.That (discardedObject.IsDiscarded, Is.True);

      _dataManager.Delete (discardedObject);
    }

    private DataContainer CreateOrder1DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));

      return dataContainer;
    }

    private DataContainer CreateOrderTicket1DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      return dataContainer;
    }

    private DataContainer CreateOrderTicket2DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderTicket2DataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      return dataContainer;
    }

    private DataContainer CreateOrder2DataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrder2DataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      return dataContainer;
    }

    private void CreateOrderWithoutOrderItemDataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
    }

    private DataContainer CreateClassWithAllDataTypesDataContainer ()
    {
      var dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      dataContainer.RegisterWithTransaction (_dataManager.ClientTransaction);
      dataContainer.SetDomainObject (ClientTransactionMock.GetObjectForDataContainer (dataContainer));
      return dataContainer;
    }

    private void EnsureDataAvailable (ObjectID objectID)
    {
      _dataManager.ClientTransaction.EnsureDataAvailable (objectID);
    }

  }
}
