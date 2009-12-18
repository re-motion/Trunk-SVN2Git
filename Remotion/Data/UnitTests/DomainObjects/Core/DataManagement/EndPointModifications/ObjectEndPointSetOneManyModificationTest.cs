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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneManyModificationTest : ObjectEndPointSetModificationBaseTest
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
      return new RelationEndPointID (DomainObjectIDs.OrderItem1, typeof (OrderItem).FullName + ".Order");
    }

    protected override ObjectEndPointSetModificationBase CreateModification (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetOneManyModification (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetOneManyModification> (endPoint, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var relationEndPointID = new RelationEndPointID (Client.GetObject(DomainObjectIDs.Client1).ID, definition);
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyModification (endPoint, Client.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order' "
        + "is from a 1:1 relation - use a ObjectEndPointSetOneOneModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).ID, definition);
      var endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneManyModification (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' is the same as its old value - use a ObjectEndPointSetSameModification "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetOneManyModification (endPoint, OldRelatedObject);
    }

    [Test]
    public void CreateBidirectionalModification_SetDifferent_BidirectionalOneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (orderItem.ID, orderEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);
      var setDifferentModification = new ObjectEndPointSetOneManyModification (bidirectionalEndPoint, newOrder);

      var bidirectionalModification = setDifferentModification.CreateRelationModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeRelationModificationWithEvents)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (3));

      // orderItem.Order = newOrder;
      Assert.That (steps[0], Is.SameAs (setDifferentModification));

      // newOrder.OrderItems.Add (orderItem);

      var orderItemsOfNewOrderEndPointID = new RelationEndPointID (newOrder.ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderItemsOfNewOrderEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderItemsOfNewOrderEndPointID);

      Assert.That (steps[1], Is.InstanceOfType (typeof (CollectionEndPointInsertModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderItemsOfNewOrderEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.Null);
      Assert.That (steps[1].NewRelatedObject, Is.SameAs (orderItem));

      // oldOrder.OrderItems.Remove (orderItem)

      var orderItemsOfOldOrderEndPointID = new RelationEndPointID (orderItem.Order.ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderItemsOfOldOrderEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderItemsOfOldOrderEndPointID);

      Assert.That (steps[2], Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderItemsOfOldOrderEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (orderItem));
      Assert.That (steps[2].NewRelatedObject, Is.Null);
    }

  }
}
