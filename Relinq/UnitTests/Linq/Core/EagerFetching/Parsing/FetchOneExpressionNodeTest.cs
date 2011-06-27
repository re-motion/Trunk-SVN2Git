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
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.Parsing.Structure.IntermediateModel;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.EagerFetching.Parsing;

namespace Remotion.Linq.UnitTests.Linq.Core.EagerFetching.Parsing
{
  [TestFixture]
  public class FetchOneExpressionNodeTest : ExpressionNodeTestBase
  {
    private FetchOneExpressionNode _node;

    public override void SetUp ()
    {
      base.SetUp ();

      _node = new FetchOneExpressionNode (CreateParseInfo (), ExpressionHelper.CreateLambdaExpression<Cook, Cook> (s => s.Substitution));
    }

    [Test]
    public void Apply ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);

      Assert.That (QueryModel.ResultOperators.Count, Is.EqualTo (1));
      Assert.That (QueryModel.ResultOperators[0], Is.InstanceOf (typeof (FetchOneRequest)));
      Assert.That (((FetchOneRequest) QueryModel.ResultOperators[0]).RelationMember, Is.EqualTo (typeof (Cook).GetProperty ("Substitution")));
    }

    [Test]
    public void Apply_AddsMapping ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (ClauseGenerationContext.GetContextInfo (_node), Is.SameAs (QueryModel.ResultOperators[0]));
    }
  }
}
