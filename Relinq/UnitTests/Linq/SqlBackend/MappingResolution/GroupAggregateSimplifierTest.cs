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
using Remotion.Linq.UnitTests.Linq.Core.Parsing;
using Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.MappingResolution
{
  [TestFixture]
  public class GroupAggregateSimplifierTest
  {
    private StreamedScalarValueInfo _dataInfo;

    private SqlGroupingSelectExpression _associatedGroupingSelectExpression;
    private SqlStatement _resolvedJoinedGroupingSubStatement;
    private SqlTable _resolvedJoinedGroupingTable;
    private SqlColumnDefinitionExpression _resolvedElementExpressionReference;
    private NamedExpression _resolvedSelectProjection;
    private SqlStatement _simplifiableResolvedSqlStatement;
    private AggregationExpression _simplifiableUnresolvedProjection;

    private IMappingResolutionStage _stageMock;
    private MappingResolutionContext _context;

    [SetUp]
    public void SetUp ()
    {
      _dataInfo = new StreamedScalarValueInfo (typeof (int));

      _resolvedElementExpressionReference = new SqlColumnDefinitionExpression (typeof (string), "q0", "element", false);
      _resolvedSelectProjection = new NamedExpression (
          null, 
          new AggregationExpression (typeof (int), _resolvedElementExpressionReference, AggregationModifier.Min));

      _associatedGroupingSelectExpression = new SqlGroupingSelectExpression (
          new NamedExpression ("key", Expression.Constant ("k")),
          new NamedExpression ("element", Expression.Constant ("e")));

      _resolvedJoinedGroupingSubStatement = SqlStatementModelObjectMother.CreateSqlStatement (_associatedGroupingSelectExpression);
      _resolvedJoinedGroupingTable = new SqlTable (
          new ResolvedJoinedGroupingTableInfo (
              "q1",
              _resolvedJoinedGroupingSubStatement,
              _associatedGroupingSelectExpression,
              "q0"), JoinSemantics.Inner);

      _simplifiableResolvedSqlStatement = new SqlStatement (
          _dataInfo,
          _resolvedSelectProjection,
          new[] { _resolvedJoinedGroupingTable },
          null,
          null,
          new Ordering[0],
          null,
          false,
          Expression.Constant (0),
          Expression.Constant (0));
      _simplifiableUnresolvedProjection = new AggregationExpression (
          typeof (int),
          new SqlTableReferenceExpression (_resolvedJoinedGroupingTable),
          AggregationModifier.Count);

      _stageMock = MockRepository.GenerateStrictMock<IMappingResolutionStage> ();
      _context = new MappingResolutionContext();
    }
    
    [Test]
    public void IsSimplifiableGroupAggregate_True_WithReferenceToJoinGroupInAggregationExpression ()
    {
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (_simplifiableResolvedSqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_True_WithNamedExpressionInAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        SelectProjection = new AggregationExpression (
          typeof (int), new NamedExpression ("value", _resolvedElementExpressionReference), AggregationModifier.Min)
      }.GetSqlStatement();
      
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_True_WithAggregationExpressionInNamedExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        SelectProjection = new NamedExpression (
            null, 
            new AggregationExpression (typeof (int), _resolvedElementExpressionReference, AggregationModifier.Min))
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_True_WithNonReferenceExpressionInAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        SelectProjection = new AggregationExpression (typeof (int), Expression.Constant (0), AggregationModifier.Min)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_NoAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        SelectProjection = _resolvedElementExpressionReference
      }.GetSqlStatement ();
      
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_WhereExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        WhereCondition = Expression.Constant (true)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_OrderingExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        Orderings = { new Ordering(Expression.Constant("order"), OrderingDirection.Asc) }
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_GroupByExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        GroupByExpression = Expression.Constant(0)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_TooManySqlTables ()
    {
      var sqlStatementBuilder = new SqlStatementBuilder (_simplifiableResolvedSqlStatement);
      sqlStatementBuilder.SqlTables.Add (SqlStatementModelObjectMother.CreateSqlTable());
      var sqlStatement = sqlStatementBuilder.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_NoResolvedJoinedGroupingTableInfo ()
    {
      var sqlStatementBuilder = new SqlStatementBuilder (_simplifiableResolvedSqlStatement);
      sqlStatementBuilder.SqlTables.Clear();
      sqlStatementBuilder.SqlTables.Add (SqlStatementModelObjectMother.CreateSqlTable (new ResolvedSimpleTableInfo (typeof (int), "table", "t0")));
      var sqlStatement = sqlStatementBuilder.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_TopExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        TopExpression = Expression.Constant (0)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_DistinctQuery()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        IsDistinctQuery = true
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void SimplifyIfPossible_NonSimplifiableStatement ()
    {
      var resolvedSqlStatement = new SqlStatementBuilder (_simplifiableResolvedSqlStatement)
      {
        IsDistinctQuery = true
      }.GetSqlStatement ();
      
      _stageMock.Replay();

      var result = GroupAggregateSimplifier.SimplifyIfPossible (resolvedSqlStatement, _simplifiableUnresolvedProjection, _stageMock, _context);

      _stageMock.VerifyAllExpectations();

      var expected = new SqlSubStatementExpression (resolvedSqlStatement);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }

    [Test]
    public void SimplifyIfPossible_SimplifiableStatement_AddsAggregationAndReturnsReference ()
    {
      Assert.That (_associatedGroupingSelectExpression.AggregationExpressions.Count, Is.EqualTo (0));

      var preparedResolvedAggregate = new AggregationExpression (
          typeof (int), 
          new NamedExpression ("element", Expression.Constant ("e")), 
          AggregationModifier.Count);
      _stageMock
          .Expect (mock => mock.ResolveAggregationExpression(Arg<Expression>.Is.Anything, Arg.Is (_context)))
          .Return (preparedResolvedAggregate)
          .WhenCalled (mi => {
            var expectedReplacedAggregate = new AggregationExpression (
                typeof (int),
                ((NamedExpression) _associatedGroupingSelectExpression.ElementExpression).Expression, 
                AggregationModifier.Count);
            ExpressionTreeComparer.CheckAreEqualTrees (expectedReplacedAggregate, (Expression) mi.Arguments[0]);
          });
      _stageMock.Replay();

      var result = GroupAggregateSimplifier.SimplifyIfPossible (_simplifiableResolvedSqlStatement, _simplifiableUnresolvedProjection, _stageMock, _context);

      _stageMock.VerifyAllExpectations();

      Assert.That (_associatedGroupingSelectExpression.AggregationExpressions.Count, Is.EqualTo (1));
      Assert.That (
          ((NamedExpression) _associatedGroupingSelectExpression.AggregationExpressions[0]).Expression, 
          Is.SameAs (preparedResolvedAggregate));

      var expected = new SqlColumnDefinitionExpression (typeof (int), "q0", "a0", false);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }

    [Test]
    public void SimplifyIfPossible_WithNonSimplifiableProjection_ReturnsOriginalStatement ()
    {
      Assert.That (_associatedGroupingSelectExpression.AggregationExpressions.Count, Is.EqualTo (0));

      _stageMock.Replay ();

      var nonSimplifiableProjection = new AggregationExpression (
          typeof (int), 
          new SqlTableReferenceExpression (SqlStatementModelObjectMother.CreateSqlTable ()), AggregationModifier.Count);
      var result = GroupAggregateSimplifier.SimplifyIfPossible (_simplifiableResolvedSqlStatement, nonSimplifiableProjection, _stageMock, _context);

      _stageMock.VerifyAllExpectations ();

      var expected = new SqlSubStatementExpression (_simplifiableResolvedSqlStatement);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);

      Assert.That (_associatedGroupingSelectExpression.AggregationExpressions.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The unresolved projection doesn't match the resolved statement: it has no aggregation.\r\nParameter name: unresolvedSelectProjection")]
    public void SimplifyIfPossible_WithUnresolvedProjection_NotMatchingResolvedOned_NoAggregation ()
    {
      var nonSimplifiableProjection = new SqlTableReferenceExpression (SqlStatementModelObjectMother.CreateSqlTable ());
      GroupAggregateSimplifier.SimplifyIfPossible (_simplifiableResolvedSqlStatement, nonSimplifiableProjection, _stageMock, _context);
    }

    [Test]
    public void VisitExpression_ReferenceToRightTable ()
    {
      var visitor = new TestableGroupAggregateSimplifier (_resolvedJoinedGroupingTable, _associatedGroupingSelectExpression.ElementExpression);

      var input = new SqlTableReferenceExpression (_resolvedJoinedGroupingTable);
      var result = visitor.VisitExpression (input);

      Assert.That (visitor.CanBeTransferredToGroupingSource, Is.True);
      Assert.That (result, Is.SameAs (_associatedGroupingSelectExpression.ElementExpression));
    }

    [Test]
    public void VisitExpression_ReferenceToRightTable_Nested ()
    {
      var visitor = new TestableGroupAggregateSimplifier (_resolvedJoinedGroupingTable, _associatedGroupingSelectExpression.ElementExpression);

      var input = Expression.Equal (
          new SqlTableReferenceExpression (_resolvedJoinedGroupingTable), 
          new SqlTableReferenceExpression (_resolvedJoinedGroupingTable));
      
      var result = visitor.VisitExpression (input);

      Assert.That (visitor.CanBeTransferredToGroupingSource, Is.True);
      var expectedResult = Expression.Equal (
          _associatedGroupingSelectExpression.ElementExpression,
          _associatedGroupingSelectExpression.ElementExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void VisitExpression_ReferenceToOtherTable ()
    {
      var visitor = new TestableGroupAggregateSimplifier (_resolvedJoinedGroupingTable, _associatedGroupingSelectExpression.ElementExpression);

      var input = new SqlTableReferenceExpression (SqlStatementModelObjectMother.CreateSqlTable());
      visitor.VisitExpression (input);

      Assert.That (visitor.CanBeTransferredToGroupingSource, Is.False);
    }

    [Test]
    public void VisitExpression_AnyOtherExpression ()
    {
      var visitor = new TestableGroupAggregateSimplifier (_resolvedJoinedGroupingTable, _associatedGroupingSelectExpression.ElementExpression);

      var input = Expression.Constant (0);
      var result = visitor.VisitExpression (input);

      Assert.That (visitor.CanBeTransferredToGroupingSource, Is.True);
      Assert.That (result, Is.SameAs (input));
    }
  }
}