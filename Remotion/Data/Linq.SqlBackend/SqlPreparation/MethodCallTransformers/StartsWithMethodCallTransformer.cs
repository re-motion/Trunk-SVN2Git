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
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  /// <summary>
  /// <see cref="StartsWithMethodCallTransformer"/> implements <see cref="IMethodCallTransformer"/> for the starts-with method.
  /// </summary>
  public class StartsWithMethodCallTransformer : IMethodCallTransformer
  {
    public static readonly MethodInfo[] SupportedMethods =
        new[]
        {
            typeof (string).GetMethod ("StartsWith", new[] { typeof (string) })
        };

    public Expression Transform (MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      MethodCallTransformerUtility.CheckConstantExpression (methodCallExpression.Method.Name, methodCallExpression.Arguments[0]);
      
      var rightExpression =
          Expression.Constant (
              string.Format ("{0}%", LikeEscapeUtility.Escape (((ConstantExpression) methodCallExpression.Arguments[0]).Value.ToString())));

      return new SqlBinaryOperatorExpression ("LIKE", methodCallExpression.Object, rightExpression);
    }
  }
}