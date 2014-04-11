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
using NUnit.Framework;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Development.UnitTesting;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class SkipExpressionNodeTest : ExpressionNodeTestBase
  {
    private SkipExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();
      _node = new SkipExpressionNode (CreateParseInfo (), Expression.Constant (3));
    }

    [Test]
    public void SupportedMethod ()
    {
      AssertSupportedMethod_Generic (SkipExpressionNode.SupportedMethods, q => q.Skip (3), e => e.Skip (3));
    }

    [Test]
    public void Resolve_PassesExpressionToSource ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode> ();
      var node = new SkipExpressionNode (CreateParseInfo (sourceMock), Expression.Constant (2));
      var expression = ExpressionHelper.CreateLambdaExpression ();
      var parameter = ExpressionHelper.CreateParameterExpression ();
      var expectedResult = ExpressionHelper.CreateExpression ();
      sourceMock.Expect (mock => mock.Resolve (parameter, expression, ClauseGenerationContext)).Return (expectedResult);

      var result = node.Resolve (parameter, expression, ClauseGenerationContext);

      sourceMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    public void Apply ()
    {
      var result = _node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (result, Is.SameAs (QueryModel));

      Assert.That (((SkipResultOperator) QueryModel.ResultOperators[0]).Count, Is.SameAs (_node.Count));
      Assert.That (((SkipResultOperator) QueryModel.ResultOperators[0]).GetConstantCount (), Is.EqualTo (3));
    }
  }
}
