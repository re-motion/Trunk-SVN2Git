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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.ExpressionTreeVisitors
{
  [TestFixture]
  public class ReplacingExpressionTreeVisitorTest
  {
    private ParameterExpression _replacedNode;
    private ParameterExpression _replacementNode;

    [SetUp]
    public void SetUp ()
    {
      _replacedNode = ExpressionHelper.CreateParameterExpression ("replaced node");
      _replacementNode = ExpressionHelper.CreateParameterExpression ("replacement node");
    }

    [Test]
    public void ReplacesGivenNode_ByGivenReplacement ()
    {
      var tree = _replacedNode;

      var result = ReplacingExpressionTreeVisitor.Replace (_replacedNode, _replacementNode, tree);
      Assert.That (result, Is.SameAs (_replacementNode));
    }

    [Test]
    public void IgnoresTree_WhenReplacedNodeDoesNotExist ()
    {
      var tree = ExpressionHelper.CreateLambdaExpression();

      var result = ReplacingExpressionTreeVisitor.Replace (_replacedNode, _replacementNode, tree);
      Assert.That (result, Is.SameAs (tree));
    }

    [Test]
    public void ReplacesTreePart ()
    {
      var tree = Expression.MakeBinary (ExpressionType.Add, Expression.Constant (0), _replacedNode);

      var result = ReplacingExpressionTreeVisitor.Replace (_replacedNode, _replacementNode, tree);

      var expectedResult = Expression.MakeBinary (ExpressionType.Add, Expression.Constant (0), _replacementNode);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void ReplacesTreePart_InSubQueries ()
    {
      var replacedNode = ExpressionHelper.CreateCookQueryable ().Expression;
      var replacementNode = Expression.Constant (null, typeof (Cook[]));

      var subQueryMainFromClause = new MainFromClause ("c", typeof (Cook), replacedNode);
      var subQuery = ExpressionHelper.CreateQueryModel (subQueryMainFromClause);

      var tree = new SubQueryExpression (subQuery);

      ReplacingExpressionTreeVisitor.Replace (replacedNode, replacementNode, tree);

      Assert.That (subQueryMainFromClause.FromExpression, Is.SameAs (replacementNode));
    }

    [Test]
    public void VisitUnknownExpression_Ignored ()
    {
      var expression = new UnknownExpression (typeof (object));
      var result = ReplacingExpressionTreeVisitor.Replace (_replacedNode, _replacementNode, expression);

      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    [Ignore ("TODO 3197")]
    public void ReplacesNode_WithinExtensionNode ()
    {
      var tree = new VBStringComparisonExpression (_replacedNode, true);

      var result = ReplacingExpressionTreeVisitor.Replace (_replacedNode, _replacementNode, tree);

      var expected = new VBStringComparisonExpression (_replacementNode, true);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }
  }
}
