// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Remotion.Linq.UnitTests.Linq.Core.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class MaxExpressionNodeTest : ExpressionNodeTestBase
  {
    private MaxExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();
      _node = new MaxExpressionNode (CreateParseInfo (), null);
    }

    [Test]
    public void SupportedMethod_WithoutSelector ()
    {
      AssertSupportedMethod_Generic (MaxExpressionNode.SupportedMethods, q => q.Max(), e => e.Max());
    }

    [Test]
    public void SupportedMethod_WithSelector ()
    {
      AssertSupportedMethod_Generic (MaxExpressionNode.SupportedMethods, q => q.Max (i => i.ToString()), e => e.Max (i => i.ToString()));
    }

    [Test]
    public void SupportedMethod_IEnumerableOverloads ()
    {
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<decimal>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<decimal?>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<double>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<double?>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<int>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<int?>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<long>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<long?>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<float>) e).Max ());
      AssertSupportedMethod_NonGeneric (MaxExpressionNode.SupportedMethods, null, e => ((IEnumerable<float?>) e).Max ());
      AssertSupportedMethod_Generic<object, decimal> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => 0.0m));
      AssertSupportedMethod_Generic<object, decimal?> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => (decimal?) 0.0m));
      AssertSupportedMethod_Generic<object, double> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => 0.0));
      AssertSupportedMethod_Generic<object, double?> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => (double?) 0.0));
      AssertSupportedMethod_Generic<object, int> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => 0));
      AssertSupportedMethod_Generic<object, int?> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => (int?) 0));
      AssertSupportedMethod_Generic<object, long> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => 0L));
      AssertSupportedMethod_Generic<object, long?> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => (long?) 0L));
      AssertSupportedMethod_Generic<object, float> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => 0.0f));
      AssertSupportedMethod_Generic<object, float?> (MaxExpressionNode.SupportedMethods, null, e => e.Max (i => (float?) 0.0f));
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Resolve_ThrowsInvalidOperationException ()
    {
      _node.Resolve (ExpressionHelper.CreateParameterExpression (), ExpressionHelper.CreateExpression (), ClauseGenerationContext);
    }

    [Test]
    public void ApplySelector ()
    {
      TestApply (_node, typeof (MaxResultOperator));
    }
  }
}
