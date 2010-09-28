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
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.IntegrationTests.Common.TestDomain.Northwind.CustomTransformers
{
  /// <summary>
  /// Transforms the <see cref="SqlMethods.DateDiffDay(System.Nullable{System.DateTime},System.Nullable{System.DateTime})"/> method to SQL.
  /// </summary>
  public class DateDiffDayMethodCallTransformer : IMethodCallTransformer
  {
    public static readonly MethodInfo[] SupportedMethods = typeof (SqlMethods).GetMethods ().Where (mi => mi.Name == "DateDiffDay").ToArray ();

    public Expression Transform(MethodCallExpression methodCallExpression)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);

      return new SqlFunctionExpression (
          methodCallExpression.Type, 
          "DATEDIFF", 
          new SqlCustomTextExpression ("day", typeof (string)), 
          methodCallExpression.Arguments[0], 
          methodCallExpression.Arguments[1]);
    }
  }
}