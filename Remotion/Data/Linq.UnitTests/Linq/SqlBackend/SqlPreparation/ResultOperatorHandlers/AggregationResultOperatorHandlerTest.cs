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
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  [TestFixture]
  public class AggregationResultOperatorHandlerTest
  {
    private ISqlPreparationStage _stageMock;
    private UniqueIdentifierGenerator _generator;
    private TestableAggregationResultOperatorHandler _handler;
    private SqlStatementBuilder _sqlStatementBuilder;
    private SqlPreparationContext _context;

    [SetUp]
    public void SetUp ()
    {
      _generator = new UniqueIdentifierGenerator ();
      _stageMock = new DefaultSqlPreparationStage(MethodCallTransformerRegistry.CreateDefault(), ResultOperatorHandlerRegistry.CreateDefault(), _generator); // MockRepository.GenerateMock<ISqlPreparationStage> ();
      _handler = new TestableAggregationResultOperatorHandler();
      _sqlStatementBuilder = new SqlStatementBuilder (SqlStatementModelObjectMother.CreateSqlStatement ())
      {
        DataInfo = new StreamedSequenceInfo (typeof (int[]), Expression.Constant (5)),
        SelectProjection = new NamedExpression (null, Expression.Constant (0))
      };
      _context = new SqlPreparationContext ();
    }

    [Test]
    public void HandleResultOperator ()
    {
      // TODO Review 2917: Write a dedicated unit test for orderings (like with CountResultOperatorHandlerTest)
      // TODO Review 2917: Write a unit test with TopExpression and orderings - the inner statement should keep the orderings, the outer statement should not
      _sqlStatementBuilder.Orderings.Add (new Ordering (Expression.Constant ("order"), OrderingDirection.Asc));
      var averageResultOperator = new MaxResultOperator ();

      _handler.HandleResultOperator (averageResultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      Assert.That (((AggregationExpression) _sqlStatementBuilder.SelectProjection).AggregationModifier, Is.EqualTo (AggregationModifier.Max));
      Assert.That (_sqlStatementBuilder.DataInfo, Is.TypeOf (typeof (StreamedSingleValueInfo)));
      Assert.That (((StreamedSingleValueInfo) _sqlStatementBuilder.DataInfo).DataType, Is.EqualTo (typeof (int)));
      Assert.That (_sqlStatementBuilder.Orderings.Count, Is.EqualTo(0));
    }

    [Test]
    public void HandleResultOperator_AfterTopExpression_CreatesSubstatement ()
    {
      _sqlStatementBuilder.TopExpression = Expression.Constant ("top");

      var resultOperator = new MaxResultOperator ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
    }

    [Test]
    public void HandleResultOperator_AfterGroupExpression_CreatesSubStatement ()
    {
      _sqlStatementBuilder.GroupByExpression = Expression.Constant ("group");

      var resultOperator = new MaxResultOperator ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
    }

    [Test]
    public void HandleResultOperator_AfterDistinctExpression_CreatesSubStatement ()
    {
      _sqlStatementBuilder.IsDistinctQuery = true;
      _sqlStatementBuilder.TopExpression = Expression.Constant ("top");

      var resultOperator = new MaxResultOperator ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);

      Assert.That (_sqlStatementBuilder.SqlTables.Count, Is.EqualTo (1));
      Assert.That (((SqlTable) _sqlStatementBuilder.SqlTables[0]).TableInfo, Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
      Assert.That (
          ((SqlTable) ((SqlTableReferenceExpression) ((NamedExpression) ((AggregationExpression) _sqlStatementBuilder.SelectProjection).Expression).Expression).SqlTable).TableInfo,
          Is.TypeOf (typeof (ResolvedSubStatementTableInfo)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Named expression expected at this point")]
    public void HandleResultOperator_SelectProjectionNoNamedExpression_ThrowsException ()
    {
      _sqlStatementBuilder.SelectProjection = Expression.Constant ("select");
     
      var resultOperator = new MaxResultOperator ();

      _handler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stageMock, _context);
    }
    
  }
}