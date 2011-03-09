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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class IncompleteCollectionEndPointLoadStateTest : StandardMappingTest
  {
    private ICollectionEndPoint _collectionEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private ICollectionEndPointDataKeeper _dataKeeperMock;

    private IncompleteCollectionEndPointLoadState _loadState;
    private Order _relatedObject;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();
      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new IObjectEndPoint[0]);

      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeperMock, _lazyLoaderMock);
      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
    }

    [Test]
    public void Initialization_WithOriginalEndPoints ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint>();
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();

      var dataKeeperStub = MockRepository.GenerateStub<ICollectionEndPointDataKeeper>();
      dataKeeperStub.Stub (stub => stub.OriginalOppositeEndPoints).Return (new[] { endPointMock });

      new IncompleteCollectionEndPointLoadState (dataKeeperStub, _lazyLoaderMock);

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
    public void MarkDataComplete_SortsDataAndSetsState ()
    {
      bool stateSetterCalled = false;

      _collectionEndPointMock.Replay ();

      _dataKeeperMock.Expect (mock => mock.SortCurrentAndOriginalData ());
      _dataKeeperMock.Replay ();

      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (
          _collectionEndPointMock,
          items,
          keeper =>
          {
            stateSetterCalled = true;
            Assert.That (keeper, Is.SameAs (_dataKeeperMock));
          });

      _collectionEndPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();

      Assert.That (stateSetterCalled, Is.True);
    }

    [Test]
    public void MarkDataComplete_WithEndPointsInDataKeeper ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay ();

      _dataKeeperMock.BackToRecord();
      _dataKeeperMock.Stub (stub => stub.OriginalOppositeEndPoints).Return (new[] { endPointMock });

      _dataKeeperMock.Expect (mock => mock.SortCurrentAndOriginalData ());
      _dataKeeperMock.Replay ();

      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (_collectionEndPointMock, items, keeper => { });

      _dataKeeperMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "The data is already incomplete.")]
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
          new DomainObjectCollectionData ());
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      CheckOperationDelegatesToCompleteState (
          s => s.GetCollectionWithOriginalData (_collectionEndPointMock), 
          s => s.GetCollectionWithOriginalData(), 
          new DomainObjectCollection ());
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      CheckOperationDelegatesToCompleteState(
          s => s.GetOppositeRelationEndPointIDs (_collectionEndPointMock),
          s => s.GetOppositeRelationEndPointIDs (),
          new RelationEndPointID[0]);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint> ();
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointMock)).Return (false);
      _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointMock);

      endPointMock.VerifyAllExpectations();
      _dataKeeperMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInDataKeeper ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _dataKeeperMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (endPointStub));
      _dataKeeperMock.Replay ();

      _loadState.UnregisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetUnsynchronizedOppositeEndPoints ()
    {
      Assert.That (_loadState.GetUnsynchronizedOppositeEndPoints(), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot synchronize opposite end-point "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - the "
        + "end-point is not in the list of unsynchronized end-points.")]
    public void SynchronizeOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderItem1, "Order"));

      _loadState.SynchronizeOppositeEndPoint (endPointStub);
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
    public void CheckMandatory ()
    {
      CheckOperationDelegatesToCompleteState (s => s.CheckMandatory (_collectionEndPointMock), s => s.CheckMandatory ());
    }

    [Test]
    public void HasChanged ()
    {
      var changeDetectionStrategy = MockRepository.GenerateStub<ICollectionEndPointChangeDetectionStrategy> ();
      _dataKeeperMock.Expect (mock => mock.HasDataChanged (changeDetectionStrategy)).Return (true);
      _dataKeeperMock.Replay ();

      var result = _loadState.HasChanged (changeDetectionStrategy);

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
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableCollectionEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var state = new IncompleteCollectionEndPointLoadState (dataKeeper, lazyLoader);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.LazyLoader, Is.Not.Null);
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
      Assert.That (result, Is.SameAs (fakeResult));
    }
  }
}