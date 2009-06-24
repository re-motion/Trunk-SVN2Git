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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class ThenByExpressionNodeTest : ExpressionNodeTestBase
  {
    [Test]
    public void SupportedMethod_WithoutComparer ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => ((IOrderedQueryable<object>) q).ThenBy (i => i));
      Assert.That (ThenByExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void Resolve_PassesExpressionToSource ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode>();
      var selector = ExpressionHelper.CreateLambdaExpression<int, int> (i => i);
      var node = new ThenByExpressionNode (CreateParseInfo (sourceMock), selector);
      var expression = ExpressionHelper.CreateLambdaExpression();
      var parameter = ExpressionHelper.CreateParameterExpression();
      var expectedResult = ExpressionHelper.CreateExpression();
      sourceMock.Expect (mock => mock.Resolve (parameter, expression, ClauseGenerationContext)).Return (expectedResult);

      var result = node.Resolve (parameter, expression, ClauseGenerationContext);

      sourceMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    public void CreateParameterForOutput ()
    {
      var source = new ConstantExpressionNode ("x", typeof (int[]), new[] { 1, 2, 3, 4, 5 });
      var node = new ThenByExpressionNode (CreateParseInfo (source, "y"), ExpressionHelper.CreateLambdaExpression<int, int> (i => i));
      var parameter = node.CreateParameterForOutput ();

      Assert.That (parameter.Name, Is.EqualTo ("x"));
      Assert.That (parameter.Type, Is.SameAs (typeof (int)));
    }

    [Test]
    public void GetResolvedSelector ()
    {
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new ThenByExpressionNode (CreateParseInfo (), selector);

      var expectedResult = Expression.MakeBinary (ExpressionType.GreaterThan, SourceReference, Expression.Constant (5));

      var result = node.GetResolvedKeySelector (ClauseGenerationContext);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void CreateClause ()
    {
      var previousClause = ExpressionHelper.CreateOrderByClause();
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new ThenByExpressionNode (CreateParseInfo (), selector);
      var oldCount = previousClause.Orderings.Count;

      var clause = (OrderByClause) node.CreateClause (previousClause, ClauseGenerationContext);

      Assert.That (clause, Is.SameAs (previousClause));
      Assert.That (clause.Orderings.Count, Is.EqualTo (oldCount + 1));
      Assert.That (clause.Orderings.Last ().OrderingDirection, Is.EqualTo (OrderingDirection.Asc));
      Assert.That (clause.Orderings.Last ().Expression, Is.SameAs (node.GetResolvedKeySelector (ClauseGenerationContext)));
      Assert.That (clause.Orderings.Last ().OrderByClause, Is.SameAs (previousClause));
    }

    [Test]
    [ExpectedException(typeof(ArgumentTypeException))]
    public void CreateClause_InvalidPreviousClause ()
    {
      var previousClause = ExpressionHelper.CreateMainFromClause();
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new ThenByExpressionNode (CreateParseInfo (), selector);

      node.CreateClause (previousClause, ClauseGenerationContext);
    }

    [Test]
    public void CreateSelectClause ()
    {
      var previousClause = ExpressionHelper.CreateClause ();
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new ThenByExpressionNode (CreateParseInfo (), selector);

      var selectClause = node.CreateSelectClause (previousClause, ClauseGenerationContext);
      Assert.That (((QuerySourceReferenceExpression) selectClause.Selector).ReferencedClause, Is.SameAs (SourceClause));
    }
  }
}