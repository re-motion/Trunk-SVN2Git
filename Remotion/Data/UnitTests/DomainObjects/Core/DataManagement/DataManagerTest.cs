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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    private IClientTransactionEventSink _transactionEventSinkStub;
    private IDataContainerEventListener _dataContainerEventListenerStub;
    private IObjectLoader _objectLoaderMock;
    private IRelationEndPointManager _endPointManagerMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;

    private DataManager _dataManagerWithMocks;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = TestableClientTransaction.DataManager;

      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink>();
      _dataContainerEventListenerStub = MockRepository.GenerateStub<IDataContainerEventListener>();
      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader> ();
      _endPointManagerMock = MockRepository.GenerateStrictMock<IRelationEndPointManager> ();
      _invalidDomainObjectManagerMock = MockRepository.GenerateMock<IInvalidDomainObjectManager>();

      _dataManagerWithMocks = new DataManager (
          TestableClientTransaction,
          _transactionEventSinkStub,
          _dataContainerEventListenerStub,
          _invalidDomainObjectManagerMock,
          _objectLoaderMock,
          _endPointManagerMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_dataManagerWithMocks.ClientTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (_dataManagerWithMocks.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
      Assert.That (_dataManagerWithMocks.DataContainerEventListener, Is.SameAs (_dataContainerEventListenerStub));
      Assert.That (DataManagerTestHelper.GetRelationEndPointManager (_dataManagerWithMocks), Is.SameAs (_endPointManagerMock));
      
      var dataContainerMap = DataManagerTestHelper.GetDataContainerMap (_dataManagerWithMocks);
      Assert.That (dataContainerMap.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
    }

    [Test]
    public void DomainObjectStateCache ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (_dataManager.DomainObjectStateCache.GetState (order1.ID), Is.EqualTo (StateType.Unchanged));

      var propertyName = GetPropertyDefinition (typeof (Order), "OrderNumber");
      _dataManager.DataContainers[order1.ID].SetValue (propertyName, 100);

      Assert.That (_dataManager.DomainObjectStateCache.GetState (order1.ID), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetLoadedDataByObjectState ()
    {
      var unchangedInstance = DomainObjectMother.GetUnchangedObject(TestableClientTransaction, DomainObjectIDs.Order1);
      var changedInstance = DomainObjectMother.GetChangedObject(TestableClientTransaction, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject();
      var deletedInstance = DomainObjectMother.GetDeletedObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);

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
      var changedInstance = DomainObjectMother.GetChangedObject (TestableClientTransaction, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject ();
      var deletedInstance = DomainObjectMother.GetDeletedObject (TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);

      DomainObjectMother.GetUnchangedObject (TestableClientTransaction, DomainObjectIDs.Order1);
      DomainObjectMother.GetInvalidObject (TestableClientTransaction);
      DomainObjectMother.GetNotLoadedObject (TestableClientTransaction, DomainObjectIDs.Order2);

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

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (dataContainer.EventListener, Is.SameAs (_dataManager.DataContainerEventListener));
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

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (dataContainer.EventListener, Is.SameAs (_dataManager.DataContainerEventListener));
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
    public void RegisterDataContainer_ContainerAlreadyHasTransaction ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var otherTransaction = new TestableClientTransaction ();
      otherTransaction.DataManager.RegisterDataContainer (dataContainer);
      Assert.That (dataContainer.IsRegistered, Is.True);
      var previousEventListener = dataContainer.EventListener;

      Assert.That (
          () => _dataManager.RegisterDataContainer (dataContainer), 
          Throws.InvalidOperationException.With.Message.EqualTo ("This DataContainer has already been registered with a ClientTransaction."));

      Assert.That (dataContainer.ClientTransaction, Is.SameAs (otherTransaction));
      Assert.That (dataContainer.EventListener, Is.SameAs (previousEventListener));
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void RegisterDataContainer_AlreadyRegistered ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (dataContainer);

      var otherDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      SetDomainObject (otherDataContainer);

      _dataManager.RegisterDataContainer (otherDataContainer);
      Assert.That (otherDataContainer.IsRegistered, Is.True);

      Assert.That (
          () => _dataManager.RegisterDataContainer (dataContainer),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "A DataContainer with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction."));

      Assert.That (dataContainer.IsRegistered, Is.False);
      Assert.That (otherDataContainer.IsRegistered, Is.True);
      Assert.That (otherDataContainer.ClientTransaction, Is.SameAs (_dataManager.ClientTransaction));
      Assert.That (otherDataContainer.EventListener, Is.SameAs (_dataManager.DataContainerEventListener));
      Assert.That (_dataManager.DataContainers[dataContainer.ID], Is.SameAs (otherDataContainer));
    }

    [Test]
    public void Discard_RemovesEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer (_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
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
      var dataContainer = PrepareNewDataContainer (_dataManager, DomainObjectIDs.OrderTicket1);

      _dataManager.Discard (dataContainer);

      Assert.That (_dataManager.ClientTransaction.IsInvalid (dataContainer.ID), Is.True);
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

      var endPointID = RelationEndPointID.Create (dataContainer.ID, typeof (OrderTicket).FullName + ".Order");
      var endPoint = (RealObjectEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      RealObjectEndPointTestHelper.SetOppositeObjectID (endPoint, DomainObjectIDs.Order1);

      _dataManager.Discard (dataContainer);
    }
    
    [Test]
    public void MarkObjectInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_dataManagerWithMocks.ClientTransaction);
      _invalidDomainObjectManagerMock.Expect (mock => mock.MarkInvalid (domainObject)).Return (true);
      _invalidDomainObjectManagerMock.Replay();

      Assert.That (_dataManagerWithMocks.ClientTransaction.IsEnlisted (domainObject), Is.True);
      Assert.That (_dataManagerWithMocks.GetDataContainerWithoutLoading (domainObject.ID), Is.Null);
      _endPointManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (Arg<RelationEndPointID>.Is.Anything))
          .Return (null);
      
      _dataManagerWithMocks.MarkInvalid (domainObject);

      _invalidDomainObjectManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkObjectInvalid_NotEnlisted_Throws ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order>();
      Assert.That (_dataManagerWithMocks.ClientTransaction.IsEnlisted (domainObject), Is.False);
      
      Assert.That (() => _dataManagerWithMocks.MarkInvalid (domainObject), Throws.TypeOf<ClientTransactionsDifferException>());

      _invalidDomainObjectManagerMock.AssertWasNotCalled (mock => mock.MarkInvalid (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void MarkObjectInvalid_DataContainerRegistered_Throws ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_dataManagerWithMocks.ClientTransaction);
      PrepareLoadedDataContainer (_dataManagerWithMocks, domainObject.ID);
      Assert.That (_dataManagerWithMocks.ClientTransaction.IsEnlisted (domainObject), Is.True);
      Assert.That (_dataManagerWithMocks.GetDataContainerWithoutLoading (domainObject.ID), Is.Not.Null);

      Assert.That (() => _dataManagerWithMocks.MarkInvalid (domainObject), Throws.InvalidOperationException.With.Message.EqualTo (
          "Cannot mark DomainObject '" + domainObject.ID + "' invalid because there is data registered for the object."));

      _invalidDomainObjectManagerMock.AssertWasNotCalled (mock => mock.MarkInvalid (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void MarkObjectInvalid_EndPointRegistered_Throws ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_dataManagerWithMocks.ClientTransaction);
      Assert.That (_dataManagerWithMocks.ClientTransaction.IsEnlisted (domainObject), Is.True);
      Assert.That (_dataManagerWithMocks.GetDataContainerWithoutLoading (domainObject.ID), Is.Null);

      _endPointManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (domainObject.ID, typeof (Order), "OrderTicket")))
          .Return (MockRepository.GenerateStub<IRelationEndPoint>());
      _endPointManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (Arg<RelationEndPointID>.Is.Anything))
          .Return (null);

      Assert.That (() => _dataManagerWithMocks.MarkInvalid (domainObject), Throws.InvalidOperationException.With.Message.EqualTo (
          "Cannot mark DomainObject '" + domainObject.ID + "' invalid because there are relation end-points registered for the object."));

      _invalidDomainObjectManagerMock.AssertWasNotCalled (mock => mock.MarkInvalid (Arg<DomainObject>.Is.Anything));
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
      Assert.That (((DeleteCommand) command).TransactionEventSink, Is.SameAs (_dataManager.TransactionEventSink));
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
      Assert.That (unloadCommand.DomainObjects, Is.EqualTo (new[] { loadedObject1, loadedObject2 }));
      Assert.That (unloadCommand.UnloadDataCommand, Is.TypeOf<CompositeCommand> ());
      Assert.That (unloadCommand.TransactionEventSink, Is.SameAs (_dataManager.TransactionEventSink));

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
      var endPointIDOfUnloadedObject = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointIDOfUnchangedObject = RelationEndPointID.Create (DomainObjectIDs.Order2, typeof (Order), "OrderItems");
      var endPointIDOfChangedObject = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");

      PrepareLoadedDataContainer (_dataManagerWithMocks, endPointIDOfUnchangedObject.ObjectID);

      var dataContainerOfChangedObject = PrepareLoadedDataContainer (_dataManagerWithMocks, endPointIDOfChangedObject.ObjectID);
      dataContainerOfChangedObject.MarkAsChanged();

      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _endPointManagerMock
          .Expect (mock => mock.CreateUnloadVirtualEndPointsCommand (
              new[] { endPointIDOfUnloadedObject, endPointIDOfUnchangedObject, endPointIDOfChangedObject }))
          .Return (fakeCommand);
      _endPointManagerMock.Replay();

      var result = _dataManagerWithMocks.CreateUnloadVirtualEndPointsCommand (endPointIDOfUnloadedObject, endPointIDOfUnchangedObject, endPointIDOfChangedObject);

      _endPointManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateUnloadVirtualEndPointCommand_NewAndDeletedObjects ()
    {
      var endPointIDOfUnloadedObject = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointIDOfNewObject = RelationEndPointID.Create (DomainObjectIDs.Order2, typeof (Order), "OrderItems");
      var endPointIDOfDeletedObject = RelationEndPointID.Create (DomainObjectIDs.Order3, typeof (Order), "OrderItems");

      PrepareNewDataContainer (_dataManagerWithMocks, endPointIDOfNewObject.ObjectID);
      var dataContainerOfDeletedObject = PrepareLoadedDataContainer (_dataManagerWithMocks, endPointIDOfDeletedObject.ObjectID);
      dataContainerOfDeletedObject.Delete();

      _endPointManagerMock.Replay ();

      var result = _dataManagerWithMocks.CreateUnloadVirtualEndPointsCommand (endPointIDOfUnloadedObject, endPointIDOfNewObject, endPointIDOfDeletedObject);

      Assert.That (result, Is.TypeOf<ExceptionCommand>());
      var exception = ((ExceptionCommand) result).Exception;
      var expectedMessage = string.Format (
          "Cannot unload the following relation end-points because they belong to new or deleted objects: {0}, {1}.", 
          endPointIDOfNewObject, 
          endPointIDOfDeletedObject);
      Assert.That (exception.Message, Is.EqualTo (expectedMessage));
    }

    [Test]
    public void CreateUnloadAllCommand ()
    {
      PrepareLoadedDataContainer (_dataManagerWithMocks);
      PrepareNewDataContainer (_dataManagerWithMocks, DomainObjectIDs.Order1);

      var command = _dataManagerWithMocks.CreateUnloadAllCommand();

      Assert.That (command, Is.TypeOf<UnloadAllCommand>());
      var unloadAllCommand = (UnloadAllCommand) command;
      Assert.That (unloadAllCommand.RelationEndPointManager, Is.SameAs (DataManagerTestHelper.GetRelationEndPointManager (_dataManagerWithMocks)));
      Assert.That (unloadAllCommand.DataContainerMap, Is.SameAs (DataManagerTestHelper.GetDataContainerMap (_dataManagerWithMocks)));
      Assert.That (unloadAllCommand.InvalidDomainObjectManager, Is.SameAs (DataManagerTestHelper.GetInvalidDomainObjectManager (_dataManagerWithMocks)));
      Assert.That (unloadAllCommand.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
    }

    [Test]
    public void CreateUnloadAllCommand_EmptyDataManager ()
    {
      Assert.That (_dataManagerWithMocks.DataContainers, Is.Empty);

      var command = _dataManagerWithMocks.CreateUnloadAllCommand ();
      Assert.That (command, Is.TypeOf<NopCommand> ());
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
      var result = _dataManagerWithMocks.GetDataContainerWithoutLoading (DomainObjectIDs.Order1);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetDataContainerWithoutLoading_Loaded ()
    {
      var dataContainer = PrepareLoadedDataContainer (_dataManagerWithMocks);
      
      var result = _dataManagerWithMocks.GetDataContainerWithoutLoading (dataContainer.ID);

      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetDataContainerWithoutLoading_Invalid ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (true);

      _dataManagerWithMocks.GetDataContainerWithoutLoading (DomainObjectIDs.Order1);
    }

    [Test]
    public void GetDataContainerWithLazyLoad_Loaded ()
    {
      var dataContainer = PrepareLoadedDataContainer (_dataManagerWithMocks);

      _objectLoaderMock.Replay ();

      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad (dataContainer.ID);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    public void GetDataContainerWithLazyLoad_NotLoaded ()
    {
      var dataContainer = PrepareNonLoadedDataContainer ();

      _objectLoaderMock
          .Expect (mock => mock.LoadObject (dataContainer.ID))
          .WhenCalled (mi => DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, dataContainer))
          .Return (new FreshlyLoadedObjectData (dataContainer));
      _objectLoaderMock.Replay ();

      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad (dataContainer.ID);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (dataContainer));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetDataContainerWithLazyLoad_Invalid ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (true);

      _dataManagerWithMocks.GetDataContainerWithLazyLoad (DomainObjectIDs.Order1);
    }

    [Test]
    public void GetDataContainersWithLazyLoad ()
    {
      var loadedDataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);

      var nonLoadedDataContainer1 = PrepareNonLoadedDataContainer ();
      var nonLoadedDataContainer2 = PrepareNonLoadedDataContainer ();

      _objectLoaderMock
          .Expect (mock => mock.LoadObjects (
              Arg<IEnumerable<ObjectID>>.List.Equal (new[] { nonLoadedDataContainer1.ID, nonLoadedDataContainer2.ID }), 
              Arg.Is (true)))
          .WhenCalled (
              mi =>
              {
                DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, nonLoadedDataContainer1);
                DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, nonLoadedDataContainer2);
              })
          .Return (new[] { new FreshlyLoadedObjectData (nonLoadedDataContainer1), new FreshlyLoadedObjectData (nonLoadedDataContainer2) });
      _objectLoaderMock.Replay ();

      var result = _dataManagerWithMocks.GetDataContainersWithLazyLoad (
          new[] { nonLoadedDataContainer1.ID, loadedDataContainer.ID, nonLoadedDataContainer2.ID }, 
          true);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { nonLoadedDataContainer1, loadedDataContainer, nonLoadedDataContainer2 }));
    }

    [Test]
    public void GetDataContainersWithLazyLoad_ThrowOnNotFoundFalse ()
    {
      _objectLoaderMock
          .Expect (mock => mock.LoadObjects (
              Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1 }),
              Arg.Is (false)))
          .Return (new[] { new NullLoadedObjectData() });
      _objectLoaderMock.Replay ();

      var result = _dataManagerWithMocks.GetDataContainersWithLazyLoad (new[] { DomainObjectIDs.Order1 }, false);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new DataContainer[] { null }));
    }

    [Test]
    public void GetDataContainersWithLazyLoad_WithInvalidID ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (DomainObjectIDs.Order1)).Return (true);

      _objectLoaderMock
          .Expect (mock => mock.LoadObjects (Arg<IEnumerable<ObjectID>>.Is.Anything, Arg.Is (true)))
          // evaluate args to trigger exception
          .WhenCalled (mi => ((IEnumerable<ObjectID>) mi.Arguments[0]).ToList())
          .Return (null);
      _objectLoaderMock.Replay ();

      Assert.That (
          () => _dataManagerWithMocks.GetDataContainersWithLazyLoad (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true),
          Throws.TypeOf<ObjectInvalidException>());

    }

    [Test]
    public void LoadLazyCollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      var fakeOrderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      var loadedObjectDataStub = MockRepository.GenerateStub<ILoadedObjectData>();
      loadedObjectDataStub.Stub (stub => stub.GetDomainObjectReference()).Return (fakeOrderItem);

      var endPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub(stub => stub.IsDataComplete).Return(false);
      endPointMock.Expect (mock => mock.MarkDataComplete (Arg<DomainObject[]>.List.Equal (new[] { fakeOrderItem })));
      endPointMock.Replay();

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);
      
      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadRelatedObjects (Arg.Is (endPointID)))
          .Return (new[] { loadedObjectDataStub });
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyCollectionEndPoint (endPointID);

      _objectLoaderMock.VerifyAllExpectations();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given ID does not identify an ICollectionEndPoint managed by this DataManager.\r\nParameter name: endPointID")]
    public void LoadLazyCollectionEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (null);

      _dataManagerWithMocks.LoadLazyCollectionEndPoint (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given ID does not identify an ICollectionEndPoint managed by this DataManager.\r\nParameter name: endPointID")]
    public void LoadLazyCollectionEndPoint_NotICollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointStub);

      _dataManagerWithMocks.LoadLazyCollectionEndPoint (endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given end-point cannot be loaded, its data is already complete.")]
    public void LoadLazyCollectionEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointStub);
      

      _dataManagerWithMocks.LoadLazyCollectionEndPoint (endPointID);
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_MarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var fakeOrderTicket = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      var loadedObjectDataStub = MockRepository.GenerateStub<ILoadedObjectData> ();
      loadedObjectDataStub.Stub (stub => stub.GetDomainObjectReference ()).Return (fakeOrderTicket);
      
      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub (stub => stub.IsDataComplete).Return (false).Repeat.Once();
      endPointMock.Replay ();

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadRelatedObject (Arg.Is (endPointID)))
          .Return (loadedObjectDataStub)
          .WhenCalled (mi => endPointMock.Stub (stub => stub.IsDataComplete).Return (true));
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointID);

      _objectLoaderMock.VerifyAllExpectations ();

      endPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (fakeOrderTicket));
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_NotMarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      var fakeOrderTicket = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      var loadedObjectDataStub = MockRepository.GenerateStub<ILoadedObjectData> ();
      loadedObjectDataStub.Stub (stub => stub.GetDomainObjectReference ()).Return (fakeOrderTicket);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      endPointMock.Expect (mock => mock.MarkDataComplete (fakeOrderTicket));
      endPointMock.Replay ();

      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointMock);

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadRelatedObject (Arg.Is (endPointID)))
          .Return (loadedObjectDataStub);
      _objectLoaderMock.Replay ();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointID);

      _objectLoaderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.\r\nParameter name: endPointID")]
    public void LoadLazyVirtualObjectEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (null);

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.\r\nParameter name: endPointID")]
    public void LoadLazyVirtualObjectEndPoint_NotIVirtualObjectEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      _endPointManagerMock.Stub (stub => stub.GetRelationEndPointWithoutLoading (endPointID)).Return (endPointStub);

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint (endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The given end-point cannot be loaded, its data is already complete.")]
    public void LoadLazyVirtualObjectEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = (IVirtualObjectEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (endPointID);
      Assert.That (endPoint.IsDataComplete, Is.True);

      _dataManager.LoadLazyVirtualObjectEndPoint (endPointID);
    }

    [Test]
    public void LoadLazyDataContainer ()
    {
      var fakeDataContainer = PrepareNonLoadedDataContainer ();

      _objectLoaderMock
          .Expect (mock => mock.LoadObject (fakeDataContainer.ID))
          .Return (new FreshlyLoadedObjectData (fakeDataContainer))
          .WhenCalled (mi => DataManagerTestHelper.AddDataContainer (_dataManagerWithMocks, fakeDataContainer));
      _objectLoaderMock.Replay();

      var result = _dataManagerWithMocks.LoadLazyDataContainer (fakeDataContainer.ID);

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
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetPropertyIdentifier (typeof (Order), "OrderTicket"));
      var fakeEndPoint = MockRepository.GenerateStub<IRelationEndPoint> ();

      _endPointManagerMock.Expect (mock => mock.GetRelationEndPointWithLazyLoad (endPointID)).Return (fakeEndPoint);
      _endPointManagerMock.Replay ();

      var result = _dataManagerWithMocks.GetRelationEndPointWithLazyLoad (endPointID);

      _endPointManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeEndPoint));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetPropertyIdentifier (typeof (Order), "OrderTicket"));
      var fakeEndPoint = MockRepository.GenerateStub<IRelationEndPoint> ();

      _endPointManagerMock.Expect (mock => mock.GetRelationEndPointWithoutLoading (endPointID)).Return (fakeEndPoint);
      _endPointManagerMock.Replay ();

      var result = _dataManagerWithMocks.GetRelationEndPointWithoutLoading (endPointID);

      _endPointManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeEndPoint));
    }

    [Test]
    public void GetOrCreateVirtualEndPoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetPropertyIdentifier (typeof (Order), "OrderTicket"));
      var fakeVirtualEndPoint = MockRepository.GenerateStub<IVirtualEndPoint>();

      _endPointManagerMock.Expect (mock => mock.GetOrCreateVirtualEndPoint (endPointID)).Return (fakeVirtualEndPoint);
      _endPointManagerMock.Replay();

      var result = _dataManagerWithMocks.GetOrCreateVirtualEndPoint (endPointID);

      _endPointManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeVirtualEndPoint));
    }

    private PersistableData CreatePersistableData (DomainObject domainObject)
    {
      var dataContainer = TestableClientTransaction.DataManager.DataContainers[domainObject.ID];
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

    private DataContainer PrepareNonLoadedDataContainer ()
    {
      var nonLoadedDomainObject = DomainObjectMother.CreateFakeObject<Order> ();
      var nonLoadedDataContainer = DataContainer.CreateNew (nonLoadedDomainObject.ID);
      return nonLoadedDataContainer;
    }

    private DataContainer PrepareLoadedDataContainer (DataManager dataManager)
    {
      return PrepareLoadedDataContainer (dataManager, new ObjectID (typeof (Order), Guid.NewGuid()));
    }

    private DataContainer PrepareLoadedDataContainer (DataManager dataManager, ObjectID objectID)
    {
      var loadedDomainObject = DomainObjectMother.CreateFakeObject (objectID);
      var loadedDataContainer = DataContainer.CreateForExisting (objectID, null, pd => pd.DefaultValue);
      loadedDataContainer.SetDomainObject (loadedDomainObject);
      DataManagerTestHelper.AddDataContainer (dataManager, loadedDataContainer);
      return loadedDataContainer;
    }

    private DataContainer PrepareNewDataContainer (DataManager dataManager, ObjectID objectID)
    {
      var loadedDomainObject = DomainObjectMother.CreateFakeObject (objectID);
      var loadedDataContainer = DataContainer.CreateNew (objectID);
      loadedDataContainer.SetDomainObject (loadedDomainObject);
      DataManagerTestHelper.AddDataContainer (dataManager, loadedDataContainer);
      return loadedDataContainer;
    }

  }
}
