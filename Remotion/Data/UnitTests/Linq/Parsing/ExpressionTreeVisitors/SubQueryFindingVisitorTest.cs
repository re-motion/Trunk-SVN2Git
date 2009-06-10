// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Expressions;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.Linq.Parsing.ExpressionTreeVisitors
{
  [TestFixture]
  public class SubQueryFindingVisitorTest
  {
    private List<QueryModel> _subQueryRegistry;
    private MethodCallExpressionNodeTypeRegistry _nodeTypeRegistry;

    [SetUp]
    public void SetUp ()
    {
      _nodeTypeRegistry = MethodCallExpressionNodeTypeRegistry.CreateDefault();
      _subQueryRegistry = new List<QueryModel> ();
    }

    [Test]
    public void TreeWithNoSubquery ()
    {
      Expression expression = Expression.Constant ("test");

      Expression newExpression = SubQueryFindingVisitor.ReplaceSubQueries (expression, _nodeTypeRegistry, _subQueryRegistry);
      Assert.That (newExpression, Is.SameAs (expression));
    }

    [Test]
    public void TreeWithSubquery ()
    {
      Expression subQuery = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQuerySource ()).Expression;
      Expression surroundingExpression = Expression.Lambda (subQuery);

      Expression newExpression = SubQueryFindingVisitor.ReplaceSubQueries (surroundingExpression, _nodeTypeRegistry, _subQueryRegistry);

      Assert.That (newExpression, Is.Not.SameAs (surroundingExpression));
      Assert.That (newExpression, Is.InstanceOfType (typeof (LambdaExpression)));

      var newLambdaExpression = (LambdaExpression) newExpression;
      Assert.That (newLambdaExpression.Body, Is.InstanceOfType (typeof (SubQueryExpression)));

      var newSubQueryExpression = (SubQueryExpression) newLambdaExpression.Body;
      Assert.That (newSubQueryExpression.QueryModel.GetExpressionTree (), Is.SameAs (subQuery));
    }

    [Test]
    public void SubqueryIsRegistered ()
    {
      Assert.That (_subQueryRegistry, Is.Empty);

      Expression subQuery = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQuerySource ()).Expression;
      Expression surroundingExpression = Expression.Lambda (subQuery);

      var newLambdaExpression =
          (LambdaExpression) SubQueryFindingVisitor.ReplaceSubQueries (surroundingExpression, _nodeTypeRegistry, _subQueryRegistry);
      var newSubQueryExpression = (SubQueryExpression) newLambdaExpression.Body;
      Assert.That (_subQueryRegistry, Is.EquivalentTo (new[] { newSubQueryExpression.QueryModel }));
    }

    [Test]
    public void VisitorUsesNodeTypeRegistry_ToParseAndAnalyzeSubQueries ()
    {
      Expression subQuery = ExpressionHelper.MakeExpression (() => CustomSelect (ExpressionHelper.CreateQuerySource (), s => s));
      Expression surroundingExpression = Expression.Lambda (subQuery);

      var emptyNodeTypeRegistry = new MethodCallExpressionNodeTypeRegistry ();
      emptyNodeTypeRegistry.Register (new[] { ((MethodCallExpression) subQuery).Method }, typeof (SelectExpressionNode));

      var newLambdaExpression =
          (LambdaExpression) SubQueryFindingVisitor.ReplaceSubQueries (surroundingExpression, emptyNodeTypeRegistry, _subQueryRegistry);
      Assert.That (newLambdaExpression.Body, Is.InstanceOfType (typeof (SubQueryExpression)));
    }

    public static IQueryable<Student> CustomSelect (IQueryable<Student> source, Expression<Func<Student, Student>> selector)
    {
      throw new NotImplementedException ();
    }
  }
}