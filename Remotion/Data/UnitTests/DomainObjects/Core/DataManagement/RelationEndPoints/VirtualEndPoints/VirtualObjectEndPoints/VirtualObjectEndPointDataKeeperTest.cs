// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataKeeperTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    
    private VirtualObjectEndPointDataKeeper _dataKeeper;

    private OrderTicket _oppositeObject;
    private IRealObjectEndPoint _oppositeEndPointStub;

    private OrderTicket _oppositeObject2;
    private IRealObjectEndPoint _oppositeEndPointStub2;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _dataKeeper = new VirtualObjectEndPointDataKeeper (_endPointID);

      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _oppositeEndPointStub.Stub (stub => stub.GetDomainObjectReference()).Return (_oppositeObject);

      _oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _oppositeEndPointStub2 = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _oppositeEndPointStub2.Stub (stub => stub.GetDomainObjectReference()).Return (_oppositeObject2);
    }

    [Test]
    public void CurrentOppositeObject_Set ()
    {
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Null);

      _dataKeeper.CurrentOppositeObject = _oppositeObject;

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void ContainsOriginalObjectID_False ()
    {
      Assert.That (_dataKeeper.ContainsOriginalObjectID (_oppositeObject.ID), Is.False);
    }

    [Test]
    public void ContainsOriginalObjectID_True ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.ContainsOriginalObjectID (_oppositeObject.ID), Is.True);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_CurrentValueAlreadySet ()
    {
      _dataKeeper.CurrentOppositeObject = _oppositeObject2;
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);

      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.EqualTo (_oppositeObject));

      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_PreviouslyItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "A different original opposite item has already been registered.")]
    public void RegisterOriginalOppositeEndPoint_PreviouslyOtherItemWithoutEndPoint ()
    {
      var oppositeObject2 = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket2);
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (oppositeObject2);

      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
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

      _dataKeeper.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Null);
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
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Not.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Not.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void RegisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataKeeper.CurrentOppositeObject = _oppositeObject2;
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);

      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
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
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Not.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Not.Null);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Not.Null);

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Null);
    }

    [Test]
    public void UnregisterOriginalItemWithoutEndPoint_CurrentValueAlreadySet ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (_oppositeObject);
      _dataKeeper.CurrentOppositeObject = _oppositeObject2;

      _dataKeeper.UnregisterOriginalItemWithoutEndPoint (_oppositeObject);

      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject2));
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

      _dataKeeper.CurrentOppositeObject = _oppositeObject;

      Assert.That (_dataKeeper.HasDataChanged(), Is.True);

      _dataKeeper.CurrentOppositeObject = null;

      Assert.That (_dataKeeper.HasDataChanged(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);
      _dataKeeper.CurrentOppositeObject = _oppositeObject2;

      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Not.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.Not.SameAs (_oppositeObject2));

      _dataKeeper.Commit();

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.EqualTo (_oppositeObject2));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.EqualTo (_oppositeObject2));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub2));
    }

    [Test]
    public void Commit_ClearsItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket>());
      _dataKeeper.CurrentOppositeObject = _oppositeObject;
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.Null);
    }

    [Test]
    public void Commit_SetsItemWithoutEndPoint ()
    {
      _dataKeeper.RegisterOriginalItemWithoutEndPoint (DomainObjectMother.CreateFakeObject<OrderTicket> ());
      _dataKeeper.CurrentOppositeObject = _oppositeObject;

      _dataKeeper.Commit ();

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.Null);
      Assert.That (_dataKeeper.OriginalItemWithoutEndPoint, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void Rollback ()
    {
      _dataKeeper.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.EqualTo (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.EqualTo (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));

      _dataKeeper.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub2);
      _dataKeeper.CurrentOppositeObject = _oppositeObject2;
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Not.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Not.SameAs (_oppositeObject));

      _dataKeeper.Rollback();

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.OriginalOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
      Assert.That (_dataKeeper.OriginalOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      var sourceOppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      sourceOppositeEndPointStub.Stub (stub => stub.ID).Return (_oppositeEndPointStub.ID);

      var sourceDataKeeper = new VirtualObjectEndPointDataKeeper (_endPointID);
      sourceDataKeeper.CurrentOppositeObject = _oppositeObject;
      sourceDataKeeper.RegisterCurrentOppositeEndPoint (sourceOppositeEndPointStub);

      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (sourceOppositeEndPointStub.ID))
          .Return (_oppositeEndPointStub);

      _dataKeeper.SetDataFromSubTransaction (sourceDataKeeper, endPointProviderStub);

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.SameAs (_oppositeObject));
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.SameAs (_oppositeEndPointStub));
    }

    [Test]
    public void SetDataFromSubTransaction_Null ()
    {
      _dataKeeper.CurrentOppositeObject = _oppositeObject;
      _dataKeeper.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      var sourceDataKeeper = new VirtualObjectEndPointDataKeeper (_endPointID);
      Assert.That (sourceDataKeeper.CurrentOppositeObject, Is.Null);
      Assert.That (sourceDataKeeper.CurrentOppositeEndPoint, Is.Null);
      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();

      _dataKeeper.SetDataFromSubTransaction (sourceDataKeeper, endPointProviderStub);

      Assert.That (_dataKeeper.CurrentOppositeObject, Is.Null);
      Assert.That (_dataKeeper.CurrentOppositeEndPoint, Is.Null);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var data = new VirtualObjectEndPointDataKeeper (_endPointID);

      var endPointFake = new SerializableRealObjectEndPointFake (null, DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1));
      data.RegisterOriginalOppositeEndPoint (endPointFake);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (data);

      Assert.That (deserializedInstance.EndPointID, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.OriginalOppositeObject, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.CurrentOppositeObject, Is.Not.Null);
    }
  }
}