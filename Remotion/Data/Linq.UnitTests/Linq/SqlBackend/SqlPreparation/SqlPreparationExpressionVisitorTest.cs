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
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.Core;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation
{
  [TestFixture]
  public class SqlPreparationExpressionVisitorTest
  {
    private SqlPreparationContext _context;

    private MainFromClause _cookMainFromClause;
    private QuerySourceReferenceExpression _cookQuerySourceReferenceExpression;

    private MainFromClause _kitchenMainFromClause;

    private SqlTable _sqlTable;
    private ISqlPreparationStage _stageMock;
    private MethodCallTransformerRegistry _registry;

    [SetUp]
    public void SetUp ()
    {
      _stageMock = MockRepository.GenerateMock<ISqlPreparationStage>();
      _context = new SqlPreparationContext();
      _cookMainFromClause = ExpressionHelper.CreateMainFromClause_Cook();
      _cookQuerySourceReferenceExpression = new QuerySourceReferenceExpression (_cookMainFromClause);
      _kitchenMainFromClause = ExpressionHelper.CreateMainFromClause_Kitchen();
      var source = new UnresolvedTableInfo (_cookMainFromClause.ItemType);
      _sqlTable = new SqlTable (source);
      _context.AddQuerySourceMapping (_cookMainFromClause, _sqlTable);
      _registry = MethodCallTransformerRegistry.CreateDefault();
    }

    [Test]
    public void VisitQuerySourceReferenceExpression_CreatesSqlTableReferenceExpression ()
    {
      var result = SqlPreparationExpressionVisitor.TranslateExpression (_cookQuerySourceReferenceExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlTableReferenceExpression)));
      Assert.That (((SqlTableReferenceExpression) result).SqlTable, Is.SameAs (_sqlTable));
      Assert.That (result.Type, Is.SameAs (typeof (Cook)));
    }

    [Test]
    public void VisitMemberExpression_CreatesSqlMemberExpression ()
    {
      Expression memberExpression = Expression.MakeMemberAccess (_cookQuerySourceReferenceExpression, typeof (Cook).GetProperty ("FirstName"));

      var result = SqlPreparationExpressionVisitor.TranslateExpression (memberExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlMemberExpression)));
      Assert.That (((SqlMemberExpression) result).SqlTable, Is.SameAs (_sqlTable));
      Assert.That (result.Type, Is.SameAs (typeof (string)));
    }

    [Test]
    public void VisitSeveralMemberExpression_CreatesSqlMemberExpression_AndJoin ()
    {
      var expression = ExpressionHelper.Resolve<Kitchen, string> (_kitchenMainFromClause, k => k.Cook.FirstName);

      var source = SqlStatementModelObjectMother.CreateUnresolvedTableInfo (typeof (Kitchen));
      var sqlTable = new SqlTable (source);

      _context.AddQuerySourceMapping (_kitchenMainFromClause, sqlTable);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      var kitchenCookMember = typeof (Kitchen).GetProperty ("Cook");
      var cookFirstNameMember = typeof (Cook).GetProperty ("FirstName");

      Assert.That (result, Is.TypeOf (typeof (SqlMemberExpression)));
      Assert.That (((SqlMemberExpression) result).MemberInfo, Is.EqualTo (cookFirstNameMember));

      var join = sqlTable.GetJoin (kitchenCookMember);
      Assert.That (((SqlMemberExpression) result).SqlTable, Is.SameAs (join));

      Assert.That (join.JoinInfo, Is.TypeOf (typeof (UnresolvedJoinInfo)));
      Assert.That (((UnresolvedJoinInfo) join.JoinInfo).MemberInfo, Is.EqualTo (kitchenCookMember));
      Assert.That (((UnresolvedJoinInfo) join.JoinInfo).Cardinality, Is.EqualTo (JoinCardinality.One));
    }

    [Test]
    public void VisitMemberExpression_NonQuerySourceReferenceExpression ()
    {
      var memberExpression = Expression.MakeMemberAccess (Expression.Constant ("Test"), typeof (string).GetProperty ("Length"));
      var result = SqlPreparationExpressionVisitor.TranslateExpression (memberExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.EqualTo (memberExpression));
    }


    [Test]
    public void VisitNotSupportedExpression_ThrowsNotImplentedException ()
    {
      var expression = new CustomExpression (typeof (int));
      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      Assert.That (result, Is.EqualTo (expression));
    }

    [Test]
    public void VisitSubQueryExpression_NoContains ()
    {
      var querModel = ExpressionHelper.CreateQueryModel (_kitchenMainFromClause);
      var expression = new SubQueryExpression (querModel);
      var fakeSqlStatement = SqlStatementModelObjectMother.CreateSqlStatement_Resolved (typeof (Cook));

      _stageMock
          .Expect (mock => mock.PrepareSqlStatement (querModel))
          .Return (fakeSqlStatement);
      _stageMock.Replay();

      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      _stageMock.VerifyAllExpectations();
      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (SqlSubStatementExpression)));
      Assert.That (((SqlSubStatementExpression) result).SqlStatement, Is.SameAs (fakeSqlStatement));
      Assert.That (result.Type, Is.EqualTo (expression.Type));
    }

    [Test]
    public void VisitSubQueryExpression_WithContains ()
    {
      var querModel = ExpressionHelper.CreateQueryModel (_kitchenMainFromClause);
      var constantExpression = Expression.Constant (new Kitchen());
      var containsResultOperator = new ContainsResultOperator (constantExpression);
      querModel.ResultOperators.Add (containsResultOperator);

      var expression = new SubQueryExpression (querModel);
      var fakeSqlStatement =
          SqlStatementModelObjectMother.CreateSqlStatement (
                                   new SqlBinaryOperatorExpression ("IN", Expression.Constant (0), Expression.Constant (new[] { 1, 2, 3 }))
              );
      
      _stageMock
          .Expect (mock => mock.PrepareSqlStatement (Arg<QueryModel>.Matches (q => q.ResultOperators.Count == 1)))
          .Return (fakeSqlStatement);
      _stageMock.Replay();

      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      _stageMock.VerifyAllExpectations();

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.SameAs (fakeSqlStatement.SelectProjection));
    }

    [Test]
    public void VisitSubQueryExpression_WithContainsAndConstantCollection ()
    {
      var constantExpressionCollection = Expression.Constant (new[] { "Huber", "Maier" });
      var mainFromClause = new MainFromClause ("generated", typeof (string), constantExpressionCollection);
      var querModel = ExpressionHelper.CreateQueryModel (mainFromClause);

      var itemExpression = Expression.Constant ("Huber");
      var containsResultOperator = new ContainsResultOperator (itemExpression);
      querModel.ResultOperators.Add (containsResultOperator);

      var expression = new SubQueryExpression (querModel);
      var fakeConstantExpression = Expression.Constant ("Sepp");

      _stageMock
          .Expect (mock => mock.PrepareItemExpression (itemExpression))
          .Return (fakeConstantExpression);
      _stageMock.Replay();

      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlBinaryOperatorExpression)));
      Assert.That (((SqlBinaryOperatorExpression) result).BinaryOperator, Is.EqualTo ("IN"));
      Assert.That (((SqlBinaryOperatorExpression) result).LeftExpression, Is.EqualTo (fakeConstantExpression));
      Assert.That (((SqlBinaryOperatorExpression) result).RightExpression, Is.EqualTo (constantExpressionCollection));

      _stageMock.VerifyAllExpectations();
    }

    [Test]
    public void VisitSubQueryExpression_WithContainsAndEmptyConstantCollection ()
    {
      var constantExpressionCollection = Expression.Constant (new string[] { });
      var mainFromClause = new MainFromClause ("generated", typeof (string), constantExpressionCollection);
      var querModel = ExpressionHelper.CreateQueryModel (mainFromClause);

      var itemExpression = Expression.Constant ("Huber");
      var containsResultOperator = new ContainsResultOperator (itemExpression);
      querModel.ResultOperators.Add (containsResultOperator);

      var expression = new SubQueryExpression (querModel);
      var fakeConstantExpression = Expression.Constant ("Sepp");

      _stageMock
          .Expect (mock => mock.PrepareItemExpression (itemExpression))
          .Return (fakeConstantExpression);
      _stageMock.Replay ();

      var result = SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (((ConstantExpression) result).Value, Is.EqualTo(false));
      
      _stageMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException))]
    public void VisitSubQueryExpression_WithSeveralResultOperatorsAndConstantCollection ()
    {
      var constantExpressionCollection = Expression.Constant (new[] { "Huber", "Maier" });
      var mainFromClause = new MainFromClause ("generated", typeof (string), constantExpressionCollection);
      var querModel = ExpressionHelper.CreateQueryModel (mainFromClause);

      var itemExpression = Expression.Constant ("Huber");
      var containsResultOperator = new ContainsResultOperator (itemExpression);

      var resultOperator = new TakeResultOperator (Expression.Constant (1));
      querModel.ResultOperators.Add (resultOperator);
      querModel.ResultOperators.Add (containsResultOperator);

      var expression = new SubQueryExpression (querModel);
      
      SqlPreparationExpressionVisitor.TranslateExpression (expression, _context, _stageMock, _registry);
    }

    [Test]
    public void VisitBinaryExpression ()
    {
      var binaryExpression = Expression.And (Expression.Constant (1), Expression.Constant (1));
      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.SameAs (binaryExpression));
    }

    [Test]
    public void VisitBinaryExpression_QuerySourceReferenceExpressionsOnBothSide ()
    {
      var binaryExpression = Expression.Equal (_cookQuerySourceReferenceExpression, _cookQuerySourceReferenceExpression);
      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (BinaryExpression)));
      Assert.That (((BinaryExpression) result).Left, Is.TypeOf (typeof (SqlTableReferenceExpression)));
      Assert.That (((BinaryExpression) result).Right, Is.TypeOf (typeof (SqlTableReferenceExpression)));
    }

    [Test]
    public void VisitBinaryExpression_ReturnsSqlIsNullExpression_NullLeft ()
    {
      var leftExpression = Expression.Constant (null);
      var binaryExpression = Expression.Equal (leftExpression, _cookQuerySourceReferenceExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlIsNullExpression)));
      Assert.That (((SqlIsNullExpression) result).Expression, Is.TypeOf (typeof (SqlTableReferenceExpression)));
    }

    [Test]
    public void VisitBinaryExpression_ReturnsSqlIsNullExpression_NullRight ()
    {
      var leftExpression = Expression.Constant ("1");
      var rightExpression = Expression.Constant (null);
      var binaryExpression = Expression.Equal (leftExpression, rightExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlIsNullExpression)));
      Assert.That (((SqlIsNullExpression) result).Expression, Is.SameAs (leftExpression));
    }

    [Test]
    public void VisitBinaryExpression_ReturnsSqlIsNotNullExpression_NullLeft ()
    {
      var leftExpression = Expression.Constant (null);
      var rightExpression = Expression.Constant ("1");
      var binaryExpression = Expression.NotEqual (leftExpression, rightExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlIsNotNullExpression)));
      Assert.That (((SqlIsNotNullExpression) result).Expression, Is.SameAs (rightExpression));
    }

    [Test]
    public void VisitBinaryExpression_ReturnsSqlIsNotNullExpression_NullRight ()
    {
      var rightExpression = Expression.Constant (null);
      var leftExpression = Expression.Constant ("1");
      var binaryExpression = Expression.NotEqual (leftExpression, rightExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlIsNotNullExpression)));
      Assert.That (((SqlIsNotNullExpression) result).Expression, Is.SameAs (leftExpression));
    }

    [Test]
    public void VisitBinaryExpression_NotEqual_WithNullOnRightSide ()
    {
      var rightExpression = Expression.Constant (null);
      var binaryExpression = Expression.Coalesce (_cookQuerySourceReferenceExpression, rightExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (BinaryExpression)));
      Assert.That (((BinaryExpression) result).Left, Is.TypeOf (typeof (SqlTableReferenceExpression)));
      Assert.That (((BinaryExpression) result).Right, Is.TypeOf (typeof (ConstantExpression)));
    }

    [Test]
    public void VisitBinaryExpression_NotEqual_WithNullOnLeftSide ()
    {
      var leftExpression = Expression.Constant (null);
      var binaryExpression = Expression.Coalesce (leftExpression, _cookQuerySourceReferenceExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (BinaryExpression)));
      Assert.That (((BinaryExpression) result).Left, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (((BinaryExpression) result).Right, Is.TypeOf (typeof (SqlTableReferenceExpression)));
    }

    [Test]
    public void VisitBinaryExpression_WithConditionalExpression ()
    {
      var leftExpression = Expression.Constant ("Name");
      var testPredicate = Expression.Constant (true);
      var ifTrueExpression = Expression.Constant ("true");
      var ifFalseExpression = Expression.Constant ("false");
      var rightExpression = Expression.Condition (testPredicate, ifTrueExpression, ifFalseExpression);
      var binaryExpression = Expression.Equal (leftExpression, rightExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (binaryExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (BinaryExpression)));
      Assert.That (((BinaryExpression) result).Left, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (((BinaryExpression) result).Right, Is.TypeOf (typeof (SqlCaseExpression)));

      Assert.That (((SqlCaseExpression) ((BinaryExpression) result).Right).TestPredicate, Is.SameAs (testPredicate));
      Assert.That (((SqlCaseExpression) ((BinaryExpression) result).Right).ThenValue, Is.SameAs (ifTrueExpression));
      Assert.That (((SqlCaseExpression) ((BinaryExpression) result).Right).ElseValue, Is.SameAs (ifFalseExpression));
    }

    [Test]
    public void VisitMethodCallExpression ()
    {
      var method = typeof (string).GetMethod ("ToUpper", new Type[] { });
      var constantExpression = Expression.Constant ("Test");
      var methodCallExpression = Expression.Call (constantExpression, method);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (methodCallExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlFunctionExpression)));
      Assert.That (((SqlFunctionExpression) result).SqlFunctioName, Is.EqualTo ("UPPER"));
      Assert.That (((SqlFunctionExpression) result).Prefix, Is.SameAs (constantExpression));
      Assert.That (((SqlFunctionExpression) result).Args.Count, Is.EqualTo (0));
    }

    // TODO Review 2511: Add a test showing that the expression's properties are visited before being passed to the method transformer - use a MethodCallExpression whose instance or argument is the _cookQuerySourceReferenceExpression, and use a transformer mock to verify that a SqlTableReferenceExpression is passed instead of the _cookQuerySourceReferenceExpression
    // TODO Review 2511: Add a test showing that the transformed expression's properties are visited after the transformation - use a transformer mock that returns the _cookQuerySourceReferenceExpression, then verify that a SqlTableReferenceExpression is created for that

    // TODO Review 2511: The following two tests are not required in the SqlPreparationExpressionVisitor test fixture; they are integration tests and should thus be removed

    [Test]
    public void VisitMethodCallExpression_ConvertToInt64 ()
    {
      var method = typeof (Convert).GetMethod ("ToInt64", new[] { typeof (string) });
      var constantExpression = Expression.Constant ("1");
      var methodCallExpression = Expression.Call (constantExpression, method, constantExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (methodCallExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlConvertExpression)));
      Assert.That (result.Type, Is.EqualTo (typeof (Int64)));
    }

    [Test]
    public void VisitMethodCallExpression_ConvertToString ()
    {
      var method = typeof (Convert).GetMethod ("ToString", new[] { typeof (int) });
      var constantExpression = Expression.Constant (1);
      var methodCallExpression = Expression.Call (constantExpression, method, constantExpression);

      var result = SqlPreparationExpressionVisitor.TranslateExpression (methodCallExpression, _context, _stageMock, _registry);

      Assert.That (result, Is.TypeOf (typeof (SqlConvertExpression)));
      Assert.That (result.Type, Is.EqualTo (typeof (string)));
    }
  }
}