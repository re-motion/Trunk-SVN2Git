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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class ObjectListTest : ClientTransactionBaseTest
  {
    private Order _order;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;
    private IList<OrderItem> _orderItemListAsIList;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = Order.GetObject (DomainObjectIDs.Order1);
      _orderItem1 = _order.OrderItems[0];
      _orderItem2 = _order.OrderItems[1];
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      _orderItem4 = OrderItem.GetObject (DomainObjectIDs.OrderItem4);
      _orderItemListAsIList = _order.OrderItems;
    }

    [Test]
    public void Initialization_WithIEnumerable_ReadOnlyFalse ()
    {
      var list = new ObjectList<OrderItem> (new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 }, false);
      Assert.That (list, Is.EqualTo (new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 }));
      Assert.That (list.IsReadOnly, Is.False);
    }

    [Test]
    public void Initialization_WithIEnumerable_ReadOnlyTrue ()
    {
      var list = new ObjectList<OrderItem> (new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 }, true);
      Assert.That (list, Is.EqualTo (new[] { _orderItem1, _orderItem2, _orderItem3, _orderItem4 }));
      Assert.That (list.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemDuplicateException), ExpectedMessage = "Item 1 of argument domainObjects is a duplicate "
        + "('OrderItem|ad620a11-4bc4-4791-bcf4-a0770a08c5b0|System.Guid').")]
    public void Initialization_WithEnumerableWithDuplicates ()
    {
      new ObjectList<OrderItem> (new[] { _orderItem1, _orderItem1 }, false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException), ExpectedMessage = "Item 0 of argument domainObjects is null.")]
    public void Initialization_WithEnumerableWithNulls ()
    {
      new ObjectList<OrderItem> (new OrderItem[] { null }, false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException), ExpectedMessage = "Item 2 of argument domainObjects is null.")]
    public void Initialization_WithEnumerableWithLaterNulls ()
    {
      new ObjectList<OrderItem> (new[] { _orderItem1, _orderItem2, null }, false);
    }

    [Test]
    public void ObjectList_IsIList ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      Assert.IsInstanceOfType (typeof (IList<OrderItem>), list);
    }

    [Test]
    public void IList_IndexOf ()
    {
      IList<OrderItem> emptyList = new ObjectList<OrderItem> ();
      Assert.AreEqual (-1, emptyList.IndexOf (_orderItem1));
      Assert.AreEqual (0, _orderItemListAsIList.IndexOf (_orderItem1));
      Assert.AreEqual (1, _orderItemListAsIList.IndexOf (_orderItem2));
      Assert.AreEqual (-1, _orderItemListAsIList.IndexOf (_orderItem3));
    }

    [Test]
    public void IList_Insert ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Insert (0, _orderItem2);
      Assert.That (list, Is.EqualTo (new[] { _orderItem2 }));
      list.Insert (0, _orderItem1);
      Assert.That (list, Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
      list.Insert (1, _orderItem3);
      Assert.That (list, Is.EqualTo (new[] { _orderItem1, _orderItem3, _orderItem2 }));
      list.Insert (3, _orderItem4);
      Assert.That (list, Is.EqualTo (new[] { _orderItem1, _orderItem3, _orderItem2, _orderItem4 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot insert an item into a read-only collection.")]
    public void IList_InsertThrowsIfReadOnly ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> (new ObjectList<OrderItem> (), true);
      readOnlyList.Insert (0, _orderItem2);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than or equal to the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void IList_InsertThrowsOnWrongIndex ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Insert (1, _orderItem2);
    }

    [Test]
    public void IList_Item ()
    {
      Assert.AreSame (_orderItem1, _orderItemListAsIList[0]);
      Assert.AreSame (_orderItem2, _orderItemListAsIList[1]);

      Assert.That (_orderItemListAsIList, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));

      _orderItemListAsIList[0] = _orderItem3;

      Assert.That (_orderItemListAsIList, Is.EqualTo (new object[] { _orderItem3, _orderItem2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index was out of range. Must be non-negative and less than the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void IList_ItemGetThrowsOnWrongIndex ()
    {
      Dev.Null = _orderItemListAsIList[2];
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Index is out of range. Must be non-negative and less than the size of the collection.",
        MatchType = MessageMatch.Contains)]
    public void IList_ItemSetThrowsOnWrongIndex ()
    {
      _orderItemListAsIList[-1] = null;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot modify a read-only collection.")]
    public void IList_ItemSetThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>)_orderItemListAsIList, true);
      readOnlyList[0] = null;
    }

    [Test]
    public void IList_Add ()
    {
      IList<OrderItem> list = new ObjectList<OrderItem> ();
      list.Add (_orderItem1);
      Assert.That (list, Is.EqualTo (new object[] { _orderItem1 }));
      
      list.Add (_orderItem2);
      Assert.That (list, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot add an item to a read-only collection.")]
    public void IList_AddThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemListAsIList, true);
      readOnlyList.Add (_orderItem1);
    }

    [Test]
    public void IList_Contains ()
    {
      Assert.IsTrue (_orderItemListAsIList.Contains (_orderItem1));
      Assert.IsTrue (_orderItemListAsIList.Contains (_orderItem2));
      Assert.IsFalse (_orderItemListAsIList.Contains (_orderItem3));
    }

    [Test]
    public void IList_CopyToTightFit ()
    {
      var destination = new OrderItem[2];
      _orderItemListAsIList.CopyTo (destination, 0);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void IList_CopyToLargeFit ()
    {
      var destination = new OrderItem[5];
      _orderItemListAsIList.CopyTo (destination, 0);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem2, null, null, null }));
      _orderItemListAsIList.CopyTo (destination, 1);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem1, _orderItem2, null, null}));
      _orderItemListAsIList.CopyTo (destination, 3);
      Assert.That (destination, Is.EqualTo (new object[] { _orderItem1, _orderItem1, _orderItem2, _orderItem1, _orderItem2}));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException),
        ExpectedMessage = "Number was less than the array's lower bound in the first dimension.", MatchType = MessageMatch.Contains)]
    public void IList_CopyToNegativeIndex ()
    {
      var destination = new OrderItem[5];
      _orderItemListAsIList.CopyTo (destination, -1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.",
        MatchType = MessageMatch.Contains)]
    public void IList_CopyToGreatIndex ()
    {
      var destination = new OrderItem[5];
      _orderItemListAsIList.CopyTo (destination, 5);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Destination array was not long enough. Check destIndex and length, and the array's lower bounds.",
        MatchType = MessageMatch.Contains)]
    public void IList_CopyToTooLittleSpace ()
    {
      var destination = new OrderItem[5];
      _orderItemListAsIList.CopyTo (destination, 4);
    }

    [Test]
    public void IList_Remove ()
    {
      Assert.IsFalse (_orderItemListAsIList.Remove (_orderItem3));
      Assert.IsTrue (_orderItemListAsIList.Remove (_orderItem1));
      Assert.IsFalse (_orderItemListAsIList.Remove (_orderItem1));
      Assert.IsTrue (_orderItemListAsIList.Remove (_orderItem2));
      Assert.IsFalse (_orderItemListAsIList.Remove (_orderItem2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void IList_RemoveThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemListAsIList, true);
      readOnlyList.Remove (_orderItem1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot remove an item from a read-only collection.")]
    public void IList_RemoveInexistentThrowsOnReadOnlyList ()
    {
      IList<OrderItem> readOnlyList = new ObjectList<OrderItem> ((ObjectList<OrderItem>) _orderItemListAsIList, true);
      readOnlyList.Remove (_orderItem3);
    }

    [Test]
    public void GetEnumerator ()
    {
      using (IEnumerator<OrderItem> enumerator = new ObjectList<OrderItem>().GetEnumerator())
      {
        Assert.IsFalse (enumerator.MoveNext());
      }

      using (IEnumerator<OrderItem> enumerator = _orderItemListAsIList.GetEnumerator())
      {
        Assert.IsTrue (enumerator.MoveNext());
        Assert.AreSame (_orderItem1, enumerator.Current);
        Assert.IsTrue (enumerator.MoveNext());
        Assert.AreSame (_orderItem2, enumerator.Current);
        Assert.IsFalse (enumerator.MoveNext());
      }
    }

    [Test]
    public void ToArray ()
    {
      OrderItem[] orderItems = _order.OrderItems.ToArray ();

      Assert.That (orderItems, Is.EquivalentTo (_order.OrderItems));
    }

    [Test]
    public void Linq ()
    {
      var result = from oi in _order.OrderItems where oi.Product == _orderItem1.Product select oi;
      Assert.That (result.ToArray(), Is.EqualTo (new[] {_orderItem1}));
    }

    [Test]
    public void AddRange ()
    {
      var newList = new ObjectList<OrderItem> ();
      newList.AddRange (new[] { _orderItem1, _orderItem2 });

      Assert.That (newList, Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException), ExpectedMessage = "Item 1 of argument domainObjects is null.")]
    public void AddRange_NullValue ()
    {
      var newList = new ObjectList<OrderItem> ();
      newList.AddRange (new[] { _orderItem1, null });
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemDuplicateException), ExpectedMessage = "Item 1 of argument domainObjects is a duplicate " 
        + "('OrderItem|ad620a11-4bc4-4791-bcf4-a0770a08c5b0|System.Guid').")]
    public void AddRange_DuplicateValue ()
    {
      var newList = new ObjectList<OrderItem> ();
      newList.AddRange (new[] { _orderItem1, _orderItem1 });
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException), ExpectedMessage = "Item 0 of argument domainObjects has the type " 
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order instead of Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.")]
    public void AddRange_InvalidType ()
    {
      var newList = new ObjectList<OrderItem> ();
      newList.AddRange (new DomainObject[] { _order });
    }
  }
}
