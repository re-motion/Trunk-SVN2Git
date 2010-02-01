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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using ReflectionUtility=Remotion.Data.Linq.ReflectionUtility;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class ContainsObjectExpressionNodeTest 
  {
    private ContainsObjectExpressionNode _node;

    [SetUp]
    public void SetUp ()
    {
      SourceNode = new MainSourceExpressionNode ("x", Expression.Constant (new[] { 1, 2, 3 }));
      ClauseGenerationContext = new ClauseGenerationContext (
          MethodCallExpressionNodeTypeRegistry.CreateDefault ());

      QueryModel = SourceNode.Apply (null, ClauseGenerationContext);
      SourceClause = QueryModel.MainFromClause;
      SourceReference = (QuerySourceReferenceExpression) QueryModel.SelectClause.Selector;

      var fakeParseInfo = CreateFakeParseInfo();
      _node = new ContainsObjectExpressionNode (fakeParseInfo, Expression.Constant ("test"));
    }

    public MainSourceExpressionNode SourceNode { get; private set; }
    public MainFromClause SourceClause { get; private set; }
    public QuerySourceReferenceExpression SourceReference { get; private set; }
    public ClauseGenerationContext ClauseGenerationContext { get; private set; }
    public QueryModel QueryModel { get; private set; }

    [Test]
    public void SupportedMethods ()
    {
      var method = typeof (DomainObjectCollection).GetMethod ("ContainsObject");
      Assert.That (ContainsObjectExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Resolve_ThrowsInvalidOperationException ()
    {
      var parameterExpression = Expression.Parameter (typeof (int), "i");
      var resolvedExpression = Expression.Parameter (typeof (int), "j");
      _node.Resolve (parameterExpression, resolvedExpression, ClauseGenerationContext);
    }

    [Test]
    public void Apply ()
    {
      TestApply (_node, typeof (ContainsResultOperator));
    }

    private void TestApply (ResultOperatorExpressionNodeBase node, Type expectedResultOperatorType)
    {
      var result = node.Apply (QueryModel, ClauseGenerationContext);
      Assert.That (result, Is.SameAs (QueryModel));

      Assert.That (QueryModel.ResultOperators.Count, Is.EqualTo (1));
      Assert.That (QueryModel.ResultOperators[0], Is.InstanceOfType (expectedResultOperatorType));
    }

    private MethodCallExpressionParseInfo CreateFakeParseInfo ()
    {
      var query = ExpressionHelper.CreateStudentQueryable ();
      var methodInfo = ReflectionUtility.GetMethod (() => query.Count ());
      var methodCallExpression = Expression.Call (methodInfo, query.Expression);
      return new MethodCallExpressionParseInfo ("x", SourceNode, methodCallExpression);
    }

  }
}
