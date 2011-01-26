// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.ExpressionTransformation;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.ExpressionTreeVisitors
{
  [TestFixture]
  public class TransformingExpressionTreeVisitorTest
  {
    [Test]
    public void Transform_SingleMatchingTransformation ()
    {
      var inputExpression = CreateSimpleExpression(0);
      var transformedExpression = CreateSimpleExpression(0);

      var providerMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider> ();
      providerMock
          .Expect (mock => mock.GetTransformations (inputExpression))
          .Return (new ExpressionTransformation[] { expr => transformedExpression });
      providerMock
          .Expect (mock => mock.GetTransformations (transformedExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock.Replay ();

      var result = TransformingExpressionTreeVisitor.Transform (inputExpression, providerMock);

      providerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (transformedExpression));
    }

    [Test]
    public void Transform_NoMatchingTransformation ()
    {
      var inputExpression = CreateSimpleExpression(0);

      var providerMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider> ();
      providerMock
          .Expect (mock => mock.GetTransformations (inputExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock.Replay();

      var result = TransformingExpressionTreeVisitor.Transform (inputExpression, providerMock);

      providerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (inputExpression));
    }

    [Test]
    public void Transform_TwoMatchingTransformations_FirstTransformationChangesExpression ()
    {
      var inputExpression = CreateSimpleExpression(0);
      var transformedExpression = CreateSimpleExpression(0);

      var providerMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider> ();
      providerMock
          .Expect (mock => mock.GetTransformations (inputExpression))
          .Return (new ExpressionTransformation[] { 
              expr => transformedExpression, 
              expr => { throw new InvalidOperationException ("Must not be called."); }
          });
      providerMock
          .Expect (mock => mock.GetTransformations (transformedExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock.Replay();

      var result = TransformingExpressionTreeVisitor.Transform (inputExpression, providerMock);

      providerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (transformedExpression));
    }

    [Test]
    public void Transform_TwoMatchingTransformations_FirstTransformationDoesNotChangeExpression ()
    {
      var inputExpression = CreateSimpleExpression(0);
      var transformedExpression = CreateSimpleExpression(0);

      var providerMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider> ();
      providerMock
          .Expect (mock => mock.GetTransformations (inputExpression))
          .Return (new ExpressionTransformation[] { 
              expr => expr, 
              expr => transformedExpression
          });
      providerMock
          .Expect (mock => mock.GetTransformations (transformedExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock.Replay ();

      var result = TransformingExpressionTreeVisitor.Transform (inputExpression, providerMock);

      providerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (transformedExpression));
    }

    [Test]
    public void Transform_ChildrenTransformedBeforeParent ()
    {
      var inputChildExpression = CreateSimpleExpression(0);
      var transformedChildExpression = CreateSimpleExpression (1);

      var inputParentExpression = Expression.UnaryPlus (inputChildExpression);
      var transformedParentExpression = CreateSimpleExpression(2);

      var providerMock = MockRepository.GenerateStrictMock<IExpressionTranformationProvider> ();
      providerMock
          .Expect (mock => mock.GetTransformations (inputChildExpression))
          .Return (new ExpressionTransformation[] { expr => transformedChildExpression });
      providerMock
          .Expect (mock => mock.GetTransformations (transformedChildExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock
          .Expect (mock => mock.GetTransformations (
              Arg<Expression>.Matches (expr => expr is UnaryExpression && ((UnaryExpression) expr).Operand == transformedChildExpression)))
          .Return (new ExpressionTransformation[] { expr => transformedParentExpression });
      providerMock
          .Expect (mock => mock.GetTransformations (transformedParentExpression))
          .Return (new ExpressionTransformation[0]);
      providerMock.Replay ();

      var result = TransformingExpressionTreeVisitor.Transform (inputParentExpression, providerMock);

      providerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (transformedParentExpression));
    }

    private Expression CreateSimpleExpression (int id)
    {
      return Expression.Constant (id);
    }
  }
}