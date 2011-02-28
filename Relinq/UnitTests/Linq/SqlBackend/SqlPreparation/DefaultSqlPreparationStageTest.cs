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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.Core;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation
{
  [TestFixture]
  public class DefaultSqlPreparationStageTest
  {
    private ISqlPreparationContext _context;
    private SqlTable _sqlTable;
    private QuerySourceReferenceExpression _querySourceReferenceExpression;
    private DefaultSqlPreparationStage _stage;

    [SetUp]
    public void SetUp ()
    {
      _context = SqlStatementModelObjectMother.CreateSqlPreparationContext();

      var querySource = ExpressionHelper.CreateMainFromClause_Cook();
      _sqlTable = new SqlTable (new UnresolvedTableInfo (typeof (Cook)), JoinSemantics.Inner);

      _context.AddExpressionMapping (new QuerySourceReferenceExpression(querySource), new SqlTableReferenceExpression(_sqlTable));

      _querySourceReferenceExpression = new QuerySourceReferenceExpression (querySource);

      _stage = new DefaultSqlPreparationStage (
          CompoundMethodCallTransformerProvider.CreateDefault(), ResultOperatorHandlerRegistry.CreateDefault(), new UniqueIdentifierGenerator());
    }

    [Test]
    public void PrepareSelectExpression ()
    {
      var singleDataInfo = new StreamedSingleValueInfo (typeof (int), false);
      var selectProjection = Expression.Constant (0);
      var subStatement = new SqlStatement (singleDataInfo, selectProjection, new SqlTable[0], null, null, new Ordering[0], null, false, null, null);
      var expressionWithSubStatement = new SqlSubStatementExpression (subStatement);

      var result = _stage.PrepareSelectExpression (expressionWithSubStatement, _context);

      Assert.That (result, Is.SameAs(expressionWithSubStatement));
    }

    [Test]
    public void PrepareWhereExpression ()
    {
      var result = _stage.PrepareWhereExpression (_querySourceReferenceExpression, _context);

      var expectedExpression = new SqlTableReferenceExpression (_sqlTable);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, result);
    }

    [Test]
    public void PrepareTopExpression ()
    {
      var result = _stage.PrepareTopExpression (_querySourceReferenceExpression, _context);

      var expectedExpression = new SqlTableReferenceExpression (_sqlTable);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, result);
    }

    [Test]
    public void GetTableForFromExpression ()
    {
      var fromExpression = Expression.Constant (new Cook[0]);
      var result = _stage.PrepareFromExpression (fromExpression, _context, info=>new SqlTable(info, JoinSemantics.Inner));

      Assert.That (result.SqlTable, Is.TypeOf (typeof (SqlTable)));
    }

    [Test]
    public void PrepareSqlStatement ()
    {
      var queryModel = ExpressionHelper.CreateQueryModel_Cook();

      var result = _stage.PrepareSqlStatement (queryModel, _context);

      Assert.That (result, Is.Not.Null);
    }
  }
}