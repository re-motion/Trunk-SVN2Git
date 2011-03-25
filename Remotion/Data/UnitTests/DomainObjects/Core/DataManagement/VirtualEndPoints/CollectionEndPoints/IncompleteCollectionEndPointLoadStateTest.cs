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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private ICollectionEndPoint _collectionEndPointMock;

    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private ICollectionEndPointDataKeeper _dataKeeperMock;
    private IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _dataKeeperFactoryStub;

    private IncompleteCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;
    private IRealObjectEndPoint _relatedEndPointStub2;


    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
    
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();
      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();
      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();

      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeperMock, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataKeeper ()
    {
      bool stateSetterCalled = false;

      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IRealObjectEndPoint[0]);
      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
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

      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order2);

      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new[] { _relatedEndPointStub, _relatedEndPointStub2 });
      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();
      
      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
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
      var item1 = DomainObjectMother.CreateFakeObject<Order> ();
      var item2 = DomainObjectMother.CreateFakeObject<Order> ();

      var oppositeEndPointForItem1Mock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointForItem1Mock.Stub (stub => stub.ObjectID).Return (item1.ID);
      oppositeEndPointForItem1Mock.Expect (mock => mock.MarkSynchronized ());
      oppositeEndPointForItem1Mock.Replay ();

      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new[] { oppositeEndPointForItem1Mock });

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      _collectionEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _collectionEndPointMock.Replay ();
      
      var newKeeperMock = MockRepository.GenerateMock<ICollectionEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointForItem1Mock));
        newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (item2));
      }
      newKeeperMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_collectionEndPointMock, new DomainObject[] { item1, item2 }, keeper => { });

      newKeeperMock.VerifyAllExpectations ();
      oppositeEndPointForItem1Mock.VerifyAllExpectations ();
      _collectionEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
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
    public void GetOriginalOppositeEndPoints ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoints).Return (new[] { _relatedEndPointStub });
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedEndPointStub }));
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