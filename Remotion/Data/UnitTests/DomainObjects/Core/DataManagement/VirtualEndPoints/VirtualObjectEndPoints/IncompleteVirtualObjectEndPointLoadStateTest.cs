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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class IncompleteVirtualObjectEndPointLoadStateTest : StandardMappingTest
  {
    private IVirtualObjectEndPoint _virtualObjectEndPointMock;
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private IVirtualObjectEndPointDataKeeper _dataKeeperMock;
    private IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactoryStub;

    private IncompleteVirtualObjectEndPointLoadState _loadState;

    private OrderTicket _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _virtualObjectEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      _lazyLoaderMock = MockRepository.GenerateStrictMock<IRelationEndPointLazyLoader> ();

      _dataKeeperMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointDataKeeper> ();

      _dataKeeperFactoryStub = MockRepository.GenerateStub<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>> ();

      _loadState = new IncompleteVirtualObjectEndPointLoadState (_dataKeeperMock, _lazyLoaderMock, _dataKeeperFactoryStub);

      _relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void CreateSetCommand ()
    {
      Action<ObjectID> fakeSetter = collection => { };
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, _relatedObject, fakeSetter),
          s => s.CreateSetCommand (_relatedObject),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      Action<ObjectID> fakeSetter = collection => { };
      CheckOperationDelegatesToCompleteState (
          s => s.CreateSetCommand (_virtualObjectEndPointMock, null, fakeSetter),
          s => s.CreateSetCommand (null),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      Action<ObjectID> fakeSetter = collection => { };
      CheckOperationDelegatesToCompleteState (
          s => s.CreateDeleteCommand (_virtualObjectEndPointMock, fakeSetter),
          s => s.CreateDeleteCommand (),
          MockRepository.GenerateStub<IDataManagementCommand> ());
    }

    [Test]
    public void GetOriginalOppositeEndPoints ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (_relatedEndPointStub);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.EqualTo (new[] { _relatedEndPointStub }));
    }

    [Test]
    public void GetOriginalOppositeEndPoints_None ()
    {
      _dataKeeperMock.Stub (mock => mock.OriginalOppositeEndPoint).Return (null);
      _dataKeeperMock.Replay ();

      var result = (IEnumerable<IRealObjectEndPoint>) PrivateInvoke.InvokeNonPublicMethod (_loadState, "GetOriginalOppositeEndPoints");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var dataKeeper = new SerializableVirtualObjectEndPointDataKeeperFake ();
      var lazyLoader = new SerializableRelationEndPointLazyLoaderFake ();
      var dataKeeperFactory = new SerializableVirtualObjectEndPointDataKeeperFactoryFake ();

      var state = new IncompleteVirtualObjectEndPointLoadState (dataKeeper, lazyLoader, dataKeeperFactory);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.DataKeeper, Is.Not.Null);
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
  }
}