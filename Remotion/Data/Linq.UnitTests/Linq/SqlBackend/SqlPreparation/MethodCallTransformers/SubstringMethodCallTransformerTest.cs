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
using NUnit.Framework;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;
using System.Linq;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  [TestFixture]
  public class SubstringMethodCallTransformerTest
  {
    [Test]
    public void SupportedMethods ()
    {
      Assert.IsTrue (SubstringMethodCallTransformer.SupportedMethods.Contains (typeof (string).GetMethod ("Substring", new[] { typeof (int) })));
      Assert.IsTrue (SubstringMethodCallTransformer.SupportedMethods.Contains (typeof (string).GetMethod ("Substring", new[] { typeof (int), typeof (int) })));
    }

    [Test]
    public void Transform_WithOneArgument ()
    {
      var method = typeof (string).GetMethod ("Substring", new Type[] { typeof (int) });
      var objectExpression = Expression.Constant ("Test");
      var expression = Expression.Call (objectExpression, method, Expression.Constant (1));

      var transformer = new SubstringMethodCallTransformer ();
      var result = transformer.Transform (expression);

      var fakeResult = new SqlFunctionExpression (
          expression.Type,
          "SUBSTRING",
          objectExpression,
          Expression.Constant (1),
          new SqlFunctionExpression (objectExpression.Type, "LEN", objectExpression)
          );

      ExpressionTreeComparer.CheckAreEqualTrees (result, fakeResult);
    }

    [Test]
    public void Transform_WithTwoArgument ()
    {
      var method = typeof (string).GetMethod ("Substring", new Type[] { typeof (int), typeof (int) });
      var objectExpression = Expression.Constant ("Test");
      var expression = Expression.Call (objectExpression, method, Expression.Constant (1), Expression.Constant (3));

      var transformer = new SubstringMethodCallTransformer ();
      var result = transformer.Transform (expression);

      var fakeResult = new SqlFunctionExpression (
          expression.Type,
          "SUBSTRING",
          objectExpression,
          Expression.Constant (1),
          Expression.Constant (3)
          );

      ExpressionTreeComparer.CheckAreEqualTrees (result, fakeResult);
    }
  }
}