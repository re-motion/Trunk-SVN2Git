// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Linq.Clauses;
using Remotion.Linq.UnitTests.TestDomain;
using Remotion.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Linq.UnitTests.Parsing.Structure.QueryParserIntegrationTests
{
  [TestFixture]
  public class OrderByQueryParserIntegrationTest : QueryParserIntegrationTestBase
  {
    [Test]
    public void OrderByAndThenBy ()
    {
      var expression = OrderByTestQueryGenerator.CreateOrderByQueryWithOrderByAndThenBy (QuerySource).Expression;
      var queryModel = QueryParser.GetParsedQuery (expression);

      var mainFromClause = queryModel.MainFromClause;
      Assert.That (mainFromClause.ItemName, Is.EqualTo ("s"));
      CheckConstantQuerySource (mainFromClause.FromExpression, QuerySource);

      var orderByClause = (OrderByClause) queryModel.BodyClauses[0];
      Assert.That (orderByClause.Orderings.Count, Is.EqualTo (3));

      var ordering1 = orderByClause.Orderings[0];
      Assert.That (ordering1.OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
      CheckResolvedExpression<Cook, string> (ordering1.Expression, mainFromClause, s => s.FirstName);

      var ordering2 = orderByClause.Orderings[1];
      Assert.That (ordering2.OrderingDirection, Is.EqualTo (OrderingDirection.Desc));
      CheckResolvedExpression<Cook, string> (ordering2.Expression, mainFromClause, s => s.Name);

      var ordering3 = orderByClause.Orderings[2];
      Assert.That (ordering3.OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
      CheckResolvedExpression<Cook, List<int>> (ordering3.Expression, mainFromClause, s => s.Holidays);

      var selectClause = queryModel.SelectClause;
      CheckResolvedExpression<Cook, Cook> (selectClause.Selector, queryModel.MainFromClause, s => s);
    }

    [Test]
    public void MultipleOrderBys ()
    {
      var expression = OrderByTestQueryGenerator.CreateOrderByQueryWithMultipleOrderBys (QuerySource).Expression;
      var queryModel = QueryParser.GetParsedQuery (expression);

      var mainFromClause = queryModel.MainFromClause;
      CheckConstantQuerySource (mainFromClause.FromExpression, QuerySource);
      
      var orderByClause1 = (OrderByClause) queryModel.BodyClauses[0];
      Assert.That (orderByClause1.Orderings.Count, Is.EqualTo (3));

      var orderByClause2 = (OrderByClause) queryModel.BodyClauses[1];
      Assert.That (orderByClause2.Orderings.Count, Is.EqualTo (1));
    }

    [Test]
    public void OrderByAndWhere ()
    {
      var expression = MixedTestQueryGenerator.CreateOrderByWithWhereCondition (QuerySource).Expression;
      var queryModel = QueryParser.GetParsedQuery (expression);

      var whereClause = (WhereClause) queryModel.BodyClauses[0];
      CheckResolvedExpression<Cook, bool> (whereClause.Predicate, queryModel.MainFromClause, s1 => s1.FirstName == "Garcia");

      var orderByClause = (OrderByClause) queryModel.BodyClauses[1];
      CheckResolvedExpression<Cook, string> (orderByClause.Orderings[0].Expression, queryModel.MainFromClause, s1 => s1.FirstName);
    }

    [Test]
    public void MultiFromsWithOrderBy ()
    {
      var expression = MixedTestQueryGenerator.CreateMultiFromWhereOrderByQuery (QuerySource, QuerySource).Expression;
      var queryModel = QueryParser.GetParsedQuery (expression);
      
      var additionalFromClause = (AdditionalFromClause) queryModel.BodyClauses[0];
      CheckConstantQuerySource (additionalFromClause.FromExpression, QuerySource);

      var whereClause = (WhereClause) queryModel.BodyClauses[1];
      CheckResolvedExpression<Cook, bool> (whereClause.Predicate, queryModel.MainFromClause, s1 => s1.Name == "Garcia");

      var orderByClause = (OrderByClause) queryModel.BodyClauses[2];
      Assert.That (orderByClause.Orderings[0].OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
      CheckResolvedExpression<Cook, string> (orderByClause.Orderings[0].Expression, queryModel.MainFromClause, s1 => s1.FirstName);
      Assert.That (orderByClause.Orderings[1].OrderingDirection, Is.EqualTo (OrderingDirection.Desc));
      CheckResolvedExpression<Cook, string> (orderByClause.Orderings[1].Expression, additionalFromClause, s2 => s2.Name);
    }
  }
}
