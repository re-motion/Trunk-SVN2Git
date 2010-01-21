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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneOneCommandTest : ObjectEndPointSetCommandTestBase
  {
    protected override DomainObject OldRelatedObject
    {
      get { return OrderTicket.GetObject (DomainObjectIDs.OrderTicket1); }
    }

    protected override DomainObject NewRelatedObject
    {
      get { return OrderTicket.GetObject (DomainObjectIDs.OrderTicket2); }
    }

    protected override RelationEndPointID GetRelationEndPointID ()
    {
      return new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderTicket");
    }

    protected override ObjectEndPointSetCommand CreateCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetOneOneCommand (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetCommand CreateCommandMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetOneOneCommand> (endPoint, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var client = Client.GetObject (DomainObjectIDs.Client1);
      var id = new RelationEndPointID (client.ID, definition);
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      new ObjectEndPointSetOneOneCommand (endPoint, Client.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a 1:n relation - use a ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (OrderItem.GetObject (DomainObjectIDs.OrderItem1).ID, definition);
      var endPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneOneCommand (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetOneOneCommand (endPoint, OldRelatedObject);
    }

    [Test]
    public void ExtendToAllRelatedObjects_SetDifferent_BidirectionalOneOne ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var orderTicketEndPointID = new RelationEndPointID (order.ID, orderTicketEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderTicketEndPointID);

      // order.OrderTicket = newOrderTicket;
      var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      var setDifferentModification = new ObjectEndPointSetOneOneCommand (bidirectionalEndPoint, newOrderTicket);

      var bidirectionalModification = setDifferentModification.ExtendToAllRelatedObjects ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeDataManagementCommand)));

      var steps = GetAllCommands (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (4));

      // order.OrderTicket = newOrderTicket;
      Assert.That (steps[0], Is.SameAs (setDifferentModification));

      // oldOrderTicket.Order = null;

      var orderOfOldOrderTicketEndPointID = 
          new RelationEndPointID (bidirectionalEndPoint.GetOppositeObject (true).ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderOfOldOrderTicketEndPoint = 
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderOfOldOrderTicketEndPointID);

      Assert.That (steps[1], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderOfOldOrderTicketEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (order));
      Assert.That (steps[1].NewRelatedObject, Is.Null);

      // newOrderTicket.Order = order;

      var orderOfNewOrderTicketEndPointID = new RelationEndPointID (newOrderTicket.ID, bidirectionalEndPoint.Definition.GetOppositeEndPointDefinition());
      var orderOfNewOrderTicketEndPoint = 
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderOfNewOrderTicketEndPointID);

      Assert.That (steps[2], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderOfNewOrderTicketEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (newOrderTicket.Order));
      Assert.That (steps[2].NewRelatedObject, Is.SameAs (order));

      // oldOrderOfNewOrderTicket.OrderTicket = null

      var orderTicketOfOldOrderOfNewOrderTicketEndPointID = new RelationEndPointID (newOrderTicket.Order.ID, bidirectionalEndPoint.Definition);
      var orderTicketOfOldOrderOfNewOrderTicketEndPoint = 
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderTicketOfOldOrderOfNewOrderTicketEndPointID);

      Assert.That (steps[3], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[3].ModifiedEndPoint, Is.SameAs (orderTicketOfOldOrderOfNewOrderTicketEndPoint));
      Assert.That (steps[3].OldRelatedObject, Is.SameAs (newOrderTicket));
      Assert.That (steps[3].NewRelatedObject, Is.SameAs (null));
    }
  }
}
