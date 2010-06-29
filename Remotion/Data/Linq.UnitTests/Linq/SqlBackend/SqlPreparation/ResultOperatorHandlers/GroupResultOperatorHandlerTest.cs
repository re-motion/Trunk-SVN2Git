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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  [TestFixture]
  public class GroupResultOperatorHandlerTest
  {
    private ISqlPreparationStage _stageMock;
    private UniqueIdentifierGenerator _generator;
    private GroupResultOperatorHandler _handler;
    private SqlStatementBuilder _sqlStatementBuilder;
    private SqlPreparationContext _context;

    [SetUp]
    public void SetUp ()
    {
      _generator = new UniqueIdentifierGenerator();
      _stageMock = MockRepository.GenerateMock<ISqlPreparationStage>();
      _handler = new GroupResultOperatorHandler();
      _sqlStatementBuilder = new SqlStatementBuilder (SqlStatementModelObjectMother.CreateSqlStatement())
                             {
                                 DataInfo = new StreamedSequenceInfo (typeof (Cook[]), Expression.Constant (new Cook()))
                             };
      _context = new SqlPreparationContext();
    }


    [Test]
    public void HandleResultOperator ()
    {
      var keySelector = Expression.Constant ("keySelector");
      var elementSelector = Expression.Constant ("elementSelector");
      var resultOperator = new GroupResultOperator ("itemName", keySelector, elementSelector);

      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (keySelector, _context))
          .Return (keySelector);
      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (elementSelector, _context))
          .Return (elementSelector);
      _stageMock.Replay();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      _stageMock.VerifyAllExpectations();
      Assert.That (_sqlStatementBuilder.GroupByExpression, Is.SameAs (keySelector));
      
      var expectedSelectProjection = new SqlGroupingSelectExpression (
          new NamedExpression ("key", keySelector), 
          new NamedExpression ("element", elementSelector));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSelectProjection, _sqlStatementBuilder.SelectProjection);

      Assert.That (
          _sqlStatementBuilder.DataInfo.DataType,
          Is.EqualTo (typeof (IQueryable<>).MakeGenericType (typeof (IGrouping<,>).MakeGenericType (typeof (string), typeof (string)))));
    }

    [Test]
    public void HandleResultOperator_GroupByAfterTopExpression ()
    {
      var topExpression = Expression.Constant ("top");
      _sqlStatementBuilder.TopExpression = topExpression;

      var keySelector = Expression.Constant ("keySelector");
      var elementSelector = Expression.Constant ("elementSelector");
      var resultOperator = new GroupResultOperator ("itemName", keySelector, elementSelector);
      var fakeFromExpressionInfo = new FromExpressionInfo (
          new SqlTable (new ResolvedSubStatementTableInfo("sc", _sqlStatementBuilder.GetSqlStatement())), new Ordering[0], elementSelector, null, false);

      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (keySelector, _context))
          .Return (keySelector);
      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (elementSelector, _context))
          .Return (elementSelector);
      _stageMock
          .Expect (
              mock => mock.PrepareFromExpression (Arg<Expression>.Is.Anything, Arg.Is (_context), Arg<Func<ITableInfo, SqlTableBase>>.Is.Anything))
          .Return (fakeFromExpressionInfo);
      _stageMock.Replay ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      _stageMock.VerifyAllExpectations();
      Assert.That (_sqlStatementBuilder.GroupByExpression, Is.SameAs (keySelector));
      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
      var subStatement = ((ResolvedSubStatementTableInfo) ((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo).SqlStatement;
      Assert.That (subStatement.TopExpression, Is.SameAs (topExpression));
    }

    [Test]
    public void HandleResultOperator_GroupByAfterDistinct ()
    {
      _sqlStatementBuilder.IsDistinctQuery = true;

      var keySelector = Expression.Constant ("keySelector");
      var elementSelector = Expression.Constant ("elementSelector");
      var resultOperator = new GroupResultOperator ("itemName", keySelector, elementSelector);
      var fakeFromExpressionInfo = new FromExpressionInfo (
          new SqlTable (new ResolvedSubStatementTableInfo ("sc", _sqlStatementBuilder.GetSqlStatement ())), new Ordering[0], elementSelector, null, false);

      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (keySelector, _context))
          .Return (keySelector);
      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (elementSelector, _context))
          .Return (elementSelector);
      _stageMock
          .Expect (
              mock => mock.PrepareFromExpression (Arg<Expression>.Is.Anything, Arg.Is (_context), Arg<Func<ITableInfo, SqlTableBase>>.Is.Anything))
          .Return (fakeFromExpressionInfo);
      _stageMock.Replay ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      _stageMock.VerifyAllExpectations();
      Assert.That (_sqlStatementBuilder.GroupByExpression, Is.SameAs (keySelector));
      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
      var subStatement = ((ResolvedSubStatementTableInfo) ((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo).SqlStatement;
      Assert.That (subStatement.IsDistinctQuery, Is.True);
    }

    [Test]
    public void HandleResultOperator_GroupByAfterGroupBy ()
    {
      var groupByExpression = Expression.Constant("group");
      _sqlStatementBuilder.GroupByExpression = groupByExpression;

      var keySelector = Expression.Constant ("keySelector");
      var elementSelector = Expression.Constant ("elementSelector");
      var resultOperator = new GroupResultOperator ("itemName", keySelector, elementSelector);
      var fakeFromExpressionInfo = new FromExpressionInfo (
          new SqlTable (new ResolvedSubStatementTableInfo ("sc", _sqlStatementBuilder.GetSqlStatement ())), new Ordering[0], elementSelector, null, false);

      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (keySelector, _context))
          .Return (keySelector);
      _stageMock
          .Expect (mock => mock.PrepareResultOperatorItemExpression (elementSelector, _context))
          .Return (elementSelector);
      _stageMock
          .Expect (
              mock => mock.PrepareFromExpression (Arg<Expression>.Is.Anything, Arg.Is (_context), Arg<Func<ITableInfo, SqlTableBase>>.Is.Anything))
          .Return (fakeFromExpressionInfo);
      _stageMock.Replay ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      _stageMock.VerifyAllExpectations ();
      Assert.That (_sqlStatementBuilder.GroupByExpression, Is.SameAs (keySelector));
      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
      var subStatement = ((ResolvedSubStatementTableInfo) ((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo).SqlStatement;
      Assert.That (subStatement.GroupByExpression, Is.SameAs(groupByExpression));
    }
 }
}