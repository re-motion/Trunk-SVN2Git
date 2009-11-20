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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : RelationEndPointBaseTest
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

      _customerEndPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
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
      CreateCollectionEndPoint (null, new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Initialize_WithNullInitialContents_Throws ()
    {
      CreateCollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void Initialize_WithoutCollectionCtorTakingData_Throws ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainObjectWithCollectionMissingCtor));

      var endPointID = new RelationEndPointID (
          new ObjectID (classDefinition, Guid.NewGuid ()), 
          typeof (DomainObjectWithCollectionMissingCtor) + ".OppositeObjects");
      CreateCollectionEndPoint (endPointID, new DomainObject[0]);
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

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
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

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification =
          _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Perform();

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count != _customerEndPoint.OppositeDomainObjects.Count, Is.True);
    }

    [Test]
    public void PerformDelete ()
    {
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count));

      IEndPoint endPointOfObjectBeingRemoved = CreateObjectEndPoint (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _customerEndPoint.ObjectID);
      var modification = _customerEndPoint.CreateRemoveModification (endPointOfObjectBeingRemoved.GetDomainObject());
      modification.Begin ();
      _customerEndPoint.PerformDelete ();
      modification.End ();

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents.Count != _customerEndPoint.OppositeDomainObjects.Count, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects.Count, Is.EqualTo (0));
    }

    private void CheckIfRelationEndPointsAreEqual (CollectionEndPoint expected, CollectionEndPoint actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.That (actual, Is.Not.SameAs (expected));

      Assert.That (actual.ChangeDelegate, Is.SameAs (expected.ChangeDelegate));
      Assert.That (actual.ClientTransaction, Is.SameAs (expected.ClientTransaction));
      Assert.That (actual.Definition, Is.SameAs (expected.Definition));
      Assert.That (actual.GetDomainObject (), Is.SameAs (expected.GetDomainObject ()));
      Assert.That (actual.HasChanged, Is.EqualTo (expected.HasChanged));
      Assert.That (actual.HasBeenTouched, Is.EqualTo (expected.HasBeenTouched));
      Assert.That (actual.ID, Is.EqualTo (expected.ID));
      Assert.That (actual.ObjectID, Is.EqualTo (expected.ObjectID));

      Assert.That (actual.OppositeDomainObjects, Is.Not.Null);
      Assert.That (actual.OppositeDomainObjects, Is.Not.SameAs (expected.OppositeDomainObjects));

      Assert.That (actual.OppositeDomainObjects.Count, Is.EqualTo (expected.OppositeDomainObjects.Count));
      for (int i = 0; i < expected.OppositeDomainObjects.Count; ++i)
        Assert.That (actual.OppositeDomainObjects[i], Is.SameAs (expected.OppositeDomainObjects[i]));

      Assert.That (actual.OriginalOppositeDomainObjectsContents, Is.Not.Null);
      Assert.That (actual.OriginalOppositeDomainObjectsContents, Is.Not.SameAs (expected.OriginalOppositeDomainObjectsContents));
      Assert.That (actual.OriginalOppositeDomainObjectsContents.IsReadOnly, Is.EqualTo (expected.OriginalOppositeDomainObjectsContents.IsReadOnly));

      Assert.That (actual.OriginalOppositeDomainObjectsContents.Count, Is.EqualTo (expected.OriginalOppositeDomainObjectsContents.Count));
      for (int i = 0; i < expected.OriginalOppositeDomainObjectsContents.Count; ++i)
        Assert.That (actual.OriginalOppositeDomainObjectsContents[i], Is.SameAs (expected.OriginalOppositeDomainObjectsContents[i]));
    }

    [Test]
    public void CloneUnchanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem item1 = order.OrderItems[0];
      OrderItem item2 = order.OrderItems[1];

      var id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.That (endPoint.ChangeDelegate, Is.Not.Null);
      Assert.That (endPoint.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (endPoint.Definition, Is.Not.Null);
      Assert.That (endPoint.GetDomainObject (), Is.SameAs (order));
      Assert.That (endPoint.HasChanged, Is.False);
      Assert.That (endPoint.HasBeenTouched, Is.False);
      Assert.That (endPoint.ID, Is.EqualTo (id));
      Assert.That (endPoint.ObjectID, Is.EqualTo (order.ID));
      Assert.That (endPoint.OppositeDomainObjects, Is.Not.Null);

      Assert.That (endPoint.OppositeDomainObjects.Count, Is.EqualTo (2));
      Assert.That (endPoint.OppositeDomainObjects[0], Is.SameAs (item1));
      Assert.That (endPoint.OppositeDomainObjects[1], Is.SameAs (item2));

      Assert.That (endPoint.OriginalOppositeDomainObjectsContents, Is.Not.SameAs (endPoint.OppositeDomainObjects));

      Assert.That (endPoint.OriginalOppositeDomainObjectsContents.Count, Is.EqualTo (2));
      Assert.That (endPoint.OriginalOppositeDomainObjectsContents[0], Is.SameAs (item1));
      Assert.That (endPoint.OriginalOppositeDomainObjectsContents[1], Is.SameAs (item2));

      var clone = (CollectionEndPoint) endPoint.Clone (endPoint.ClientTransaction);

      Assert.That (clone, Is.Not.Null);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void CloneChanged ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var originalItem1 = order.OrderItems[0];
      var originalItem2 = order.OrderItems[1];

      order.OrderItems.Clear ();

      var item1 = OrderItem.NewObject ();
      var item2 = OrderItem.NewObject ();
      var item3 = OrderItem.NewObject ();

      order.OrderItems.Add (item1);
      order.OrderItems.Add (item2);
      order.OrderItems.Add (item3);

      var id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      var endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.That (endPoint.ChangeDelegate, Is.Not.Null);
      Assert.That (endPoint.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (endPoint.Definition, Is.Not.Null);
      Assert.That (endPoint.GetDomainObject (), Is.SameAs (order));
      Assert.That (endPoint.HasChanged, Is.True);
      Assert.That (endPoint.HasBeenTouched, Is.True);
      Assert.That (endPoint.ID, Is.EqualTo (id));
      Assert.That (endPoint.ObjectID, Is.EqualTo (order.ID));
      Assert.That (endPoint.OppositeDomainObjects, Is.Not.Null);

      Assert.That (endPoint.OppositeDomainObjects.Count, Is.EqualTo (3));
      Assert.That (endPoint.OppositeDomainObjects[0], Is.SameAs (item1));
      Assert.That (endPoint.OppositeDomainObjects[1], Is.SameAs (item2));
      Assert.That (endPoint.OppositeDomainObjects[2], Is.SameAs (item3));

      Assert.That (endPoint.OriginalOppositeDomainObjectsContents, Is.Not.SameAs (endPoint.OppositeDomainObjects));

      Assert.That (endPoint.OriginalOppositeDomainObjectsContents.Count, Is.EqualTo (2));
      Assert.That (endPoint.OriginalOppositeDomainObjectsContents[0], Is.SameAs (originalItem1));
      Assert.That (endPoint.OriginalOppositeDomainObjectsContents[1], Is.SameAs (originalItem2));

      var clone = (CollectionEndPoint) endPoint.Clone (endPoint.ClientTransaction);
      Assert.That (clone, Is.Not.Null);
      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void ReplaceOppositeCollection ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsReference.AssociatedEndPoint, Is.Null);
    }

    [Test]
    public void ReplaceOppositeCollection_PerformsBidirectionalChange ()
    {
      var newOpposites = new OrderCollection { _order2 };
      Assert.That (_order2.Customer, Is.Not.SameAs (_customerEndPoint.GetDomainObject ()));

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_order2.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
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
    [Ignore ("TODO 992")]
    public void Rollback_RestoresCollectionStrategies_AfterReplace ()
    {
      var oldCollection = _customerEndPoint.OppositeDomainObjects;

      var newCollection = new OrderCollection { _order2 };
      _customerEndPoint.ReplaceOppositeCollection (newCollection);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldCollection.AssociatedEndPoint, Is.Null);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.Null);
      Assert.That (oldCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoUnchanged ()
    {
      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, new[] { _orderWithoutOrderItem });

      var newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      Assert.That (endPoint1.HasChanged, Is.False);
      Assert.That (endPoint1.HasBeenTouched, Is.False);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _order1 }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjectsContents;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _orderWithoutOrderItem, newOrder }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      Assert.That (endPoint1.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoUnchanged ()
    {
      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, new[] { _orderWithoutOrderItem });

      Assert.That (endPoint1.HasChanged, Is.False);
      Assert.That (endPoint1.HasBeenTouched, Is.False);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _order1 }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjectsContents;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _orderWithoutOrderItem }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      Assert.That (endPoint1.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoChanged ()
    {
      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, new[] { _orderWithoutOrderItem });

      var newOrder = Order.NewObject ();
      endPoint2.OppositeDomainObjects.Add (newOrder);

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjectsContents;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _orderWithoutOrderItem, newOrder }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      Assert.That (endPoint1.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoChanged ()
    {
      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, new[] { _orderWithoutOrderItem });

      endPoint1.OppositeDomainObjects.Clear ();

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new object[0]));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjectsContents;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.That (endPoint1.HasChanged, Is.True);
      Assert.That (endPoint1.HasBeenTouched, Is.True);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _orderWithoutOrderItem }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      Assert.That (endPoint1.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoEqual ()
    {
      var endPoint1 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });
      var endPoint2 = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1 });

      Assert.That (endPoint1.HasChanged, Is.False);
      Assert.That (endPoint1.HasBeenTouched, Is.False);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _order1 }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      var collectionBefore = endPoint1.OppositeDomainObjects;
      var originalCollectionBefore = endPoint1.OriginalOppositeDomainObjectsContents;

      PrivateInvoke.InvokeNonPublicMethod (endPoint1, "TakeOverCommittedData", endPoint2);

      Assert.That (endPoint1.HasChanged, Is.False);
      Assert.That (endPoint1.HasBeenTouched, Is.False);
      Assert.That (endPoint1.OppositeDomainObjects, Is.EquivalentTo (new[] { _order1 }));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.EquivalentTo (new[] { _order1 }));

      Assert.That (endPoint1.OppositeDomainObjects, Is.SameAs (collectionBefore));
      Assert.That (endPoint1.OriginalOppositeDomainObjectsContents, Is.SameAs (originalCollectionBefore));
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
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointSelfReplaceModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_order1));
      Assert.That (modification.NewRelatedObject, Is.SameAs (_order1));
    }

    [Test]
    public void CreateSelfReplaceModification ()
    {
      var modification = _customerEndPoint.CreateSelfReplaceModification (_orderWithoutOrderItem);
      Assert.That (modification, Is.InstanceOfType (typeof (CollectionEndPointSelfReplaceModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (_orderWithoutOrderItem));

      var collectionData =
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (_customerEndPoint.OppositeDomainObjects);
      Assert.That (((CollectionEndPointSelfReplaceModification) modification).ModifiedCollectionData, Is.SameAs (collectionData.GetUndecoratedDataStore ()));
    }

    [Test]
    public void CreateDelegatingCollectionData ()
    {
      var dataStore = new DomainObjectCollectionData (new[] { _order1 });
      var data = _customerEndPoint.CreateDelegatingCollectionData (dataStore);

      Assert.That (data.AssociatedEndPoint, Is.SameAs (_customerEndPoint));

      Assert.That (data.Count, Is.EqualTo (1), "contains data of data store");
      dataStore.Insert (1, _orderWithoutOrderItem);
      Assert.That (data.Count, Is.EqualTo (2), "is bound to data store");
    }

    [Test]
    public void SetOppositeCollection ()
    {
      var dataStore = new DomainObjectCollectionData (new[] { _order1 });
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData (dataStore);
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.SetOppositeCollection (newOppositeCollection);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOppositeCollection));
    }

    [Test]
    public void SetOppositeCollection_TouchesEndPoint ()
    {
      var dataStore = new DomainObjectCollectionData (new[] { _order1 });
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData (dataStore);
      var newOppositeCollection = new OrderCollection (delegatingData);

      _customerEndPoint.SetOppositeCollection (newOppositeCollection);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The new opposite collection must have been prepared to delegate to this end point. Use ReplaceOppositeCollection instead."
        + "\r\nParameter name: oppositeDomainObjects")]
    public void SetOppositeCollection_NotAssociated ()
    {
      var dataStore = new DomainObjectCollectionData (new[] { _order1 });
      var newOppositeCollection = new OrderCollection (dataStore);

      _customerEndPoint.SetOppositeCollection (newOppositeCollection);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The new opposite collection must have the same type as the old collection "
        + "('Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection'), but its type is 'Remotion.Data.DomainObjects.DomainObjectCollection'."
        + "\r\nParameter name: oppositeDomainObjects")]
    public void SetOppositeCollection_OtherType ()
    {
      var dataStore = new DomainObjectCollectionData (new[] { _order1 });
      var delegatingData = _customerEndPoint.CreateDelegatingCollectionData (dataStore);
      var newOppositeCollection = new DomainObjectCollection (delegatingData, null);

      _customerEndPoint.SetOppositeCollection (newOppositeCollection);
    }
  }
}
