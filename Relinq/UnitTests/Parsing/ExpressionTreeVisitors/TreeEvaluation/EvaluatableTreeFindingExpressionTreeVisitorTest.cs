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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.Development.UnitTesting.Clauses.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.TreeEvaluation;
using Remotion.Linq.UnitTests.Parsing.Structure.TestDomain;
using Remotion.Linq.UnitTests.TestDomain;

namespace Remotion.Linq.UnitTests.Parsing.ExpressionTreeVisitors.TreeEvaluation
{
  [TestFixture]
  public class EvaluatableTreeFindingExpressionTreeVisitorTest
  {
    [Test]
    public void SimpleExpression_IsEvaluatable ()
    {
      var expression = Expression.Constant (0);
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.True);
    }

    [Test]
    public void NestedExpression_InnerAndOuterAreEvaluatable ()
    {
      var innerExpressionLeft = Expression.Constant (0);
      var innerExpressionRight = Expression.Constant (0);
      var outerExpression = Expression.MakeBinary (ExpressionType.Add, innerExpressionLeft, innerExpressionRight);
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (outerExpression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (outerExpression), Is.True);
      Assert.That (evaluationInfo.IsEvaluatableExpression (innerExpressionLeft), Is.True);
      Assert.That (evaluationInfo.IsEvaluatableExpression (innerExpressionRight), Is.True);
    }

    [Test]
    public void ParameterExpression_IsNotEvaluatable ()
    {
      var expression = ExpressionHelper.CreateParameterExpression ();
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void ExpressionContainingParameterExpression_IsNotEvaluatable ()
    {
      var expression = Expression.MakeBinary (
          ExpressionType.Equal, 
          ExpressionHelper.CreateParameterExpression (), 
          ExpressionHelper.CreateParameterExpression ());
      
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void ParameterExpression_SiblingCanBeEvaluatable ()
    {
      var expression = Expression.MakeBinary (
          ExpressionType.Equal,
          ExpressionHelper.CreateParameterExpression (),
          Expression.Constant (0));

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression.Right), Is.True);
    }

    [Test]
    public void VisitUnknownExpression_NotEvaluatable ()
    {
      var expression = new SubQueryExpression (ExpressionHelper.CreateQueryModel<Cook>());
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void VisitExtensionExpression_NotEvaluatable_ButChildrenMayBe ()
    {
      var innerExpression = Expression.MakeBinary (ExpressionType.Equal, Expression.Constant (0), Expression.Constant (0));
      var extensionExpression = new TestExtensionExpression (innerExpression);
      
      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (extensionExpression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (extensionExpression), Is.False);
      Assert.That (evaluationInfo.IsEvaluatableExpression (innerExpression), Is.True);
    }

    [Test]
    public void NullExpression_InOtherExpression_IsIgnored ()
    {
      var expression = Expression.MakeBinary (
          ExpressionType.Equal,
          ExpressionHelper.CreateParameterExpression (),
          ExpressionHelper.CreateParameterExpression ());

      Assert.That (expression.Conversion, Is.Null);

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.Count, Is.EqualTo (0));
    }

    [Test]
    public void MethodCall_WithIQueryableObject_IsNotEvaluatable ()
    {
      var source = ExpressionHelper.CreateQueryable<Cook> ();
      var expression = ExpressionHelper.MakeExpression (() => source.ToString());

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void MethodCall_WithIQueryableParameter_IsNotEvaluatable ()
    {
      var source = ExpressionHelper.CreateQueryable<Cook> ();
      var expression = ExpressionHelper.MakeExpression (() => source.Count ());

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void MemberExpression_WithIQueryableObject_IsNotEvaluatable ()
    {
      var source = new QueryableFakeWithCount<int>();
      var expression = ExpressionHelper.MakeExpression (() => source.Count);

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void MemberInitialization_WithParametersInMemberAssignments_IsNotEvaluatable ()
    {
      var expression = (MemberInitExpression) ExpressionHelper.MakeExpression<int, AnonymousType> (i => new AnonymousType { a = i, b = 1 });

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression.NewExpression), Is.False);
    }

    [Test]
    public void ListInitialization_WithParametersInMemberAssignments_IsNotEvaluatable ()
    {
      var expression = (ListInitExpression) ExpressionHelper.MakeExpression<int, List<int>> (i => new List<int> { i, 1 });

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.False);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression.NewExpression), Is.False);
    }

    [Test]
    public void MemberInitialization_WithoutParametersInMemberAssignments_IsEvaluatable ()
    {
      var expression = (MemberInitExpression) ExpressionHelper.MakeExpression<int, AnonymousType> (i => new AnonymousType { a = 1, b = 1 });

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.True);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression.NewExpression), Is.True);
    }

    [Test]
    public void ListInitialization_WithoutParametersInMemberAssignments_IsEvaluatable ()
    {
      var expression = (ListInitExpression) ExpressionHelper.MakeExpression<int, List<int>> (i => new List<int> { 2, 1 });

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression), Is.True);
      Assert.That (evaluationInfo.IsEvaluatableExpression (expression.NewExpression), Is.True);
    }

    [Test]
    public void VisitUnknownExpression_Ignored ()
    {
      var expression = new UnknownExpression (typeof (object));
      var result = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (expression);

      Assert.That (result.IsEvaluatableExpression (expression), Is.False);
    }

    [Test]
    public void VisitDynamicExpression_WithParameterReference_NonEvaluable ()
    {
      var parameterExpression = Expression.Parameter (typeof (object), "x");

      var dynamicExpressionWithParameterReference =
          Expression.Dynamic (
              Binder.GetMember (
                  CSharpBinderFlags.InvokeSimpleName,
                  "colour",
                  typeof (object),
                  new[] { CSharpArgumentInfo.Create (CSharpArgumentInfoFlags.None, null) }),
              typeof (object),
              parameterExpression);

      var body = Expression.MakeBinary (ExpressionType.Equal, dynamicExpressionWithParameterReference, Expression.Constant ("orange"));
      
      var result = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (body);

      Assert.That (result.IsEvaluatableExpression (body), Is.False);
    }

    [Test]
    public void PartialEvaluationExceptionExpression_NotEvaluable_AndChildrenNeither ()
    {
      var inner = Expression.Constant (0);
      var partialEvaluationExceptionExpression = new PartialEvaluationExceptionExpression (new Exception(), inner);

      var evaluationInfo = EvaluatableTreeFindingExpressionTreeVisitor.Analyze (partialEvaluationExceptionExpression);

      Assert.That (evaluationInfo.IsEvaluatableExpression (partialEvaluationExceptionExpression), Is.False);
      Assert.That (evaluationInfo.IsEvaluatableExpression (inner), Is.False);
    }
  }
}
