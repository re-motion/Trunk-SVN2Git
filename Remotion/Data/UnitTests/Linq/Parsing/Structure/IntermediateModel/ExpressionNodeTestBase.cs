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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  public abstract class ExpressionNodeTestBase
  {
    [SetUp]
    public virtual void SetUp ()
    {
      SourceNode = ExpressionNodeObjectMother.CreateConstant();
      QuerySourceClauseMapping = new QuerySourceClauseMapping();
      SourceClause = (FromClauseBase) SourceNode.CreateClause (null, QuerySourceClauseMapping);
      SourceReference = new QuerySourceReferenceExpression (SourceClause);
    }

    public IQuerySourceExpressionNode SourceNode { get; private set; }
    public FromClauseBase SourceClause { get; private set; }
    public QuerySourceReferenceExpression SourceReference { get; private set; }
    public QuerySourceClauseMapping QuerySourceClauseMapping { get; private set; }

    public Expression<Func<int, string>> OptionalSelector
    {
      get { return (i => i.ToString ()); }
    }

    protected MethodInfo GetGenericMethodDefinition<TReturn> (Expression<Func<IQueryable<int>, TReturn>> methodCallLambda)
    {
      return GetMethod (methodCallLambda).GetGenericMethodDefinition ();
    }

    protected MethodInfo GetMethod<TReturn> (Expression<Func<IQueryable<int>, TReturn>> methodCallLambda)
    {
      var methodCallExpression = (MethodCallExpression) ExpressionHelper.MakeExpression (methodCallLambda);
      return methodCallExpression.Method;
    }


    protected void TestCreateClause_PreviousClauseIsSelect (IExpressionNode node, Type expectedResultModificationType)
    {
      var previousClause = ExpressionHelper.CreateSelectClause();

      var clause = (SelectClause) node.CreateClause (previousClause, QuerySourceClauseMapping);

      Assert.That (clause, Is.SameAs (previousClause));
      Assert.That (clause.ResultModifications.Count, Is.EqualTo (1));
      Assert.That (clause.ResultModifications[0], Is.InstanceOfType (expectedResultModificationType));
      Assert.That (clause.ResultModifications[0].SelectClause, Is.SameAs (clause));
    }

    protected void TestCreateClause_PreviousClauseIsNoSelect (IExpressionNode node, Type expectedResultModificationType)
    {
      var previousClause = SourceClause;

      var clause = (SelectClause) node.CreateClause (previousClause, QuerySourceClauseMapping);

      Assert.That (clause.PreviousClause, Is.SameAs (previousClause));
      Assert.That (clause.ResultModifications.Count, Is.EqualTo (1));
      Assert.That (clause.ResultModifications[0], Is.InstanceOfType (expectedResultModificationType));
      Assert.That (clause.ResultModifications[0].SelectClause, Is.SameAs (clause));

      var expectedLegacySelectorParameter = node.Source.CreateParameterForOutput ();
      var expectedLegacySelector = Expression.Lambda (expectedLegacySelectorParameter, expectedLegacySelectorParameter);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedLegacySelector, clause.LegacySelector);

      var expectedSelectorParameter = node.Source.CreateParameterForOutput ();
      var expectedSelector = node.Source.Resolve (expectedSelectorParameter, expectedSelectorParameter, QuerySourceClauseMapping);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedSelector, clause.Selector);
    }

    protected void TestCreateClause_WithOptionalPredicate (IExpressionNode node, LambdaExpression optionalPredicate)
    {
      var previousClause = ExpressionHelper.CreateSelectClause ();
      var previousPreviousClause = previousClause.PreviousClause;

      // chain: previousPreviousClause <- previousClause

      var clause = (SelectClause) node.CreateClause (previousClause, QuerySourceClauseMapping);

      // chain: previousPreviousClause <- whereClause <- previousClause

      Assert.That (clause, Is.SameAs (previousClause));
      Assert.That (clause.PreviousClause, Is.Not.SameAs (previousPreviousClause));
      var newWhereClause = (WhereClause) clause.PreviousClause;
      Assert.That (newWhereClause.PreviousClause, Is.SameAs (previousPreviousClause));
      Assert.That (newWhereClause.LegacyPredicate, Is.SameAs (optionalPredicate));
    }

    protected void TestCreateClause_WithOptionalSelector (IExpressionNode node)
    {
      var legacySelectorOfPreviousClause = ExpressionHelper.CreateLambdaExpression<Student, int> (s => s.ID);
      var expectedNewLegacySelector = ExpressionHelper.CreateLambdaExpression<Student, string> (s => s.ID.ToString ());

      var selectorOfPreviousClause = (MemberExpression) ExpressionHelper.MakeExpression<Student, int> (s => s.ID);
      var expectedNewSelector = (MethodCallExpression) ExpressionHelper.MakeExpression<Student, string> (s => s.ID.ToString());

      var previousPreviousClause = ExpressionHelper.CreateClause ();
      var previousClause = new SelectClause (previousPreviousClause, legacySelectorOfPreviousClause, selectorOfPreviousClause);

      var clause = (SelectClause) node.CreateClause (previousClause, QuerySourceClauseMapping);

      Assert.That (clause, Is.SameAs (previousClause));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedNewLegacySelector, clause.LegacySelector);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedNewSelector, clause.Selector);
    }

    protected MethodCallExpressionParseInfo CreateParseInfo ()
    {
      return CreateParseInfo (SourceNode);
    }

    protected MethodCallExpressionParseInfo CreateParseInfo (IExpressionNode source)
    {
      return CreateParseInfo (source, "x");
    }

    protected MethodCallExpressionParseInfo CreateParseInfo (IExpressionNode source, string associatedIdentifier)
    {
      return new MethodCallExpressionParseInfo (associatedIdentifier, source, ExpressionHelper.CreateMethodCallExpression ());
    }

    protected MethodCallExpressionParseInfo CreateParseInfo (MethodInfo method)
    {
      var arguments = from p in method.GetParameters ()
                      let t = p.ParameterType
                      let defaultValue = t.IsValueType ? Activator.CreateInstance (t) : null
                      select Expression.Constant (defaultValue, t);
      var methodCallExpression = Expression.Call (method, arguments.ToArray());

      return new MethodCallExpressionParseInfo ("x", SourceNode, methodCallExpression);
    }
  }
}