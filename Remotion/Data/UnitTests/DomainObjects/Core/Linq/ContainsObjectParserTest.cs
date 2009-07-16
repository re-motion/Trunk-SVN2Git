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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Backend.DetailParser;
using Remotion.Data.Linq.Backend.DetailParser.WhereConditionParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.UnitTests.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;
using ReflectionUtility=Remotion.Data.Linq.ReflectionUtility;

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
    private QueryModel _queryModel;

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

      _parser = new ContainsObjectParser (_registryStub);

      _queryModel = ExpressionHelper.ParseQuery (_query);
    }

    [Test]
    public void CreateEquivalentSubQuery_FromClauseIdentifier ()
    {
      SubQueryExpression subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;

      Assert.That (mainFromClause.ItemType, Is.EqualTo (typeof (OrderItem)));
      Assert.That (mainFromClause.ItemName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<<generated>>"));
    }

    [Test]
    public void CreateEquivalentSubQuery_FromClause_QuerySource ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      
      Assert.That (mainFromClause.FromExpression, Is.InstanceOfType (typeof (ConstantExpression)));
      object value = ((ConstantExpression) mainFromClause.FromExpression).Value;
      Assert.That (value, Is.InstanceOfType (typeof (DomainObjectQueryable<OrderItem>)));
    }

    [Test]
    public void CreateEquivalentSubQuery_WhereClause_CreatesBinaryEqualsExpression ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var whereClause = (WhereClause) subQuery.QueryModel.BodyClauses[0];

      Assert.That (whereClause.Predicate.NodeType, Is.EqualTo (ExpressionType.Equal));
    }

    [Test]
    public void CreateEquivalentSubQuery_WhereClause_CreatesBinaryEqualsExpression_LeftSide ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");
      
      var whereComparison = (BinaryExpression) ((WhereClause) subQuery.QueryModel.BodyClauses[0]).Predicate;
      var memberExpression = (MemberExpression) whereComparison.Left;

      Assert.That (whereComparison.Left, Is.InstanceOfType (typeof (MemberExpression)));
      Assert.That (((QuerySourceReferenceExpression) memberExpression.Expression).ReferencedClause, Is.SameAs (mainFromClause));
      Assert.That (memberExpression.Member, Is.SameAs (foreignKeyProperty));
    }

    [Test]
    public void CreateEquivalentSubQuery_SelectClause ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      var selectClause = subQuery.QueryModel.SelectClause;

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
    public void CreateEquivalentSubQuery_Clauses ()
    {
      var collectionElementType = _containsObjectCallExpression.Arguments[0].Type;
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;

      var whereClause = (WhereClause) subQuery.QueryModel.BodyClauses[0];
      var binaryExpression = (BinaryExpression) whereClause.Predicate;
      var memberExpression = (MemberExpression) binaryExpression.Left;
      var selectClause = subQuery.QueryModel.SelectClause;
      var fromClause = subQuery.QueryModel.MainFromClause;

      var queryModel1 = new QueryModel (mainFromClause, selectClause);
      queryModel1.BodyClauses.Add (whereClause);

      Assert.That (fromClause.ItemType, Is.EqualTo (typeof (OrderItem)));
      Assert.That (fromClause.ItemName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<<generated>>"));
      Assert.That (whereClause.Predicate, Is.InstanceOfType (typeof (BinaryExpression)));
      Assert.That (binaryExpression.Left, Is.InstanceOfType (typeof (MemberExpression)));
      Assert.That (((QuerySourceReferenceExpression) memberExpression.Expression).ReferencedClause, Is.SameAs (fromClause));
      Assert.That (memberExpression.Member, Is.EqualTo (typeof (OrderItem).GetProperty ("Order")));
      Assert.That (binaryExpression.Right, Is.SameAs (_queriedObjectExpression));
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedClause, Is.SameAs (fromClause));
    }

    [Test]
    public void CreateEquivalentSubQuery_ResultType ()
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", _containsObjectCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", ExpressionHelper.CreateQueryModel ());

      Type collectionElementType = _containsObjectCallExpression.Arguments[0].Type;

      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      var whereClause = (WhereClause) subQuery.QueryModel.BodyClauses[0];
      var selectClause = subQuery.QueryModel.SelectClause;

      var queryModel1 = new QueryModel (mainFromClause, selectClause);
      queryModel1.BodyClauses.Add (whereClause);
      QueryModel queryModel = queryModel1;

      Assert.That (queryModel.GetResultType(), Is.EqualTo (typeof (IQueryable<OrderItem>)));
    }

    [Test]
    public void CreateEquivalentSubQuery_CreatesSubQuery_WithQueryModel ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel ());

      Assert.That (subQuery.QueryModel, Is.Not.Null);
    }

    [Test]
    
    public void CreateExpressionForContainsParser ()
    {
      SubQueryExpression subQueryExpression = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, ExpressionHelper.CreateQueryModel ());
      Expression queryParameterExpression = Expression.Constant (null, typeof (OrderItem));
      MethodCallExpression methodCallExpression = _parser.CreateExpressionForContainsParser (subQueryExpression, queryParameterExpression);
      MethodInfo containsMethod = GetContainsMethod ();

      Assert.That (methodCallExpression.Object, Is.Null);
      Assert.That (methodCallExpression.Method, Is.EqualTo (containsMethod));
      Assert.That (methodCallExpression.Arguments, Is.EqualTo (new[] { subQueryExpression, queryParameterExpression }));
    }

    private MethodInfo GetContainsMethod ()
    {
      return ReflectionUtility.GetMethod (() => (from oi in QueryFactory.CreateLinqQuery<OrderItem>() select oi).Contains (_orderItem1));
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
          ExpressionHelper.CreateQueryModel(), 
          new List<FieldDescriptor>(), 
          new JoinedTableContext (StubDatabaseInfo.Instance));

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
