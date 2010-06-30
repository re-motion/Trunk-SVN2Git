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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.InheritanceRootSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class MappingResolverTest
  {
    private MappingResolver _resolver;
    private UniqueIdentifierGenerator _generator;
    private SqlTable _orderTable;
    private SqlTable _companyTable;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new MappingResolver();
      _generator = new UniqueIdentifierGenerator();
      _orderTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Order), "Order", "o"));
      _companyTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (Company), "Company", "c"));
    }

    [Test]
    public void ResolveTableReferenceExpression ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_orderTable);

      var sqlEntityExpression =
          (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (tableReferenceExpression.SqlTable.GetResolvedTableInfo(), _generator);

      var primaryKeyColumn = new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "ID", true);
      var starColumn = new SqlColumnDefinitionExpression (typeof (Order), "o", "*", false);

      var expectedExpression = new SqlEntityDefinitionExpression (typeof (Order), "o", null, primaryKeyColumn, starColumn);

      ExpressionTreeComparer.CheckAreEqualTrees (sqlEntityExpression, expectedExpression);
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
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, typeof (Customer).GetProperty ("Orders"), JoinCardinality.Many);

      var resolvedJoinInfo = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (resolvedJoinInfo, Is.Not.Null);
      Assert.That (resolvedJoinInfo.ItemType, Is.EqualTo (typeof (Order)));

      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableName, Is.EqualTo ("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).TableAlias, Is.EqualTo ("t0"));
      Assert.That (((ResolvedSimpleTableInfo) resolvedJoinInfo.ForeignTableInfo).ItemType, Is.EqualTo (typeof (Order)));

      Assert.That (((SqlColumnExpression) resolvedJoinInfo.LeftKey).ColumnName, Is.EqualTo ("ID"));
      Assert.That (((SqlColumnExpression) resolvedJoinInfo.LeftKey).OwningTableAlias, Is.EqualTo ("c"));
      Assert.That (resolvedJoinInfo.LeftKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) resolvedJoinInfo.LeftKey).IsPrimaryKey, Is.True);
      Assert.That (((SqlColumnExpression) resolvedJoinInfo.RightKey).ColumnName, Is.EqualTo ("CustomerID"));
      Assert.That (resolvedJoinInfo.RightKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) resolvedJoinInfo.RightKey).OwningTableAlias, Is.EqualTo ("t0"));
      Assert.That (((SqlColumnExpression) resolvedJoinInfo.RightKey).IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveJoinInfo_WithMixedRelationProperty ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
        typeof (TargetClassForPersistentMixin), "m", null, new SqlColumnDefinitionExpression (typeof (int), "m", "ID", false));
      var memberInfo = typeof (IMixinAddingPeristentProperties).GetProperty ("RelationProperty");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, memberInfo, JoinCardinality.One);

      var resolvedJoinInfo = _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);

      Assert.That (resolvedJoinInfo, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), 
      ExpectedMessage = "The type 'Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain.IMixinAddingPeristentProperties' "
                       +"does not identify a queryable table.")]
    public void ResolveJoinInfo_UnknownType ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
        typeof (IMixinAddingPeristentProperties), "m", null, new SqlColumnDefinitionExpression (typeof (int), "m", "ID", false));
      var memberInfo = typeof (IMixinAddingPeristentProperties).GetProperty ("RelationProperty");
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, memberInfo, JoinCardinality.One);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Order.OrderNumber' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityOne_ThrowsException ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, typeof (Order).GetProperty ("OrderNumber"), JoinCardinality.One);

      _resolver.ResolveJoinInfo (unresolvedJoinInfo, _generator);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Student.Scores' does not identify a relation.")]
    public void ResolveJoinInfo_NoRelation_CardinalityMany_ThrowsExcpetion ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      var unresolvedJoinInfo = new UnresolvedJoinInfo (entityExpression, typeof (Student).GetProperty ("Scores"), JoinCardinality.Many);

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
    public void ResolveMemberExpression_ReturnsSqlColumnDefinitionExpression ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression, Is.TypeOf(typeof(SqlColumnDefinitionExpression)));
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
      Assert.That (sqlColumnExpression.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveMemberExpression_IDMember ()
    {
      var property = typeof (Order).GetProperty ("ID");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("ID"));
      Assert.That (sqlColumnExpression.IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveMemberExpression_RedirectedProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var sqlColumnExpression = (SqlColumnExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlColumnExpression, Is.Not.Null);
      Assert.That (sqlColumnExpression.ColumnName, Is.EqualTo ("OrderNo"));
      Assert.That (sqlColumnExpression.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveMemberExpression_MixedRelationProperty ()
    {
      var property = typeof (IMixinAddingPeristentProperties).GetProperty ("RelationProperty");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (TargetClassForPersistentMixin), "c", null, new SqlColumnDefinitionExpression (typeof (int), "m", "ID", false));

      var result = _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.TypeOf (typeof (SqlEntityRefMemberExpression)));
      Assert.That (((SqlEntityRefMemberExpression) result).MemberInfo, Is.SameAs (property));
      Assert.That (((SqlEntityRefMemberExpression) result).OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The member 'Order.OriginalCustomer' does not have a queryable database mapping.")]
    public void ResolvememberExpression_NotAMappedMember ()
    {
      var property = typeof (Order).GetProperty ("OriginalCustomer");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlEntityRefMemberExpression ()
    {
      var property = typeof (Order).GetProperty ("Customer");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    public void ResolveMemberExpression_CardinalityOne_MemberIsTheNonForeignKeySide ()
    {
      var property = typeof (Employee).GetProperty ("Computer");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Employee), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var sqlEntityRefMemberExpression = (SqlEntityRefMemberExpression) _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (sqlEntityRefMemberExpression, Is.Not.Null);
      Assert.That (sqlEntityRefMemberExpression.MemberInfo, Is.SameAs (property));
      Assert.That (sqlEntityRefMemberExpression.OriginatingEntity, Is.SameAs (entityExpression));
    }

    [Test]
    public void ResolveMemberExpression_PropertyAboveInheritanceRoot ()
    {
      var property = typeof (StorageGroupClass).GetProperty ("AboveInheritanceIdentifier");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (StorageGroupClass), "s", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = _resolver.ResolveMemberExpression (entityExpression, property);

      Assert.That (result, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      Assert.That (((SqlColumnDefinitionExpression) result).OwningTableAlias, Is.EqualTo ("s"));
      Assert.That (((SqlColumnDefinitionExpression) result).ColumnName, Is.EqualTo ("AboveInheritanceIdentifier"));
      Assert.That (result.Type, Is.EqualTo (typeof (string)));
    }


    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The type 'Remotion.Data.DomainObjects.DomainObject' does not identify a queryable table.")]
    public void ResolveMemberExpression_InvalidDeclaringType_ThrowsUnmappedItemException ()
    {
      var property = typeof (Student).GetProperty ("First");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (DomainObject), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException),
        ExpectedMessage = "The member 'Order.NotInMapping' does not have a queryable database mapping.")]
    public void ResolveMemberExpression_InvalidMember_ThrowsUnmappedItemException ()
    {
      var property = typeof (Order).GetProperty ("NotInMapping");
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _resolver.ResolveMemberExpression (entityExpression, property);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = "The member 'Order.NotInMapping' does not identify a mapped property.")]
    public void ResolveMemberExpression_OnColumn_UnMappedProperty ()
    {
      var property = typeof (Order).GetProperty ("NotInMapping");
      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "o", "Name", false);

      _resolver.ResolveMemberExpression (columnExpression, property);
    }

    [Test]
    public void ResolveMemberExpression_OnColumn_WithPropertyClassID ()
    {
      var property = typeof (ObjectID).GetProperty ("ClassID");
      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "o", "Name", false);

      var result = _resolver.ResolveMemberExpression (columnExpression, property);

      Assert.That (result, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      Assert.That (((SqlColumnDefinitionExpression) result).OwningTableAlias, Is.EqualTo (columnExpression.OwningTableAlias));
      Assert.That (((SqlColumnDefinitionExpression) result).ColumnName, Is.EqualTo ("ClassID"));
      Assert.That (((SqlColumnDefinitionExpression) result).IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveMemberExpression_ReturnsSqlColumnReferenceExpression ()
    {
      var property = typeof (ObjectID).GetProperty ("ClassID");
      var referencedEntity = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (int), "o", "ID", true));

      var sqlColumnReferenceExpression = new SqlColumnReferenceExpression (typeof (string), "c", "Name", false, referencedEntity);

      var result = (SqlColumnExpression) _resolver.ResolveMemberExpression (sqlColumnReferenceExpression, property);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (SqlColumnReferenceExpression)));
      Assert.That (result.ColumnName, Is.EqualTo("ClassID"));
      Assert.That (result.OwningTableAlias, Is.EqualTo (sqlColumnReferenceExpression.OwningTableAlias));
      Assert.That (result.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveTypeCheck_DesiredTypeIsAssignableFromExpressionType ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_companyTable);
      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (tableReferenceExpression.SqlTable.GetResolvedTableInfo(), _generator);

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Company));

      Assert.That (result, Is.TypeOf (typeof (ConstantExpression)));
    }

    [Test]
    public void ResolveTypeCheck_NoDomainTypeWithSameDesiredType ()
    {
      var sqlEntityExpression = new SqlEntityDefinitionExpression (
          typeof (Student), "t", null, new SqlColumnDefinitionExpression (typeof (string), "t", "Name", false));
      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));

      ExpressionTreeComparer.CheckAreEqualTrees (result, Expression.Constant (true));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException))]
    public void ResolveTypeCheck_NoDomainTypeWithDifferentDesiredType ()
    {
      var sqlEntityExpression = new SqlEntityDefinitionExpression (
          typeof (object), "t", null, new SqlColumnDefinitionExpression (typeof (string), "t", "Name", false));
      _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));
    }

    [Test]
    public void ResolveTypeCheck_NoQueryableTable ()
    {
      var sqlTable = new SqlTable (new ResolvedSimpleTableInfo (typeof (AboveInheritanceRootClass), "Table", "t"));
      var tableReferenceExpression = new SqlTableReferenceExpression (sqlTable);
      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (tableReferenceExpression.SqlTable.GetResolvedTableInfo(), _generator);

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (StorageGroupClass));

      var idExpression = Expression.MakeMemberAccess (sqlEntityExpression, typeof (DomainObject).GetProperty ("ID"));
      var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
      var expectedTree = Expression.Equal (classIDExpression, new SqlLiteralExpression ("StorageGroupClass"));

      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedTree);
    }

    [Test]
    public void ResolveTypeCheck_DesiredTypeNotRelatedWithExpressionType ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_companyTable);
      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (tableReferenceExpression.SqlTable.GetResolvedTableInfo(), _generator);

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Student));

      ExpressionTreeComparer.CheckAreEqualTrees (result, Expression.Constant (false));
    }

    [Test]
    public void ResolveTypeCheck_ExpressionTypeIsAssignableFromDesiredType ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_companyTable);
      var sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveSimpleTableInfo (tableReferenceExpression.SqlTable.GetResolvedTableInfo(), _generator);

      var result = _resolver.ResolveTypeCheck (sqlEntityExpression, typeof (Customer));

      var idExpression = Expression.MakeMemberAccess (sqlEntityExpression, typeof (DomainObject).GetProperty ("ID"));
      var classIDExpression = Expression.MakeMemberAccess (idExpression, typeof (ObjectID).GetProperty ("ClassID"));
      var expectedExpression = Expression.Equal (classIDExpression, new SqlLiteralExpression ("Customer"));

      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }
  }
}