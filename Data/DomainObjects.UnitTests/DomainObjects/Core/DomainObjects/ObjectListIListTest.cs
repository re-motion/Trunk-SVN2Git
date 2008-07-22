/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class ObjectListIListTest : ClientTransactionBaseTest
  {
    private Order _order;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;
    private IList<OrderItem> _orderItemList;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = Order.GetObject (DomainObjectIDs.Order1);
      _orderItem1 = _order.OrderItems[0];
      _orderItem2 = _order.OrderItems[1];
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      _orderItem4 = OrderItem.GetObject (DomainObjectIDs.OrderItem4);
      _orderItemList = _order.OrderItems;
    }

    [Test]
    public void ObjectListIsIList ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      Assert.IsInstanceOfType (typeof (IList<OrderItem>), list);
    }

    [Test]
    public void IndexOf ()
    {
      IList<OrderItem> emptyList = new ObjectList<OrderItem> ();
      Assert.AreEqual (-1, emptyList.IndexOf (_orderItem1));
      Assert.AreEqual (0, _orderItemList.IndexOf (_orderItem1));
      Assert.AreEqual (1, _orderItemList.IndexOf (_orderItem2));
      Assert.AreEqual (-1, _orderItemList.IndexOf (_orderItem3));
    }

    [Test]
    public void Insert ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Insert (0, _orderItem2);
      Assert.That (list, Is.EqualTo (new OrderItem[] { _orderItem2 }));
      list.Insert (0, _orderItem1);
      Assert.That (list, Is.EqualTo (new OrderItem[] { _orderItem1, _orderItem2 }));
      list.Insert (1, _orderItem3);
      Assert.That (list, Is.EqualTo (new OrderItem[] { _orderItem1, _orderItem3, _orderItem2 }));
      list.Insert (3, _orderItem4);
      Assert.That (list, Is.EqualTo (new OrderItem[] { _orderItem1, _orderItem3, _orderItem2, _orderItem4 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot insert an item into a read-only collection.")]
    public void InsertThrowsIfReadOnly ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> (new ObjectList<OrderItem> (), true);
      readOnlyList.Insert (0, _orderItem2);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than or equal to the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void InsertThrowsOnWrongIndex ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Insert (1, _orderItem2);
    }

    [Test]
    public void Item ()
    {
      Assert.AreSame (_orderItem1, _orderItemList[0]);
      Assert.AreSame (_orderItem2, _orderItemList[1]);

      Assert.That (_orderItemList, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));

      _orderItemList[0] = _orderItem3;

      Assert.That (_orderItemList, Is.EqualTo (new object[] { _orderItem3, _orderItem2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index was out of range. Must be non-negative and less than the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void ItemGetThrowsOnWrongIndex ()
    {
      Dev.Null = _orderItemList[2];
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void ItemSetThrowsOnWrongIndex ()
    {
      _orderItemList[-1] = null;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot modify a read-only collection.")]
    public void ItemSetThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>)_orderItemList, true);
      readOnlyList[0] = null;
    }

    [Test]
    public void Add ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Add (_orderItem1);
      Assert.That (list, Is.EqualTo (new object[] { _orderItem1 }));
      list.Add (_orderItem2);
      Assert.That (list, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot add an item to a read-only collection.")]
    public void AddThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemList, true);
      readOnlyList.Add (_orderItem1);
    }

    [Test]
    public void Contains ()
    {
      Assert.IsTrue (_orderItemList.Contains (_orderItem1));
      Assert.IsTrue (_orderItemList.Contains (_orderItem2));
      Assert.IsFalse (_orderItemList.Contains (_orderItem3));
    }

    [Test]
    public void CopyToTightFit ()
    {
      OrderItem[] destination = new OrderItem[2];
      _orderItemList.CopyTo (destination, 0);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void CopyToLargeFit ()
    {
      OrderItem[] destination = new OrderItem[5];
      _orderItemList.CopyTo (destination, 0);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem2, null, null, null }));
      _orderItemList.CopyTo (destination, 1);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem1, _orderItem2, null, null}));
      _orderItemList.CopyTo (destination, 3);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem1, _orderItem2, _orderItem1, _orderItem2}));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index must be greater than or equal to zero.", MatchType = MessageMatch.Contains)]
    public void CopyToNegativeIndex ()
    {
      OrderItem[] destination = new OrderItem[5];
      _orderItemList.CopyTo (destination, -1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Index cannot be equal to or greater than the length of the array.",
        MatchType = MessageMatch.Contains)]
    public void CopyToGreatIndex ()
    {
      OrderItem[] destination = new OrderItem[5];
      _orderItemList.CopyTo (destination, 5);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The number of items in the source collection is greater than the available space from index to the end of the destination array.",
        MatchType = MessageMatch.Contains)]
    public void CopyToTooLittleSpace ()
    {
      OrderItem[] destination = new OrderItem[5];
      _orderItemList.CopyTo (destination, 4);
    }

    [Test]
    public void Remove ()
    {
      Assert.IsFalse (_orderItemList.Remove (_orderItem3));
      Assert.IsTrue (_orderItemList.Remove (_orderItem1));
      Assert.IsFalse (_orderItemList.Remove (_orderItem1));
      Assert.IsTrue (_orderItemList.Remove (_orderItem2));
      Assert.IsFalse (_orderItemList.Remove (_orderItem2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void RemoveThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemList, true);
      readOnlyList.Remove (_orderItem1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void RemoveInexistentThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemList, true);
      readOnlyList.Remove (_orderItem3);
    }

    [Test]
    public void GetEnumerator ()
    {
      IEnumerator<OrderItem> enumerator = new ObjectList<OrderItem> ().GetEnumerator();
      Assert.IsFalse (enumerator.MoveNext ());

      enumerator = _orderItemList.GetEnumerator ();
      Assert.IsTrue (enumerator.MoveNext ());
      Assert.AreSame (_orderItem1, enumerator.Current);
      Assert.IsTrue (enumerator.MoveNext ());
      Assert.AreSame (_orderItem2, enumerator.Current);
      Assert.IsFalse (enumerator.MoveNext ());
    }
  }
}
