// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneOneModificationTest : ObjectEndPointSetModificationBaseTest
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

    protected override ObjectEndPointSetModificationBase CreateModification (ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetOneOneModification (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetOneOneModification> (endPoint, newRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (Client.GetObject (DomainObjectIDs.Client1), definition);
      new ObjectEndPointSetOneOneModification (endPoint, Client.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a 1:n relation - use a ObjectEndPointSetOneManyModification instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var endPoint = (ObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (OrderItem.GetObject (DomainObjectIDs.OrderItem1), definition);
      new ObjectEndPointSetOneOneModification (endPoint, Order.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' is the same as its old value - use a ObjectEndPointSetSameModification "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = new ObjectEndPoint (ClientTransactionMock, GetRelationEndPointID (), OldRelatedObject.ID);
      new ObjectEndPointSetOneOneModification (endPoint, OldRelatedObject);
    }

    [Test]
    public void CreateBidirectionalModification_SetDifferent_BidirectionalOneOne ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      order, orderTicketEndPointDefinition);

      // order.OrderTicket = newOrderTicket;
      var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      var setDifferentModification = new ObjectEndPointSetOneOneModification (bidirectionalEndPoint, newOrderTicket);

      var bidirectionalModification = setDifferentModification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NotifyingBidirectionalRelationModification)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (4));

      // order.OrderTicket = newOrderTicket;
      Assert.That (steps[0], Is.SameAs (setDifferentModification));

      // oldOrderTicket.Order = null;

      var orderOfOldOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              bidirectionalEndPoint.GetOppositeObject (true),
              bidirectionalEndPoint.OppositeEndPointDefinition);

      Assert.That (steps[1], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderOfOldOrderTicketEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (order));
      Assert.That (steps[1].NewRelatedObject, Is.Null);

      // newOrderTicket.Order = order;

      var orderOfNewOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              newOrderTicket,
              bidirectionalEndPoint.OppositeEndPointDefinition);

      Assert.That (steps[2], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderOfNewOrderTicketEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (newOrderTicket.Order));
      Assert.That (steps[2].NewRelatedObject, Is.SameAs (order));

      // oldOrderOfNewOrderTicket.OrderTicket = null

      var orderTicketOfOldOrderOfNewOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              newOrderTicket.Order,
              bidirectionalEndPoint.Definition);

      Assert.That (steps[3], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[3].ModifiedEndPoint, Is.SameAs (orderTicketOfOldOrderOfNewOrderTicketEndPoint));
      Assert.That (steps[3].OldRelatedObject, Is.SameAs (newOrderTicket));
      Assert.That (steps[3].NewRelatedObject, Is.SameAs (null));
    }
  }
}