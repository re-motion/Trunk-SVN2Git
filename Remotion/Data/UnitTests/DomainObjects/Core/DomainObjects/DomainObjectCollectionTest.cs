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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    private DomainObjectCollection _collection;
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
      _customer3NotInCollection = Customer.GetObject (DomainObjectIDs.Customer3);

      _collection = CreateCustomerCollection ();
    }

    [Test]
    public void ExactType ()
    {
      ObjectID id1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
      ObjectID id2 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));

      ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (id1);
      ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (id2);

      DomainObjectCollection domainObjectCollection = new DomainObjectCollection (typeof (ClassWithAllDataTypes));
      domainObjectCollection.Add (c1);
      domainObjectCollection.Add (c2);

      Assert.AreEqual (2, domainObjectCollection.Count, "Count");
      CheckKeys (new ObjectID[] { id1, id2 }, domainObjectCollection);
      Assert.AreSame (c1, domainObjectCollection[id1], "ClassWithAllDataTypes1");
      Assert.AreSame (c2, domainObjectCollection[id2], "ClassWithAllDataTypes2");
    }

    [Test]
    public void DerivedType ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Company1);
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      DomainObjectCollection domainObjectCollection = new DomainObjectCollection (typeof (Company));
      domainObjectCollection.Add (company);
      domainObjectCollection.Add (partner);
      domainObjectCollection.Add (distributor);

      Assert.AreEqual (3, domainObjectCollection.Count, "Count");

      CheckKeys (new ObjectID[] { DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2 },
          domainObjectCollection);

      Assert.AreSame (company, domainObjectCollection[DomainObjectIDs.Company1], "Company");
      Assert.AreSame (partner, domainObjectCollection[DomainObjectIDs.Partner1], "Partner");
      Assert.AreSame (distributor, domainObjectCollection[DomainObjectIDs.Distributor2], "Distributor");
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage = "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Distributor' cannot be added to this collection. "
        + "Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' "
        + "or derived from 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer'.\r\nParameter name: domainObject")]
    public void InvalidDerivedType ()
    {
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      DomainObjectCollection customerCollection = new DomainObjectCollection (typeof (Customer));
      customerCollection.Add (distributor);
    }

    [Test]
    public void CopyConstructorWithDerivedType ()
    {
      Company company = Company.GetObject (DomainObjectIDs.Company1);
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

      DomainObjectCollection domainObjectCollection1 = new DomainObjectCollection (typeof (Company));
      domainObjectCollection1.Add (company);
      domainObjectCollection1.Add (partner);
      domainObjectCollection1.Add (distributor);

      DomainObjectCollection domainObjectCollection2 = new DomainObjectCollection (
          domainObjectCollection1, true);

      Assert.AreEqual (3, domainObjectCollection2.Count, "Count");

      CheckKeys (new ObjectID[] { DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2 },
          domainObjectCollection2);

      Assert.AreSame (company, domainObjectCollection2[DomainObjectIDs.Company1], "Company");
      Assert.AreSame (partner, domainObjectCollection2[DomainObjectIDs.Partner1], "Partner");
      Assert.AreSame (distributor, domainObjectCollection2[DomainObjectIDs.Distributor2], "Distributor");
    }

    private void CheckKeys (ObjectID[] expectedKeys, DomainObjectCollection collection)
    {
      Assert.AreEqual (expectedKeys.Length, collection.Count, "DomainObjectCollection.Count");
      foreach (ObjectID expectedKey in expectedKeys)
        Assert.IsTrue (collection.Contains (expectedKey), string.Format ("Key {0}", expectedKey));
    }

    [Test]
    public void ContainsObjectIDTrue ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      DomainObjectCollection customers = new DomainObjectCollection ();

      customers.Add (customer);

      Assert.IsTrue (customers.Contains (customer.ID));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsObjectIDNull ()
    {
      DomainObjectCollection customers = new DomainObjectCollection ();

      customers.Contains ((ObjectID) null);
    }

    [Test]
    public void ContainsDomainObjectTrue ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      DomainObjectCollection customers = new DomainObjectCollection ();

      customers.Add (customer);

      Assert.IsTrue (customers.ContainsObject (customer));
    }

    [Test]
    public void ContainsDomainObjectFalse ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      DomainObjectCollection customers = new DomainObjectCollection ();

      customers.Add (customer);

      Customer copy;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        copy = Customer.GetObject (customer.ID);
      }

      Assert.IsFalse (customers.ContainsObject (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsDomainObjectNull ()
    {
      DomainObjectCollection customers = new DomainObjectCollection ();

      customers.ContainsObject ((DomainObject) null);
    }

    [Test]
    public void CloneRequiredItemType ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
      DomainObjectCollection clone = (DomainObjectCollection) collection.Clone ();

      Assert.IsNotNull (clone, "Clone does not exist");
      Assert.AreEqual (typeof (Order), clone.RequiredItemType, "Required item type does not match.");
    }

    [Test]
    public void Remove ()
    {
      _collection.Remove (_customer2.ID);

      Assert.AreEqual (1, _collection.Count, "Item count");
      Assert.AreSame (_customer1, _collection[_customer1.ID], "Customer 1");

      _collection.Remove (_customer2.ID);
    }

    [Test]
    public void RemoveObject ()
    {
      Assert.IsTrue (_collection.Remove (_customer2));

      Assert.AreEqual (1, _collection.Count, "Item count");
      Assert.AreSame (_customer1, _collection[_customer1.ID], "Customer 1");

      Assert.IsFalse (_collection.Remove (_customer2));
    }

    [Test]
    public void RemoveOverNumericIndex ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      collection.Add (customer);

      Assert.AreEqual (1, collection.Count);
      Assert.AreSame (customer, collection[0]);

      collection.RemoveAt (0);

      Assert.AreEqual (0, collection.Count);
    }

    [Test]
    public void RemoveObjectNotInCollection ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      collection.Remove (customer);

      // expectation: no exception
    }

    [Test]
    public void Clear ()
    {
      _collection.Clear ();

      Assert.AreEqual (0, _collection.Count, "Item count");
    }

    [Test]
    public void AddEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, false);

      collection.Add (_customer1);

      Assert.AreEqual (1, collection.Count);
      Assert.AreEqual (true, eventReceiver.HasAddingEventBeenCalled);
      Assert.AreEqual (true, eventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_customer1, eventReceiver.AddingDomainObject);
      Assert.AreSame (_customer1, eventReceiver.AddedDomainObject);
    }

    [Test]
    public void CancelAddEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, true);

      try
      {
        collection.Add (_customer1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (0, collection.Count);
        Assert.AreEqual (true, eventReceiver.HasAddingEventBeenCalled);
        Assert.AreEqual (false, eventReceiver.HasAddedEventBeenCalled);
        Assert.AreSame (_customer1, eventReceiver.AddingDomainObject);
        Assert.IsNull (eventReceiver.AddedDomainObject);
      }
    }

    [Test]
    public void RemoveEvents ()
    {
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, false);

      _collection.Remove (_customer1.ID);

      Assert.AreEqual (1, _collection.Count);
      Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
      Assert.AreEqual (true, eventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (1, eventReceiver.RemovingDomainObjects.Count);
      Assert.AreEqual (1, eventReceiver.RemovedDomainObjects.Count);
      Assert.AreSame (_customer1, eventReceiver.RemovingDomainObjects[0]);
      Assert.AreSame (_customer1, eventReceiver.RemovedDomainObjects[0]);
    }

    [Test]
    public void CancelRemoveEvents ()
    {
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Remove (_customer1.ID);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (2, _collection.Count);
        Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
        Assert.AreEqual (false, eventReceiver.HasRemovedEventBeenCalled);
        Assert.AreEqual (1, eventReceiver.RemovingDomainObjects.Count);
        Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
        Assert.AreSame (_customer1, eventReceiver.RemovingDomainObjects[0]);
      }
    }

    [Test]
    public void ClearEvents ()
    {
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, false);

      _collection.Clear ();

      Assert.AreEqual (0, _collection.Count);
      Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
      Assert.AreEqual (true, eventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (2, eventReceiver.RemovingDomainObjects.Count);
      Assert.AreEqual (2, eventReceiver.RemovedDomainObjects.Count);
      Assert.AreSame (_customer1, eventReceiver.RemovingDomainObjects[_customer1.ID]);
      Assert.AreSame (_customer2, eventReceiver.RemovingDomainObjects[_customer2.ID]);
      Assert.AreSame (_customer1, eventReceiver.RemovedDomainObjects[_customer1.ID]);
      Assert.AreSame (_customer2, eventReceiver.RemovedDomainObjects[_customer2.ID]);
    }

    [Test]
    public void CancelClearEvents ()
    {
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
          _collection, true);

      try
      {
        _collection.Clear ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (2, _collection.Count);
        Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
        Assert.AreEqual (false, eventReceiver.HasRemovedEventBeenCalled);
        Assert.AreEqual (1, eventReceiver.RemovingDomainObjects.Count);
        Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
      }
    }

    [Test]
    public void CompareTrue ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      Assert.IsTrue (DomainObjectCollection.Compare (collection, collection));
    }

    [Test]
    public void CompareFalse ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
    }

    [Test]
    public void CompareFalseWithSameCount ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
    }

    [Test]
    public void CompareFalseWithDifferentOrder ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
    }

    [Test]
    public void CompareTrueWithSameOrder ()
    {
      DomainObjectCollection collection1 = CreateCustomerCollection ();
      DomainObjectCollection collection2 = CreateCustomerCollection ();

      Assert.IsTrue (DomainObjectCollection.Compare (collection1, collection2));
    }

    [Test]
    public void CompareFalseWithCollection1Null ()
    {
      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

      Assert.IsFalse (DomainObjectCollection.Compare (null, collection2));
    }

    [Test]
    public void CompareFalseWithCollection2Null ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));

      Assert.IsFalse (DomainObjectCollection.Compare (collection1, null));
    }

    [Test]
    public void CompareTrueWithBothCollectionsNull ()
    {
      Assert.IsTrue (DomainObjectCollection.Compare (null, null));
    }

    [Test]
    public void CompareWithIgnoreFalse ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2, false));
    }

    [Test]
    public void CompareWithIgnoreTrue ()
    {
      DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
      collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

      DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
      collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

      Assert.IsTrue (DomainObjectCollection.Compare (collection1, collection2, true));
    }

    [Test]
    public void RemoveObjectNotInCollectionWithEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      collection.Remove (customer);

      Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
      Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
    }

    [Test]
    public void RemoveNullObjectIDWithEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((ObjectID) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
      Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
    }

    [Test]
    public void RemoveNullDomainObjectWithEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((DomainObject) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
      Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IndexerWithNullObjectID ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

      DomainObject domainObject = collection[null];
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
    [ExpectedException (typeof (NotSupportedException))]
    public void RemoveFromReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Remove (DomainObjectIDs.OrderItem1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddToReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem3));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ClearReadOnlyCollection ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

      readOnlyCollection.Clear ();
    }

    [Test]
    public void CopyToArray ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem[] items = new OrderItem[order.OrderItems.Count];

      order.OrderItems.CopyTo (items, 0);

      Assert.AreSame (order.OrderItems[0], items[0]);
      Assert.AreSame (order.OrderItems[1], items[1]);
    }

    [Test]
    public void CopyToEmptyArrayWithEmptyCollection ()
    {
      DomainObjectCollection emptyCollection = new DomainObjectCollection ();
      DomainObject[] array = new DomainObject[0];

      emptyCollection.CopyTo (array, 0);

      // expectation: no exception
    }

    [Test]
    public void CopyToNonEmptyArrayWithEmptyCollection ()
    {
      DomainObjectCollection emptyCollection = new DomainObjectCollection ();
      DomainObject[] array = new DomainObject[1];

      emptyCollection.CopyTo (array, 0);

      Assert.IsNull (array[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The number of items in the source collection is greater than the available space from index to the end of the destination array.\r\nParameter name: index")]
    public void CopyToArraySmallerThanCollection ()
    {
      DomainObject[] array = new DomainObject[_collection.Count - 1];

      _collection.CopyTo (array, 0);
    }

    [Test]
    public void CopyToArrayWithIndex ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem[] items = new OrderItem[order.OrderItems.Count + 1];

      OrderItem otherItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      items[0] = otherItem;

      order.OrderItems.CopyTo (items, 1);

      Assert.AreSame (otherItem, items[0]);
      Assert.AreSame (order.OrderItems[0], items[1]);
      Assert.AreSame (order.OrderItems[1], items[2]);
    }

    [Test]
    public void Clone ()
    {
      ICloneable cloneableCollection = (ICloneable) _collection;
      DomainObjectCollection clonedCollection = (DomainObjectCollection) cloneableCollection.Clone ();

      Assert.IsNotNull (clonedCollection);
      Assert.AreEqual (_collection.Count, clonedCollection.Count);
      Assert.AreEqual (_collection.IsReadOnly, clonedCollection.IsReadOnly);
      Assert.AreEqual (_collection.RequiredItemType, clonedCollection.RequiredItemType);
      Assert.AreSame (_collection[0], clonedCollection[0]);
      Assert.AreSame (_collection[1], clonedCollection[1]);
    }

    [Test]
    public void CloneReadOnlyCollection ()
    {
      DomainObjectCollection readOnlyCollection = new DomainObjectCollection (_collection, true);

      DomainObjectCollection clone = (DomainObjectCollection) readOnlyCollection.Clone ();

      Assert.IsNotNull (clone, "Clone does not exist");
      Assert.AreEqual (2, clone.Count, "Item count of clone");
      Assert.AreSame (_customer1, clone[_customer1.ID], "Customer1");
      Assert.AreSame (_customer2, clone[_customer2.ID], "Customer2");
      Assert.AreEqual (true, clone.IsReadOnly, "IsReadOnly");
    }

    [Test]
    public void CloneDerivedCollection ()
    {
      OrderCollection orderCollection = new OrderCollection ();
      OrderCollection clone = (OrderCollection) orderCollection.Clone ();

      Assert.AreEqual (typeof (OrderCollection), clone.GetType ());
      Assert.AreEqual (orderCollection.RequiredItemType, clone.RequiredItemType);
    }

    [Test]
    public void IsFixedSize ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      Assert.IsFalse (collection.IsFixedSize);
    }

    [Test]
    public void IndexOf ()
    {
      Assert.AreEqual (0, _collection.IndexOf (_customer1));
      Assert.AreEqual (1, _collection.IndexOf (_customer2.ID));
    }

    [Test]
    public void IndexOfNullObject ()
    {
      Assert.AreEqual (-1, _collection.IndexOf ((DomainObject) null));
      Assert.AreEqual (-1, _collection.IndexOf ((ObjectID) null));
    }

    [Test]
    public void Insert ()
    {
      _collection.Insert (1, _customer3NotInCollection);

      Assert.AreEqual (3, _collection.Count);
      Assert.AreEqual (0, _collection.IndexOf (_customer1));
      Assert.AreEqual (1, _collection.IndexOf (_customer3NotInCollection.ID));
      Assert.AreEqual (2, _collection.IndexOf (_customer2));

      DomainObject[] exptectedOrderedDomainObject = new DomainObject[] { _customer1, _customer3NotInCollection, _customer2 };
      CheckForEachOrder (exptectedOrderedDomainObject, _collection);
    }

    [Test]
    public void InsertWithNegativeIndex ()
    {
      try
      {
        _collection.Insert (-1, _customer3NotInCollection);
        Assert.Fail ("Insert with wrong index did not raise an exception.");
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      Assert.IsFalse (_collection.Contains (_customer3NotInCollection.ID));
    }

    [Test]
    public void InsertWithIndexGreaterThanCollectionSize ()
    {
      try
      {
        _collection.Insert (_collection.Count + 1, _customer3NotInCollection);
        Assert.Fail ("Insert with wrong index did not raise an exception.");
      }
      catch (ArgumentOutOfRangeException)
      {
      }

      Assert.IsFalse (_collection.Contains (_customer3NotInCollection.ID));
    }

    [Test]
    public void InsertAfterLastElement ()
    {
      _collection.Insert (_collection.Count, _customer3NotInCollection);
    }

    [Test]
    public void AccessIListInterfaceWithDomainObject ()
    {
      IList list = (IList) _collection;

      Assert.IsTrue (list.Contains (_customer1));
      Assert.AreEqual (0, list.IndexOf (_customer1));
      Assert.AreSame (_customer1, list[0]);

      list.Remove (_customer1);
      Assert.IsFalse (list.Contains (_customer1));

      Assert.AreEqual (list.Count, list.Add (_customer1));
      Assert.AreEqual (list.Count - 1, list.IndexOf (_customer1));

      list.Insert (0, _customer3NotInCollection);
      Assert.AreEqual (0, list.IndexOf (_customer3NotInCollection));
    }

    [Test]
    public void AccessIListInterfaceWithObjectID ()
    {
      IList list = (IList) _collection;

      Assert.IsTrue (list.Contains (_customer1.ID));
      Assert.AreEqual (0, list.IndexOf (_customer1.ID));

      list.Remove (_customer1.ID);
      Assert.IsFalse (list.Contains (_customer1));
    }

    [Test]
    public void RemoveObjectOfInvalidType ()
    {
      int count = _collection.Count;

      IList list = (IList) _collection;
      list.Remove (new object ());

      Assert.AreEqual (count, _collection.Count);
    }

    [Test]
    public void ContainsObjectOfInvalidType ()
    {
      IList list = (IList) _collection;
      Assert.IsFalse (list.Contains (new object ()));
    }

    [Test]
    public void IndexOfForObjectOfInvalidType ()
    {
      IList list = (IList) _collection;
      Assert.AreEqual (-1, list.IndexOf (new object ()));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void InsertObjectOfInvalidType ()
    {
      IList list = (IList) _collection;
      list.Insert (0, new object ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void AddObjectOfInvalidType ()
    {
      IList list = (IList) _collection;
      list.Add (new object ());
    }

    [Test]
    public void ChangeObjectThroughIListInterface ()
    {
      IList list = (IList) _collection;
      list[0] = _customer3NotInCollection;

      Assert.AreSame (_customer3NotInCollection, list[0]);
      Assert.AreEqual (2, list.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void ChangeToObjectOfInvalidType ()
    {
      IList list = (IList) _collection;
      list[0] = new object ();
    }

    [Test]
    public void InsertEvents ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

      DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, false);

      collection.Insert (0, _customer1);

      Assert.AreEqual (1, collection.Count);
      Assert.AreEqual (true, eventReceiver.HasAddingEventBeenCalled);
      Assert.AreEqual (true, eventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_customer1, eventReceiver.AddingDomainObject);
      Assert.AreSame (_customer1, eventReceiver.AddedDomainObject);
    }

    [Test]
    public void ChangeEvents ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_collection);

      _collection[0] = _customer3NotInCollection;

      ChangeState[] expectedStates = new ChangeState[] 
    {
      new CollectionChangeState (_collection, _customer1, "1. Removing event"),
      new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event"),
      new CollectionChangeState (_collection, _customer1, "3. Removed event"),
      new CollectionChangeState (_collection, _customer3NotInCollection, "4. Added event")
    };

      Assert.AreSame (_customer3NotInCollection, _collection[0]);
      Assert.AreEqual (2, _collection.Count);

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ChangeEventsWithRemovalCancelled ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_collection, 1);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new CollectionChangeState (_collection, _customer1, "1. Removing event") };

        Assert.AreSame (_customer1, _collection[0]);
        Assert.AreEqual (2, _collection.Count);

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void ChangeEventsWithAdditionCancelled ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_collection, 2);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] 
            {
              new CollectionChangeState (_collection, _customer1, "1. Removing event"),
              new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event")
            };

        Assert.AreSame (_customer1, _collection[0]);
        Assert.AreEqual (2, _collection.Count);

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than the size of the collection."
        + "\r\nParameter name: index\r\nActual value was 2.")]
    public void SetInvalidNumericIndex ()
    {
      _collection[_collection.Count] = Customer.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetInvalidNumericIndex ()
    {
      Customer customer = (Customer) _collection[_collection.Count];
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetNumericIndexerWithInvalidType ()
    {
      _collection[0] = Order.NewObject ();
    }

    [Test]
    public void CreateWithItemType ()
    {
      DomainObjectCollection collection = DomainObjectCollection.Create (
          typeof (DomainObjectCollection), new DataContainerCollection (), typeof (Order));

      Assert.IsNotNull (collection);
      Assert.AreEqual (typeof (Order), collection.RequiredItemType);
    }

    [Test]
    public void GetItemsNotInCollection ()
    {
      DomainObjectCollection collection2 = new DomainObjectCollection ();
      collection2.Add (_customer1);

      DomainObjectCollection itemsNotInCollection = collection2.GetItemsNotInCollection (_collection);

      Assert.AreEqual (1, itemsNotInCollection.Count);
      Assert.AreSame (_customer2, itemsNotInCollection[0]);
    }

    [Test]
    public void GetItemsNotInCollectionBothCollectionsEmpty ()
    {
      DomainObjectCollection collection = new DomainObjectCollection ();
      DomainObjectCollection itemsNotInCollection = collection.GetItemsNotInCollection (new DomainObjectCollection ());
      Assert.AreEqual (0, itemsNotInCollection.Count);
    }

    [Test]
    public void GetItemsNotInCollectionForEmptyCollection ()
    {
      DomainObjectCollection collection2 = new DomainObjectCollection ();

      DomainObjectCollection itemsNotInCollection = collection2.GetItemsNotInCollection (_collection);

      Assert.AreEqual (2, itemsNotInCollection.Count);
      Assert.IsTrue (itemsNotInCollection.ContainsObject (_customer1));
      Assert.IsTrue (itemsNotInCollection.ContainsObject (_customer2));
    }

    [Test]
    public void Combine ()
    {
      DomainObjectCollection secondCollection = new DomainObjectCollection (_collection, false);
      secondCollection.Add (Customer.GetObject (DomainObjectIDs.Customer3));

      _collection.Combine (secondCollection);

      Assert.AreEqual (3, _collection.Count);
      Assert.IsTrue (_collection.ContainsObject (_customer1));
      Assert.IsTrue (_collection.ContainsObject (_customer2));
      Assert.IsTrue (_collection.Contains (DomainObjectIDs.Customer3));
      Assert.IsFalse (_collection.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CombineWithItemOfInvalidType ()
    {
      DomainObjectCollection secondCollection = new DomainObjectCollection ();
      secondCollection.Add (Order.GetObject (DomainObjectIDs.Order1));

      _collection.Combine (secondCollection);
    }

    [Test]
    public void CloneOrderCollection ()
    {
      OrderCollection orders = new OrderCollection ();
      orders.Add (Order.GetObject (DomainObjectIDs.Order1));

      DomainObjectCollection clonedOrders = orders.Clone (true);

      Assert.AreEqual (typeof (OrderCollection), clonedOrders.GetType ());
      Assert.AreEqual (orders.Count, clonedOrders.Count);
      Assert.AreEqual (orders.RequiredItemType, clonedOrders.RequiredItemType);
    }

    [Test]
    public void RemoveWithObjectIDNotInCollection ()
    {
      int oldCount = _collection.Count;

      _collection.Remove (DomainObjectIDs.Customer4);

      Assert.AreEqual (oldCount, _collection.Count);
    }

    [Test]
    public void ClearWithDiscardedObject ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      domainObjectCollection.Add (customer);
      customer.Delete ();
      Assert.IsTrue (customer.IsDiscarded);

      //The next line throws an ObjectDiscardedException:
      domainObjectCollection.Clear ();

      Assert.IsEmpty (domainObjectCollection);
    }

    [Test]
    public void RemoveWithDiscardedObject ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      domainObjectCollection.Add (customer);
      customer.Delete ();
      Assert.IsTrue (customer.IsDiscarded);

      //The next line throws an ObjectDiscardedException:
      domainObjectCollection.Remove (customer);

      Assert.IsEmpty (domainObjectCollection);
    }

    private DomainObjectCollection CreateCustomerCollection ()
    {
      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (_customer1);
      collection.Add (_customer2);

      return collection;
    }

    private void CheckForEachOrder (DomainObject[] exptectedDomainObjects, DomainObjectCollection collection)
    {
      Assert.AreEqual (exptectedDomainObjects.Length, collection.Count);

      int i = 0;
      foreach (DomainObject domainObject in collection)
      {
        Assert.AreSame (exptectedDomainObjects[i], domainObject);
        i++;
      }
    }

    public interface IEventHandlerReceiver
    {
      void Added (object sender, DomainObjectCollectionChangeEventArgs args);
      void Adding (object sender, DomainObjectCollectionChangeEventArgs args);
      void Removed (object sender, DomainObjectCollectionChangeEventArgs args);
      void Removing (object sender, DomainObjectCollectionChangeEventArgs args);
    }

    [Test]
    public void CopyEventHandlers ()
    {
      MockRepository mockRepository = new MockRepository ();
      IEventHandlerReceiver eventReceiver1 = mockRepository.StrictMock<IEventHandlerReceiver> ();
      IEventHandlerReceiver eventReceiver2 = mockRepository.StrictMock<IEventHandlerReceiver> ();

      DomainObjectCollection source = new DomainObjectCollection ();
      DomainObjectCollection destination = new DomainObjectCollection ();

      source.Added += eventReceiver1.Added;
      source.Added += eventReceiver2.Added;
      source.Adding += eventReceiver1.Adding;
      source.Adding += eventReceiver2.Adding;
      source.Removed += eventReceiver1.Removed;
      source.Removed += eventReceiver2.Removed;
      source.Removing += eventReceiver1.Removing;
      source.Removing += eventReceiver2.Removing;

      using (mockRepository.Ordered ())
      {
        // expectations
        eventReceiver1.Adding (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull());
        eventReceiver2.Adding (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull ());

        eventReceiver1.Added (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull());
        eventReceiver2.Added (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull ());

        eventReceiver1.Removing (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull());
        eventReceiver2.Removing (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull ());

        eventReceiver1.Removed (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull());
        eventReceiver2.Removed (null, null);
        LastCall.Constraints (Mocks_Is.Same (destination), Mocks_Is.NotNull ());
      }

      mockRepository.ReplayAll ();

      PrivateInvoke.InvokeNonPublicMethod (destination, "CopyEventHandlersFrom", source);

      destination.Add (_customer1);
      destination.Remove (_customer1);

      mockRepository.VerifyAll ();
    }
  }
}
