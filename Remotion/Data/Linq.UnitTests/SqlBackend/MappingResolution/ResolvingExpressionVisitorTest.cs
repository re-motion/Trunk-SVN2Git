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
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.MappingResolution
{
  [TestFixture]
  public class ResolvingExpressionVisitorTest
  {
    private ISqlStatementResolver _resolverMock;
    private ConstantTableSource _source;
    private SqlTable _sqlTable;
    private UniqueIdentifierGenerator _generator;

    [SetUp]
    public void SetUp ()
    {
      _resolverMock = MockRepository.GenerateMock<ISqlStatementResolver>();
      _source = SqlStatementModelObjectMother.CreateConstantTableSource_TypeIsCook();
      _sqlTable = new SqlTable (_source);
      _generator = new UniqueIdentifierGenerator();
    }

    [Test]
    public void VisitSqlTableReferenceExpression_ResolvesExpression ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_sqlTable);
      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (tableReferenceExpression))
          .Return (fakeResult);

      var result = ResolvingExpressionVisitor.ResolveExpression (tableReferenceExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (fakeResult));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlTableReferenceExpression_ResolvesExpression_AndRevisitsResult ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_sqlTable);
      var unresolvedResult = new SqlTableReferenceExpression (_sqlTable);
      var resolvedResult = Expression.Constant (0);

      using (_resolverMock.GetMockRepository().Ordered())
      {
        _resolverMock
            .Expect (mock => mock.ResolveTableReferenceExpression (tableReferenceExpression))
            .Return (unresolvedResult);
        _resolverMock
            .Expect (mock => mock.ResolveTableReferenceExpression (unresolvedResult))
            .Return (resolvedResult);
      }

      var result = ResolvingExpressionVisitor.ResolveExpression (tableReferenceExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (resolvedResult));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlTableReferenceExpression_ResolvesExpression_AndRevisitsResult_OnlyIfDifferent ()
    {
      var tableReferenceExpression = new SqlTableReferenceExpression (_sqlTable);
      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (tableReferenceExpression))
          .Return (tableReferenceExpression);

      var result = ResolvingExpressionVisitor.ResolveExpression (tableReferenceExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (tableReferenceExpression));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlMemberExpression_CreatesSqlColumnExpression ()
    {
      var memberInfo = typeof (Cook).GetProperty ("Substitution");
      var memberExpression = new SqlMemberExpression (_sqlTable, memberInfo);

      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveMemberExpression (memberExpression, _generator))
          .Return (fakeResult);

      var result = ResolvingExpressionVisitor.ResolveExpression (memberExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (fakeResult));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlMemberExpression_ResolvesExpression_AndRevisitsResult ()
    {
      var memberInfo = typeof (Cook).GetProperty ("FirstName");
      var sqlMemberExpression = new SqlMemberExpression (_sqlTable, memberInfo);

      var unresolvedResult = new SqlMemberExpression (_sqlTable, memberInfo);
      var resolvedResult = Expression.Constant (0);

      using (_resolverMock.GetMockRepository().Ordered())
      {
        _resolverMock
            .Expect (mock => mock.ResolveMemberExpression (sqlMemberExpression, _generator))
            .Return (unresolvedResult);
        _resolverMock
            .Expect (mock => mock.ResolveMemberExpression (unresolvedResult, _generator))
            .Return (resolvedResult);
      }

      var result = ResolvingExpressionVisitor.ResolveExpression (sqlMemberExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (resolvedResult));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlMemberExpression_ResolvesExpression_AndRevisitsResult_OnlyIfDifferent ()
    {
      var memberInfo = typeof (Cook).GetProperty ("FirstName");
      var sqlMemberExpression = new SqlMemberExpression (_sqlTable, memberInfo);

      _resolverMock
          .Expect (mock => mock.ResolveMemberExpression (sqlMemberExpression, _generator))
          .Return (sqlMemberExpression);

      var result = ResolvingExpressionVisitor.ResolveExpression (sqlMemberExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (sqlMemberExpression));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSqlEntityRefMemberExpression_ResolvesExpression ()
    {
      var memberInfo = typeof (Cook).GetProperty ("Substitution");
      var sqlEntityRefMemberExpression = new SqlEntityRefMemberExpression (_sqlTable, memberInfo);
      var primaryColumn = new SqlColumnExpression (typeof (int), "c", "ID");
      var foreignColumn = new SqlColumnExpression (typeof (int), "s", "ID");
      var tableSource = new SqlTableSource (typeof(Cook), "CookTable", "s");
      var expectedSqlJoinedTableSource = new SqlJoinedTableSource (tableSource, primaryColumn, foreignColumn);

      using (_resolverMock.GetMockRepository ().Ordered ())
      {
        _resolverMock
            .Expect (mock => mock.ResolveJoinedTableSource (Arg<JoinedTableSource>.Is.Anything))
            .Return (expectedSqlJoinedTableSource);

        _resolverMock
            .Expect (mock => mock.ResolveTableReferenceExpression (Arg<SqlTableReferenceExpression>.Is.Anything))
            .Return (new SqlColumnListExpression (typeof (Cook)));
      }

      var result = ResolvingExpressionVisitor.ResolveExpression (sqlEntityRefMemberExpression, _resolverMock, _generator);

      Assert.That (result, Is.TypeOf (typeof (SqlColumnListExpression)));
      _resolverMock.VerifyAllExpectations();
    }

    [Test]
    public void UnknownExpression ()
    {
      var unknownExpression = new NotSupportedExpression (typeof (int));
      var result = ResolvingExpressionVisitor.ResolveExpression (unknownExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (unknownExpression));
    }
  }
}