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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Data.UnitTests.Linq.TestDomain;
using System.Linq;

namespace Remotion.Data.UnitTests.Linq.EagerFetching
{
  [TestFixture]
  public class ThenFetchOneExpressionNodeTest : ExpressionNodeTestBase
  {
    private ThenFetchOneExpressionNode _node;
    
    private TestFetchRequest _sourceFetchRequest;
    private IExpressionNode _sourceFetchRequestNode;

    public override void SetUp ()
    {
      base.SetUp ();

      _sourceFetchRequest = new TestFetchRequest (typeof (Student).GetProperty ("OtherStudent"));
      _sourceFetchRequestNode = new MainSourceExpressionNode ("x", Expression.Constant (new Student[0]));
      ClauseGenerationContext.AddContextInfo (_sourceFetchRequestNode, _sourceFetchRequest);

      QueryModel.ResultOperators.Add (_sourceFetchRequest);

      _node = new ThenFetchOneExpressionNode (CreateParseInfo (_sourceFetchRequestNode), ExpressionHelper.CreateLambdaExpression<Student, Student> (s => s.OtherStudent));
    }

    [Test]
    public void SupportedMethod ()
    {
      var method = typeof (ExtensionMethods).GetMethod ("ThenFetchOne");
      Assert.That (ThenFetchOneExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void Apply ()
    {
      var queryModel = _node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (queryModel, Is.SameAs (QueryModel));

      Assert.That (QueryModel.ResultOperators, Is.EqualTo (new[] { _sourceFetchRequest }));
      var innerFetchRequests = _sourceFetchRequest.InnerFetchRequests.ToArray();
      Assert.That (innerFetchRequests.Length, Is.EqualTo (1));
      Assert.That (innerFetchRequests[0], Is.InstanceOfType (typeof (FetchOneRequest)));
      Assert.That (innerFetchRequests[0].RelationMember, Is.SameAs (typeof (Student).GetProperty ("OtherStudent")));
    }

    [Test]
    public void Apply_AddsMapping ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);

      var innerFetchRequest = ((FetchRequestBase) QueryModel.ResultOperators[0]).InnerFetchRequests.Single();
      Assert.That (ClauseGenerationContext.GetContextInfo (_node), Is.SameAs (innerFetchRequest));
    }

    [Test]
    public void Apply_AddsMappingForExisting ()
    {
      _node.Apply (QueryModel, ClauseGenerationContext);

      var node = new ThenFetchOneExpressionNode (CreateParseInfo (_sourceFetchRequestNode), ExpressionHelper.CreateLambdaExpression<Student, Student> (s => s.OtherStudent));
      node.Apply (QueryModel, ClauseGenerationContext);

      var innerFetchRequest = ((FetchRequestBase) QueryModel.ResultOperators[0]).InnerFetchRequests.Single ();
      Assert.That (ClauseGenerationContext.GetContextInfo (node), Is.SameAs (innerFetchRequest));
    }

    [Test]
    [ExpectedException (typeof (ParserException))]
    public void Apply_WithoutPreviousFetchRequest ()
    {
      var node = new ThenFetchOneExpressionNode (CreateParseInfo (), ExpressionHelper.CreateLambdaExpression<Student, Student> (s => s.OtherStudent));
      node.Apply (QueryModel, ClauseGenerationContext);
    }
  }
}