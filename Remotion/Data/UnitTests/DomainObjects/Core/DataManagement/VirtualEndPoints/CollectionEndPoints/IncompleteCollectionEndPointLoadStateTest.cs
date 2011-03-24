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
    private ICollectionEndPoint _collectionEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private ICollectionEndPointDataKeeper _dataKeeperMock;
    private IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper> _dataKeeperFactoryStub;

    private IncompleteCollectionEndPointLoadState _loadState;

    private Order _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _collectionEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _dataKeeperMock = MockRepository.GenerateStrictMock<ICollectionEndPointDataKeeper> ();

      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<ICollectionEndPointDataKeeper>> ();

      _loadState = new IncompleteCollectionEndPointLoadState (_dataKeeperMock, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
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