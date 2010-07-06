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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.Core;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation
{
  [TestFixture]
  public class SqlPreparationContextTest
  {
    private ISqlPreparationContext _context;
    private MainFromClause _source;
    private SqlTable _sqlTable;
    private SqlPreparationContext _parentContext;
    private MainFromClause _parentSource;
    private SqlTable _parentSqlTable;
    private TestableSqlPreparationQueryModelVisitor _visitor;
    private ISqlPreparationStage _stageMock;
    private ISqlPreparationContext _contextWithParent;

    [SetUp]
    public void SetUp ()
    {
      _context = new SqlPreparationContext();
      _source = ExpressionHelper.CreateMainFromClause_Cook();
      var source = new UnresolvedTableInfo (typeof (int));
      _sqlTable = new SqlTable (source);

      _parentContext = new SqlPreparationContext();
      _parentSource = ExpressionHelper.CreateMainFromClause_Cook();
      _parentSqlTable = new SqlTable (new UnresolvedTableInfo (typeof (int)));
      _stageMock = MockRepository.GenerateStrictMock<ISqlPreparationStage>();
      _visitor = new TestableSqlPreparationQueryModelVisitor (
          _parentContext, _stageMock, new UniqueIdentifierGenerator(), ResultOperatorHandlerRegistry.CreateDefault());
      _contextWithParent = new SqlPreparationContext (_parentContext, _visitor);
    }

    [Test]
    public void AddExpressionMapping ()
    {
      _context.AddExpressionMapping (new QuerySourceReferenceExpression (_source), new SqlTableReferenceExpression (_sqlTable));
      Assert.That (_context.TryGetExpressionMapping (new QuerySourceReferenceExpression (_source)), Is.Not.Null);
    }

    [Test]
    public void GetExpressionMapping ()
    {
      var querySourceReferenceExpression = new QuerySourceReferenceExpression (_source);
      _context.AddExpressionMapping (querySourceReferenceExpression, new SqlTableReferenceExpression (_sqlTable));
      Assert.That (
          ((SqlTableReferenceExpression) _context.TryGetExpressionMapping (querySourceReferenceExpression)).SqlTable,
          Is.SameAs (_sqlTable));
    }

    [Test]
    public void TryGetExpressiontMappingFromHierarchy ()
    {
      var querySourceReferenceExpression = new QuerySourceReferenceExpression (_source);
      var sqlTableReferenceExpression = new SqlTableReferenceExpression (_sqlTable);
      _context.AddExpressionMapping (querySourceReferenceExpression, sqlTableReferenceExpression);

      Expression result = _context.TryGetExpressionMappingFromHierarchy (querySourceReferenceExpression);

      Assert.That (result, Is.SameAs (sqlTableReferenceExpression));
    }

    [Test]
    public void GetExpressionMapping_GetFromParentContext ()
    {
      var querySourceReferenceExpression = new QuerySourceReferenceExpression (_parentSource);
      var sqlTableReferenceExpression = new SqlTableReferenceExpression (_parentSqlTable);
      _parentContext.AddExpressionMapping (querySourceReferenceExpression, sqlTableReferenceExpression);
      Assert.That (_contextWithParent.TryGetExpressionMapping (querySourceReferenceExpression), Is.SameAs (sqlTableReferenceExpression));
    }

    [Test]
    public void TryGetExpressionMappingFromHierarchy_GetFromParentContext ()
    {
      var querySourceReferenceExpression = new QuerySourceReferenceExpression (_parentSource);
      var sqlTableReferenceExpression = new SqlTableReferenceExpression (_parentSqlTable);
      _parentContext.AddExpressionMapping (querySourceReferenceExpression, sqlTableReferenceExpression);

      Expression result = _contextWithParent.TryGetExpressionMappingFromHierarchy (querySourceReferenceExpression);

      Assert.That (result, Is.SameAs (sqlTableReferenceExpression));
    }

    [Test]
    public void GetExpressionMapping_GroupJoinClause ()
    {
      var groupJoinClause = ExpressionHelper.CreateGroupJoinClause();
      var preparedExpression = Expression.Constant (0);
      var preparedSqlTable = SqlStatementModelObjectMother.CreateSqlTable();
      var preparedFromExpressionInfo = new FromExpressionInfo (
          preparedSqlTable, new Ordering[] { }, new SqlTableReferenceExpression (preparedSqlTable), null, false);

      _stageMock
        .Expect (
          mock => mock.PrepareFromExpression (
              Arg<Expression>.Matches (e => e == groupJoinClause.JoinClause.InnerSequence),
              Arg<ISqlPreparationContext>.Matches (c => c != _context), Arg < Func<ITableInfo, SqlTableBase>>.Is.Anything))
         .Return (preparedFromExpressionInfo);
      _stageMock
          .Expect (
              mock =>
              mock.PrepareWhereExpression (
                  Arg<Expression>.Matches (
                      e =>
                      ((BinaryExpression) e).Left == groupJoinClause.JoinClause.OuterKeySelector
                      && ((BinaryExpression) e).Right == groupJoinClause.JoinClause.InnerKeySelector),
                  Arg<ISqlPreparationContext>.Matches (c => c != _context)))
          .Return (preparedExpression);
      _stageMock.Replay();

      var result = _contextWithParent.TryGetExpressionMapping (new QuerySourceReferenceExpression (groupJoinClause));

      _stageMock.VerifyAllExpectations();
      Assert.That (result, Is.Not.Null);
      Assert.That (((SqlTableReferenceExpression) result).SqlTable, Is.SameAs (preparedSqlTable));
      Assert.That (_visitor.SqlStatementBuilder.WhereCondition, Is.SameAs (preparedExpression));
    }

    [Test]
    public void TryGetExpressionMapping_GroupJoinClause ()
    {
      var groupJoinClause = ExpressionHelper.CreateGroupJoinClause();

      Expression result = _contextWithParent.TryGetExpressionMappingFromHierarchy (new QuerySourceReferenceExpression (groupJoinClause));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetExpressionMappingFromHierarchy_ReturnsNullWhenSourceNotAdded ()
    {
      Expression result = _context.TryGetExpressionMappingFromHierarchy (new QuerySourceReferenceExpression (_source));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void AddFromExpression ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable ();
      var whereCondition = Expression.Constant (true);
      var ordering = new Ordering (Expression.Constant ("order"), OrderingDirection.Asc);
      var preparedFromExpressionInfo = new FromExpressionInfo (
          sqlTable, new[] { ordering }, new SqlTableReferenceExpression (sqlTable), whereCondition, true);
      _visitor.SqlStatementBuilder.WhereCondition = null;
      _visitor.SqlStatementBuilder.Orderings.Clear ();
      _visitor.SqlStatementBuilder.SqlTables.Clear ();

      _contextWithParent.AddFromExpression (preparedFromExpressionInfo);

      Assert.That (_visitor.SqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (_visitor.SqlStatementBuilder.SqlTables[0], Is.SameAs (sqlTable));
      Assert.That (_visitor.SqlStatementBuilder.Orderings.Count, Is.EqualTo (1));
      Assert.That (_visitor.SqlStatementBuilder.Orderings[0], Is.SameAs (ordering));
      Assert.That (_visitor.SqlStatementBuilder.WhereCondition, Is.SameAs (whereCondition));
    }
  }
}