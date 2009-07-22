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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.QueryParserIntegrationTests
{
  [TestFixture]
  public class GroupByQueryParserIntegrationTest : QueryParserIntegrationTestBase
  {
    [Test]
    public void GroupBy ()
    {
      var query = (from s in QuerySource group s.ID by s.HasDog);
      
      var queryModel = QueryParser.GetParsedQuery (query.Expression);
      Assert.That (queryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, int>>)));

      var mainFromClause = queryModel.MainFromClause;
      CheckConstantQuerySource (mainFromClause.FromExpression, QuerySource);
      Assert.That (mainFromClause.ItemType, Is.SameAs (typeof (Student)));
      Assert.That (mainFromClause.ItemName, Is.EqualTo ("s"));

      var selectClause = queryModel.SelectClause;
      CheckResolvedExpression<Student, Student> (selectClause.Selector, mainFromClause, s => s);

      var groupResultOperator = (GroupResultOperator) queryModel.ResultOperators[0];
      CheckResolvedExpression<Student, bool> (groupResultOperator.KeySelector.ResolvedExpression, mainFromClause, s => s.HasDog);
      CheckResolvedExpression<Student, int> (groupResultOperator.ElementSelector.ResolvedExpression, mainFromClause, s => s.ID);
    }

    [Test]
    public void GroupByWithoutElementSelector ()
    {
      var query = QuerySource.GroupBy (s => s.HasDog);
      
      var queryModel = QueryParser.GetParsedQuery (query.Expression);
      Assert.That (queryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, Student>>)));

      var mainFromClause = queryModel.MainFromClause;
      CheckConstantQuerySource (mainFromClause.FromExpression, QuerySource);
      Assert.That (mainFromClause.ItemType, Is.SameAs (typeof (Student)));
      Assert.That (mainFromClause.ItemName, Is.EqualTo ("s"));

      var selectClause = queryModel.SelectClause;
      CheckResolvedExpression<Student, Student> (selectClause.Selector, mainFromClause, s => s);

      var groupResultOperator = (GroupResultOperator) queryModel.ResultOperators[0];
      CheckResolvedExpression<Student, bool> (groupResultOperator.KeySelector.ResolvedExpression, mainFromClause, s => s.HasDog);
      CheckResolvedExpression<Student, Student> (groupResultOperator.ElementSelector.ResolvedExpression, mainFromClause, s => s);
    }

    [Test]
    public void GroupIntoWithAggregate ()
    {
      var query = from s in QuerySource 
                  group s.ID by s.HasDog 
                  into x 
                      where x.Count() > 0
                      select x;

      // equivalent to:
      //var query2 = from x in
      //               (from s in _querySource
      //                group s.ID by s.HasDog)
      //             where x.Count () > 0
      //             select x;

      // parsed as:
      //var query2 = from x in
      //               (from s in _querySource
      //                group s.ID by s.HasDog)
      //             where (from generated in x select generated).Count () > 0
      //             select x;

      var queryModel = QueryParser.GetParsedQuery (query.Expression);
      Assert.That (queryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, int>>)));

      var mainFromClause = queryModel.MainFromClause;
      Assert.That (mainFromClause.FromExpression, Is.InstanceOfType (typeof (SubQueryExpression)));
      Assert.That (mainFromClause.ItemType, Is.SameAs (typeof (IGrouping<bool, int>)));
      Assert.That (mainFromClause.ItemName, Is.EqualTo ("x"));

      var subQueryModel = ((SubQueryExpression) mainFromClause.FromExpression).QueryModel;
      var subQuerySelectClause = queryModel.SelectClause;
      CheckResolvedExpression<Student, Student> (subQuerySelectClause.Selector, mainFromClause, s => s);
      
      var subQueryGroupResultOperator = (GroupResultOperator) subQueryModel.ResultOperators[0];
      CheckResolvedExpression<Student, bool> (subQueryGroupResultOperator.KeySelector.ResolvedExpression, subQueryModel.MainFromClause, s => s.HasDog);
      CheckResolvedExpression<Student, int> (subQueryGroupResultOperator.ElementSelector.ResolvedExpression, subQueryModel.MainFromClause, s => s.ID);
      
      Assert.That (subQueryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, int>>)));

      var whereClause = (WhereClause) queryModel.BodyClauses[0];
      Assert.That (whereClause.Predicate, Is.InstanceOfType (typeof (BinaryExpression)));
      var predicateLeftSide = ((BinaryExpression) whereClause.Predicate).Left;
      Assert.That (predicateLeftSide, Is.InstanceOfType (typeof (SubQueryExpression)));
      var predicateSubQueryModel = ((SubQueryExpression) predicateLeftSide).QueryModel;
      Assert.That (predicateSubQueryModel.MainFromClause.ItemType, Is.SameAs (typeof (int)));
      Assert.That (predicateSubQueryModel.MainFromClause.ItemName, NUnit.Framework.SyntaxHelpers.Text.StartsWith ("<generated>"));
      Assert.That (((QuerySourceReferenceExpression) predicateSubQueryModel.MainFromClause.FromExpression).ReferencedQuerySource, Is.SameAs (mainFromClause));
      Assert.That (((QuerySourceReferenceExpression) predicateSubQueryModel.SelectClause.Selector).ReferencedQuerySource, 
                   Is.SameAs (predicateSubQueryModel.MainFromClause));
      Assert.That (predicateSubQueryModel.ResultOperators[0], Is.InstanceOfType (typeof (CountResultOperator)));
      
      var selectClause = queryModel.SelectClause;
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedQuerySource, Is.SameAs (mainFromClause));
    }

    [Test]
    public void GroupByFollowedByWhere ()
    {
      var query = (from s in ExpressionHelper.CreateStudentQueryable ()
                   group s by s.HasDog).Where (g => g.Key);

      var queryModel = QueryParser.GetParsedQuery (query.Expression);
      Assert.That (queryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, Student>>)));

      var mainFromClause = queryModel.MainFromClause;
      Assert.That (mainFromClause.FromExpression, Is.InstanceOfType (typeof (SubQueryExpression)));
      Assert.That (mainFromClause.ItemType, Is.SameAs (typeof (IGrouping<bool, Student>)));
      Assert.That (mainFromClause.ItemName, Is.EqualTo ("g"));

      var subQueryModel = ((SubQueryExpression) mainFromClause.FromExpression).QueryModel;
      var subQuerySelectClause = subQueryModel.SelectClause;
      CheckResolvedExpression<Student, Student> (subQuerySelectClause.Selector, subQueryModel.MainFromClause, s => s);

      Assert.That (subQueryModel.ResultOperators[0], Is.InstanceOfType (typeof (GroupResultOperator)));

      Assert.That (subQueryModel.GetResultType(), Is.SameAs (typeof (IQueryable<IGrouping<bool, Student>>)));

      var whereClause = (WhereClause) queryModel.BodyClauses[0];
      CheckResolvedExpression<IGrouping<bool, Student>, bool> (whereClause.Predicate, mainFromClause, g => g.Key);

      var selectClause = queryModel.SelectClause;
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedQuerySource, Is.SameAs (mainFromClause));
    }
  }
}