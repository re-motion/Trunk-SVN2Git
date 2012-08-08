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
  public class VirtualObjectRelationPropertyWriteTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private OrderTicket _orderTicket1;
    private OrderTicket _orderTicket2;
    private OrderTicket _orderTicket3;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _orderTicket1 = ActiveSubTransaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket1));
      _orderTicket2 = ActiveSubTransaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket2));
      _orderTicket3 = ActiveSubTransaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket3));

      ActiveSubTransaction.Execute (() => _order1.OrderTicket = _orderTicket2);
      ActiveSubTransaction.Execute (() => _orderTicket3.Order.EnsureDataAvailable());
    }

    [Test]
    public void RelationSetInInactiveRootTransaction_IsForbidden ()
    {
      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);

      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderTicket = _orderTicket3), "RelationChanging");

      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);
    }

    [Test]
    public void RelationSetInInactiveMiddleTransaction_IsForbidden ()
    {
      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);

      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderTicket = _orderTicket3), "RelationChanging");

      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);
    }
  }
}