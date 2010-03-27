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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq.TestDomain;
using System.Linq.Expressions;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class MappingResolverTest
  {
    private MappingResolver _resolver;
    private UniqueIdentifierGenerator _generator;
    private SqlTable _sqlTable;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new MappingResolver();
      _generator = new UniqueIdentifierGenerator();
      _sqlTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Order), "Order", "o")); // TODO Review 2439: Rename to _orderTable
    }

    [Test]
    public void ResolveTableReferenceExpression ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression(_sqlTable);

      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);

      var primaryKeyColumn = new SqlColumnExpression (typeof (ObjectID), "o", "ID");
      var column1 = new SqlColumnExpression (typeof (string), "o", "ClassID");
      var column2 = new SqlColumnExpression (typeof (DateTime), "o", "Timestamp");
      var column3 = new SqlColumnExpression (typeof (int), "o", "OrderNo");
      var column4 = new SqlColumnExpression (typeof (DateTime), "o", "DeliveryDate");
      var column5 = new SqlColumnExpression (typeof (ObjectID), "o", "OfficialID");
      var column6 = new SqlColumnExpression (typeof (ObjectID), "o", "CustomerID");

      // TODO Review 2439: Create an expectedExpresion with above columns, then use ExpressionTreeComparer to compare the sqlEntityExpression to the expectedExpresion instead of manually comparing each column

      Assert.That (sqlEntityExpression, Is.Not.Null);
      Assert.That (sqlEntityExpression.ProjectionColumns.Count, Is.EqualTo (7));

      Assert.That (sqlEntityExpression.PrimaryKeyColumn.ColumnName, Is.EqualTo (primaryKeyColumn.ColumnName));
      Assert.That (sqlEntityExpression.PrimaryKeyColumn.OwningTableAlias, Is.EqualTo (primaryKeyColumn.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[0].ColumnName, Is.EqualTo (primaryKeyColumn.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[0].OwningTableAlias, Is.EqualTo (primaryKeyColumn.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[1].ColumnName, Is.EqualTo (column1.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[1].OwningTableAlias, Is.EqualTo (column1.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[2].ColumnName, Is.EqualTo (column2.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[2].OwningTableAlias, Is.EqualTo (column2.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[3].ColumnName, Is.EqualTo (column3.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[3].OwningTableAlias, Is.EqualTo (column3.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[4].ColumnName, Is.EqualTo (column4.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[4].OwningTableAlias, Is.EqualTo (column4.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[5].ColumnName, Is.EqualTo (column5.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[5].OwningTableAlias, Is.EqualTo (column5.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[6].ColumnName, Is.EqualTo (column6.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[6].OwningTableAlias, Is.EqualTo (column6.OwningTableAlias));      
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The type 'Student' does not identify a queryable table.")]
    public void ResolveTableReferenceExpression_NoTable_ThrowsException ()
    {
      var sqlTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Student), "Student", "s"));
      var tableReferenceExpression = new SqlTableReferenceExpression (sqlTable);
      _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);
    }

    [Test]
    public void ResolveTableInfo ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Order));

      var resolvedTableInfo = (ResolvedSimpleTableInfo) _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);

      Assert.That (resolvedTableInfo, Is.Not.Null);
      Assert.That (resolvedTableInfo.TableName, Is.EqualTo ("OrderView"));
      Assert.That (resolvedTableInfo.TableAlias, Is.EqualTo("t0"));
      // TODO Review 2439: Check item type
    }

    [Test]
    [ExpectedException(typeof(UnmappedItemException))]
    public void ResolveTableInfo_NoDomainObject_ThrowsException ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Student));

      _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);
    }

    [Test]
    public void ResolveJoinInfo ()
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (_sqlTable, typeof (Customer).GetProperty ("Orders"), JoinCardinality.Many); // TODO Review 2439: Use a _customerTable here

      var resolvedJoinInfo = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (resolvedJoinInfo, Is.Not.Null);
      Assert.That (resolvedJoinInfo.ItemType, Is.EqualTo (typeof (Order)));
      
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableName, Is.EqualTo ("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableAlias, Is.EqualTo ("t0"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).ItemType, Is.EqualTo (typeof(Order)));

      // TODO Review 2439: also test types and aliases of key columns
      Assert.That (resolvedJoinInfo.PrimaryColumn.ColumnName, Is.EqualTo ("ID"));
      Assert.That (resolvedJoinInfo.ForeignColumn.ColumnName, Is.EqualTo ("CustomerID"));
    }

    // TODO Review 2439: Also test with Order.OrderNumber
    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Student.Scores' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_ThrowsExcpetion ()
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (_sqlTable, typeof (Student).GetProperty ("Scores"), JoinCardinality.Many);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    public void ResolveConstantExpression_ConstantExpression ()
    {
      var constantExpression = Expression.Constant (10);

      var expression = _resolver.ResolveConstantExpression(constantExpression);

      Assert.That (expression, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (expression, Is.SameAs (constantExpression));
    }

    [Test]
    public void ResolveConstantExpression_EntityExpression ()
    {
      Order order;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        order = Order.NewObject();
      }
      var constantExpression = Expression.Constant (order);

      var expression = _resolver.ResolveConstantExpression (constantExpression);

      Assert.That (expression, Is.TypeOf (typeof (SqlEntityConstantExpression)));
      Assert.That (((SqlEntityConstantExpression) expression).Value, Is.EqualTo (order));
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlColumnExpression ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var memberExpression = new SqlMemberExpression (_sqlTable, property);

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      // TODO Review 2439: test table alias
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveMemberExpression_RedirectedProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");
      var memberExpression = new SqlMemberExpression (_sqlTable, property);

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlEntityRefMemberExpression ()
    {
      var property = typeof (Order).GetProperty ("Customer");
      var memberExpression = new SqlMemberExpression (_sqlTable, property);

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      // TODO Review 2439: Test properties
      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);      
    }

    // TODO Review 2439: Test with non-key property, e.g., Order.OrderTicket

    // TODO Review 2439: check exception message
    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveMemberExpression_InvalidDeclaringType_ThrowsUnmappedItemExpression ()
    {
      var property = typeof (Student).GetProperty ("First");
      var memberExpression = new SqlMemberExpression (_sqlTable, property);

      _resolver.ResolveMemberExpression (memberExpression, _generator);
    }

    // TODO Review 2439: Also test for invalid member, e.g., Order.NotInMapping
  }
}