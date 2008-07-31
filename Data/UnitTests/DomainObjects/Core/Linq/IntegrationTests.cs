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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Customer=Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer;
using Order=Remotion.Data.UnitTests.DomainObjects.TestDomain.Order;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class IntegrationTests : ClientTransactionBaseTest
  {
    [Test]
    public void SimpleQuery ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4,
                        DomainObjectIDs.Computer5);
    }

    [Test]
    public void SimpleQuery_WithRelatedEntity ()
    {
      var query =
          from o in DataContext.Entity<OrderTicket> ()
          select o.Order;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem);
    }
    
    [Test]
    public void QueryWithWhereConditions ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.SerialNumber == "93756-ndf-23" || c.SerialNumber == "98678-abc-43"
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionsAndNull ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.Employee != null
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithWhereConditionAndStartsWith ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.SerialNumber.StartsWith ("9")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionAndEndsWith ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.SerialNumber.EndsWith ("7")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithWhere_OuterObject ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      var employees =
          from e in DataContext.Entity<Employee> ()
          where e == employee
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1);
    }

    [Test]
    public void QueryWithWhereConditionAndGreaterThan ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          where o.OrderNumber <= 3
          select o;

      CheckQueryResult (orders, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order2, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsNull ()
    {
      var employees =
          from e in DataContext.Entity<Employee> ()
          where e.Computer == null
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2, DomainObjectIDs.Employee6, DomainObjectIDs.Employee7);
    }

    [Test]
    public void QueryWithVirtualKeySide_NotEqualsNull ()
    {
      var employees =
          from e in DataContext.Entity<Employee> ()
          where e.Computer != null
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee3, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsOuterObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      var employees =
          from e in DataContext.Entity<Employee> ()
          where e.Computer == computer
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee3);
    }

    [Test]
    public void QueryWithVirtualKeySide_NotEqualsOuterObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      var employees =
          from e in DataContext.Entity<Employee> ()
          where e.Computer != computer
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5,
                        DomainObjectIDs.Employee6, DomainObjectIDs.Employee7);
    }

    [Test]
    public void QueryWithOuterEntityInCondition ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.Employee == employee
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void QueryWithIDInCondition ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.Employee.ID == employee.ID
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void QueryWithSimpleOrderBy ()
    {
      var query =
          from o in DataContext.Entity<Order> ()
          orderby o.OrderNumber
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order2, DomainObjectIDs.Order3,
                        DomainObjectIDs.Order4, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithOrderByAndImplicitJoin ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          where o.OrderNumber <= 4
          orderby o.Customer.Name
          select o;

      Order[] expected =
          GetExpectedObjects<Order> (DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      Assert.That (orders.ToArray (), Is.EqualTo (expected));
    }
    

    [Test]
    public void QueryWithSelectAndImplicitJoin_VirtualSide ()
    {
      var ceos =
          (from o in DataContext.Entity<Order> ()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct ();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    [Test]
    public void QueryWithSelectAndImplicitJoin ()
    {
      var ceos =
          from o in DataContext.Entity<Order> ()
          where o.Customer.Ceo.Name == "Hugo Boss"
          select o.Customer.Ceo;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithSelectAndImplicitJoin_UsingJoinPartTwice ()
    {
      var ceos =
          from o in DataContext.Entity<Order> ()
          where o.Customer.Name == "Kunde 3"
          select o.Customer.Ceo;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithDistinct ()
    {
      var ceos =
          (from o in DataContext.Entity<Order> ()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct ();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    [Test]
    public void QueryWithWhereAndImplicitJoin ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          where o.Customer.Type == Customer.CustomerType.Gold
          select o;

      CheckQueryResult (orders, DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order3, DomainObjectIDs.Order2, DomainObjectIDs.Order4);
    }

    [Test]
    public void QueryWithSubQueryAndWhereInAdditionalFrom ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          from o2 in
              (from oi in DataContext.Entity<OrderItem> () where oi.Order == o select oi)
          select o2;

      CheckQueryResult (orders, DomainObjectIDs.OrderItem5, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1,
                        DomainObjectIDs.OrderItem3);
    }

    [Test]
    public void QueryWithSubQueryInWhere ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          where (from c in DataContext.Entity<Customer> () select c).Contains (o.Customer)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithContains_Like ()
    {
      var ceos = from c in DataContext.Entity<Ceo> ()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere ()
    {
      var orders =
          from o in DataContext.Entity<Order> ()
          where (from c in DataContext.Entity<OrderTicket> () select c.Order).Contains (o)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void QueryWithSubQueryAndJoinInWhere_WithOuterVariable ()
    {
      OrderItem myOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders =
          from o in DataContext.Entity<Order> ()
          where (from oi in DataContext.Entity<OrderItem> () where oi.Order == o select oi).Contains (myOrderItem)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection ()
    {
      ObjectID[] possibleItems = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 };
      var orders =
          from o in DataContext.Entity<Order> ()
          where possibleItems.Contains (o.ID)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2);
    }

    [Test]
    public void QueryWithContainsObject ()
    {
      OrderItem item = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders =
          from o in DataContext.Entity<Order> ()
          where o.OrderItems.ContainsObject (item)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSubQuery ()
    {
      OrderItem item = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders = 
          from o in DataContext.Entity<Order> ()
          where (from y in DataContext.Entity <OrderItem>() where y == item select y.Order).Contains(o) 
          select o;
      CheckQueryResult (orders, DomainObjectIDs.Order1);

    }

    [Test]
    public void QueryWithLet_LethWithTable ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   let x = o
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order4,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void QueryWithLet_LetWithColumn ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   let y = o.OrderNumber
                   where y > 1
                   select o;

      CheckQueryResult (orders,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order3, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order2,
                        DomainObjectIDs.Order4);
    }

    [Test]
    public void QueryWithLet_LetWithColumn2 ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   let x = o.Customer.Name
                   where x == "Kunde 1"
                   select o;
      CheckQueryResult (orders, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSeveralJoinsAndCrossApply ()
    {
      var ceos = from o in DataContext.Entity<Order> ()
                 let x = o.Customer.Ceo
                 where x.Name == "Hugo Boss"
                 select x;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithLet_SeveralCrossApplies ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   let x = o
                   let y = o.Customer
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order4,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected no subqueries for Select expressions, found DataContext.Entity<Order>()"
                                                                    + ".Select(o => Entity().Select(c => c)) (MethodCallExpression).")]
    public void QueryWithSubQuery_InSelectClause ()
    {
      var orders = from o in DataContext.Entity<Order>()
                   select
                       (from c in DataContext.Entity<Computer>() select c);

      IQueryable<Computer>[] result = orders.ToArray();
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected no subqueries for Select expressions, found DataContext.Entity<Order>()"
                                                                    + ".Where(o => (o.OrderNumber = 5)).Select(o => Entity().Select(c => c)) (MethodCallExpression).")]
    public void QueryWithSubQueryInSelectClause_WhereClause ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   where o.OrderNumber == 5
                   select
                       (from c in DataContext.Entity<Computer> () select c);

      IQueryable<Computer>[] result = orders.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected no subqueries for Select expressions, found DataContext.Entity<Order>()"
                                                                    + ".Where(o => (o.OrderNumber = 5)).Select(o => Entity().Where(c => (c = null))) (MethodCallExpression).")]
    public void QueryWithSubQueryInSelectClause_WhereClause2 ()
    {
      var orders = from o in DataContext.Entity<Order> ()
                   where o.OrderNumber == 5
                   select
                       (from c in DataContext.Entity<Computer> () where c == null select c);

      IQueryable<Computer>[] result = orders.ToArray ();
    }

    [Test]
    public void QueryWithSeveralOrderBys ()
    {
      var orders = from o in DataContext.Entity<Order>()
                   orderby o.OrderNumber
                   orderby o.Customer.Name descending
                   select o;

      CheckQueryResult (orders, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order2, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Query_WithToUpper ()
    {
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.Employee.Name.ToUpper () == "TRILLIAN"
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer2);
    }

    // test should show how own methods are registered and custom sql code is generated
    // MethodExtension defines extension method "ExtendString"
    // MethodExtendString generates sql code for this method
    [Test]
    public void Query_WithCustomSqlGenerator_ForExtendStringMethod ()
    {
      DataContext.SqlGenerator.MethodCallRegistry.Register (
          typeof (MethodExtensions).GetMethod ("ExtendString", new Type[] { typeof (string)}), new MethodExtendString ());
      
      var computers =
          from c in DataContext.Entity<Computer> ()
          where c.Employee.Name.ExtendString () == "Trillian"
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer2);
    }

    [Test]
    public void Query_WithView ()
    {
      var domainBases =
          from d in DataContext.Entity<DomainBase>()
          select d;

      Assert.That (domainBases.ToArray(), Is.Not.Empty);
    }

    [Test]
    public void Query_WithSeveralFroms ()
    {
      var query =
          from o in DataContext.Entity<Order>()
          from c in DataContext.Entity<OrderTicket>()
          where c.Order == o
          where o.OrderNumber == 1
          select c;

      CheckQueryResult (query, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void Query_WithCastOnResultSet ()
    {
      var query =
          (from o in DataContext.Entity<Order> ()
          where o.OrderNumber == 1
          select o).Cast<TestDomainBase>();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This query provider does not support the given select projection "
        + "('NewObject'). The projection must select single DomainObject instances.")]
    public void Query_WithUnsupportedType_NewObject ()
    {
      var query =
          from o in DataContext.Entity<Order> ()
          where o.OrderNumber == 1
          select new { o, o.Customer };

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This query provider does not support the given select projection "
        + "('Constant'). The projection must select single DomainObject instances.")]
    public void Query_WithUnsupportedType_Constant ()
    {
      var query =
          from o in DataContext.Entity<Order> ()
          where o.OrderNumber == 1
          select 1;

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage=
      "This query provider does not support selecting single columns ('o.ID'). The projection must select whole DomainObject instances.")]
    public void Query_WithUnsupportedType_NonDomainObjectColumn ()
    {
      var query =
          from o in DataContext.Entity<Order> ()
          where o.OrderNumber == 1
          select o.ID;

      query.ToArray ();
    }
    
    public static void CheckQueryResult<T> (IEnumerable<T> query, params ObjectID[] expectedObjectIDs)
        where T : TestDomainBase
    {
      T[] results = query.ToArray ();
      T[] expected = GetExpectedObjects<T> (expectedObjectIDs);
      Assert.That (results, Is.EquivalentTo (expected));
    }

    private static T[] GetExpectedObjects<T> (params ObjectID[] expectedObjectIDs)
        where T: TestDomainBase
    {
      return (from id in expectedObjectIDs select (id == null ? null : (T) TestDomainBase.GetObject (id))).ToArray();
    }

  }
}
