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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class SelectExpressionNodeTest : ExpressionNodeTestBase
  {
    private SelectExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();

      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      _node = new SelectExpressionNode (CreateParseInfo (), selector);
    }

    [Test]
    public void SupportedMethod_WithoutPosition ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.Select (i => i.ToString()));
      Assert.That (SelectExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void Resolve_ReplacesParameter_WithProjection ()
    {
      var node = new SelectExpressionNode (CreateParseInfo (), ExpressionHelper.CreateLambdaExpression<int, int> (j => j * j));
      var expression = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);

      var result = node.Resolve (expression.Parameters[0], expression.Body, ClauseGenerationContext);

      var expectedResult = Expression.MakeBinary (
          ExpressionType.GreaterThan,
          Expression.MakeBinary (ExpressionType.Multiply, SourceReference, SourceReference),
          Expression.Constant (5));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void GetResolvedSelector ()
    {
      var selector = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new SelectExpressionNode (CreateParseInfo (), selector);

      var expectedResult = Expression.MakeBinary (ExpressionType.GreaterThan, SourceReference, Expression.Constant (5));

      var result = node.GetResolvedSelector(ClauseGenerationContext);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void Apply ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);
      var selectClause = (SelectClause) QueryModel.SelectOrGroupClause;

      Assert.That (selectClause.ResultModifications, Is.Empty);
      Assert.That (selectClause.Selector, Is.EqualTo (_node.GetResolvedSelector (ClauseGenerationContext)));
    }
  }
}