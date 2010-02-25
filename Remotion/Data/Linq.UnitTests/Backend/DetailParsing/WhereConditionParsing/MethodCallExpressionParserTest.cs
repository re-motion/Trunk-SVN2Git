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
using NUnit.Framework;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Backend.DetailParsing.WhereConditionParsing
{
  [TestFixture]
  public class MethodCallExpressionParserTest : DetailParserTestBase
  {
    private IColumnSource _fromSource;
    private MainFromClause _fromClause;
    private WhereConditionParserRegistry _parserRegistry;
    private FieldResolver _resolver;
    private MemberExpression _memberExpression;

    public override void SetUp ()
    {
      base.SetUp();

      _fromClause = ExpressionHelper.CreateMainFromClause_Student ();
      _fromSource = ParseContext.JoinedTableContext.GetColumnSource (_fromClause);
      QueryModel = ExpressionHelper.CreateQueryModel (_fromClause);
      _resolver = new FieldResolver (StubDatabaseInfo.Instance, new WhereFieldAccessPolicy (StubDatabaseInfo.Instance));
      _parserRegistry = new WhereConditionParserRegistry (StubDatabaseInfo.Instance);
      _parserRegistry.RegisterParser (typeof (BinaryExpression), new BinaryExpressionParser (_parserRegistry));
      _parserRegistry.RegisterParser (typeof (MemberExpression), new MemberExpressionParser (_resolver));
      _parserRegistry.RegisterParser (typeof (ConstantExpression), new ConstantExpressionParser (StubDatabaseInfo.Instance));

      _memberExpression = Expression.MakeMemberAccess (new QuerySourceReferenceExpression (_fromClause), typeof (Cook).GetProperty ("FirstName"));
    }

    [Test]
    public void CreateMethodCall_ToUpper_NoArguments ()
    {
      MethodInfo methodInfo = typeof (string).GetMethod ("ToUpper", new Type[] { });
      MethodCallExpression methodCallExpression = Expression.Call (_memberExpression, methodInfo);

      //expected Result
      var column = new Column (_fromSource, "FirstNameColumn");
      var expected = new MethodCall (methodInfo, column, new List<IEvaluation>());

      var methodCallExpressionParser = new MethodCallExpressionParser (_parserRegistry);

      //result
      ICriterion result = methodCallExpressionParser.Parse (methodCallExpression, ParseContext);

      //asserts
      Assert.IsEmpty (((MethodCall) result).Arguments);
      Assert.AreEqual (expected.EvaluationMethodInfo, ((MethodCall) result).EvaluationMethodInfo);
      Assert.AreEqual (expected.TargetObject, ((MethodCall) result).TargetObject);
    }

    [Test]
    public void CreateMethodCall_Remove_WithArguments ()
    {
      MethodInfo methodInfo = typeof (string).GetMethod ("Remove", new[] { typeof (int) });
      MethodCallExpression methodCallExpression = Expression.Call (_memberExpression, methodInfo, Expression.Constant (5));

      //expected result
      var column = new Column (_fromSource, "FirstNameColumn");
      var item = new Constant (5);
      var arguments = new List<IEvaluation> { item };
      var expected = new MethodCall (methodInfo, column, arguments);

      var methodCallExpressionParser =
          new MethodCallExpressionParser (_parserRegistry);

      //result
      ICriterion result = methodCallExpressionParser.Parse (methodCallExpression, ParseContext);

      //asserts
      Assert.IsNotEmpty (((MethodCall) result).Arguments);
      Assert.AreEqual (((MethodCall) result).Arguments, expected.Arguments);
      Assert.AreEqual (expected.EvaluationMethodInfo, ((MethodCall) result).EvaluationMethodInfo);
      Assert.AreEqual (expected.TargetObject, ((MethodCall) result).TargetObject);
    }
  }
}
