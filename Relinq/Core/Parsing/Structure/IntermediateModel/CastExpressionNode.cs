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
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for 
  /// <see cref="Queryable.Cast{TResult}"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// </summary>
  public class CastExpressionNode : ResultOperatorExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.Cast<object> (null)),
                                                               GetSupportedMethod (() => Enumerable.Cast<object> (null)),
                                                           };

    public CastExpressionNode (MethodCallExpressionParseInfo parseInfo)
        : base (parseInfo, null, null)
    {
      if (!parseInfo.ParsedExpression.Method.IsGenericMethod || parseInfo.ParsedExpression.Method.GetGenericArguments ().Length != 1)
        throw new ArgumentException ("The parsed method must have exactly one generic argument.", "parseInfo");
    }

    public Type CastItemType
    {
      get
      {
        return ParsedExpression.Method.GetGenericArguments ()[0];
      }
    }

    public override Expression Resolve (
        ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      var convertExpression = Expression.Convert (inputParameter, CastItemType);
      var expressionWithCast = ReplacingExpressionTreeVisitor.Replace (inputParameter, convertExpression, expressionToBeResolved);
      return Source.Resolve (inputParameter, expressionWithCast, clauseGenerationContext);
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      var castItemType = CastItemType;
      return new CastResultOperator (castItemType);
    }
  }
}
