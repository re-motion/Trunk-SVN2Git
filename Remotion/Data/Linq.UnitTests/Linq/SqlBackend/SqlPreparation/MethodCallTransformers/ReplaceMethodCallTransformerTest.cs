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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.MethodCallTransformers
{
  [TestFixture]
  public class ReplaceMethodCallTransformerTest
  {
    [Test]
    public void SupportedMethods ()
    {
      Assert.IsTrue (
          ReplaceMethodCallTransformer.SupportedMethods.Contains (
              typeof (string).GetMethod ("Replace", new [] { typeof (string), typeof (string) })));
    }

    [Test]
    public void Transform ()
    {
      var method = typeof (string).GetMethod ("Replace", new [] { typeof (string), typeof (string) });
      var objectExpression = Expression.Constant ("TAst");
      var expression = Expression.Call (objectExpression, method, Expression.Constant ("A"), Expression.Constant ("B"));
      var transformer = new ReplaceMethodCallTransformer();
      var result = transformer.Transform (expression);

      Assert.That (result, Is.InstanceOfType (typeof (SqlFunctionExpression)));
      Assert.That (result.Type, Is.EqualTo (typeof (string)));
      Assert.That (((SqlFunctionExpression) result).SqlFunctioName, Is.EqualTo ("REPLACE"));
      Assert.That (((SqlFunctionExpression) result).Prefix, Is.EqualTo (objectExpression));
      Assert.That (((SqlFunctionExpression) result).Args.Count, Is.EqualTo (2));
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void Transform_InvalidArgumentCount ()
    {
      var method = typeof (string).GetMethod ("ToUpper", new Type[] { });
      var objectExpression = Expression.Constant ("Test");
      var expression = Expression.Call (objectExpression, method);
      var transformer = new ReplaceMethodCallTransformer();
      transformer.Transform (expression);
    }

  }
}