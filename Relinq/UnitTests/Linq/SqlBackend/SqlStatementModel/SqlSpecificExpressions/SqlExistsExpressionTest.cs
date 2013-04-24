// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  [TestFixture]
  public class SqlExistsExpressionTest
  {
    private SqlExistsExpression _sqlExistsExpression;

    [SetUp]
    public void SetUp ()
    {
      _sqlExistsExpression = new SqlExistsExpression (Expression.Constant ("test"));
    }

    [Test]
    public void Initialization_CheckType ()
    {
      Assert.That (_sqlExistsExpression.Type, Is.EqualTo (typeof (bool)));
    }

    [Test]
    public void VisitChildren_SameSource ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ExpressionTreeVisitor> ();

      visitorMock
          .Expect (mock => mock.VisitExpression (_sqlExistsExpression.Expression))
          .Return (_sqlExistsExpression.Expression);
      visitorMock.Replay ();

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_sqlExistsExpression, visitorMock);

      visitorMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (_sqlExistsExpression));
    }

    [Test]
    public void VisitChildren_NewSource ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ExpressionTreeVisitor> ();
      var newPrefix = Expression.Constant (3);

      visitorMock
          .Expect (mock => mock.VisitExpression (_sqlExistsExpression.Expression))
          .Return (newPrefix);
      visitorMock.Replay ();

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_sqlExistsExpression, visitorMock);

      visitorMock.VerifyAllExpectations ();

      Assert.That (result, Is.Not.SameAs (_sqlExistsExpression));
    }

    [Test]
    public void Accept_VisitorSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorSupportingType<SqlExistsExpression, ISqlExistsExpressionVisitor> (
          _sqlExistsExpression,
          mock => mock.VisitSqlExistsExpression(_sqlExistsExpression));
    }

    [Test]
    public void Accept_VisitorNotSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorNotSupportingType (_sqlExistsExpression);
    }

    [Test]
    public void To_String ()
    {
      var result = _sqlExistsExpression.ToString();

      Assert.That (result, Is.EqualTo ("EXISTS(\"test\")"));
    }
  }
}