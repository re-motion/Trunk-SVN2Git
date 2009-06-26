// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Utilities;

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
                                                                   () => Queryable.SelectMany<object, object[], object> (null, o => null, null))
                                                           };

    private readonly ResolvedExpressionCache _cachedCollectionSelector;
    private readonly ResolvedExpressionCache _cachedResultSelector;

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

      _cachedCollectionSelector = new ResolvedExpressionCache (Source);
      _cachedResultSelector = new ResolvedExpressionCache (Source);
    }

    public LambdaExpression CollectionSelector { get; private set; }
    public LambdaExpression ResultSelector { get; private set; }

    public Type QuerySourceElementType
    {
      get { return ResultSelector.Parameters[1].Type; }
    }

    public Expression GetResolvedCollectionSelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedCollectionSelector.GetOrCreate (r => r.GetResolvedExpression (CollectionSelector.Body, CollectionSelector.Parameters[0], clauseGenerationContext));
    }

    public Expression GetResolvedResultSelector (ClauseGenerationContext clauseGenerationContext)
    {
      // our result selector usually looks like this: (i, j) => new { i = i, j = j }
      // with the data for i coming from the previous node and j identifying the data from this node

      // we resolve the selector by first substituting j by a QuerySourceReferenceExpression pointing back to us, before asking the previous node 
      // to resolve i

      return _cachedResultSelector.GetOrCreate (r => r.GetResolvedExpression (
          GetResultSelectorWithBackReference (clauseGenerationContext.ClauseMapping),
          ResultSelector.Parameters[0],
          clauseGenerationContext));
    }

    private Expression GetResultSelectorWithBackReference (QuerySourceClauseMapping querySourceClauseMapping)
    {
      var clause = GetClauseForResolve (querySourceClauseMapping);
      var referenceExpression = new QuerySourceReferenceExpression (clause);

      return ReplacingVisitor.Replace (ResultSelector.Parameters[1], referenceExpression, ResultSelector.Body);
    }

    private FromClauseBase GetClauseForResolve (QuerySourceClauseMapping querySourceClauseMapping)
    {
      try
      {
        return querySourceClauseMapping.GetClause (this);
      }
      catch (KeyNotFoundException ex)
      {
        var message = string.Format (
            "Cannot resolve with a {0} for which no clause was created. Be sure to call CreateClause before calling GetResolvedResultSelector, and pass in the same "
            + "QuerySourceClauseMapping to both methods.",
            GetType ().Name);
        throw new InvalidOperationException (message, ex);
      }
    }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // we modify the structure of the stream of data coming into this node by our result selector,
      // so we first resolve the result selector, then we substitute the result for the inputParameter in the expressionToBeResolved
      var resolvedResultSelector = GetResolvedResultSelector (clauseGenerationContext);
      return ReplacingVisitor.Replace (inputParameter, resolvedResultSelector, expressionToBeResolved);
    }

    public override void Apply (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var clause = CreateFromClauseOfCorrectType (
          ResultSelector.Parameters[1].Name,
          ResultSelector.Parameters[1].Type,
          clauseGenerationContext);
      queryModel.BodyClauses.Add ((IBodyClause) clause);

      clauseGenerationContext.ClauseMapping.AddMapping (this, clause);

      var selectClause = ((SelectClause) queryModel.SelectOrGroupClause);
      selectClause.Selector = GetResolvedResultSelector (clauseGenerationContext);
    }

    private FromClauseBase CreateFromClauseOfCorrectType (string itemName, Type itemType, ClauseGenerationContext clauseGenerationContext)
    {
      var resolvedCollectionSelector = GetResolvedCollectionSelector (clauseGenerationContext);
      if (resolvedCollectionSelector is SubQueryExpression)
        return new SubQueryFromClause (itemName, itemType, (SubQueryExpression) resolvedCollectionSelector);
      else
        return new AdditionalFromClause (itemName, itemType, resolvedCollectionSelector);
    }
  }
}