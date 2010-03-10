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
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// <see cref="SqlGeneratingExpressionVisitor"/> implements <see cref="ThrowingExpressionTreeVisitor"/> and <see cref="IResolvedSqlExpressionVisitor"/>.
  /// </summary>
  public class SqlGeneratingExpressionVisitor : ThrowingExpressionTreeVisitor, IResolvedSqlExpressionVisitor
  {
    public static void GenerateSql (
        Expression expression, SqlCommandBuilder commandBuilder, MethodCallSqlGeneratorRegistry methodCallRegistry)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("methodCallRegistry", methodCallRegistry);

      var visitor = new SqlGeneratingExpressionVisitor (commandBuilder, methodCallRegistry);
      visitor.VisitExpression (expression);
    }

    private readonly SqlCommandBuilder _commandBuilder;
    private readonly MethodCallSqlGeneratorRegistry _methodCallRegistry;
    private readonly BinaryExpressionTextGenerator _binaryExpressionTextGenerator;

    protected SqlGeneratingExpressionVisitor (SqlCommandBuilder commandBuilder, MethodCallSqlGeneratorRegistry methodCallRegistry)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("methodCallRegistry", methodCallRegistry);

      _commandBuilder = commandBuilder;
      _methodCallRegistry = methodCallRegistry;
      _binaryExpressionTextGenerator = new BinaryExpressionTextGenerator (commandBuilder, this);
    }

    public Expression VisitSqlColumListExpression (SqlColumnListExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var first = true;
      foreach (var column in expression.Columns)
      {
        if (!first)
          _commandBuilder.Append (",");
        column.Accept (this);
        first = false;
      }

      return expression;
    }

    public Expression VisitSqlColumnExpression (SqlColumnExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var prefix = expression.OwningTableAlias;
      var columnName = expression.ColumnName;
      _commandBuilder.AppendFormat ("[{0}].[{1}]", prefix, columnName);

      return expression;
    }

    protected override Expression VisitConstantExpression (ConstantExpression expression)
    {
      if (expression.Type == typeof (bool))
      {
        var parameter = _commandBuilder.AddParameter ((bool) expression.Value ? 1 : 0);
        _commandBuilder.Append (parameter.Name);
      }
      else if (expression.Value == null)
        _commandBuilder.Append ("NULL");
      else
      {
        var parameter = _commandBuilder.AddParameter (expression.Value);
        _commandBuilder.Append (parameter.Name);
      }

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
          _commandBuilder.Append ("NOT ");
          break;
        case ExpressionType.Negate:
          _commandBuilder.Append ("-");
          break;
        case ExpressionType.UnaryPlus:
          _commandBuilder.Append ("+");
          break;
        default:
          throw new NotSupportedException();
      }

      VisitExpression (expression.Operand);

      return expression;
    }

    protected override Expression VisitMethodCallExpression (MethodCallExpression expression)
    {
      _methodCallRegistry.GetGenerator (expression.Method).GenerateSql (expression, _commandBuilder, this);
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
  }
}