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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    private ObjectID _oppositeObjectID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
      _oppositeObjectID = DomainObjectIDs.Order1;

      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oppositeObjectID);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (_endPointID, _endPoint.ID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OppositeObjectID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidRelationEndPointID ()
    {
      var id = new ObjectID ("Order", Guid.NewGuid ());
      RelationEndPointObjectMother.CreateObjectEndPoint (null, id);
    }

    [Test]
    public void InitializeWithNullObjectID ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, null);

      Assert.IsNull (endPoint.OriginalOppositeObjectID);
      Assert.IsNull (endPoint.OppositeObjectID);
    }

    [Test]
    public void ChangeOppositeObjectID ()
    {
      var newObjectID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newObjectID;

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (newObjectID));
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (_oppositeObjectID));
      Assert.That (((OrderItem)_endPoint.GetDomainObject()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].Value,
          Is.EqualTo (newObjectID));
    }

    [Test]
    public void GetOppositeObject ()
    {
      var oppositeObject = _endPoint.GetOppositeObject (true);
      Assert.That (Order.GetObject (_endPoint.OppositeObjectID), Is.SameAs (oppositeObject));
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var oppositeObject = (Order) _endPoint.GetOppositeObject (true);
      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetOppositeObject_Deleted_NoDeleted ()
    {
      var oppositeObject = (Order) _endPoint.GetOppositeObject (true);
      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Deleted));

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Discarded ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void GetOppositeObject_Discarded_NoDeleted ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      var originalOppositeObject = _endPoint.GetOppositeObject (true);
      _endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      var originalOppositeObject = (Order) _endPoint.GetOppositeObject (true);
      _endPoint.SetOppositeObjectAndNotify (Order.NewObject ());

      originalOppositeObject.Delete ();
      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void GetOppositeObject_Null ()
    {
      _endPoint.OppositeObjectID = null;
      var oppositeObject = _endPoint.GetOppositeObject (false);
      Assert.That (oppositeObject, Is.Null);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.IsFalse (_endPoint.HasChanged);

      _endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());
      Assert.IsTrue (_endPoint.HasChanged);

      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsFalse (_endPoint.HasChanged);
    }

    [Test]
    public void HasChangedWithInitializedWithNull ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, null);

      Assert.IsFalse (endPoint.HasChanged);
    }

    [Test]
    public void HasChangedWithOldNullValue ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, null);
      endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());

      Assert.IsTrue (endPoint.HasChanged);
    }

    [Test]
    public void HasChangedWithNewNullValue ()
    {
      _endPoint.OppositeObjectID = null;

      Assert.IsTrue (_endPoint.HasChanged);
    }

    [Test]
    public void HasChangedWithSameValueSet ()
    {
      Assert.IsFalse (_endPoint.HasChanged);
      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsFalse (_endPoint.HasChanged);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.IsFalse (_endPoint.HasBeenTouched);

      _endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());
      Assert.IsTrue (_endPoint.HasBeenTouched);

      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithInitializedWithNull ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, null);
      Assert.IsFalse (endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithOldNullValue ()
    {
      ObjectEndPoint endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, null);
      endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());

      Assert.IsTrue (endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithNewNullValue ()
    {
      _endPoint.OppositeObjectID = null;

      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithSameValueSet ()
    {
      Assert.IsFalse (_endPoint.HasBeenTouched);
      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithPerformRelationChange ()
    {
      var removedRelatedObject = _endPoint.GetOppositeObject (false);
      Assert.IsFalse (_endPoint.HasBeenTouched);
      _endPoint.CreateRemoveModification (removedRelatedObject).Perform ();
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void Touch_AlsoTouchesForeignKey ()
    {
      Assert.That (_endPoint.IsVirtual, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (((OrderItem)_endPoint.GetDomainObject()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].HasBeenTouched, Is.False);

      _endPoint.Touch();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (((OrderItem) _endPoint.GetDomainObject ()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].HasBeenTouched, Is.True);
    }

    [Test]
    public void Touch_WorksIfNoForeignKey ()
    {
      var virtualEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      var oppositeID = DomainObjectIDs.OrderTicket1;
      var virtualEndPoint = RelationEndPointObjectMother.CreateObjectEndPoint (virtualEndPointID, oppositeID);

      Assert.That (virtualEndPoint.IsVirtual, Is.True);
      Assert.That (virtualEndPoint.HasBeenTouched, Is.False);
      Assert.That (((Order) virtualEndPoint.GetDomainObject ()).InternalDataContainer.PropertyValues.Contains (virtualEndPoint.PropertyName), Is.False);

      virtualEndPoint.Touch();
      Assert.That (virtualEndPoint.HasBeenTouched, Is.True);
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
    public void GetEndPointDefinition ()
    {
      IRelationEndPointDefinition endPointDefinition = _endPoint.Definition;
      Assert.IsNotNull (endPointDefinition);

      Assert.AreSame (
        MappingConfiguration.Current.ClassDefinitions["OrderItem"],
        endPointDefinition.ClassDefinition);

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", endPointDefinition.PropertyName);
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition = _endPoint.OppositeEndPointDefinition;
      Assert.IsNotNull (oppositeEndPointDefinition);

      Assert.AreSame (
        MappingConfiguration.Current.ClassDefinitions["Order"],
        oppositeEndPointDefinition.ClassDefinition);

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetRelationDefinition ()
    {
      RelationDefinition relationDefinition = _endPoint.RelationDefinition;
      Assert.IsNotNull (relationDefinition);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", relationDefinition.ID);
    }

    [Test]
    public void IsVirtual ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      RelationEndPoint orderEndPoint = RelationEndPointObjectMother.CreateObjectEndPoint (orderContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", DomainObjectIDs.OrderTicket1);

      Assert.AreEqual (true, orderEndPoint.IsVirtual);
    }

    [Test]
    public void IsNotVirtual ()
    {
      Assert.AreEqual (false, _endPoint.IsVirtual);
    }

    [Test]
    public void ID ()
    {
      Assert.IsNotNull (_endPoint.ID);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _endPoint.ID.PropertyName);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, _endPoint.ID.ObjectID);
    }

    [Test]
    public void Commit ()
    {
      var newOppositeID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newOppositeID;

      Assert.IsTrue (_endPoint.HasBeenTouched);
      Assert.IsTrue (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);

      _endPoint.Commit ();

      Assert.IsFalse (_endPoint.HasBeenTouched);
      Assert.IsFalse (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (newOppositeID, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void Rollback ()
    {
      var newOppositeID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newOppositeID;

      Assert.IsTrue (_endPoint.HasBeenTouched);
      Assert.IsTrue (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);

      _endPoint.Rollback ();

      Assert.IsFalse (_endPoint.HasBeenTouched);
      Assert.IsFalse (_endPoint.HasChanged);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void SetValueFrom_SetsOppositeObjectID ()
    {
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfEndPointWasTouched ()
    {
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);

      _endPoint.Touch ();
      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourceWasTouched ()
    {
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);

      source.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);

      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (source.HasBeenTouched, Is.False);

      _endPoint.SetValueFrom (source);

      Assert.That (_endPoint.HasChanged, Is.True);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_FalseIfNothingHappened ()
    {
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order1);

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
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (otherID, null);

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
      var unidirectionalEndPoint = (ObjectEndPoint)
                                   ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                       client, parentClientEndPointDefinition);
      Assert.That (unidirectionalEndPoint.OppositeEndPointDefinition.IsAnonymous, Is.True);
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
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      order, orderTicketEndPointDefinition);

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
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      orderItem, orderEndPointDefinition);

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
      var orderFromOtherTransaction = new ClientTransactionMock ().GetObject (DomainObjectIDs.Order1);
      _endPoint.SetOppositeObjectAndNotify (orderFromOtherTransaction);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetOppositeObjectAndNotify_WithInvalidType ()
    {
      DomainObject customer = ClientTransactionMock.GetObject (DomainObjectIDs.Customer1);
      _endPoint.SetOppositeObjectAndNotify (customer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetOppositeObjectAndNotify_WithInvalidBaseType ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Person1, "AssociatedPartnerCompany");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (endPointID, DomainObjectIDs.Partner1);

      var nonPartnerCompany = Company.GetObject (DomainObjectIDs.Company1);
      endPoint.SetOppositeObjectAndNotify (nonPartnerCompany);
    }
  }
}
