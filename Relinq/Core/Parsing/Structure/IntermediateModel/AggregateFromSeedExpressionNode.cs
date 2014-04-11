// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for the 
  /// <see cref="Queryable.Aggregate{TSource,TAccumulate}"/>, <see cref="Queryable.Aggregate{TSource,TAccumulate,TResult}"/>,
  /// <see cref="Enumerable.Aggregate{TSource,TAccumulate}"/>, and <see cref="Enumerable.Aggregate{TSource,TAccumulate,TResult}"/>
  /// methods.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class AggregateFromSeedExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Aggregate<object, object>(null, null, (o1, o2) => null)),
                                                               GetSupportedMethod (() => Queryable.Aggregate<object, object, object>(null, null, (o1, o2) => null, o => null)),
                                                               GetSupportedMethod (() => Enumerable.Aggregate<object, object>(null, null, (o1, o2) => null)),
                                                               GetSupportedMethod (() => Enumerable.Aggregate<object, object, object>(null, null, (o1, o2) => null, o => null)),
                                                           };

    private readonly ResolvedExpressionCache<LambdaExpression> _cachedFunc;

    public AggregateFromSeedExpressionNode (
        MethodCallExpressionParseInfo parseInfo, 
        Expression seed, 
        LambdaExpression func,
        LambdaExpression optionalResultSelector)
      : base(parseInfo, null, null)
    {
      ArgumentUtility.CheckNotNull ("seed", seed);
      ArgumentUtility.CheckNotNull ("func", func);

      if (func.Parameters.Count != 2)
        throw new ArgumentException ("Func must have exactly two parameters.", "func");

      if (optionalResultSelector != null && optionalResultSelector.Parameters.Count != 1)
        throw new ArgumentException ("Result selector must have exactly one parameter.", "optionalResultSelector");

      Seed = seed;
      Func = func;
      OptionalResultSelector = optionalResultSelector;

      _cachedFunc = new ResolvedExpressionCache<LambdaExpression> (this);
    }

    public Expression Seed { get; private set; }
    public LambdaExpression Func { get; private set; }
    public LambdaExpression OptionalResultSelector { get; private set; }

    public LambdaExpression GetResolvedFunc (ClauseGenerationContext clauseGenerationContext)
    {
      // '(total, current) => total + current' becomes 'total => total + [current]'
      return _cachedFunc.GetOrCreate (
          r => Expression.Lambda (r.GetResolvedExpression (Func.Body, Func.Parameters[1], clauseGenerationContext), Func.Parameters[0]));
    }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      throw CreateResolveNotSupportedException ();
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new AggregateFromSeedResultOperator (Seed, GetResolvedFunc (clauseGenerationContext), OptionalResultSelector);
    }
  }
}