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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class CastExpressionNodeTest : ExpressionNodeTestBase
  {
    private CastExpressionNode _node;
    private MethodInfo _castToChefMethod;
    private MainSourceExpressionNode _cookSource;
    private MainFromClause _cookClause;

    public override void SetUp ()
    {
      base.SetUp ();

      _cookSource = new MainSourceExpressionNode ("s", Expression.Constant (new[] { new Cook () }));
      _cookClause = ExpressionHelper.CreateMainFromClause_Cook ();
      ClauseGenerationContext.AddContextInfo (_cookSource, _cookClause);

      _castToChefMethod = ReflectionUtility.GetMethod (() => ((IQueryable<Cook[]>)null).Cast<Chef>());
      _node = new CastExpressionNode (CreateParseInfo (_cookSource, "s", _castToChefMethod));
    }

    [Test]
    public void SupportedMethods ()
    {
      AssertSupportedMethod_Generic (CastExpressionNode.SupportedMethods, q => q.Cast<int> (), e => e.Cast<int> ());
    }

    [Test]
    public void CastItemType ()
    {
      Assert.That (_node.CastItemType, Is.SameAs (typeof (Chef)));
    }

    [Test]
    public void Resolve_PassesConvertedExpressionToSource ()
    {
      var expression = ExpressionHelper.CreateLambdaExpression<Chef, string> (s => s.LetterOfRecommendation);
      var result = _node.Resolve (expression.Parameters[0], expression.Body, ClauseGenerationContext);

      var expectedResult = ExpressionHelper.Resolve<Cook, string> (_cookClause, s => ((Chef) s).LetterOfRecommendation);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedResult, result);
    }

    [Test]
    public void Apply ()
    {
      var result = _node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (result, Is.SameAs (QueryModel));
      Assert.That (QueryModel.ResultOperators.Count, Is.EqualTo (1));

      var castResultOperator = (CastResultOperator) QueryModel.ResultOperators[0];
      Assert.That (castResultOperator.CastItemType, Is.SameAs (typeof (Chef)));
    }
  }
}
