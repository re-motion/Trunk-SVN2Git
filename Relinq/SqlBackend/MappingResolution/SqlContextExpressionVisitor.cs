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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// Ensures that a given expression matches SQL server value semantics.
  /// </summary>
  /// <remarks>
  /// <see cref="SqlContextExpressionVisitor"/> traverses an expression tree and ensures that the tree fits SQL server requirements for
  /// boolean expressions. In scenarios where a value is required as per SQL server standards, bool expressions are converted to integers using
  /// CASE WHEN expressions. In such situations, <see langword="true" /> and <see langword="false" /> constants are converted to 1 and 0 values,
  /// and boolean columns are interpreted as integer values. In scenarios where a predicate is required, boolean expressions are constructed by 
  /// comparing those integer values to 1 and 0 literals.
  /// </remarks>
  public class SqlContextExpressionVisitor
      : ExpressionTreeVisitor,
        ISqlSpecificExpressionVisitor,
        IResolvedSqlExpressionVisitor,
        IUnresolvedSqlExpressionVisitor,
        ISqlSubStatementVisitor,
        INamedExpressionVisitor,
        ISqlGroupingSelectExpressionVisitor,
        ISqlConvertedBooleanExpressionVisitor,
        ISqlPredicateAsValueExpressionVisitor
  {
    public static Expression ApplySqlExpressionContext (
        Expression expression, SqlExpressionContext initialSemantics, IMappingResolutionStage stage, IMappingResolutionContext context)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      var visitor = new SqlContextExpressionVisitor (initialSemantics, stage, context);
      return visitor.VisitExpression (expression);
    }

    private readonly SqlExpressionContext _currentContext;
    private readonly IMappingResolutionStage _stage;
    private readonly IMappingResolutionContext _context;

    protected SqlContextExpressionVisitor (SqlExpressionContext currentContext, IMappingResolutionStage stage, IMappingResolutionContext context)
    {
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      _currentContext = currentContext;
      _stage = stage;
      _context = context;
    }

    public override Expression VisitExpression (Expression expression)
    {
      if (expression == null)
        return null;

      switch (_currentContext)
      {
        case SqlExpressionContext.SingleValueRequired:
        case SqlExpressionContext.ValueRequired:
          return HandleValueSemantics (expression);
        case SqlExpressionContext.PredicateRequired:
          return HandlePredicateSemantics (expression);
      }

      throw new InvalidOperationException ("Invalid enum value: " + _currentContext);
    }

    public Expression VisitSqlConvertedBooleanExpression (SqlConvertedBooleanExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newInner = ApplySqlExpressionContext (expression.Expression, SqlExpressionContext.ValueRequired, _stage, _context);
      Debug.Assert (
          newInner == expression.Expression,
          "There is currently no visit method that would change an int-typed expression with ValueRequired.");

      // This condition cannot be true at the moment because there currently is no int-typed expression that would be changed by ValueRequired.
      //if (newInner != expression.Expression)
      //  return new ConvertedBooleanExpression (newInner);

      return expression;
    }

    public Expression VisitSqlPredicateAsValueExpression (SqlPredicateAsValueExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newPredicate = ApplySqlExpressionContext (expression.Predicate, SqlExpressionContext.PredicateRequired, _stage, _context);
      if (newPredicate != expression.Predicate)
        return new SqlPredicateAsValueExpression (newPredicate);

      return expression;
    }

    protected override Expression VisitConstantExpression (ConstantExpression expression)
    {
      // Always convert boolean constants to int constants because in the database, there are no boolean constants
      
      if (BooleanUtility.IsBooleanType (expression.Type))
      {
        var intType = BooleanUtility.GetMatchingIntType (expression.Type);
        var convertedExpression = expression.Value == null
                                      ? Expression.Constant (null, intType)
                                      : expression.Value.Equals (true)
                                            ? Expression.Constant (1, intType)
                                            : Expression.Constant (0, intType);
        return new SqlConvertedBooleanExpression (convertedExpression);
      }
      
      return expression; // rely on VisitExpression to apply correct semantics
    }

    public Expression VisitSqlColumnExpression (SqlColumnExpression expression)
    {
      // We always need to convert boolean columns to int columns because in the database, the column is represented as a bit (integer) value
      if (BooleanUtility.IsBooleanType (expression.Type))
      {
        var intType = BooleanUtility.GetMatchingIntType (expression.Type);
        Expression convertedExpression = expression.Update (intType, expression.OwningTableAlias, expression.ColumnName, expression.IsPrimaryKey);
        return new SqlConvertedBooleanExpression (convertedExpression);
      }
      
      return expression; // rely on VisitExpression to apply correct semantics
    }

    public Expression VisitSqlEntityExpression (SqlEntityExpression expression)
    {
      if (_currentContext == SqlExpressionContext.SingleValueRequired)
        return expression.PrimaryKeyColumn;
      else
        return expression; // rely on VisitExpression to apply correct semantics
    }

    protected override Expression VisitConditionalExpression (ConditionalExpression expression)
    {
      var testPredicate = ApplySqlExpressionContext (expression.Test, SqlExpressionContext.PredicateRequired, _stage, _context);
      var thenValue = ApplySqlExpressionContext (expression.IfTrue, SqlExpressionContext.SingleValueRequired, _stage, _context);
      var elseValue = ApplySqlExpressionContext (expression.IfFalse, SqlExpressionContext.SingleValueRequired, _stage, _context);

      if (testPredicate != expression.Test || thenValue != expression.IfTrue || elseValue != expression.IfFalse)
        return Expression.Condition (testPredicate, thenValue, elseValue);
      else
        return expression;
    }

    protected override Expression VisitBinaryExpression (BinaryExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      if (!BooleanUtility.IsBooleanType (expression.Type))
        return base.VisitBinaryExpression (expression);

      var childContext = GetChildSemanticsForBinaryBoolExpression (expression.NodeType);
      var left = ApplySqlExpressionContext (expression.Left, childContext, _stage, _context);
      var right = ApplySqlExpressionContext (expression.Right, childContext, _stage, _context);

      if (expression.NodeType == ExpressionType.Coalesce)
      {
        // In predicate context, we can ignore coalesces towards false, treat like a conversion to bool instead. (SQL treats NULL values in a falsey
        // way in predicate contexts.)
        if (_currentContext == SqlExpressionContext.PredicateRequired
            && expression.Right is ConstantExpression
            && Equals (((ConstantExpression) expression.Right).Value, false))
        {
          return VisitExpression (Expression.Convert (expression.Left, expression.Type));
        }

        // We'll pull out the bool conversion marker from the operands of the Coalesce expression and instead put it around the whole expression.
        // That way, HandleValueSemantics will not try to convert us back to a value; this avoids double CASE WHENs.
        // We know that left and right must be ConvertedBooleanExpressions because Coalesce has single value semantics for its operands, and boolean
        // Coalesces must have booleans operands. Applying value semantics to boolean operands results in ConvertedBooleanExpression values.
        
        Debug.Assert (childContext == SqlExpressionContext.SingleValueRequired);
        Debug.Assert (left is SqlConvertedBooleanExpression);
        Debug.Assert (right is SqlConvertedBooleanExpression);
        var newCoalesceExpression = Expression.Coalesce (((SqlConvertedBooleanExpression) left).Expression, ((SqlConvertedBooleanExpression) right).Expression);
        return new SqlConvertedBooleanExpression (newCoalesceExpression);
      }

      if (left != expression.Left || right != expression.Right)
        return ConversionUtility.MakeBinaryWithOperandConversion (expression.NodeType, left, right, expression.IsLiftedToNull, expression.Method);

      return expression;
    }

    protected override Expression VisitUnaryExpression (UnaryExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var childContext = GetChildSemanticsForUnaryExpression (expression);
      var newOperand = ApplySqlExpressionContext (expression.Operand, childContext, _stage, _context);

      if (newOperand != expression.Operand)
      {
        if (expression.NodeType == ExpressionType.Convert)
        {
          // If the operand changes its type due to context application, we must also strip any Convert nodes since they are most likely no longer 
          // applicable after the context switch.
          if (expression.Operand.Type != newOperand.Type)
            return newOperand;

          // If this is a convert of a SqlConvertedBooleanExpression to bool? or bool, move the Convert into the SqlConvertedBooleanExpression
          var convertedBooleanExpressionOperand = newOperand as SqlConvertedBooleanExpression;
          if (convertedBooleanExpressionOperand != null)
          {
            if (expression.Type == typeof (bool))
              return new SqlConvertedBooleanExpression (Expression.Convert (convertedBooleanExpressionOperand.Expression, typeof (int)));
            else if (expression.Type == typeof (bool?))
              return new SqlConvertedBooleanExpression (Expression.Convert (convertedBooleanExpressionOperand.Expression, typeof (int?)));
          }
        }

        return Expression.MakeUnary (expression.NodeType, newOperand, expression.Type, expression.Method);
      }

      return expression;
    }

    public Expression VisitSqlIsNullExpression (SqlIsNullExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newExpression = ApplySqlExpressionContext (expression.Expression, SqlExpressionContext.SingleValueRequired, _stage, _context);
      if (newExpression != expression.Expression)
        return new SqlIsNullExpression (newExpression);
      return expression;
    }

    public Expression VisitSqlIsNotNullExpression (SqlIsNotNullExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newExpression = ApplySqlExpressionContext (expression.Expression, SqlExpressionContext.SingleValueRequired, _stage, _context);
      if (newExpression != expression.Expression)
        return new SqlIsNotNullExpression (newExpression);
      return expression;
    }

    public Expression VisitSqlEntityConstantExpression (SqlEntityConstantExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      if (_currentContext == SqlExpressionContext.SingleValueRequired)
        return expression.PrimaryKeyExpression;
      else
        return expression; // rely on VisitExpression to apply correct semantics
    }

    public Expression VisitSqlSubStatementExpression (SqlSubStatementExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newSqlStatement = _stage.ApplySelectionContext (expression.SqlStatement, _currentContext, _context);
      if (!ReferenceEquals (expression.SqlStatement, newSqlStatement))
        return new SqlSubStatementExpression (newSqlStatement);
      return expression;
    }

    public Expression VisitSqlEntityRefMemberExpression (SqlEntityRefMemberExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedJoinInfo = _stage.ResolveJoinInfo (
          new UnresolvedJoinInfo (expression.OriginatingEntity, expression.MemberInfo, JoinCardinality.One), _context);
      switch (_currentContext)
      {
        case SqlExpressionContext.ValueRequired:
          return _stage.ResolveEntityRefMemberExpression (expression, resolvedJoinInfo, _context);
        case SqlExpressionContext.SingleValueRequired:
          var columnExpression = resolvedJoinInfo.RightKey as SqlColumnExpression;
          if (columnExpression != null && columnExpression.IsPrimaryKey)
            return resolvedJoinInfo.LeftKey;
          else
            return _stage.ResolveEntityRefMemberExpression (expression, resolvedJoinInfo, _context).PrimaryKeyColumn;
      }
      
      var message = string.Format (
          "Context '{0}' is not allowed for members referencing entities: '{1}'.", 
          _currentContext, 
          FormattingExpressionTreeVisitor.Format (expression));
      throw new NotSupportedException (message);
    }

    public Expression VisitNamedExpression (NamedExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var expressionWithAppliedInnerContext = new NamedExpression (
          expression.Name,
          ApplySqlExpressionContext (expression.Expression, _currentContext, _stage, _context));

      var result = NamedExpressionCombiner.ProcessNames (_context, expressionWithAppliedInnerContext);

      if (result != expressionWithAppliedInnerContext || expressionWithAppliedInnerContext.Expression != expression.Expression)
        return result;
      else
        return expression;
    }

    protected override Expression VisitNewExpression (NewExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var expressions = expression.Arguments.Select (expr => ApplySqlExpressionContext (expr, SqlExpressionContext.ValueRequired, _stage, _context));
// ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (expression.Members != null && expression.Members.Count > 0)
        return Expression.New (expression.Constructor, expressions, expression.Members);
      else
        return Expression.New (expression.Constructor, expressions);
// ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    public Expression VisitSqlGroupingSelectExpression (SqlGroupingSelectExpression expression)
    {
      var newKeyExpression = ApplySqlExpressionContext (expression.KeyExpression, SqlExpressionContext.ValueRequired, _stage, _context);
      var newElementExpression = ApplySqlExpressionContext (expression.ElementExpression, SqlExpressionContext.ValueRequired, _stage, _context);
      var newAggregationExpressions = expression.AggregationExpressions
          .Select (e => ApplySqlExpressionContext (e, SqlExpressionContext.ValueRequired, _stage, _context))
          .ToArray();

      if (newKeyExpression != expression.KeyExpression
          || newElementExpression != expression.ElementExpression
          || !newAggregationExpressions.SequenceEqual (expression.AggregationExpressions))
        return _context.UpdateGroupingSelectAndAddMapping (expression, newKeyExpression, newElementExpression, newAggregationExpressions);

      return expression;
    }

    public Expression VisitSqlTableReferenceExpression (SqlTableReferenceExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlFunctionExpression (SqlFunctionExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlConvertExpression (SqlConvertExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlExistsExpression (SqlExistsExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlRowNumberExpression (SqlRowNumberExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlLikeExpression (SqlLikeExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlLengthExpression (SqlLengthExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlLiteralExpression (SqlLiteralExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    public Expression VisitSqlBinaryOperatorExpression (SqlBinaryOperatorExpression expression)
    {
      return VisitChildrenWithSingleValueSemantics (expression);
    }

    protected override Expression VisitInvocationExpression (InvocationExpression expression)
    {
      var message = string.Format (
          "InvocationExpressions are not supported in the SQL backend. Expression: '{0}'.", FormattingExpressionTreeVisitor.Format (expression));
      throw new NotSupportedException (message);
    }

    protected override Expression VisitLambdaExpression (LambdaExpression expression)
    {
      var message = string.Format (
          "LambdaExpressions are not supported in the SQL backend. Expression: '{0}'.", FormattingExpressionTreeVisitor.Format (expression));
      throw new NotSupportedException (message);
    }

    private Expression VisitChildrenWithSingleValueSemantics (ExtensionExpression expression)
    {
      var visitor = new SqlContextExpressionVisitor (SqlExpressionContext.SingleValueRequired, _stage, _context);
      return visitor.VisitExtensionExpression (expression);
    }

    private SqlExpressionContext GetChildSemanticsForUnaryExpression (Expression expression)
    {
      switch (expression.NodeType)
      {
        case ExpressionType.Convert:
          return _currentContext;
        case ExpressionType.Not:
          if (BooleanUtility.IsBooleanType (expression.Type))
            return SqlExpressionContext.PredicateRequired;
          else
            return SqlExpressionContext.SingleValueRequired;
        default:
          return SqlExpressionContext.SingleValueRequired;
      }
    }

    private SqlExpressionContext GetChildSemanticsForBinaryBoolExpression (ExpressionType expressionType)
    {
      switch (expressionType)
      {
        case ExpressionType.AndAlso:
        case ExpressionType.OrElse:
        case ExpressionType.And:
        case ExpressionType.Or:
        case ExpressionType.ExclusiveOr:
          return SqlExpressionContext.PredicateRequired;

        default:
          // case ExpressionType.NotEqual:
          // case ExpressionType.Equal:
          // case ExpressionType.Coalesce:
          return SqlExpressionContext.SingleValueRequired;
      }
    }

    private Expression HandleValueSemantics (Expression expression)
    {
      var newExpression = base.VisitExpression (expression);
      if (newExpression is SqlConvertedBooleanExpression)
        return newExpression;

      if (BooleanUtility.IsBooleanType (newExpression.Type))
      {
        var convertedExpression = new SqlPredicateAsValueExpression (newExpression);
        return new SqlConvertedBooleanExpression (convertedExpression);
      }
      else
      {
        return newExpression;
      }
    }

    private Expression HandlePredicateSemantics (Expression expression)
    {
      var newExpression = base.VisitExpression (expression);

      var convertedBooleanExpression = newExpression as SqlConvertedBooleanExpression;
      if (convertedBooleanExpression != null)
      {
        var isNullableExpression = convertedBooleanExpression.Expression.Type == typeof (int?);
        return Expression.Equal (convertedBooleanExpression.Expression, new SqlLiteralExpression (1, isNullableExpression), isNullableExpression, null);
      }

      if (!BooleanUtility.IsBooleanType (newExpression.Type))
      {
        var message = string.Format (
            "Cannot convert an expression of type '{0}' to a boolean expression. Expression: '{1}'", 
            newExpression.Type, 
            FormattingExpressionTreeVisitor.Format(expression));
        throw new NotSupportedException (message);
      }

      return newExpression;
    }
  }
}