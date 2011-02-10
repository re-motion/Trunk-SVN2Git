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
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class SynchronizedObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IObjectEndPoint _endPointMock;
    private SynchronizedObjectEndPointSyncState _state;

    private Order _order;
    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _endPointMock = MockRepository.GenerateStrictMock<IObjectEndPoint>();
      _state = new SynchronizedObjectEndPointSyncState (_endPointMock);

      _order = DomainObjectMother.CreateFakeObject<Order>();
      _orderOrderTicketEndPointDefinition = Configuration.ClassDefinitions.GetMandatory (typeof (OrderTicket)).GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oppositeObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (oppositeObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (oppositeObject);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (oppositeObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (oppositeObject));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (null);
      _endPointMock.Stub (stub => stub.GetOppositeObject (Arg<bool>.Is.Anything)).Return (null);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (null);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
    }

    //[Test]
    //public void CreateSetCommand_Unidirectional ()
    //{
    //  var client = Client.GetObject (DomainObjectIDs.Client2);
    //  var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
    //  var relationEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
    //  var unidirectionalEndPoint =
    //      (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
    //  Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);
    //  var newClient = Client.NewObject ();

    //  var command = (RelationEndPointModificationCommand) unidirectionalEndPoint.CreateSetCommand (newClient);
    //  Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalCommand)));
    //  Assert.That (command.ModifiedEndPoint, Is.SameAs (unidirectionalEndPoint));
    //  Assert.That (command.NewRelatedObject, Is.SameAs (newClient));
    //}

    //[Test]
    //public void CreateSetCommand_OneOne ()
    //{
    //  var order = Order.GetObject (DomainObjectIDs.Order1);
    //  var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
    //  var relationEndPointID = new RelationEndPointID (order.ID, orderTicketEndPointDefinition);
    //  var bidirectionalEndPoint =
    //      (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

    //  var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

    //  var command = (RelationEndPointModificationCommand) bidirectionalEndPoint.CreateSetCommand (newOrderTicket);
    //  Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneCommand)));
    //  Assert.That (command.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
    //  Assert.That (command.NewRelatedObject, Is.SameAs (newOrderTicket));
    //}

    //[Test]
    //public void CreateSetCommand_OneMany ()
    //{
    //  var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
    //  var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
    //  var relationEndPointID = new RelationEndPointID (orderItem.ID, orderEndPointDefinition);
    //  var bidirectionalEndPoint =
    //      (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

    //  // orderItem.Order = newOrder;
    //  var newOrder = Order.GetObject (DomainObjectIDs.Order2);

    //  var command = (RelationEndPointModificationCommand) bidirectionalEndPoint.CreateSetCommand (newOrder);
    //  Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyCommand)));
    //  Assert.That (command.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
    //  Assert.That (command.NewRelatedObject, Is.SameAs (newOrder));
    //}
  }
}