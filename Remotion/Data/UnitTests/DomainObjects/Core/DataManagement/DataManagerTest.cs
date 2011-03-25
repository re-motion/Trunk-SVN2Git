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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private IInvalidDomainObjectManager _invalidDomainObjectManager;
    private DataManager _dataManager;
    private DataManager _dataManagerWitLoaderMock;
    private IObjectLoader _objectLoaderMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
      _invalidDomainObjectManager = (IInvalidDomainObjectManager) PrivateInvoke.GetNonPublicField (_dataManager, "_invalidDomainObjectManager");

      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader> ();
      _dataManagerWitLoaderMock = new DataManager (
          ClientTransactionMock,
          new RootCollectionEndPointChangeDetectionStrategy (),
          new RootInvalidDomainObjectManager (),
          _objectLoaderMock);
    }

    [Test]
    public void Initialization ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var collectionEndPointChangeDetectionStrategy = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy> ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();

      var dataManager = new DataManager (clientTransaction, collectionEndPointChangeDetectionStrategy, invalidDomainObjectManager, objectLoader);

      Assert.That (dataManager.RelationEndPointMap, Is.TypeOf (typeof (RelationEndPointMap)));
      var map = (RelationEndPointMap) dataManager.RelationEndPointMap;

      Assert.That (map.ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (map.ObjectLoader, Is.SameAs (objectLoader));
      Assert.That (map.EndPointProvider, Is.SameAs (dataManager));
      Assert.That (map.CollectionEndPointDataKeeperFactory, Is.TypeOf (typeof (CollectionEndPointDataKeeperFactory)));
      Assert.That (((CollectionEndPointDataKeeperFactory) map.CollectionEndPointDataKeeperFactory).ClientTransaction, Is.SameAs (clientTransaction));
      Assert.That (((CollectionEndPointDataKeeperFactory) map.CollectionEndPointDataKeeperFactory).ChangeDetectionStrategy, 
          Is.SameAs (collectionEndPointChangeDetectionStrategy));
      Assert.That (map.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf (typeof (VirtualObjectEndPointDataKeeperFactory)));
      Assert.That (((VirtualObjectEndPointDataKeeperFactory) map.VirtualObjectEndPointDataKeeperFactory).ClientTransaction, Is.SameAs (clientTransaction));
    }

    [Test]
    public void DomainObjectStateCache ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (_dataManager.DomainObjectStateCache.GetState (order1.ID), Is.EqualTo (StateType.Unchanged));

      var propertyName = ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderNumber");
      _dataManager.DataContainerMap[order1.ID].PropertyValues[propertyName].Value = 100;

      Assert.That (_dataManager.DomainObjectStateCache.GetState (order1.ID), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetLoadedDomainObjects_Empty ()
    {
      var loadedDomainObjects = _dataManager.GetLoadedData ();
      Assert.That (loadedDomainObjects.ToArray (), Is.Empty);
    }

    [Test]
    public void GetLoadedDomainObjects_NonEmpty ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

      var loadedDomainObjects = _dataManager.GetLoadedData ();

      var expected = new[] { 
          new Tuple<DomainObject, DataContainer> (order1, order1.InternalDataContainer), 
          new Tuple<DomainObject, DataContainer> (orderItem1, orderItem1.InternalDataContainer) };
      Assert.That (loadedDomainObjects.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetLoadedDomainObjects_WithStates ()
    {
      var unchangedInstance = DomainObjectMother.GetUnchangedObject(ClientTransactionMock, DomainObjectIDs.Order1);
      var changedInstance = DomainObjectMother.GetChangedObject(ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject();
      var deletedInstance = DomainObjectMother.GetDeletedObject(ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      var unchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Unchanged);
      var changedOrNewObjects = _dataManager.GetLoadedDataByObjectState (StateType.Changed, StateType.New);
      var deletedOrUnchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Deleted, StateType.Unchanged);

      Assert.That (unchangedObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (unchangedInstance) }));
      Assert.That (changedOrNewObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (changedInstance), CreateDataTuple (newInstance) }));
      Assert.That (deletedOrUnchangedObjects.ToArray (), Is.EquivalentTo (new[] { CreateDataTuple (deletedInstance), CreateDataTuple (unchangedInstance) }));
    }

    [Test]
    public void GetChangedData_Empty ()
    {
      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      Assert.That (changedDomainObjects.ToArray(), Is.Empty);
    }

    [Test]
    public void GetChangedData ()
    {
      var changedInstance = DomainObjectMother.GetChangedObject (ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject ();
      var deletedInstance = DomainObjectMother.GetDeletedObject (ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (ClientTransactionMock, DomainObjectIDs.Order1);
      DomainObjectMother.GetInvalidObject (ClientTransactionMock);
      DomainObjectMother.GetNotLoadedObject (ClientTransactionMock, DomainObjectIDs.Order2);

      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      
      var expected = new[] { 
          CreateDataTuple (changedInstance), 
          CreateDataTuple (newInstance),
          CreateDataTuple (deletedInstance) };
      Assert.That (changedDomainObjects.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetChangedData_ReturnsObjectsChangedByRelation ()
    {
      var orderWithChangedRelation = Order.GetObject (DomainObjectIDs.Order1);

      orderWithChangedRelation.OrderTicket = null;
      Assert.That (orderWithChangedRelation.State, Is.EqualTo (StateType.Changed));
      Assert.That (orderWithChangedRelation.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      var changedDomainObjects = _dataManager.GetChangedDataByObjectState ();
      
      var expected = CreateDataTuple (orderWithChangedRelation);
      Assert.That (changedDomainObjects.ToArray (), List.Contains (expected));
    }

    [Test]
    public void GetChangedDataContainersForCommit_ReturnsObjectsToBeCommitted ()
    {
      var changedInstance = (TestDomainBase) DomainObjectMother.GetChangedObject (ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = (TestDomainBase) DomainObjectMother.GetNewObject ();
      var deletedInstance = (TestDomainBase) DomainObjectMother.GetDeletedObject (ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (ClientTransactionMock, DomainObjectIDs.Order1);
      DomainObjectMother.GetInvalidObject (ClientTransactionMock);
      DomainObjectMother.GetNotLoadedObject (ClientTransactionMock, DomainObjectIDs.Order2);

      var result = _dataManager.GetChangedDataContainersForCommit ();

      var expected = new[] { changedInstance.InternalDataContainer, newInstance.InternalDataContainer, deletedInstance.InternalDataContainer };
      Assert.That (result.ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetChangedDataContainersForCommit_DoesNotReturnRelationChangedObjects ()
    {
      var orderWithChangedRelation = Order.GetObject (DomainObjectIDs.Order1);

      orderWithChangedRelation.OrderItems.Add (OrderItem.NewObject ());
      Assert.That (orderWithChangedRelation.State, Is.EqualTo (StateType.Changed));
      Assert.That (orderWithChangedRelation.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      var result = _dataManager.GetChangedDataContainersForCommit ();

      Assert.That (result.ToArray (), List.Not.Contains (orderWithChangedRelation.InternalDataContainer));
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException))]
    public void GetChangedDataContainersForCommit_ChecksMandatoryRelations ()
    {
      var invalidOrder = Order.GetObject (DomainObjectIDs.Order1);
      invalidOrder.OrderTicket = null;

      _dataManager.GetChangedDataContainersForCommit().ToArray();
    }

    [Test]
    public void GetChangedDataContainersForCommit_WithDeletedObject ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem1.Delete ();

      _dataManager.GetChangedDataContainersForCommit ().ToArray ();

      // expectation: no exception
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

      Assert.IsEmpty (new List<IRelationEndPoint> (_dataManager.GetChangedRelationEndPoints ()));

      orderItem1.Order = null; // 2 endpoints
      orderTicket.Order = null; // 2 endpoints

      computer.Employee = Employee.NewObject (); // 3 endpoints
      employee.Computer = null; // (1 endpoint)

      location.Client = Client.NewObject (); // 1 endpoint

      var changedEndPoints = new List<IRelationEndPoint> (_dataManager.GetChangedRelationEndPoints ());

      Assert.That (changedEndPoints.Count, Is.EqualTo (8));

      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(order1.ID, typeof (Order) + ".OrderItems")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(orderItem1.ID, typeof (OrderItem) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(orderTicket.ID, typeof (OrderTicket) + ".Order")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(order1.ID, typeof (Order) + ".OrderTicket")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(computer.ID, typeof (Computer) + ".Employee")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(computer.Employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(employee.ID, typeof (Employee) + ".Computer")],
          changedEndPoints);
      Assert.Contains (_dataManager.RelationEndPointMap[RelationEndPointID.Create(location.ID, typeof (Location) + ".Client")],
          changedEndPoints);
    }

    [Test]
    public void RegisterDataContainer_RegistersDataContainerInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dataContainer));
    }

    [Test]
    public void RegisterDataContainer_RegistersEndPointsInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);
    }

    [Test]
    public void RegisterDataContainer_New ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (_dataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (_dataManager.RelationEndPointMap[realEndPointID], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dataContainer));

      Assert.That (_dataManager.RelationEndPointMap[collectionEndPointID].ObjectID, Is.EqualTo (dataContainer.ID));
      Assert.That (_dataManager.RelationEndPointMap[realEndPointID].ObjectID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void RegisterDataContainer_Existing ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      SetDomainObject (dataContainer);

      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (_dataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (_dataManager.RelationEndPointMap[realEndPointID], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (_dataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dataContainer));

      Assert.That (_dataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (_dataManager.RelationEndPointMap[realEndPointID].ObjectID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The DomainObject of a DataContainer must be set before it can be registered with a transaction.")]
    public void RegisterDataContainer_ContainerHasNoDomainObject ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _dataManager.RegisterDataContainer (dataContainer);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "This DataContainer has already been registered with a ClientTransaction.")]
    public void RegisterDataContainer_ContainerAlreadyHasTransaction ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);

      var otherTransaction = new ClientTransactionMock ();
      otherTransaction.DataManager.RegisterDataContainer (dataContainer);
      Assert.That (dataContainer.IsRegistered, Is.True);

      _dataManager.RegisterDataContainer (dataContainer);
    }

    [Test]
    public void RegisterDataContainer_ContainerAlreadyHasTransaction_DataRemainsIntact ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var otherTransaction = new ClientTransactionMock ();
      otherTransaction.DataManager.RegisterDataContainer (dataContainer);
      Assert.That (dataContainer.IsRegistered, Is.True);

      try
      {
        _dataManager.RegisterDataContainer (dataContainer);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (otherTransaction));
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "A DataContainer with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction.")]
    public void RegisterDataContainer_AlreadyRegistered ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var otherDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (otherDataContainer);

      _dataManager.RegisterDataContainer (otherDataContainer);
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.SameAs (otherDataContainer));

      _dataManager.RegisterDataContainer (dataContainer);
    }

    [Test]
    public void RegisterDataContainer_AlreadyRegistered_DataRemainsIntact ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var otherDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (otherDataContainer);

      _dataManager.RegisterDataContainer (otherDataContainer);
      Assert.That (otherDataContainer.IsRegistered, Is.True);

      try
      {
        _dataManager.RegisterDataContainer (dataContainer);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException)
      {
        // ok
      }

      Assert.That (dataContainer.IsRegistered, Is.False);
      Assert.That (otherDataContainer.IsRegistered, Is.True);
      Assert.That (otherDataContainer.ClientTransaction, Is.SameAs (_dataManager.ClientTransaction));
      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.SameAs (otherDataContainer));
    }

    [Test]
    public void RegisterDataContainer_InconsistentForeignKey ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      SetDomainObject (dataContainer);

      var otherDataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket2);
      SetDomainObject (otherDataContainer);

      dataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value = DomainObjectIDs.Order1;
      otherDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value = DomainObjectIDs.Order1;

      _dataManager.RegisterDataContainer (dataContainer);
      var endPointCountBefore = _dataManager.RelationEndPointMap.Count;

      try
      {
        _dataManager.RegisterDataContainer (otherDataContainer);
        Assert.Fail ("InvalidOperationException was expected.");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo (
            "The data of object 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' conflicts with existing data: It has a foreign key "
            + "property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' which points to object "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. However, that object has previously been determined to point back to object "
            + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. These two pieces of information contradict each other."));
        Assert.That (_dataManager.DataContainerMap[otherDataContainer.ID], Is.Null);
        Assert.That (otherDataContainer.IsRegistered, Is.False);
        Assert.That (_dataManager.RelationEndPointMap.Count, Is.EqualTo (endPointCountBefore));
      }
    }

    [Test]
    public void MarkCollectionEndPointComplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      
      var items = new[] { DomainObjectMother.CreateFakeObject<Order>() };

      var endPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, endPointMock);
      endPointMock.Expect (mock => mock.MarkDataComplete (items));
      endPointMock.Replay ();

      _dataManager.MarkCollectionEndPointComplete (endPointID, items);

      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void Discard_RemovesEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Discard_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.SameAs (dataContainer));

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Discard_DiscardsDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      _dataManager.Discard (dataContainer);

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_MarksObjectInvalid ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.ClientTransaction.IsInvalid (dataContainer.ID), Is.True);
      listenerMock.AssertWasCalled (mock => mock.DataManagerDiscardingObject (ClientTransactionMock, dataContainer.ID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot discard data container 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid', it might leave dangling references: 'End point "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' still "
        + "references object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.'\r\nParameter name: dataContainer")]
    public void Discard_ThrowsOnDanglingReferences ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (IObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.CreateSetCommand (LifetimeService.GetObjectReference (_dataManager.ClientTransaction, DomainObjectIDs.Order1)).Perform ();

      _dataManager.Discard (dataContainer);
    }

    [Test]
    public void Commit_CommitsRelationEndPointMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (IObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.CreateSetCommand (LifetimeService.GetObjectReference (_dataManager.ClientTransaction, DomainObjectIDs.Order2)).Perform();

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Commit ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Commit_CommitsDataContainerMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));
      
      _dataManager.Commit ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Commit_RemovesDeletedEndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

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
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Commit_MarksDeletedObjectsAsInvalid ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      Assert.That (_dataManager.ClientTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);

      _dataManager.Commit ();

      Assert.That (_dataManager.ClientTransaction.IsInvalid (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void Rollback_RollsBackRelationEndPointStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (IObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      endPoint.CreateSetCommand (LifetimeService.GetObjectReference (_dataManager.ClientTransaction, DomainObjectIDs.Order2)).Perform ();

      Assert.That (endPoint.HasChanged, Is.True);

      _dataManager.Rollback ();

      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void Rollback_RollsBackDataContainerStates ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManager.Rollback ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Rollback_RemovesNewEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      _dataManager.Rollback ();

      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _dataManager.Rollback ();

      Assert.That (_dataManager.DataContainerMap[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Rollback_DiscardsNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (dataContainer.IsDiscarded, Is.False);

      _dataManager.Rollback ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Rollback_MarksNewObjectsAsInvalid ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (_dataManager.ClientTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);

      _dataManager.Rollback ();

      Assert.That (_dataManager.ClientTransaction.IsInvalid (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var deletedObject = Order.GetObject (DomainObjectIDs.Order1);

      var command = _dataManager.CreateDeleteCommand (deletedObject);

      Assert.That (command, Is.InstanceOfType (typeof (DeleteCommand)));
      Assert.That (((DeleteCommand) command).ClientTransaction, Is.SameAs (_dataManager.ClientTransaction));
      Assert.That (((DeleteCommand) command).DeletedObject, Is.SameAs (deletedObject));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Cannot delete DomainObject '.*', because it belongs to a different ClientTransaction.",
       MatchType = MessageMatch.Regex)]
    public void CreateDeleteCommand_OtherClientTransaction ()
    {
      var order1 = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      _dataManager.CreateDeleteCommand (order1);
    }

    [Test]
    public void CreateDeleteCommand_DeletedObject ()
    {
      var deletedObject = Order.GetObject (DomainObjectIDs.Order1);
      deletedObject.Delete ();
      Assert.That (deletedObject.State, Is.EqualTo (StateType.Deleted));

      var command = _dataManager.CreateDeleteCommand (deletedObject);
      Assert.That (command, Is.InstanceOfType (typeof (NopCommand)));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void CreateDeleteCommand_InvalidObject ()
    {
      var invalidObject = Order.NewObject ();
      invalidObject.Delete ();
      Assert.That (invalidObject.IsInvalid, Is.True);

      _dataManager.CreateDeleteCommand (invalidObject);
    }

    [Test]
    public void CheckMandatoryRelations_AllRelationsOk ()
    {
      var dataContainer = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).InternalDataContainer;

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' of domain object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null.")]
    public void CheckMandatoryRelations_RelationsNotOk ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    public void CheckMandatoryRelations_UnregisteredRelations_Ignored ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
        DataManagerTestHelper.RemoveEndPoint (_dataManager, endPointID);

      _dataManager.CheckMandatoryRelations (dataContainer);
    }

    [Test]
    public void CheckMandatoryRelations_IncompleteRelations_Ignored ()
    {
      var dataContainer = DataContainer.CreateNew (new ObjectID (DomainObjectIDs.Order1.ClassDefinition, Guid.NewGuid()));
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var orderItemsPropertyName = typeof (Order).FullName + ".OrderItems";

      // remove all but OrderItems
      foreach (var endPointID in dataContainer.AssociatedRelationEndPointIDs)
      {
        if (endPointID.Definition.PropertyName != orderItemsPropertyName)
          DataManagerTestHelper.RemoveEndPoint (_dataManager, endPointID);
      }

      var orderItemEndPoint = (ICollectionEndPoint) _dataManager.RelationEndPointMap[RelationEndPointID.Create(dataContainer.ID, orderItemsPropertyName)];
      Assert.That (orderItemEndPoint, Is.Not.Null);
      Assert.That (orderItemEndPoint.Definition.IsMandatory, Is.True);
      orderItemEndPoint.MarkDataIncomplete ();
      Assert.That (orderItemEndPoint.IsDataComplete, Is.False);

      _dataManager.CheckMandatoryRelations (dataContainer); // does not throw

      Assert.That (orderItemEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void HasRelationChanged_True ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Official");
      var endPoint = (RealObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      RealObjectEndPointTestHelper.SetOppositeObjectID (endPoint, DomainObjectIDs.Official1);
      Assert.That (endPoint.HasChanged, Is.True);

      var result = _dataManager.HasRelationChanged (dataContainer);

      Assert.That (result, Is.True);
    }

    [Test]
    public void HasRelationChanged_False ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var result = _dataManager.HasRelationChanged (dataContainer);

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasRelationChanged_IgnoresUnregisteredEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Official");
      var endPoint = (RealObjectEndPoint) _dataManager.RelationEndPointMap[endPointID];
      RealObjectEndPointTestHelper.SetOppositeObjectID (endPoint, DomainObjectIDs.Official1);
      Assert.That (endPoint.HasChanged, Is.True);

      DataManagerTestHelper.RemoveEndPoint (_dataManager, endPointID);

      Assert.That (_dataManager.HasRelationChanged (dataContainer), Is.False);
    }

    [Test]
    public void GetOppositeRelationEndPoints ()
    {
      var dataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;

      var endPoints = _dataManager.GetOppositeRelationEndPoints (dataContainer).ToArray ();

      var expectedIDs = new[] {
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem2, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Official1, "Orders"),
        RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders"),
      };
      var expectedEndPoints = expectedIDs.Select (id => _dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id)).ToArray();
      Assert.That (endPoints, Is.EquivalentTo (expectedEndPoints));
    }

    [Test]
    public void GetOppositeRelationEndPoints_WithNulls ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.OrderTicket = null;
      var dataContainer = order.InternalDataContainer;
      
      var endPoints = _dataManager.GetOppositeRelationEndPoints (dataContainer).ToArray ();

      var nullEndPoint = endPoints.OfType<NullObjectEndPoint>().SingleOrDefault();
      Assert.That (nullEndPoint, Is.Not.Null);
      var expectedEndPointDefinition = 
          Configuration.ClassDefinitions[typeof (OrderTicket)].GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      Assert.That (nullEndPoint.Definition, Is.EqualTo (expectedEndPointDefinition));
    }

    [Test]
    public void GetDataContainerWithoutLoading_NotLoaded ()
    {
      var result = _dataManager.GetDataContainerWithoutLoading (DomainObjectIDs.Order1);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetDataContainerWithoutLoading_Loaded ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);
      
      var result = _dataManager.GetDataContainerWithoutLoading (DomainObjectIDs.Order1);

      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetDataContainerWithoutLoading_Invalid ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _invalidDomainObjectManager.MarkInvalid (domainObject);

      _dataManager.GetDataContainerWithoutLoading (domainObject.ID);
    }

    [Test]
    public void GetDataContainerWithLazyLoad_Loaded ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      dataContainer.SetDomainObject (domainObject);

      _objectLoaderMock.Replay ();

      _dataManagerWitLoaderMock.RegisterDataContainer (dataContainer);

      var result = _dataManagerWitLoaderMock.GetDataContainerWithLazyLoad (domainObject.ID);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    public void GetDataContainerWithLazyLoad_NotLoaded ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      dataContainer.SetDomainObject (domainObject);

      _objectLoaderMock
          .Expect (mock => mock.LoadObject (domainObject.ID, _dataManagerWitLoaderMock))
          .WhenCalled (mi => _dataManagerWitLoaderMock.RegisterDataContainer (dataContainer))
          .Return (domainObject);
      _objectLoaderMock.Replay ();

      var result = _dataManagerWitLoaderMock.GetDataContainerWithLazyLoad (domainObject.ID);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetDataContainerWithLazyLoad_Invalid ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order> ();
      _invalidDomainObjectManager.MarkInvalid (domainObject);

      _dataManager.GetDataContainerWithLazyLoad (domainObject.ID);
    }

    [Test]
    public void LoadLazyVirtualEndPoint_CollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var loaderResult = new[] { DomainObjectMother.CreateFakeObject<OrderItem> () };

      var endPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManagerWitLoaderMock.RelationEndPointMap, endPointMock);
      endPointMock.Stub(stub => stub.IsDataComplete).Return(false);
      endPointMock.Expect (mock => mock.MarkDataComplete (loaderResult));
      endPointMock.Replay();

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObjects (endPointID, _dataManagerWitLoaderMock))
          .Return (loaderResult);
      _objectLoaderMock.Replay ();

      _dataManagerWitLoaderMock.LoadLazyVirtualEndPoint (endPointMock);

      _objectLoaderMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given end-point is not managed by this DataManager.\r\nParameter name: virtualEndPoint")]
    public void LoadLazyCollectionEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, null);
      
      _dataManager.LoadLazyVirtualEndPoint (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given end-point cannot be loaded, its data is already complete.")]
    public void LoadLazyCollectionEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = ((RelationEndPointMap) _dataManager.RelationEndPointMap).RegisterCollectionEndPoint (endPointID);
      endPoint.MarkDataComplete (new DomainObject[0]);
      Assert.That (endPoint.IsDataComplete, Is.True);

      _dataManager. LoadLazyVirtualEndPoint (endPoint);
    }

    [Test]
    public void LoadOppositeVirtualEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var objectEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.GetOppositeRelationEndPointID ()).Return (oppositeEndPointID);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, objectEndPointStub);

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointMock.Stub (stub => stub.ObjectID).Return (oppositeEndPointID.ObjectID);
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      oppositeEndPointMock.Expect (mock => mock.EnsureDataComplete ());
      oppositeEndPointMock.Replay ();
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, oppositeEndPointMock);

      _dataManager.LoadOppositeVirtualEndPoint (objectEndPointStub);

      oppositeEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The end-point's opposite object is null, so no opposite end-point can be loaded.")]
    public void LoadOppositeVirtualEndPoint_OppositeIsNull ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var oppositeEndPointID = RelationEndPointID.Create (null, endPointID.Definition.GetMandatoryOppositeEndPointDefinition ());

      var objectEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.GetOppositeRelationEndPointID ()).Return (oppositeEndPointID);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, objectEndPointStub);

      _dataManager.LoadOppositeVirtualEndPoint (objectEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has already been loaded.")]
    public void LoadOppositeEndPoint_AlreadLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var objectEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      objectEndPointStub.Stub (stub => stub.ID).Return (endPointID);
      objectEndPointStub.Stub (stub => stub.GetOppositeRelationEndPointID ()).Return (oppositeEndPointID);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, objectEndPointStub);

      var oppositeEndPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointStub.Stub (stub => stub.ObjectID).Return (oppositeEndPointID.ObjectID);
      oppositeEndPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      RelationEndPointMapTestHelper.AddEndPoint ((RelationEndPointMap) _dataManager.RelationEndPointMap, oppositeEndPointStub);

      _dataManager.LoadOppositeVirtualEndPoint (objectEndPointStub);
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");

      var result = _dataManager.GetRelationEndPointWithLazyLoad (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.RelationEndPointMap[endPointID]));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointNotAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);

      var result = _dataManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Null);
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (_dataManager.RelationEndPointMap[endPointID], Is.Not.Null);

      var result = _dataManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.RelationEndPointMap[endPointID]));
    }

    [Test]
    public void GetOppositeVirtualEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      var originatingEndPoint = (IRealObjectEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      var oppositeEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var oppositeEndPoint = _dataManager.GetRelationEndPointWithoutLoading (oppositeEndPointID);
      Assert.That (oppositeEndPoint, Is.Not.Null);

      var result = _dataManager.GetOppositeVirtualEndPoint (originatingEndPoint);

      Assert.That (result, Is.SameAs (oppositeEndPoint));
    }

    [Test]
    public void GetOppositeVirtualEndPoint_NullEndPoint ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<OrderTicket> (dataContainer.ID));
      _dataManager.RegisterDataContainer (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (dataContainer.ID, "Order");
      var originatingEndPoint = (IRealObjectEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (originatingEndPoint.OppositeObjectID, Is.Null);

      var result = _dataManager.GetOppositeVirtualEndPoint (originatingEndPoint);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (NullVirtualObjectEndPoint)));
    }

    private Tuple<DomainObject, DataContainer, StateType> CreateDataTuple (DomainObject domainObject)
    {
      var dataContainer = ClientTransactionMock.DataManager.DataContainerMap[domainObject.ID];
      return Tuple.Create (domainObject, dataContainer, domainObject.State);
    }

    private void SetDomainObject (DataContainer dc)
    {
      dc.SetDomainObject (DomainObjectMother.GetObjectReference<DomainObject> (_dataManager.ClientTransaction, dc.ID));
    }
  }
}
