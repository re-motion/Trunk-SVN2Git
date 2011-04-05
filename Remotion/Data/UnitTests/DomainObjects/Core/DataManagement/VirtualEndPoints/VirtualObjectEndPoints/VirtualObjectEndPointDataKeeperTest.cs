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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataKeeperTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    
    private IVirtualEndPointStateUpdateListener _stateUpdateListenerMock;
    private VirtualObjectEndPointDataKeeper _dataKeeper;

    private OrderTicket _oppositeObject;
    private IRealObjectEndPoint _oppositeEndPointStub;

    public override void SetUp ()
    {
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _stateUpdateListenerMock = MockRepository.GenerateMock<IVirtualEndPointStateUpdateListener>();
      _dataKeeper = new VirtualObjectEndPointDataKeeper (_endPointID, _stateUpdateListenerMock);

      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _oppositeEndPointStub.Stub (stub => stub.ObjectID).Return (_oppositeObject.ID);
    }

    [Test]
    public void CurrentOppositeObjectID_Set ()
    {
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Null);

      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket4;

      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket4));
    }

    [Test]
    public void CurrentOppositeObjectID_Set_RaisesNotification ()
    {
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Null);

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (true));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket4;

      _stateUpdateListenerMock.VerifyAllExpectations();
      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (false));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.CurrentOppositeObjectID = null;

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    public void ContainsOriginalObjectID_False ()
    {
      Assert.That (_dataKeeper.ContainsOriginalObjectID (DomainObjectIDs.OrderTicket1), Is.False);
    }

    [Test]
    public void ContainsOriginalObjectID_True ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.ContainsOriginalObjectID (DomainObjectIDs.OrderTicket1), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.SameAs (_oppositeEndPointStub.ObjectID));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.SameAs (_oppositeEndPointStub.ObjectID));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_CurrentValueAlreadySet ()
    {
      var currentOppositeEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      currentOppositeEndPoint.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.OrderTicket2);
      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket2;
      _dataKeeper.RegisterCurrentOppositeEndPoint (currentOppositeEndPoint);

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (true));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (_oppositeEndPointStub.ObjectID));

      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (currentOppositeEndPoint));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_AlreadyRegistered ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.RegisterOriginalOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Not.Null);

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (false));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Null);

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_NotRegistered ()
    {
      _dataKeeper.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The original opposite end-point has not been registered.")]
    public void UnregisterOriginalOppositeEndPoint_DifferentRegistered ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.UnregisterOriginalOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint ()
    {
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Not.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Not.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      var currentOppositeEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      currentOppositeEndPoint.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.OrderTicket2);
      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket2;
      _dataKeeper.RegisterCurrentOppositeEndPoint (currentOppositeEndPoint);

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (true));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (currentOppositeEndPoint));

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An original opposite item has already been registered.")]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeObjectID ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An original opposite item has already been registered.")]
    public void RegisterOriginalItemWithoutEndPoint_WithOriginalOppositeEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Not.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Not.Null);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Not.Null);

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Null);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket2;

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (true));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      _stateUpdateListenerMock.VerifyAllExpectations();
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot unregister original item, it has not been registered.")]
    public void UnregisterOriginalItemWithoutEndPoint_InvalidID ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot unregister original item, an end-point has been registered for it.")]
    public void UnregisterOriginalItemWithoutEndPoint_EndPointExists ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An opposite end-point has already been registered.")]
    public void RegisterCurrentOppositeEndPoint_AlreadyRegistered ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Not.Null);

      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_NotRegistered ()
    {
      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The opposite end-point has not been registered.")]
    public void UnregisterCurrentOppositeEndPoint_DifferentRegistered ()
    {
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.UnregisterCurrentOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
    }

    [Test]
    public void HasDataChanged ()
    {
      Assert.That (_dataKeeper.HasDataChanged(), Is.False);

      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.Order2;

      Assert.That (_dataKeeper.HasDataChanged(), Is.True);

      _dataKeeper.CurrentOppositeObjectID = null;

      Assert.That (_dataKeeper.HasDataChanged(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      var newOppositeEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.RegisterCurrentOppositeEndPoint (newOppositeEndPoint);
      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket3;

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Not.SameAs (newOppositeEndPoint));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket3));

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (false));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.Commit();

      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket3));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (newOppositeEndPoint));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (newOppositeEndPoint));

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    public void Commit_ClearsItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket>());
      _dataKeeper.CurrentOppositeObjectID = _oppositeObject.ID;
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);

      _stateUpdateListenerMock.VerifyAllExpectations ();
    }

    [Test]
    [Ignore ("TODO 3825")]
    public void Commit_SetsItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket> ());
      _dataKeeper.CurrentOppositeObjectID = _oppositeObject.ID;

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (_oppositeObject.ID));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));

      _stateUpdateListenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Rollback ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.RegisterCurrentOppositeEndPoint (MockRepository.GenerateStub<IRealObjectEndPoint>());
      _dataKeeper.CurrentOppositeObjectID = DomainObjectIDs.OrderTicket3;
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Not.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.OrderTicket1));

      _stateUpdateListenerMock.BackToRecord();
      _stateUpdateListenerMock.Expect (mock => mock.StateUpdated (false));
      _stateUpdateListenerMock.Replay();

      _dataKeeper.Rollback();

      Assert.That (_dataKeeper.CurrentOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _stateUpdateListenerMock.VerifyAllExpectations();
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var updateListener = new VirtualEndPointStateUpdateListener (ClientTransaction.CreateRootTransaction(), _endPointID);
      var data = new VirtualObjectEndPointDataKeeper (_endPointID, updateListener);

      var endPointFake = new SerializableRealObjectEndPointFake (null, DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1));
      data.RegisterOriginalOppositeEndPoint (endPointFake);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.UpdateListener, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeObjectID, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeObjectID, Is.Not.Null);
    }
  }
}