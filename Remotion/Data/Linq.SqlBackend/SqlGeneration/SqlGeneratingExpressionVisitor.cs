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
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// <see cref="SqlGeneratingExpressionVisitor"/> implements <see cref="ThrowingExpressionTreeVisitor"/> and <see cref="IResolvedSqlExpressionVisitor"/>.
  /// </summary>
  public class SqlGeneratingExpressionVisitor
      : ThrowingExpressionTreeVisitor,
        IResolvedSqlExpressionVisitor,
        ISqlSpecificExpressionVisitor,
        ISqlSubStatementVisitor,
        IJoinConditionExpressionVisitor,
        ISqlCustomTextGeneratorExpressionVisitor,
        INamedExpressionVisitor,
        IAggregationExpressionVisitor
  {
    public static void GenerateSql (Expression expression, ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("stage", stage);

      var visitor = new SqlGeneratingExpressionVisitor (commandBuilder, stage);
      visitor.VisitExpression (expression);
    }

    private readonly ISqlCommandBuilder _commandBuilder;
    private readonly BinaryExpressionTextGenerator _binaryExpressionTextGenerator;
    private readonly ISqlGenerationStage _stage;

    protected SqlGeneratingExpressionVisitor (ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("stage", stage);

      _commandBuilder = commandBuilder;
      _binaryExpressionTextGenerator = new BinaryExpressionTextGenerator (commandBuilder, this);
      _stage = stage;
    }

    public Expression VisitSqlEntityExpression (SqlEntityExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.AppendSeparated (",", expression.Columns, (cb, column) => AppendColumnForEntity(expression, column));
      return expression;
    }

    public Expression VisitSqlColumnExpression (SqlColumnExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      AppendColumn (expression.ColumnName, expression.OwningTableAlias);
      return expression;
    }

    public Expression VisitSqlValueReferenceExpression (SqlValueReferenceExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      // becomes SqlColumnDefinitionExpression _or_ directly emit corresponding SQL
      var columnExpression = new SqlColumnExpression (expression.Type, expression.TableAlias, expression.Name ?? "value", false);
      return VisitExpression (columnExpression);
    }

    public Expression VisitJoinConditionExpression (JoinConditionExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var whereExpression = Expression.Equal (
          expression.JoinedTable.JoinInfo.GetResolvedLeftJoinInfo().LeftKey,
          expression.JoinedTable.JoinInfo.GetResolvedLeftJoinInfo().RightKey);
      return VisitExpression (whereExpression);
    }

    protected override Expression VisitConstantExpression (ConstantExpression expression)
    {
      Debug.Assert (expression.Type != typeof (bool), "Boolean constants should have been removed by SqlContextExpressionVisitor.");

      if (expression.Value == null)
        _commandBuilder.Append ("NULL");
      else if (expression.Value is ICollection)
      {
        _commandBuilder.Append ("(");

        var collection = (ICollection) expression.Value;
        if (collection.Count == 0)
          _commandBuilder.Append ("SELECT NULL WHERE 1 = 0");

        var items = collection.Cast<object>();
        _commandBuilder.AppendSeparated (", ", items, (cb, value) => cb.AppendParameter (value));
        _commandBuilder.Append (")");
      }
      else
      {
        var parameter = _commandBuilder.CreateParameter (expression.Value);
        _commandBuilder.Append (parameter.Name);
      }

      return expression;
    }

    public Expression VisitSqlLiteralExpression (SqlLiteralExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      if (expression.Type == typeof (int))
        _commandBuilder.Append (expression.Value.ToString());
      else
        _commandBuilder.AppendStringLiteral ((string) expression.Value);
      return expression;
    }

    public Expression VisitSqlBinaryOperatorExpression (SqlBinaryOperatorExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      VisitExpression (expression.LeftExpression);
      _commandBuilder.Append (string.Format (" {0} ", expression.BinaryOperator));
      VisitExpression (expression.RightExpression);

      return expression;
    }

    public Expression VisitSqlIsNullExpression (SqlIsNullExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.Append ("(");
      VisitExpression (expression.Expression);
      _commandBuilder.Append (" IS NULL");
      _commandBuilder.Append (")");

      return expression;
    }

    public Expression VisitSqlIsNotNullExpression (SqlIsNotNullExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.Append ("(");
      VisitExpression (expression.Expression);
      _commandBuilder.Append (" IS NOT NULL");
      _commandBuilder.Append (")");

      return expression;
    }

    public Expression VisitSqlFunctionExpression (SqlFunctionExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.Append (expression.SqlFunctioName);
      _commandBuilder.Append ("(");
      _commandBuilder.AppendSeparated (", ", expression.Args, (cb, exp) => VisitExpression (exp));
      _commandBuilder.Append (")");
      return expression;
    }

    public Expression VisitSqlConvertExpression (SqlConvertExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.Append ("CONVERT");
      _commandBuilder.Append ("(");
      _commandBuilder.Append (expression.GetSqlTypeName());
      _commandBuilder.Append (", ");
      VisitExpression (expression.Source);
      _commandBuilder.Append (")");

      return expression;
    }

    public Expression VisitSqlExistsExpression (SqlExistsExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      _commandBuilder.Append ("EXISTS");
      _commandBuilder.Append ("(");
      VisitExpression (expression.Expression);
      _commandBuilder.Append (")");

      return expression;
    }

    protected override Expression VisitBinaryExpression (BinaryExpression expression)
    {
      _commandBuilder.Append ("(");
      _binaryExpressionTextGenerator.GenerateSqlForBinaryExpression (expression);
      _commandBuilder.Append (")");
      return expression;
    }

    protected override Expression VisitUnaryExpression (UnaryExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      switch (expression.NodeType)
      {
        case ExpressionType.Not:
          if (expression.Operand.Type == typeof (bool))
            _commandBuilder.Append ("NOT ");
          else
            _commandBuilder.Append ("~");
          break;
        case ExpressionType.Negate:
          _commandBuilder.Append ("-");
          break;
        case ExpressionType.UnaryPlus:
          _commandBuilder.Append ("+");
          break;
        case ExpressionType.Convert:
          break;
        default:
          throw new NotSupportedException();
      }

      VisitExpression (expression.Operand);

      return expression;
    }

    protected override Exception CreateUnhandledItemException<T> (T unhandledItem, string visitMethod)
    {
      throw new NotSupportedException (
          string.Format (
              "The expression '{0}' cannot be translated to SQL text by this SQL generator. Expression type '{1}' is not supported.",
              unhandledItem,
              unhandledItem.GetType().Name));
    }

    public Expression VisitSqlCaseExpression (SqlCaseExpression expression)
    {
      _commandBuilder.Append ("CASE WHEN ");
      VisitExpression (expression.TestPredicate);
      _commandBuilder.Append (" THEN ");
      VisitExpression (expression.ThenValue);
      _commandBuilder.Append (" ELSE ");
      VisitExpression (expression.ElseValue);
      _commandBuilder.Append (" END");
      return expression;
    }

    public Expression VisitSqlSubStatementExpression (SqlSubStatementExpression expression)
    {
      _commandBuilder.Append ("(");
      _stage.GenerateTextForSqlStatement (_commandBuilder, expression.SqlStatement);
      _commandBuilder.Append (")");
      return expression;
    }

    public Expression VisitSqlCustomTextGeneratorExpression (SqlCustomTextGeneratorExpressionBase expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      expression.Generate (_commandBuilder, this, _stage);
      return expression;
    }

    public Expression VisitNamedExpression (NamedExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      VisitExpression (expression.Expression);
      _commandBuilder.Append (" AS ");
      _commandBuilder.AppendIdentifier (expression.Name ?? "value");

      return expression;
    }

    public Expression VisitAggregationExpression (AggregationExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      if (expression.AggregationModifier == AggregationModifier.Count) {
        _commandBuilder.Append ("COUNT(*)");
        return expression;
      }

      if (expression.AggregationModifier == AggregationModifier.Average)
        _commandBuilder.Append("AVG");
      else if (expression.AggregationModifier == AggregationModifier.Max)
        _commandBuilder.Append ("MAX");
      else if (expression.AggregationModifier == AggregationModifier.Min)
        _commandBuilder.Append ("MIN");
      else if (expression.AggregationModifier == AggregationModifier.Sum)
        _commandBuilder.Append ("SUM");
      else
        throw new NotSupportedException (string.Format ("AggregationModifier '{0}' is not supported.", expression.AggregationModifier));

      _commandBuilder.Append ("(");
      VisitExpression (((NamedExpression) expression.Expression).Expression);
      _commandBuilder.Append (")");

      return expression;
    }

    private void AppendColumnForEntity (SqlEntityExpression entity, SqlColumnExpression column)
    {
      column.Accept (this);
      // if (entity.Name != null)
      //   Append " AS ";
      //   AppendIdentifier (entity.Name + "_" + column.Name);
    }

    private void AppendColumn (string columnName, string prefix) // add string referencedEntityName; pass null for column definitions; pass name for column references
    {
      if (columnName == "*")
      {
        _commandBuilder.AppendIdentifier (prefix);
        _commandBuilder.Append (".*");
      }
      else
      {
        _commandBuilder.AppendIdentifier (prefix);
        _commandBuilder.Append (".");
        // if referencedEntityName != null, append referencedEntityName, append "_" here
        _commandBuilder.AppendIdentifier (columnName);
      }
    }
  }
}