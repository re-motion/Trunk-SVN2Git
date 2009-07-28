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
using System.Collections;
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
  public class LastResultOperatorTest
  {
    private LastResultOperator _resultOperatorNoDefault;
    private LastResultOperator _resultOperatorWithDefault;
    private QuerySourceMapping _querySourceMapping;
    private CloneContext _cloneContext;

    [SetUp]
    public void SetUp ()
    {
      _resultOperatorNoDefault = new LastResultOperator (false);
      _resultOperatorWithDefault = new LastResultOperator (true);
      _querySourceMapping = new QuerySourceMapping ();
      _cloneContext = new CloneContext(_querySourceMapping);
    }

    [Test]
    public void Clone ()
    {
      var clone = _resultOperatorWithDefault.Clone (_cloneContext);

      Assert.That (clone, Is.InstanceOfType (typeof (LastResultOperator)));
      Assert.That (((LastResultOperator) clone).ReturnDefaultWhenEmpty, Is.True);
    }

    [Test]
    public void Clone_ReturnDefaultIfEmpty_False ()
    {
      var clone = _resultOperatorNoDefault.Clone (_cloneContext);

      Assert.That (clone, Is.InstanceOfType (typeof (LastResultOperator)));
      Assert.That (((LastResultOperator) clone).ReturnDefaultWhenEmpty, Is.False);
    }

    [Test]
    public void ExecuteInMemory ()
    {
      IEnumerable items = new[] { 1, 2, 3 };
      var input = new StreamedSequence (items, Expression.Constant (0));
      var result = _resultOperatorWithDefault.ExecuteInMemory (input);

      Assert.That (result.Value, Is.EqualTo (3));
    }

    [Test]
    public void ExecuteInMemory_Empty_Default ()
    {
      IEnumerable items = new int[0];
      var input = new StreamedSequence (items, Expression.Constant (0));
      var result = _resultOperatorWithDefault.ExecuteInMemory (input);

      Assert.That (result.Value, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Sequence contains no elements")]
    public void ExecuteInMemory_Empty_NoDefault ()
    {
      IEnumerable items = new int[0];
      var input = new StreamedSequence (items, Expression.Constant (0));
      _resultOperatorNoDefault.ExecuteInMemory (input);
    }


    [Test]
    public void ExecutionStrategy_Default ()
    {
      Assert.That (_resultOperatorWithDefault.ExecutionStrategy, Is.SameAs (SingleExecutionStrategy.InstanceWithDefaultWhenEmpty));
    }

    [Test]
    public void ExecutionStrategy_NoDefault ()
    {
      Assert.That (_resultOperatorNoDefault.ExecutionStrategy, Is.SameAs (SingleExecutionStrategy.InstanceNoDefaultWhenEmpty));
    }
  }
}