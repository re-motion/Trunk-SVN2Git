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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class TakeExpressionNodeTest : ExpressionNodeTestBase
  {
    [Test]
    public void SupportedMethod ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.Take(3));
      Assert.That (TakeExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void Resolve_PassesExpressionToSource ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode>();
      var node = new TakeExpressionNode (sourceMock,0);
      var expression = ExpressionHelper.CreateLambdaExpression();
      var parameter = ExpressionHelper.CreateParameterExpression();
      var expectedResult = ExpressionHelper.CreateExpression();
      sourceMock.Expect (mock => mock.Resolve (parameter, expression)).Return (expectedResult);
      
      var result = node.Resolve (parameter, expression);

      sourceMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "TakeExpressionNode does not support to get a resolved expression.")]
    public void GetResolvedExpression_ThrowsInvalidOperationException ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode> ();
      var node = new TakeExpressionNode (sourceMock, 0);
      node.GetResolvedExpression();
    }
  }
}