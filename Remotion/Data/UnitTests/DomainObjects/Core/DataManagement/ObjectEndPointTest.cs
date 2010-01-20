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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

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
    public void IsDataAvailable_True ()
    {
      Assert.That (_endPoint.IsDataAvailable, Is.True);
    }

    [Test]
    public void EnsureDataAvailable_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _endPoint.EnsureDataAvailable ();
    }
    
    [Test]
    public void PerformWithoutBegin ()
    {
      _endPoint.OppositeObjectID = DomainObjectIDs.Order1;
      Assert.IsNotNull (_endPoint.OppositeObjectID);
      _endPoint.CreateRemoveModification (Order.GetObject (DomainObjectIDs.Order1)).Perform();
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
    public void CreateSetModification_Same ()
    {
      var modification = _endPoint.CreateSetModification (_endPoint.GetOppositeObject (true));
      Assert.That (modification.GetType(), Is.EqualTo (typeof (ObjectEndPointSetSameModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void CreateSetModification_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var relationEndPointID = new RelationEndPointID (client.ID, parentClientEndPointDefinition);
      var unidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);
      var newClient = Client.NewObject ();

      var modification = unidirectionalEndPoint.CreateSetModification (newClient);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (unidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newClient));
    }

    [Test]
    public void CreateSetModification_OneOne ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var relationEndPointID = new RelationEndPointID (order.ID, orderTicketEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      var modification = bidirectionalEndPoint.CreateSetModification (newOrderTicket);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newOrderTicket));
    }

    [Test]
    public void CreateSetModification_OneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = new RelationEndPointID (orderItem.ID, orderEndPointDefinition);
      var bidirectionalEndPoint = 
          (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);

      var modification = bidirectionalEndPoint.CreateSetModification (newOrder);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newOrder));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      var order = Order.GetObject (_endPoint.OppositeObjectID);
      var modification = _endPoint.CreateRemoveModification (order);
      Assert.That (modification, Is.InstanceOfType (typeof (ObjectEndPointSetOneManyModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (order));
      Assert.That (modification.NewRelatedObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot remove object "
        + "'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' from object end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - it currently holds object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void CreateRemoveModification_InvalidID ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order4);
      _endPoint.CreateRemoveModification (order);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        "Property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' of DomainObject "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be set to DomainObject "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. The objects do not belong to the same ClientTransaction.")]
    public void SetOppositeObjectAndNotify_WithOtherClientTransaction ()
    {
      var orderFromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      _endPoint.SetOppositeObjectAndNotify (orderFromOtherTransaction);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetOppositeObjectAndNotify_WithInvalidType ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _endPoint.SetOppositeObjectAndNotify (customer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetOppositeObjectAndNotify_WithInvalidBaseType ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Person1, "AssociatedPartnerCompany");
      var endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (endPointID, DomainObjectIDs.Partner1);

      var nonPartnerCompany = Company.GetObject (DomainObjectIDs.Company1);
      endPoint.SetOppositeObjectAndNotify (nonPartnerCompany);
    }
  }
}
