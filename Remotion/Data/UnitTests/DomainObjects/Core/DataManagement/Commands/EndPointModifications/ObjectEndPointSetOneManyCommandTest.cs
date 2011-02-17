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
// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneManyCommandTest : ObjectEndPointSetCommandTestBase
  {
    protected override DomainObject OldRelatedObject
    {
      get { return Order.GetObject (DomainObjectIDs.Order1); }
    }

    protected override DomainObject NewRelatedObject
    {
      get { return Order.GetObject (DomainObjectIDs.Order2); }
    }

    protected override RelationEndPointID GetRelationEndPointID ()
    {
      return RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof (OrderItem).FullName + ".Order");
    }

    protected override ObjectEndPointSetCommand CreateCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
    {
      return new ObjectEndPointSetOneManyCommand (endPoint, newRelatedObject, oppositeObjectIDSetter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var relationEndPointID = RelationEndPointID.Create(Client.GetObject(DomainObjectIDs.Client1).ID, definition);
      var endPoint = (IObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyCommand (endPoint, Client.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a 1:1 relation - use a ObjectEndPointSetOneOneCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create(OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).ID, definition);
      var endPoint = (IObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyCommand (endPoint, Order.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetOneManyCommand (endPoint, OldRelatedObject, mi => { });
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetDifferent_BidirectionalOneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create(orderItem.ID, orderEndPointDefinition);
      var bidirectionalEndPoint = 
          (IObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);
      var setDifferentModification = new ObjectEndPointSetOneManyCommand (bidirectionalEndPoint, newOrder, mi => { });

      var bidirectionalModification = setDifferentModification.ExpandToAllRelatedObjects ();

      var steps = GetAllCommands (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (3));

      // orderItem.Order = newOrder;
      Assert.That (steps[0], Is.SameAs (setDifferentModification));

      // newOrder.OrderItems.Add (orderItem);

      var orderItemsOfNewOrderEndPointID = RelationEndPointID.Create(newOrder.ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderItemsOfNewOrderEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderItemsOfNewOrderEndPointID);

      Assert.That (steps[1], Is.InstanceOfType (typeof (CollectionEndPointInsertCommand)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderItemsOfNewOrderEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.Null);
      Assert.That (steps[1].NewRelatedObject, Is.SameAs (orderItem));

      // oldOrder.OrderItems.Remove (orderItem)

      var orderItemsOfOldOrderEndPointID = RelationEndPointID.Create(orderItem.Order.ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderItemsOfOldOrderEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderItemsOfOldOrderEndPointID);

      Assert.That (steps[2], Is.InstanceOfType (typeof (CollectionEndPointRemoveCommand)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderItemsOfOldOrderEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (orderItem));
      Assert.That (steps[2].NewRelatedObject, Is.Null);
    }

  }
}
