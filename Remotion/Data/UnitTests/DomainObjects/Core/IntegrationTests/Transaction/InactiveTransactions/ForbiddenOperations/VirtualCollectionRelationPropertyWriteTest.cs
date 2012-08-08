// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.ForbiddenOperations
{
  [TestFixture]
  public class VirtualCollectionRelationPropertyWriteTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _orderItem1 = ActiveSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      _orderItem2 = ActiveSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem2));

      _orderItem3 = ActiveSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem3));
      _orderItem4 = ActiveSubTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem4));

      ActiveSubTransaction.Execute (() => _order1.OrderItems.Add (_orderItem3));
      ActiveSubTransaction.Execute (() => _orderItem4.Order.EnsureDataAvailable ());
    }

    [Test]
    public void RelationSetInInactiveRootTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderItems = new ObjectList<OrderItem>()), "RelationChanging");

      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }

    [Test]
    public void RelationSetInInactiveMiddleTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderItems = new ObjectList<OrderItem> ()), "RelationChanging");

      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }
  }
}