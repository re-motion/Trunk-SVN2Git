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
using NUnit.Framework;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.UnitTests.Linq.Core.Clauses.Expressions;
using Remotion.Linq.UnitTests.Linq.Core.Parsing;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.Resolved
{
  [TestFixture]
  public class SqlColumnDefinitionExpressionTest
  {
    private SqlColumnDefinitionExpression _columnExpression;

    [SetUp]
    public void SetUp ()
    {
      _columnExpression = new SqlColumnDefinitionExpression (typeof (int), "t", "name", false);
    }

    [Test]
    public void Accept_VisitorSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorSupportingType<SqlColumnDefinitionExpression, ISqlColumnExpressionVisitor> (
          _columnExpression,
          mock => mock.VisitSqlColumnDefinitionExpression (_columnExpression));
    }

    [Test]
    public void Accept_VisitorSupportingExpressionType_BaseAccept ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorSupportingType<SqlColumnExpression, IResolvedSqlExpressionVisitor> (
          _columnExpression,
          mock => mock.VisitSqlColumnExpression (_columnExpression));
    }

    [Test]
    public void Accept_VisitorNotSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorNotSupportingType (_columnExpression);
    }

    [Test]
    public void Update ()
    {
      var columnExpression = new SqlColumnDefinitionExpression (typeof (string), "c", "columnName", false);

      var result = columnExpression.Update (typeof (char), "f", "test", false);

      var expectedResult = new SqlColumnDefinitionExpression (typeof (char), "f", "test", false);

      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedResult);
    }

    [Test]
    public new void ToString ()
    {
      var result = _columnExpression.ToString();

      Assert.That (result, Is.EqualTo ("[t].[name]"));
    }
  }
}