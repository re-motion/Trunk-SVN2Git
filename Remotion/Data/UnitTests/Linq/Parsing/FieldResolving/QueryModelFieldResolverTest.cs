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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing.FieldResolving;

namespace Remotion.Data.UnitTests.Linq.Parsing.FieldResolving
{
  [TestFixture]
  public class QueryModelFieldResolverTest
  {
    private ClauseFieldResolver _resolver;
    private WhereFieldAccessPolicy _policy;
    private JoinedTableContext _context;

    [SetUp]
    public void SetUp ()
    {
      _policy = new WhereFieldAccessPolicy (StubDatabaseInfo.Instance);
      _context = new JoinedTableContext();
      _resolver = new ClauseFieldResolver (StubDatabaseInfo.Instance, _policy);
    }

    [Test]
    public void ResolveField_MainFromClause ()
    {
      QueryModel queryModel = CreateQueryExpressionForResolve ();

      Expression fieldAccessExpression = Expression.Parameter (typeof (String), "s1");
      FieldDescriptor descriptor = new QueryModelFieldResolver(queryModel).ResolveField (_resolver, fieldAccessExpression, _context);

      IColumnSource expectedTable = queryModel.MainFromClause.GetColumnSource (StubDatabaseInfo.Instance);
      var expectedPath = new FieldSourcePath(expectedTable, new SingleJoin[0]);

      //Assert.AreSame (queryModel.MainFromClause, descriptor.FromClause);
      Assert.AreEqual (new Column (expectedTable, "*"), descriptor.Column);
      Assert.IsNull (descriptor.Member);
      Assert.AreEqual (expectedPath, descriptor.SourcePath);
    }

    [Test]
    [ExpectedException (typeof (FieldAccessResolveException), ExpectedMessage = "The field access expression 'fzlbf' does "
        + "not contain a from clause identifier.")]
    public void NoFromIdentifierFound ()
    {
      QueryModel queryModel = CreateQueryExpressionForResolve ();
      Expression sourceExpression = Expression.Parameter (typeof (Student), "fzlbf");

      new QueryModelFieldResolver (queryModel).ResolveField (_resolver, sourceExpression, _context);
    }

    [Test]
    public void ResolveInParentQuery ()
    {
      QueryModel parentQueryModel = CreateQueryExpressionForResolve ();
      QueryModel subQueryModel =
          ExpressionHelper.CreateQueryModel (new MainFromClause (Expression.Parameter (typeof (Student), "a"), Expression.Constant (null)));
      subQueryModel.SetParentQuery (parentQueryModel);
      Expression sourceExpression = Expression.Parameter (typeof (string), "s1");

      var fieldResolver = new QueryModelFieldResolver (subQueryModel);

      FieldDescriptor fieldDescriptor = fieldResolver.ResolveField (_resolver, sourceExpression, _context);
      Assert.AreEqual (parentQueryModel.MainFromClause.JoinClauses, fieldDescriptor.SourcePath.Joins);
    }

    private QueryModel CreateQueryExpressionForResolve ()
    {
      ParameterExpression s1 = Expression.Parameter (typeof (String), "s1");
      ParameterExpression s2 = Expression.Parameter (typeof (String), "s2");
      MainFromClause mainFromClause = ExpressionHelper.CreateMainFromClause(s1, ExpressionHelper.CreateQuerySource ());
      var additionalFromClause = new AdditionalFromClause (mainFromClause, s2, ExpressionHelper.CreateExpression ());

      var expression = ExpressionHelper.CreateQueryModel (mainFromClause);
      
      expression.AddBodyClause (additionalFromClause);

      return expression;
    }
  }
}
