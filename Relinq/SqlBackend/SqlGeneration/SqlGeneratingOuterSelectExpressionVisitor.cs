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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// Extends <see cref="SqlGeneratingSelectExpressionVisitor"/> by building an in-memory projection. This should be used for the 
  /// <see cref="SqlStatement.SelectProjection"/> of the outermost <see cref="SqlStatement"/> in a query. For substatements, 
  /// <see cref="SqlGeneratingSelectExpressionVisitor"/> should be used instead.
  /// </summary>
  public class SqlGeneratingOuterSelectExpressionVisitor : SqlGeneratingSelectExpressionVisitor
  {
    private static readonly MethodInfo s_getValueMethod = typeof (IDatabaseResultRow).GetMethod ("GetValue");
    private static readonly MethodInfo s_getEntityMethod = typeof (IDatabaseResultRow).GetMethod ("GetEntity");

    public new static void GenerateSql (Expression expression, ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("stage", stage);

      EnsureNoCollectionExpression (expression);

      var visitor = new SqlGeneratingOuterSelectExpressionVisitor (commandBuilder, stage);
      visitor.VisitExpression (expression);
    }

    protected SqlGeneratingOuterSelectExpressionVisitor (ISqlCommandBuilder commandBuilder, ISqlGenerationStage stage)
        : base (commandBuilder, stage)
    {
    }

    protected int ColumnPosition { get; set; }

    public override Expression VisitNamedExpression (NamedExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var result = base.VisitNamedExpression (expression);

      var newInMemoryProjectionBody = Expression.Call (
          CommandBuilder.InMemoryProjectionRowParameter,
          s_getValueMethod.MakeGenericMethod (expression.Type),
          Expression.Constant (GetNextColumnID (expression.Name ?? NamedExpression.DefaultName)));

      CommandBuilder.SetInMemoryProjectionBody (newInMemoryProjectionBody);

      return result;
    }

    public override Expression VisitSqlEntityExpression (SqlEntityExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var columnIds = expression.Columns
          .Select (e => GetNextColumnID (GetAliasForColumnOfEntity (e, expression) ?? e.ColumnName))
          .ToArray();

      var result = base.VisitSqlEntityExpression (expression);

      var newInMemoryProjectionBody = Expression.Call (
          CommandBuilder.InMemoryProjectionRowParameter,
          s_getEntityMethod.MakeGenericMethod (expression.Type),
          Expression.Constant (columnIds));
      CommandBuilder.SetInMemoryProjectionBody (newInMemoryProjectionBody);

      return result;
    }

    protected override Expression VisitNewExpression (NewExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var projectionExpressions = new List<Expression>();
      CommandBuilder.AppendSeparated (",", expression.Arguments, (cb, expr) => projectionExpressions.Add (VisitArgumentOfLocalEvaluation (expr)));

      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      // ReSharper disable HeuristicUnreachableCode
      if (expression.Members == null)
        CommandBuilder.SetInMemoryProjectionBody (Expression.New (expression.Constructor, projectionExpressions));
      else
        CommandBuilder.SetInMemoryProjectionBody (Expression.New (expression.Constructor, projectionExpressions, expression.Members));
      // ReSharper restore HeuristicUnreachableCode
      // ReSharper restore ConditionIsAlwaysTrueOrFalse

      return expression;
    }

    protected override Expression VisitMethodCallExpression (MethodCallExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var allItems = expression.Object != null ? new[] { expression.Object }.Concat (expression.Arguments) : expression.Arguments;
      var projectionItems = new List<Expression>();
      CommandBuilder.AppendSeparated (",", allItems, (cb, expr) => projectionItems.Add (VisitArgumentOfLocalEvaluation (expr)));

      var instance = expression.Object != null ? projectionItems.First() : null;
      var arguments = expression.Object != null ? projectionItems.Skip (1) : projectionItems;

      var newInMemoryProjection = Expression.Call (instance, expression.Method, arguments);
      CommandBuilder.SetInMemoryProjectionBody (newInMemoryProjection);

      // If there are no projection items, we need to emit a NULL constant for the MethodCallExpression to avoid producing an empty SELECT list
      if (projectionItems.Count == 0)
      {
        VisitExpression (Expression.Constant (null));
        // Don't forget to increment the column position!
        GetNextColumnID ("");
      }

      return expression;
    }

    protected override Expression VisitUnaryExpression (UnaryExpression expression)
    {
      var result = base.VisitUnaryExpression (expression);

      var oldInMemoryProjectionBody = CommandBuilder.GetInMemoryProjectionBody();
      if (oldInMemoryProjectionBody != null
          && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked))
      {
        var newInMemoryProjectionBody = Expression.MakeUnary (expression.NodeType, oldInMemoryProjectionBody, expression.Type, expression.Method);
        CommandBuilder.SetInMemoryProjectionBody (newInMemoryProjectionBody);
      }

      return result;
    }

    public override Expression VisitSqlGroupingSelectExpression (SqlGroupingSelectExpression expression)
    {
      throw new NotSupportedException (
          "This SQL generator does not support queries returning groupings that result from a GroupBy operator because SQL is not suited to "
          + "efficiently return "
          + "LINQ groupings. Use 'group into' and either return the items of the groupings by feeding them into an additional from clause, or perform "
          + "an aggregation on the groupings. "
          + Environment.NewLine
          + Environment.NewLine
          + "Eg., instead of: "
          + Environment.NewLine + "'from c in Cooks group c.ID by c.Name', "
          + Environment.NewLine + "write: "
          + Environment.NewLine + "'from c in Cooks group c.ID by c.Name into groupedCooks "
          + Environment.NewLine + " from c in groupedCooks select new { Key = groupedCooks.Key, Item = c }', "
          + Environment.NewLine + "or: "
          + Environment.NewLine + "'from c in Cooks group c.ID by c.Name into groupedCooks "
          + Environment.NewLine + " select new { Key = groupedCooks.Key, Count = groupedCooks.Count() }'.");
    }

    protected ColumnID GetNextColumnID (string columnName)
    {
      return new ColumnID (columnName, ColumnPosition++);
    }

    private Expression VisitArgumentOfLocalEvaluation (Expression argumentExpression)
    {
      try
      {
        var namedExpression = argumentExpression as NamedExpression;
        if (namedExpression != null && namedExpression.Expression is ConstantExpression)
        {
          // Do not emit constants within a local evaluation; instead, emit "NULL", and directly use the constant expression as the in-memory projection.
          // This enables us to use complex, local-only constants within a local expression.
          VisitExpression (new NamedExpression (namedExpression.Name, Expression.Constant (null)));
          return namedExpression.Expression;
        }
        else
        {
          VisitExpression (argumentExpression);
          var argumentInMemoryProjectionBody = CommandBuilder.GetInMemoryProjectionBody();
          Debug.Assert (argumentInMemoryProjectionBody != null);

          return argumentInMemoryProjectionBody;
        }
      }
      finally
      {
        CommandBuilder.SetInMemoryProjectionBody (null);
      }
    }
  }
}