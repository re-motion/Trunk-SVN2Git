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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.FunctionalProgramming;

namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class MaybeTest
  {
    private Maybe<string> _stringNothing;
    private Maybe<string> _stringNonNothingTest;

    private Maybe<int> _intNothing;
    private Maybe<int> _intNonNothingFour;

    [SetUp]
    public void SetUp ()
    {
      _stringNothing = Maybe<string>.Nothing;
      _stringNonNothingTest = new Maybe<string> ("Test");

      _intNothing = Maybe<int>.Nothing;
      _intNonNothingFour = new Maybe<int> (4);
    }

    [Test]
    public void Initialization_Default ()
    {
      var value = new Maybe<string>();
      Assert.That (value.GetValueOrNull(), Is.Null);
    }

    [Test]
    public void Initialization_Null ()
    {
      var value = new Maybe<string> (null);
      Assert.That (value.GetValueOrNull(), Is.Null);
    }

    [Test]
    public void Initialization_NonNull ()
    {
      var innerValue = "Test";
      var value = new Maybe<string> (innerValue);
      Assert.That (value.GetValueOrNull(), Is.SameAs (innerValue));
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That (_stringNothing, Is.EqualTo (Maybe<string>.Nothing));
      Assert.That (_stringNonNothingTest, Is.EqualTo (new Maybe<string> ("Test")));
    }

    [Test]
    public void Equals_False ()
    {
      Assert.That (_stringNonNothingTest, Is.Not.EqualTo (Maybe<string>.Nothing));
      Assert.That (_stringNothing, Is.Not.EqualTo (new Maybe<string> ("Test")));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      Assert.That (_stringNothing.GetHashCode(), Is.EqualTo (Maybe<string>.Nothing.GetHashCode()));
      Assert.That (_stringNonNothingTest.GetHashCode(), Is.EqualTo (new Maybe<string> ("Test").GetHashCode()));
    }

    [Test]
    public void ForValue_Null ()
    {
      var value = Maybe.ForValue ((string) null);
      Assert.That (value, Is.EqualTo (_stringNothing));
    }

    [Test]
    public void ForValue_NonNull ()
    {
      var value = Maybe.ForValue ("Test");
      Assert.That (value, Is.EqualTo (_stringNonNothingTest));
    }

    [Test]
    public void ForValue_NullableValueType_Null ()
    {
      int? nullableInt = null;
      var value = Maybe.ForValue (nullableInt);

      Assert.That (value, Is.EqualTo (_intNothing));
    }

    [Test]
    public void ForValue_NullableValueType_NonNull ()
    {
      int? nullableInt = 4;
      var value = Maybe.ForValue (nullableInt);

      Assert.That (value, Is.EqualTo (_intNonNothingFour));
    }

    [Test]
    public void ForCondition_False ()
    {
      var value = Maybe.ForCondition (false, "Test");
      Assert.That (value, Is.EqualTo (_stringNothing));
    }

    [Test]
    public void ForCondition_True_Null ()
    {
      var value = Maybe.ForCondition (true, (string) null);
      Assert.That (value, Is.EqualTo (_stringNothing));
    }

    [Test]
    public void ForCondition_True_NonNull ()
    {
      var value = Maybe.ForCondition (true, "Test");
      Assert.That (value, Is.EqualTo (_stringNonNothingTest));
    }

    [Test]
    public void ForCondition_NullableValueType_False ()
    {
      int? nullableInt = 4;
      var value = Maybe.ForCondition (false, nullableInt);
      Assert.That (value, Is.EqualTo (_intNothing));
    }

    [Test]
    public void ForCondition_NullableValueType_True_Null ()
    {
      int? nullableInt = null;
      var value = Maybe.ForCondition (true, nullableInt);
      Assert.That (value, Is.EqualTo (_intNothing));
    }

    [Test]
    public void ForCondition_NullableValueType_True_NonNull ()
    {
      int? nullableInt = 4;
      var value = Maybe.ForCondition (true, nullableInt);
      Assert.That (value, Is.EqualTo (_intNonNothingFour));
    }

    [Test]
    public void HasValue_Nothing ()
    {
      Assert.That (_stringNothing.HasValue, Is.False);
    }

    [Test]
    public void HasValue_NonNothing ()
    {
      Assert.That (_stringNonNothingTest.HasValue, Is.True);
    }

    [Test]
    public void HasValue_ValueType_Nothing ()
    {
      var nothing = new Maybe<int>();
      Assert.That (nothing.HasValue, Is.False);
    }

    [Test]
    public void HasValue_ValueType_NonNothing ()
    {
      var nonNothing = new Maybe<int> (10);
      Assert.That (nonNothing.HasValue, Is.True);
    }

    [Test]
    public void ToString_Nothing ()
    {
      Assert.That (_stringNothing.ToString(), Is.EqualTo ("Nothing (String)"));
    }

    [Test]
    public void ToString_NonNothing ()
    {
      Assert.That (_stringNonNothingTest.ToString(), Is.EqualTo ("Value: Test (String)"));
    }

    [Test]
    public void Select_Nothing_DoesNotCallSelector ()
    {
      bool selectorCalled = false;
      _stringNothing.Select (
          s =>
          {
            selectorCalled = true;
            return s.Length;
          });
      Assert.That (selectorCalled, Is.False);
    }

    [Test]
    public void Select_Nothing_ReturnsNothing ()
    {
      Assert.That (_stringNothing.Select (s => s.Length), Is.EqualTo (_intNothing));
    }

    [Test]
    public void Select_NonNothing_NonNull_ReturnsValue ()
    {
      Assert.That (_stringNonNothingTest.Select (s => s.Length), Is.EqualTo (_intNonNothingFour)); // "Test"
    }

    [Test]
    public void Select_NonNothing_Null_ReturnsNothing ()
    {
      Assert.That (_stringNonNothingTest.Select (s => ((object) null)), Is.EqualTo (Maybe<object>.Nothing));
    }

    [Test]
    public void Select_NullableValueType_Nothing_ReturnsNothing ()
    {
      Assert.That (_stringNothing.Select (s => (int?) s.Length), Is.EqualTo (_intNothing));
    }

    [Test]
    public void Select_NullableValueType_NonNothing_NonNull_ReturnsValue ()
    {
      Assert.That (_stringNonNothingTest.Select (s => (int?) s.Length), Is.EqualTo (_intNonNothingFour)); // "Test"
    }

    [Test]
    public void Select_NullableValueType_NonNothing_Null_ReturnsNothing ()
    {
      Assert.That (_stringNonNothingTest.Select (s => ((int?) null)), Is.EqualTo (_intNothing));
    }

    [Test]
    public void Do_Nothing_DoesNotExecuteAction ()
    {
      bool actionExecuted = false;

      _stringNothing.Do (s => actionExecuted = true);

      Assert.That (actionExecuted, Is.False);
    }

    [Test]
    public void Do_NonNothing ()
    {
      bool actionExecuted = false;
      string value = null;

      _stringNonNothingTest.Do (
          s =>
          {
            value = s;
            actionExecuted = true;
          });

      Assert.That (actionExecuted, Is.True);
      Assert.That (value, Is.EqualTo ("Test"));
    }

    [Test]
    public void Where_Nothing_DoesNotCallPredicate ()
    {
      bool predicateCalled = false;
      _stringNothing.Where (
          s =>
          {
            predicateCalled = true;
            return true;
          });
      Assert.That (predicateCalled, Is.False);
    }

    [Test]
    public void Where_Nothing_ReturnsNothing ()
    {
      Assert.That (_stringNothing.Where (s => true), Is.EqualTo (_stringNothing));
    }

    [Test]
    public void Where_NonNothing_CallsPredicateWithValue ()
    {
      string predicateParameter = null;
      _stringNonNothingTest.Where (
          s =>
          {
            predicateParameter = s;
            return false;
          });

      Assert.That (predicateParameter, Is.EqualTo ("Test"));
    }

    [Test]
    public void Where_NonNothing_False_ReturnsNothing ()
    {
      Assert.That (_stringNonNothingTest.Where (s => false), Is.EqualTo (_stringNothing));
    }

    [Test]
    public void Where_NonNothing_True_ReturnsNonNothing ()
    {
      Assert.That (_stringNonNothingTest.Where (s => true), Is.EqualTo (_stringNonNothingTest));
    }

    [Test]
    public void EnumerateValues ()
    {
      var enumerable = Maybe.EnumerateValues (Maybe.ForValue ("One"), Maybe.ForValue ((string) null), Maybe.ForValue ("Two"));

      Assert.That (enumerable.ToArray(), Is.EqualTo (new[] { "One", "Two" }));
    }
  }
}