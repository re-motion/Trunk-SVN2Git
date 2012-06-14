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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointMapTest : StandardMappingTest
  {
    private ClientTransactionEventSinkWithMock _transactionEventSinkWithMock;
    private RelationEndPointMap _map;

    private RelationEndPointID _endPointID1;
    private RelationEndPointID _endPointID2;

    private IRelationEndPoint _endPointMock1;
    private IRelationEndPoint _endPointMock2;

    public override void SetUp ()
    {
      base.SetUp ();

      _transactionEventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock(ClientTransaction.CreateRootTransaction());
      _map = new RelationEndPointMap (_transactionEventSinkWithMock);

      _endPointID1 = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");
      _endPointID2 = RelationEndPointID.Create (DomainObjectIDs.Order2, typeof (Order), "Customer");

      _endPointMock1 = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _endPointMock1.Stub (stub => stub.ID).Return (_endPointID1);
      _endPointMock2 = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      _endPointMock2.Stub (stub => stub.ID).Return (_endPointID2);
    }

    [Test]
    public void Item ()
    {
      StubEvents();
      Assert.That (_map[_endPointID1], Is.Null);

      _map.AddEndPoint (_endPointMock1);

      Assert.That (_map[_endPointID1], Is.SameAs (_endPointMock1));
    }

    [Test]
    public void Count ()
    {
      StubEvents ();
      Assert.That (_map.Count, Is.EqualTo (0));

      _map.AddEndPoint (_endPointMock1);

      Assert.That (_map.Count, Is.EqualTo (1));

      _map.AddEndPoint (_endPointMock2);

      Assert.That (_map.Count, Is.EqualTo (2));
    }

    [Test]
    public void GetEnumerator ()
    {
      StubEvents ();

      _map.AddEndPoint (_endPointMock1);
      _map.AddEndPoint (_endPointMock2);

      var items = new List<IRelationEndPoint>();

      using (var enumerator = _map.GetEnumerator ())
      {
        Assert.That (enumerator.MoveNext (), Is.True);
        items.Add (enumerator.Current);
        Assert.That (enumerator.MoveNext (), Is.True);
        items.Add (enumerator.Current);
        Assert.That (enumerator.MoveNext (), Is.False);
      }

      Assert.That (items, Is.EquivalentTo (new[] { _endPointMock1, _endPointMock2 }));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      StubEvents ();

      _map.AddEndPoint (_endPointMock1);
      _map.AddEndPoint (_endPointMock2);

      var items = new List<IRelationEndPoint> ();

      var enumerator = ((IEnumerable) _map).GetEnumerator();
      Assert.That (enumerator.MoveNext (), Is.True);
      items.Add ((IRelationEndPoint) enumerator.Current);
      Assert.That (enumerator.MoveNext (), Is.True);
      items.Add ((IRelationEndPoint) enumerator.Current);
      Assert.That (enumerator.MoveNext (), Is.False);

      Assert.That (items, Is.EquivalentTo (new[] { _endPointMock1, _endPointMock2 }));
    }

    [Test]
    public void CommitAllEndPoints ()
    {
      StubEvents ();
      
      _map.AddEndPoint (_endPointMock1);
      _map.AddEndPoint (_endPointMock2);

      _endPointMock1.Expect (mock => mock.Commit ());
      _endPointMock2.Expect (mock => mock.Commit ());
      _endPointMock1.Replay ();
      _endPointMock2.Replay ();

      _map.CommitAllEndPoints();

      _endPointMock1.VerifyAllExpectations();
      _endPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void RollbackAllEndPoints ()
    {
      StubEvents ();

      _map.AddEndPoint (_endPointMock1);
      _map.AddEndPoint (_endPointMock2);

      _endPointMock1.Expect (mock => mock.Rollback ());
      _endPointMock2.Expect (mock => mock.Rollback ());
      _endPointMock1.Replay ();
      _endPointMock2.Replay ();

      _map.RollbackAllEndPoints ();

      _endPointMock1.VerifyAllExpectations ();
      _endPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void AddEndPoint ()
    {
      _transactionEventSinkWithMock.ExpectMock (l => l.RelationEndPointMapRegistering (_transactionEventSinkWithMock.ClientTransaction, _endPointMock1));
      _transactionEventSinkWithMock.ReplayMock ();

      _map.AddEndPoint (_endPointMock1);

      _transactionEventSinkWithMock.VerifyMock ();
      Assert.That (_map[_endPointID1], Is.SameAs (_endPointMock1));
    }

    [Test]
    public void AddEndPoint_KeyAlreadyExists ()
    {
      // Note: We'll get an event even when an exception is thrown. This is more an optimization than a feature.
      _transactionEventSinkWithMock
          .StubMock (l => l.RelationEndPointMapRegistering (Arg<ClientTransaction>.Is.Anything, Arg<IRelationEndPoint>.Is.Anything))
          .Repeat.Twice();

      _map.AddEndPoint (_endPointMock1);

      var secondEndPointStub = MockRepository.GenerateStub<IRelationEndPoint>();
      secondEndPointStub.Stub (stub => stub.ID).Return (_endPointID1);

      Assert.That (() => _map.AddEndPoint (secondEndPointStub), Throws.InvalidOperationException.With.Message.EqualTo (
          "A relation end-point with ID "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' "
          + "has already been registered."));

      _transactionEventSinkWithMock.VerifyMock ();
    }

    [Test]
    public void RemoveEndPoint ()
    {
      StubEvents ();

      _map.AddEndPoint (_endPointMock1);
      Assert.That (_map[_endPointID1], Is.Not.Null);

      _transactionEventSinkWithMock.BackToRecordMock();
      _transactionEventSinkWithMock.ExpectMock (l => l.RelationEndPointMapUnregistering (_transactionEventSinkWithMock.ClientTransaction, _endPointID1));
      _transactionEventSinkWithMock.ReplayMock ();

      _map.RemoveEndPoint (_endPointID1);

      _transactionEventSinkWithMock.VerifyMock ();
      Assert.That (_map[_endPointID1], Is.Null);
    }

    [Test]
    public void RemoveEndPoint_KeyNotExists ()
    {
      // Note: We'll get an event even when an exception is thrown. This is more an optimization than a feature.
      _transactionEventSinkWithMock
          .StubMock (l => l.RelationEndPointMapUnregistering (_transactionEventSinkWithMock.ClientTransaction, _endPointID1));

      Assert.That (() => _map.RemoveEndPoint (_endPointID1), Throws.ArgumentException.With.Message.EqualTo (
          "End point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer' is not "
          + "part of this map.\r\nParameter name: endPointID"));
    }

    [Test]
    public void FlattenedSerialization ()
    {
      var map = new RelationEndPointMap (new SerializableClientTransactionEventSinkFake());

      var serializableEndPoint = new SerializableRealObjectEndPointFake (_endPointID1, null);
      map.AddEndPoint (serializableEndPoint);

      var deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);

      Assert.That (deserializedMap[_endPointID1], Is.Not.Null);
      Assert.That (deserializedMap.TransactionEventSink, Is.Not.Null);
    }

    private void StubEvents ()
    {
      _transactionEventSinkWithMock
          .StubMock (l => l.RelationEndPointMapRegistering (Arg<ClientTransaction>.Is.Anything, Arg<IRelationEndPoint>.Is.Anything)).Repeat.Any();
      _transactionEventSinkWithMock
          .StubMock (l => l.RelationEndPointMapUnregistering (Arg<ClientTransaction>.Is.Anything, Arg<RelationEndPointID>.Is.Anything)).Repeat.Any();
    }
  }
}