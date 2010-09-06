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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class ResultOperatorIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithDistinct ()
    {
      var ceos =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    [Test]
    public void QueryWithContainsObject ()
    {
      OrderItem item = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderItems.ContainsObject (item)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    // test should show how own methods are registered and custom sql code is generated
    // MethodExtension defines extension method "ExtendString"
    // MethodExtendString generates sql code for this method
    [Test]
    [Ignore ("TODO 2612")]
    public void Query_WithCustomSqlGenerator_ForExtendStringMethod ()
    {
      //QueryFactory.GetDefaultSqlGenerator(typeof (Computer)).MethodCallRegistry.Register (
      //    typeof (MethodExtensions).GetMethod ("ExtendString", new[] { typeof (string)}), new MethodExtendString ());

      //var computers =
      //    from c in QueryFactory.CreateLinqQuery<Computer>()
      //    where c.Employee.Name.ExtendString () == "Trillian"
      //    select c;
      //CheckQueryResult (computers, DomainObjectIDs.Computer2);
      Assert.Fail();
    }

    [Test]
    public void Query_WithCastOnResultSet ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.OrderNumber == 1
           select o).Cast<TestDomainBase>();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void Query_WithCastInSubQuery ()
    {
      var query = from c in
                      (from o in QueryFactory.CreateLinqQuery<Order>()
                       where o.OrderNumber == 1
                       select o).Cast<TestDomainBase>()
                  where c.ID == DomainObjectIDs.Order1
                  select c;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithFirst ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         orderby o.ID
                         select o).First();
      Assert.That (queryResult, Is.EqualTo (Order.GetObject (DomainObjectIDs.InvalidOrder)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void QueryWithFirst_Throws_WhenNoItems ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       where false
       select o).First();
    }

    [Test]
    public void QueryWithFirstOrDefault ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         orderby o.ID
                         select o).FirstOrDefault();
      Assert.That (queryResult, Is.EqualTo (Order.GetObject (DomainObjectIDs.InvalidOrder)));
    }

    [Test]
    public void QueryWithFirstOrDefault_ReturnsNull_WhenNoItems ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where false
                         select o).FirstOrDefault();
      Assert.That (queryResult, Is.Null);
    }

    [Test]
    public void QueryWithSingle ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 1
                         select o).Single();
      Assert.That (queryResult, Is.EqualTo (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void QueryWithSingle_ThrowsException_WhenMoreThanOneElement ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       select o).Single();
    }

    [Test]
    public void QueryWithSingleOrDefault_ReturnsSingleItem ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 1
                         select o).SingleOrDefault();
      Assert.That (queryResult, Is.EqualTo (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    public void QueryWithSingleOrDefault_ReturnsNull_WhenNoItem ()
    {
      var queryResult = (from o in QueryFactory.CreateLinqQuery<Order>()
                         where o.OrderNumber == 99999
                         select o).SingleOrDefault();
      Assert.That (queryResult, Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains more than one element")]
    public void QueryWithSingleOrDefault_ThrowsException_WhenMoreThanOneElement ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order>()
       select o).SingleOrDefault();
    }

    [Test]
    public void QueryWithCount ()
    {
      var number = (from o in QueryFactory.CreateLinqQuery<Order>()
                    select o).Count();
      Assert.AreEqual (number, 6);
    }

    [Test]
    public void QueryWithCount_InSubquery ()
    {
      var number = (from o in QueryFactory.CreateLinqQuery<Order>()
                    where (from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Order == o select oi).Count() == 2
                    select o);
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryDistinctTest ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   from oi in o.OrderItems
                   where o.OrderNumber == 1
                   select o).Distinct();
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithConvertToString ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<OrderItem>()
          where Convert.ToString (o.Position).Contains ("2")
          select o;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithArithmeticOperations ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where (oi.Position + oi.Position) == 4
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithSubString ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.Name.Substring (1, 3).Contains ("und")
                  select c;
      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4);
    }

    [Test]
    public void QueryWithTake ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() select o).Take (3);
      CheckQueryResult (query, DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order3, DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void QueryWithTake_SubQueryAsArgument ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> () 
                  from oi in o.OrderItems.Take (o.OrderItems.Count() / 2)
                  where o.OrderNumber == 1
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection ()
    {
      var possibleItems = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains (o.ID)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
    }

    [Test]
    public void Query_WithSupportForObjectList ()
    {
      var orders =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           from oi in QueryFactory.CreateLinqQuery<OrderItem>()
           where oi.Order == o
           select o).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "This query provider does not support the given query ('from OrderItem oi in DomainObjectQueryable<OrderItem> "
        + "where (([oi].Order.OrderNumber = 1) || ([oi].Order.OrderNumber = 3)) "
        + "orderby [oi].Order.OrderNumber asc "
        + "select [oi] => GroupBy([oi].Order, [oi].Product)'). re-store only supports queries selecting a scalar value, a single DomainObject, or a "
        + "collection of DomainObjects. GroupBy must be executed in memory, for example by issuing AsEnumerable() before performing the grouping "
        + "operation.")]
    public void Query_WithGroupBy ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where oi.Order.OrderNumber == 1 || oi.Order.OrderNumber == 3
                  orderby oi.Order.OrderNumber
                  group oi.Product by oi.Order;

      query.ToArray();
    }

    [Test]
    public void Query_WithOfType_SelectingBaseType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Company>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_WithOfType_DerivedType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Customer>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_WithOfType_SameType ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Customer>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Query_WithOfType_UnrelatedType ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Order>();

      CheckQueryResult (query);
    }

    [Test]
    public void QueryWithAny_WithoutPredicate ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Any();

      Assert.IsTrue (query);
    }

    [Test]
    public void QueryWithAny_WithPredicate ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Any (c => c.SerialNumber == "123456");

      Assert.IsFalse (query);
    }

    [Test]
    public void QueryWithAny_InSubquery ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order>()
                  where !o.OrderItems.Any()
                  select o;

      CheckQueryResult (query, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithAll ()
    {
      var result1 = QueryFactory.CreateLinqQuery<Computer>().All (c => c.SerialNumber == "123456");
      Assert.That (result1, Is.False);

      var result2 = QueryFactory.CreateLinqQuery<Computer> ().All (c => c.SerialNumber != string.Empty);
      Assert.That (result2, Is.True);
    }

    [Test]
    public void QueryWithAll_AfterIncompatibleResultOperator ()
    {
      var query = QueryFactory.CreateLinqQuery<Computer>().Take (10).Take (20).All (c => c.SerialNumber == "123456");

      Assert.IsFalse (query);
    }

    [Test]
    public void QueryWithOrderBy_BeforeDistinct ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Distinct ().Count ();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithOrderBy_BeforeCount ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Count();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithOrderBy_BeforeCount_DueToIncompatibleResultOperators ()
    {
      var result = QueryFactory.CreateLinqQuery<Computer> ().OrderBy (c => c.SerialNumber).Take (10).Count ();

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void QueryWithAll_InSubquery ()
    {
      var query1 = from o in QueryFactory.CreateLinqQuery<Order>()
                  where o.OrderItems.All (oi => oi.Position == 1)
                  select o;

      CheckQueryResult (
          query1,
          DomainObjectIDs.OrderWithoutOrderItem,
          DomainObjectIDs.Order2,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.InvalidOrder);

      var query2 = from c in QueryFactory.CreateLinqQuery<Customer> ()
                  where c.Orders.All (o => o.OrderItems.Count() > 0)
                  select c;

      CheckQueryResult (
          query2,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4);
    }

    [Test]
    public void DefaultIsEmpty_WithoutJoin ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                  where o.ID == DomainObjectIDs.Order1
                  select o).DefaultIfEmpty();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void DefaultIsEmpty_WithoutJoin_EmptyResult ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   where o.OrderNumber == -1
                   select o).DefaultIfEmpty ();

      CheckQueryResult (query, null);
    }

    [Test]
    public void DefaultIsEmpty_WithJoin ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                   join c in QueryFactory.CreateLinqQuery<Customer>() on o.Customer equals c into goc
                   from oc in goc.DefaultIfEmpty()
                   where o.OrderNumber == 5
                   select oc);

      CheckQueryResult (query, DomainObjectIDs.Customer4);
    }

    [Test]
    public void Max_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o.OrderNumber).Max();

      Assert.That(query, Is.EqualTo(6));
    }

    [Test]
    public void Max_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where (from s2 in QueryFactory.CreateLinqQuery<Order>() select s2.OrderNumber).Max()== 6
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);   
    }

    [Test]
    public void Min_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o.OrderNumber).Min ();

      Assert.That (query, Is.EqualTo (1));
    }

    [Test]
    public void Min_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where (from s2 in QueryFactory.CreateLinqQuery<Order> () select s2.OrderNumber).Min () == 1
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Avaerage_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o).Average(o=>o.OrderNumber);

      Assert.That (query, Is.EqualTo (3.0));
    }

    [Test]
    public void Average_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where (from s2 in QueryFactory.CreateLinqQuery<Order> () select s2.OrderNumber).Average() == 3.0
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Sum_OnTopLevel ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o).Sum(o => o.OrderNumber);

      Assert.That (query, Is.EqualTo (21));
    }

    [Test]
    public void Sum_InSubquery ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where (from s2 in QueryFactory.CreateLinqQuery<Order> () select s2.OrderNumber).Sum () == 21
           select o);

      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Skip_WithEntity ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() orderby o.OrderNumber select o).Skip (5);

      CheckQueryResult (query, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Skip_WithEntity_WithoutExplicitOrdering ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() select o).Skip (5);

      CheckQueryResult (query, DomainObjectIDs.Order4);
    }

    [Test]
    public void TakeAfterSkip ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() orderby o.OrderNumber select o).Skip (3).Take (2);

      CheckQueryResult (query, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
    }

    [Test]
    public void QueryWithCastToInterface_ThrowsNoException ()
    {
      (from o in QueryFactory.CreateLinqQuery<Order> () select o).Cast<IOrder>();
    }

    [Test]
    public void GroupBy_NonEntityKey ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  group o by o.Customer.ID into ordersByCustomer
                  from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.ID == ordersByCustomer.Key
                  select c;

      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4, DomainObjectIDs.Customer5);
    }

    [Test]
    public void GroupBy_SelectKey_Nesting ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from x in
            (
              from oi in o.OrderItems
              group oi by oi.Product into orderItemsByProduct
              select new { OrderID = o.ID, OrderItems = orderItemsByProduct }
              )
          let product = x.OrderItems.Key
          where product == "Mainboard"
          select o;

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void GroupBy_UseGroupInFromExpression ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order> ()
                  group o.ID by o.OrderNumber into orderByOrderNo
                  from id in orderByOrderNo
                  select new { orderByOrderNo.Key, OrderID = id };
      Assert.That (query.Count(), Is.EqualTo (6));

      var query2 =
          from o in QueryFactory.CreateLinqQuery<Order>()
          group o by o.OrderNumber
          into orderByOrderNo
          from o in orderByOrderNo
          where o != null
          select new { orderByOrderNo.Key, Order = o };
      Assert.That (query2.Count (), Is.EqualTo (6));
      
      var query3 =
          from o in QueryFactory.CreateLinqQuery<Order>()
          group o.OrderNumber by o.OrderNumber into orderByOrderNo
          from o in
            (
              from so in orderByOrderNo
              select so).Distinct ()
          select new { orderByOrderNo.Key, Order = o };
      Assert.That (query3.Count (), Is.EqualTo (6));
    }

    [Test]
    [Ignore ("TODO 2990: Support for full column lists required in order to group by entity")]
    public void GroupBy_EntityKey ()
    {
      var query1 = from o in QueryFactory.CreateLinqQuery<Order> ()
                   group o by o.Customer into ordersByCustomer
                   select ordersByCustomer.Key;

      CheckQueryResult (query1, DomainObjectIDs.Customer1, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4);

      var query2 =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          group o by o.OrderTicket into ordersByOrderTicket
          where ordersByOrderTicket.Key != null
          select ordersByOrderTicket.Key.FileName;

      Assert.That (query2.Count (), Is.EqualTo (5));

      var query3 = from r in QueryFactory.CreateLinqQuery<Order> ()
                   from c in r.OrderItems
                   group c.ID by r
                     into cooksByRestaurant
                     from cook in cooksByRestaurant
                     select new { cooksByRestaurant.Key.DeliveryDate, CookID = cook };
      Assert.That (query3.Count (), Is.GreaterThan (0));
    }

    [Test]
    public void GroupBy_ResultSelector ()
    {
      var query = QueryFactory.CreateLinqQuery<Order> ().GroupBy (o => o.Customer.ID, (key, group) => key);

      Assert.That (query.Count (), Is.EqualTo (4));
    }

    [Test]
    public void GroupBy_SubqueryUsedAsGroupByKey ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> ()
                   group o by QueryFactory.CreateLinqQuery<OrderItem> ().Where (oi => oi != null).Select(soi=>soi.Product).First ()).Select (g => g.Key);
      Assert.That (query.Count(), Is.GreaterThan (0));
    }

    [Test]
    public void GroupBy_WithConstantKey_GetsReplacedBySubquery ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>().GroupBy (c => 0).Select (c => c.Key);

      Assert.That (query.Count (), Is.GreaterThan (0));
    }
    
  }
}