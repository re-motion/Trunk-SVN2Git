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
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.Legacy.QueryParserIntegrationTests
{
  [TestFixture]
  public class SimpleWhereQueryTest : SimpleQueryTest
  {
    protected override IQueryable<Student> CreateQuery ()
    {
      return WhereTestQueryGenerator.CreateSimpleWhereQuery(QuerySource);
    }

    [Test]
    public override void CheckBodyClauses ()
    {
      Assert.AreEqual (1, ParsedQuery.BodyClauses.Count);
      var whereClause = ParsedQuery.BodyClauses.First() as WhereClause;
      Assert.IsNotNull (whereClause);

      var navigator = new ExpressionTreeNavigator (whereClause.Predicate);
      Assert.IsNotNull (whereClause.Predicate);
      Assert.IsInstanceOfType (typeof (LambdaExpression), whereClause.Predicate);
      Assert.AreSame (ParsedQuery.MainFromClause.Identifier, navigator.Parameters[0].Expression);
    }

    [Test]
    public override void CheckSelectOrGroupClause ()
    {
      Assert.IsNotNull (ParsedQuery.SelectOrGroupClause);
      var clause = ParsedQuery.SelectOrGroupClause as SelectClause;
      Assert.IsNotNull (clause);

      Assert.That (clause.Selector.Parameters.Count, Is.EqualTo (1));
      Assert.That (clause.Selector.Parameters[0].Name, Is.EqualTo ("s"));
      Assert.That (clause.Selector.Parameters[0].Type, Is.EqualTo (typeof (Student)));
      Assert.That (clause.Selector.Body, Is.SameAs (clause.Selector.Parameters[0]));
    }
  }
}
