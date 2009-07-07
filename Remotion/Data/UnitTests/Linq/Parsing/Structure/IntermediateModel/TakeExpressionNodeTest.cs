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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultModifications;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class TakeExpressionNodeTest : ExpressionNodeTestBase
  {
    private TakeExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();
      _node = new TakeExpressionNode (CreateParseInfo (), 3);
    }

    [Test]
    public void SupportedMethod ()
    {
      AssertSupportedMethod_Generic (TakeExpressionNode.SupportedMethods, q => q.Take(3), e => e.Take(3));
    }

    [Test]
    public void Resolve_PassesExpressionToSource ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode>();
      var node = new TakeExpressionNode (CreateParseInfo (sourceMock),0);
      var expression = ExpressionHelper.CreateLambdaExpression();
      var parameter = ExpressionHelper.CreateParameterExpression();
      var expectedResult = ExpressionHelper.CreateExpression();
      sourceMock.Expect (mock => mock.Resolve (parameter, expression, ClauseGenerationContext)).Return (expectedResult);

      var result = node.Resolve (parameter, expression, ClauseGenerationContext);

      sourceMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    public void Apply ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel();

      var result = _node.Apply (queryModel, ClauseGenerationContext);
      Assert.That (result, Is.SameAs (queryModel));
      
      var selectClause = (SelectClause) queryModel.SelectOrGroupClause;
      Assert.That (((TakeResultOperator) selectClause.ResultOperators[0]).Count, Is.EqualTo (3));
    }
  }
}