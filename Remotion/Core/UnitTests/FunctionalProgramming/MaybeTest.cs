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
using Remotion.Collections;
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
    public void Initialize_And_ValueOrDefault_DefaultFromReferenceType ()
    {
      var value = new Maybe<string> ();
      Assert.That (value.ValueOrDefault (), Is.Null);
    }

    [Test]
    public void Initialize_And_ValueOrDefault_NullFromReferenceType ()
    {
      var value = new Maybe<string> (null);
      Assert.That (value.ValueOrDefault (), Is.Null);
    }

    [Test]
    public void Initialize_And_ValueOrDefault_NonNullFromReferenceType ()
    {
      var innerValue = "Test";
      var value = new Maybe<string> (innerValue);
      Assert.That (value.ValueOrDefault (), Is.SameAs (innerValue));
    }

    [Test]
    public void Initialize_And_ValueOrDefault_DefaultFromValueType ()
    {
      var value = new Maybe<int> ();
      Assert.That (value.ValueOrDefault (), Is.EqualTo (default (int)));
    }

    [Test]
    public void Initialize_And_ValueOrDefault_NonNullFromValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int> (innerValue);
      Assert.That (value.ValueOrDefault (), Is.EqualTo (innerValue));
    }

    [Test]
    public void Initialize_And_ValueOrDefault_DefaultFromNullableValueType ()
    {
      var value = new Maybe<int?> ();
      Assert.That (value.ValueOrDefault (), Is.Null);
    }

    [Test]
    public void Initialize_And_ValueOrDefault_NullFromNullableValueType ()
    {
      var value = new Maybe<int?> (null);
      Assert.That (value.ValueOrDefault (), Is.Null);
    }

    [Test]
    public void Initialize_And_ValueOrDefault_NonNullFromNullableValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int?> (innerValue);
      Assert.That (value.ValueOrDefault (), Is.EqualTo (innerValue));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_DefaultFromReferenceType ()
    {
      var value = new Maybe<string> ();
      Assert.That (value.ValueOrDefault ("42"), Is.EqualTo ("42"));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_NullFromReferenceType ()
    {
      var value = new Maybe<string> (null);
      Assert.That (value.ValueOrDefault ("42"), Is.EqualTo ("42"));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_NonNullFromReferenceType ()
    {
      var innerValue = "Test";
      var value = new Maybe<string> (innerValue);
      Assert.That (value.ValueOrDefault ("42"), Is.SameAs (innerValue));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_DefaultFromValueType ()
    {
      var value = new Maybe<int> ();
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (-1));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_NonNullFromValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int> (innerValue);
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (innerValue));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_DefaultFromNullableValueType ()
    {
      var value = new Maybe<int?> ();
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (-1));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_NullFromNullableValueType ()
    {
      var value = new Maybe<int?> (null);
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (-1));
    }

    [Test]
    public void ValueOrDefault_WithExplicitDefaultValue_NonNullFromNullableValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int> (innerValue);
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (innerValue));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Maybe-Object must have a value.")]
    public void Value_DefaultFromReferenceType_Throws ()
    {
      var value = new Maybe<string> ();
      value.Value ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Value_NullFromReferenceType_Throws ()
    {
      var value = new Maybe<string> (null);
      value.Value ();
    }

    [Test]
    public void Value_NonNullFromReferenceType ()
    {
      var innerValue = "Test";
      var value = new Maybe<string> (innerValue);
      Assert.That (value.Value (), Is.SameAs (innerValue));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Value_DefaultFromValueType_Throws ()
    {
      var value = new Maybe<int> ();
      value.Value ();
    }

    [Test]
    public void Value_NonNullFromValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int> (innerValue);
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (innerValue));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Value_DefaultFromNullableValueType_Throws ()
    {
      var value = new Maybe<int?> ();
      value.Value ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Value_NullFromNullableValueType_Throws ()
    {
      var value = new Maybe<int?> (null);
      value.Value ();
    }

    [Test]
    public void Value_NonNullFromNullableValueType ()
    {
      var innerValue = 42;
      var value = new Maybe<int> (innerValue);
      Assert.That (value.ValueOrDefault (-1), Is.EqualTo (innerValue));
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
    public void Do_ReturnsInstance ()
    {
      var result = _stringNonNothingTest.Do (s => { });
      Assert.That (result, Is.EqualTo (_stringNonNothingTest));
    }

    [Test]
    public void Do_Otherwise_Nothing_DoesNotExecuteAction_ExecutesOtherwise ()
    {
      bool actionExecuted = false;
      bool otherwiseExecuted = false;

      _stringNothing.Do (s => actionExecuted = true, () => otherwiseExecuted = true);

      Assert.That (actionExecuted, Is.False);
      Assert.That (otherwiseExecuted, Is.True);
    }

    [Test]
    public void Do_Otherwise_NonNothing_ExecutesAction_DoesNotExecuteOtherwise ()
    {
      bool actionExecuted = false;
      bool otherwiseExecuted = false;
      string value = null;

      _stringNonNothingTest.Do (
          s =>
          {
            value = s;
            actionExecuted = true;
          },
          () => otherwiseExecuted = true);

      Assert.That (actionExecuted, Is.True);
      Assert.That (value, Is.EqualTo ("Test"));
      Assert.That (otherwiseExecuted, Is.False);
    }

    [Test]
    public void Do_Otherwise_ReturnsInstance ()
    {
      var result = _stringNonNothingTest.Do (s => { }, () => { });
      Assert.That (result, Is.EqualTo (_stringNonNothingTest));
    }

    [Test]
    public void Otherwise_Nothing_ExecutesAction ()
    {
      bool otherwiseExecuted = false;

      _stringNothing.Otherwise (() => otherwiseExecuted = true);

      Assert.That (otherwiseExecuted, Is.True);
    }

    [Test]
    public void Otherwise_NonNothing_DoesNotExecuteAction ()
    {
      bool otherwiseExecuted = false;

      _stringNonNothingTest.Otherwise (() => otherwiseExecuted = true);

      Assert.That (otherwiseExecuted, Is.False);
    }

    [Test]
    public void Otherwise_ReturnsInstance ()
    {
      var result = _stringNonNothingTest.Otherwise (() => { });
      Assert.That (result, Is.EqualTo (_stringNonNothingTest));
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
    public void SelectMany_FirstNothing_ReturnsNothing ()
    {
      var result = _stringNothing.SelectMany (s => _intNonNothingFour, (s, i) => Tuple.NewTuple (s, i));

      Assert.That (result, Is.EqualTo (Maybe<Tuple<string, int>>.Nothing));
    }

    [Test]
    public void SelectMany_FirstNothing_DoesNotCallSelectors ()
    {
      bool firstSelectorCalled = false;
      bool secondSelectorCalled = false;
      _stringNothing.SelectMany (s =>
      {
        firstSelectorCalled = true;
        return _intNonNothingFour;
      }, (s, i) =>
      {
        secondSelectorCalled = true;
        return Tuple.NewTuple (s, i);
      });

      Assert.That (firstSelectorCalled, Is.False);
      Assert.That (secondSelectorCalled, Is.False);
    }

    [Test]
    public void SelectMany_SecondNothing_ReturnsNothing ()
    {
      var result = _stringNonNothingTest.SelectMany (s => _intNothing, (s, i) => Tuple.NewTuple (s, i));

      Assert.That (result, Is.EqualTo (Maybe<Tuple<string, int>>.Nothing));
    }

    [Test]
    public void SelectMany_SecondNothing_DoesNotCallSecondSelector ()
    {
      bool firstSelectorCalled = false;
      bool secondSelectorCalled = false;
      _stringNonNothingTest.SelectMany (s =>
      {
        firstSelectorCalled = true;
        return _intNothing;
      }, (s, i) =>
      {
        secondSelectorCalled = true;
        return Tuple.NewTuple (s, i);
      });

      Assert.That (firstSelectorCalled, Is.True);
      Assert.That (secondSelectorCalled, Is.False);
    }

    [Test]
    public void SelectMany_ResultNull_ReturnsNothing ()
    {
      var result = _stringNonNothingTest.SelectMany (s => _intNonNothingFour, (s, i) => (Tuple<string, int>) null);

      Assert.That (result, Is.EqualTo (Maybe<Tuple<string, int>>.Nothing));
    }

    [Test]
    public void SelectMany_ResultNull_CallsAllSelectors ()
    {
      bool firstSelectorCalled = false;
      bool secondSelectorCalled = false;
      _stringNonNothingTest.SelectMany (s =>
      {
        firstSelectorCalled = true;
        return _intNonNothingFour;
      }, (s, i) =>
      {
        secondSelectorCalled = true;
        return (Tuple<string, int>) null;
      });

      Assert.That (firstSelectorCalled, Is.True);
      Assert.That (secondSelectorCalled, Is.True);
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

    [Test]
    public void Linq_IntegrationTest ()
    {
      Assert.That (ExecuteLinqQuery ("s", 100).ValueOrDefault (), Is.EqualTo ("s/100"));
      Assert.That (ExecuteLinqQuery ("s", 99), Is.EqualTo (Maybe<string>.Nothing));
      Assert.That (ExecuteLinqQuery (null, 100), Is.EqualTo (Maybe<string>.Nothing));
      Assert.That (ExecuteLinqQuery ("s", null), Is.EqualTo (Maybe<string>.Nothing));
    }

    private Maybe<string> ExecuteLinqQuery (string stringValue, int? intValue)
    {
      var r = from s in Maybe.ForValue (stringValue)
              from i in Maybe.ForValue (intValue)
              let j = s.Length + i
              where j > 100
              select s + "/" + i;
      return r;
    }
  }
}