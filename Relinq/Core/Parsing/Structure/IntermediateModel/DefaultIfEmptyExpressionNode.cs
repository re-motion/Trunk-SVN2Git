// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for <see cref="Queryable.DefaultIfEmpty{TSource}(System.Linq.IQueryable{TSource})"/> and
  /// <see cref="Queryable.DefaultIfEmpty{TSource}(System.Linq.IQueryable{TSource},TSource)"/> and 
  /// <see cref="Enumerable.DefaultIfEmpty{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> and
  /// <see cref="Enumerable.DefaultIfEmpty{TSource}(System.Collections.Generic.IEnumerable{TSource},TSource)"/>
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// When this node is used, it usually follows (or replaces) a <see cref="SelectExpressionNode"/> of an <see cref="IExpressionNode"/> chain that 
  /// represents a query.
  /// </summary>
  public class DefaultIfEmptyExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.DefaultIfEmpty<object> (null)),
                                                               GetSupportedMethod (() => Queryable.DefaultIfEmpty<object> (null,null)),
                                                               GetSupportedMethod (() => Enumerable.DefaultIfEmpty<object> (null)),
                                                               GetSupportedMethod (() => Enumerable.DefaultIfEmpty<object> (null, null)),
                                                           };

    public DefaultIfEmptyExpressionNode (MethodCallExpressionParseInfo parseInfo, Expression optionalDefaultValue)
        : base(parseInfo, null, null)
    {
      OptionalDefaultValue = optionalDefaultValue;
    }

    public Expression OptionalDefaultValue { get; set; }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // this simply streams its input data to the output without modifying its structure, so we resolve by passing on the data to the previous node
      return Source.Resolve (inputParameter, expressionToBeResolved, clauseGenerationContext);
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      return new DefaultIfEmptyResultOperator (OptionalDefaultValue);
    }
  }
}
