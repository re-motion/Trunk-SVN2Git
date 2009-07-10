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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.Parsing;
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
          from c in QueryFactory.CreateLinqQuery<Computer>()
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4,
                        DomainObjectIDs.Computer5);
    }

    [Test]
    public void SimpleQuery_WithRelatedEntity ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<OrderTicket>()
          select o.Order;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem);
    }
    
    [Test]
    public void QueryWithWhereConditions ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber == "93756-ndf-23" || c.SerialNumber == "98678-abc-43"
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionsAndNull ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee != null
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithBase ()
    {
      Company partner = Company.GetObject (DomainObjectIDs.Partner1);
      IQueryable<Company> result = (from c in QueryFactory.CreateLinqQuery<Company> ()
                           where c.ID == partner.ID
                           select c);
      CheckQueryResult (result, DomainObjectIDs.Partner1);
    }


    //[Test]
    //public void QueryWithWhereAndNullWithOr ()
    //{
    //  var computers =
    //      from c in QueryFactory.CreateLinqQuery<Company>()
    //      where c.C
    //      select c;
      
    //  CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    //}

    [Test]
    public void QueryWithWhereConditionAndStartsWith ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber.StartsWith ("9")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionAndEndsWith ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber.EndsWith ("7")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithWhere_OuterObject ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e == employee
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1);
    }

    [Test]
    public void QueryWithWhereConditionAndGreaterThan ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber <= 3
          select o;

      CheckQueryResult (orders, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order2, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsNull ()
    {
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer == null
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2, DomainObjectIDs.Employee6, DomainObjectIDs.Employee7);
    }

    [Test]
    public void QueryWithVirtualKeySide_NotEqualsNull ()
    {
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer != null
          select e;
      CheckQueryResult (employees, DomainObjectIDs.Employee3, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsOuterObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer == computer
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee3);
    }
    
    [Test]
    public void QueryWithVirtualKeySide_NotEqualsOuterObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
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
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee == employee
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void QueryWithIDInCondition ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee.ID == employee.ID
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void QueryWithSimpleOrderBy ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          orderby o.OrderNumber
          select o;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order2, DomainObjectIDs.Order3,
                        DomainObjectIDs.Order4, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void QueryWithOrderByAndImplicitJoin ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber <= 4
          orderby o.Customer.Name
          select o;

      Order[] expected =
          GetExpectedObjects<Order> (DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      Assert.That (orders.ToArray (), Is.EqualTo (expected));
    }

    [Test]
    public void QueryWithSeveralImplicitJoins ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.Customer.Ceo.Company.IndustrialSector.Name == "Raumschiffproduktion"
          orderby o.Customer.Name
          select o;

      Order[] expected =
          GetExpectedObjects<Order> (DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order1);
      Assert.That (orders.ToArray (), Is.EqualTo (expected));
    }

    [Test]
    public void QueryWithSelectAndImplicitJoin_VirtualSide ()
    {
      var ceos =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct ();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    [Test]
    public void QueryWithSelectAndImplicitJoin ()
    {
      var ceos =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.Customer.Ceo.Name == "Hugo Boss"
          select o.Customer.Ceo;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithSelectAndImplicitJoin_UsingJoinPartTwice ()
    {
      var ceos =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.Customer.Name == "Kunde 3"
          select o.Customer.Ceo;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithDistinct ()
    {
      var ceos =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.Customer.Ceo != null
           select o.Customer.Ceo).Distinct ();

      CheckQueryResult (ceos, DomainObjectIDs.Ceo12, DomainObjectIDs.Ceo5, DomainObjectIDs.Ceo3);
    }

    
    [Test]
    public void QueryWithWhereAndImplicitJoin ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.Customer.Type == Customer.CustomerType.Gold
          select o;

      CheckQueryResult (orders, DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order3, DomainObjectIDs.Order2, DomainObjectIDs.Order4);
    }

    [Test]
    public void QueryWithSubQueryAndWhereInAdditionalFrom ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          from o2 in
              (from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Order == o select oi)
          select o2;

      CheckQueryResult (orders, DomainObjectIDs.OrderItem5, DomainObjectIDs.OrderItem4, DomainObjectIDs.OrderItem2, DomainObjectIDs.OrderItem1,
                        DomainObjectIDs.OrderItem3);
    }

    [Test]
    public void QueryWithSubQueryInWhere ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from c in QueryFactory.CreateLinqQuery<Customer>() select c).Contains (o.Customer)
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    [Ignore ("TODO 1157: Fix this")]
    public void QueryWithSubQueryInWhere_AccessingOuterVariable_InMainFromClause ()
    {
      var orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      var number = from o in QueryFactory.CreateLinqQuery<Order> ()
                   where (from oi in o.OrderItems select oi).Contains (orderItem2)
                   select o;
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithContains_Like ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo>()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
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
    public void QueryWithContainsInWhere_OnEmptyCollection ()
    {
      var possibleItems = new ObjectID[] {  };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains (o.ID)
          select o;

      CheckQueryResult (orders);
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

    [Test]
    public void QueryWithSubQuery ()
    {
      OrderItem item = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orders = 
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from y in QueryFactory.CreateLinqQuery<OrderItem>() where y == item select y.Order).Contains(o) 
          select o;
      CheckQueryResult (orders, DomainObjectIDs.Order1);

    }

    [Test]
    public void QueryWithSubQueryInMainFrom ()
    {
      var orders = from c in (from ci in QueryFactory.CreateLinqQuery<Computer> () select ci) select c;

      CheckQueryResult (orders, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4, 
          DomainObjectIDs.Computer5 );
    }

    [Test]
    public void QueryWithInto ()
    {
      var orders = from c in QueryFactory.CreateLinqQuery<Customer> () 
                   where c.ID == DomainObjectIDs.Customer1 
                   select c.Orders 
                   into x 
                   from o in x 
                   from oi in o.OrderItems
                   select oi;

      CheckQueryResult (orders, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithLet_LetWithTable ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order4,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void QueryWithLet_LetWithColumn ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
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
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o.Customer.Name
                   where x == "Kunde 1"
                   select o;
      CheckQueryResult (orders, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithSeveralJoinsAndLet ()
    {
      var ceos = from o in QueryFactory.CreateLinqQuery<Order>()
                 let x = o.Customer.Ceo
                 where x.Name == "Hugo Boss"
                 select x;

      CheckQueryResult (ceos, DomainObjectIDs.Ceo5);
    }

    [Test]
    public void QueryWithSeveralLets ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   let x = o
                   let y = o.Customer
                   select x;

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order4,
                        DomainObjectIDs.InvalidOrder, DomainObjectIDs.OrderWithoutOrderItem);
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
    public void QueryWithSeveralOrderBys ()
    {
      var orders = from o in QueryFactory.CreateLinqQuery<Order>()
                   orderby o.OrderNumber
                   orderby o.Customer.Name descending
                   select o;

      CheckQueryResult (orders, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order2, DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem, DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void Query_WithToUpper ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
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
      QueryFactory.GetDefaultSqlGenerator(typeof (Computer)).MethodCallRegistry.Register (
          typeof (MethodExtensions).GetMethod ("ExtendString", new[] { typeof (string)}), new MethodExtendString ());
      
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee.Name.ExtendString () == "Trillian"
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer2);
    }

    [Test]
    public void Query_WithView ()
    {
      var domainBases =
          from d in QueryFactory.CreateLinqQuery<TableInheritance.TestDomain.DomainBase>()
          select d;

      Assert.That (domainBases.ToArray(), Is.Not.Empty);
    }

    [Test]
    public void Query_WithSeveralFroms ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          from c in QueryFactory.CreateLinqQuery<OrderTicket>()
          where c.Order == o
          where o.OrderNumber == 1
          select c;

      CheckQueryResult (query, DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void Query_WithSupportForObjectList ()
    {
      var orders =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
          where oi.Order == o
          // where ...
          select o).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This query provider does not support the given select projection "
        + "('NewObject'). The projection must select single DomainObject instances, because re-store does not support this kind of select projection.")]
    public void Query_WithUnsupportedType_NewObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select new { o, o.Customer };

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This query provider does not support the given select projection "
        + "('Constant'). The projection must select single DomainObject instances, because re-store does not support this kind of select projection.")]
    public void Query_WithUnsupportedType_Constant ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
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
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select o.ID;

      query.ToArray ();
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
      var number = (from o in QueryFactory.CreateLinqQuery<Order> ()
                    where (from oi in QueryFactory.CreateLinqQuery <OrderItem> () where oi.Order == o select oi).Count () == 2
                    select o);
      CheckQueryResult (number, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithFirst ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() 
                   select o).First();
      Assert.That (query, Is.EqualTo ((TestDomainBase.GetObject (DomainObjectIDs.InvalidOrder))));
    }

    [Test]
    public void QueryWithSingleAndPredicate ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>()
                    select o).Single (i => i.OrderNumber == 5);
      Assert.That (query, Is.EqualTo ((TestDomainBase.GetObject (DomainObjectIDs.Order4))));
    }

    [Test]
    public void QueryTest ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber == 4
                   select o ).Distinct();
      query.Single();

      Assert.That (query, Is.InstanceOfType (typeof(DomainObjectQueryable<Order>)));
    }

    [Test]
    public void QueryWithWhereOnForeignKey_RealSide ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where oi.Order.ID == id
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithWhereOnForeignKey_VirtualSide ()
    {
      ObjectID id = DomainObjectIDs.Computer1;
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
                  where e.Computer.ID == id
                  select e;
      CheckQueryResult (query, DomainObjectIDs.Employee3);
    }

    [Test]
    public void QueryWithMemberFromClause ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          from oi in o.OrderItems
          where o.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithMemberFromClause_WithJoin ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          from oi in ot.Order.OrderItems
          where ot.Order.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithMemberFromClause_WithLet ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          let x = ot.Order
          from oi in x.OrderItems
          where ot.Order.OrderNumber == 1
          select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithLet_AndMultipleFromClauses ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          from o in QueryFactory.CreateLinqQuery<Order>()
          let x = ot.Order
          where ot.Order.OrderNumber == 1
          where o == ot.Order
          select x;
      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithConvertToString ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<OrderItem> ()
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

    
    //[Test]
    //public void QueryWithLength ()
    //{
    //  var query = from c in QueryFactory.CreateLinqQuery<Company>()
    //              where c.Name.Contains("Test")
    //              select c;
    //  CheckQueryResult (query, DomainObjectIDs.Company1, DomainObjectIDs.Company2);
    //}

    //[Test]
    //[Ignore("anonymous type is not supported at the moment")]
    //public void QueryTransparentIdentifiers ()
    //{
    //  //gets an anonymouse type
    //  var query = from o in QueryFactory.CreateLinqQuery<Order>()
    //              select new { OrderItems = from oi in o.OrderItems select oi };
    //  //CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    //}

    ///////////////////////////////////////////////////////////////////////

    //spike for fetching (2 queries)
    //[Test]
    //public void SimpleQueryBased ()
    //{
    //  var query = QueryFactory.CreateLinqQuery<Customer>();
    //  var result = from c in query
    //               where c.Name == "Kunde 3"
    //               select c;
    //  CheckQueryResult (result, DomainObjectIDs.Customer3);
    //}

    //[Test]
    //public void ExtendSimpleQueryWithAdditionalFrom ()
    //{
    //  var query = QueryFactory.CreateLinqQuery<Customer> ();
    //  var result = from c in query
    //               from o in c.Orders
    //               where c.Name == "Kunde 3"
    //               select o;
    //  CheckQueryResult (result, DomainObjectIDs.Order2);
    //}

    [Test]
    public void QueryWithSubString ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<Customer> ()
                  where c.Name.Substring (2, 3).Contains ("und")
                  select c;
      CheckQueryResult (query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer2, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4);
    }

    [Test]
    public void QueryWithTake ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order> () select o).Take (3);
      CheckQueryResult (query, DomainObjectIDs.InvalidOrder, DomainObjectIDs.Order3, DomainObjectIDs.OrderWithoutOrderItem);
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
                   select o1).FetchMany (x => x.OrderItems).Distinct();

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
    public void TableInheritance_AccessingPropertyFromBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TableInheritance.TestDomain.ClassWithUnidirectionalRelation> ()
                  where c.DomainBase.CreatedAt == new DateTime (2006, 01, 03)
                  select c;
      CheckQueryResult (query, new TableInheritance.DomainObjectIDs ().ClassWithUnidirectionalRelation);
    }

    [Test]
    public void TableInheritance_MemberJoinViaBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TableInheritance.TestDomain.Client> ()
                  from domainBase in c.AssignedObjects
                  where domainBase.CreatedAt == new DateTime (2006, 01, 03)
                  select domainBase;
      
      var domainObjectIDs = new TableInheritance.DomainObjectIDs ();
      CheckQueryResult (query, domainObjectIDs.PersonForUnidirectionalRelationTest, domainObjectIDs.Person);
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

    public static void CheckQueryResult<T> (IEnumerable<T> query, params ObjectID[] expectedObjectIDs)
        where T : DomainObject
    {
      T[] results = query.ToArray ();
      T[] expected = GetExpectedObjects<T> (expectedObjectIDs);
      Assert.That (results, Is.EquivalentTo (expected));
    }

    private static T[] GetExpectedObjects<T> (params ObjectID[] expectedObjectIDs)
        where T: DomainObject
    {
      return (from id in expectedObjectIDs select (id == null ? null : (T) RepositoryAccessor.GetObject (id, false))).ToArray();
    }

    private void CheckDataContainersRegistered (params ObjectID[] objectIDs)
    {
      // check that related objects have been loaded

      foreach (var id in objectIDs)
        Assert.That (ClientTransactionMock.DataManager.DataContainerMap[id], Is.Not.Null);
    }

    private void CheckCollectionRelationRegistered (ObjectID originatingObjectID, string shortPropertyName, bool checkOrdering, params ObjectID[] expectedRelatedObjectIDs)
    {
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (
              originatingObjectID.ClassDefinition.ClassType.FullName + "." + shortPropertyName);

      var collectionEndPoint = (CollectionEndPoint)
                               ClientTransactionMock.DataManager.RelationEndPointMap[
                                   new RelationEndPointID (originatingObjectID, relationEndPointDefinition)];
      Assert.That (collectionEndPoint, Is.Not.Null);

      var expectedRelatedObjects = expectedRelatedObjectIDs.Select (id => RepositoryAccessor.GetObject (id, false)).ToArray();
      if (checkOrdering)
        Assert.That (collectionEndPoint.OppositeDomainObjects, Is.EqualTo (expectedRelatedObjects));
      else
        Assert.That (collectionEndPoint.OppositeDomainObjects, Is.EquivalentTo (expectedRelatedObjects));
    }

    private void CheckObjectRelationRegistered (ObjectID originatingObjectID, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var declaringType = originatingObjectID.ClassDefinition.ClassType;
      CheckObjectRelationRegistered(originatingObjectID, declaringType, shortPropertyName, expectedRelatedObjectID);
    }

    private void CheckObjectRelationRegistered (ObjectID originatingObjectID, Type declaringType, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var longPropertyName = declaringType.FullName + "." + shortPropertyName;
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (
              longPropertyName);

      var objectEndPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
                                                new RelationEndPointID (originatingObjectID, relationEndPointDefinition)];
      Assert.That (objectEndPoint, Is.Not.Null);
      Assert.That (objectEndPoint.OppositeObjectID, Is.EqualTo (expectedRelatedObjectID));
    }
  }
}
