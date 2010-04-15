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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class MappingResolverTest
  {
    private MappingResolver _resolver;
    private UniqueIdentifierGenerator _generator;
    private SqlTable _orderTable;
    private SqlTable _customerTable;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new MappingResolver();
      _generator = new UniqueIdentifierGenerator();
      _orderTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Order), "Order", "o"));
      _customerTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Customer), "Customer", "c"));
    }

    [Test]
    public void ResolveTableReferenceExpression ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_orderTable);

      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);

      var primaryKeyColumn = new SqlColumnExpression (typeof (ObjectID), "o", "ID");
      var starColumn = new SqlColumnExpression (typeof (Order), "o", "*");

      var expectedExpression = new SqlEntityExpression (typeof (Order), primaryKeyColumn, starColumn);

      ExpressionTreeComparer.CheckAreEqualTrees (sqlEntityExpression, expectedExpression);
    }

    [Test]
    public void ResolveTableReferenceExpression_SubStatementTableInfo_TableTypeNotInheritedFromDomainObject ()
    {
      var sqlStatement = new SqlStatement (Expression.Constant ("test"), new SqlTable[] { }, new Ordering[] { }, null, null, false, false);
      var tableInfo = new ResolvedSubStatementTableInfo (typeof (Student), "Student",sqlStatement);
      var sqlTable = new SqlTable (tableInfo);
      var tableReferenceExpression = new SqlTableReferenceExpression (sqlTable);

      var sqlValueTableReferenceExpression =
          (SqlValueTableReferenceExpression) _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);

      Assert.That (((SqlTable) sqlValueTableReferenceExpression.SqlTable).TableInfo, Is.EqualTo (tableInfo));
    }

    [Test]
    public void ResolveTableReferenceExpression_NoTable_InSubStatement ()
    {
      var selectProjection = new SqlColumnExpression (typeof (int), "o", "OrderNo");
      var sqlTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Order), "Order", "o"));
      var sqlStatement = new SqlStatement (selectProjection, new[] { sqlTable }, new Ordering[] { }, null, null, false, false);

      var subStatementTable = new SqlTable (new ResolvedSubStatementTableInfo (typeof (string), "q", sqlStatement));
      var tableReferenceExpression = new SqlTableReferenceExpression (subStatementTable);
      var result = _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);

      var fakeTable = new SqlTable (subStatementTable.GetResolvedTableInfo());

      Assert.That (result, Is.TypeOf (typeof (SqlValueTableReferenceExpression)));
      Assert.That (((SqlTable) ((SqlValueTableReferenceExpression) result).SqlTable).TableInfo, Is.EqualTo (fakeTable.TableInfo));
    }

    [Test]
    public void ResolveTableInfo ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Order));
      var resolvedTableInfo = (ResolvedSimpleTableInfo) _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);

      Assert.That (resolvedTableInfo, Is.Not.Null);
      Assert.That (resolvedTableInfo.TableName, Is.EqualTo ("OrderView"));
      Assert.That (resolvedTableInfo.TableAlias, Is.EqualTo ("t0"));
      Assert.That (resolvedTableInfo.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveTableInfo_NoDomainObject_ThrowsException ()
    {
      var unresolvedTableInfo = new UnresolvedTableInfo (typeof (Student));

      _resolver.ResolveTableInfo (unresolvedTableInfo, _generator);
    }

    [Test]
    public void ResolveJoinInfo ()
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (_customerTable, typeof (Customer).GetProperty ("Orders"), JoinCardinality.Many);

      var resolvedJoinInfo = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (resolvedJoinInfo, Is.Not.Null);
      Assert.That (resolvedJoinInfo.ItemType, Is.EqualTo (typeof (Order)));

      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableName, Is.EqualTo ("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableAlias, Is.EqualTo ("t0"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).ItemType, Is.EqualTo (typeof (Order)));

      Assert.That (resolvedJoinInfo.LeftKeyColumn.ColumnName, Is.EqualTo ("ID"));
      Assert.That (resolvedJoinInfo.LeftKeyColumn.OwningTableAlias, Is.EqualTo ("c"));
      Assert.That (resolvedJoinInfo.LeftKeyColumn.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (resolvedJoinInfo.RightKeyColumn.ColumnName, Is.EqualTo ("CustomerID"));
      Assert.That (resolvedJoinInfo.RightKeyColumn.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (resolvedJoinInfo.RightKeyColumn.OwningTableAlias, Is.EqualTo ("t0"));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Order.OrderNumber' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityOne_ThrowsException ()
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (_orderTable, typeof (Order).GetProperty ("OrderNumber"), JoinCardinality.One);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }


    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Student.Scores' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityMany_ThrowsExcpetion ()
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (_orderTable, typeof (Student).GetProperty ("Scores"), JoinCardinality.Many);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    public void ResolveConstantExpression_ConstantExpression ()
    {
      var constantExpression = Expression.Constant (10);

      var expression = _resolver.ResolveConstantExpression (constantExpression);

      Assert.That (expression, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (expression, Is.SameAs (constantExpression));
    }

    [Test]
    public void ResolveConstantExpression_EntityExpression ()
    {
      Order order;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
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
      var memberExpression = new SqlMemberExpression (_orderTable, property);

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveMemberExpression_RedirectedProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");
      var memberExpression = new SqlMemberExpression (_orderTable, property);

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlEntityRefMemberExpression ()
    {
      var property = typeof (Order).GetProperty ("Customer");
      var memberExpression = new SqlMemberExpression (_orderTable, property);

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.SqlTable, Is.SameAs (_orderTable));
    }

    [Test]
    public void ResolveMemberExpression_CardibalityOne_MemberIsTheNonForeignKeySide ()
    {
      var property = typeof (Employee).GetProperty ("Computer");
      var memberExpression = new SqlMemberExpression (_customerTable, property);

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (memberExpression, _generator);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.SqlTable, Is.SameAs (_customerTable));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The type 'Student' declaring member 'First' does not identify a queryable table.")]
    public void ResolveMemberExpression_InvalidDeclaringType_ThrowsUnmappedItemException ()
    {
      var property = typeof (Student).GetProperty ("First");
      var memberExpression = new SqlMemberExpression (_orderTable, property);

      _resolver.ResolveMemberExpression (memberExpression, _generator);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The member 'Order.NotInMapping' does not have a queryable database mapping.")]
    public void ResolveMemberExpression_InvalidMember_ThrowsUnmappedItemException ()
    {
      var property = typeof (Order).GetProperty ("NotInMapping");
      var memberExpression = new SqlMemberExpression (_orderTable, property);

      _resolver.ResolveMemberExpression (memberExpression, _generator);
    }
  }
}