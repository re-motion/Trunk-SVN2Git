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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class WhereExpressionNodeTest : ExpressionNodeTestBase
  {
    private WhereExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();
      
      var predicate = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      _node = new WhereExpressionNode (CreateParseInfo (), predicate);
    }

    [Test]
    public void SupportedMethod_WithoutPosition ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.Where (i => i > 5));
      Assert.That (WhereExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void Resolve_PassesExpressionToSource ()
    {
      var sourceMock = MockRepository.GenerateMock<IExpressionNode>();
      var predicate = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new WhereExpressionNode (CreateParseInfo (sourceMock), predicate);
      var expression = ExpressionHelper.CreateLambdaExpression();
      var parameter = ExpressionHelper.CreateParameterExpression();
      var expectedResult = ExpressionHelper.CreateExpression();
      sourceMock.Expect (mock => mock.Resolve (parameter, expression, ClauseGenerationContext)).Return (expectedResult);

      var result = node.Resolve (parameter, expression, ClauseGenerationContext);

      sourceMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (expectedResult));
    }

    [Test]
    public void GetResolvedPredicate ()
    {
      var predicate = ExpressionHelper.CreateLambdaExpression<int, bool> (i => i > 5);
      var node = new WhereExpressionNode (CreateParseInfo (), predicate);

      var expectedResult = Expression.MakeBinary (ExpressionType.GreaterThan, SourceReference, Expression.Constant (5));

      var result = node.GetResolvedPredicate (ClauseGenerationContext);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void CreateParameterForOutput ()
    {
      var source = new ConstantExpressionNode ("x", typeof (int[]), new[] { 1, 2, 3, 4, 5 });
      var node = new WhereExpressionNode (CreateParseInfo (source, "y"), ExpressionHelper.CreateLambdaExpression<int, bool> (y => y > 4));

      var parameter = node.CreateParameterForOutput ();

      Assert.That (parameter.Name, Is.EqualTo ("x"));
      Assert.That (parameter.Type, Is.SameAs (typeof (int)));
    }

    [Test]
    public void Apply ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);
      var clause = (WhereClause) QueryModel.BodyClauses[0];

      Assert.That (clause.Predicate, Is.EqualTo (_node.GetResolvedPredicate (ClauseGenerationContext)));
    }
  }
}