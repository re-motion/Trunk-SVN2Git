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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.DataObjectModel;

namespace Remotion.Data.UnitTests.Linq.Clauses
{
  [TestFixture]
  public class FromClauseBaseTest
  {
    [Test]
    public void GetColumnSource ()
    {
      IQueryable querySource = ExpressionHelper.CreateQuerySource ();

      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause ("s1", typeof (Student), querySource);
      Assert.AreEqual (new Table ("studentTable", "s1"), fromClause.GetColumnSource (StubDatabaseInfo.Instance));
    }

    [Test]
    public void GetColumnSource_CachesInstance ()
    {
      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause_Student ();
      IColumnSource t1 = fromClause.GetColumnSource (StubDatabaseInfo.Instance);
      IColumnSource t2 = fromClause.GetColumnSource (StubDatabaseInfo.Instance);
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void GetColumnSource_SubQuery ()
    {
      var subQueryExpression = new SubQueryExpression (ExpressionHelper.CreateQueryModel ());
      var fromClause = new AdditionalFromClause ("s", typeof (Student), subQueryExpression);
      
      IColumnSource columnSource = fromClause.GetColumnSource (StubDatabaseInfo.Instance);
      var subQuery = (SubQuery) columnSource;
      Assert.That (subQuery.Alias, Is.EqualTo ("s"));
      Assert.That (subQuery.QueryModel, Is.SameAs (subQueryExpression.QueryModel));
    }

    [Test]
    public void AddJoinClause()
    {
      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause();

      JoinClause joinClause1 = ExpressionHelper.CreateJoinClause();
      JoinClause joinClause2 = ExpressionHelper.CreateJoinClause();

      fromClause.JoinClauses.Add (joinClause1);
      fromClause.JoinClauses.Add (joinClause2);

      Assert.That (fromClause.JoinClauses, Is.EqualTo (new object[] { joinClause1, joinClause2 }));
      Assert.AreEqual (2, fromClause.JoinClauses.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void AddJoinClause_Null_ThrowsArgumentNullException ()
    {
      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause ();
      fromClause.JoinClauses.Add (null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ChangeJoinClause_WithNull_ThrowsArgumentNullException ()
    {
      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause ();
      JoinClause joinClause = ExpressionHelper.CreateJoinClause ();
      fromClause.JoinClauses.Add (joinClause);
      fromClause.JoinClauses[0] = null;
    }

    [Test]
    public void TransformExpressions ()
    {
      var oldExpression = ExpressionHelper.CreateExpression ();
      var newExpression = ExpressionHelper.CreateExpression ();
      var clause = new MainFromClause ("x", typeof (Student), oldExpression);

      clause.TransformExpressions (ex =>
      {
        Assert.That (ex, Is.SameAs (oldExpression));
        return newExpression;
      });

      Assert.That (clause.FromExpression, Is.SameAs (newExpression));
    }
  }
}
