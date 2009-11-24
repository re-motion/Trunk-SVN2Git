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
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    #region Setup/Teardown

    public override void SetUp ()
    {
      base.SetUp();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
      _customer3NotInCollection = Customer.GetObject (DomainObjectIDs.Customer3);

      _collection = CreateCustomerCollection();
    }

    #endregion

    private DomainObjectCollection _collection;
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    private void ReplaceItems (DomainObjectCollection collection, DomainObjectCollection sourceCollection)
    {
      PrivateInvoke.InvokeNonPublicMethod (collection, "ReplaceItems", sourceCollection);
    }

    private void CheckKeys (ObjectID[] expectedKeys, DomainObjectCollection collection)
    {
      Assert.That (collection.Count, Is.EqualTo (expectedKeys.Length), "DomainObjectCollection.Count");
      foreach (ObjectID expectedKey in expectedKeys)
        Assert.That (collection.Contains (expectedKey), Is.True, string.Format ("Key {0}", expectedKey));
    }

    private IDomainObjectCollectionData GetNonNotifyingData (DomainObjectCollection collection)
    {
      return (IDomainObjectCollectionData) PrivateInvoke.InvokeNonPublicMethod (collection, "GetNonNotifyingData");
    }

    private void CopyEventHandlersFrom (DomainObjectCollection source, DomainObjectCollection destination)
    {
      PrivateInvoke.InvokeNonPublicMethod (destination, "CopyEventHandlersFrom", source);
    }

    private DomainObjectCollection CreateCustomerCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (_customer1);
      collection.Add (_customer2);

      return collection;
    }

    private void CheckForEachOrder (DomainObject[] exptectedDomainObjects, DomainObjectCollection collection)
    {
      Assert.That (collection.Count, Is.EqualTo (exptectedDomainObjects.Length));

      int i = 0;
      foreach (DomainObject domainObject in collection)
      {
        Assert.That (domainObject, Is.SameAs (exptectedDomainObjects[i]));
        i++;
      }
    }

    private void CheckSameEventHandlers (DomainObjectCollection source, DomainObjectCollection destination, string eventName)
    {
      var sourceEvent = ((Delegate) PrivateInvoke.GetNonPublicField (source, eventName));
      Delegate[] sourceInvocationList = sourceEvent.GetInvocationList();

      var destinationEvent = ((Delegate) PrivateInvoke.GetNonPublicField (destination, eventName));
      Assert.That (destinationEvent, SyntaxHelper.Not.Null, eventName + " event handlers not copied");
      Delegate[] destinationInvocationList = destinationEvent.GetInvocationList();

      Assert.That (sourceInvocationList, Is.EqualTo (destinationInvocationList), eventName + " event handlers not copied");
    }

    private CollectionEndPoint CreateCollectionEndPointForOrders ()
    {
      var customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      return new CollectionEndPoint (
          ClientTransactionMock,
          customerEndPointID,
          new DomainObject[0]);
    }

    private OrderCollection CreateAssociatedCollection ()
    {
      var collectionEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      var actualData = new DomainObjectCollectionData();

      var delegatingStrategy = new EndPointDelegatingCollectionData (collectionEndPointStub, actualData);
      var associatedCollection = new OrderCollection (new ArgumentCheckingCollectionDataDecorator (typeof (Order), delegatingStrategy));
      Assert.That (associatedCollection.AssociatedEndPoint, Is.SameAs (collectionEndPointStub));
      return associatedCollection;
    }

    [Test]
    public void AccessIListInterfaceWithDomainObject ()
    {
      IList list = _collection;

      Assert.That (list.Contains (_customer1), Is.True);
      Assert.That (list.IndexOf (_customer1), Is.EqualTo (0));
      Assert.That (list[0], Is.SameAs (_customer1));

      list.Remove (_customer1);
      Assert.That (list.Contains (_customer1), Is.False);

      Assert.That (list.Add (_customer1), Is.EqualTo (1));
      Assert.That (list.IndexOf (_customer1), Is.EqualTo (1));

      list.Insert (0, _customer3NotInCollection);
      Assert.That (list.IndexOf (_customer3NotInCollection), Is.EqualTo (0));
    }

    [Test]
    public void AccessIListInterfaceWithObjectID ()
    {
      IList list = _collection;

      Assert.That (list.Contains (_customer1.ID), Is.True);
      Assert.That (list.IndexOf (_customer1.ID), Is.EqualTo (0));

      list.Remove (_customer1.ID);
      Assert.That (list.Contains (_customer1), Is.False);
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_customer3NotInCollection);

      Assert.That (_collection, Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Add_ReadOnly_Throws ()
    {
      new DomainObjectCollection (_collection, true).Add (_customer3NotInCollection);
    }

    [Test]
    public void AddEvents ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));

      var eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, false);

      collection.Add (_customer1);

      Assert.That (collection.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer1));
      Assert.That (eventReceiver.AddedDomainObject, Is.SameAs (_customer1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void AddObjectOfInvalidType ()
    {
      IList list = _collection;
      list.Add (new object());
    }

    [Test]
    public void AddRange ()
    {
      var collection = new DomainObjectCollection();
      collection.AddRange (new[] { _customer1, _customer2 });

      Assert.That (collection, Is.EqualTo (new[] { _customer1, _customer2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException), ExpectedMessage = "Item 1 of argument domainObjects is null.")]
    public void AddRange_ChecksItems ()
    {
      var collection = new DomainObjectCollection();
      collection.AddRange (new[] { _customer1, null });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddToReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem3));
    }

    [Test]
    public void AssociatedEndPoint ()
    {
      Assert.That (_collection.AssociatedEndPoint, Is.Null);

      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint>();
      var endPointStrategy = new EndPointDelegatingCollectionData (endPointStub, new DomainObjectCollectionData());

      var endPointCollection = new DomainObjectCollection (endPointStrategy);
      Assert.That (endPointCollection.AssociatedEndPoint, Is.SameAs (endPointStub));
    }

    [Test]
    public void CancelAddEvents ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));

      var eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, true);

      try
      {
        collection.Add (_customer1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (collection.Count, Is.EqualTo (0));
        Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer1));
        Assert.That (eventReceiver.AddedDomainObject, Is.Null);
      }
    }

    [Test]
    public void CancelClearEvents ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Clear();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_collection.Count, Is.EqualTo (2));
        Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
      }
    }

    [Test]
    public void CancelRemoveEvents ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Remove (_customer1.ID);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_collection.Count, Is.EqualTo (2));
        Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
        Assert.That (eventReceiver.RemovingDomainObjects[0], Is.SameAs (_customer1));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ChangeCollectionDuringEnumeration ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      foreach (OrderItem item in order.OrderItems)
        order.OrderItems.Remove (item);
    }

    [Test]
    public void ChangeEvents ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection);

      _collection[0] = _customer3NotInCollection;

      var expectedStates = new ChangeState[]
                           {
                               new CollectionChangeState (_collection, _customer1, "1. Removing event"),
                               new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event"),
                               new CollectionChangeState (_collection, _customer1, "3. Removed event"),
                               new CollectionChangeState (_collection, _customer3NotInCollection, "4. Added event")
                           };

      Assert.That (_collection[0], Is.SameAs (_customer3NotInCollection));
      Assert.That (_collection.Count, Is.EqualTo (2));

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ChangeEventsWithAdditionCancelled ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection, 2);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[]
                             {
                                 new CollectionChangeState (_collection, _customer1, "1. Removing event"),
                                 new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event")
                             };

        Assert.That (_collection[0], Is.SameAs (_customer1));
        Assert.That (_collection.Count, Is.EqualTo (2));

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void ChangeEventsWithRemovalCancelled ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection, 1);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[] { new CollectionChangeState (_collection, _customer1, "1. Removing event") };

        Assert.That (_collection[0], Is.SameAs (_customer1));
        Assert.That (_collection.Count, Is.EqualTo (2));

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void ChangeObjectThroughIListInterface ()
    {
      IList list = _collection;
      list[0] = _customer3NotInCollection;

      Assert.That (list[0], Is.SameAs (_customer3NotInCollection));
      Assert.That (list.Count, Is.EqualTo (2));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void ChangeToObjectOfInvalidType ()
    {
      IList list = _collection;
      list[0] = new object();
    }

    [Test]
    public void Clear ()
    {
      _collection.Clear();

      Assert.That (_collection.Count, Is.EqualTo (0), "Item count");
    }

    [Test]
    public void ClearEvents ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, false);

      _collection.Clear();

      Assert.That (_collection.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (2));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (2));
      Assert.That (eventReceiver.RemovingDomainObjects[_customer1.ID], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovingDomainObjects[_customer2.ID], Is.SameAs (_customer2));
      Assert.That (eventReceiver.RemovedDomainObjects[_customer1.ID], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovedDomainObjects[_customer2.ID], Is.SameAs (_customer2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ClearReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Clear();
    }

    [Test]
    public void ClearWithDiscardedObject ()
    {
      var domainObjectCollection = new DomainObjectCollection();
      Customer customer = Customer.NewObject();
      domainObjectCollection.Add (customer);
      customer.Delete();
      Assert.That (customer.IsDiscarded, Is.True);

      //The next line throws an ObjectDiscardedException:
      domainObjectCollection.Clear();

      Assert.IsEmpty (domainObjectCollection);
    }

    [Test]
    public void Clone ()
    {
      ICloneable cloneableCollection = _collection;
      var clonedCollection = (DomainObjectCollection) cloneableCollection.Clone();

      Assert.That (clonedCollection, Is.Not.Null);
      Assert.That (clonedCollection.Count, Is.EqualTo (_collection.Count));
      Assert.That (clonedCollection.IsReadOnly, Is.EqualTo (_collection.IsReadOnly));
      Assert.That (clonedCollection.RequiredItemType, Is.EqualTo (_collection.RequiredItemType));
      Assert.That (clonedCollection[0], Is.SameAs (_collection[0]));
      Assert.That (clonedCollection[1], Is.SameAs (_collection[1]));

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (clonedCollection, typeof (Customer));
    }

    [Test]
    public void Clone_AssociatedCollection ()
    {
      OrderCollection associatedCollection = CreateAssociatedCollection();
      var clonedCollection = (DomainObjectCollection) associatedCollection.Clone();

      // clone is always stand-alone, even when source is associated with end point
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (clonedCollection, associatedCollection.RequiredItemType);
    }

    [Test]
    public void Clone_DerivedCollection ()
    {
      var orderCollection = new OrderCollection();

      var clone = (OrderCollection) orderCollection.Clone();

      Assert.That (clone.GetType (), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (clone.RequiredItemType, Is.EqualTo (orderCollection.RequiredItemType));
    }

    [Test]
    public void Clone_MakeReadOnly_False ()
    {
      var readOnlyCollection = new DomainObjectCollection (_collection, true);
      DomainObjectCollection clonedCollection = readOnlyCollection.Clone (false);

      Assert.That (clonedCollection, Is.EqualTo (readOnlyCollection));
      Assert.That (clonedCollection.IsReadOnly, Is.False);
    }

    [Test]
    public void Clone_MakeReadOnly_True ()
    {
      DomainObjectCollection clonedCollection = _collection.Clone (true);

      Assert.That (clonedCollection, Is.EqualTo (_collection));
      Assert.That (clonedCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void Clone_ReadOnlyCollection ()
    {
      var readOnlyCollection = new DomainObjectCollection (_collection, true);

      DomainObjectCollection clone = readOnlyCollection.Clone();

      Assert.That (clone, Is.Not.Null, "Clone does not exist");
      Assert.That (clone.Count, Is.EqualTo (2), "Item count of clone");
      Assert.That (clone[_customer1.ID], Is.SameAs (_customer1), "Customer1");
      Assert.That (clone[_customer2.ID], Is.SameAs (_customer2), "Customer2");
      Assert.That (clone.IsReadOnly, Is.EqualTo (true), "IsReadOnly");
    }

    [Test]
    public void Clone_RequiredItemType ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      DomainObjectCollection clone = collection.Clone();

      Assert.That (clone, Is.Not.Null, "Clone does not exist");
      Assert.That (clone.RequiredItemType, Is.EqualTo (typeof (Order)), "Required item type does not match.");
    }

    [Test]
    public void CloneOrderCollection ()
    {
      var orders = new OrderCollection();
      orders.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection clonedOrders = orders.Clone (true);

      Assert.That (clonedOrders.GetType (), Is.EqualTo (typeof (OrderCollection)));
      Assert.That (clonedOrders.Count, Is.EqualTo (orders.Count));
      Assert.That (clonedOrders.RequiredItemType, Is.EqualTo (orders.RequiredItemType));
    }

    [Test]
    public void CompareFalse ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));
      var collection2 = new DomainObjectCollection (typeof (Customer));

      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.That (DomainObjectCollection.Compare (collection1, collection2), Is.False);
    }

    [Test]
    public void CompareFalseWithCollection1Null ()
    {
      var collection2 = new DomainObjectCollection (typeof (Customer));

      Assert.That (DomainObjectCollection.Compare (null, collection2), Is.False);
    }

    [Test]
    public void CompareFalseWithCollection2Null ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));

      Assert.That (DomainObjectCollection.Compare (collection1, null), Is.False);
    }

    [Test]
    public void CompareFalseWithDifferentOrder ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      var collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.That (DomainObjectCollection.Compare (collection1, collection2), Is.False);
    }

    [Test]
    public void CompareFalseWithSameCount ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));
      var collection2 = new DomainObjectCollection (typeof (Customer));

      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      Assert.That (DomainObjectCollection.Compare (collection1, collection2), Is.False);
    }

    [Test]
    public void CompareTrue ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      Assert.That (DomainObjectCollection.Compare (collection, collection), Is.True);
    }

    [Test]
    public void CompareTrueWithBothCollectionsNull ()
    {
      Assert.That (DomainObjectCollection.Compare (null, null), Is.True);
    }

    [Test]
    public void CompareTrueWithSameOrder ()
    {
      DomainObjectCollection collection1 = CreateCustomerCollection();
      DomainObjectCollection collection2 = CreateCustomerCollection();

      Assert.That (DomainObjectCollection.Compare (collection1, collection2), Is.True);
    }

    [Test]
    public void CompareWithIgnoreFalse ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      var collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.That (DomainObjectCollection.Compare (collection1, collection2, false), Is.False);
    }

    [Test]
    public void CompareWithIgnoreTrue ()
    {
      var collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      var collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.That (DomainObjectCollection.Compare (collection1, collection2, true), Is.True);
    }

    [Test]
    public void ContainsDomainObjectFalse ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var customers = new DomainObjectCollection();

      customers.Add (customer);

      Customer copy;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        copy = Customer.GetObject (customer.ID);
      }

      Assert.That (customers.ContainsObject (copy), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsDomainObjectNull ()
    {
      var customers = new DomainObjectCollection();

      customers.ContainsObject (null);
    }

    [Test]
    public void ContainsDomainObjectTrue ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var customers = new DomainObjectCollection();

      customers.Add (customer);

      Assert.That (customers.ContainsObject (customer), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsObjectIDNull ()
    {
      var customers = new DomainObjectCollection();

      customers.Contains (null);
    }

    [Test]
    public void ContainsObjectIDTrue ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var customers = new DomainObjectCollection();

      customers.Add (customer);

      Assert.That (customers.Contains (customer.ID), Is.True);
    }

    [Test]
    public void ContainsObjectOfInvalidType ()
    {
      IList list = _collection;
      Assert.That (list.Contains (new object ()), Is.False);
    }

    [Test]
    public void CopyConstructorWithDerivedType ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Company1);
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      var domainObjectCollection1 = new DomainObjectCollection (typeof (Company));
      domainObjectCollection1.Add (company);
      domainObjectCollection1.Add (partner);
      domainObjectCollection1.Add (distributor);

      var domainObjectCollection2 = new DomainObjectCollection (
          domainObjectCollection1, true);

      Assert.That (domainObjectCollection2.Count, Is.EqualTo (3), "Count");

      CheckKeys (
          new[] { DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2 },
          domainObjectCollection2);

      Assert.That (domainObjectCollection2[DomainObjectIDs.Company1], Is.SameAs (company), "Company");
      Assert.That (domainObjectCollection2[DomainObjectIDs.Partner1], Is.SameAs (partner), "Partner");
      Assert.That (domainObjectCollection2[DomainObjectIDs.Distributor2], Is.SameAs (distributor), "Distributor");
    }

    [Test]
    public void CopyEventHandlers ()
    {
      var source = new DomainObjectCollection();
      var destination = new DomainObjectCollection();

      source.Added += delegate { };
      source.Added += delegate { };
      source.Adding += delegate { };
      source.Adding += delegate { };
      source.Removed += delegate { };
      source.Removed += delegate { };
      source.Removing += delegate { };
      source.Removing += delegate { };

      CopyEventHandlersFrom (source, destination);

      CheckSameEventHandlers (source, destination, "Adding");
      CheckSameEventHandlers (source, destination, "Added");
      CheckSameEventHandlers (source, destination, "Removing");
      CheckSameEventHandlers (source, destination, "Removed");
    }

    [Test]
    public void CopyToArray ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var items = new OrderItem[order.OrderItems.Count];

      order.OrderItems.CopyTo (items, 0);

      Assert.That (items[0], Is.SameAs (order.OrderItems[0]));
      Assert.That (items[1], Is.SameAs (order.OrderItems[1]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.")]
    public void CopyToArraySmallerThanCollection ()
    {
      var array = new DomainObject[_collection.Count - 1];

      _collection.CopyTo (array, 0);
    }

    [Test]
    public void CopyToArrayWithIndex ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var items = new OrderItem[order.OrderItems.Count + 1];

      OrderItem otherItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      items[0] = otherItem;

      order.OrderItems.CopyTo (items, 1);

      Assert.That (items[0], Is.SameAs (otherItem));
      Assert.That (items[1], Is.SameAs (order.OrderItems[0]));
      Assert.That (items[2], Is.SameAs (order.OrderItems[1]));
    }

    [Test]
    public void CopyToEmptyArrayWithEmptyCollection ()
    {
      var emptyCollection = new DomainObjectCollection();
      var array = new DomainObject[0];

      emptyCollection.CopyTo (array, 0);

      // expectation: no exception
    }

    [Test]
    public void CopyToNonEmptyArrayWithEmptyCollection ()
    {
      var emptyCollection = new DomainObjectCollection();
      var array = new DomainObject[1];

      emptyCollection.CopyTo (array, 0);

      Assert.That (array[0], Is.Null);
    }

    [Test]
    public void CreateAssociationModification ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPointForOrders();
      IDomainObjectCollectionData endPointDataStore = DomainObjectCollectionDataTestHelper
          .GetCollectionDataAndCheckType<IDomainObjectCollectionData> (endPoint.OppositeDomainObjects)
          .GetUndecoratedDataStore();

      var newCollection = new OrderCollection();
      var modification = (CollectionEndPointReplaceWholeCollectionModification) newCollection.CreateAssociationModification (endPoint);

      Assert.That (modification.ModifiedEndPoint, Is.SameAs (endPoint));
      Assert.That (modification.NewOppositeCollection, Is.SameAs (newCollection));
      Assert.That (modification.NewOppositeCollectionTransformer.Collection, Is.SameAs (newCollection));
      Assert.That (modification.OldOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
      Assert.That (modification.ModifiedEndPointDataStore, Is.SameAs (endPointDataStore));
    }

    [Test]
    public void CreateAssociationModification_CollectionIsReadOnly ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPointForOrders();

      var newCollection = new OrderCollection();
      newCollection.SetIsReadOnly (true);
      newCollection.CreateAssociationModification (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This collection ('Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.UnitTests.DomainObjects.TestDomain.Order]') is not of the same type "
        + "as the end point's current opposite collection ('Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderCollection').")]
    public void CreateAssociationModification_DifferentCollectionTypes ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPointForOrders();

      var newCollection = new ObjectList<Order>();
      newCollection.CreateAssociationModification (endPoint);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This collection has a different item type than the end point's current opposite collection.")]
    public void CreateAssociationModification_DifferentRequiredItemType ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPointForOrders();

      var newCollection = new DomainObjectCollection (typeof (Customer));
      newCollection.CreateAssociationModification (endPoint);
    }

    [Test]
    public void CreateAssociationModification_SelfReplace ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPointForOrders();

      var modification = (CollectionEndPointReplaceWholeCollectionModification)
                         endPoint.OppositeDomainObjects.CreateAssociationModification (endPoint);

      Assert.That (modification.ModifiedEndPoint, Is.SameAs (endPoint));
      Assert.That (modification.NewOppositeCollection, Is.SameAs (endPoint.OppositeDomainObjects));
      Assert.That (modification.NewOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
      Assert.That (modification.OldOppositeCollectionTransformer.Collection, Is.SameAs (endPoint.OppositeDomainObjects));
    }

    [Test]
    public void DerivedType ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Company1);
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      var domainObjectCollection = new DomainObjectCollection (typeof (Company));
      domainObjectCollection.Add (company);
      domainObjectCollection.Add (partner);
      domainObjectCollection.Add (distributor);

      Assert.That (domainObjectCollection.Count, Is.EqualTo (3), "Count");

      CheckKeys (
          new[] { DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2 },
          domainObjectCollection);

      Assert.That (domainObjectCollection[DomainObjectIDs.Company1], Is.SameAs (company), "Company");
      Assert.That (domainObjectCollection[DomainObjectIDs.Partner1], Is.SameAs (partner), "Partner");
      Assert.That (domainObjectCollection[DomainObjectIDs.Distributor2], Is.SameAs (distributor), "Distributor");
    }

    [Test]
    public void EventRaiser_BeginAdd ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginAdd (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnAdding",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_BeginDelete ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginDelete();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnDeleting"));
    }

    [Test]
    public void EventRaiser_BeginRemove ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.BeginRemove (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnRemoving",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_EndAdd ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndAdd (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnAdded",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void EventRaiser_EndDelete ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndDelete();

      collectionMock.AssertWasCalled (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnDeleted"));
    }

    [Test]
    public void EventRaiser_EndRemove ()
    {
      var collectionMock = new MockRepository().PartialMock<DomainObjectCollection>();
      collectionMock.Replay();

      var eventRaiser = (IDomainObjectCollectionEventRaiser) collectionMock;
      eventRaiser.EndRemove (1, _customer1);

      collectionMock.AssertWasCalled (
          mock => PrivateInvoke.InvokeNonPublicMethod (
                      mock,
                      "OnRemoved",
                      Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _customer1)));
    }

    [Test]
    public void ExactType ()
    {
      var id1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
      var id2 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));

      ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (id1);
      ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (id2);

      var domainObjectCollection = new DomainObjectCollection (typeof (ClassWithAllDataTypes));
      domainObjectCollection.Add (c1);
      domainObjectCollection.Add (c2);

      Assert.That (domainObjectCollection.Count, Is.EqualTo (2), "Count");
      CheckKeys (new[] { id1, id2 }, domainObjectCollection);
      Assert.That (domainObjectCollection[id1], Is.SameAs (c1), "ClassWithAllDataTypes1");
      Assert.That (domainObjectCollection[id2], Is.SameAs (c2), "ClassWithAllDataTypes2");
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetInvalidNumericIndex ()
    {
      Dev.Null = _collection[_collection.Count];
    }

    [Test]
    public void GetItemsNotInCollection ()
    {
      var collection2 = new DomainObjectCollection();
      collection2.Add (_customer1);

      DomainObjectCollection itemsNotInCollection = collection2.GetItemsNotInCollection (_collection);

      Assert.That (itemsNotInCollection.Count, Is.EqualTo (1));
      Assert.That (itemsNotInCollection[0], Is.SameAs (_customer2));
    }

    [Test]
    public void GetItemsNotInCollectionBothCollectionsEmpty ()
    {
      var collection = new DomainObjectCollection();
      DomainObjectCollection itemsNotInCollection = collection.GetItemsNotInCollection (new DomainObjectCollection());
      Assert.That (itemsNotInCollection.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetItemsNotInCollectionForEmptyCollection ()
    {
      var collection2 = new DomainObjectCollection();

      DomainObjectCollection itemsNotInCollection = collection2.GetItemsNotInCollection (_collection);

      Assert.That (itemsNotInCollection.Count, Is.EqualTo (2));
      Assert.That (itemsNotInCollection.ContainsObject (_customer1), Is.True);
      Assert.That (itemsNotInCollection.ContainsObject (_customer2), Is.True);
    }

    [Test]
    // integration test
    public void GetNonNotifyingData_DoesNotRaiseEvents ()
    {
      IDomainObjectCollectionData nonNotifyingData = GetNonNotifyingData (_collection);

      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection);

      nonNotifyingData.Insert (1, _customer3NotInCollection);

      Assert.That (eventReceiver.AddingDomainObject, Is.Null);
      Assert.That (eventReceiver.AddedDomainObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\nParameter name: domainObject"
        )]
    public void GetNonNotifyingData_PerformsArgumentChecks ()
    {
      IDomainObjectCollectionData nonNotifyingData = GetNonNotifyingData (_collection);

      nonNotifyingData.Insert (1, _customer1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetNonNotifyingData_PerformsTypeChecks ()
    {
      IDomainObjectCollectionData nonNotifyingData = GetNonNotifyingData (_collection);

      nonNotifyingData.Insert (1, Order.NewObject());
    }

    [Test]
    // integration test
    public void GetNonNotifyingData_RepresentsCollectionData ()
    {
      IDomainObjectCollectionData nonNotifyingData = GetNonNotifyingData (_collection);

      _collection.Add (_customer3NotInCollection);
      Assert.That (nonNotifyingData.ToArray(), Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));

      nonNotifyingData.Remove (_customer1.ID);
      Assert.That (_collection, Is.EqualTo (new[] { _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void GetNonNotifyingData_UsesUndecoratedDataStore ()
    {
      var dataStore = new DomainObjectCollectionData();

      var dataDecoratorMock = MockRepository.GenerateMock<IDomainObjectCollectionData>();
      dataDecoratorMock.Stub (mock => mock.GetUndecoratedDataStore()).Return (dataStore);

      var collection = new DomainObjectCollection (dataDecoratorMock);
      IDomainObjectCollectionData nonNotifyingData = GetNonNotifyingData (collection);

      nonNotifyingData.Insert (0, _customer1);
      dataDecoratorMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));

      Assert.That (dataStore.ToArray(), Is.EqualTo (new[] { _customer1 }));
    }

    [Test]
    public void IList_IsFixedSize ()
    {
      var collection = new DomainObjectCollection();
      Assert.That (((IList) collection).IsFixedSize, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IndexerWithNullObjectID ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));

      Dev.Null = collection[null];
    }

    [Test]
    public void IndexOf ()
    {
      Assert.That (_collection.IndexOf (_customer1), Is.EqualTo (0));
      Assert.That (_collection.IndexOf (_customer2.ID), Is.EqualTo (1));
    }

    [Test]
    public void IndexOf_ObjectWithameID_FromOtherTransaction ()
    {
      DomainObject customer2FromOtherTransaction = new ClientTransactionMock().GetObject (DomainObjectIDs.Customer2);
      Assert.That (_collection.IndexOf (customer2FromOtherTransaction.ID), Is.EqualTo (1));
      Assert.That (_collection.IndexOf (customer2FromOtherTransaction), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOfForObjectOfInvalidType ()
    {
      IList list = _collection;
      Assert.That (list.IndexOf (new object ()), Is.EqualTo (-1));
    }

    [Test]
    public void IndexOfNullObject ()
    {
      Assert.That (_collection.IndexOf ((DomainObject) null), Is.EqualTo (-1));
      Assert.That (_collection.IndexOf ((ObjectID) null), Is.EqualTo (-1));
    }

    [Test]
    public void Initialization_StandaloneVariants_Strategies ()
    {
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (new DomainObjectCollection(), null);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (new DomainObjectCollection (typeof (Order)), typeof (Order));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (new DomainObjectCollection (_collection, true), typeof (Customer));
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (
          new DomainObjectCollection (new[] { _customer1, _customer2 }, typeof (Customer), false), typeof (Customer));
    }

    [Test]
    public void Initialization_WithData ()
    {
      var givenData = new DomainObjectCollectionData();
      var collection = new DomainObjectCollection (givenData);

      var actualData = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (collection);
      Assert.That (actualData, Is.SameAs (givenData));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException), ExpectedMessage =
        "Item 0 of argument domainObjects has the type Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer instead of "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.")]
    public void Initialization_WithEnumerable_ChecksItems ()
    {
      new DomainObjectCollection (new[] { _customer1 }, typeof (Order), false);
    }

    [Test]
    public void Insert ()
    {
      _collection.Insert (1, _customer3NotInCollection);

      Assert.That (_collection.Count, Is.EqualTo (3));
      Assert.That (_collection.IndexOf (_customer1), Is.EqualTo (0));
      Assert.That (_collection.IndexOf (_customer3NotInCollection.ID), Is.EqualTo (1));
      Assert.That (_collection.IndexOf (_customer2), Is.EqualTo (2));

      var exptectedOrderedDomainObject = new DomainObject[] { _customer1, _customer3NotInCollection, _customer2 };
      CheckForEachOrder (exptectedOrderedDomainObject, _collection);
    }

    [Test]
    public void Insert_AfterLastElement ()
    {
      _collection.Insert (_collection.Count, _customer3NotInCollection);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Insert_ReadOnly_Throws ()
    {
      new DomainObjectCollection (_collection, true).Insert (0, _customer3NotInCollection);
    }

    [Test]
    public void Insert_WithIndexGreaterThanCollectionSize ()
    {
      try
      {
        _collection.Insert (_collection.Count + 1, _customer3NotInCollection);
        Assert.Fail ("Insert with wrong index did not raise an exception.");
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      Assert.That (_collection.Contains (_customer3NotInCollection.ID), Is.False);
    }

    [Test]
    public void Insert_WithNegativeIndex ()
    {
      try
      {
        _collection.Insert (-1, _customer3NotInCollection);
        Assert.Fail ("Insert with wrong index did not raise an exception.");
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      Assert.That (_collection.Contains (_customer3NotInCollection.ID), Is.False);
    }


    [Test]
    public void Insert_Events ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));

      var eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, false);

      collection.Insert (0, _customer1);

      Assert.That (collection.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer1));
      Assert.That (eventReceiver.AddedDomainObject, Is.SameAs (_customer1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Insert_ObjectOfInvalidType ()
    {
      IList list = _collection;
      list.Insert (0, new object());
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor' cannot be added to this collection. "
                          + "Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' "
                          + "or derived from 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer'.\r\nParameter name: domainObject")]
    public void InvalidDerivedType ()
    {
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      var customerCollection = new DomainObjectCollection (typeof (Customer));
      customerCollection.Add (distributor);
    }

    [Test]
    public void Remove ()
    {
      _collection.Remove (_customer2.ID);

      Assert.That (_collection.Count, Is.EqualTo (1), "Item count");
      Assert.That (_collection[_customer1.ID], Is.SameAs (_customer1), "Customer 1");

      _collection.Remove (_customer2.ID);
    }

    [Test]
    public void RemoveEvents ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, false);

      _collection.Remove (_customer1.ID);

      Assert.That (_collection.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.RemovingDomainObjects[0], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovedDomainObjects[0], Is.SameAs (_customer1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void RemoveFromReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      var readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Remove (DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void RemoveNullDomainObjectWithEvents ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((DomainObject) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void RemoveNullObjectIDWithEvents ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((ObjectID) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void RemoveObject ()
    {
      Assert.That (_collection.Remove (_customer2), Is.True);

      Assert.That (_collection.Count, Is.EqualTo (1), "Item count");
      Assert.That (_collection[_customer1.ID], Is.SameAs (_customer1), "Customer 1");

      Assert.That (_collection.Remove (_customer2), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object to be removed has the same ID as an object in this collection, but "
                                                                      + "is a different object reference.\r\nParameter name: domainObject")]
    public void RemoveObjectFromOtherTransaction_WhoseIDIsInCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      collection.Add (customer);

      Customer customerInOtherTx;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        customerInOtherTx = Customer.GetObject (customer.ID);
      }

      collection.Remove (customerInOtherTx);

      Assert.That (collection.ContainsObject (customer), Is.True);
    }

    [Test]
    public void RemoveObjectNotInCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      collection.Remove (customer);

      // expectation: no exception
    }

    [Test]
    public void RemoveObjectNotInCollectionWithEvents ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      collection.Remove (customer);

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void RemoveObjectOfInvalidType ()
    {
      int count = _collection.Count;

      IList list = _collection;
      list.Remove (new object());

      Assert.That (_collection.Count, Is.EqualTo (count));
    }

    [Test]
    public void RemoveOverNumericIndex ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      collection.Add (customer);

      Assert.That (collection.Count, Is.EqualTo (1));
      Assert.That (collection[0], Is.SameAs (customer));

      collection.RemoveAt (0);

      Assert.That (collection.Count, Is.EqualTo (0));
    }

    [Test]
    public void RemoveWithDiscardedObject ()
    {
      var domainObjectCollection = new DomainObjectCollection();
      Customer customer = Customer.NewObject();
      domainObjectCollection.Add (customer);
      customer.Delete();
      Assert.That (customer.IsDiscarded, Is.True);

      //The next line throws an ObjectDiscardedException:
      domainObjectCollection.Remove (customer);

      Assert.IsEmpty (domainObjectCollection);
    }

    [Test]
    public void RemoveWithObjectIDNotInCollection ()
    {
      int oldCount = _collection.Count;

      _collection.Remove (DomainObjectIDs.Customer4);

      Assert.That (_collection.Count, Is.EqualTo (oldCount));
    }

    [Test]
    public void ReplaceItems_Content ()
    {
      var sourceCollection = new DomainObjectCollection { _customer3NotInCollection };
      ReplaceItems (_collection, sourceCollection);

      Assert.That (_collection, Is.EqualTo (sourceCollection));
    }

    [Test]
    public void ReplaceItems_DecoratorChain_Kept ()
    {
      var originalData = new DomainObjectCollectionData();
      var collection = new DomainObjectCollection (originalData);

      var sourceCollection = new DomainObjectCollection { _customer3NotInCollection };
      ReplaceItems (collection, sourceCollection);

      var data = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<IDomainObjectCollectionData> (collection);
      Assert.That (data, Is.SameAs (originalData));
    }

    [Test]
    public void ReplaceItems_ReadOnlyness_Kept ()
    {
      var readOnlyCollection = new DomainObjectCollection (_collection, true);
      var sourceCollection = new DomainObjectCollection { _customer3NotInCollection };
      ReplaceItems (readOnlyCollection, sourceCollection);

      Assert.That (readOnlyCollection, Is.EqualTo (sourceCollection));
      Assert.That (readOnlyCollection.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than the size of the collection."
                          + "\r\nParameter name: index\r\nActual value was 2.")]
    public void SetInvalidNumericIndex ()
    {
      _collection[_collection.Count] = Customer.NewObject();
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetNumericIndexerWithInvalidType ()
    {
      _collection[0] = Order.NewObject();
    }
  }
}