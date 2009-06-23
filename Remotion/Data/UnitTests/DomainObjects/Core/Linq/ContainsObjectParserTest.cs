// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.Parsing.Details.WhereConditionParsing;
using Remotion.Data.Linq.Parsing.FieldResolving;
using Remotion.Data.UnitTests.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class ContainsObjectParserTest : ClientTransactionBaseTest
  {
    private OrderItem _orderItem1;
    private IQueryable<Order> _query;
    private MethodCallExpression _containsObjectCallExpression;
    private ContainsObjectParser _parser;
    private ParameterExpression _queriedObjectExpression;

    private MockRepository _mockRepository;
    private WhereConditionParserRegistry _registryStub;
    private IWhereConditionParser _containsParserMock;
    private QuerySourceReferenceExpression _orderReference;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _registryStub =  _mockRepository.Stub<WhereConditionParserRegistry>(StubDatabaseInfo.Instance);
      _containsParserMock = _mockRepository.StrictMock<IWhereConditionParser> ();

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _query = GetQuery();
      _containsObjectCallExpression = (MethodCallExpression) new ExpressionTreeNavigator (_query.Expression).Arguments[1].Operand.Body.Expression;
      _queriedObjectExpression =
          (ParameterExpression) new ExpressionTreeNavigator (_containsObjectCallExpression).Object.MemberAccess_Expression.Expression;

      _orderReference = new QuerySourceReferenceExpression (ExpressionHelper.CreateMainFromClause (Expression.Parameter (typeof (Order), "o"), _query));

      _parser = new ContainsObjectParser (_registryStub);
    }

    [Test]
    public void CreateFromClause_Identifier ()
    {
      MainFromClause mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      Assert.That (mainFromClause.Identifier.Type, Is.EqualTo (typeof (OrderItem)));
      Assert.That (mainFromClause.Identifier.Name, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<<generated>>"));
    }

    [Test]
    public void CreateFromClause_QuerySource ()
    {
      MainFromClause mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      Assert.That (mainFromClause.QuerySource, Is.InstanceOfType (typeof (ConstantExpression)));
      object value = ((ConstantExpression) mainFromClause.QuerySource).Value;
      Assert.That (value, Is.InstanceOfType (typeof (DomainObjectQueryable<OrderItem>)));
    }

    [Test]
    public void CreateWhereComparison_CreatesBinaryEqualsExpression ()
    {
      var mainFromClause = _parser.CreateFromClause (typeof (OrderItem));

      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");
      Expression whereComparison = _parser.CreateWhereClause (mainFromClause, foreignKeyProperty, _orderReference).Predicate;

      Assert.That (whereComparison.NodeType, Is.EqualTo (ExpressionType.Equal));
    }

    [Test]
    public void CreateWhereComparison_CreatesBinaryEqualsExpression_LeftSide ()
    {
      var mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");

      var whereComparison = (BinaryExpression) _parser.CreateWhereClause (mainFromClause, foreignKeyProperty, _orderReference).Predicate;
      
      Assert.That (whereComparison.Left, Is.InstanceOfType (typeof (MemberExpression)));
      var memberExpression = (MemberExpression) whereComparison.Left;
      Assert.That (((QuerySourceReferenceExpression) memberExpression.Expression).ReferencedClause, Is.SameAs (mainFromClause));
      Assert.That (memberExpression.Member, Is.SameAs (foreignKeyProperty));
    }

    [Test]
    public void CreateWhereComparison_CreatesBinaryEqualsExpression_RightSide ()
    {
      var mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");

      var whereComparison = (BinaryExpression) _parser.CreateWhereClause (mainFromClause, foreignKeyProperty, _orderReference).Predicate;
      Assert.That (whereComparison.Right, Is.SameAs (_orderReference));
    }

    [Test]
    public void CreateWhereClause ()
    {
      MainFromClause mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");

      WhereClause whereClause = _parser.CreateWhereClause (mainFromClause, foreignKeyProperty, _orderReference);
      Assert.That (whereClause.PreviousClause, Is.SameAs (mainFromClause));
    }

    [Test]
    public void CreateSelectClause ()
    {
      MainFromClause mainFromClause = _parser.CreateFromClause (typeof (OrderItem));
      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");

      WhereClause whereClause = _parser.CreateWhereClause (mainFromClause, foreignKeyProperty, _orderReference);

      SelectClause selectClause = _parser.CreateSelectClause (whereClause, mainFromClause);
      Assert.That (selectClause.PreviousClause, Is.SameAs (whereClause));
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedClause, Is.SameAs (mainFromClause));
    }

    [Test]
    public void GetForeignKeyProperty ()
    {
      PropertyInfo collectionProperty = typeof (Order).GetProperty ("OrderItems");
      PropertyInfo expectedForeignKey = typeof (OrderItem).GetProperty ("Order");
      Assert.That (_parser.GetForeignKeyProperty (collectionProperty), Is.EqualTo (expectedForeignKey));
    }

    [Test]
    public void CreateQueryModel_Clauses ()
    {
      QueryModel queryModel = _parser.CreateQueryModel (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel (), Expression.Constant(0));

      MainFromClause fromClause = queryModel.MainFromClause;
      Assert.That (fromClause.Identifier.Type, Is.EqualTo (typeof (OrderItem)));
      Assert.That (fromClause.Identifier.Name, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<<generated>>"));

      var whereClause = (WhereClause) queryModel.BodyClauses[0];
      Assert.That (whereClause.Predicate, Is.InstanceOfType (typeof (BinaryExpression)));
      var binaryExpression = (BinaryExpression) whereClause.Predicate;
      Assert.That (binaryExpression.Left, Is.InstanceOfType (typeof (MemberExpression)));
      var memberExpression = (MemberExpression) binaryExpression.Left;
      Assert.That (((QuerySourceReferenceExpression) memberExpression.Expression).ReferencedClause, Is.SameAs (fromClause));
      Assert.That (memberExpression.Member, Is.EqualTo (typeof (OrderItem).GetProperty ("Order")));
      Assert.That (binaryExpression.Right, Is.SameAs (_queriedObjectExpression));

      var selectClause = (SelectClause) queryModel.SelectOrGroupClause;
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedClause, Is.SameAs (fromClause));
    }

    [Test]
    public void CreateQueryModel_ResultType ()
    {
      QueryModel queryModel = _parser.CreateQueryModel (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel (), Expression.Constant (0));
      Assert.That (queryModel.ResultType, Is.EqualTo (typeof (IQueryable<OrderItem>)));
    }

    [Test]
    public void CreateQueryModel_ParentQuery ()
    {
      QueryModel parentQuery = ExpressionHelper.CreateQueryModel();
      QueryModel queryModel = _parser.CreateQueryModel (_containsObjectCallExpression, parentQuery, Expression.Constant (0));
      Assert.That (queryModel.ParentQuery, Is.SameAs (parentQuery));
    }

    [Test]
    public void CreateEquivalentSubQuery_CreatesSubQuery_WithQueryModel ()
    {
      SubQueryExpression subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel (), Expression.Constant (0));
      Assert.That (subQuery.QueryModel, Is.Not.Null);
    }

    [Test]
    public void CreateExpressionForContainsParser ()
    {
      SubQueryExpression subQueryExpression1 = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel (), Expression.Constant (0));
      Expression queryParameterExpression = Expression.Constant (null, typeof (OrderItem));
      MethodCallExpression methodCallExpression = _parser.CreateExpressionForContainsParser (subQueryExpression1, queryParameterExpression);
      Assert.That (methodCallExpression.Object, Is.Null);
      MethodInfo containsMethod = GetContainsMethod();
      Assert.That (methodCallExpression.Method, Is.EqualTo (containsMethod));
      Assert.That (methodCallExpression.Arguments, Is.EqualTo (new[] { subQueryExpression1, queryParameterExpression }));
    }

    private MethodInfo GetContainsMethod ()
    {
      return ParserUtility.GetMethod (() => (from oi in QueryFactory.CreateLinqQuery<OrderItem>() select oi).Contains (_orderItem1));
    }

    [Test]
    public void CanParse ()
    {
      Assert.That (_parser.CanParse (_containsObjectCallExpression), Is.True);
      Assert.That (_parser.CanParse (Expression.Constant (0)), Is.False);
      Assert.That (_parser.CanParse (Expression.Call (typeof (DateTime).GetMethod ("get_Now"))), Is.False);
    }

    [Test]
    public void Parse ()
    {
      ICriterion expectedResult = new Constant (false);

      SetupResult.For (_registryStub.GetParser (null)).IgnoreArguments().Return (_containsParserMock);
      Func<Expression, ParseContext, ICriterion> action = (expression, parseContext) =>
      {
        Assert.That (expression, Is.InstanceOfType (typeof (MethodCallExpression)));
        Assert.That (((MethodCallExpression) expression).Method, Is.EqualTo (GetContainsMethod()));
        Assert.That (parseContext, Is.SameAs (parseContext));
        return expectedResult;
      };
      Expect.Call (_containsParserMock.Parse (null, null))
          .IgnoreArguments()
          .Do (action);

      _mockRepository.ReplayAll ();
      var expectedParseContext = new ParseContext (
          ExpressionHelper.CreateQueryModel(), _query.Expression, new List<FieldDescriptor>(), new JoinedTableContext());
      ICriterion result = _parser.Parse (_containsObjectCallExpression, expectedParseContext);
      Assert.That (result, Is.SameAs (expectedResult));
      _mockRepository.VerifyAll ();
    }

    private IQueryable<Order> GetQuery ()
    {
      return from o in QueryFactory.CreateLinqQuery<Order>()
             where o.OrderItems.ContainsObject (_orderItem1)
             select o;
    }
  }
}
