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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  /// <summary>
  /// <see cref="SubstringMethodCallTransformer"/> implements <see cref="IMethodCallTransformer"/> for the substring method.
  /// </summary>
  public class SubstringMethodCallTransformer : IMethodCallTransformer
  {
    public static readonly MethodInfo[] SupportedMethods =
        new[]
        {
            MethodCallTransformerUtility.GetInstanceMethod (typeof (string), "Substring", typeof (int)),
            MethodCallTransformerUtility.GetInstanceMethod (typeof (string), "Substring", typeof (int), typeof (int))
        };

    public Expression Transform (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      MethodCallTransformerUtility.CheckArgumentCount (methodCallExpression, 1, 2);
      MethodCallTransformerUtility.CheckInstanceMethod (methodCallExpression);

      var startIndexExpression = Expression.Add (methodCallExpression.Arguments[0], new SqlLiteralExpression (1));

      if (methodCallExpression.Arguments.Count == 1)
      {
        return new SqlFunctionExpression (
            methodCallExpression.Type,
            "SUBSTRING",
            methodCallExpression.Object,
            startIndexExpression,
            new SqlFunctionExpression (methodCallExpression.Object.Type, "LEN", methodCallExpression.Object));
      }
      else if (methodCallExpression.Arguments.Count == 2)
      {
        return new SqlFunctionExpression (
            methodCallExpression.Type,
            "SUBSTRING",
            methodCallExpression.Object,
            startIndexExpression,
            methodCallExpression.Arguments[1]);
      }
      else
      {
        var message = string.Format (
            "Substring function with {0} arguments is not supported. Expression: '{1}'",
            methodCallExpression.Arguments.Count,
            FormattingExpressionTreeVisitor.Format (methodCallExpression));
        throw new NotSupportedException (message);
      }
    }
  }
}