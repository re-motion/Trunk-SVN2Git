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
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Rhino.Mocks;
using Remotion.Data.Linq.UnitTests.TestUtilities;

namespace Remotion.Data.Linq.UnitTests.Clauses.Expressions
{
  [TestFixture]
  public class ExtensionExpressionTest
  {
    [Test]
    public void Initialize_WithoutNodeType ()
    {
      var expression = new TestableExtensionExpression (typeof (int));

      Assert.That (expression.Type, Is.SameAs (typeof (int)));
      Assert.That (expression.NodeType, Is.EqualTo (ExtensionExpression.ExtensionExpressionNodeType));
    }

    [Test]
    public void Accept_CallsVisitUnknownExpression ()
    {
      var mockRepository = new MockRepository ();
      var visitorMock = mockRepository.StrictMock<ExpressionTreeVisitor>();
      var expression = new TestableExtensionExpression (typeof (int));

      visitorMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (visitorMock, "VisitUnknownExpression", expression))
          .Return (expression);
      visitorMock.Replay ();

      expression.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void Accept_ReturnsResultOf_VisitUnknownExpression ()
    {
      var mockRepository = new MockRepository ();
      var visitorMock = mockRepository.StrictMock<ExpressionTreeVisitor> ();
      var expression = new TestableExtensionExpression (typeof (int));
      var expectedResult = Expression.Constant (0);

      visitorMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (visitorMock, "VisitUnknownExpression", expression))
          .Return (expectedResult);
      visitorMock.Replay ();

      var result = expression.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (expectedResult));
    }
  }
}