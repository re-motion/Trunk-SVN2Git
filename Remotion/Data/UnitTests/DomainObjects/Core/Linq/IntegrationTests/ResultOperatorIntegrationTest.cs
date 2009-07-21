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
  public class ResultOperatorIntegrationTest : IntegrationTestBase
  {
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
    public void QueryWithContains_Like ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo>()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
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
    public void Query_WithCastOnResultSet ()
    {
      var query =
          (from o in QueryFactory.CreateLinqQuery<Order>()
           where o.OrderNumber == 1
           select o).Cast<TestDomainBase>();

      CheckQueryResult (query, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithFirst ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() 
                   select o).First();
      Assert.That (query, Is.EqualTo ((TestDomainBase.GetObject (DomainObjectIDs.InvalidOrder))));
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
    public void QueryDistinctTest ()
    {
      var query = (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber == 4
                   select o ).Distinct();
      query.Single();

      Assert.That (query, Is.InstanceOfType (typeof(DomainObjectQueryable<Order>)));
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
    [Ignore ("TODO 1313")]
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
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
           where oi.Order == o
           select o).Distinct();

      CheckQueryResult (orders, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4);
    }
  }
}