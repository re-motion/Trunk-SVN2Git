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

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointIntegrationTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _customerEndPointID;

    private Order _order1; // belongs to customer1
    private Order _orderWithoutOrderItem; // belongs to customer1
    private Order _order2; // belongs to customer3

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
    }

    [Test]
    public void AddToOppositeDomainObjects ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
      var newOrder = Order.NewObject ();

      endPoint.OppositeDomainObjects.Add (newOrder);

      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, newOrder }), "changes go down to actual data store");
      Assert.That (newOrder.Customer, Is.SameAs (endPoint.GetDomainObject()), "bidirectional modification");
    }

    [Test]
    public void RemoveFromOppositeDomainObjects ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });

      endPoint.OppositeDomainObjects.Remove (_order1.ID);

      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }), "changes go down to actual data store");
      Assert.That (_order1.Customer, Is.Null, "bidirectional modification");
    }

    [Test]
    public void ReplaceOppositeCollection()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });

      var oldOpposites = endPoint.OppositeDomainObjects;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetUndecoratedDataStore();

      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));
      var originalDataOfNewOpposites = GetDomainObjectCollectionData (newOpposites);
      var originalDataStoreOfNewOpposites = originalDataOfNewOpposites.GetUndecoratedDataStore();

      endPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (oldOpposites, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      var dataOfNewOpposites = GetDomainObjectCollectionData (newOpposites);
      var dataStoreOfNewOpposites = dataOfNewOpposites.GetUndecoratedDataStore();
      Assert.That (dataOfNewOpposites, Is.TypeOf (typeof (EndPointDelegatingCollectionData)));
      Assert.That (dataOfNewOpposites.AssociatedEndPoint, Is.SameAs (endPoint));
      Assert.That (dataStoreOfNewOpposites, Is.SameAs (originalDataStoreOfNewOpposites));

      var dataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var dataStoreOfOldOpposites = dataOfOldOpposites.GetUndecoratedDataStore ();
      Assert.That (dataOfOldOpposites, Is.TypeOf (typeof (EventRaisingCollectionDataDecorator)));
      Assert.That (dataOfOldOpposites.AssociatedEndPoint, Is.Null);
      Assert.That (dataStoreOfOldOpposites, Is.SameAs (originalDataStoreOfOldOpposites));
    }

    [Test]
    public void ReplaceOppositeCollection_PerformsBidirectionalChanges ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });

      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem, _order2}));

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      
      Assert.That (_order1.Customer, Is.SameAs (endPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (endPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (customer3));
      Assert.That (customer3.Orders, List.Contains (_order2));

      endPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_order1.Customer, Is.Null);
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (endPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (endPoint.GetDomainObject()));
      Assert.That (customer3.Orders, List.Not.Contains (_order2));
    }

    [Test]
    public void ReplaceOppositeCollection_RaisesNoEventsOnCollections ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });

      var oldOpposites = endPoint.OppositeDomainObjects;
      var oldEventListener = new DomainObjectCollectionEventReceiver (oldOpposites);

      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));
      var newEventListener = new DomainObjectCollectionEventReceiver (newOpposites);

      endPoint.ReplaceOppositeCollection (newOpposites);

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
    public void ReplaceOppositeCollection_SetsChangedFlag_WhenSetToNewCollection ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _orderWithoutOrderItem });
      Assert.That (endPoint.HasChanged, Is.False);

      endPoint.ReplaceOppositeCollection (endPoint.OppositeDomainObjects);
      Assert.That (endPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      endPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (endPoint.HasChanged, Is.True);
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
      var oldOpposites = endPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection

      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference_UndoesModifications_LeavesModificationOnDetached ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
      endPoint.OppositeDomainObjects.Clear (); // modify collection
      var oldOpposites = endPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection

      newOpposites.Add (_order1);

      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
      Assert.That (newOpposites, Is.EqualTo (new[] { _orderWithoutOrderItem, _order1 })); // does not undo changes on detached collection
    }

    [Test]
    public void CommitAfterReplace_SavesReference ()
    {
      var endPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
      var oldOpposites = endPoint.OppositeDomainObjects;

      oldOpposites.Clear (); // modify collection

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      endPoint.ReplaceOppositeCollection (newOpposites); // replace collection
      endPoint.Commit ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);

      endPoint.Rollback ();

      Assert.That (endPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (endPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (endPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (endPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);
    }

    private IDomainObjectCollectionData GetDomainObjectCollectionData (DomainObjectCollection collection)
    {
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      return DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (decorator);
    }
  }
}