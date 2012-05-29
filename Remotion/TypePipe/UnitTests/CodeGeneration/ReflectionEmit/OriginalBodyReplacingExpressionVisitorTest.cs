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
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class OriginalBodyReplacingExpressionVisitorTest
  {
    private OriginalBodyReplacingExpressionVisitor _visitorPartialMock;

    [SetUp]
    public void SetUp ()
    {
      _visitorPartialMock = MockRepository.GeneratePartialMock<OriginalBodyReplacingExpressionVisitor>();
    }

    [Test]
    public void VisitOriginalBody_Method ()
    {
      var methodBase = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method (7, "string"));
      var arguments = new ArgumentTestHelper (7, "string").Expressions;
      var expression = new OriginalBodyExpression (methodBase, typeof (double), arguments);
      Action<MethodInfo> checkMethodInCallExpressionAction =
          methodInfo => Assert.That (methodInfo, Is.TypeOf<BaseCallMethodInfoAdapter>().And.Property ("AdaptedMethodInfo").SameAs (methodBase));

      CheckVisitOriginalBodyForInstanceMethod (expression, arguments, checkMethodInCallExpressionAction);
    }

    [Test]
    public void VisitOriginalBody_StaticMethod ()
    {
      var methodBase = MemberInfoFromExpressionUtility.GetMethod (() => DomainClass.StaticMethod (7, "string"));
      var arguments = new ArgumentTestHelper (7, "string").Expressions;
      var expression = new OriginalBodyExpression (methodBase, typeof (double), arguments);
      Action<MethodCallExpression> checkMethodCallExpressionAction = methodCallExpression =>
      {
        Assert.That (methodCallExpression.Object, Is.Null);
        Assert.That (methodCallExpression.Method, Is.TypeOf<BaseCallMethodInfoAdapter>().And.Property ("AdaptedMethodInfo").SameAs (methodBase));
      };

      CheckVisitOriginalBody (expression, arguments, checkMethodCallExpressionAction);
    }

    [Test]
    public void VisitOriginalBody_Constructor ()
    {
      var methodBase = MemberInfoFromExpressionUtility.GetConstructor (() => new DomainClass());
      var arguments = new Expression[0];
      var expression = new OriginalBodyExpression (methodBase, typeof (void), arguments);
      Action<MethodInfo> checkMethodInCallExpressionAction = methodInfo =>
      {
        Assert.That (methodInfo, Is.TypeOf<BaseCallMethodInfoAdapter> ());
        var baseCallMethodInfoAdapter = (BaseCallMethodInfoAdapter) methodInfo;
        Assert.That (baseCallMethodInfoAdapter.AdaptedMethodInfo, Is.TypeOf<ConstructorAsMethodInfoAdapter>());
        var constructorAsMethodInfoAdapter = (ConstructorAsMethodInfoAdapter) baseCallMethodInfoAdapter.AdaptedMethodInfo;
        Assert.That (constructorAsMethodInfoAdapter.ConstructorInfo, Is.SameAs (methodBase));
      };

      CheckVisitOriginalBodyForInstanceMethod (expression,arguments, checkMethodInCallExpressionAction);
    }

    [Test]
    public void VisitOriginalBody_StaticConstructor ()
    {
      var methodBase = typeof (DomainClass).GetConstructors (BindingFlags.NonPublic | BindingFlags.Static).Single();
      var arguments = new Expression[0];
      var expression = new OriginalBodyExpression (methodBase, typeof (void), arguments);
      Action<MethodCallExpression> checkMethodInCallExpressionAction = methodCallExpression =>
      {
        Assert.That (methodCallExpression.Object, Is.Null);

        Assert.That (methodCallExpression.Method, Is.TypeOf<BaseCallMethodInfoAdapter> ());
        var baseCallMethodInfoAdapter = (BaseCallMethodInfoAdapter) methodCallExpression.Method;
        Assert.That (baseCallMethodInfoAdapter.AdaptedMethodInfo, Is.TypeOf<ConstructorAsMethodInfoAdapter> ());
        var constructorAsMethodInfoAdapter = (ConstructorAsMethodInfoAdapter) baseCallMethodInfoAdapter.AdaptedMethodInfo;
        Assert.That (constructorAsMethodInfoAdapter.ConstructorInfo, Is.SameAs (methodBase));
      };

      CheckVisitOriginalBody (expression, arguments, checkMethodInCallExpressionAction);
    }

    private void CheckVisitOriginalBodyForInstanceMethod (
        OriginalBodyExpression expression, Expression[] expectedMethodCallArguments, Action<MethodInfo> checkMethodInCallExpressionAction)
    {
      CheckVisitOriginalBody (expression, expectedMethodCallArguments, methodCallExpression =>
      {
        Assert.That (methodCallExpression.Object, Is.TypeOf<TypeAsUnderlyingSystemTypeExpression> ());
        var typeAsUnderlyingSystemTypeExpression = ((TypeAsUnderlyingSystemTypeExpression) methodCallExpression.Object);
        Assert.That (
            typeAsUnderlyingSystemTypeExpression.InnerExpression, Is.TypeOf<ThisExpression>().With.Property ("Type").SameAs (typeof (DomainClass)));

        checkMethodInCallExpressionAction (methodCallExpression.Method);
      });
    }

    private void CheckVisitOriginalBody (
        OriginalBodyExpression expression, Expression[] expectedMethodCallArguments, Action<MethodCallExpression> checkMethodCallExpressionAction)
    {
      var fakeResult = ExpressionTreeObjectMother.GetSomeExpression();
      _visitorPartialMock
          .Expect (mock => mock.Visit (Arg<Expression>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (
              mi =>
              {
                Assert.That (mi.Arguments[0], Is.InstanceOf<MethodCallExpression>());
                var methodCallExpression = (MethodCallExpression) mi.Arguments[0];

                checkMethodCallExpressionAction (methodCallExpression);
                Assert.That (methodCallExpression.Arguments, Is.EqualTo (expectedMethodCallArguments));
              });

      var result = TypePipeExpressionVisitorTestHelper.CallVisitOriginalBody (_visitorPartialMock, expression);

      _visitorPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    public class DomainClass
    {
// ReSharper disable EmptyConstructor
      static DomainClass () { }
// ReSharper restore EmptyConstructor

      public static double StaticMethod (int p1, string p2)
      {
        Dev.Null = p1;
        Dev.Null = p2;
        return 7.7;
      }

      public double Method (int p1, string p2)
      {
        Dev.Null = p1;
        Dev.Null = p2;
        return 7.7;
      }
    }
  }
}