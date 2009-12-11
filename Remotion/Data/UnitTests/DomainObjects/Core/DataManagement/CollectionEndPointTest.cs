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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private CollectionEndPoint _customerEndPoint;
    private Order _order1; // Customer1
    private Order _orderWithoutOrderItem; // Customer1
    private Order _order2; // Customer3

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _customerEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_customerEndPoint.ID, Is.EqualTo (_customerEndPointID));

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsReference, Is.SameAs (_customerEndPoint.OppositeDomainObjects));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.Not.SameAs (_customerEndPoint.OppositeDomainObjects));
    }

    [Test]
    public void Initialize_UsesEndPointDelegatingData ()
    {
      var dataDecorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (
          _customerEndPoint.OppositeDomainObjects);
      Assert.That (dataDecorator.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      
      DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (dataDecorator);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_WithInvalidRelationEndPointID_Throws ()
    {
      RelationEndPointObjectMother.CreateCollectionEndPoint (null, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_WithNullInitialContents_Throws ()
    {
      RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void Initialize_WithoutCollectionCtorTakingData_Throws ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainObjectWithCollectionMissingCtor));

      var endPointID = new RelationEndPointID (
          new ObjectID (classDefinition, Guid.NewGuid ()), 
          typeof (DomainObjectWithCollectionMissingCtor) + ".OppositeObjects");
      RelationEndPointObjectMother.CreateCollectionEndPoint (endPointID, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ChangeOriginalOppositeDomainObjects ()
    {
      _customerEndPoint.OriginalOppositeDomainObjectsContents.Remove (_order1.ID);
    }

    [Test]
    public void HasChangedFalse ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChangedFalseWhenSameElements ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);
      _customerEndPoint.OppositeDomainObjects.Add (Order.NewObject ());
      Assert.That (_customerEndPoint.HasChanged, Is.True);
      _customerEndPoint.OppositeDomainObjects.RemoveAt (_customerEndPoint.OppositeDomainObjects.Count - 1);
      Assert.That (_customerEndPoint.HasChanged, Is.False);
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Touch ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedAddAndRemove_LeavingSameElements ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Add (Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      _customerEndPoint.OppositeDomainObjects.RemoveAt (_customerEndPoint.OppositeDomainObjects.Count - 1);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedInsert ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Insert (0, Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemove ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Remove (_customerEndPoint.OppositeDomainObjects[0]);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemoveNonExisting ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Remove (Order.NewObject());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClear ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClearEmpty ()
    {
      _customerEndPoint.OppositeDomainObjects.Clear ();
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplace ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects[0] = Order.NewObject();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplaceWithSame ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedPerformDelete ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.PerformDelete ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void OriginalOppositeDomainObjectsType ()
    {
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.GetType (), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.IsReadOnly, Is.True);

      Assert.That (
                  _customerEndPoint.OriginalOppositeDomainObjectsContents.RequiredItemType, Is.EqualTo (
                            _customerEndPoint.OppositeDomainObjects.RequiredItemType));
    }

    [Test]
    public void ChangeOppositeDomainObjects ()
    {
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count));

      IEndPoint endPointOfObjectBeingRemoved = RelationEndPointObjectMother.CreateObjectEndPoint (_order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification = _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Begin ();
      modification.Perform ();
      modification.End ();

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count != _customerEndPoint.OppositeDomainObjects.Count, Is.True);
    }

    [Test]
    public void PerformWithoutBegin ()
    {
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count));

      IEndPoint endPointOfObjectBeingRemoved = RelationEndPointObjectMother.CreateObjectEndPoint (_order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification =
          _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Perform();

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count != _customerEndPoint.OppositeDomainObjects.Count, Is.True);
    }

    [Test]
    public void PerformDelete ()
    {
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count));

      IEndPoint endPointOfObjectBeingRemoved = RelationEndPointObjectMother.CreateObjectEndPoint (_order1.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification = _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Begin ();
      _customerEndPoint.PerformDelete ();
      modification.End ();

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count != _customerEndPoint.OppositeDomainObjects.Count, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void SetOppositeCollectionAndNotify ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsReference.AssociatedEndPoint, Is.Null);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_PerformsBidirectionalChange ()
    {
      var newOpposites = new OrderCollection { _order2 };
      Assert.That (_order2.Customer, Is.Not.SameAs (_customerEndPoint.GetDomainObject ()));

      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_order2.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The given collection is already associated with an end point.\r\n"
        + "Parameter name: oppositeDomainObjects")]
    public void SetOppositeCollectionAndNotify_NewCollectionAlreadyAssociated ()
    {
      var otherEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new DomainObject[0]);
      _customerEndPoint.SetOppositeCollectionAndNotify (otherEndPoint.OppositeDomainObjects);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void SetOppositeCollectionAndNotify_ObjectDeleted ()
    {
      ((Customer) _customerEndPoint.GetDomainObject ()).Delete ();
      _customerEndPoint.SetOppositeCollectionAndNotify (new OrderCollection ());
    }

    [Test]
    public void SetOppositeCollectionAndNotify_SelfReplace ()
    {
      _customerEndPoint.SetOppositeCollectionAndNotify (_customerEndPoint.OppositeDomainObjects);
      
      Assert.That (_customerEndPoint.OppositeDomainObjects.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void Commit ()
    {
      var newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.That (_customerEndPoint.HasChanged, Is.True);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.ContainsObject (newOrder), Is.False);

      var collectionBefore = _customerEndPoint.OppositeDomainObjects;

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      Assert.That (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.ContainsObject (newOrder), Is.True);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.EqualTo (_customerEndPoint.OppositeDomainObjects));
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback ()
    {
      var newOrder = Order.NewObject ();
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.That (_customerEndPoint.HasChanged, Is.True);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder), Is.True);
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.ContainsObject (newOrder), Is.False);

      var collectionBefore = _customerEndPoint.OppositeDomainObjects;
      var originalCollectionBefore = _customerEndPoint.OriginalOppositeDomainObjectsContents;

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      Assert.That (_customerEndPoint.OppositeDomainObjects.ContainsObject (newOrder), Is.False);
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.ContainsObject (newOrder), Is.False);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
    }

    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _customerEndPoint.OppositeDomainObjects[0] = _customerEndPoint.OppositeDomainObjects[0];

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);

      _customerEndPoint.Rollback();

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_RestoresCollectionStrategies_AfterReplace ()
    {
      var oldCollection = _customerEndPoint.OppositeDomainObjects;

      var newCollection = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newCollection);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldCollection.AssociatedEndPoint, Is.Null);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.Null);
      Assert.That (oldCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void SetValueFrom_SetsOppositeDomainObjects ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });
      var originalOppositeCollectionReference = _customerEndPoint.OppositeDomainObjects;
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.Not.EqualTo (source.OppositeDomainObjects));

      _customerEndPoint.SetValueFrom (source);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (source.OppositeDomainObjects));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (originalOppositeCollectionReference));
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfEndPointWasTouched ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, _customerEndPoint.OppositeDomainObjects.Cast<DomainObject>());

      _customerEndPoint.Touch ();
      _customerEndPoint.SetValueFrom (source);

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfSourceWasTouched ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, _customerEndPoint.OppositeDomainObjects.Cast<DomainObject> ());

      source.Touch ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      _customerEndPoint.SetValueFrom (source);

      Assert.That (_customerEndPoint.HasChanged, Is.False);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetValueFrom_HasBeenTouched_TrueIfDataWasChanged ()
    {
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (_customerEndPointID, new[] { _order2 });

      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      Assert.That (source.HasBeenTouched, Is.False);

      _customerEndPoint.SetValueFrom (source);

      Assert.That (_customerEndPoint.HasChanged, Is.True);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Cannot set this end point's value from "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'; the end points "
        + "do not have the same end point definition.\r\nParameter name: source")]
    public void SetValueFrom_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      var source = RelationEndPointObjectMother.CreateCollectionEndPoint (otherID, new DomainObject[0]);

      _customerEndPoint.SetValueFrom (source);
    }
    
    [Test]
    public void CreateRemoveModification ()
    {
      var modification = _customerEndPoint.CreateRemoveModification (_order1);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));

      var collectionData = 
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (_customerEndPoint.OppositeDomainObjects);
      Assert.That (((CollectionEndPointRemoveModification) modification).ModifiedCollectionData, Is.SameAs (collectionData.GetUndecoratedDataStore ()));
    }

    [Test]
    public void CreateInsertModification ()
    {
      var modification = _customerEndPoint.CreateInsertModification (_order1, 12);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointInsertModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order1));
      Assert.That (((CollectionEndPointInsertModification) modification).Index, Is.EqualTo (12));

      var collectionData =
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (_customerEndPoint.OppositeDomainObjects);
      Assert.That (((CollectionEndPointInsertModification) modification).ModifiedCollectionData, Is.SameAs (collectionData.GetUndecoratedDataStore ()));
    }

    [Test]
    public void CreateAddModification ()
    {
      var modification = _customerEndPoint.CreateAddModification (_order1);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointInsertModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order1));
      Assert.That (((CollectionEndPointInsertModification) modification).Index, Is.EqualTo (2));

      var collectionData =
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (_customerEndPoint.OppositeDomainObjects);
      Assert.That (((CollectionEndPointInsertModification) modification).ModifiedCollectionData, Is.SameAs (collectionData.GetUndecoratedDataStore()));
    }

    [Test]
    public void CreateReplaceModification ()
    {
      var modification = _customerEndPoint.CreateReplaceModification(0, _orderWithoutOrderItem);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointReplaceModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_orderWithoutOrderItem));

      var collectionData =
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (_customerEndPoint.OppositeDomainObjects);
      Assert.That (((CollectionEndPointReplaceModification) modification).ModifiedCollectionData, Is.SameAs (collectionData.GetUndecoratedDataStore ()));
    }

    [Test]
    public void CreateReplaceModification_SelfReplace ()
    {
      var modification = _customerEndPoint.CreateReplaceModification (0, _customerEndPoint.OppositeDomainObjects[0]);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointReplaceSameModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order1));
    }

    [Test]
    public void CreateDelegatingCollectionData ()
    {
      var data = _customerEndPoint.CreateDelegatingCollectionData ();

      Assert.That (data.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (data.Count, Is.EqualTo (2), "contains data of end point");
      _customerEndPoint.OppositeDomainObjects.Insert (1, _order2);
      Assert.That (data.Count, Is.EqualTo (3), "represents end point");
    }

    [Test]
    public void OppositeDomainObjects_Set ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.OppositeDomainObjects = newOppositeCollection;

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOppositeCollection));
    }

    [Test]
    public void OppositeDomainObjects_Set_TouchesEndPoint ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.OppositeDomainObjects = newOppositeCollection;

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The new opposite collection must have been prepared to delegate to this end point. Use SetOppositeCollectionAndNotify instead."
        + "\r\nParameter name: value")]
    public void OppositeDomainObjects_Set_NotAssociated ()
    {
      var newOppositeCollection = new OrderCollection { _order1 };

      _customerEndPoint.OppositeDomainObjects = newOppositeCollection;
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument value has type Remotion.Data.DomainObjects.DomainObjectCollection when type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection was expected.\r\nParameter name: value")]
    public void OppositeDomainObjects_Set_OtherType ()
    {
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData ();
      var newOppositeCollection = new DomainObjectCollection (delegatingData);

      _customerEndPoint.OppositeDomainObjects = newOppositeCollection;
    }
  }
}
