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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class IncompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private IVirtualObjectEndPoint _virtualObjectEndPointMock;

    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactoryStub;

    private IncompleteVirtualObjectEndPointLoadState _loadState;

    private OrderTicket _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    private OrderTicket _relatedObject2;
    private IRealObjectEndPoint _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
    
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();
      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>> ();

      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);
      dataKeeperStub.Stub (stub => stub.HasDataChanged()).Return (false);
      _loadState = new IncompleteVirtualObjectEndPointLoadState (dataKeeperStub, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _relatedObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _relatedEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _relatedEndPointStub2.Stub (stub => stub.ObjectID).Return (_relatedObject2.ID);
    }

    [Test]
    public void Initialization_WithOriginalOppositeEndPoint ()
    {
      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoint).Return (_relatedEndPointStub);

      var loadState = new IncompleteVirtualObjectEndPointLoadState (dataKeeperStub, _lazyLoaderMock, _dataKeeperFactoryStub);

      Assert.That (loadState.OriginalOppositeEndPoints, Is.EquivalentTo (new[] { _relatedEndPointStub}));
    }

    [Test]
    public void Initialization_NoOriginalOppositeEndPoint ()
    {
      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);

      var loadState = new IncompleteVirtualObjectEndPointLoadState (dataKeeperStub, _lazyLoaderMock, _dataKeeperFactoryStub);

      Assert.That (loadState.OriginalOppositeEndPoints, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Initialization_WithChangedData ()
    {
      var dataKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      dataKeeperStub.Stub (stub => stub.HasDataChanged ()).Return (true);
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoint).Return (null);

      new IncompleteVirtualObjectEndPointLoadState (dataKeeperStub, _lazyLoaderMock, _dataKeeperFactoryStub);
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _lazyLoaderMock.Expect (mock => mock.LoadLazyVirtualObjectEndPoint (_virtualObjectEndPointMock));
      _lazyLoaderMock.Replay ();
      _virtualObjectEndPointMock.Replay ();

      _loadState.EnsureDataComplete (_virtualObjectEndPointMock);

      _lazyLoaderMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_CreatesNewDataKeeper ()
    {
      bool stateSetterCalled = false;

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newKeeperMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointDataKeeper> ();
      newKeeperMock.Replay ();
      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (
          _virtualObjectEndPointMock,
          null,
          keeper =>
          {
            stateSetterCalled = true;
            Assert.That (keeper, Is.SameAs (newKeeperMock));
          });

      Assert.That (stateSetterCalled, Is.True);
      newKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_EndPointsWithoutItem_IsRegisteredAfterStateSetter ()
    {
      bool stateSetterCalled = false;

      AddOriginalOppositeEndPoint (_loadState, _relatedEndPointStub);
      AddOriginalOppositeEndPoint (_loadState, _relatedEndPointStub2);

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      // ReSharper disable AccessToModifiedClosure
      _virtualObjectEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      _virtualObjectEndPointMock
          .Expect (mock => mock.RegisterOriginalOppositeEndPoint (_relatedEndPointStub2))
          .WhenCalled (mi => Assert.That (stateSetterCalled, Is.True));
      // ReSharper restore AccessToModifiedClosure
      _virtualObjectEndPointMock.Replay ();

      var newKeeperStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataKeeper> ();
      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperStub);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, null, keeper => stateSetterCalled = true);

      _virtualObjectEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete_ItemWithoutEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order> ();

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newKeeperMock = MockRepository.GenerateMock<IVirtualObjectEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalItemWithoutEndPoint (item));
      }
      newKeeperMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, item, keeper => { });

      newKeeperMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }
    
    [Test]
    public void MarkDataComplete_ItemWithEndPoint ()
    {
      var item = DomainObjectMother.CreateFakeObject<Order> ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.ObjectID).Return (item.ID);
      oppositeEndPointMock.Stub (stub => stub.ResetSyncState());
      oppositeEndPointMock.Expect (mock => mock.MarkSynchronized ());
      oppositeEndPointMock.Replay ();

      AddOriginalOppositeEndPoint (_loadState, oppositeEndPointMock);

      _virtualObjectEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _virtualObjectEndPointMock.Replay ();

      var newKeeperMock = MockRepository.GenerateMock<IVirtualObjectEndPointDataKeeper> ();
      using (newKeeperMock.GetMockRepository ().Ordered ())
      {
        newKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (oppositeEndPointMock));
      }
      newKeeperMock.Replay ();

      _dataKeeperFactoryStub.Stub (stub => stub.Create (_endPointID)).Return (newKeeperMock);

      _loadState.MarkDataComplete (_virtualObjectEndPointMock, item, keeper => { });

      newKeeperMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      _virtualObjectEndPointMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (Arg<IRealObjectEndPoint>.Is.Anything));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var mockRepository = new MockRepository();
      Assert.That (_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo (0));

      var endPointMock = mockRepository.StrictMock<IRealObjectEndPoint> ();
      var virtualObjectEndPointMock = mockRepository.StrictMock<IVirtualObjectEndPoint>();

      using (mockRepository.Ordered ())
      {
        endPointMock.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);
        endPointMock.Expect (mock => mock.ResetSyncState ());

        endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);
        virtualObjectEndPointMock
            .Expect (mock => mock.MarkDataComplete (_relatedObject))
            .WhenCalled (mi => Assert.That (_loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo (new[] { endPointMock })));
      }

      mockRepository.ReplayAll ();

      _loadState.RegisterOriginalOppositeEndPoint (virtualObjectEndPointMock, endPointMock);

      mockRepository.VerifyAll();
    }

    [Test]
    public void CreateSetCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject),
          s => s.CreateSetCommand (_relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, null),
          s => s.CreateSetCommand (null),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.CreateDeleteCommand (_virtualObjectEndPointMock),
          s => s.CreateDeleteCommand (),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableVirtualObjectEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var dataKeeperFactory = new SerializableVirtualObjectEndPointDataKeeperFactoryFake ();

      var state = new IncompleteVirtualObjectEndPointLoadState (dataKeeper, lazyLoader, dataKeeperFactory);
      AddOriginalOppositeEndPoint (state, new SerializableRealObjectEndPointFake (null, _relatedObject));

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Null);
      Assert.That (result.OriginalOppositeEndPoints, Is.Not.Empty);
      Assert.That (result.LazyLoader, Is.Not.Null);
      Assert.That (result.DataKeeperFactory, Is.Not.Null);
    }

    private void CheckOperationDelegatesToCompleteState<T> (
        Func<IVirtualObjectEndPointLoadState, T> operation,
        Func<IVirtualObjectEndPoint, T> operationOnEndPoint,
        T fakeResult)
    {
      using (_virtualObjectEndPointMock.GetMockRepository ().Ordered ())
      {
        _virtualObjectEndPointMock.Expect (mock => mock.EnsureDataComplete ());
        _virtualObjectEndPointMock.Expect (mock => operationOnEndPoint (mock)).Return (fakeResult);
      }
      _virtualObjectEndPointMock.Replay ();

      var result = operation (_loadState);

      _virtualObjectEndPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    private void AddOriginalOppositeEndPoint (IncompleteVirtualObjectEndPointLoadState loadState, IRealObjectEndPoint oppositeEndPoint)
    {
      var dictionary = (Dictionary<ObjectID, IRealObjectEndPoint>) PrivateInvoke.GetNonPublicField (loadState, "_originalOppositeEndPoints");
      dictionary.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
    }
  }
}