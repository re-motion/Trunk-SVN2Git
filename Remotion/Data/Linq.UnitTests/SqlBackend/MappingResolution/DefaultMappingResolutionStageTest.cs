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
  public class DefaultMappingResolutionStageTest
  {
    private ISqlStatementResolver _resolverMock;
    private UniqueIdentifierGenerator _uniqueIdentifierGenerator;
    private UnresolvedTableInfo _unresolvedTableInfo;
    private SqlTable _sqlTable;
    private ResolvedTableInfo _fakeResolvedTableInfo;
    private DefaultMappingResolutionStage _stage;

    [SetUp]
    public void SetUp ()
    {
      _resolverMock = MockRepository.GenerateMock<ISqlStatementResolver> ();
      _uniqueIdentifierGenerator = new UniqueIdentifierGenerator ();

      _unresolvedTableInfo = SqlStatementModelObjectMother.CreateUnresolvedTableInfo (typeof (Cook));
      _sqlTable = SqlStatementModelObjectMother.CreateSqlTable (_unresolvedTableInfo);
      _fakeResolvedTableInfo = SqlStatementModelObjectMother.CreateResolvedTableInfo (typeof (Cook));

      _stage = new DefaultMappingResolutionStage (_resolverMock, _uniqueIdentifierGenerator);
    }

    [Test]
    public void ResolveSelectExpression ()
    {
      var expression = new SqlTableReferenceExpression (_sqlTable);
      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (expression, _uniqueIdentifierGenerator))
          .Return (fakeResult);
      _resolverMock
        .Expect (mock => mock.ResolveConstantExpression (fakeResult))
        .Return (fakeResult);
      _resolverMock.Replay ();

      var result = _stage.ResolveSelectExpression (expression);

      _resolverMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ResolveWhereExpression ()
    {
      var expression = new SqlTableReferenceExpression (_sqlTable);
      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (expression, _uniqueIdentifierGenerator))
          .Return (fakeResult);
      _resolverMock
        .Expect (mock => mock.ResolveConstantExpression (fakeResult))
        .Return (fakeResult);
      _resolverMock.Replay ();

      var result = _stage.ResolveWhereExpression (expression);

      _resolverMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ResolveOrderingExpression ()
    {
      var expression = new SqlTableReferenceExpression (_sqlTable);
      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (expression, _uniqueIdentifierGenerator))
          .Return (fakeResult);
      _resolverMock
        .Expect (mock => mock.ResolveConstantExpression (fakeResult))
        .Return (fakeResult);
      _resolverMock.Replay();

      var result = _stage.ResolveOrderingExpression (expression);

      _resolverMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (fakeResult));
    
    }

    [Test]
    public void ResolveTopExpression ()
    {
      var expression = new SqlTableReferenceExpression (_sqlTable);
      var fakeResult = Expression.Constant (0);

      _resolverMock
          .Expect (mock => mock.ResolveTableReferenceExpression (expression, _uniqueIdentifierGenerator))
          .Return (fakeResult);
      _resolverMock
        .Expect (mock => mock.ResolveConstantExpression (fakeResult))
        .Return (fakeResult);
      _resolverMock.Replay ();

      var result = _stage.ResolveTopExpression (expression);

      _resolverMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ResolveTableInfo ()
    {
      _resolverMock
          .Expect (mock => mock.ResolveTableInfo (_unresolvedTableInfo, _uniqueIdentifierGenerator))
          .Return (_fakeResolvedTableInfo);
      _resolverMock.Replay ();

      var result = _stage.ResolveTableInfo (_sqlTable.TableInfo);

      _resolverMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeResolvedTableInfo));
    }

    [Test]
    public void ResolveJoinInfo ()
    {
      var memberInfo = typeof (Kitchen).GetProperty ("Cook");
      var join = _sqlTable.GetOrAddJoin (memberInfo, JoinCardinality.One);
      var joinInfo = (UnresolvedJoinInfo) join.JoinInfo;

      var fakeResolvedJoinInfo = SqlStatementModelObjectMother.CreateResolvedJoinInfo (typeof (Cook));

      _resolverMock
            .Expect (mock => mock.ResolveJoinInfo (_sqlTable, joinInfo, _uniqueIdentifierGenerator))
            .Return (fakeResolvedJoinInfo);
      _resolverMock.Replay ();

      var result = _stage.ResolveJoinInfo (_sqlTable, joinInfo);

      _resolverMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResolvedJoinInfo));
    }

    [Ignore("TODO: add test")]
    [Test]
    public void ResolveSqlStatement ()
    {
      Assert.Fail();
    }
  }
}