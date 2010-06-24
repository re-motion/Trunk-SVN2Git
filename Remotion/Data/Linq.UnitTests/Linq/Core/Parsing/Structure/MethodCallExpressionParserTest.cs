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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.Structure.IntermediateModel;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.Structure
{
  [TestFixture]
  public class MethodCallExpressionParserTest
  {
    private MethodCallExpressionNodeTypeRegistry _nodeTypeRegistry;
    private MethodCallExpressionParser _parser;
    private MainSourceExpressionNode _source;

    [SetUp]
    public void SetUp ()
    {
      _nodeTypeRegistry = new MethodCallExpressionNodeTypeRegistry ();

      _nodeTypeRegistry.Register (WhereExpressionNode.SupportedMethods, typeof (WhereExpressionNode));
      _nodeTypeRegistry.Register (SelectExpressionNode.SupportedMethods, typeof (SelectExpressionNode));
      _nodeTypeRegistry.Register (TakeExpressionNode.SupportedMethods, typeof (TakeExpressionNode));
      _nodeTypeRegistry.Register (CountExpressionNode.SupportedMethods, typeof (CountExpressionNode));
      _nodeTypeRegistry.Register (JoinExpressionNode.SupportedMethods, typeof (JoinExpressionNode));

      _parser = new MethodCallExpressionParser (_nodeTypeRegistry);

      _source = ExpressionNodeObjectMother.CreateMainSource();
    }

    [Test]
    public void Parse_WithUnary ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, IQueryable<int>> (q => q.Where (i => i > 5));

      var result = ParseMethodCallExpression (methodCallExpression);

      var whereCondition = (LambdaExpression) ((UnaryExpression) methodCallExpression.Arguments[1]).Operand;
      Assert.That (result, Is.InstanceOfType (typeof (WhereExpressionNode)));
      Assert.That (((WhereExpressionNode) result).AssociatedIdentifier, Is.EqualTo ("x"));
      Assert.That (((WhereExpressionNode) result).Source, Is.SameAs (_source));
      Assert.That (((WhereExpressionNode) result).Predicate, Is.SameAs (whereCondition));
    }

    [Test]
    public void Parse_WithLambda ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IEnumerable<Cook>, IEnumerable<Cook>> (e => e.Select (s => s));

      var result = ParseMethodCallExpression (methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).AssociatedIdentifier, Is.EqualTo ("x"));
      Assert.That (((SelectExpressionNode) result).Source, Is.SameAs (_source));

      var expectedSelector = ExpressionHelper.CreateLambdaExpression<Cook, Cook>(s => s);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSelector, ((SelectExpressionNode) result).Selector);
    }

    [Test]
    public void Parse_DifferentMethod()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, IQueryable<int>> (q => q.Select (i => i + 1));
      var selectProjection = (LambdaExpression) ((UnaryExpression) methodCallExpression.Arguments[1]).Operand;

      var result = ParseMethodCallExpression (methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).Source, Is.SameAs (_source));
      Assert.That (((SelectExpressionNode) result).Selector, Is.SameAs (selectProjection));
    }

    [Test]
    public void Parse_ParsedExpression ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, int> (q => q.Select (i => i + 1).Count());
      var result = (CountExpressionNode) ParseMethodCallExpression (methodCallExpression);

      Assert.That (result.ParsedExpression, Is.SameAs (methodCallExpression));
    }

    [Test]
    public void Parse_WithConstantExpression ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, IQueryable<int>> (
          q => q.Join (ExpressionHelper.CreateCookQueryable(), i => i, s => s.ID, (i, s) => i));

      var result = ParseMethodCallExpression (methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (JoinExpressionNode)));
      Assert.That (((JoinExpressionNode) result).Source, Is.SameAs (_source));
      Assert.That (((JoinExpressionNode) result).InnerSequence, Is.InstanceOfType (typeof (MethodCallExpression)));
    }

    [Test]
    public void Parse_WithConstantExpression_ContainingAnExpression ()
    {
      var selectMethod = ReflectionUtility.GetMethod (() => ((IQueryable<int>) null).Select (i => i));
      var p = Expression.Parameter (typeof (int), "i");
      var methodCallExpression = Expression.Call (
          selectMethod,
          Expression.Parameter (typeof (IQueryable<int>), "e"),
          Expression.Constant (Expression.Lambda<Func<int, int>> (p, p)));

      var result = ParseMethodCallExpression (methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).Source, Is.SameAs (_source));
      Assert.That (((SelectExpressionNode) result).Selector, Is.InstanceOfType (typeof (LambdaExpression)));
    }

    [Test]
    public void Parse_WithNonEvaluatedParameter ()
    {
      var innerSequence = ExpressionHelper.CreateCookQueryable ();
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, IQueryable<int>> (
          q => q.Join (innerSequence, i => i, s => s.ID, (i, s) => i));

      var result = ParseMethodCallExpression(methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (JoinExpressionNode)));
      Assert.That (((JoinExpressionNode) result).Source, Is.SameAs (_source));
      Assert.That (((JoinExpressionNode) result).InnerSequence, Is.InstanceOfType (typeof (MemberExpression)));
    }

    [Test]
    public void Parse_WithSubQuery ()
    {
      var expression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<Cook>, IQueryable<int>> (
          q => q.Select (s => (from c in s.Assistants select s).Count ()));
      var selector = ((UnaryExpression) expression.Arguments[1]).Operand;

      var result = ParseMethodCallExpression (expression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).Selector, Is.Not.SameAs (selector));
      Assert.That (((SelectExpressionNode) result).Selector.Body, Is.InstanceOfType (typeof (SubQueryExpression)));
    }

    [Test]
    public void Parse_WithSubQuery_WithinConstantExpression ()
    {
      Expression<Func<Cook, int>> subQuery = s => (from c in s.Assistants select s).Count ();
      var selectMethod = ReflectionUtility.GetMethod (() => ((IQueryable<Cook>) null).Select (i => 5));
      var methodCallExpression = Expression.Call (
          selectMethod,
          Expression.Parameter (typeof (IQueryable<Cook>), "e"),
          Expression.Constant (subQuery));

      var selector = ((ConstantExpression) methodCallExpression.Arguments[1]).Value;

      var result = ParseMethodCallExpression (methodCallExpression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).Selector, Is.Not.SameAs (selector));
      Assert.That (((SelectExpressionNode) result).Selector.Body, Is.InstanceOfType (typeof (SubQueryExpression)));
    }

    [Test]
    public void Parse_WithSubQuery_UsesNodeTypeRegistry ()
    {
      var emptyNodeTypeRegistry = new MethodCallExpressionNodeTypeRegistry ();
      emptyNodeTypeRegistry.Register (SelectExpressionNode.SupportedMethods, typeof (SelectExpressionNode));
      var parser = new MethodCallExpressionParser (emptyNodeTypeRegistry);

      var expression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<Cook>, IQueryable<int>> (
          q => q.Select (s => s.Assistants.Count ()));

      var result = parser.Parse ("t", _source, expression.Arguments.Skip (1), expression);

      Assert.That (result, Is.InstanceOfType (typeof (SelectExpressionNode)));
      Assert.That (((SelectExpressionNode) result).Selector, Is.Not.TypeOf (typeof (SubQueryExpression)),
          "The given nodeTypeRegistry does not know any query methods, so no SubQueryExpression is generated.");
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Could not parse expression 'q.First()': This overload of the method 'System.Linq.Queryable.First' " 
        + "is currently not supported, but you can register your own parser if needed.")]
    public void Parse_UnknownMethod ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, int> (q => q.First());

      ParseMethodCallExpression (methodCallExpression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Could not parse expression 'q.Select((i, j) => i)': This overload of the method 'System.Linq.Queryable.Select' "
        + "is currently not supported, but you can register your own parser if needed.")]
    public void Parse_UnknownOverload ()
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression<IQueryable<int>, IQueryable<int>> (q => q.Select ((i, j) => i));

      ParseMethodCallExpression (methodCallExpression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage =
        "Could not parse expression 'q.Select(value(System.Func`2[System.Int32,System.Int32]))': Object of type "
        + "'System.Linq.Expressions.ConstantExpression' cannot be converted to type 'System.Linq.Expressions.LambdaExpression'. If you tried to pass "
        + "a delegate instead of a LambdaExpression, this is not supported because delegates are not parsable expressions.")]
    public void Parse_InvalidParameters ()
    {
      Func<int, int> func = i => i;
      var methodCallExpression = (MethodCallExpression) 
          PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees (
              ExpressionHelper.MakeExpression<IQueryable<int>, IEnumerable<int>> (q => q.Select (func)));

      ParseMethodCallExpression (methodCallExpression);
    }

    private IExpressionNode ParseMethodCallExpression (MethodCallExpression methodCallExpression)
    {
      return _parser.Parse ("x", _source, methodCallExpression.Arguments.Skip (1), methodCallExpression);
    }
  }
}
