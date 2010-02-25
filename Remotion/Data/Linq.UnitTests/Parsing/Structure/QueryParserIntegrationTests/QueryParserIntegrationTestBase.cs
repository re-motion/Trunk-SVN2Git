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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Parsing.Structure.QueryParserIntegrationTests
{
  public abstract class QueryParserIntegrationTestBase
  {
    public IQueryable<Cook> QuerySource { get; private set; }
    public QueryParser QueryParser { get; private set; }
    public IQueryable<IndustrialSector> IndustrialSectorQuerySource { get; private set; }
    public IQueryable<Kitchen> DetailQuerySource { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      QuerySource = ExpressionHelper.CreateStudentQueryable ();
      IndustrialSectorQuerySource = ExpressionHelper.CreateIndustrialSectorQueryable();
      DetailQuerySource = ExpressionHelper.CreateStudentDetailQueryable ();
      QueryParser = new QueryParser ();
    }

    protected void CheckResolvedExpression<TParameter, TResult> (Expression expressionToCheck, IQuerySource clauseToReference, Expression<Func<TParameter, TResult>> expectedUnresolvedExpression)
    {
      var expectedPredicate = ExpressionHelper.Resolve (clauseToReference, expectedUnresolvedExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedPredicate, expressionToCheck);
    }

    protected void CheckResolvedExpression<TParameter1, TParameter2, TResult> (Expression expressionToCheck, IQuerySource clauseToReference1, IQuerySource clauseToReference2, Expression<Func<TParameter1, TParameter2, TResult>> expectedUnresolvedExpression)
    {
      var expectedPredicate = ExpressionHelper.Resolve (clauseToReference1, clauseToReference2, expectedUnresolvedExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedPredicate, expressionToCheck);
    }

    protected void CheckConstantQuerySource (Expression expression, object expectedQuerySource)
    {
      Assert.That (expression, Is.InstanceOfType (typeof (ConstantExpression)));
      Assert.That (((ConstantExpression) expression).Value, Is.SameAs (expectedQuerySource));
    }
  }
}
