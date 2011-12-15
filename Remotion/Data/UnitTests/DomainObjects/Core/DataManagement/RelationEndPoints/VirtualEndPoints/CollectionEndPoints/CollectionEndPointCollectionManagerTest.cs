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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CollectionEndPointCollectionManagerTest : StandardMappingTest
  {
    private IAssociatedCollectionDataStrategyFactory _associatedCollectionDataStrategyFactoryMock;
    private ClientTransaction _clientTransaction;
    private IClientTransactionListener _transactionEventSinkDynamicMock;
      
      private CollectionEndPointCollectionManager _manager;

    private RelationEndPointID _endPointID;

    private ICollectionEndPoint _endPointStub;
    private IDomainObjectCollectionData _dataStrategyStub;


    public override void SetUp ()
    {
      base.SetUp ();

      _associatedCollectionDataStrategyFactoryMock = MockRepository.GenerateStrictMock<IAssociatedCollectionDataStrategyFactory>();
      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _transactionEventSinkDynamicMock = MockRepository.GenerateMock<IClientTransactionListener>();
      
      _manager = new CollectionEndPointCollectionManager (_associatedCollectionDataStrategyFactoryMock, _clientTransaction, _transactionEventSinkDynamicMock);

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
    
      _endPointStub = CreateEndPointStubWithID (_endPointID);

      _dataStrategyStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _dataStrategyStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
      _dataStrategyStub.Stub (stub => stub.GetDataStore()).Return (new DomainObjectCollectionData());
    }

    [Test]
    public void GetOriginalCollectionReference ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _associatedCollectionDataStrategyFactoryMock.Replay ();

      var result = _manager.GetOriginalCollectionReference (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (result), Is.SameAs (_dataStrategyStub));

      Assert.That (result, Is.SameAs (_manager.GetOriginalCollectionReference (_endPointStub)));
    }

    [Test]
    public void GetOriginalCollectionReference_Twice_ForSameID ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub)
          .Repeat.Once ();
      _associatedCollectionDataStrategyFactoryMock.Replay ();

      var result1 = _manager.GetOriginalCollectionReference (_endPointStub);

      ICollectionEndPoint secondEndPointStub = CreateEndPointStubWithID (_endPointID);

      var result2 = _manager.GetOriginalCollectionReference (secondEndPointStub);

      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void GetOriginalCollectionReference_CollectionWithWrongCtor ()
    {
      var classDefinition = GetTypeDefinition (typeof (DomainObjectWithCollectionMissingCtor));
      var relationEndPointDefinition = GetEndPointDefinition (typeof (DomainObjectWithCollectionMissingCtor), "OppositeObjects");
      var endPointID = RelationEndPointID.Create (new ObjectID (classDefinition, Guid.NewGuid ()), relationEndPointDefinition);

      var endPointStub = CreateEndPointStubWithID (endPointID);

      _associatedCollectionDataStrategyFactoryMock
          .Stub (mock => mock.CreateDataStrategyForEndPoint (endPointStub))
          .Return (_dataStrategyStub);

      Assert.That (() => _manager.GetOriginalCollectionReference (endPointStub), Throws.TypeOf<MissingMethodException> ()
          .With.Message.ContainsSubstring ("does not provide a constructor taking an IDomainObjectCollectionData object"));
    }


    [Test]
    public void GetCurrentCollectionReference_UsesOriginalReference ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _associatedCollectionDataStrategyFactoryMock.Replay ();

      var originalResult = _manager.GetOriginalCollectionReference (_endPointStub);
      var currentResult = _manager.GetCurrentCollectionReference (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations ();
      Assert.That (currentResult, Is.SameAs (originalResult));
    }

    [Test]
    public void GetCurrentCollectionReference_CreatesOriginalReference_IfNoneAvailable ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _associatedCollectionDataStrategyFactoryMock.Replay();

      var result = _manager.GetCurrentCollectionReference (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (result), Is.SameAs (_dataStrategyStub));

      Assert.That (result, Is.SameAs (_manager.GetOriginalCollectionReference (_endPointStub)));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var result = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);

      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (result), Is.SameAs (_dataStrategyStub));
    }

    [Test]
    public void GetCollectionWithOriginalData_Twice ()
    {
      var result1 = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);
      var result2 = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);

      Assert.That (result2, Is.Not.SameAs (result1));
    }

    [Test]
    public void AssociateCollectionWithEndPoint ()
    {
      var oldCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection>();
      oldCollectionMock.Stub (mock => ((IAssociatableDomainObjectCollection) mock).IsAssociatedWith (_endPointStub)).Return (true);
      oldCollectionMock.Expect (mock => ((IAssociatableDomainObjectCollection) mock).TransformToStandAlone ());
      oldCollectionMock.Replay();

      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Expect (
              mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointStub, _associatedCollectionDataStrategyFactoryMock));
      newCollectionMock.Replay();

      RegisterOriginalCollection (_endPointID, oldCollectionMock);

      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollectionMock);

      oldCollectionMock.VerifyAllExpectations ();
      newCollectionMock.VerifyAllExpectations ();
      _transactionEventSinkDynamicMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointStub.ID, null));
    }

    [Test]
    public void AssociateCollectionWithEndPoint_RemembersTheNewCollectionAsCurrent ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      _dataStrategyStub.Stub (stub => stub.AssociatedEndPoint).Return (_endPointStub);

      var collectionBefore = _manager.GetCurrentCollectionReference (_endPointStub);
      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (collectionBefore));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (collectionBefore));

      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Stub (
              mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointStub, _associatedCollectionDataStrategyFactoryMock));

      _endPointStub.Stub (stub => stub.Collection).Return (collectionBefore);

      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollectionMock);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (newCollectionMock));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (collectionBefore));
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCollectionsYet ()
    {
      _associatedCollectionDataStrategyFactoryMock.Replay();

      var result = _manager.HasCollectionReferenceChanged (_endPointStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_NoCurrentCollectionYet ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _manager.GetOriginalCollectionReference (_endPointStub);

      var result = _manager.HasCollectionReferenceChanged (_endPointStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_False_CurrentCollectionSameAsOriginal ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _manager.GetOriginalCollectionReference (_endPointStub);
      _manager.GetCurrentCollectionReference (_endPointStub);

      var result = _manager.HasCollectionReferenceChanged (_endPointStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void HasCollectionReferenceChanged_True_CurrentCollectionChanged ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      _dataStrategyStub.Stub (stub => stub.AssociatedEndPoint).Return (_endPointStub);
      _manager.AssociateCollectionWithEndPoint (_endPointStub, new OrderCollection ());

      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.Not.SameAs (_manager.GetCurrentCollectionReference (_endPointStub)));

      var result = _manager.HasCollectionReferenceChanged (_endPointStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CommitCollectionReference_NoOriginalCollection ()
    {
      _manager.CommitCollectionReference (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (_manager.GetOriginalCollectionReference (_endPointStub)));
    }

    [Test]
    public void CommitCollectionReference_NoCurrentCollection ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);

      var originalBefore = _manager.GetOriginalCollectionReference (_endPointStub);

      _manager.CommitCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (originalBefore));
    }

    [Test]
    public void CommitCollectionReference_NoChanges ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);

      var originalBefore = _manager.GetOriginalCollectionReference (_endPointStub);
      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));

      _manager.CommitCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (originalBefore));
    }

    [Test]
    public void CommitCollectionReference_Changes ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      _dataStrategyStub.Stub (stub => stub.AssociatedEndPoint).Return (_endPointStub);

      var newCollection = new OrderCollection();
      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollection);

      _manager.CommitCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (newCollection));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (newCollection));
    }

    [Test]
    public void RollbackCollectionReference_NoOriginalCollection ()
    {
      _manager.RollbackCollectionReference (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (_manager.GetOriginalCollectionReference (_endPointStub)));
    }

    [Test]
    public void RollbackCollectionReference_NoCurrentCollection ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);

      var originalBefore = _manager.GetOriginalCollectionReference (_endPointStub);

      _manager.RollbackCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_NoChanges ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);

      var originalBefore = _manager.GetOriginalCollectionReference (_endPointStub);
      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));

      _manager.RollbackCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalBefore));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (originalBefore));
    }

    [Test]
    public void RollbackCollectionReference_UndoesAssociation ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      _dataStrategyStub.Stub (stub => stub.AssociatedEndPoint).Return (_endPointStub);

      var originalCollection = _manager.GetOriginalCollectionReference (_endPointStub);

      var newCollection = new OrderCollection ();
      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollection);

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (_dataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (originalCollection, typeof (Order));

      _transactionEventSinkDynamicMock.BackToRecord ();
      _transactionEventSinkDynamicMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, null));
      _transactionEventSinkDynamicMock.Replay ();

      // The Rollback operation must now transform the new collection to a standalone collection and reassociate the original collection with the end-
      // point being rolled back. (In addition to making the original collection the current collection again.)
      
      _manager.RollbackCollectionReference (_endPointStub);

      Assert.That (_manager.GetCurrentCollectionReference (_endPointStub), Is.SameAs (originalCollection));
      Assert.That (_manager.GetOriginalCollectionReference (_endPointStub), Is.SameAs (originalCollection));

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (originalCollection), Is.SameAs (_dataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (newCollection, typeof (Order));

      _transactionEventSinkDynamicMock.VerifyAllExpectations();
    }

    [Test]
    public void RollbackCollectionReference_LeavesNewCollectionAloneIfAlreadyReassociatedWithOther ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);
      _dataStrategyStub.Stub (stub => stub.AssociatedEndPoint).Return (_endPointStub);
      var originalCollection = _manager.GetOriginalCollectionReference (_endPointStub);

      var newCollection = new OrderCollection ();
      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollection);

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (_dataStrategyStub));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (originalCollection, typeof (Order));
      
      // Simulate that newCollection has already been re-associated by another rollback operation.
      // The Rollback operation must leave this other strategy alone.
      var otherStrategy = new DomainObjectCollectionData();
      DomainObjectCollectionDataTestHelper.SetDataStrategy (newCollection, otherStrategy);

      _transactionEventSinkDynamicMock.BackToRecord();
      _transactionEventSinkDynamicMock.Expect (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, null));
      _transactionEventSinkDynamicMock.Replay();

      _manager.RollbackCollectionReference (_endPointStub);

      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (originalCollection), Is.SameAs (_dataStrategyStub));
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (newCollection), Is.SameAs (otherStrategy));

      _transactionEventSinkDynamicMock.VerifyAllExpectations();
    }

    [Test]
    public void Serialization ()
    {
      var instance = new CollectionEndPointCollectionManager (
          new SerializableAssociatedCollectionDataStrategyFactoryFake(), 
          _clientTransaction, 
          ClientTransactionTestHelper.GetTransactionEventSink (_clientTransaction));

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.DataStrategyFactory, Is.Not.Null);
      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.TransactionEventSink, Is.Not.Null);
    }

    private ICollectionEndPoint CreateEndPointStubWithID (RelationEndPointID relationEndPointID)
    {
      var secondEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      secondEndPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      return secondEndPointStub;
    }

    private void RegisterOriginalCollection (RelationEndPointID endPointID, DomainObjectCollection domainObjectCollection)
    {
      var dataStore =
          (SimpleDataStore<RelationEndPointID, DomainObjectCollection>) PrivateInvoke.GetNonPublicField (_manager, "_originalCollectionReferences");
      dataStore.Add (endPointID, domainObjectCollection);
    }
  }
}