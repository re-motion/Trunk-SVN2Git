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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class EagerFetchingIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithSingleAndPredicate ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   select o).Single (i => i.OrderNumber == 5);
      Assert.That (query, Is.EqualTo ((TestDomainBase.GetObject (DomainObjectIDs.Order4))));
    }

    [Test]
    public void EagerFetching ()
    {
      var query = (from c in QueryFactory.CreateLinqQuery<Customer> ()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains (c.Name)
                   select c).FetchMany (c => c.Orders).ThenFetchMany (o => o.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);

      CheckDataContainersRegistered (DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_FetchAfterMultipleFromsWithDistinct ()
    {
      var query = (from c1 in QueryFactory.CreateLinqQuery<Customer> ()
                   from c2 in QueryFactory.CreateLinqQuery<Customer> ()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains (c1.Name)
                   select c1).Distinct().FetchMany (x => x.Orders).ThenFetchMany (y => y.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);
      CheckDataContainersRegistered (DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_FetchAfterMultipleFromsWithoutSelectClauseInCallChain ()
    {
      var query = (from o1 in QueryFactory.CreateLinqQuery<Order> ()
                   from o2 in QueryFactory.CreateLinqQuery<Order> ()
                   select o1).Distinct ().FetchMany (x => x.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4, 
                        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);

      CheckDataContainersRegistered (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                                     DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
      CheckDataContainersRegistered (DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem3, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem5);

      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order2, "OrderItems", false, DomainObjectIDs.OrderItem3);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order3, "OrderItems", false, DomainObjectIDs.OrderItem4);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order4, "OrderItems", false, DomainObjectIDs.OrderItem5);
      CheckCollectionRelationRegistered (DomainObjectIDs.OrderWithoutOrderItem, "OrderItems", false);
      CheckCollectionRelationRegistered (DomainObjectIDs.InvalidOrder, "OrderItems", false);
    }

    [Test]
    public void EagerFetching_FetchOne ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o).FetchOne (o => o.OrderTicket);

      CheckQueryResult (query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered (DomainObjectIDs.Order1, DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered (DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
    }

    [Test]
    public void EagerFetching_ThenFetchOne ()
    {
      var query = (from c in QueryFactory.CreateLinqQuery<Customer> ()
                   where new[] { "Kunde 1", "Kunde 2" }.Contains (c.Name)
                   select c).FetchMany (c => c.Orders).ThenFetchOne (o => o.OrderTicket);

      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2);

      CheckDataContainersRegistered (
          DomainObjectIDs.Customer1, DomainObjectIDs.Customer2,
          DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem,
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2);

      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", false, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer2, "Orders", false);
      CheckObjectRelationRegistered (DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderWithoutOrderItem, "OrderTicket", DomainObjectIDs.OrderTicket2);
    }

    [Test]
    public void EagerFetching_MultipleFetches ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o)
          .Distinct()
          .FetchMany (o => o.OrderItems)
          .FetchOne (o => o.Customer).ThenFetchMany (c => c.Orders).ThenFetchOne (o => o.Customer).ThenFetchOne (c => c.Ceo);

      CheckQueryResult (query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered (
          DomainObjectIDs.Order1, // the original order
          DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2, // their items
          DomainObjectIDs.Customer1, // their customers
          DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, // their customer's orders
          DomainObjectIDs.Customer1, // their customer's orders' customers
          DomainObjectIDs.Ceo3); // their customer's orders' customers' ceos
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckObjectRelationRegistered (DomainObjectIDs.Order1, "Customer", DomainObjectIDs.Customer1);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
      CheckObjectRelationRegistered (DomainObjectIDs.Customer1, typeof (Company), "Ceo", DomainObjectIDs.Ceo3);
    }

    [Test]
    public void EagerFetching_WithTakeResultOperator ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   orderby o.OrderNumber
                   select o)
                   .Take (1)
                   .FetchMany (o => o.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Order1);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    [Ignore ("TODO 1441: Determine how to resolve this. Possibilities: Move the result operators to the front, issue an error, execute the take in memory.")]
    public void EagerFetching_WithResultOperator_AfterFetch ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   orderby o.OrderNumber
                   select o)
                   .FetchMany (o => o.OrderItems)
                   .Take (1);

      CheckQueryResult (query, DomainObjectIDs.Order1);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }
  }
}