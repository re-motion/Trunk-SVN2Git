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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.Linq.Clauses.ResultOperators
{
  [TestFixture]
  public class UnionResultOperatorTest
  {
    private UnionResultOperator _resultOperator;
    private Expression _source2;

    [SetUp]
    public void SetUp ()
    {
      _source2 = Expression.Constant (new[] { 2 });
      _resultOperator = new UnionResultOperator (_source2);
    }

    [Test]
    public void GetConstantSource2 ()
    {
      Assert.That (_resultOperator.GetConstantSource2 (), Is.SameAs (((ConstantExpression) _source2).Value));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetConstantSource2_NoConstantExpression ()
    {
      var resultOperator = new UnionResultOperator (Expression.Parameter (typeof (IEnumerable<string>), "ss"));
      resultOperator.GetConstantSource2 ();
    }

    [Test]
    public void ExecutionStrategy ()
    {
      Assert.That (_resultOperator.ExecutionStrategy, Is.SameAs (CollectionExecutionStrategy.Instance));
    }

    [Test]
    public void Clone ()
    {
      var clonedClauseMapping = new QuerySourceMapping ();
      var cloneContext = new CloneContext (clonedClauseMapping);
      var clone = _resultOperator.Clone (cloneContext);

      Assert.That (clone, Is.InstanceOfType (typeof (UnionResultOperator)));
      Assert.That (((UnionResultOperator) clone).Source2, Is.SameAs (_source2));
    }

    [Test]
    public void ExecuteInMemory ()
    {
      var items = new[] { 1, 2, 3 };
      var input = new StreamedSequence (items, Expression.Constant (0));
      var result = _resultOperator.ExecuteInMemory<int> (input);

      Assert.That (result.GetTypedSequence<int>().ToArray(), Is.EquivalentTo (new[] { 1, 2, 3 }));
    }

    [Test]
    public void GetResultType ()
    {
      Assert.That (_resultOperator.GetResultType (typeof (IQueryable<int>)), Is.SameAs (typeof (IQueryable<int>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetResultType_InvalidType ()
    {
      _resultOperator.GetResultType (typeof (Student));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetResultType_InvalidEnumerable ()
    {
      _resultOperator.GetResultType (typeof (IEnumerable<Student>));
    }

    [Test]
    public void TransformExpressions ()
    {
      var oldExpression = ExpressionHelper.CreateExpression ();
      var newExpression = ExpressionHelper.CreateExpression ();
      var resultOperator = new UnionResultOperator (oldExpression);

      resultOperator.TransformExpressions (ex =>
      {
        Assert.That (ex, Is.SameAs (oldExpression));
        return newExpression;
      });

      Assert.That (resultOperator.Source2, Is.SameAs (newExpression));
    }
  }
}