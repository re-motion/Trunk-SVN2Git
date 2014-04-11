// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.UnitTests.SqlStatementModel;
using Remotion.Linq.SqlBackend.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Linq.SqlBackend.UnitTests.SqlGeneration
{
  [TestFixture]
  public class SqlGeneratingOuterSelectExpressionVisitorTest
  {
    private NamedExpression _namedIntExpression;
    private SqlEntityDefinitionExpression _entityExpression;
    private TestableSqlGeneratingOuterSelectExpressionVisitor _visitor;
    private ISqlGenerationStage _stageMock;
    private SqlCommandBuilder _commandBuilder;
    private SqlColumnDefinitionExpression _nameColumnExpression;

    [SetUp]
    public void SetUp ()
    {
      _stageMock = MockRepository.GenerateStrictMock<ISqlGenerationStage> ();
      _commandBuilder = new SqlCommandBuilder ();
      _visitor = new TestableSqlGeneratingOuterSelectExpressionVisitor (_commandBuilder, _stageMock);

      _namedIntExpression = new NamedExpression ("test", Expression.Constant (0));
      _nameColumnExpression = new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false);
      _entityExpression = new SqlEntityDefinitionExpression (
          typeof (Cook),
          "c",
          "test",
          e => e,
          new SqlColumnDefinitionExpression (typeof (int), "c", "ID", true),
          _nameColumnExpression,
          new SqlColumnDefinitionExpression (typeof (string), "c", "FirstName", false)
          );
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Queries selecting collections are not supported because SQL is not well-suited to returning collections.",
        MatchType = MessageMatch.Contains)]
    public void GenerateSql_Collection ()
    {
      var expression = Expression.Constant (new Cook[] { });
      SqlGeneratingOuterSelectExpressionVisitor.GenerateSql (expression, _commandBuilder, _stageMock);
    }

    [Test]
    public void GenerateSql_CreatesFullInMemoryProjectionLambda ()
    {
       SqlGeneratingOuterSelectExpressionVisitor.GenerateSql (_namedIntExpression, _commandBuilder, _stageMock);

      var expectedRowParameter = _commandBuilder.InMemoryProjectionRowParameter;
      var expectedFullProjection = GetExpectedProjectionForNamedExpression (expectedRowParameter, "test", 0, typeof (int));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedFullProjection, _commandBuilder.GetInMemoryProjectionBody());
    }

    [Test]
    public void VisitNamedExpression ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      _visitor.VisitNamedExpression (_namedIntExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedProjection = GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "test", 0, typeof (int));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody());

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("@1 AS [test]"));
    }

    [Test]
    public void VisitNamedExpression_OverridesChildProjection ()
    {
      _commandBuilder.SetInMemoryProjectionBody (Expression.Constant (0));
      var nestedNamedExpression = new NamedExpression ("outer", Expression.Convert (Expression.Constant (0), typeof (double)));

      _visitor.VisitNamedExpression (nestedNamedExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedProjection = GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "outer", 0, typeof (double));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());

      Assert.That (_commandBuilder.GetCommandText (), Is.EqualTo ("@1 AS [outer]"));
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithAnInnerNamedExpression_EmitsBitConversion_AndUsesBoolInProjection ()
    {
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (_namedIntExpression);

       Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedProjection = GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "test", 0, typeof (bool));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody());

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("CONVERT(BIT, @1) AS [test]"));
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithAnInnerNamedExpression_ReturnsAdaptedNamedExpressionOfCorrectType ()
    {
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (_namedIntExpression);

      var result = _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      var expectedResult = new NamedExpression (_namedIntExpression.Name, new SqlConvertExpression (typeof (bool), _namedIntExpression.Expression));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithAnInnerNamedExpression_NullableBool_EmitsBitConversion_AndUsesNullableBoolInProjection ()
    {
      var namedNullableIntExpression = new NamedExpression ("test", Expression.Constant (0, typeof (int?)));
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (namedNullableIntExpression);

      _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      var expectedProjection = GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "test", 0, typeof (bool?));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody());

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("CONVERT(BIT, @1) AS [test]"));
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithAnInnerNamedExpression_NullableBool_ReturnsAdaptedNamedExpressionOfCorrectType ()
    {
      var namedNullableIntExpression = new NamedExpression ("test", Expression.Constant (0, typeof (int?)));
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (namedNullableIntExpression);

      var result = _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      var expectedResult = new NamedExpression (
          namedNullableIntExpression.Name, new SqlConvertExpression (typeof (bool?), namedNullableIntExpression.Expression));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithAnInnerNamedExpression_OmitsSqlConversion_ForColumns ()
    {
      var namedIntColumnExpression = new NamedExpression ("col", SqlStatementModelObjectMother.CreateSqlColumn (typeof (int)));
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (namedIntColumnExpression);

       Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedProjection = GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "col", 0, typeof (bool));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody());

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("[t0].[column] AS [col]"));
    }

    [Test]
    public void VisitSqlConvertedBooleanExpression_WithADifferentExpression_VisitsInnerExpressionAndReturnsInputExpression ()
    {
      var sqlConvertedBooleanExpression = new SqlConvertedBooleanExpression (new SqlLiteralExpression (0));

      var result = _visitor.VisitSqlConvertedBooleanExpression (sqlConvertedBooleanExpression);

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("0"));
      Assert.That (result, Is.SameAs (sqlConvertedBooleanExpression));
    }

    [Test]
    public void VisitSqlEntityExpression ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      _visitor.VisitSqlEntityExpression (_entityExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (3));

      var expectedProjection = GetExpectedProjectionForEntityExpression (_commandBuilder.InMemoryProjectionRowParameter, 0);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());

      Assert.That (_commandBuilder.GetCommandText (), Is.EqualTo ("[c].[ID] AS [test_ID],[c].[Name] AS [test_Name],[c].[FirstName] AS [test_FirstName]"));
    }

    [Test]
    public void VisitNewExpression_WithoutMembers ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      var newExpression = Expression.New (
          typeof (KeyValuePair<int, Cook>).GetConstructor (new[] { typeof(int), typeof (Cook)}), 
          new Expression[] { _namedIntExpression, _entityExpression });
      _visitor.VisitNewExpression (newExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (4));

      var expectedRowParameter = _commandBuilder.InMemoryProjectionRowParameter;
      // Constants are directly used within the in-memory projection, whereas entities are taken from the SQL result
      var expectedProjectionForNamedExpression = Expression.Constant (0);
      var expectedProjectionForEntityExpression = GetExpectedProjectionForEntityExpression (expectedRowParameter, 1);
      
      var expectedProjectionForNewExpression = Expression.New (
          typeof (KeyValuePair<int, Cook>).GetConstructor (new[] { typeof (int), typeof (Cook) }),
          new Expression[] { expectedProjectionForNamedExpression, expectedProjectionForEntityExpression });

      Assert.That (
          _commandBuilder.GetCommandText (), 
          Is.EqualTo ("NULL AS [test],[c].[ID] AS [test_ID],[c].[Name] AS [test_Name],[c].[FirstName] AS [test_FirstName]"));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjectionForNewExpression, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitNewExpression_WithMembers ()
    {
      var keyValueType = typeof (KeyValuePair<int, Cook>);
      var newExpression = Expression.New (
          keyValueType.GetConstructor (new[] { typeof (int), typeof (Cook) }),
          new Expression[] { _namedIntExpression, _entityExpression },
          keyValueType.GetProperty("Key"),
          keyValueType.GetProperty("Value"));

      _visitor.VisitNewExpression (newExpression);

      var expectedRowParameter = _commandBuilder.InMemoryProjectionRowParameter;
      // Constants are directly used within the in-memory projection, whereas entities are taken from the SQL result
      var expectedProjectionForNamedExpression = Expression.Constant (0);
      var expectedProjectionForEntityExpression = GetExpectedProjectionForEntityExpression (expectedRowParameter, 1);
      
      var expectedProjectionForNewExpression = Expression.New (
          keyValueType.GetConstructor (new[] { typeof (int), typeof (Cook) }),
          new Expression[] { expectedProjectionForNamedExpression, expectedProjectionForEntityExpression },
          keyValueType.GetProperty ("Key"), keyValueType.GetProperty ("Value"));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjectionForNewExpression, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitMethodCallExpression ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      var namedStringExpression = new NamedExpression ("Name", _nameColumnExpression);

      var methodCallExpression = Expression.Call (
          _namedIntExpression,
          ReflectionUtility.GetMethod (() => 0.ToString ("")),
          new Expression[] { namedStringExpression });
      _visitor.VisitMethodCallExpression (methodCallExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (2));

      var expectedRowParameter = _commandBuilder.InMemoryProjectionRowParameter;
      // Constants are used as is in the projection, whereas columns are taken from the SQL result
      var expectedProjectionForInstanceExpression = Expression.Constant (0);
      var expectedProjectionForArgumentExpression = GetExpectedProjectionForNamedExpression (expectedRowParameter, "Name", 1, typeof (string));

      var expectedProjection = Expression.Call (
          expectedProjectionForInstanceExpression,
          methodCallExpression.Method,
          new Expression[] { expectedProjectionForArgumentExpression });

      Assert.That (
          _commandBuilder.GetCommandText (),
          Is.EqualTo ("NULL AS [test],[c].[Name] AS [Name]"));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitMethodCallExpression_WithoutObject ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      var namedStringExpression = new NamedExpression ("Name", _nameColumnExpression);

      var methodCallExpression = Expression.Call (
          ReflectionUtility.GetMethod (() => int.Parse ("")),
          new Expression[] { namedStringExpression });
      _visitor.VisitMethodCallExpression (methodCallExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedRowParameter = _commandBuilder.InMemoryProjectionRowParameter;
      var expectedProjectionForArgumentExpression = GetExpectedProjectionForNamedExpression (expectedRowParameter, "Name", 0, typeof (string));

      var expectedProjection = Expression.Call (
          methodCallExpression.Method,
          new Expression[] { expectedProjectionForArgumentExpression });

      Assert.That (
          _commandBuilder.GetCommandText (),
          Is.EqualTo ("[c].[Name] AS [Name]"));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitMethodCallExpression_NoObjectsOrArguments ()
    {
      Assert.That (_visitor.ColumnPosition, Is.EqualTo (0));

      var methodCallExpression = Expression.Call (ReflectionUtility.GetMethod (() => StaticMethodWithoutArguments()));
      _visitor.VisitMethodCallExpression (methodCallExpression);

      Assert.That (_visitor.ColumnPosition, Is.EqualTo (1));

      var expectedProjection = Expression.Call (methodCallExpression.Method);

      Assert.That (_commandBuilder.GetCommandText (), Is.EqualTo ("NULL"));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitUnaryExpression_Convert_ProjectionExpressonIsNull ()
    {
      var expression = Expression.Convert (Expression.Constant (1), typeof (double));

      _visitor.VisitUnaryExpression (expression);

      Assert.That (_commandBuilder.GetInMemoryProjectionBody (), Is.Null);
    }

    [Test]
    public void VisitUnaryExpression_Convert_ProjectionExpressonIsNotNull ()
    {
      var methodInfo = typeof (Convert).GetMethod ("ToDouble", new[] { typeof (int) });
      var expression = Expression.Convert (_namedIntExpression, typeof (double), methodInfo);

      _visitor.VisitUnaryExpression (expression);

      var expectedProjection = Expression.Convert (
          GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "test", 0, typeof (int)),
          typeof (double),
          methodInfo);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    public void VisitUnaryExpression_ConvertChecked_ProjectionExpressonIsNull ()
    {
      var expression = Expression.ConvertChecked (Expression.Constant (1), typeof (double));

      _visitor.VisitUnaryExpression (expression);

      Assert.That (_commandBuilder.GetInMemoryProjectionBody (), Is.Null);
    }

    [Test]
    public void VisitUnaryExpression_ConvertChecked_ProjectionExpressonIsNotNull ()
    {
      var methodInfo = typeof (Convert).GetMethod ("ToDouble", new[] { typeof (int) });
      var expression = Expression.ConvertChecked (_namedIntExpression, typeof (double), methodInfo);

      _visitor.VisitUnaryExpression (expression);

      var expectedProjection = Expression.ConvertChecked (
          GetExpectedProjectionForNamedExpression (_commandBuilder.InMemoryProjectionRowParameter, "test", 0, typeof (int)),
          typeof (double),
          methodInfo);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedProjection, _commandBuilder.GetInMemoryProjectionBody ());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "This SQL generator does not support queries returning groupings that result from a GroupBy operator because SQL is not suited to "
         + "efficiently return LINQ groupings. Use 'group into' and either return the items of the groupings by feeding them into an additional "
         + "from clause, or perform an aggregation on the groupings.", MatchType = MessageMatch.Contains)]
    public void VisitSqlGroupingSelectExpression ()
    {
      var expression = SqlStatementModelObjectMother.CreateSqlGroupingSelectExpression ();
      _visitor.VisitSqlGroupingSelectExpression (expression);
    }

    private MethodCallExpression GetExpectedProjectionForEntityExpression (ParameterExpression expectedRowParameter, int columnPositionStart)
    {
      return Expression.Call (
          expectedRowParameter,
          typeof (IDatabaseResultRow).GetMethod ("GetEntity").MakeGenericMethod (typeof (Cook)),
          Expression.Constant (new[] { 
              new ColumnID ("test_ID", columnPositionStart), 
              new ColumnID ("test_Name", columnPositionStart + 1), 
              new ColumnID ("test_FirstName", columnPositionStart + 2) }));
    }

    private MethodCallExpression GetExpectedProjectionForNamedExpression (
        ParameterExpression rowParameter,
        string name,
        int columnPosoitionStart,
        Type expressionType)
    {
      return Expression.Call (
          rowParameter,
          typeof (IDatabaseResultRow).GetMethod ("GetValue").MakeGenericMethod (expressionType),
          Expression.Constant (new ColumnID (name ?? "value", columnPosoitionStart)));
    }

    private static int StaticMethodWithoutArguments ()
    {
      throw new NotImplementedException();
    }
  }
}