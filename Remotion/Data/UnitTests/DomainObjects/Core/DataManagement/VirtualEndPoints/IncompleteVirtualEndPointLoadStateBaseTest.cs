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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.CollectionEndPoints.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints
{
  [TestFixture]
  public class IncompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private IVirtualEndPoint<object> _virtualEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private IVirtualEndPointDataKeeper _dataKeeperMock;
    private IVirtualEndPointDataKeeperFactory<IVirtualEndPointDataKeeper> _dataKeeperFactoryStub;

    private TestableIncompleteVirtualEndPointLoadState _loadState;

    private IRealObjectEndPoint _relatedEndPointStub;

    private RelationEndPointID _endPointID;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint<object>> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _dataKeeperMock = MockRepository.GenerateStrictMock<IVirtualEndPointDataKeeper> ();

      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualEndPointDataKeeper>> ();

      _loadState = new TestableIncompleteVirtualEndPointLoadState (_dataKeeperMock, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete (), Is.False);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _lazyLoaderMock.Expect (mock => mock.LoadLazyVirtualEndPoint (_virtualEndPointMock));
      _lazyLoaderMock.Replay();
      _virtualEndPointMock.Replay();
      
      _loadState.EnsureDataComplete (_virtualEndPointMock);

      _lazyLoaderMock.VerifyAllExpectations();
      _virtualEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataKeeper ()
    {
      bool stateSetterCalled = false;

      _loadState.StubOriginalOppositeEndPoints(new IRealObjectEndPoint[0]);

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay();

      var newKeeperMock = MockRepository.GenerateStrictMock<IVirtualEndPointDataKeeper> ();
      newKeeperMock.Replay();

      _virtualEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualEndPointMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (
          _virtualEndPointMock,
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

      _loadState.StubOriginalOppositeEndPoints (new[] { _relatedEndPointStub });

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      var newKeeperStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeper> ();

      _virtualEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      // ReSharper disable AccessToModifiedClosure
      _virtualEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _virtualEndPointMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperStub);

      _loadState.MarkDataComplete (_virtualEndPointMock, new DomainObject[0], keeper => stateSetterCalled = true);

      _virtualEndPointMock.VerifyAllExpectations();
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

      _loadState.StubOriginalOppositeEndPoints (new[] { oppositeEndPointForItem1Mock });

      _dataKeeperMock.Stub (stub => stub.HasDataChanged ()).Return (false);
      _dataKeeperMock.Replay ();

      var newKeeperMock = MockRepository.GenerateMock<IVirtualEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointForItem1Mock));
        newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (item2));
      }
      newKeeperMock.Replay();

      _virtualEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualEndPointMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_virtualEndPointMock, new DomainObject[] { item1, item2 }, keeper => { });

      newKeeperMock.VerifyAllExpectations();
      oppositeEndPointForItem1Mock.VerifyAllExpectations();
      _virtualEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The data is already incomplete.")]
    public void MarkDataIncomplete_ThrowsException ()
    {
      _loadState.MarkDataIncomplete (_virtualEndPointMock, keeper => Assert.Fail ("Must not be called."));
    }

    [Test]
    public void GetData ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.GetData (_virtualEndPointMock),
          s => s.GetData (),
          new object());
    }

    [Test]
    public void GetOriginalData ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.GetOriginalData (_virtualEndPointMock),
          s => s.GetOriginalData (),
          new object());
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();

      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

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
        _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
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

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.RegisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub),
         s => s.RegisterCurrentOppositeEndPoint(_relatedEndPointStub));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.UnregisterCurrentOppositeEndPoint (_virtualEndPointMock, _relatedEndPointStub),
         s => s.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub));
    }

    [Test]
    public void IsSynchronized ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.IsSynchronized (_virtualEndPointMock),
         s => s.IsSynchronized,
         true);
    }

    [Test]
    public void Synchronize ()
    {
      CheckOperationDelegatesToCompleteState (
         s => s.Synchronize (_virtualEndPointMock),
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
    public void SetValueFrom ()
    {
      var fakeSourceEndPoint = MockRepository.GenerateStub<IVirtualEndPoint<object>> ();
      CheckOperationDelegatesToCompleteState (
          s => s.SetValueFrom (_virtualEndPointMock, fakeSourceEndPoint), 
          s => s.SetValueFrom (fakeSourceEndPoint));
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
      var dataKeeper = new SerializableVirtualEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var dataKeeperFactory = new SerializableVirtualEndPointDataKeeperFactoryFake();

      var state = new TestableIncompleteVirtualEndPointLoadState (dataKeeper, lazyLoader, dataKeeperFactory);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.LazyLoader, Is.Not.Null);
      Assert.That (result.DataKeeperFactory, Is.Not.Null);
    }

    private void CheckOperationDelegatesToCompleteState (
        Action<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataKeeper>> operation,
        Action<IVirtualEndPoint<object>> operationOnEndPoint)
    {
      using (_virtualEndPointMock.GetMockRepository ().Ordered ())
      {
        _virtualEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _virtualEndPointMock.Expect (operationOnEndPoint);
      }
      _virtualEndPointMock.Replay ();

      operation (_loadState);

      _virtualEndPointMock.VerifyAllExpectations ();
    }

    private void CheckOperationDelegatesToCompleteState<T> (
        Func<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataKeeper>, T> operation, 
        Func<IVirtualEndPoint<object>, T> operationOnEndPoint, 
        T fakeResult)
    {
      using (_virtualEndPointMock.GetMockRepository ().Ordered ())
      {
        _virtualEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _virtualEndPointMock.Expect (mock => operationOnEndPoint (mock)).Return (fakeResult);
      }
      _virtualEndPointMock.Replay ();

      var result = operation (_loadState);

      _virtualEndPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }
  }
}