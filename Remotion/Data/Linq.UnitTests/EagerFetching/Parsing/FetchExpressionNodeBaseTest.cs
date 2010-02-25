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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.UnitTests.Parsing.Structure.IntermediateModel;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.EagerFetching.Parsing
{
  [TestFixture]
  public class FetchExpressionNodeBaseTest : ExpressionNodeTestBase
  {
    private TestFetchExpressionNodeBase _node;

    public override void SetUp ()
    {
      base.SetUp ();

      _node = new TestFetchExpressionNodeBase (CreateParseInfo (), ExpressionHelper.CreateLambdaExpression<Student, Student> (s => s.BuddyStudent));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        @"A fetch request must be a simple member access expression; 'new \[\] \{1, 2, 3\}' is a .* instead\.",
        MatchType = MessageMatch.Regex)]
    public void Initialization_InvalidExpression ()
    {
      var relatedObjectSelector = ExpressionHelper.CreateLambdaExpression<Student, IEnumerable<int>> (s => new[] { 1, 2, 3 });
      new TestFetchExpressionNodeBase (CreateParseInfo (), relatedObjectSelector);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "A fetch request must be a simple member access expression of the kind "
                                                                      + "o => o.Related; 's.BuddyStudent.Friends' is too complex.\r\nParameter name: relatedObjectSelector")]
    public void Initialization_InvalidExpression_MoreThanOneMember ()
    {
      var relatedObjectSelector = (Expression<Func<Student, IEnumerable<Student>>>) (s => s.BuddyStudent.Friends);
      new TestFetchExpressionNodeBase (CreateParseInfo (), relatedObjectSelector);
    }

    [Test]
    public void Resolve ()
    {
      var expression = ExpressionHelper.CreateLambdaExpression<Student, Student> (s => s);

      var result = _node.Resolve (expression.Parameters[0], expression.Body, ClauseGenerationContext);
      Assert.That (((QuerySourceReferenceExpression) result).ReferencedQuerySource, Is.SameAs (SourceClause));
    }
  }
}
