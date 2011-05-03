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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointRegistrationAgentTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IRelationEndPointProvider _endPointProviderMock;
    private RelationEndPointMap2 _relationEndPoints;

    private RelationEndPointRegistrationAgent _agent;
    private RelationEndPointID _realEndPointID;
    private RelationEndPointID _virtualEndPointID;
    private RelationEndPointID _unidirectionalEndPointID;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _endPointProviderMock = MockRepository.GenerateStrictMock<IRelationEndPointProvider> ();
      _relationEndPoints = new RelationEndPointMap2 (_clientTransaction);

      _realEndPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      _virtualEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      _unidirectionalEndPointID = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");

      _agent = new RelationEndPointRegistrationAgent (_endPointProviderMock, _relationEndPoints, _clientTransaction);
    }

    [Test]
    public void RegisterEndPoint_NonRealEndPoint ()
    {
      var endPointMock = CreateVirtualEndPointMock();
      endPointMock.Replay();

      _endPointProviderMock.Replay();

      _agent.RegisterEndPoint (endPointMock);

      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNull ()
    {
      var endPointMock = CreateRealObjectEndPointMock(null);
      endPointMock.Expect (mock => mock.MarkSynchronized());
      endPointMock.Replay ();

      _endPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations();
      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = CreateUnidirectionalEndPointMock();
      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay ();

      _endPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional ()
    {
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Replay ();
      
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint>();
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay();

      _endPointProviderMock
          .Expect (mock => mock.GetRelationEndPointWithMinimumLoading (_virtualEndPointID))
          .Return (oppositeEndPointMock);
      _endPointProviderMock.Replay();

      _agent.RegisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations();
      oppositeEndPointMock.VerifyAllExpectations();
      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_InRootTx_VirtualEndPointNotYetComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem>();
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Stub (stub => stub.GetDomainObjectReference()).Return (objectReference);
      endPointMock.Replay ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Expect (mock => mock.MarkDataComplete (objectReference));
      oppositeEndPointMock.Replay ();

      _endPointProviderMock
          .Expect (
              mock => mock.GetRelationEndPointWithMinimumLoading (_virtualEndPointID))
          .Return (oppositeEndPointMock);
      _endPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_InRootTx_VirtualEndPointAlreadyComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (objectReference);
      endPointMock.Replay ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      _endPointProviderMock
          .Expect (
              mock => mock.GetRelationEndPointWithMinimumLoading (_virtualEndPointID))
          .Return (oppositeEndPointMock);
      _endPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock);

      oppositeEndPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (objectReference));
      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_InSubTx ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (objectReference);
      endPointMock.Replay ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      var subTransaction = _clientTransaction.CreateSubTransaction();
      var relationEndPoints = new RelationEndPointMap2 (subTransaction);
      var agent = new RelationEndPointRegistrationAgent (_endPointProviderMock, relationEndPoints, subTransaction);

      _endPointProviderMock
          .Expect (mock => mock.GetRelationEndPointWithMinimumLoading (_virtualEndPointID))
          .Return (oppositeEndPointMock);
      _endPointProviderMock.Replay ();

      agent.RegisterEndPoint (endPointMock);

      oppositeEndPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (objectReference));
      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void UnregisterEndPoint_NotUnregisterable ()
    {
      var endPointMock = CreateVirtualEndPointMock();
      endPointMock.Stub (stub => stub.HasChanged).Return (true);
      endPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);

      _endPointProviderMock.Replay ();

      Assert.That (() => _agent.UnregisterEndPoint (endPointMock), Throws.InstanceOf<InvalidOperationException> ()
          .With.Message.EqualTo (
              "Cannot remove end-point "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' "
              + "because it has changed. End-points can only be unregistered when they are unchanged."));

      Assert.That (_relationEndPoints, Has.Member (endPointMock));
    }

    [Test]
    public void UnregisterEndPoint_NonRealEndPoint()
    {
      var endPointMock = CreateVirtualEndPointMock ();
      endPointMock.Stub (stub => stub.HasChanged).Return (false);
      endPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);

      _endPointProviderMock.Replay ();

      _agent.UnregisterEndPoint (endPointMock);

      Assert.That (_relationEndPoints, Has.No.Member (endPointMock));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNull ()
    {
      var endPointMock = CreateRealObjectEndPointMock (null);
      endPointMock.Stub (stub => stub.HasChanged).Return (false);
      endPointMock.Expect (mock => mock.ResetSyncState());
      endPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);

      _endPointProviderMock.Replay ();

      _agent.UnregisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations();
      Assert.That (_relationEndPoints, Has.No.Member (endPointMock));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = CreateUnidirectionalEndPointMock ();
      endPointMock.Stub (stub => stub.HasChanged).Return (false);
      endPointMock.Expect (mock => mock.ResetSyncState ());
      endPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);

      _endPointProviderMock.Replay ();

      _agent.UnregisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.No.Member (endPointMock));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional_CanBeCollectedTrue ()
    {
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Stub (stub => stub.HasChanged).Return (false);
      endPointMock.Replay ();

      var oppositeEndPointMock = CreateVirtualEndPointMock();
      oppositeEndPointMock.Stub (stub => stub.CanBeCollected).Return (true);
      oppositeEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);
      _relationEndPoints.AddEndPoint (oppositeEndPointMock);

      _endPointProviderMock.Replay ();

      _agent.UnregisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.No.Member (endPointMock));
      Assert.That (_relationEndPoints, Has.No.Member (oppositeEndPointMock));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_NotPointingToNull_CanBeCollectedFalse ()
    {
      var endPointMock = CreateRealObjectEndPointMock (_virtualEndPointID.ObjectID);
      endPointMock.Stub (stub => stub.HasChanged).Return (false);
      endPointMock.Replay ();

      var oppositeEndPointMock = CreateVirtualEndPointMock ();
      oppositeEndPointMock.Stub (stub => stub.CanBeCollected).Return (false);
      oppositeEndPointMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      _relationEndPoints.AddEndPoint (endPointMock);
      _relationEndPoints.AddEndPoint (oppositeEndPointMock);

      _endPointProviderMock.Replay ();

      _agent.UnregisterEndPoint (endPointMock);

      endPointMock.VerifyAllExpectations ();
      _endPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_relationEndPoints, Has.No.Member (endPointMock));
      Assert.That (_relationEndPoints, Has.Member (oppositeEndPointMock));
    }

    [Test]
    public void IsUnregisterable_False_Changed ()
    {
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (true);

      var result = _agent.IsUnregisterable (endPointStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsUnregisterable_True_NonReal_Unchanged ()
    {
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.Definition).Return (_virtualEndPointID.Definition);

      var result = _agent.IsUnregisterable (endPointStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsUnregisterable_True_Real_OppositeNull ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (null);
      endPointStub.Stub (stub => stub.Definition).Return (_realEndPointID.Definition);

      var result = _agent.IsUnregisterable (endPointStub);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsUnregisterable_True_Real_OppositeNonNull_AndUnchanged ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (_virtualEndPointID.ObjectID);
      endPointStub.Stub (stub => stub.Definition).Return (_realEndPointID.Definition);

      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint>();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (_virtualEndPointID);
      oppositeEndPointStub.Stub (stub => stub.HasChanged).Return (false);

      _relationEndPoints.AddEndPoint (oppositeEndPointStub);
      _endPointProviderMock.Replay();

      var result = _agent.IsUnregisterable (endPointStub);

      _endPointProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsUnregisterable_False_Real_OppositeNonNull_AndChanged ()
    {
      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (_virtualEndPointID.ObjectID);
      endPointStub.Stub (stub => stub.Definition).Return (_realEndPointID.Definition);

      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (_virtualEndPointID);
      oppositeEndPointStub.Stub (stub => stub.HasChanged).Return (true);

      _relationEndPoints.AddEndPoint (oppositeEndPointStub);
      _endPointProviderMock.Replay ();

      var result = _agent.IsUnregisterable (endPointStub);

      _endPointProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    private IVirtualEndPoint CreateVirtualEndPointMock ()
    {
      var endPointMock = MockRepository.GenerateStub<IVirtualEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (_virtualEndPointID);
      endPointMock.Stub (stub => stub.Definition).Return (_virtualEndPointID.Definition);
      return endPointMock;
    }

    private IRealObjectEndPoint CreateRealObjectEndPointMock (ObjectID oppositeObjectID)
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (_realEndPointID);
      endPointMock.Stub (stub => stub.Definition).Return (_realEndPointID.Definition);
      endPointMock.Stub (stub => stub.OppositeObjectID).Return (oppositeObjectID);
      return endPointMock;
    }

    private IRealObjectEndPoint CreateUnidirectionalEndPointMock ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      endPointMock.Stub (stub => stub.ID).Return (_unidirectionalEndPointID);
      endPointMock.Stub (stub => stub.Definition).Return (_unidirectionalEndPointID.Definition);
      endPointMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Client1);
      return endPointMock;
    }

  }
}