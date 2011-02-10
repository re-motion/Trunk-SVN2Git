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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);
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
    public void PerformWithoutBegin ()
    {
      _endPoint.OppositeObjectID = DomainObjectIDs.Order1;
      Assert.IsNotNull (_endPoint.OppositeObjectID);
      _endPoint.CreateRemoveCommand (Order.GetObject (DomainObjectIDs.Order1)).Perform();
      Assert.IsNull (_endPoint.OppositeObjectID);
    }

    [Test]
    public void SetValueFrom_SetsOppositeObjectID ()
    {
      var sourceID = new RelationEndPointID (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfEndPointWasTouched ()
    {
      var sourceID = new RelationEndPointID (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, _endPoint.OppositeObjectID);

      _endPoint.Touch ();
      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourceWasTouched ()
    {
      var sourceID = new RelationEndPointID (DomainObjectIDs.OrderItem2, _endPointID.Definition);
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
      var sourceID = new RelationEndPointID (DomainObjectIDs.OrderItem2, _endPointID.Definition);
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
      var sourceID = new RelationEndPointID (DomainObjectIDs.OrderItem2, _endPointID.Definition);
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
    public void CreateSetCommand_Same ()
    {
      var command = (RelationEndPointModificationCommand) _endPoint.CreateSetCommand (_endPoint.GetOppositeObject (true));
      Assert.That (command.GetType(), Is.EqualTo (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void CreateSetCommand_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var relationEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
      var unidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);
      var newClient = Client.NewObject ();

      var command = (RelationEndPointModificationCommand) unidirectionalEndPoint.CreateSetCommand (newClient);
      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (unidirectionalEndPoint));
      Assert.That (command.NewRelatedObject, Is.SameAs (newClient));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = new RelationEndPointID (order.ID, orderTicketEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      var command = (RelationEndPointModificationCommand) bidirectionalEndPoint.CreateSetCommand (newOrderTicket);
      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (command.NewRelatedObject, Is.SameAs (newOrderTicket));
    }

    [Test]
    public void CreateSetCommand_OneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (orderItem.ID, orderEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);

      var command = (RelationEndPointModificationCommand) bidirectionalEndPoint.CreateSetCommand (newOrder);
      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (command.NewRelatedObject, Is.SameAs (newOrder));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
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
      var command = _endPoint.CreateDeleteCommand ();
      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (((ObjectEndPointDeleteCommand) command).ModifiedEndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void GetOppositeRelationEndPoints_NullEndPoint ()
    {
      _endPoint.OppositeObjectID = null;

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
  }
}
