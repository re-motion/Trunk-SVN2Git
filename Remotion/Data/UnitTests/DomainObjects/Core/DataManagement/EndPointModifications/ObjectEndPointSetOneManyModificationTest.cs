// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
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

    protected override ObjectEndPointSetModificationBase CreateModification (ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return new ObjectEndPointSetOneManyModification (endPoint, newRelatedObject);
    }

    protected override ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      return repository.StrictMock<ObjectEndPointSetOneManyModification> (endPoint, newRelatedObject);
    }

    [Test]
    public void CreateBidirectionalModification_SetDifferent_BidirectionalOneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      orderItem, orderEndPointDefinition);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);
      var setDifferentModification = new ObjectEndPointSetOneManyModification (bidirectionalEndPoint, newOrder);

      var bidirectionalModification = setDifferentModification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NotifyingBidirectionalRelationModification)));

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (3));

      // orderItem.Order = newOrder;
      Assert.That (steps[0], Is.SameAs (setDifferentModification));

      // newOrder.OrderItems.Add (orderItem);

      var orderItemsOfNewOrderEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              newOrder,
              bidirectionalEndPoint.OppositeEndPointDefinition);

      Assert.That (steps[1], Is.InstanceOfType (typeof (CollectionEndPointInsertModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderItemsOfNewOrderEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.Null);
      Assert.That (steps[1].NewRelatedObject, Is.SameAs (orderItem));

      // oldOrder.OrderItems.Remove (orderItem)

      var orderItemsOfOldOrderEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
              orderItem.Order,
              bidirectionalEndPoint.OppositeEndPointDefinition);

      Assert.That (steps[2], Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderItemsOfOldOrderEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (orderItem));
      Assert.That (steps[2].NewRelatedObject, Is.Null);
    }

  }
}