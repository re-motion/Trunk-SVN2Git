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

      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeperMock, _lazyLoaderMock);
      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
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
    public void MarkDataComplete_SortsData ()
    {
      bool stateSetterCalled = false;
      
      _collectionEndPointMock.Replay ();

      _dataKeeperMock.Expect (mock => mock.SortCurrentAndOriginalData ());
      _dataKeeperMock.Replay ();

      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (_collectionEndPointMock, items, () => { stateSetterCalled = true; });

      _collectionEndPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();

      Assert.That (stateSetterCalled, Is.True);
    }

    [Test]
    public void MarkDataComplete_RegistersOppositeEndPoints ()
    {
      bool stateSetterCalled = false;

      var endPointStub1 = MockRepository.GenerateStub<IObjectEndPoint>();
      var endPointStub2 = MockRepository.GenerateStub<IObjectEndPoint> ();
      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub1)).Return (false);
      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub2)).Return (false);
      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub1);
      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub2);

      _collectionEndPointMock.Replay ();

      using (_dataKeeperMock.GetMockRepository().Ordered())
      {
        _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointStub1));
        _dataKeeperMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointStub2));
        _dataKeeperMock.Expect (mock => mock.SortCurrentAndOriginalData ());  
      }
      _dataKeeperMock.Replay ();

      var items = new DomainObject[] { _relatedObject };
      _loadState.MarkDataComplete (_collectionEndPointMock, items, () => { stateSetterCalled = true; });

      _collectionEndPointMock.VerifyAllExpectations ();
      _dataKeeperMock.VerifyAllExpectations ();

      Assert.That (stateSetterCalled, Is.True);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "The data is already incomplete.")]
    public void MarkDataIncomplete_ThrowsException ()
    {
       _loadState.MarkDataIncomplete (_collectionEndPointMock, () => Assert.Fail ("Must not be called."));
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
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub)).Return (false);
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);

      Assert.That (_loadState.OppositeEndPoints, Is.EqualTo(new[]{endPointStub}));
      _dataKeeperMock.AssertWasNotCalled (mock => mock.RegisterOriginalOppositeEndPoint (endPointStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_Twice ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub)).Return (false);
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);
      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_AlreadyInDataKeeper ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub)).Return (true);
      _dataKeeperMock.Replay();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInLocalList ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();

      _dataKeeperMock.Stub (stub => stub.ContainsOriginalOppositeEndPoint (endPointStub)).Return (false);
      _dataKeeperMock.Replay ();

      _loadState.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);
      Assert.That (_loadState.OppositeEndPoints, List.Contains (endPointStub));

      _loadState.UnregisterOriginalOppositeEndPoint (_collectionEndPointMock, endPointStub);

      _dataKeeperMock.AssertWasNotCalled (mock => mock.UnregisterOriginalOppositeEndPoint (endPointStub));
      Assert.That (_loadState.OppositeEndPoints, List.Not.Contains (endPointStub));
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
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableCollectionEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var state = new IncompleteCollectionEndPointLoadState (dataKeeper, lazyLoader);

      var oppositeEndPoint = new SerializableObjectEndPointFake (_relatedObject);
      state.RegisterOriginalOppositeEndPoint (_collectionEndPointMock, oppositeEndPoint);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
      Assert.That (result.LazyLoader, Is.Not.Null);
      Assert.That (result.OppositeEndPoints.Length, Is.EqualTo (1));
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