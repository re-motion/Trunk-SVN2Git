// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.Parsing.Details.SelectProjectionParsing;
using Remotion.Data.Linq.Parsing.FieldResolving;

namespace Remotion.Data.UnitTests.Linq.ParsingTest.DetailsTest.SelectProjectionParsingTest
{
  [TestFixture]
  public class MethodCallExpressionParserTest : DetailParserTestBase
  {
    private ParameterExpression _parameter;
    private IColumnSource _fromSource;
    private MainFromClause _fromClause;
    private SelectProjectionParserRegistry _parserRegistry;
    private ClauseFieldResolver _resolver;

    public override void SetUp()
    {
      base.SetUp();

      _parameter = Expression.Parameter (typeof (Student), "s");
      _fromClause = ExpressionHelper.CreateMainFromClause (_parameter, ExpressionHelper.CreateQuerySource ());
      _fromSource = _fromClause.GetFromSource (StubDatabaseInfo.Instance);
      QueryModel = ExpressionHelper.CreateQueryModel (_fromClause);
      _resolver = new ClauseFieldResolver (StubDatabaseInfo.Instance, new SelectFieldAccessPolicy());
      _parserRegistry = 
        new SelectProjectionParserRegistry (StubDatabaseInfo.Instance, new ParseMode());
      _parserRegistry.RegisterParser (typeof(ConstantExpression), new ConstantExpressionParser (StubDatabaseInfo.Instance));
      _parserRegistry.RegisterParser (typeof(ParameterExpression), new ParameterExpressionParser (_resolver));
      _parserRegistry.RegisterParser (typeof(MemberExpression), new MemberExpressionParser (_resolver));
      _parserRegistry.RegisterParser (typeof(MethodCallExpression), new MethodCallExpressionParser (_parserRegistry));
    }


    [Test]
    public void CreateMethodCallEvaluation ()
    {
      MemberExpression memberExpression = Expression.MakeMemberAccess (_parameter, typeof (Student).GetProperty ("First"));
      MethodInfo methodInfo = typeof (string).GetMethod ("ToUpper", new Type[] {  });      
      MethodCallExpression methodCallExpression = Expression.Call (memberExpression, methodInfo);

      //expected Result
      Column column = new Column (_fromSource, "FirstColumn");
      List<IEvaluation> c1 = new List<IEvaluation> { column };
      MethodCall expected = new MethodCall(methodInfo,column,new List<IEvaluation>());
      
      MethodCallExpressionParser methodCallExpressionParser = new MethodCallExpressionParser (_parserRegistry);

      //result
      IEvaluation result = methodCallExpressionParser.Parse (methodCallExpression, ParseContext);

      Assert.IsEmpty (((MethodCall)result).EvaluationArguments);
      Assert.AreEqual (expected.EvaluationMethodInfo, ((MethodCall) result).EvaluationMethodInfo);
      Assert.AreEqual (expected.EvaluationObject, ((MethodCall) result).EvaluationObject);
    }

    [Test]
    public void CreateMethodCall_WithOneArgument ()
    {
      MemberExpression memberExpression = Expression.MakeMemberAccess (_parameter, typeof (Student).GetProperty ("First"));

      MethodInfo methodInfo = typeof (string).GetMethod ("Remove", new Type[] { typeof(int) });
      MethodCallExpression methodCallExpression = Expression.Call (memberExpression, methodInfo, Expression.Constant (5));

      //expected Result
      Column column = new Column (_fromSource, "FirstColumn");
      List<IEvaluation> c1 = new List<IEvaluation> { column };
      Constant item = new Constant(5);
      List<IEvaluation> item1 = new List<IEvaluation> { item };
      List<IEvaluation> arguments = new List<IEvaluation> { item};
      MethodCall expected = new MethodCall (methodInfo, column, arguments);

      MethodCallExpressionParser methodCallExpressionParser =
        new MethodCallExpressionParser ( _parserRegistry);

      //result
      IEvaluation result = methodCallExpressionParser.Parse (methodCallExpression, ParseContext);

      
      Assert.AreEqual (((MethodCall)result).EvaluationArguments, expected.EvaluationArguments);
      Assert.AreEqual (expected.EvaluationMethodInfo, ((MethodCall) result).EvaluationMethodInfo);
      Assert.AreEqual (expected.EvaluationObject, ((MethodCall) result).EvaluationObject);
    }

    [Test]
    public void CreateMethodCall_WithStaticMethod ()
    {
      MethodInfo methodInfo = typeof (DateTime).GetMethod ("get_Now");
      MethodCallExpression methodCallExpression = Expression.Call (methodInfo);

      MethodCallExpressionParser methodCallExpressionParser = new MethodCallExpressionParser (_parserRegistry);
      IEvaluation result = methodCallExpressionParser.Parse (methodCallExpression, ParseContext);

      Assert.That (((MethodCall) result).EvaluationArguments, Is.Empty);
      Assert.That (((MethodCall) result).EvaluationMethodInfo, Is.EqualTo (methodInfo));
      Assert.That (((MethodCall) result).EvaluationObject, Is.Null);
    }
  }
}
