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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for 
  /// <see cref="Queryable.Where{TSource}(System.Linq.IQueryable{TSource},System.Linq.Expressions.Expression{System.Func{TSource,bool}})"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// </summary>
  public class WhereExpressionNode : MethodCallExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Where<object> (null, o => true))
                                                           };

    private Expression _cachedPredicate;

    public WhereExpressionNode (IExpressionNode source, LambdaExpression predicate)
      : base (ArgumentUtility.CheckNotNull ("source", source))
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      if (predicate != null && predicate.Parameters.Count != 1)
        throw new ArgumentException ("Predicate must have exactly one parameter.", "predicate");

      Predicate = predicate;
    }

    public LambdaExpression Predicate { get; private set; }

    public Expression GetResolvedPredicate ()
    {
      if (_cachedPredicate == null)
        _cachedPredicate = Source.Resolve (Predicate.Parameters[0], Predicate.Body);

      return _cachedPredicate;
    }

    public override Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved)
    {
      // this simply streams its input data to the output without modifying its structure, so we resolve by passing on the data to the previous node
      return Source.Resolve (inputParameter, expressionToBeResolved);
    }

    public override ParameterExpression CreateParameterForOutput ()
    {
      // this simply streams its input data to the output without modifying its structure, so we let the previous node create the parameter
      return Source.CreateParameterForOutput ();
    }

    public override IClause CreateClause (IClause previousClause)
    {
      ArgumentUtility.CheckNotNull ("previousClause", previousClause);
      return new WhereClause (previousClause, Predicate);
    }
  }
}