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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using System.Linq;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace Remotion.Linq.UnitTests.Linq.Core.Parsing.Structure.QueryParserIntegrationTests
{
  [TestFixture]
  public class MiscQueryParserIntegrationTest : QueryParserIntegrationTestBase
  {
    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = 
        "Could not parse expression 'c.Assistants.Select(value(System.Func`2[Remotion.Linq.UnitTests.Linq.Core.TestDomain.Cook,System.String]))': "
        + "Object of type 'System.Linq.Expressions.ConstantExpression' cannot be converted to type 'System.Linq.Expressions.LambdaExpression'. "
        + "If you tried to pass a delegate instead of a LambdaExpression, this is not supported because delegates are not parsable expressions.")]
    public void DelegateAsSelector ()
    {
      Func<Cook, string> expression = c => c.FirstName;
      var query = QuerySource.Select (c => c.Assistants.Select (expression));
      QueryParser.GetParsedQuery (query.Expression);
   }

    [Test]
    public void InvocationExpression_AppliedToLambdaExpression ()
    {
      Expression<Func<Cook, bool>> predicate1 = c => c.ID > 100;
      Expression<Func<Cook, bool>> predicate2 = c => c.Name != null;

      // c => c.ID > 100 && ((c1 => c1.Name != null) (c))
      var combinedPredicate =
          Expression.Lambda<Func<Cook, bool>> (
              Expression.AndAlso (
                  predicate1.Body,
                  Expression.Invoke (predicate2, predicate1.Parameters.Cast<Expression> ())
              ),
          predicate1.Parameters);

      var query = QuerySource.Where (combinedPredicate);

      var queryModel = QueryParser.GetParsedQuery (query.Expression);

      var predicate = ((WhereClause) queryModel.BodyClauses[0]).Predicate;
      CheckResolvedExpression<Cook, bool> (predicate, queryModel.MainFromClause, c => c.ID > 100 && c.Name != null);
    }

    [Test]
    public void InvocationExpression_AppliedToInlineLambdaExpression ()
    {
      var query = QuerySource.Where (c => ((Func<Cook, bool>) (c1 => c1.Name != null)) (c)).Select (c => c.FirstName);

      var queryModel = QueryParser.GetParsedQuery (query.Expression);

      var predicate = ((WhereClause) queryModel.BodyClauses[0]).Predicate;
      CheckResolvedExpression<Cook, bool> (predicate, queryModel.MainFromClause, c => c.Name != null);
    }

    [Test]
    public void NullableHasValue_ReplacedByNullCheck ()
    {
      var query = DetailQuerySource.Where (k => k.LastCleaningDay.HasValue);

      var queryModel = QueryParser.GetParsedQuery (query.Expression);

      var predicate = ((WhereClause) queryModel.BodyClauses[0]).Predicate;
      var expectedExpression =
          Expression.NotEqual (
              Expression.MakeMemberAccess (
                  new QuerySourceReferenceExpression (queryModel.MainFromClause), 
                  typeof (Kitchen).GetProperty ("LastCleaningDay")),
              Expression.Constant (null, typeof (DateTime?)));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, predicate);
    }

    [Test]
    public void NullableValue_ReplacedByCast ()
    {
// ReSharper disable PossibleInvalidOperationException
      var query = DetailQuerySource.Select (k => k.LastCleaningDay.Value);
// ReSharper restore PossibleInvalidOperationException

      var queryModel = QueryParser.GetParsedQuery (query.Expression);

      var selector = queryModel.SelectClause.Selector;
// ReSharper disable PossibleInvalidOperationException
      CheckResolvedExpression<Kitchen, DateTime> (selector, queryModel.MainFromClause, k => (DateTime) k.LastCleaningDay);
// ReSharper restore PossibleInvalidOperationException
    }

    [Test]
    public void ConstantReferenceToOtherQuery_IsInlined_AndPartiallyEvaluated ()
    {
      var query1 = from c in QuerySource where 1.ToString() == "1" select c;
      var query2 = from k in DetailQuerySource where query1.Contains (k.Cook) select k;

      // Handle this as if someone had written: from k in DetailQuerySource where (from c in QuerySource select c).Contains (k.Cook) select k;

      var queryModel = QueryParser.GetParsedQuery (query2.Expression);

      var whereClause = (WhereClause) queryModel.BodyClauses[0];
      Assert.That (whereClause.Predicate, Is.TypeOf (typeof (SubQueryExpression)));

      var subQuery = ((SubQueryExpression) whereClause.Predicate).QueryModel;

      CheckConstantQuerySource (subQuery.MainFromClause.FromExpression, QuerySource);
      CheckResolvedExpression<Cook, Cook> (subQuery.SelectClause.Selector, subQuery.MainFromClause, c => c);
      
      var subQueryWhereClause = (WhereClause) subQuery.BodyClauses[0];
      var expectedSubQueryWherePredicate = Expression.Constant (true);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSubQueryWherePredicate, subQueryWhereClause.Predicate);

      Assert.That (subQuery.ResultOperators.Count, Is.EqualTo (1));
      Assert.That (subQuery.ResultOperators[0], Is.TypeOf (typeof (ContainsResultOperator)));

      CheckResolvedExpression<Kitchen, Cook> (((ContainsResultOperator) subQuery.ResultOperators[0]).Item, queryModel.MainFromClause, k => k.Cook);
    }

    [Test]
    public void KeyValuePair_And_TupleCtors_GetMemberInfo ()
    {
      var query = from c in QuerySource 
                  select new 
                  { 
                    KVP = new KeyValuePair<string, int>(c.Name, 0),
                    DE = new DictionaryEntry (c.Name, 0),
                    Tuple = new Tuple<string, int> (c.Name, 0) 
                  };

      var queryModel = QueryParser.GetParsedQuery (query.Expression);

      var selector = (NewExpression) queryModel.SelectClause.Selector;
      Assert.That (selector.Arguments.Count, Is.EqualTo (3));
      Assert.That (selector.Members, Is.Not.Null);
      Assert.That (selector.Members.Count, Is.EqualTo (3));
      CheckMemberInNewExpression (selector.Type, "KVP", selector.Members[0]);
      CheckMemberInNewExpression (selector.Type, "DE", selector.Members[1]);
      CheckMemberInNewExpression (selector.Type, "Tuple", selector.Members[2]);

      var kvpArgument = (NewExpression) selector.Arguments[0];
      Assert.That (kvpArgument.Arguments.Count, Is.EqualTo (2));
      Assert.That (kvpArgument.Members, Is.Not.Null);
      Assert.That (kvpArgument.Members.Count, Is.EqualTo (2));
      CheckMemberInNewExpression (typeof(KeyValuePair<string, int>), "Key", kvpArgument.Members[0]);
      CheckMemberInNewExpression (typeof (KeyValuePair<string, int>), "Value", kvpArgument.Members[1]);

      var deArgument = (NewExpression) selector.Arguments[1];
      Assert.That (deArgument.Arguments.Count, Is.EqualTo (2));
      Assert.That (deArgument.Members, Is.Not.Null);
      Assert.That (deArgument.Members.Count, Is.EqualTo (2));
      CheckMemberInNewExpression (typeof (DictionaryEntry), "Key", deArgument.Members[0]);
      CheckMemberInNewExpression (typeof (DictionaryEntry), "Value", deArgument.Members[1]);

      var tupleArgument = (NewExpression) selector.Arguments[2];
      Assert.That (tupleArgument.Arguments.Count, Is.EqualTo (2));
      Assert.That (tupleArgument.Members, Is.Not.Null);
      Assert.That (tupleArgument.Members.Count, Is.EqualTo (2));
      CheckMemberInNewExpression (typeof (Tuple<string, int>), "Item1", tupleArgument.Members[0]);
      CheckMemberInNewExpression (typeof (Tuple<string, int>), "Item2", tupleArgument.Members[1]);
    }

    private void CheckMemberInNewExpression (Type expectedDeclaringType, string expectedPropertyName, MemberInfo actualMemberInfo)
    {
      var expectedProperty = expectedDeclaringType.GetProperty (expectedPropertyName);
      if (Environment.Version.Major < 4)
        Assert.That (actualMemberInfo, Is.EqualTo (expectedProperty.GetGetMethod()));
      else
        Assert.That (actualMemberInfo, Is.EqualTo (expectedProperty));
    }
  }
}
