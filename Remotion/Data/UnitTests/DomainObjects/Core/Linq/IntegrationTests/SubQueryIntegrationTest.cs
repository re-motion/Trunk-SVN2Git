// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class SubQueryIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithContains_Like ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo>()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
    }

    [Test]
    public void QueryWithSubQuery_InWhere ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from c in QueryFactory.CreateLinqQuery<Customer>() select c).Contains (o.Customer)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithSubQueryInWhere_AccessingOuterVariable_InMainFromClause ()
    {
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      var number = from o in QueryFactory.CreateLinqQuery<Order> ()
                   where (from oi in o.OrderItems select oi).Contains (orderItem2)
                   select o;
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from c in QueryFactory.CreateLinqQuery<OrderTicket>() select c.Order).Contains (o)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere_WithOuterVariable ()
    {
      OrderItem myOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Order == o select oi).Contains (myOrderItem)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSubQuery_InMainFrom ()
    {
      var orders = from c in (from ci in QueryFactory.CreateLinqQuery<Computer> () select ci) select c;

      CheckQueryResult (orders, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4, 
                        DomainObjectIDs.Computer5 );
    }

    [Test]
    public void QueryWithSubQuery_WithResultOperator_InMainFrom ()
    {
      var orders = from c in (from ci in QueryFactory.CreateLinqQuery<Computer> () orderby ci.ID select ci).Take (1) select c;
      CheckQueryResult (orders, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithSubQuery_InAdditionalFrom ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in
            (from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Order == o select oi)
          select oi;

      CheckQueryResult (orders, DomainObjectIDs.OrderItem5, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1,
                        DomainObjectIDs.OrderItem3);
    }


    [Test]
    [Ignore ("TODO 1445: Should generate SELECT DISTINCT [oi].* FROM [OrderView] [o1] CROSS APPLY [OrderView] [o2] CROSS APPLY (SELECT [oi].* FROM [OrderItemView] [oi] WHERE ((([oi].[OrderID] IS NULL AND [o1].[ID] IS NULL) OR [oi].[OrderID] = [o1].[ID]) OR (([oi].[OrderID] IS NULL AND [o2].[ID] IS NULL) OR [oi].[OrderID] = [o2].[ID]))) [oi]")]
    public void QueryWithSubQuery_InThirdFrom ()
    {
      var orders =
          (from o1 in QueryFactory.CreateLinqQuery<Order> ()
          from o2 in QueryFactory.CreateLinqQuery<Order> ()
          from oi in
            (from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Order == o1 || oi.Order == o2 select oi)
          select oi).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.OrderItem5, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1,
                        DomainObjectIDs.OrderItem3);
    }

    [Test]
    [ExpectedException (typeof (ParserException), 
        ExpectedMessage = "This version of re-linq does not support subqueries in the select projection of a query.")]
    public void QueryWithSubQuery_InSelectClause ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   select
                       (from c in QueryFactory.CreateLinqQuery<Computer>() select c);

      orders.ToArray();
    }

    [Test]
    public void SubQueryWithNonConstantFromExpression ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  from oi in (from oi1 in o.OrderItems select oi1)
                  where o.OrderNumber == 1
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }
  }
}