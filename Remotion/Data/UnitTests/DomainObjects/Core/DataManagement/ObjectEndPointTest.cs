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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    private IObjectEndPointSyncState _syncStateMock;
    private ObjectEndPoint _endPointWithSyncStateMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);

      _syncStateMock = MockRepository.GenerateStrictMock<IObjectEndPointSyncState>();
      _endPointWithSyncStateMock = CreateEndPointWithSyncStateMock (_syncStateMock);
    }

    [Test]
    public void Initialization_SyncState ()
    {
      var state = ObjectEndPointTestHelper.GetSyncState (_endPoint);

      Assert.That (state, Is.TypeOf (typeof (UnsynchronizedObjectEndPointSyncState)));
    }

    [Test]
    public void MarkUnsynchronized_MarkSynchronized ()
    {
      _endPoint.MarkSynchronized ();
      Assert.That (ObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (SynchronizedObjectEndPointSyncState)));
      
      _endPoint.MarkUnsynchronized();
      Assert.That (ObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (UnsynchronizedObjectEndPointSyncState)));
    }

    [Test]
    public void GetOppositeObject ()
    {
      var oppositeObject = _endPoint.GetOppositeObject (true);
      Assert.That (Order.GetObject (_endPoint.OppositeObjectID), Is.SameAs (oppositeObject));
    }


    [Test]
    public void GetOppositeObject_Null ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      var oppositeObject = _endPoint.GetOppositeObject (false);
      Assert.That (oppositeObject, Is.Null);
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, order1.ID);

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (order1));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetOppositeObject_Deleted_NoDeleted ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, order1.ID);

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Invalid_IncludeDeleted ()
    {
      _endPoint.MarkSynchronized ();
      var oppositeObject = Order.NewObject ();

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Invalid));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, oppositeObject.ID);

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetOppositeObject_Invalid_ExcludeDeleted ()
    {
      _endPoint.MarkSynchronized();
      var oppositeObject = Order.NewObject ();

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Invalid));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, oppositeObject.ID);
      
      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      _endPoint.MarkSynchronized ();
      var originalOppositeObject = _endPoint.GetOppositeObject (true);
      _endPoint.CreateSetCommand (Order.NewObject ()).Perform ();

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      _endPoint.MarkSynchronized ();
      var originalOppositeObject = (Order) _endPoint.GetOppositeObject (true);
      _endPoint.CreateSetCommand (Order.NewObject ()).ExpandToAllRelatedObjects ().Perform ();

      originalOppositeObject.Delete ();
      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That (_endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _endPoint.EnsureDataComplete ();
    }
    
    [Test]
    public void SetValueFrom_SetsOppositeObjectID ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfEndPointWasTouched ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      _endPoint.Touch ();
      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourceWasTouched ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      source.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (source.HasBeenTouched, Is.False);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.True);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_FalseIfNothingHappened ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this end point's value from "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket'; the end points "
        + "do not have the same end point definition.\r\nParameter name: source")]
    public void SetValueFrom_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      ObjectEndPoint source = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (otherID, null);

      _endPoint.SetValueFrom (source);
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();
      var relatedObject = DomainObjectMother.CreateFakeObject<Order>();

      Action<ObjectID> oppositeObjectIDSetter = null;

      _syncStateMock
          .Expect (mock => mock.CreateSetCommand (Arg.Is (_endPointWithSyncStateMock), Arg.Is (relatedObject), Arg<Action<ObjectID>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectIDSetter = (Action<ObjectID>) mi.Arguments[2]; });
      _syncStateMock.Replay ();

      var result = _endPointWithSyncStateMock.CreateSetCommand (relatedObject);

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPointWithSyncStateMock.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      oppositeObjectIDSetter (DomainObjectIDs.Order2);
      Assert.That (_endPointWithSyncStateMock.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }


    [Test]
    public void CreateRemoveCommand ()
    {
      _endPoint.MarkSynchronized();
      var order = Order.GetObject (_endPoint.OppositeObjectID);
      var command = (RelationEndPointModificationCommand) _endPoint.CreateRemoveCommand (order);
      Assert.That (command, Is.InstanceOfType (typeof (ObjectEndPointSetOneManyCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (command.OldRelatedObject, Is.SameAs (order));
      Assert.That (command.NewRelatedObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot remove object "
        + "'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' from object end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - it currently holds object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void CreateRemoveCommand_InvalidID ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order4);
      _endPoint.CreateRemoveCommand (order);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      Action<ObjectID> oppositeObjectIDSetter = null;
      _syncStateMock
          .Expect (mock => mock.CreateDeleteCommand (Arg.Is (_endPointWithSyncStateMock), Arg<Action<ObjectID>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectIDSetter = (Action<ObjectID>) mi.Arguments[1]; });
      _syncStateMock.Replay ();

      var result = _endPointWithSyncStateMock.CreateDeleteCommand();

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPointWithSyncStateMock.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      oppositeObjectIDSetter (DomainObjectIDs.Order2);
      Assert.That (_endPointWithSyncStateMock.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void GetOppositeRelationEndPoints_NullEndPoint ()
    {
      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      var oppositeEndPoints = _endPoint.GetOppositeRelationEndPoints (ClientTransactionMock.DataManager).ToArray();

      Assert.That (oppositeEndPoints, Is.Empty);
    }

    [Test]
    public void GetOppositeRelationEndPoints_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);

      Assert.That (endPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);

      var oppositeEndPoints = endPoint.GetOppositeRelationEndPoints (ClientTransactionMock.DataManager).ToArray ();

      Assert.That (oppositeEndPoints, Is.Empty);
    }

    [Test]
    public void GetOppositeRelationEndPoints_NonNullEndPoint ()
    {
      var oppositeEndPoints = _endPoint.GetOppositeRelationEndPoints (ClientTransactionMock.DataManager).ToArray ();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var expected = ClientTransactionMock.DataManager.RelationEndPointMap[expectedID];
      Assert.That (expectedID, Is.Not.Null);

      Assert.That (oppositeEndPoints, Is.EqualTo (new[] { expected }));
    }

    private ObjectEndPoint CreateEndPointWithSyncStateMock (IObjectEndPointSyncState syncStateMock)
    {
      var objectEndPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);
      PrivateInvoke.SetNonPublicField (objectEndPoint, "_syncState", syncStateMock);
      return objectEndPoint;
    }
  }
}
