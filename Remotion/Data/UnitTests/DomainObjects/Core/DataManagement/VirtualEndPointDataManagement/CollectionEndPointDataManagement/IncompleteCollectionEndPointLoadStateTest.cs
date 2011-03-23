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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private ICollectionEndPoint _collectionEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private ICollectionEndPointDataKeeper _dataKeeperMock;
    private ICollectionEndPointDataKeeperFactory _dataKeeperFactoryStub;

    private IncompleteCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    private RelationEndPointID _endPointID;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();
      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]).Repeat.Once(); // for ctor called below

      _dataKeeperFactoryStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeperFactory>();

      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeperMock, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
    }

    [Test]
    public void Initialization_WithOriginalEndPoints ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new[] { endPointMock });

      new IncompleteCollectionEndPointLoadState (dataKeeperStub, _lazyLoaderMock, _dataKeeperFactoryStub);

      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete (), Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _lazyLoaderMock.Expect (mock => mock.LoadLazyCollectionEndPoint (_collectionEndPointMock));
      _lazyLoaderMock.Replay();
      _collectionEndPointMock.Replay();
      
      _loadState.EnsureDataComplete (_collectionEndPointMock);

      _lazyLoaderMock.VerifyAllExpectations();
      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataKeeper ()
    {
      bool stateSetterCalled = false;

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Stub (stub => stub.EndPointID).Return (_endPointID);
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);
      _dataKeeperMock.Replay();

      var newKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();
      newKeeperMock.Replay();

      _collectionEndPointMock.Replay();

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
      newKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItems_AreRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Stub (stub => stub.EndPointID).Return (_endPointID);
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoints).Return (new [] { _relatedEndPointStub });
      _dataKeeperMock.Replay ();

      var newKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper> ();

      // ReSharper disable AccessToModifiedClosure
      _collectionEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _collectionEndPointMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperStub);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[0], keeper => stateSetterCalled = true);

      _collectionEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataComplete_Items_AreRegisteredInOrder_WithOrWithoutEndPoints ()
    {
      var item1 = DomainObjectMother.CreateFakeObject<Order> ();
      var item2 = DomainObjectMother.CreateFakeObject<Order> ();

      var oppositeEndPointForItem1Mock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointForItem1Mock.Stub (stub => stub.ObjectID).Return (item1.ID);
      oppositeEndPointForItem1Mock.Expect (mock => mock.MarkSynchronized());
      oppositeEndPointForItem1Mock.Replay ();

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Stub (stub => stub.EndPointID).Return (_endPointID);
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoints).Return (new[] { oppositeEndPointForItem1Mock });
      _dataKeeperMock.Replay ();

      var newKeeperMock = MockRepository.GenerateMock<ICollectionEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointForItem1Mock));
        newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (item2));
      }
      newKeeperMock.Replay();

      _collectionEndPointMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { item1, item2 }, keeper => { });

      newKeeperMock.VerifyAllExpectations();
      oppositeEndPointForItem1Mock.VerifyAllExpectations();
      _collectionEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The data is already incomplete.")]
    public void MarkDataIncomplete_ThrowsException ()
    {
      _loadState.MarkDataIncomplete (_collectionEndPointMock, keeper => Assert.Fail ("Must not be called."));
    }

    [Test]
    public void GetCollectionData ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.GetCollectionData (_collectionEndPointMock),
          s => s.GetCollectionData (),
          new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), true));
    }

    [Test]
    public void GetOriginalCollectionData ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.GetOriginalCollectionData (_collectionEndPointMock),
          s => s.GetOriginalCollectionData (),
          new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData (), true));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();

      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);

      endPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_DataKeeperThrows ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Replay ();

      var exception = new Exception ("Test");
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock)).Throw (exception);
      _dataKeeperMock.Replay ();

      try
      {
        _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }

      endPointMock.AssertWasNotCalled (mock => mock.ResetSyncState ());
      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInDataKeeper ()
    {
      _dataKeeperMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_relatedEndPointStub));
      _dataKeeperMock.Replay ();

      _loadState.UnregisterOriginalOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.RegisterCurrentOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub),
         s => s.RegisterCurrentOppositeEndPoint(_relatedEndPointStub));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.UnregisterCurrentOppositeEndPoint (_collectionEndPointMock, _relatedEndPointStub),
         s => s.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub));
    }

    [Test]
    public void IsSynchronized ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.IsSynchronized (_collectionEndPointMock),
         s => s.IsSynchronized,
         true);
    }

    [Test]
    public void Synchronize ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.Synchronize (_collectionEndPointMock),
         s => s.Synchronize());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize an opposite end-point with a collection end-point in incomplete state.")]
    public void SynchronizeOppositeEndPoint ()
    {
      _relatedEndPointStub.Stub (stub => stub.ID).Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"));

      _loadState.SynchronizeOppositeEndPoint (_relatedEndPointStub);
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection ();

      Action<DomainObjectCollection> fakeSetter = collection => { };
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCollectionCommand (_collectionEndPointMock, domainObjectCollection, fakeSetter),
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
    public void SetValueFrom ()
    {
      var fakeSourceEndPoint = MockRepository.GenerateStub<ICollectionEndPoint> ();
      CheckOperationDelegatesToCompleteState (s => s.SetValueFrom (_collectionEndPointMock, fakeSourceEndPoint), s => s.SetValueFrom (fakeSourceEndPoint));
    }

    [Test]
    public void HasChanged ()
    {
      _dataKeeperMock.Expect (mock => mock.HasDataChanged ()).Return (true);
      _dataKeeperMock.Replay ();

      var result = _loadState.HasChanged ();

      _dataKeeperMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Commit ()
    {
      _dataKeeperMock.Expect (mock => mock.Commit ());
      _dataKeeperMock.Replay ();

      _loadState.Commit ();

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void Rollback ()
    {
      _dataKeeperMock.Expect (mock => mock.Rollback ());
      _dataKeeperMock.Replay ();

      _loadState.Rollback ();

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableCollectionEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var dataKeeperFactory = new SerializableCollectionEndPointDataKeeperFactoryFake();

      var state = new IncompleteCollectionEndPointLoadState (dataKeeper, lazyLoader, dataKeeperFactory);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.LazyLoader, Is.Not.Null);
      Assert.That (result.DataKeeperFactory, Is.Not.Null);
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
  }
}