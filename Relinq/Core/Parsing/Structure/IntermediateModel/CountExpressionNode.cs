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
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for <see cref="Queryable.Count{TSource}(System.Linq.IQueryable{TSource})"/>,
  /// <see cref="Queryable.Count{TSource}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,bool}})"/>,
  /// for the Count properties of <see cref="List{T}"/>, <see cref="ArrayList"/>, <see cref="ICollection{T}"/>, and <see cref="ICollection"/>, 
  /// and for the <see cref="Array.Length"/> property of arrays.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class CountExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Count<object> (null)),
                                                               GetSupportedMethod (() => Queryable.Count<object> (null, null)),
                                                               GetSupportedMethod (() => Enumerable.Count<object> (null)),
                                                               GetSupportedMethod (() => Enumerable.Count<object> (null, null)),
// ReSharper disable PossibleNullReferenceException
                                                               GetSupportedMethod (() => ((List<int>) null).Count),
                                                               GetSupportedMethod (() => ((ICollection<int>) null).Count),
                                                               GetSupportedMethod (() => ((ArrayList) null).Count),
                                                               GetSupportedMethod (() => ((ICollection) null).Count),
                                                               GetSupportedMethod (() => (((Array) null).Length)),
// ReSharper restore PossibleNullReferenceException
                                                           };


    public CountExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
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
      return new CountResultOperator();
    }
  }
}
