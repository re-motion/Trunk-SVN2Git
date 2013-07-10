﻿// This file is part of the re-linq project (relinq.codeplex.com)
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.UnitTests.Linq.Core.Clauses.Expressions;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  [TestFixture]
  public class SqlCollectionExpressionTest
  {
    private Expression[] _items;
    private SqlCollectionExpression _collectionExpression;

    [SetUp]
    public void SetUp ()
    {
      var item1 = new SqlLiteralExpression (13);
      var item2 = Expression.Constant (12);
      _items = new Expression[] { item1, item2 };

      _collectionExpression = new SqlCollectionExpression (typeof (List<int>), _items);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_collectionExpression.Type, Is.EqualTo (typeof (List<int>)));
      Assert.That (_collectionExpression.Items, Is.EqualTo (_items));
    }

    [Test]
    public void VisitChildren_SameItems ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ExpressionTreeVisitor>();
      visitorMock
          .Expect (mock => mock.VisitAndConvert (_collectionExpression.Items, "SqlCollectionExpression.VisitChildren"))
          .Return (_collectionExpression.Items);

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_collectionExpression, visitorMock);

      visitorMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (_collectionExpression));
    }

    [Test]
    public void VisitChildren_NewItems ()
    {
      var newItem = Expression.Constant (14);

      var visitorMock = MockRepository.GenerateStrictMock<ExpressionTreeVisitor>();
      visitorMock
          .Expect (mock => mock.VisitAndConvert (_collectionExpression.Items, "SqlCollectionExpression.VisitChildren"))
          .Return (new ReadOnlyCollection<Expression> (new[] { newItem, _items[1] }));

      var result = ExtensionExpressionTestHelper.CallVisitChildren (_collectionExpression, visitorMock);

      visitorMock.VerifyAllExpectations();

      Assert.That (result, Is.Not.SameAs (_collectionExpression));
      Assert.That (result, Is.TypeOf<SqlCollectionExpression>());
      Assert.That (result.Type, Is.SameAs (_collectionExpression.Type));
      Assert.That (((SqlCollectionExpression) result).Items, Is.EqualTo (new[] { newItem, _items[1] }));
    }

    [Test]
    public void Accept_VisitorSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorSupportingType<SqlCollectionExpression, ISqlCollectionExpressionVisitor> (
          _collectionExpression,
          mock => mock.VisitSqlCollectionExpression (_collectionExpression));
    }

    [Test]
    public void Accept_VisitorNotSupportingExpressionType ()
    {
      ExtensionExpressionTestHelper.CheckAcceptForVisitorNotSupportingType (_collectionExpression);
    }

    [Test]
    public new void ToString ()
    {
      Assert.That (_collectionExpression.ToString(), Is.EqualTo ("(13,12)"));
    }
  }
}