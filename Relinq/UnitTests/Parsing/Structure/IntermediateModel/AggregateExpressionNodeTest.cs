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

namespace Remotion.Linq.UnitTests.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class AggregateExpressionNodeTest : ExpressionNodeTestBase
  {
    private Expression<Func<int, int, int>> _func;
    private AggregateExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();
      _func = ExpressionHelper.CreateLambdaExpression<int, int, int> ((total, i) => total + i);
      _node = new AggregateExpressionNode (CreateParseInfo (), _func);
    }

    [Test]
    public void SupportedMethods ()
    {
      AssertSupportedMethod_Generic (AggregateExpressionNode.SupportedMethods, q => q.Aggregate ((i, j) => null), e => e.Aggregate ((i, j) => null));
    }

    [Test]
    public void Initialization ()
    {
      var parseInfo = CreateParseInfo();
      var node = new AggregateExpressionNode (parseInfo, _func);

      Assert.That (node.Source, Is.SameAs (SourceNode));
      Assert.That (node.Func, Is.SameAs (_func));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Func must have exactly two parameters.\r\nParameter name: func")]
    public void Initialization_WrongNumberOfParameters ()
    {
      var parseInfo = CreateParseInfo ();
      new AggregateExpressionNode (parseInfo, ExpressionHelper.CreateLambdaExpression<int, bool> (i => false));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Resolve_ThrowsInvalidOperationException ()
    {
      _node.Resolve (ExpressionHelper.CreateParameterExpression (), ExpressionHelper.CreateExpression (), ClauseGenerationContext);
    }

    [Test]
    public void GetResolvedFunc ()
    {
      var expectedResult = Expression.Lambda (Expression.Add (_func.Parameters[0], SourceReference), _func.Parameters[0]);

      var result = _node.GetResolvedFunc (ClauseGenerationContext);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void Apply ()
    {
      var result = _node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (result, Is.SameAs (QueryModel));

      var resultOperator = (AggregateResultOperator) QueryModel.ResultOperators[0];
      Assert.That (resultOperator.Func, Is.EqualTo (_node.GetResolvedFunc (ClauseGenerationContext)));
    }
  }
}
