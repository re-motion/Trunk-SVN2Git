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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class UnknownRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IRelationEndPointProvider _endPointProviderMock;
    private UnknownRealObjectEndPointSyncState _state;

    private RelationEndPointID _endPointID;
    private IRealObjectEndPoint _endPointMock;
    private IVirtualEndPoint _oppositeEndPointMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _endPointProviderMock = _mockRepository.StrictMock<IRelationEndPointProvider>();
      _state = new UnknownRealObjectEndPointSyncState(_endPointProviderMock);

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");
      _endPointMock = _mockRepository.StrictMock<IRealObjectEndPoint> ();
      _endPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _endPointMock.Stub (stub => stub.Definition).Return (_endPointID.Definition);
      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);

      _oppositeEndPointMock = _mockRepository.StrictMock<IVirtualEndPoint>();
    }

    [Test]
    public void IsSynchronized ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectLoadOpposite();
        _endPointMock.Expect (mock => mock.IsSynchronized).Return (true);
      }
      _mockRepository.ReplayAll ();

      var result = _state.IsSynchronized (_endPointMock);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      using (_mockRepository.Ordered ())
      {
        ExpectLoadOpposite ();
        _endPointMock.Expect (mock => mock.Synchronize ());
      }

      _mockRepository.ReplayAll();

      _state.Synchronize(_endPointMock, _oppositeEndPointMock);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        ExpectLoadOpposite ();
        _endPointMock.Expect (mock => mock.CreateDeleteCommand ()).Return(fakeCommand);
      }
      _mockRepository.ReplayAll ();

      var result = _state.CreateDeleteCommand (_endPointMock, () => Assert.Fail ("should not be called."));

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        ExpectLoadOpposite ();
        _endPointMock.Expect (mock => mock.CreateSetCommand (newRelatedObject)).Return (fakeCommand);
      }
      _mockRepository.ReplayAll ();

      var result = _state.CreateSetCommand(_endPointMock, newRelatedObject, id => Assert.Fail ("should not be called."));

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      using (_mockRepository.Ordered ())
      {
        ExpectLoadOpposite ();
        _endPointMock.Expect (mock => mock.CreateSetCommand (null)).Return (fakeCommand);
      }
      _mockRepository.ReplayAll ();

      var result = _state.CreateSetCommand (_endPointMock, null, id => Assert.Fail ("should not be called."));

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var fakeProvider = new SerializableRelationEndPointProviderFake();
      var state = new UnknownRealObjectEndPointSyncState (fakeProvider);

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
    }

    private void ExpectLoadOpposite ()
    {
      var oppositeID = RelationEndPointID.CreateOpposite (_endPointID.Definition, DomainObjectIDs.Customer1);
      _endPointProviderMock.Expect (mock => mock.GetRelationEndPointWithMinimumLoading (oppositeID)).Return (_oppositeEndPointMock);
      _oppositeEndPointMock.Expect (mock => mock.EnsureDataComplete());
    }
  }
}