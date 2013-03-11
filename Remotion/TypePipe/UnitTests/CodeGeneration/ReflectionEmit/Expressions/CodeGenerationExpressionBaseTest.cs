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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit.Expressions
{
  [TestFixture]
  public class CodeGenerationExpressionBaseTest
  {
    private Type _type;

    private CodeGenerationExpressionBase _expressionPartialMock;

    [SetUp]
    public void SetUp ()
    {
      _type = ReflectionObjectMother.GetSomeType();

      _expressionPartialMock = MockRepository.GeneratePartialMock<CodeGenerationExpressionBase> (_type);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_expressionPartialMock.Type, Is.SameAs (_type));
    }

    [Test]
    public void Accept_StandardExpressionVisitor ()
    {
      var expressionVisitorMock = MockRepository.GenerateStrictMock<ExpressionVisitor>();
      var expectedResult = ExpressionTreeObjectMother.GetSomeExpression();
      expressionVisitorMock.Expect (mock => mock.Invoke ("VisitExtension", _expressionPartialMock)).Return (expectedResult);

      var result = _expressionPartialMock.Invoke ("Accept", expressionVisitorMock);

      _expressionPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    public void Accept_TypePipeExpressionVisitor ()
    {
      var expressionVisitorMock = MockRepository.GenerateStrictMock<ExpressionVisitor, ICodeGenerationExpressionVisitor>();
      var expectedResult = ExpressionTreeObjectMother.GetSomeExpression();
      _expressionPartialMock.Expect (mock => mock.Accept ((ICodeGenerationExpressionVisitor) expressionVisitorMock)).Return (expectedResult);

      var result = _expressionPartialMock.Invoke ("Accept", expressionVisitorMock);

      _expressionPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }
  }
}