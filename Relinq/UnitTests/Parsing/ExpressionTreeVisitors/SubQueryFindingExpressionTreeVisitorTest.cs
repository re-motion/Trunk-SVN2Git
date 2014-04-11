// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using Remotion.Linq.UnitTests.Parsing.Structure.TestDomain;
using Remotion.Linq.UnitTests.TestDomain;
using Remotion.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Linq.UnitTests.Parsing.ExpressionTreeVisitors
{
  [TestFixture]
  public class SubQueryFindingExpressionTreeVisitorTest
  {
    private MethodInfoBasedNodeTypeRegistry _methodInfoBasedNodeTypeRegistry;

    [SetUp]
    public void SetUp ()
    {
      _methodInfoBasedNodeTypeRegistry = MethodInfoBasedNodeTypeRegistry.CreateFromTypes (typeof(SelectExpressionNode).Assembly.GetTypes());
    }

    [Test]
    public void Initialization_InnerParserHasNoTransformations ()
    {
      var visitorInstance = Activator.CreateInstance (
          typeof (SubQueryFindingExpressionTreeVisitor), BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { _methodInfoBasedNodeTypeRegistry }, null);

      var innerParser = (QueryParser) PrivateInvoke.GetNonPublicField (visitorInstance, "_queryParser");
      Assert.That (innerParser.ExpressionTreeParser.Processor, Is.TypeOf (typeof (NullExpressionTreeProcessor)));
    }

    [Test]
    public void TreeWithNoSubquery ()
    {
      Expression expression = Expression.Constant ("test");

      Expression newExpression = SubQueryFindingExpressionTreeVisitor.Process (expression, _methodInfoBasedNodeTypeRegistry);
      Assert.That (newExpression, Is.SameAs (expression));
    }

    [Test]
    public void TreeWithSubquery ()
    {
      Expression subQuery = SelectTestQueryGenerator.CreateSimpleQuery (ExpressionHelper.CreateQueryable<Cook>()).Expression;
      Expression surroundingExpression = Expression.Lambda (subQuery);

      Expression newExpression = SubQueryFindingExpressionTreeVisitor.Process (surroundingExpression, _methodInfoBasedNodeTypeRegistry);

      Assert.That (newExpression, Is.Not.SameAs (surroundingExpression));
      Assert.That (newExpression, Is.InstanceOf (typeof (LambdaExpression)));

      var newLambdaExpression = (LambdaExpression) newExpression;
      Assert.That (newLambdaExpression.Body, Is.InstanceOf (typeof (SubQueryExpression)));

      var newSubQueryExpression = (SubQueryExpression) newLambdaExpression.Body;
      Assert.That (
          ((QuerySourceReferenceExpression) newSubQueryExpression.QueryModel.SelectClause.Selector).ReferencedQuerySource,
          Is.SameAs (newSubQueryExpression.QueryModel.MainFromClause));
    }

    [Test]
    public void VisitorUsesNodeTypeRegistry_ToParseAndAnalyzeSubQueries ()
    {
      Expression subQuery = ExpressionHelper.MakeExpression (() => CustomSelect (ExpressionHelper.CreateQueryable<Cook>(), s => s));
      Expression surroundingExpression = Expression.Lambda (subQuery);
      // evaluate the ExpressionHelper.CreateQueryable<Cook> () method
      var inputExpression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees (surroundingExpression);

      var emptyNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();
      emptyNodeTypeRegistry.Register (new[] { ((MethodCallExpression) subQuery).Method }, typeof (SelectExpressionNode));

      var newLambdaExpression =
          (LambdaExpression) SubQueryFindingExpressionTreeVisitor.Process (inputExpression, emptyNodeTypeRegistry);
      Assert.That (newLambdaExpression.Body, Is.InstanceOf (typeof (SubQueryExpression)));
    }

    [Test]
    public void VisitorUsesExpressionTreeVisitor_ToGetPotentialQueryOperator ()
    {
      _methodInfoBasedNodeTypeRegistry.Register (new[] { typeof (QueryableFakeWithCount<>).GetMethod ("get_Count") }, typeof (CountExpressionNode));
      Expression subQuery = ExpressionHelper.MakeExpression (() => new QueryableFakeWithCount<int>().Count);
      Expression surroundingExpression = Expression.Lambda (subQuery);

      var newLambdaExpression =
          (LambdaExpression) SubQueryFindingExpressionTreeVisitor.Process (surroundingExpression, _methodInfoBasedNodeTypeRegistry);
      Assert.That (newLambdaExpression.Body, Is.InstanceOf (typeof (SubQueryExpression)));
    }

    [Test]
    public void VisitUnknownNonExtensionExpression_Ignored ()
    {
      var expression = new UnknownExpression (typeof (object));
      var result = SubQueryFindingExpressionTreeVisitor.Process (expression, _methodInfoBasedNodeTypeRegistry);

      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitExtensionExpression_ChildrenAreEvaluated ()
    {
      var subQuery = ExpressionHelper.MakeExpression (() => (from s in ExpressionHelper.CreateQueryable<Cook> () select s).Any());
      var extensionExpression = new VBStringComparisonExpression (subQuery, true);
      // evaluate the ExpressionHelper.CreateQueryable<Cook> () method
      var inputExpression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees (extensionExpression);

      var result = SubQueryFindingExpressionTreeVisitor.Process (inputExpression, _methodInfoBasedNodeTypeRegistry);

      Assert.That (((VBStringComparisonExpression) result).Comparison, Is.TypeOf (typeof (SubQueryExpression)));
    }

    public static IQueryable<Cook> CustomSelect (IQueryable<Cook> source, Expression<Func<Cook, Cook>> selector)
    {
      throw new NotImplementedException();
    }
  }
}