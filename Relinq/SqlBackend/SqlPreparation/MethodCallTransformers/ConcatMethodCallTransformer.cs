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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using System.Linq;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  /// <summary>
  /// Implements the <see cref="IMethodCallTransformer"/> interface for the <see cref="O:string.Concat"/> overloads. 
  /// </summary>
  /// <remarks>
  /// Calls to those methods
  /// are represented as simple 
  /// <see cref="Expression.Equal(System.Linq.Expressions.Expression,System.Linq.Expressions.Expression,bool,System.Reflection.MethodInfo)"/> 
  /// expressions that refer to the <see cref="string.Concat(string,string)"/> method. Arguments that are not of static type <see cref="string"/>
  /// are converted to the nvarchar data type via <see cref="SqlConvertExpression"/>.
  /// </remarks>
  public class ConcatMethodCallTransformer : IMethodCallTransformer
  {
    public static readonly MethodInfo[] SupportedMethods = new[]
      {
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (object)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (object), typeof (object)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (object), typeof (object), typeof (object)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (object), typeof (object), typeof (object), typeof (object)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (string), typeof (string)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (string), typeof (string), typeof (string)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (string), typeof (string), typeof (string), typeof (string)),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (object[])),
        MethodCallTransformerUtility.GetStaticMethod (typeof (string), "Concat", typeof (string[])),
      };

    private static readonly MethodInfo s_standardConcatMethodInfo = typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) });

    public Expression Transform (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      var concatenatedItems = GetConcatenatedItems (methodCallExpression);
      return concatenatedItems
          .Select (a => a.Type == typeof (string) ? a : (Expression) new SqlConvertExpression (typeof (string), a))
          .Aggregate ((a1, a2) => Expression.Add (a1, a2, s_standardConcatMethodInfo));
    }

    private IEnumerable<Expression> GetConcatenatedItems (MethodCallExpression methodCallExpression)
    {
      if (methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].Type.IsArray)
      {
        ConstantExpression argumentAsConstantExpression;
        NewArrayExpression argumentAsNewArrayExpression;

        if ((argumentAsNewArrayExpression = methodCallExpression.Arguments[0] as NewArrayExpression) != null)
        {
          return argumentAsNewArrayExpression.Expressions;
        }
        else if ((argumentAsConstantExpression = methodCallExpression.Arguments[0] as ConstantExpression) != null)
        {
          return ((object[]) argumentAsConstantExpression.Value).Select (element => (Expression) Expression.Constant (element));
        }
        else
        {
          var message = string.Format (
              "The method call '{0}' is not supported. When the array overloads of String.Concat are used, only constant or new array expressions can "
              + "be translated to SQL; in this usage, the expression has type '{1}'.",
              FormattingExpressionTreeVisitor.Format (methodCallExpression),
              methodCallExpression.Arguments[0].GetType ());
          throw new NotSupportedException (message);
        }
      }
      else 
      {
        return methodCallExpression.Arguments;
      }
    }
  }
}