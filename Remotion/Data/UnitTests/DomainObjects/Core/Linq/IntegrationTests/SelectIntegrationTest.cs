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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class SelectIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void SimpleQuery ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer> ()
          select c;
      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3, DomainObjectIDs.Computer4,
                        DomainObjectIDs.Computer5);
    }

    [Test]
    public void SimpleQuery_WithRelatedEntity ()
    {
      var query =
          from ot in QueryFactory.CreateLinqQuery<OrderTicket> ()
          select ot.Order;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5,
                        DomainObjectIDs.Order2, DomainObjectIDs.OrderWithoutOrderItems);
    }

    [Test]
    public void Query_WithView ()
    {
      var domainBases =
          from d in QueryFactory.CreateLinqQuery<TIDomainBase>()
          select d;

      Assert.That (domainBases.ToArray(), Is.Not.Empty);
    }

    [Test]
    public void MethodCallOnCoalesceExpression ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where (oi.Product ?? oi.Order.Customer.Name).ToUpper() == "BLUMENTOPF"
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem5);
    }

    [Test]
    public void MethodCallOnConditionalExpression ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
                  where (oi.Product == "Blumentopf" ? oi.Product : oi.Order.Customer.Name).ToUpper () == "BLUMENTOPF"
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem5);
    }

    [Test]
    public void LogicalMemberAccessOnCoalesceExpression ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
                  where (oi.Product ?? oi.Order.Customer.Name).Length == 10
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem5);
    }

    [Test]
    public void LogicalMemberAccessOnConditionalExpression ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
                  where (oi.Product == "Blumentopf" ? oi.Product : oi.Order.Customer.Name).Length == 10
                  select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem5);
    }

    [Test]
    public void ColumnMemberAccessOnCoalesceExpression ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee> ()
                  where (e.Computer ?? (DomainObject) e).ID == DomainObjectIDs.Employee2
                  select e;

      CheckQueryResult (query, DomainObjectIDs.Employee2);
    }

    [Test]
    public void ColumnMemberAccessOnConditionalExpression ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee> ()
                  where (e.Computer.ID == DomainObjectIDs.Computer1 ? e.Computer : (DomainObject) e).ID == DomainObjectIDs.Computer1
                  select e;

      CheckQueryResult (query, DomainObjectIDs.Employee3);
    }

    [Test]
    public void Query_WithConstant ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber == 1
          select 1).Single();

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Type 'ObjectID' ist not supported by this storage provider.\r\n"
        + "Please select the ID and ClassID values separately, then create an ObjectID with it in memory "
        + "(e.g., 'select new ObjectID (o.ID.ClassID, o.ID.Value)').")]
    public void Query_WithObjectID ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber == 1
          select o.ID;

      query.ToArray ();
    }

  }
}
