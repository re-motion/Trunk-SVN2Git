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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Linq.UnitTests.Linq.Core.Clauses.Expressions;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.Unresolved
{
  [TestFixture]
  public class SqlEntityRefMemberExpressionTest
  {
    private PropertyInfo _memberInfo;
    private SqlEntityRefMemberExpression _expression;
    private SqlEntityExpression _entityExpression;

    [SetUp]
    public void SetUp ()
    {
      _memberInfo = typeof (Cook).GetProperty ("Substitution");
      _entityExpression = new SqlEntityDefinitionExpression (typeof (Cook), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      _expression = new SqlEntityRefMemberExpression (_entityExpression, _memberInfo);
    }

    [Test]
    public void Initialization_TypeInferredFromMemberType ()
    {
      Assert.That (_expression.Type, Is.SameAs (typeof (Cook)));
    }

    [Test]
    public void VisitChildren_ReturnsThis_WithoutCallingVisitMethods ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ExpressionTreeVisitor>();
      visitorMock.Replay();

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_expression, visitorMock);

      Assert.That (result, Is.SameAs (_expression));
      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_VisitorSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorSupportingType<SqlEntityRefMemberExpression, IUnresolvedSqlExpressionVisitor> (
          _expression,
          mock => mock.VisitSqlEntityRefMemberExpression (_expression));
    }

    [Test]
    public void Accept_VisitorNotSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorNotSupportingType (_expression);
    }

    [Test]
    public new void ToString ()
    {
      var result = _expression.ToString();

      Assert.That (result, Is.EqualTo ("[c].[Substitution]"));
    }
  }
}