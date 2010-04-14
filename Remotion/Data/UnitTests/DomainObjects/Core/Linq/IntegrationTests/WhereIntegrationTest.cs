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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class WhereIntegrationTest : IntegrationTestBase
  {
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
    public void QueryWithContains_Like ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo> ()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
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
    public void QueryWithWhere_BooleanPropertyOnly()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.BooleanProperty
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanProperty_ExplicitComparison()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
// ReSharper disable RedundantBoolCompare
          where e.BooleanProperty == true
// ReSharper restore RedundantBoolCompare
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyOnly_Negate()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where !e.BooleanProperty
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother ()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
          where e.Int32Property == -2147483647 && e.BooleanProperty
          select e;

      CheckQueryResult (objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes2);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_Negate()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.Int32Property == 2147483647 && !e.BooleanProperty
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes1);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_ExplicitComparison_True()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
// ReSharper disable RedundantBoolCompare
          where e.Int32Property == -2147483647 && e.BooleanProperty == true
// ReSharper restore RedundantBoolCompare
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes2);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_ExplicitComparison_False()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.Int32Property == 2147483647 && e.BooleanProperty == false
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes1);
    }

    [Test]
    public void QueryWithWhere_LessThan ()
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

      CheckQueryResult (employees, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5);

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
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This query provider does not support the given query "
      +"('from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select new <>f__AnonymousTypef`2(o = [o], Customer = [o].Customer)'). "
      +"re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.")]
    public void Query_WithUnsupportedType_NewObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select new { o, o.Customer };

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This query provider does not support the given query "
      +"('from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select 1'). "
      +"re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.")]
    public void Query_WithUnsupportedType_Constant ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select 1;

      query.ToArray ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage=
        "This query provider does not support the given query "
        +"('from Order o in DomainObjectQueryable<Order> where ([o].OrderNumber = 1) select [o].ID'). "
        +"re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.")]
    public void Query_WithUnsupportedType_NonDomainObjectColumn ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select o.ID;

      query.ToArray ();
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
    public void TableInheritance_AccessingPropertyFromBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TableInheritance.TestDomain.ClassWithUnidirectionalRelation> ()
                  where c.DomainBase.CreatedAt == new DateTime (2006, 01, 03)
                  select c;
      CheckQueryResult (query, new TableInheritance.DomainObjectIDs ().ClassWithUnidirectionalRelation);
    }

    [Test]
    public void QueryWithCustomParser ()
    {
      foreach (StorageProviderDefinition definition in DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions)
      {
        Assert.That (
            definition.LinqSqlGenerator.DetailParserRegistries.WhereConditionParser.GetParsers (typeof (ConditionalExpression)).Any (), 
            Is.False, 
            "Choose another expression type for the test; ConditionalExpression is already supported.");
        ConditionalExpressionWhereConditionParser.Register (definition);
      }

      var query1 = GetQueryWithIif (true);
      var query2 = GetQueryWithIif (false);

      CheckQueryResult (query1, DomainObjectIDs.Order1);
      CheckQueryResult (
          query2, 
          DomainObjectIDs.Order1, 
          DomainObjectIDs.Order2, 
          DomainObjectIDs.Order3, 
          DomainObjectIDs.Order4, 
          DomainObjectIDs.OrderWithoutOrderItem, 
          DomainObjectIDs.InvalidOrder);
    }

    private IQueryable<Order> GetQueryWithIif (bool selectNumberOne)
    {
      return from o in QueryFactory.CreateLinqQuery<Order> ()
             where o.OrderNumber == (selectNumberOne ? 1 : o.OrderNumber)
             select o;
    }
  }
}
