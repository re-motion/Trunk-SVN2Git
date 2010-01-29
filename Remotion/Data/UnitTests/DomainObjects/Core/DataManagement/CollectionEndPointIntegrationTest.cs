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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointIntegrationTest : ClientTransactionBaseTest
  {
    private Order _order1; // belongs to customer1
    private Order _orderWithoutOrderItem; // belongs to customer1
    private Order _order2; // belongs to customer3

    private CollectionEndPoint _customerEndPoint;
    
    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _customerEndPoint = RelationEndPointObjectMother.CreateCollectionEndPoint_Customer1_Orders (_order1, _orderWithoutOrderItem);
    }

    [Test]
    public void AddToOppositeDomainObjects ()
    {
      var newOrder = Order.NewObject ();

      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, newOrder }), "changes go down to actual data store");
      Assert.That (newOrder.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()), "bidirectional modification");
    }

    [Test]
    public void RemoveFromOppositeDomainObjects ()
    {
      _customerEndPoint.OppositeDomainObjects.Remove (_order1.ID);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }), "changes go down to actual data store");
      Assert.That (_order1.Customer, Is.Null, "bidirectional modification");
    }

    [Test]
    public void SetOppositeCollectionAndNotify ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };

      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (oldOpposites, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      Assert.That (newOpposites.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldOpposites.AssociatedEndPoint, Is.Null);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_DataStrategy_OfOldOpposites ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetDataStore ();

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(oldOpposites, typeof (Order));

      // old collection got a new data store...
      var dataStoreOfOldOpposites = 
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (oldOpposites).GetDataStore();
      Assert.That (dataStoreOfOldOpposites, Is.Not.SameAs (originalDataStoreOfOldOpposites));

      // with the data it had before!
      Assert.That (dataStoreOfOldOpposites.ToArray (), Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
    }

    [Test]
    public void SetOppositeCollectionAndNotify_DataStrategy_OfNewOpposites ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetDataStore ();
      
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      // end point still holds on to the same old data store...
      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (newOpposites, typeof (Order), _customerEndPoint, originalDataStoreOfOldOpposites);

      // but with the new data!
      Assert.That (originalDataStoreOfOldOpposites.ToArray (), Is.EqualTo (new[] { _orderWithoutOrderItem }));

    }

    [Test]
    public void SetOppositeCollectionAndNotify_PerformsAllBidirectionalChanges ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2};

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      
      Assert.That (_order1.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (customer3));
      Assert.That (customer3.Orders, List.Contains (_order2));

      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_order1.Customer, Is.Null);
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()));
      Assert.That (customer3.Orders, List.Not.Contains (_order2));
    }

    [Test]
    public void SetOppositeCollectionAndNotify_RaisesNoEventsOnCollections ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var oldEventListener = new DomainObjectCollectionEventReceiver (oldOpposites);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      var newEventListener = new DomainObjectCollectionEventReceiver (newOpposites);

      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (oldEventListener.HasAddedEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasAddingEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasRemovedEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasRemovingEventBeenCalled, Is.False);

      Assert.That (newEventListener.HasAddedEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasAddingEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasRemovedEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasRemovingEventBeenCalled, Is.False);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_SourceCollection_IsReadOnly ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2 };
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      ((OrderCollection) _customerEndPoint.OppositeDomainObjects).SetIsReadOnly (true);
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OriginalCollectionReference, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetOppositeCollectionAndNotify_TargetCollection_IsReadOnly ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2 };
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      newOpposites.SetIsReadOnly (true);
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OriginalCollectionReference, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetOppositeCollectionAndNotify_HasChanged_OnlyWhenSetToNewCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      _customerEndPoint.SetOppositeCollectionAndNotify (_customerEndPoint.OppositeDomainObjects);
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_HasChanged_EvenWhenSetToEqualCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _order1, _orderWithoutOrderItem };
      Assert.That (newOpposites, Is.EqualTo (_customerEndPoint.OppositeDomainObjects));
      Assert.That (newOpposites, Is.Not.SameAs (_customerEndPoint.OppositeDomainObjects));

      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_RemembersOriginalCollection ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var oldOriginalOpposites = _customerEndPoint.OriginalOppositeDomainObjectsContents;

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.SameAs (oldOriginalOpposites));
      Assert.That (_customerEndPoint.OriginalCollectionReference, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetOppositeCollectionAndNotify_SetsTouchedFlag ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetOppositeCollectionAndNotify_SelfSet ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var originalOpposites = _customerEndPoint.OppositeDomainObjects;
      _customerEndPoint.SetOppositeCollectionAndNotify (_customerEndPoint.OppositeDomainObjects);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (originalOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void Rollback_AfterReplace_RestoresPreviousReference ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites); // replace collection

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
    }

    [Test]
    public void Rollback_AfterReplace_RestoresDelegationChain ()
    {
      var oldCollection = _customerEndPoint.OppositeDomainObjects;
      var oldCollectionDataStore = 
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (oldCollection).GetDataStore ();

      var newCollection = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newCollection);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldCollection.AssociatedEndPoint, Is.Null);

      _customerEndPoint.Rollback ();

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (oldCollection, typeof (Order), _customerEndPoint, oldCollectionDataStore);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (newCollection, typeof (Order));
    }

    [Test]
    public void Rollback_AfterReplace_RestoresPreviousReference_UndoesModifications_LeavesModificationOnDetached ()
    {
      _customerEndPoint.OppositeDomainObjects.Clear (); // modify collection
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites); // replace collection

      newOpposites.Add (_order1);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (newOpposites, Is.EqualTo (new[] { _orderWithoutOrderItem, _order1 })); // does not undo changes on detached collection
    }

    [Test]
    public void Rollback_ReadOnly ()
    {
      _customerEndPoint.OppositeDomainObjects.Add (_order2);
      ((OrderCollection) _customerEndPoint.OppositeDomainObjects).SetIsReadOnly (true);

      _customerEndPoint.Rollback();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OriginalCollectionReference, Is.SameAs (_customerEndPoint.OppositeDomainObjects));
      Assert.That (_customerEndPoint.OppositeDomainObjects.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit_AfterReplace_SavesReference ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      oldOpposites.Clear (); // modify collection

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.SetOppositeCollectionAndNotify (newOpposites); // replace collection
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
    }

    [Test]
    public void Commit_AfterReplace_DelegationChain ()
    {
      var oldCollection = _customerEndPoint.OppositeDomainObjects;
      var oldCollectionDataStore =
          DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (oldCollection).GetDataStore ();

      var newCollection = new OrderCollection { _order2 };
      _customerEndPoint.SetOppositeCollectionAndNotify (newCollection);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newCollection));
      Assert.That (newCollection.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldCollection.AssociatedEndPoint, Is.Null);

      _customerEndPoint.Commit ();

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (oldCollection, typeof (Order));
      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (newCollection, typeof (Order), _customerEndPoint, oldCollectionDataStore);
    }

    [Test]
    public void Commit_ReadOnly ()
    {
      _customerEndPoint.OppositeDomainObjects.Add (_order2);
      ((OrderCollection) _customerEndPoint.OppositeDomainObjects).SetIsReadOnly (true);

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, _order2 }));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, _order2 }));
      Assert.That (_customerEndPoint.OriginalCollectionReference, Is.SameAs (_customerEndPoint.OppositeDomainObjects));
      Assert.That (_customerEndPoint.OppositeDomainObjects.IsReadOnly, Is.True);
    }

    private IDomainObjectCollectionData GetDomainObjectCollectionData (DomainObjectCollection collection)
    {
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      return DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (decorator);
    }
  }
}