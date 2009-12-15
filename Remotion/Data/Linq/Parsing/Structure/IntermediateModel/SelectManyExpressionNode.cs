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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for 
  /// <see cref="Queryable.SelectMany{TSource,TCollection,TResult}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,System.Collections.Generic.IEnumerable{TCollection}}},System.Linq.Expressions.Expression{System.Func{TSource,TCollection,TResult}})"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// This node represents an additional query source introduced to the query.
  /// </summary>
  public class SelectManyExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (
                                                                   () => Queryable.SelectMany<object, object[], object> (null, o => null, null)),
                                                               GetSupportedMethod (
                                                                   () => Enumerable.SelectMany<object, object[], object> (null, o => null, null)),
                                                           };

    private readonly ResolvedExpressionCache<Expression> _cachedCollectionSelector;
    private readonly ResolvedExpressionCache<Expression> _cachedResultSelector;

    public SelectManyExpressionNode (
        MethodCallExpressionParseInfo parseInfo, LambdaExpression collectionSelector, LambdaExpression resultSelector)
        : base (parseInfo)
    {
      ArgumentUtility.CheckNotNull ("collectionSelector", collectionSelector);
      ArgumentUtility.CheckNotNull ("resultSelector", resultSelector);

      if (collectionSelector.Parameters.Count != 1)
        throw new ArgumentException ("Collection selector must have exactly one parameter.", "collectionSelector");
      if (resultSelector.Parameters.Count != 2)
        throw new ArgumentException ("Result selector must have exactly two parameters.", "resultSelector");

      CollectionSelector = collectionSelector;
      ResultSelector = resultSelector;

      _cachedCollectionSelector = new ResolvedExpressionCache<Expression> (this);
      _cachedResultSelector = new ResolvedExpressionCache<Expression> (this);
    }

    public LambdaExpression CollectionSelector { get; private set; }
    public LambdaExpression ResultSelector { get; private set; }

    public Expression GetResolvedCollectionSelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedCollectionSelector.GetOrCreate (
          r => r.GetResolvedExpression (CollectionSelector.Body, CollectionSelector.Parameters[0], clauseGenerationContext));
    }

    public Expression GetResolvedResultSelector (ClauseGenerationContext clauseGenerationContext)
    {
      // our result selector usually looks like this: (i, j) => new { i = i, j = j }
      // with the data for i coming from the previous node and j identifying the data from this node

      // we resolve the selector by first substituting j by a QuerySourceReferenceExpression pointing back to us, before asking the previous node 
      // to resolve i

      return _cachedResultSelector.GetOrCreate (
          r => r.GetResolvedExpression (
                   QuerySourceExpressionNodeUtility.ReplaceParameterWithReference (this, ResultSelector.Parameters[1], ResultSelector.Body, clauseGenerationContext),
                   ResultSelector.Parameters[0],
                   clauseGenerationContext));
    }

    public override Expression Resolve (
        ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // we modify the structure of the stream of data coming into this node by our result selector,
      // so we first resolve the result selector, then we substitute the result for the inputParameter in the expressionToBeResolved
      var resolvedResultSelector = GetResolvedResultSelector (clauseGenerationContext);
      return ReplacingExpressionTreeVisitor.Replace (inputParameter, resolvedResultSelector, expressionToBeResolved);
    }

    protected override QueryModel ApplyNodeSpecificSemantics (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var resolvedCollectionSelector = GetResolvedCollectionSelector (clauseGenerationContext);
      var clause = new AdditionalFromClause (ResultSelector.Parameters[1].Name, ResultSelector.Parameters[1].Type, resolvedCollectionSelector);
      queryModel.BodyClauses.Add (clause);

      clauseGenerationContext.AddContextInfo (this, clause);

      var selectClause = queryModel.SelectClause;
      selectClause.Selector = GetResolvedResultSelector (clauseGenerationContext);

      return queryModel;
    }
  }
}
