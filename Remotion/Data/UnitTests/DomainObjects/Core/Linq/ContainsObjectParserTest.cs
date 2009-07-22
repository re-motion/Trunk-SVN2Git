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
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.UnitTests.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.Linq.TestDomain;
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
    private WhereConditionParserRegistry _registryMock;
    private IWhereConditionParser _containsParserMock;
    private QueryModel _queryModel;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _registryMock =  _mockRepository.StrictMock<WhereConditionParserRegistry>(StubDatabaseInfo.Instance);
      _containsParserMock = _mockRepository.StrictMock<IWhereConditionParser> ();

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _query = (from o in QueryFactory.CreateLinqQuery<Order>()
                where o.OrderItems.ContainsObject (_orderItem1)
                select o);
      _containsObjectCallExpression = (MethodCallExpression) new ExpressionTreeNavigator (_query.Expression).Arguments[1].Operand.Body.Expression;
      _queriedObjectExpression =
          (ParameterExpression) new ExpressionTreeNavigator (_containsObjectCallExpression).Object.MemberAccess_Expression.Expression;

      _parser = new ContainsObjectParser (_registryMock);

      _queryModel = ExpressionHelper.ParseQuery (_query);
    }

    [Test]
    public void CreateEquivalentSubQuery_FromClause ()
    {
      SubQueryExpression subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;

      Assert.That (mainFromClause.ItemType, Is.EqualTo (typeof (OrderItem)));
      Assert.That (mainFromClause.ItemName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<<generated>>"));

      Assert.That (mainFromClause.FromExpression, Is.InstanceOfType (typeof (ConstantExpression)));
      object value = ((ConstantExpression) mainFromClause.FromExpression).Value;
      Assert.That (value, Is.InstanceOfType (typeof (DomainObjectQueryable<OrderItem>)));
    }

    [Test]
    public void CreateEquivalentSubQuery_WhereClause_CreatesBinaryEqualsExpression ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      var whereClause = (WhereClause) subQuery.QueryModel.BodyClauses[0];

      Assert.That (whereClause.Predicate.NodeType, Is.EqualTo (ExpressionType.Equal));

      var whereComparison = (BinaryExpression) ((WhereClause) subQuery.QueryModel.BodyClauses[0]).Predicate;
      var memberExpression = (MemberExpression) whereComparison.Left;

      PropertyInfo foreignKeyProperty = typeof (OrderItem).GetProperty ("Order");
      Assert.That (whereComparison.Left, Is.InstanceOfType (typeof (MemberExpression)));
      Assert.That (((QuerySourceReferenceExpression) memberExpression.Expression).ReferencedQuerySource, Is.SameAs (mainFromClause));
      Assert.That (memberExpression.Member, Is.SameAs (foreignKeyProperty));

      Assert.That (whereComparison.Right, Is.SameAs (_queriedObjectExpression));
    }

    [Test]
    public void CreateEquivalentSubQuery_SelectClause ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      var mainFromClause = subQuery.QueryModel.MainFromClause;
      var selectClause = subQuery.QueryModel.SelectClause;

      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedQuerySource, Is.SameAs (mainFromClause));
    }

    [Test]
    public void CreateEquivalentSubQuery_ResultType ()
    {
      var subQuery = _parser.CreateEquivalentSubQuery (_containsObjectCallExpression, _queryModel);
      Assert.That (subQuery.QueryModel.GetResultType(), Is.EqualTo (typeof (IQueryable<OrderItem>)));
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
      var expectedParseContext = new ParseContext (
          ExpressionHelper.CreateQueryModel(), 
          new List<FieldDescriptor>(), 
          new JoinedTableContext (StubDatabaseInfo.Instance));

      ICriterion expectedResult = new Constant (false);

      _registryMock
          .Expect (mock => mock.GetParser (Arg<Expression>.Matches (
              ex => ex is SubQueryExpression 
              && ((SubQueryExpression)ex).QueryModel.ResultOperators[0] is ContainsResultOperator)))
          .Return (_containsParserMock);

      _containsParserMock
          .Expect (mock => mock.Parse (
              Arg<Expression>.Matches (
                  ex => ex is SubQueryExpression
                  && ((SubQueryExpression) ex).QueryModel.ResultOperators[0] is ContainsResultOperator
                  && ((ContainsResultOperator) ((SubQueryExpression) ex).QueryModel.ResultOperators[0]).Item == _containsObjectCallExpression.Arguments[0]),
              Arg.Is (expectedParseContext)))
          .Return (expectedResult);

      _mockRepository.ReplayAll ();

      ICriterion result = _parser.Parse (_containsObjectCallExpression, expectedParseContext);
      Assert.That (result, Is.SameAs (expectedResult));
      _mockRepository.VerifyAll ();
    }
  }
}
