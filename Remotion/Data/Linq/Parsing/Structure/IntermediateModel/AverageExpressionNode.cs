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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for the different overloads of <see cref="O:Queryable.Average"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it marks the beginning (i.e. the last node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class AverageExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<decimal>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<decimal?>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<double>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<double?>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<int>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<int?>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<long>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<long?>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<float>) null)),
                                                               GetSupportedMethod (() => Queryable.Average ((IQueryable<float?>) null)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (decimal) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (decimal?) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (double) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (double?) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (int) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (int?) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (long) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (long?) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (float) 0)),
                                                               GetSupportedMethod (() => Queryable.Average<object> (null, o => (float?) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<decimal>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<decimal?>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<double>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<double?>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<int>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<int?>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<long>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<long?>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<float>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average ((IEnumerable<float?>) null)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (decimal) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (decimal?) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (double) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (double?) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (int) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (int?) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (long) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (long?) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (float) 0)),
                                                               GetSupportedMethod (() => Enumerable.Average<object> (null, o => (float?) 0))
                                                           };

    public AverageExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression optionalPredicate)
        : base(parseInfo, null, optionalPredicate)
    {
    }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // no data streams out from this node, so we cannot resolve any expressions
      throw CreateResolveNotSupportedException ();
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new AverageResultOperator ();
    }
  }
}
