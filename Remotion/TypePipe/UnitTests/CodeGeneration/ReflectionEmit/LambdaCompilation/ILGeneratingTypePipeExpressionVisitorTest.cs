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
using System.Reflection.Emit;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = NUnit.Framework.Is;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit.LambdaCompilation
{
  [TestFixture]
  public class ILGeneratingTypePipeExpressionVisitorTest
  {
    public interface IChildExpressionEmitter
    {
      void EmitChildExpression (Expression childExpression);
    }

    private IILGenerator _ilGeneratorMock;
    private ILGeneratingTypePipeExpressionVisitor _visitor;
    private IChildExpressionEmitter _childExpressionEmitterMock;

    [SetUp]
    public void SetUp ()
    {
      _ilGeneratorMock = MockRepository.GenerateStrictMock<IILGenerator>();
      _childExpressionEmitterMock = MockRepository.GenerateStrictMock<IChildExpressionEmitter>();
      _visitor = new ILGeneratingTypePipeExpressionVisitor (_ilGeneratorMock, _childExpressionEmitterMock.EmitChildExpression);
    }

    [Test]
    public void VisitThis ()
    {
      var expression = ExpressionTreeObjectMother.GetSomeThisExpression();
      _ilGeneratorMock.Expect (mock => mock.Emit (OpCodes.Ldarg_0));

      var result = _visitor.VisitThis (expression);

      _ilGeneratorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "OriginalBodyExpression must be replaced before code generation.")]
    public void VisitOriginalBody ()
    {
      _visitor.VisitOriginalBody (ExpressionTreeObjectMother.GetSomeOriginalBodyExpression());
    }

    [Test]
    public void VisitNonVirtualMethodAddress ()
    {
      var expression = ExpressionTreeObjectMother.GetSomeMethodAddressExpression();
      _ilGeneratorMock.Expect (mock => mock.Emit (OpCodes.Ldftn, expression.Method));

      var result = _visitor.VisitNonVirtualMethodAddress (expression);
      
      _ilGeneratorMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitVirtualMethodAddress ()
    {
      var expression = ExpressionTreeObjectMother.GetSomeVirtualMethodAddressExpression();
      _childExpressionEmitterMock.Expect (mock => mock.EmitChildExpression (expression.Instance));
      _ilGeneratorMock.Expect (mock => mock.Emit (OpCodes.Ldvirtftn, expression.Method));

      var result = _visitor.VisitVirtualMethodAddress (expression);

      _childExpressionEmitterMock.VerifyAllExpectations ();
      _ilGeneratorMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitNewDelegate ()
    {
      var delegateType = typeof (Action);
      var delegateCtor = delegateType.GetConstructors().Single();
      var targetExpression = ExpressionTreeObjectMother.GetSomeExpression (GetType());
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method());
      var expression = new NewDelegateExpression (delegateType, targetExpression, method);

      _childExpressionEmitterMock.Expect (mock => mock.EmitChildExpression (expression.Target));
      _ilGeneratorMock.Expect (mock => mock.Emit (OpCodes.Ldftn, expression.Method));
      _ilGeneratorMock.Expect (mock => mock.Emit (OpCodes.Newobj, delegateCtor));

      var result = _visitor.VisitNewDelegate (expression);

      _childExpressionEmitterMock.VerifyAllExpectations();
      _ilGeneratorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expression));
    }

    private void Method () { }
  }
}