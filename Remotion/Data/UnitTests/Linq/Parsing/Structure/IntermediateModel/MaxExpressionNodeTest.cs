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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class MaxExpressionNodeTest : ExpressionNodeTestBase
  {
    [Test]
    public void SupportedMethod_WithoutSelector ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.Max());
      Assert.That (MaxExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void SupportedMethod_WithSelector ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.Max (i => i.ToString()));
      Assert.That (MaxExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Resolve_ThrowsInvalidOperationException ()
    {
      var node = new MaxExpressionNode (SourceStub, null);
      node.Resolve (ExpressionHelper.CreateParameterExpression (), ExpressionHelper.CreateExpression ());
    }

    [Test]
    public void GetResolvedPredicate ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode> ();
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new MaxExpressionNode (sourceMock, selector);

      var expectedResult = ExpressionHelper.CreateLambdaExpression ();
      sourceMock.Expect (mock => mock.Resolve (Arg<ParameterExpression>.Is.Anything, Arg<Expression>.Is.Anything)).Return (expectedResult);

      var result = node.GetResolvedExpression ();

      sourceMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Predicate must not be null.\r\nParameter name: OptionalSelector")]
    public void GetResolvedPredicate_ThrowsArgumentNullException ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode> ();
      var node = new MaxExpressionNode (sourceMock, null);
      node.GetResolvedExpression ();
    }

    [Test]
    public void CachedSelector ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode> ();
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new MaxExpressionNode (sourceMock, selector);
      var expectedResult = ExpressionHelper.CreateLambdaExpression ();
      sourceMock.Expect (mock => mock.Resolve (Arg<ParameterExpression>.Is.Anything, Arg<Expression>.Is.Anything)).Repeat.Once ().Return (expectedResult);
      node.GetResolvedExpression ();
      node.GetResolvedExpression ();
      sourceMock.VerifyAllExpectations ();
      Assert.That (PrivateInvoke.GetNonPublicField (node, "_cachedSelector"), Is.Not.Null);
    }
  }
}