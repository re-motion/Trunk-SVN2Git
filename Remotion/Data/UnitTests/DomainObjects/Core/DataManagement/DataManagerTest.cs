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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
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

    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IObjectLoader _objectLoaderMock;
    private IRelationEndPointManager _endPointManagerMock;
    private DataManager _dataManagerWithMocks;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
      _invalidDomainObjectManager = (IInvalidDomainObjectManager) PrivateInvoke.GetNonPublicField (_dataManager, "_invalidDomainObjectManager");

      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader> ();
      _endPointManagerMock = MockRepository.GenerateStrictMock<IRelationEndPointManager> ();

      _invalidDomainObjectManagerMock = MockRepository.GenerateMock<IInvalidDomainObjectManager>();
      _dataManagerWithMocks = new DataManager (
          ClientTransactionMock,
          _invalidDomainObjectManagerMock,
          _objectLoaderMock,
          dm => _endPointManagerMock);
    }

    [Test]
    public void Initialization ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var invalidDomainObjectManager = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoader = MockRepository.GenerateStub<IObjectLoader> ();
      var endPointManager = MockRepository.GenerateStub<IRelationEndPointManager>();

      var dataManager = new DataManager (clientTransaction, invalidDomainObjectManager, objectLoader, dm => endPointManager);

      var manager = DataManagerTestHelper.GetRelationEndPointManager (dataManager);
      Assert.That (manager, Is.SameAs (endPointManager));
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
    public void GetLoadedDataByObjectState ()
    {
      var unchangedInstance = DomainObjectMother.GetUnchangedObject(ClientTransactionMock, DomainObjectIDs.Order1);
      var changedInstance = DomainObjectMother.GetChangedObject(ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject();
      var deletedInstance = DomainObjectMother.GetDeletedObject(ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      var unchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Unchanged);
      var changedOrNewObjects = _dataManager.GetLoadedDataByObjectState (StateType.Changed, StateType.New);
      var deletedOrUnchangedObjects = _dataManager.GetLoadedDataByObjectState (StateType.Deleted, StateType.Unchanged);

      CheckPersistableDataSequence (new[] { CreatePersistableData (unchangedInstance) }, unchangedObjects);
      CheckPersistableDataSequence (new[] { CreatePersistableData (changedInstance), CreatePersistableData (newInstance) }, changedOrNewObjects);
      CheckPersistableDataSequence (new[] { CreatePersistableData (deletedInstance), CreatePersistableData (unchangedInstance) }, deletedOrUnchangedObjects);
    }

    [Test]
    public void GetNewChangedDeletedData_Empty ()
    {
      var changedDomainObjects = _dataManager.GetNewChangedDeletedData ();
      Assert.That (changedDomainObjects.ToArray(), Is.Empty);
    }

    [Test]
    public void GetNewChangedDeletedData ()
    {
      var changedInstance = DomainObjectMother.GetChangedObject (ClientTransactionMock, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject ();
      var deletedInstance = DomainObjectMother.GetDeletedObject (ClientTransactionMock, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (ClientTransactionMock, DomainObjectIDs.Order1);
      DomainObjectMother.GetInvalidObject (ClientTransactionMock);
      DomainObjectMother.GetNotLoadedObject (ClientTransactionMock, DomainObjectIDs.Order2);

      var changedDomainObjects = _dataManager.GetNewChangedDeletedData ();
      
      var expected = new[] { 
          CreatePersistableData (changedInstance), 
          CreatePersistableData (newInstance),
          CreatePersistableData (deletedInstance) };

      CheckPersistableDataSequence(expected, changedDomainObjects);
    }

    [Test]
    public void GetNewChangedDeletedData_ReturnsObjectsChangedByRelation ()
    {
      var orderWithChangedRelation = Order.GetObject (DomainObjectIDs.Order1);

      orderWithChangedRelation.OrderTicket = null;
      Assert.That (orderWithChangedRelation.State, Is.EqualTo (StateType.Changed));
      Assert.That (orderWithChangedRelation.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));

      var data = _dataManager.GetNewChangedDeletedData ();
      
      var expected = CreatePersistableData (orderWithChangedRelation);
      CheckHasPersistableDataItem (expected, data.ToDictionary (item => item.DomainObject));
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
    public void TrySetCollectionEndPointData_True ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      
      var items = new[] { DomainObjectMother.CreateFakeObject<Order>() };

      var result = _dataManager.TrySetCollectionEndPointData (endPointID, items);

      Assert.That (result, Is.True);
      var endPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint.GetData(), Is.EqualTo (items));
    }

    [Test]
    public void TrySetCollectionEndPointData_False ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var endPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithMinimumLoading (endPointID);
      endPoint.EnsureDataComplete();

      var items = new[] { DomainObjectMother.CreateFakeObject<Order> () };

      var result = _dataManager.TrySetCollectionEndPointData (endPointID, items);

      Assert.That (result, Is.False);
      Assert.That (endPoint.GetData (), Is.Not.EqualTo (items));
    }

    [Test]
    public void TrySetVirtualObjectEndPointData_True ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var item = DomainObjectMother.CreateFakeObject<Order> ();

      var result = _dataManager.TrySetVirtualObjectEndPointData (endPointID, item);

      Assert.That (result, Is.True);
      var endPoint = (IVirtualObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint.GetData (), Is.SameAs (item));
    }

    [Test]
    public void TrySetVirtualObjectEndPointData_True_Null ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var result = _dataManager.TrySetVirtualObjectEndPointData (endPointID, null);

      Assert.That (result, Is.True);
      var endPoint = (IVirtualObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint.GetData (), Is.Null);
    }

    [Test]
    public void TrySetVirtualObjectEndPointData_False ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = (IVirtualObjectEndPoint) _dataManager.GetRelationEndPointWithMinimumLoading (endPointID);
      endPoint.EnsureDataComplete ();

      var item = DomainObjectMother.CreateFakeObject<Order> ();

      var result = _dataManager.TrySetVirtualObjectEndPointData (endPointID, item);

      Assert.That (result, Is.False);
      Assert.That (endPoint.GetData (), Is.Not.SameAs (item));
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot discard data for object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid': The relations of object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be unloaded.\r\n"
        + "Relation end-point "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' would "
        + "leave a dangling reference.")]
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
      _endPointManagerMock.Expect (mock => mock.CommitAllEndPoints());
      _endPointManagerMock.Replay();

      _dataManagerWithMocks.Commit();

      _endPointManagerMock.VerifyAllExpectations ();
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
    public void Commit_RemovesDeletedDataContainers_EndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete ();

      var endPointID = RelationEndPointID.Create (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _dataManager.Commit ();

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
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
    public void Rollback_RollsBackRelationEndPoints ()
    {
      _endPointManagerMock.Expect (mock => mock.RollbackAllEndPoints());
      _endPointManagerMock.Replay();

      _dataManagerWithMocks.Rollback();

      _endPointManagerMock.VerifyAllExpectations ();
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
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _dataManager.Rollback ();

      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers_EndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _dataManager.Rollback ();

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);
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
      Assert.That (((UnregisterEndPointsCommand) unloadDataCommandSteps[1]).EndPoints, Has.Count.EqualTo (2));

      Assert.That (unloadDataCommandSteps[2], Is.TypeOf<UnregisterDataContainerCommand> ());
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[2]).Map, Is.SameAs (_dataManager.DataContainers));
      Assert.That (((UnregisterDataContainerCommand) unloadDataCommandSteps[2]).ObjectID, Is.EqualTo (DomainObjectIDs.Order2));

      Assert.That (unloadDataCommandSteps[3], Is.TypeOf<UnregisterEndPointsCommand> ());
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
    public void CreateUnloadVirtualEndPointCommand ()
    {
      var endPointID1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointID2 = RelationEndPointID.Create (DomainObjectIDs.Order2, typeof (Order), "OrderItems");
      var endPointID3 = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");

      var endPoint1 = _dataManager.GetRelationEndPointWithMinimumLoading (endPointID1);
      var endPoint2 = _dataManager.GetRelationEndPointWithMinimumLoading (endPointID2);
      
      var command = _dataManager.CreateUnloadVirtualEndPointsCommand (endPointID1, endPointID2, endPointID3);

      Assert.That (
          command, Is.TypeOf<UnloadVirtualEndPointsCommand>().With.Property ("VirtualEndPoints").EqualTo (new[] { endPoint1, endPoint2 }));
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
      var expectedEndPoints = expectedIDs.Select (id => _dataManager.GetRelationEndPointWithLazyLoad (id)).ToArray();
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

      DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, dataContainer);

      _objectLoaderMock.Replay ();
      
      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad (domainObject.ID);

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
          .Expect (mock => mock.LoadObject (domainObject.ID, _dataManagerWithMocks))
          .WhenCalled (mi => DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, dataContainer))
          .Return (domainObject);
      _objectLoaderMock.Replay ();

      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad (domainObject.ID);

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
      endPointMock.Stub(stub => stub.IsDataComplete).Return(false);
      endPointMock.Expect (mock => mock.MarkDataComplete (loaderResult));
      endPointMock.Replay();

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObjects (
              Arg.Is (endPointID), 
              Arg.Is (_dataManagerWithMocks),
              Arg<ILoadedObjectProvider>.Matches (p => p is LoadedObjectProvider 
                  && ((LoadedObjectProvider) p).DataContainerProvider == _dataManagerWithMocks
                  && ((LoadedObjectProvider) p).InvalidDomainObjectManager == _invalidDomainObjectManagerMock)))
          .Return (loaderResult);
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyCollectionEndPoint (endPointMock);

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
      DataManagerTestHelper.AddEndPoint (_dataManager, endPointStub);

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

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObject (Arg
              .Is (endPointID), 
              Arg.Is (_dataManagerWithMocks),
              Arg<ILoadedObjectProvider>.Matches (p => p is LoadedObjectProvider 
                  && ((LoadedObjectProvider) p).DataContainerProvider == _dataManagerWithMocks
                  && ((LoadedObjectProvider) p).InvalidDomainObjectManager == _invalidDomainObjectManagerMock)))
          .Return (loaderResult)
          .WhenCalled (mi => endPointMock.Stub (stub => stub.IsDataComplete).Return (true));
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointMock);

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

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.LoadRelatedObject (
              Arg.Is (endPointID),
              Arg.Is (_dataManagerWithMocks),
              Arg<ILoadedObjectProvider>.Matches (p => p is LoadedObjectProvider
                  && ((LoadedObjectProvider) p).DataContainerProvider == _dataManagerWithMocks
                  && ((LoadedObjectProvider) p).InvalidDomainObjectManager == _invalidDomainObjectManagerMock)))
          .Return (loaderResult);
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointMock);

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
      var endPoint = (IVirtualObjectEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
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
          .Expect (mock => mock.LoadObject (DomainObjectIDs.Order1, _dataManagerWithMocks))
          .Return (fakeObject)
          .WhenCalled (mi => DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, fakeDataContainer));
      _objectLoaderMock.Replay();

      var result = _dataManagerWithMocks.LoadLazyDataContainer (DomainObjectIDs.Order1);

      _objectLoaderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeDataContainer));
    }

    [Test]
    public void LoadLazyDataContainer_AlreadyLoaded ()
    {
      var fakeObject = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      var fakeDataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      fakeDataContainer.SetDomainObject (fakeObject);
      DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, fakeDataContainer);
      
      _objectLoaderMock.Replay();

      Assert.That (
          () => _dataManagerWithMocks.LoadLazyDataContainer (DomainObjectIDs.Order1),
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

    private PersistableData CreatePersistableData (DomainObject domainObject)
    {
      var dataContainer = ClientTransactionMock.DataManager.DataContainers[domainObject.ID];
      return new PersistableData (domainObject, domainObject.State, dataContainer, _dataManager.RelationEndPoints.Where (ep => ep.ObjectID == domainObject.ID));
    }

    private void SetDomainObject (DataContainer dc)
    {
      dc.SetDomainObject (DomainObjectMother.GetObjectReference<DomainObject> (_dataManager.ClientTransaction, dc.ID));
    }

    private void CheckPersistableDataSequence (IEnumerable<PersistableData> expected, IEnumerable<PersistableData> actual)
    {
      var expectedList = expected.ToList();
      var actualDictionary = actual.ToDictionary (pd => pd.DomainObject);

      Assert.That (actualDictionary.Count, Is.EqualTo (expectedList.Count));
      foreach (var expectedPersistableData in expectedList)
      {
        CheckHasPersistableDataItem (expectedPersistableData, actualDictionary);
      }
    }

    private void CheckHasPersistableDataItem (PersistableData expectedPersistableData, Dictionary<DomainObject, PersistableData> actualDictionary)
    {
      var actualPersistableData = actualDictionary.GetValueOrDefault (expectedPersistableData.DomainObject);
      Assert.That (actualPersistableData, Is.Not.Null, "Expected persistable item: {0}", expectedPersistableData.DomainObject.ID);
      Assert.That (actualPersistableData.DomainObjectState, Is.EqualTo (expectedPersistableData.DomainObjectState));
      Assert.That (actualPersistableData.DataContainer, Is.SameAs (expectedPersistableData.DataContainer));
    }
  }
}
