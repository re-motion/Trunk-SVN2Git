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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

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
          from o in QueryFactory.CreateLinqQuery<OrderTicket> ()
          select o.Order;
      CheckQueryResult (query, DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order4,
                        DomainObjectIDs.OrderWithoutOrderItem);
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
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "This query provider does not support the given query ('from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select 1'). "
        + "re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.")]
    public void Query_WithUnsupportedType_Constant ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber == 1
          select 1;

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "This query provider does not support the given query "
        + "('from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o].ID'). re-store only supports queries selecting a "
        + "scalar value, a single DomainObject, or a collection of DomainObjects.")]
    public void Query_WithUnsupportedType_NonDomainObjectColumn ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order> ()
          where o.OrderNumber == 1
          select o.ID;

      query.ToArray ();
    }

    // TODO Review 3309: Add a new MethodCallTest and move tests for method calls over there

    [Test]
    public void Trim ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>() where oi.Product.Trim()=="CPU Fan" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    // TODO Review 3309: This is not correct, Insert (1, "Test") should return ("CTestPU Fan").
    [Test]
    public void Insert ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Product.Insert(1, "Test") == "TestCPU Fan" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    [Ignore ("TODO Review 3309: This doesn't work because STUFF can't be used when the index is too large => CASE WHEN is missing.")]
    public void Insert_AtEndOfString ()
    {
      // TODO Review 3309: Should actually be Insert (7, "Test") when the bug described above is solved.
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Product.Insert (8, "Test") == "CPU FanTest" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

  }
}
