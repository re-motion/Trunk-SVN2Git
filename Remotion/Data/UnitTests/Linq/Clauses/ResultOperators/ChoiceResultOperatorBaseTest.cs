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
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.UnitTests.Linq.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.Linq.Clauses.ResultOperators
{
  [TestFixture]
  public class ChoiceResultOperatorBaseTest
  {
    private TestChoiceResultOperator _resultOperatorWithDefaultWhenEmpty;
    private TestChoiceResultOperator _resultOperatorNoDefaultWhenEmpty;

    [SetUp]
    public void SetUp ()
    {
      _resultOperatorWithDefaultWhenEmpty = new TestChoiceResultOperator (true);
      _resultOperatorNoDefaultWhenEmpty = new TestChoiceResultOperator (false);
    }

    [Test]
    public void ExecutionStrategy ()
    {
      Assert.That (_resultOperatorWithDefaultWhenEmpty.ExecutionStrategy, Is.SameAs (SingleExecutionStrategy.InstanceWithDefaultWhenEmpty));
      Assert.That (_resultOperatorNoDefaultWhenEmpty.ExecutionStrategy, Is.SameAs (SingleExecutionStrategy.InstanceNoDefaultWhenEmpty));
    }

    [Test]
    public void GetOutputDataInfo ()
    {
      var studentExpression = Expression.Constant (new Student ());
      var input = new StreamedSequenceInfo (typeof (Student[]), studentExpression);
      var result = _resultOperatorNoDefaultWhenEmpty.GetOutputDataInfo (input);

      Assert.That (result, Is.InstanceOfType (typeof (StreamedSingleValueInfo)));
      Assert.That (result.DataType, Is.SameAs (typeof (Student)));
    }

    [Test]
    public void GetOutputDataInfo_DefaultWhenEmpty ()
    {
      var studentExpression = Expression.Constant (new Student ());
      var input = new StreamedSequenceInfo (typeof (Student[]), studentExpression);

      Assert.That (((StreamedSingleValueInfo) _resultOperatorWithDefaultWhenEmpty.GetOutputDataInfo (input)).ReturnDefaultWhenEmpty, Is.True);
      Assert.That (((StreamedSingleValueInfo) _resultOperatorNoDefaultWhenEmpty.GetOutputDataInfo (input)).ReturnDefaultWhenEmpty, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetOutputDataInfo_InvalidInput ()
    {
      var input = new StreamedScalarValueInfo (typeof (Student));
      _resultOperatorNoDefaultWhenEmpty.GetOutputDataInfo (input);
    }
  }
}