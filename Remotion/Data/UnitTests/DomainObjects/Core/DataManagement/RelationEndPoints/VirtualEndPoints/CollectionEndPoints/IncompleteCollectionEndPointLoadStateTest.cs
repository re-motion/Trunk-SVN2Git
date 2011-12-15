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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPoint _collectionEndPointMock;

    private ILazyLoader _lazyLoaderMock;
    private IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _dataKeeperFactoryStub;

    private IncompleteCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    private Order _relatedObject2;
    private IRealObjectEndPoint _relatedEndPointStub2;
    
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
    
      _lazyLoaderMock = MockRepository.GenerateStrictMock<ILazyLoader> ();
      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();
      
      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.HasDataChanged()).Return (false);

      _loadState = new IncompleteCollectionEndPointLoadState (_lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (_relatedObject2.ID);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _lazyLoaderMock.Expect (mock => mock.LoadLazyCollectionEndPoint (_collectionEndPointMock));
      _lazyLoaderMock.Replay ();
      _collectionEndPointMock.Replay ();

      _loadState.EnsureDataComplete (_collectionEndPointMock);

      _lazyLoaderMock.VerifyAllExpectations ();
      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataKeeper ()
    {
      bool stateSetterCalled = false;

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser>());
      _collectionEndPointMock.Replay ();

      var newKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();
      newKeeperMock.Replay ();
      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (
          _collectionEndPointMock,
          new DomainObject[0],
          keeper =>
          {
            stateSetterCalled = true;
            Assert.That (keeper, Is.SameAs (newKeeperMock));
          });

      Assert.That (stateSetterCalled, Is.True);
      newKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItems_AreRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);
      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub2);
      
      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ());
      // ReSharper disable AccessToModifiedClosure
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub2))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _collectionEndPointMock.Replay ();

      var newKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();
      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperStub);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[0], keeper => stateSetterCalled = true);

      _collectionEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_Items_AreRegisteredInOrder_WithOrWithoutEndPoints ()
    {
      var oppositeEndPointForItem1Mock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointForItem1Mock.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
      oppositeEndPointForItem1Mock.Stub (stub => stub.ResetSyncState());
      oppositeEndPointForItem1Mock.Expect (mock => mock.MarkSynchronized ());
      oppositeEndPointForItem1Mock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, oppositeEndPointForItem1Mock);
      
      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser ()).Return (MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ());
      _collectionEndPointMock.Replay ();
      
      var newKeeperMock = MockRepository.GenerateMock<ICollectionEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointForItem1Mock));
        newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (_relatedObject2));
      }
      newKeeperMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { _relatedObject, _relatedObject2 }, keeper => { });

      newKeeperMock.VerifyAllExpectations ();
      oppositeEndPointForItem1Mock.VerifyAllExpectations ();
      _collectionEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    public void MarkDataComplete_RaisesEvent ()
    {
      var counter = new OrderedExpectationCounter ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);

      var eventRaiserMock = MockRepository.GenerateStrictMock<IDomainObjectCollectionEventRaiser> ();

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Stub (stub => stub.GetCollectionEventRaiser()).Return (eventRaiserMock);
      _collectionEndPointMock.Replay ();

      var newKeeperMock = MockRepository.GenerateMock<ICollectionEndPointDataKeeper> ();
      newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub)).Ordered (counter);
      newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (_relatedObject2)).Ordered (counter);
      newKeeperMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      var expectedKeeperPosition = counter.GetNextExpectedPosition ();
      Action<ICollectionEndPointDataKeeper> stateSetter = keeper => counter.CheckPosition ("stateSetter", expectedKeeperPosition);

      eventRaiserMock.Expect (mock => mock.WithinReplaceData()).Ordered(counter);
      eventRaiserMock.Replay();

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { _relatedObject, _relatedObject2 }, stateSetter);

      newKeeperMock.VerifyAllExpectations ();
      eventRaiserMock.VerifyAllExpectations();
    }

    [Test]
    public void SortCurrentData ()
    {
      Comparison<DomainObject> comparison = (one, two) => 0;
      CheckOperationDelegatesToCompleteState (
          s => s.SortCurrentData (_collectionEndPointMock, comparison),
          s => s.SortCurrentData (comparison));
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection ();

      Action<DomainObjectCollection> fakeSetter = collection => { };
      var fakeManager = MockRepository.GenerateStub<ICollectionEndPointCollectionManager>();
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCollectionCommand (_collectionEndPointMock, domainObjectCollection, fakeSetter, fakeManager),
          s => s.CreateSetCollectionCommand (domainObjectCollection),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateRemoveCommand (_collectionEndPointMock, _relatedObject),
          s => s.CreateRemoveCommand (_relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateDeleteCommand (_collectionEndPointMock), 
          s => s.CreateDeleteCommand (), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateInsertCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateInsertCommand (_collectionEndPointMock, _relatedObject, 0), 
          s => s.CreateInsertCommand (_relatedObject, 0), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateAddCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateAddCommand (_collectionEndPointMock, _relatedObject),
          s => s.CreateAddCommand (_relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateReplaceCommand (_collectionEndPointMock, 0, _relatedObject),
          s => s.CreateReplaceCommand (0, _relatedObject), 
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var lazyLoader = new SerializableLazyLoaderFake ();
      var dataKeeperFactory = new SerializableCollectionEndPointDataKeeperFactoryFake();

      var state = new IncompleteCollectionEndPointLoadState (lazyLoader, dataKeeperFactory);

      var oppositeEndPoint = new SerializableRealObjectEndPointFake(null, _relatedObject);
      state.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
      Assert.That (result.LazyLoader, Is.Not.Null);
      Assert.That (result.DataKeeperFactory, Is.Not.Null);
    }
    
    private void CheckOperationDelegatesToCompleteState<T> (
        Func<ICollectionEndPointLoadState, T> operation, 
        Func<ICollectionEndPoint, T> operationOnEndPoint, 
        T fakeResult)
    {
      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _collectionEndPointMock.Expect (mock => operationOnEndPoint (mock)).Return (fakeResult);
      }
      _collectionEndPointMock.Replay ();

      var result = operation (_loadState);

      _collectionEndPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    private void CheckOperationDelegatesToCompleteState (
        Action<ICollectionEndPointLoadState> operation,
        Action<ICollectionEndPoint> operationOnEndPoint)
    {
      using (_collectionEndPointMock.GetMockRepository ().Ordered ())
      {
        _collectionEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _collectionEndPointMock.Expect (operationOnEndPoint);
      }
      _collectionEndPointMock.Replay ();

      operation (_loadState);

      _collectionEndPointMock.VerifyAllExpectations ();
    }
  }
}