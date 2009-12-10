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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for the <see cref="Queryable.Any{TSource}(System.Linq.IQueryable{TSource})"/>,
  /// <see cref="Queryable.Any{TSource}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,bool}})"/>,
  /// <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>, and
  /// <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,bool})"/> methods.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class AnyExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Any<object> (null)),
                                                               GetSupportedMethod (() => Queryable.Any<object> (null, null)),
                                                               GetSupportedMethod (() => Enumerable.Any<object> (null)),
                                                               GetSupportedMethod (() => Enumerable.Any<object> (null, null))
                                                           };


    public AnyExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
        : base (parseInfo, optionalPredicate, null)
    {
    }

    public override Expression Resolve (
        ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      // no data streams out from this node, so we cannot resolve any expressions
      throw CreateResolveNotSupportedException();
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new AnyResultOperator();
    }
  }
}
