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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;
using System.Linq;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  [TestFixture]
  public class StringLengthMethodCallTransformerTest
  {
    [Test]
    public void SupportedMethods ()
    {
      Assert.That(
          StringLengthMethodCallTransformer.SupportedMethods.Contains (
              MethodCallTransformerUtility.GetInstanceMethod (typeof (string), "get_Length")));
    }

    [Test]
    public void Transform ()
    {
      var method = typeof (string).GetMethod ("get_Length", new Type[] { });
      var objectExpression = Expression.Constant ("Test");
      var expression = Expression.Call (objectExpression, method);
      var transformer = new StringLengthMethodCallTransformer ();

      var result = transformer.Transform (expression);

      var expectedResult = new SqlFunctionExpression (typeof (int), "LEN", objectExpression);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }
  }
}