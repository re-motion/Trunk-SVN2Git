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
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Expressions
{
  [TestFixture]
  public class VirtualMethodAddressExpressionTest
  {
    private Expression _instance;
    private MethodInfo _method;

    private VirtualMethodAddressExpression _expression;

    [SetUp]
    public void SetUp ()
    {
      _method = ReflectionObjectMother.GetSomeVirtualMethod();
      _instance = ExpressionTreeObjectMother.GetSomeExpression (_method.DeclaringType);

      _expression = new VirtualMethodAddressExpression (_instance, _method);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_expression.Type, Is.SameAs (typeof (IntPtr)));
      Assert.That (_expression.Instance, Is.SameAs (_instance));
      Assert.That (_expression.Method, Is.SameAs (_method));
    }

    [Test]
    public void Accept ()
    {
      ExpressionTestHelper.CheckAccept (_expression, mock => mock.VisitVirtualMethodAddress (_expression));
    }

    [Test]
    public void VisitVirtualMethodAddress_NoChanges ()
    {
      ExpressionTestHelper.CheckVisitChildren_NoChanges (_expression, _expression.Instance);
    }

    [Test]
    public void VisitVirtualMethodAddress_WithChanges ()
    {
      var newInnerExpression = ExpressionTreeObjectMother.GetSomeExpression (_method.DeclaringType);

      var expressionVisitorMock = MockRepository.GenerateStrictMock<ExpressionVisitor> ();
      expressionVisitorMock.Expect (mock => mock.Visit (_expression.Instance)).Return (newInnerExpression);

      var result = ExpressionTestHelper.CallVisitChildren (_expression, expressionVisitorMock);

      expressionVisitorMock.VerifyAllExpectations ();

      Assert.That (result, Is.Not.SameAs (_expression));
      Assert.That (result.Type, Is.SameAs (_expression.Type));
      Assert.That (result, Is.TypeOf<VirtualMethodAddressExpression> ());

      var virtualMethodAddressExpression = (VirtualMethodAddressExpression) result;
      Assert.That (virtualMethodAddressExpression.Method, Is.SameAs (_expression.Method));
      Assert.That (virtualMethodAddressExpression.Instance, Is.SameAs (newInnerExpression));
    }
  }
}