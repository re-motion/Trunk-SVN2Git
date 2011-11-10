// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for 
  /// <see cref="Queryable.Join{TOuter,TInner,TKey,TResult}(System.Linq.IQueryable{TOuter},System.Collections.Generic.IEnumerable{TInner},System.Linq.Expressions.Expression{System.Func{TOuter,TKey}},System.Linq.Expressions.Expression{System.Func{TInner,TKey}},System.Linq.Expressions.Expression{System.Func{TOuter,TInner,TResult}})"/>
  /// or <see cref="Enumerable.Join{TOuter,TInner,TKey,TResult}(System.Collections.Generic.IEnumerable{TOuter},System.Collections.Generic.IEnumerable{TInner},System.Func{TOuter,TKey},System.Func{TInner,TKey},System.Func{TOuter,TInner,TResult})"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// </summary>
  public class JoinExpressionNode : MethodCallExpressionNodeBase, IQuerySourceExpressionNode
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (
                                                                   () => Queryable.Join<object, object, object, object> (
                                                                             null, null, o => null, o => null, (o, i) => null)),
                                                               GetSupportedMethod (
                                                                   () => Enumerable.Join<object, object, object, object> (
                                                                             null, null, o => null, o => null, (o, i) => null)),
                                                           };

    private readonly ResolvedExpressionCache<Expression> _cachedOuterKeySelector;
    private readonly ResolvedExpressionCache<Expression> _cachedInnerKeySelector;
    private readonly ResolvedExpressionCache<Expression> _cachedResultSelector;

    public JoinExpressionNode (
        MethodCallExpressionParseInfo parseInfo,
        Expression innerSequence,
        LambdaExpression outerKeySelector,
        LambdaExpression innerKeySelector,
        LambdaExpression resultSelector)
        : base (parseInfo)
    {
      ArgumentUtility.CheckNotNull ("innerSequence", innerSequence);
      ArgumentUtility.CheckNotNull ("outerKeySelector", outerKeySelector);
      ArgumentUtility.CheckNotNull ("innerKeySelector", innerKeySelector);
      ArgumentUtility.CheckNotNull ("resultSelector", resultSelector);

      if (outerKeySelector.Parameters.Count != 1)
        throw new ArgumentException ("Outer key selector must have exactly one parameter.", "outerKeySelector");
      if (innerKeySelector.Parameters.Count != 1)
        throw new ArgumentException ("Inner key selector must have exactly one parameter.", "innerKeySelector");
      if (resultSelector.Parameters.Count != 2)
        throw new ArgumentException ("Result selector must have exactly two parameters.", "resultSelector");

      InnerSequence = innerSequence;
      OuterKeySelector = outerKeySelector;
      InnerKeySelector = innerKeySelector;
      ResultSelector = resultSelector;

      _cachedOuterKeySelector = new ResolvedExpressionCache<Expression> (this);
      _cachedInnerKeySelector = new ResolvedExpressionCache<Expression> (this);
      _cachedResultSelector = new ResolvedExpressionCache<Expression> (this);
    }

    public Expression InnerSequence { get; private set; }
    public LambdaExpression OuterKeySelector { get; private set; }
    public LambdaExpression InnerKeySelector { get; private set; }
    public LambdaExpression ResultSelector { get; private set; }
    
    public Expression GetResolvedOuterKeySelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedOuterKeySelector.GetOrCreate (
          r => r.GetResolvedExpression (OuterKeySelector.Body, OuterKeySelector.Parameters[0], clauseGenerationContext));
    }

    public Expression GetResolvedInnerKeySelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedInnerKeySelector.GetOrCreate (
          r => QuerySourceExpressionNodeUtility.ReplaceParameterWithReference (
              this, InnerKeySelector.Parameters[0], InnerKeySelector.Body, clauseGenerationContext));
    }

    public Expression GetResolvedResultSelector (ClauseGenerationContext clauseGenerationContext)
    {
      return _cachedResultSelector.GetOrCreate (r => r.GetResolvedExpression (
          QuerySourceExpressionNodeUtility.ReplaceParameterWithReference (this, ResultSelector.Parameters[1], ResultSelector.Body, clauseGenerationContext),
          ResultSelector.Parameters[0], clauseGenerationContext));
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
      // The resolved inner key selector has a back-reference to the clause, so we need to create the clause with a dummy selector before we can 
      // get the real inner key selector.
      JoinClause joinClause = CreateJoinClause(clauseGenerationContext);
      queryModel.BodyClauses.Add (joinClause);

      var selectClause = queryModel.SelectClause;
      selectClause.Selector = GetResolvedResultSelector (clauseGenerationContext);

      return queryModel;
    }

    public JoinClause CreateJoinClause (ClauseGenerationContext clauseGenerationContext)
    {
      var dummyInnerKeySelector = Expression.Constant (null);
      var joinClause = new JoinClause (
          ResultSelector.Parameters[1].Name,
          ResultSelector.Parameters[1].Type,
          InnerSequence,
          GetResolvedOuterKeySelector (clauseGenerationContext),
          dummyInnerKeySelector);
      
      clauseGenerationContext.AddContextInfo (this, joinClause);

      joinClause.InnerKeySelector = GetResolvedInnerKeySelector (clauseGenerationContext);
      return joinClause;
    }
  }
}
