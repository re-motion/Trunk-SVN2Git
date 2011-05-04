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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
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

      Assert.That (dataManager.RelationEndPointManager, Is.TypeOf (typeof (RelationEndPointManager)));
      var map = (RelationEndPointManager) dataManager.RelationEndPointManager;

      Assert.That (map.ClientTransaction, Is.SameAs (clientTransaction));
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
      _dataManager.DataContainers[order1.ID].PropertyValues[propertyName].Value = 100;

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
      Assert.That (changedDomainObjects.ToArray (), Has.Member(expected));
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

      Assert.That (result.ToArray (), Has.No.Member(orderWithChangedRelation.InternalDataContainer));
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

      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(order1.ID, typeof (Order) + ".OrderItems")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(orderItem1.ID, typeof (OrderItem) + ".Order")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(orderTicket.ID, typeof (OrderTicket) + ".Order")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(order1.ID, typeof (Order) + ".OrderTicket")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(computer.ID, typeof (Computer) + ".Employee")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(computer.Employee.ID, typeof (Employee) + ".Computer")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(employee.ID, typeof (Employee) + ".Computer")),
          changedEndPoints);
      Assert.Contains (_dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(location.ID, typeof (Location) + ".Client")),
          changedEndPoints);
    }

    [Test]
    public void RegisterDataContainer_RegistersDataContainerInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);
      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs (dataContainer));
    }

    [Test]
    public void RegisterDataContainer_RegistersEndPointsInMap ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void RegisterDataContainer_New ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (collectionEndPointID), Is.Null);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (realEndPointID), Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs (dataContainer));

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (collectionEndPointID).ObjectID, Is.EqualTo (dataContainer.ID));
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (realEndPointID).ObjectID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void RegisterDataContainer_Existing ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      SetDomainObject (dataContainer);

      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (collectionEndPointID), Is.Null);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (realEndPointID), Is.Null);

      _dataManager.RegisterDataContainer (dataContainer);

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs (dataContainer));

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (collectionEndPointID), Is.Null);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (realEndPointID).ObjectID, Is.EqualTo (dataContainer.ID));
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
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
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
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.SameAs (otherDataContainer));

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
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.SameAs (otherDataContainer));
    }

    [Test]
    public void MarkCollectionEndPointComplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      
      var items = new[] { DomainObjectMother.CreateFakeObject<Order>() };

      var endPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) _dataManager.RelationEndPointManager, endPointMock);
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
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void Discard_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.SameAs (dataContainer));

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
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
      var endPoint = (RealObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      RealObjectEndPointTestHelper.SetOppositeObjectID (endPoint, DomainObjectIDs.Order1);

      _dataManager.Discard (dataContainer);
    }

    [Test]
    public void Commit_CommitsRelationEndPoints ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderTicket1, typeof (OrderTicket).FullName + ".Order");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint>();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Commit());
      endPointMock.Replay();

      RelationEndPointManagerTestHelper.AddEndPoint (DataManagerTestHelper.GetRelationEndPointMap (_dataManager), endPointMock);

      endPointMock.Replay();
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
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (dataContainer.IsDiscarded, Is.True);
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
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
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderTicket1, typeof (OrderTicket).FullName + ".Order");
      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Expect (mock => mock.Rollback ());
      endPointMock.Replay ();

      RelationEndPointManagerTestHelper.AddEndPoint (DataManagerTestHelper.GetRelationEndPointMap (_dataManager), endPointMock);

      endPointMock.Replay ();

      _dataManager.Rollback();

      endPointMock.VerifyAllExpectations();
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
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _dataManager.Rollback ();

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _dataManager.Rollback ();

      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
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

      Assert.That (command, Is.InstanceOf (typeof (DeleteCommand)));
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
      Assert.That (command, Is.InstanceOf (typeof (NopCommand)));
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
    public void CreateUnloadCommand_NoObjects ()
    {
      var result = _dataManager.CreateUnloadCommand();

      Assert.That (result, Is.TypeOf<NopCommand>());
    }

    [Test]
    public void CreateUnloadCommand_NoLoadedObjects ()
    {
      var result = _dataManager.CreateUnloadCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2);

      Assert.That (result, Is.TypeOf<NopCommand> ());
    }

    [Test]
    public void CreateUnloadCommand_WithLoadedObjects ()
    {
      var loadedDataContainer1 = _dataManager.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1);
      var loadedDataContainer2 = _dataManager.GetDataContainerWithLazyLoad (DomainObjectIDs.Order2);

      var loadedObject1 = loadedDataContainer1.DomainObject;
      var loadedObject2 = loadedDataContainer2.DomainObject;

      var result = _dataManager.CreateUnloadCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);

      Assert.That (result, Is.TypeOf<UnloadCommand>());
      var unloadCommand = (UnloadCommand) result;
      Assert.That (unloadCommand.ClientTransaction, Is.SameAs (_dataManager.ClientTransaction));
      Assert.That (unloadCommand.DomainObjects, Is.EqualTo (new[] { loadedObject1, loadedObject2 }));
      Assert.That (unloadCommand.UnloadDataCommand, Is.TypeOf<CompositeCommand> ());

      var unloadDataCommandSteps = ((CompositeCommand) unloadCommand.UnloadDataCommand).GetNestedCommands();
      Assert.That (unloadDataCommandSteps, Has.Count.EqualTo (4));

      Assert.That (unloadDataCommandSteps[0], Is.TypeOf<UnregisterDataContainerCommand> ());
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[0]).Map, Is.SameAs (_dataManager.DataContainers));
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[0]).ObjectID, Is.EqualTo (DomainObjectIDs.Order1));

      Assert.That (unloadDataCommandSteps[1], Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) unloadDataCommandSteps[1]).RegistrationAgent, 
          Is.SameAs (((RelationEndPointManager) _dataManager.RelationEndPointManager).RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) unloadDataCommandSteps[1]).EndPoints, Has.Count.EqualTo (2));

      Assert.That (unloadDataCommandSteps[2], Is.TypeOf<UnregisterDataContainerCommand> ());
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[2]).Map, Is.SameAs (_dataManager.DataContainers));
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[2]).ObjectID, Is.EqualTo (DomainObjectIDs.Order2));

      Assert.That (unloadDataCommandSteps[3], Is.TypeOf<UnregisterEndPointsCommand> ());
      Assert.That (((UnregisterEndPointsCommand) unloadDataCommandSteps[3]).RegistrationAgent, 
          Is.SameAs (((RelationEndPointManager) _dataManager.RelationEndPointManager).RegistrationAgent));
      Assert.That (((UnregisterEndPointsCommand) unloadDataCommandSteps[3]).EndPoints, Has.Count.EqualTo (2));
    }

    [Test]
    public void CreateUnloadCommand_WithChangedObjects ()
    {
      _dataManager.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1);
      var loadedDataContainer2 = _dataManager.GetDataContainerWithLazyLoad (DomainObjectIDs.Order2);
      var loadedDataContainer3 = _dataManager.GetDataContainerWithLazyLoad (DomainObjectIDs.Order3);

      loadedDataContainer2.MarkAsChanged ();
      loadedDataContainer3.Delete();

      var result = _dataManager.CreateUnloadCommand (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);

      Assert.That (result, Is.TypeOf<ExceptionCommand> ());
      var exceptionCommand = (ExceptionCommand) result;
      Assert.That (exceptionCommand.Exception.Message, Is.EqualTo (
          "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
          + "'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' (Changed), 'Order|3c0fb6ed-de1c-4e70-8d80-218e0bf58df3|System.Guid' (Deleted)."));
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

      var orderItemEndPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Create(dataContainer.ID, orderItemsPropertyName));
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
      var endPoint = (RealObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
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
      var endPoint = (RealObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
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
      var expectedEndPoints = expectedIDs.Select (id => _dataManager.RelationEndPointManager.GetRelationEndPointWithLazyLoad (id)).ToArray();
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
          Configuration.GetTypeDefinition (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
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
    public void LoadLazyCollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var loaderResult = new[] { DomainObjectMother.CreateFakeObject<OrderItem> () };

      var endPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) _dataManagerWitLoaderMock.RelationEndPointManager, endPointMock);
      endPointMock.Stub(stub => stub.IsDataComplete).Return(false);
      endPointMock.Expect (mock => mock.MarkDataComplete (loaderResult));
      endPointMock.Replay();

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObjects (endPointID, _dataManagerWitLoaderMock))
          .Return (loaderResult);
      _objectLoaderMock.Replay ();

      _dataManagerWitLoaderMock.LoadLazyCollectionEndPoint (endPointMock);

      _objectLoaderMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given end-point is not managed by this DataManager.\r\nParameter name: collectionEndPoint")]
    public void LoadLazyCollectionEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, null);
      
      _dataManager.LoadLazyCollectionEndPoint (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given end-point cannot be loaded, its data is already complete.")]
    public void LoadLazyCollectionEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) _dataManager.RelationEndPointManager, endPointStub);

      _dataManager.LoadLazyCollectionEndPoint (endPointStub);
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_MarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var loaderResult = DomainObjectMother.CreateFakeObject<OrderTicket>();

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub (stub => stub.IsDataComplete).Return (false).Repeat.Once();
      endPointMock.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) _dataManagerWitLoaderMock.RelationEndPointManager, endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObject (endPointID, _dataManagerWitLoaderMock))
          .Return (loaderResult)
          .WhenCalled (mi => endPointMock.Stub (stub => stub.IsDataComplete).Return (true));
      _objectLoaderMock.Replay ();

      _dataManagerWitLoaderMock.LoadLazyVirtualObjectEndPoint (endPointMock);

      _objectLoaderMock.VerifyAllExpectations ();

      endPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (loaderResult));
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_NotMarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var loaderResult = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      endPointMock.Expect (mock => mock.MarkDataComplete (loaderResult));
      endPointMock.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) _dataManagerWitLoaderMock.RelationEndPointManager, endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObject (endPointID, _dataManagerWitLoaderMock))
          .Return (loaderResult);
      _objectLoaderMock.Replay ();

      _dataManagerWitLoaderMock.LoadLazyVirtualObjectEndPoint (endPointMock);

      _objectLoaderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given end-point is not managed by this DataManager.\r\nParameter name: virtualObjectEndPoint")]
    public void LoadLazyVirtualObjectEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (endPointID, ClientTransaction.Current);

      _dataManager.LoadLazyVirtualObjectEndPoint (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given end-point cannot be loaded, its data is already complete.")]
    public void LoadLazyVirtualObjectEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = (IVirtualObjectEndPoint) _dataManager.RelationEndPointManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (endPoint.IsDataComplete, Is.True);

      _dataManager.LoadLazyVirtualObjectEndPoint (endPoint);
    }

    [Test]
    public void LoadLazyDataContainer ()
    {
      var fakeObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeDataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      fakeDataContainer.SetDomainObject (fakeObject);

      _objectLoaderMock
          .Expect (mock => mock.LoadObject (DomainObjectIDs.Order1, _dataManagerWitLoaderMock))
          .Return (fakeObject)
          .WhenCalled (mi => _dataManagerWitLoaderMock.RegisterDataContainer (fakeDataContainer));
      _objectLoaderMock.Replay();

      var result = _dataManagerWitLoaderMock.LoadLazyDataContainer (DomainObjectIDs.Order1);

      _objectLoaderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeDataContainer));
    }

    [Test]
    public void LoadLazyDataContainer_AlreadyLoaded ()
    {
      var fakeObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeDataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      fakeDataContainer.SetDomainObject (fakeObject);
      _dataManagerWitLoaderMock.RegisterDataContainer (fakeDataContainer);

      _objectLoaderMock.Replay();

      Assert.That (
          () => _dataManagerWitLoaderMock.LoadLazyDataContainer (DomainObjectIDs.Order1),
          Throws.InvalidOperationException.With.Message.EquivalentTo (
              "The given DataContainer cannot be loaded, its data is already available."));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");

      var result = _dataManager.GetRelationEndPointWithLazyLoad (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.GetRelationEndPointWithoutLoading (endPointID)));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointNotAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      var result = _dataManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Null);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
    }

    [Test]
    public void GetRelationEndPointWithoutLoading_EndPointAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      var result = _dataManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.GetRelationEndPointWithoutLoading (endPointID)));
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_RealEndPointNotAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      var result = _dataManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.GetRelationEndPointWithoutLoading (endPointID)));
      Assert.That (result.IsDataComplete, Is.True);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_VirtualEndPointNotAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      var result = _dataManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.GetRelationEndPointWithoutLoading (endPointID)));
      Assert.That (result.IsDataComplete, Is.False);
    }

    [Test]
    public void GetRelationEndPointWithMinimumLoading_EndPointAvailable ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
      _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      var result = _dataManager.GetRelationEndPointWithMinimumLoading (endPointID);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (_dataManager.GetRelationEndPointWithoutLoading (endPointID)));
    }

    private Tuple<DomainObject, DataContainer, StateType> CreateDataTuple (DomainObject domainObject)
    {
      var dataContainer = ClientTransactionMock.DataManager.DataContainers[domainObject.ID];
      return Tuple.Create (domainObject, dataContainer, domainObject.State);
    }

    private void SetDomainObject (DataContainer dc)
    {
      dc.SetDomainObject (DomainObjectMother.GetObjectReference<DomainObject> (_dataManager.ClientTransaction, dc.ID));
    }
  }
}
