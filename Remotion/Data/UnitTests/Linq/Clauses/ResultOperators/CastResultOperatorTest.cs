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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.Linq.Clauses.ResultOperators
{
  [TestFixture]
  public class CastResultOperatorTest
  {
    private CastResultOperator _resultOperator;

    [SetUp]
    public void SetUp ()
    {
      _resultOperator = new CastResultOperator (typeof (GoodStudent));
    }

    [Test]
    public void Clone ()
    {
      var clonedClauseMapping = new QuerySourceMapping ();
      var cloneContext = new CloneContext (clonedClauseMapping);
      var clone = _resultOperator.Clone (cloneContext);

      Assert.That (clone, Is.InstanceOfType (typeof (CastResultOperator)));
      Assert.That (((CastResultOperator) clone).CastItemType, Is.SameAs (_resultOperator.CastItemType));
    }

    [Test]
    public void ExecuteInMemory ()
    {
      var student1 = new GoodStudent ();
      var student2 = new GoodStudent ();
      object items = new Student[] { student1, student2 };
      var input = new ExecuteInMemorySequenceData (items, Expression.Constant (student1, typeof (Student)));

      var result = _resultOperator.ExecuteInMemory (input);

      var sequence = result.GetCurrentSequence<GoodStudent>();
      Assert.That (sequence.A.ToArray (), Is.EquivalentTo (new[] { student1, student2 }));
      Assert.That (sequence.B.Type, Is.EqualTo (typeof (GoodStudent)));
      Assert.That (((UnaryExpression) sequence.B).Operand, Is.SameAs (input.ItemExpression));
    }

    [Test]
    public void ExecutionStrategy ()
    {
      Assert.That (_resultOperator.ExecutionStrategy, Is.SameAs (CollectionExecutionStrategy.Instance));
    }

    [Test]
    public void GetResultType ()
    {
      Assert.That (_resultOperator.GetResultType (typeof (IQueryable<Student>)), Is.SameAs (typeof (IQueryable<GoodStudent>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetResultType_InvalidType ()
    {
      _resultOperator.GetResultType (typeof (Student));
    }
  }
}