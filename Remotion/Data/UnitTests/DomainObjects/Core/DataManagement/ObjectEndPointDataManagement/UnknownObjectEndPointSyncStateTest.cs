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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class UnknownObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IRelationEndPointLazyLoader _lazyLoaderMock;
    private UnknownObjectEndPointSyncState _state;
    private IRealObjectEndPoint _endPointMock;
    private MockRepository _mockRepository;
    private IRelationEndPoint _oppositeEndPointStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _endPointMock = _mockRepository.StrictMock<IRealObjectEndPoint> ();
      _oppositeEndPointStub = _mockRepository.Stub<IRelationEndPoint>();
      _lazyLoaderMock = _mockRepository.StrictMock<IRelationEndPointLazyLoader> ();
      _state = new UnknownObjectEndPointSyncState(_lazyLoaderMock);
    }

    [Test]
    public void IsSynchronized ()
    {
      using (_mockRepository.Ordered ())
      {
        _lazyLoaderMock.Expect (mock => mock.LoadOppositeEndPoint (_endPointMock));
        _endPointMock.Expect (mock => mock.IsSynchronized).Return (true);
      }
      _lazyLoaderMock.Replay ();
      _endPointMock.Replay ();

      var result = _state.IsSynchronized (_endPointMock);

      _lazyLoaderMock.VerifyAllExpectations();
      _endPointMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      using (_mockRepository.Ordered ())
      {
        _lazyLoaderMock.Expect (mock => mock.LoadOppositeEndPoint (_endPointMock));
        _endPointMock.Expect (mock => mock.Synchronize(_oppositeEndPointStub));
      }
      _lazyLoaderMock.Replay ();
      _endPointMock.Replay ();

      _state.Synchronize(_endPointMock, _oppositeEndPointStub);

      _lazyLoaderMock.VerifyAllExpectations ();
      _endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        _lazyLoaderMock.Expect (mock => mock.LoadOppositeEndPoint (_endPointMock));
        _endPointMock.Expect (mock => mock.CreateDeleteCommand ()).Return(fakeCommand);
      }
      _lazyLoaderMock.Replay ();
      _endPointMock.Replay ();

      var result = _state.CreateDeleteCommand (_endPointMock, id => Assert.Fail ("should not be called."));

      _lazyLoaderMock.VerifyAllExpectations ();
      _endPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        _lazyLoaderMock.Expect (mock => mock.LoadOppositeEndPoint (_endPointMock));
        _endPointMock.Expect (mock => mock.CreateSetCommand (newRelatedObject)).Return (fakeCommand);
      }
      _lazyLoaderMock.Replay ();
      _endPointMock.Replay ();

      var result = _state.CreateSetCommand(_endPointMock, newRelatedObject, id => Assert.Fail ("should not be called."));

      _lazyLoaderMock.VerifyAllExpectations ();
      _endPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        _lazyLoaderMock.Expect (mock => mock.LoadOppositeEndPoint (_endPointMock));
        _endPointMock.Expect (mock => mock.CreateSetCommand (null)).Return (fakeCommand);
      }
      _lazyLoaderMock.Replay ();
      _endPointMock.Replay ();

      var result = _state.CreateSetCommand (_endPointMock, null, id => Assert.Fail ("should not be called."));

      _lazyLoaderMock.VerifyAllExpectations ();
      _endPointMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var fakeLazyLoader = new SerializableRelationEndPointLazyLoaderFake();
      var state = new UnknownObjectEndPointSyncState (fakeLazyLoader);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.LazyLoader, Is.Not.Null);
    }
  }
}