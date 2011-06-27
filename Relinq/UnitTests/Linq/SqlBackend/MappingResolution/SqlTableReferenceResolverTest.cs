// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.Parsing;
using Remotion.Linq.UnitTests.Linq.Core.Parsing.ExpressionTreeVisitorTests;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.MappingResolution
{
  [TestFixture]
  public class SqlTableReferenceResolverTest
  {
    private IMappingResolver _resolverMock;
    private UniqueIdentifierGenerator _generator;
    private IMappingResolutionContext _mappingResolutionContext;

    [SetUp]
    public void SetUp ()
    {
      _resolverMock = MockRepository.GenerateStrictMock<IMappingResolver>();
      _generator = new UniqueIdentifierGenerator();
      _mappingResolutionContext = new MappingResolutionContext();
    }

    [Test]
    public void ResolveSqlTableReferenceExpression_WithResolvedSimpleTableInfo ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable_WithResolvedTableInfo (typeof (Cook));
      var expression = new SqlTableReferenceExpression (sqlTable);
      var fakeResult = new SqlEntityDefinitionExpression (
          typeof (Cook), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _resolverMock
          .Expect (mock => mock.ResolveSimpleTableInfo (expression.SqlTable.GetResolvedTableInfo(), _generator))
          .Return (fakeResult);
      _resolverMock.Replay();

      var result = SqlTableReferenceResolver.ResolveTableReference (expression, _resolverMock, _generator, _mappingResolutionContext);

      _resolverMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeResult));
      Assert.That (_mappingResolutionContext.GetSqlTableForEntityExpression ((SqlEntityExpression) result), Is.SameAs (sqlTable));
    }

    [Test]
    public void ResolveSqlTableReferenceExpression_WithResolvedSubStatementTableInfo ()
    {
      var sqlStatement = new SqlStatementBuilder (SqlStatementModelObjectMother.CreateSqlStatement_Resolved (typeof (Cook)))
                         {
                             SelectProjection = new NamedExpression ("test", Expression.Constant (5)),
                             DataInfo = new StreamedSequenceInfo (typeof (Cook[]), Expression.Constant (new Cook()))
                         }.GetSqlStatement();
      var tableInfo = new ResolvedSubStatementTableInfo ("q0", sqlStatement);
      var sqlTable = new SqlTable (tableInfo, JoinSemantics.Inner);
      var expression = new SqlTableReferenceExpression (sqlTable);

      var result = SqlTableReferenceResolver.ResolveTableReference (expression, _resolverMock, _generator, _mappingResolutionContext);

      Assert.That (result, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      Assert.That (((SqlColumnDefinitionExpression) result).ColumnName, Is.EqualTo ("test"));
      Assert.That (((SqlColumnDefinitionExpression) result).OwningTableAlias, Is.EqualTo (tableInfo.TableAlias));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void ResolveSqlTableReferenceExpression_WithResolvedJoinedGroupingTableInfo_LikeSubStatement ()
    {
      var sqlStatement = new SqlStatementBuilder (SqlStatementModelObjectMother.CreateSqlStatement_Resolved (typeof (Cook)))
      {
        SelectProjection = new NamedExpression ("test", Expression.Constant (5)),
      }.GetSqlStatement ();

      var tableInfo = SqlStatementModelObjectMother.CreateResolvedJoinedGroupingTableInfo (sqlStatement);
      var sqlTable = new SqlTable (tableInfo, JoinSemantics.Inner);
      var expression = new SqlTableReferenceExpression (sqlTable);

      var result = SqlTableReferenceResolver.ResolveTableReference (expression, _resolverMock, _generator, _mappingResolutionContext);

      Assert.That (result, Is.TypeOf (typeof (SqlColumnDefinitionExpression)));
      Assert.That (((SqlColumnDefinitionExpression) result).ColumnName, Is.EqualTo ("test"));
      Assert.That (((SqlColumnDefinitionExpression) result).OwningTableAlias, Is.EqualTo (tableInfo.TableAlias));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This table has not yet been resolved; call the resolution step first.")]
    public void ResolveSqlTableReferenceExpression_VisitUnresolvedTableInfo ()
    {
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable_WithUnresolvedTableInfo (typeof (Cook));
      var expression = new SqlTableReferenceExpression (sqlTable);

      SqlTableReferenceResolver.ResolveTableReference (expression, _resolverMock, _generator, _mappingResolutionContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This table has not yet been resolved; call the resolution step first.")]
    public void ResolveSqlTableReferenceExpression_VisitUnresolvedGroupReferenceTableInfo ()
    {
      var tableInfo = SqlStatementModelObjectMother.CreateUnresolvedGroupReferenceTableInfo();
      var sqlTable = new SqlTable (tableInfo, JoinSemantics.Inner);
      var expression = new SqlTableReferenceExpression (sqlTable);

      SqlTableReferenceResolver.ResolveTableReference (expression, _resolverMock, _generator, _mappingResolutionContext);
    }
  }
}