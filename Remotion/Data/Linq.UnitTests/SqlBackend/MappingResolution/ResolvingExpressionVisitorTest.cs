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
    public void VisitSqlMemberExpression_CreatesSqlColumnExpression() 
    {
      var memberInfo = typeof (Cook).GetProperty ("Substitution");
      var memberExpression = new SqlMemberExpression (_sqlTable, memberInfo);

      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveMemberExpression (memberExpression, _generator))
          .Return (fakeResult);

      var result = ResolvingExpressionVisitor.ResolveExpression (memberExpression, _resolverMock, _generator);

      Assert.That (result, Is.SameAs (fakeResult));
      _resolverMock.VerifyAllExpectations ();
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