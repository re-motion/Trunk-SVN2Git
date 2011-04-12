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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints
{
  [TestFixture]
  public class IncompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private IVirtualEndPoint<object> _virtualEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private IVirtualEndPointDataKeeperFactory<IVirtualEndPointDataKeeper> _dataKeeperFactoryStub;

    private TestableIncompleteVirtualEndPointLoadState _loadState;

    private IRealObjectEndPoint _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint<object>>();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader>();

      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualEndPointDataKeeper>>();

      _loadState = new TestableIncompleteVirtualEndPointLoadState (new IRealObjectEndPoint[0], _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
    }

    [Test]
    public void Initialization ()
    {
      var realObjectEndPoint1 = MockRepository.GenerateStub<IRealObjectEndPoint>();
      realObjectEndPoint1.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      var realObjectEndPoint2 = MockRepository.GenerateStub<IRealObjectEndPoint>();
      realObjectEndPoint2.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order2);

      var loadState = new TestableIncompleteVirtualEndPointLoadState (
          new[] { realObjectEndPoint1, realObjectEndPoint2 }, _lazyLoaderMock, _dataKeeperFactoryStub);

      Assert.That (loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { realObjectEndPoint1, realObjectEndPoint2 }));
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_loadState.IsDataComplete (), Is.False);
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
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));

      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();
      
      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      Assert.That (_loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointMock }));
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInDataKeeper ()
    {
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      endPointMock.Expect (mock => mock.ResetSyncState ());
      endPointMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
      Assert.That (_loadState.OriginalOppositeEndPoints.ToArray (), Is.EqualTo (new[] { endPointMock }));
      
      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);

      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_ThrowsIfNotRegistered ()
    {
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);

      _loadState.UnregisterOriginalOppositeEndPoint (_virtualEndPointMock, endPointMock);
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
        "Cannot synchronize an opposite end-point with a virtual end-point in incomplete state.")]
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
      var result = _loadState.HasChanged ();

      Assert.That (result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      _loadState.Commit ();
    }

    [Test]
    public void Rollback ()
    {
      _loadState.Rollback ();
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var dataKeeperFactory = new SerializableVirtualEndPointDataKeeperFactoryFake();

      var oppositeEndPoint = new SerializableRealObjectEndPointFake (
          null, 
          DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1));
      var state = new TestableIncompleteVirtualEndPointLoadState (new[] { oppositeEndPoint }, lazyLoader, dataKeeperFactory);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
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