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
using System.Reflection;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="ResolvingExpressionVisitor"/> implements <see cref="IUnresolvedSqlExpressionVisitor"/> and <see cref="ThrowingExpressionTreeVisitor"/>.
  /// </summary>
  public class ResolvingExpressionVisitor : ExpressionTreeVisitor, IUnresolvedSqlExpressionVisitor, ISqlSubStatementVisitor
  {
    private readonly IMappingResolver _resolver;
    private readonly IMappingResolutionStage _stage;
    private readonly IMappingResolutionContext _context;
    private readonly UniqueIdentifierGenerator _generator;

    public static Expression ResolveExpression (
        Expression expression,
        IMappingResolver resolver,
        IMappingResolutionStage stage,
        IMappingResolutionContext context,
        UniqueIdentifierGenerator generator
        )
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var visitor = new ResolvingExpressionVisitor (resolver, stage, context, generator);
      var result = visitor.VisitExpression (expression);
      return result;
    }

    protected IMappingResolver Resolver
    {
      get { return _resolver; }
    }

    protected IMappingResolutionStage Stage
    {
      get { return _stage; }
    }

    protected IMappingResolutionContext Context
    {
      get { return _context; }
    }

    protected UniqueIdentifierGenerator Generator
    {
      get { return _generator; }
    }

    protected ResolvingExpressionVisitor (IMappingResolver resolver, IMappingResolutionStage stage, IMappingResolutionContext context, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("generator", generator);

      _resolver = resolver;
      _stage = stage;
      _context = context;
      _generator = generator;
    }

    public Expression VisitSqlTableReferenceExpression (SqlTableReferenceExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = _stage.ResolveTableReferenceExpression (expression, _context);
      return VisitExpression (resolvedExpression);
    }

    protected override Expression VisitConstantExpression (ConstantExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return _resolver.ResolveConstantExpression (expression);
    }

    protected override Expression VisitMemberExpression (MemberExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      // First process any nested expressions
      // E.g, for (kitchen.Cook).FirstName, first process kitchen => newExpression1 (SqlEntity)
      // then newExpression1.Cook => newExpression2 (SqlEntityRef/SqlEntity)
      // then newExpression2.FirstName => result (SqlColumn)

      var sourceExpression = VisitExpression (expression.Expression);
      return _stage.ResolveMemberAccess (sourceExpression, expression.Member, _resolver, _context);
    }

    protected override Expression VisitBinaryExpression (BinaryExpression expression)
    {
      var newBinaryExpression = (BinaryExpression) base.VisitBinaryExpression (expression);

      // NewExpressions are compared by comparing them member-wise
      var leftExpressionAsNewExpression = newBinaryExpression.Left as NewExpression;
      var rightExpressionAsNewExpression = newBinaryExpression.Right as NewExpression;

      // TODO Review 3031: The BinaryExpression might have a conversion lambda, null lifting, and MethodInfo associated with it. Those should not be removed by the following three calls. Write tests showing that these items are retained. (Take a look at the tests for ExpressionTreeVisitor to see how to construct such expressions.)
      if (leftExpressionAsNewExpression != null && rightExpressionAsNewExpression != null)
        return GetBinaryExpressionForNewExpressionComparison (expression.NodeType, leftExpressionAsNewExpression, rightExpressionAsNewExpression);

      if (leftExpressionAsNewExpression != null)
        return GetBinaryExpressionForMemberAccessComparison (expression.NodeType, leftExpressionAsNewExpression, newBinaryExpression.Right);

      if (rightExpressionAsNewExpression != null)
        return GetBinaryExpressionForMemberAccessComparison (expression.NodeType, rightExpressionAsNewExpression, newBinaryExpression.Left);

      return newBinaryExpression;
    }

    protected override Expression VisitTypeBinaryExpression (TypeBinaryExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var newExpression = VisitExpression (expression.Expression);
      var resolvedTypeExpression = _resolver.ResolveTypeCheck (newExpression, expression.TypeOperand);
      return VisitExpression (resolvedTypeExpression);
    }

    public virtual Expression VisitSqlSubStatementExpression (SqlSubStatementExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedSqlStatement = _stage.ResolveSqlStatement (expression.SqlStatement, _context);

      return GroupAggregateSimplifier.SimplifyIfPossible (resolvedSqlStatement, expression.SqlStatement.SelectProjection, _stage, _context);
    }

    Expression IUnresolvedSqlExpressionVisitor.VisitSqlEntityRefMemberExpression (SqlEntityRefMemberExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return base.VisitUnknownExpression (expression);
    }

    Expression IUnresolvedSqlExpressionVisitor.VisitSqlEntityConstantExpression (SqlEntityConstantExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return base.VisitUnknownExpression (expression);
    }

    private Expression GetBinaryExpressionForNewExpressionComparison (
        ExpressionType expressionType, 
        NewExpression leftNewExpression, 
        NewExpression rightNewExpression)
    {
      // TODO Review 3031: Use Equals to compare ConstructorInfos
      if (leftNewExpression.Constructor != rightNewExpression.Constructor)
        throw new NotSupportedException ("The results of constructor invocations can only be compared if the same ctors are used.");

      Expression binaryExpression = null;
      for (int i = 0; i < leftNewExpression.Arguments.Count; i++)
      {
        var argumentComparisonExpression = Expression.MakeBinary (
            expressionType, leftNewExpression.Arguments[i], rightNewExpression.Arguments[i]);

        if (binaryExpression == null)
          binaryExpression = argumentComparisonExpression;
        else
          binaryExpression = Expression.AndAlso (binaryExpression, argumentComparisonExpression);
      }
      return binaryExpression;
    }

    private Expression GetBinaryExpressionForMemberAccessComparison (
        ExpressionType expressionType, 
        NewExpression newExpression, 
        Expression memberAccessExpression)
    {
      if (newExpression.Members == null || newExpression.Members.Count == 0)
      {
        throw new NotSupportedException (
            "Compound values can only be compared if the respective new expression has members associated with it.");
      }

      Expression binaryExpression = null;
      for (int i = 0; i < newExpression.Members.Count; i++)
      {
        Expression memberExpression;
        if (newExpression.Members[i] is MethodInfo)
          memberExpression = Expression.Call (memberAccessExpression, (MethodInfo) newExpression.Members[i]);
        else
          memberExpression = Expression.MakeMemberAccess (memberAccessExpression, newExpression.Members[i]);
        var argumentComparisonExpression = Expression.MakeBinary (expressionType, newExpression.Arguments[i], memberExpression);

        if (binaryExpression == null)
          binaryExpression = argumentComparisonExpression;
        else
          binaryExpression = Expression.AndAlso (binaryExpression, argumentComparisonExpression);
      }
      return PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees (binaryExpression);
    }
  }
}