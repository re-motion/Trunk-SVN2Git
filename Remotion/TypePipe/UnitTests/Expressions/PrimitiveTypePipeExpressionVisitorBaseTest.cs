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
using Remotion.TypePipe.Expressions;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Remotion.Development.UnitTesting;

namespace Remotion.TypePipe.UnitTests.Expressions
{
  [TestFixture]
  public class PrimitiveTypePipeExpressionVisitorBaseTest
  {
    [Test]
    public void Visit_XXX ()
    {
      var thisExpression = ExpressionTreeObjectMother.GetSomeThisExpression();
      CheckDefaultVisitImplementation (
          thisExpression,
          mock => PrimitiveTypePipeExpressionVisitorTestHelper.CallVisitThis (mock, thisExpression),
          visitor => visitor.VisitThis (thisExpression));

      var newDelegateExpression = ExpressionTreeObjectMother.GetSomeNewDelegateExpression ();
      CheckDefaultVisitImplementation (
          newDelegateExpression,
          mock => PrimitiveTypePipeExpressionVisitorTestHelper.CallVisitNewDelegate (mock, newDelegateExpression),
          visitor => visitor.VisitNewDelegate (newDelegateExpression));
    }

    private void CheckDefaultVisitImplementation<T> (
      T expression,
      Function<PrimitiveTypePipeExpressionVisitorBase, Expression> expectedVisitMethod,
      Func<IPrimitiveTypePipeExpressionVisitor, Expression> invokedMethod)
        where T : Expression
    {
      var fakeResult = ExpressionTreeObjectMother.GetSomeExpression();

      var visitorBaseMock = MockRepository.GenerateStrictMock<PrimitiveTypePipeExpressionVisitorBase>();
      visitorBaseMock.Expect (expectedVisitMethod).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      visitorBaseMock.Expect (mock => mock.Invoke ("VisitExtension", expression)).Return (fakeResult);

      var result = invokedMethod (visitorBaseMock);

      visitorBaseMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeResult));
    }
  }
}