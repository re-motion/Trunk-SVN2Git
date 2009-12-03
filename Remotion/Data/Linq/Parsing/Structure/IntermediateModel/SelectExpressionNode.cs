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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for 
  /// <see cref="Queryable.Select{TSource,TResult}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,TResult}})"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// </summary>
  public class SelectExpressionNode : MethodCallExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Select<object, object> (null, o => null)),
                                                               GetSupportedMethod (() => Enumerable.Select<object, object> (null, o => null)),
                                                           };

    private readonly ResolvedExpressionCache _cachedSelector;

    public SelectExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression selector)
        : base (parseInfo)
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      if (selector.Parameters.Count != 1)
        throw new ArgumentException ("Selector must have exactly one parameter.", "selector");

      Selector = selector;
      _cachedSelector = new ResolvedExpressionCache (this);
    }

    public LambdaExpression Selector { get; private set; }

    public Expression GetResolvedSelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedSelector.GetOrCreate (r => r.GetResolvedExpression (Selector.Body, Selector.Parameters[0], clauseGenerationContext));
    }

    public override Expression Resolve (
        ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // we modify the structure of the stream of data coming into this node by our selector,
      // so we first resolve the selector, then we substitute the result for the inputParameter in the expressionToBeResolved
      var resolvedSelector = GetResolvedSelector (clauseGenerationContext);
      return ReplacingExpressionTreeVisitor.Replace (inputParameter, resolvedSelector, expressionToBeResolved);
    }

    protected override QueryModel ApplyNodeSpecificSemantics (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      queryModel.SelectClause.Selector = GetResolvedSelector (clauseGenerationContext);
      return queryModel;
    }
  }
}
