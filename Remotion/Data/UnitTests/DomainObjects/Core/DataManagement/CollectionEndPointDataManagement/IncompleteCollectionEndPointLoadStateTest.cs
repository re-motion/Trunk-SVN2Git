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
    public void GetOppositeRelationEndPoints ()
    {
      var dataManager = MockRepository.GenerateStub<IDataManager>();
      CheckOperationDelegatesToCompleteState (
          s => s.GetOppositeRelationEndPoints (_collectionEndPointMock, dataManager),
          s => s.GetOppositeRelationEndPoints (dataManager),
          new IRelationEndPoint[0]);
    }

    [Test]
    public void RegisterOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.GetDomainObjectReference ()).Return (_relatedObject);

      _dataKeeperMock.Expect (mock => mock.RegisterOriginalObject (_relatedObject));
      _dataKeeperMock.Replay ();

      _loadState.RegisterOppositeEndPoint (_collectionEndPointMock, endPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOppositeEndPoint ()
    {
      var endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      endPointStub.Stub (stub => stub.ObjectID).Return (_relatedObject.ID);

      _dataKeeperMock.Expect (mock => mock.UnregisterOriginalObject (_relatedObject.ID));
      _dataKeeperMock.Replay ();

      _loadState.UnregisterOppositeEndPoint (_collectionEndPointMock, endPointStub);

      _dataKeeperMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetOppositeCollectionCommand ()
    {
      var domainObjectCollection = new DomainObjectCollection ();

      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetOppositeCollectionCommand (_collectionEndPointMock, domainObjectCollection),
          s => s.CreateSetOppositeCollectionCommand (domainObjectCollection),
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