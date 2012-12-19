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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
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
    public void EagerFetching_MultipleFetches_OnSameLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o)
          .Distinct ()
          .FetchMany (o => o.OrderItems)
          .FetchOne (o => o.Customer).ThenFetchMany (c => c.Orders).ThenFetchOne (c => c.OrderTicket)
          .FetchOne (o => o.Customer).ThenFetchMany (c => c.Orders).ThenFetchMany (c => c.OrderItems)
          .FetchMany (o => o.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered (
          DomainObjectIDs.Order1, // the original order
          DomainObjectIDs.Customer1, // the customer
          DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, // the customer's orders
          DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2, // the customer's orders' tickets
          DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 // the customer's orders' items
          );
      
      CheckObjectRelationRegistered (DomainObjectIDs.Order1, "Customer", DomainObjectIDs.Customer1);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", true, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);

      CheckObjectRelationRegistered (DomainObjectIDs.Order1, "OrderTicket", DomainObjectIDs.OrderTicket1);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderWithoutOrderItem, "OrderTicket", DomainObjectIDs.OrderTicket2);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderTicket2, "Order", DomainObjectIDs.OrderWithoutOrderItem);

      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderItem1, "Order", DomainObjectIDs.Order1);
      CheckObjectRelationRegistered (DomainObjectIDs.OrderItem2, "Order", DomainObjectIDs.Order1);
      CheckCollectionRelationRegistered (DomainObjectIDs.OrderWithoutOrderItem, "OrderItems", false);
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
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "There was an error preparing or resolving query "
        + "'from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o] => Fetch (Order.OrderItems) => Take(1)' for "
        + "SQL generation. The fetch query operator methods must be the last query operators in a LINQ query. All calls to Where, Select, Take, etc. "
        + "must go before the fetch operators.\r\n\r\n"
        + "E.g., instead of 'QueryFactory.CreateLinqQuery<Order>().FetchMany (o => o.OrderItems).Where (o => o.OrderNumber > 1)', "
        + "write 'QueryFactory.CreateLinqQuery<Order>().Where (o => o.OrderNumber > 1).FetchMany (o => o.OrderItems)'.")]
    public void EagerFetching_WithResultOperator_AfterFetch ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o)
                   .FetchMany (o => o.OrderItems)
                   .Take (1);

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "There was an error preparing or resolving query "
        + "'from Order o in {DomainObjectQueryable<Order> => Fetch (Order.OrderItems)} where ([o].OrderNumber = 1) select [o]' for SQL generation. "
        + "The fetch query operator methods must be the last query operators in a LINQ query. All calls to Where, Select, Take, etc. must go before "
        + "the fetch operators.\r\n\r\n"
        + "E.g., instead of 'QueryFactory.CreateLinqQuery<Order>().FetchMany (o => o.OrderItems).Where (o => o.OrderNumber > 1)', "
        + "write 'QueryFactory.CreateLinqQuery<Order>().Where (o => o.OrderNumber > 1).FetchMany (o => o.OrderItems)'.")]
    public void EagerFetching_WithFetch_InASubQuery ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>()
                   .FetchMany (o => o.OrderItems)
                   .Where (o => o.OrderNumber == 1);
      query.ToArray ();
    }

    [Test]
    public void EagerFetching_WithOrderBy_WithoutTake ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   orderby o.OrderNumber
                   select o)
                   .FetchMany (o => o.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.Order1);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void EagerFetching_FetchNull_VirtualSide ()
    {
      var query = (from employee in QueryFactory.CreateLinqQuery<Employee> ()
                   where employee.ID == DomainObjectIDs.Employee1
                   select employee).FetchOne (e => e.Computer);

      CheckQueryResult (query, DomainObjectIDs.Employee1);

      CheckDataContainersRegistered (DomainObjectIDs.Employee1);
      CheckObjectRelationRegistered (DomainObjectIDs.Employee1, "Computer", null);
    }

    [Test]
    public void EagerFetching_FetchNull_NonVirtualSide ()
    {
      var query = (from computer in QueryFactory.CreateLinqQuery<Computer> ()
                   where computer.ID == DomainObjectIDs.Computer4
                   select computer).FetchOne (c => c.Employee);

      CheckQueryResult (query, DomainObjectIDs.Computer4);

      CheckDataContainersRegistered (DomainObjectIDs.Computer4);
      CheckObjectRelationRegistered (DomainObjectIDs.Computer4, "Employee", null);
    }

    [Test]
    public void EagerFetching_WithDifferentEntityThanInMainFromClause ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o.Customer).FetchMany (c => c.Orders);

      CheckQueryResult (query, DomainObjectIDs.Customer1);

      CheckDataContainersRegistered (DomainObjectIDs.Customer1, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
      CheckCollectionRelationRegistered (DomainObjectIDs.Customer1, "Orders", false, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void EagerFetching_FetchEmptyCollection ()
    {
      var query = (from order in QueryFactory.CreateLinqQuery<Order> ()
                   where order.ID == DomainObjectIDs.OrderWithoutOrderItem
                   select order).FetchMany (o => o.OrderItems);

      CheckQueryResult (query, DomainObjectIDs.OrderWithoutOrderItem);

      CheckDataContainersRegistered (DomainObjectIDs.OrderWithoutOrderItem);
      CheckCollectionRelationRegistered (DomainObjectIDs.OrderWithoutOrderItem, "OrderItems", false);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Only queries returning DomainObjects can perform eager fetching.")]
    public void EagerFetching_WithCustomQuery ()
    {
      QueryFactory
          .CreateLinqQuery<Customer>()
          .Select (c => new { c.Name })
          .FetchOne (x => x.Name)
          .ToArray();
    }

    [Test]
    [Ignore ("TODO 5284")]
    public void EagerFetching_RedirectedProperty ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == 1
                   select o).FetchMany (o => o.RedirectedOrderItems);

      CheckQueryResult (query, DomainObjectIDs.Order1);

      CheckDataContainersRegistered (DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      CheckCollectionRelationRegistered (DomainObjectIDs.Order1, "OrderItems", false, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    [Ignore ("TODO 5284")]
    public void EagerFetching_MixedProperty_ViaCastInFetchClause ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
                   where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
                   select o)
                   .FetchOne (o => ((IMixinAddingPersistentProperties) o).CollectionProperty1Side);

      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckDataContainersRegistered (DomainObjectIDs.RelationTargetForPersistentMixin3);
      CheckCollectionRelationRegistered (
          DomainObjectIDs.TargetClassForPersistentMixins2,
          typeof (IMixinAddingPersistentProperties),
          "CollectionProperty1Side",
          false,
          DomainObjectIDs.RelationTargetForPersistentMixin3);
    }

    [Test]
    [Ignore ("TODO 5284")]
    public void EagerFetching_MixedProperty_ViaCastInSelect ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
                   where o.ID == DomainObjectIDs.TargetClassForPersistentMixins2
                   select (IMixinAddingPersistentProperties) o)
                   .FetchOne (o => o.CollectionProperty1Side);

      CheckQueryResult (query.AsEnumerable().Cast<DomainObject>(), DomainObjectIDs.TargetClassForPersistentMixins2);

      CheckDataContainersRegistered (DomainObjectIDs.RelationTargetForPersistentMixin3);
      CheckCollectionRelationRegistered (
          DomainObjectIDs.TargetClassForPersistentMixins2,
          typeof (IMixinAddingPersistentProperties),
          "CollectionProperty1Side",
          false,
          DomainObjectIDs.RelationTargetForPersistentMixin3);
    }
  }
}
