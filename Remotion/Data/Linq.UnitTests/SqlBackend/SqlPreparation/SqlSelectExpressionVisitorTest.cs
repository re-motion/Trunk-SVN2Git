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
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlPreparation
{
  [TestFixture]
  public class SqlSelectExpressionVisitorTest
  {
    private SqlPreparationContext _context;
    private MainFromClause _mainFromClause;
    private QuerySourceReferenceExpression _querySourceReferenceExpression;
    private SqlTable _sqlTable;


    [SetUp]
    public void SetUp ()
    {
      _context = new SqlPreparationContext();
      _mainFromClause = ClauseObjectMother.CreateMainFromClause ();
      _querySourceReferenceExpression = new QuerySourceReferenceExpression (_mainFromClause);
      var source = new ConstantTableSource ((ConstantExpression) _mainFromClause.FromExpression);
      _sqlTable = new SqlTable ();
      _sqlTable.TableSource = source;
      _context.AddQuerySourceMapping (_mainFromClause, _sqlTable);
    }

    [Test]
    public void VisitQuerySourceReferenceExpression_CreatesSqlTableReferenceExpression ()
    {
      var result = SqlSelectExpressionVisitor.TranslateExpression (_querySourceReferenceExpression, _context);

      Assert.That (result, Is.TypeOf (typeof (SqlTableReferenceExpression)));
      Assert.That (((SqlTableReferenceExpression) result).SqlTable, Is.SameAs (_sqlTable));
      Assert.That (result.Type, Is.SameAs (typeof (Cook[])));
    }

    [Test]
    public void VisitMemberExpression_CreatesSqlMemberExpression ()
    {
      Expression memberExpression = Expression.MakeMemberAccess (_querySourceReferenceExpression, typeof (Cook).GetProperty ("FirstName"));

      var result = SqlSelectExpressionVisitor.TranslateExpression (memberExpression, _context);

      Assert.That (result, Is.TypeOf (typeof(SqlMemberExpression)));
      Assert.That (((SqlMemberExpression) result).SqlTable, Is.SameAs (_sqlTable));
      Assert.That (result.Type, Is.SameAs (typeof (Cook[])));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage  =
        "The given expression type 'NotSupportedExpression' is not supported in select clauses. (Expression: '[2147483647]')")]
    public void VisitNotSupportedExpression_ThrowsNotImplentedException ()
    {
      var expression = new NotSupportedExpression (typeof (int));
      SqlSelectExpressionVisitor.TranslateExpression (expression, _context);
    }

  }
}