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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.FunctionalProgramming;

namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class MaybeTest
  {
    private Maybe<string> _nothing;
    private Maybe<string> _nonNothing;

    [SetUp]
    public void SetUp ()
    {
      _nothing = Maybe<string>.Nothing;
      _nonNothing = new Maybe<string> ("Test");
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
      Assert.That (value.GetValueOrNull (), Is.Null);
    }

    [Test]
    public void Initialization_NonNull ()
    {
      var innerValue = "Test";
      var value = new Maybe<string> (innerValue);
      Assert.That (value.GetValueOrNull (), Is.SameAs (innerValue));
    }

    [Test]
    public void Equals_True ()
    {
      Assert.That (_nothing, Is.EqualTo (Maybe<string>.Nothing));
      Assert.That (_nonNothing, Is.EqualTo (new Maybe<string> ("Test")));
    }

    [Test]
    public void Equals_False ()
    {
      Assert.That (_nonNothing, Is.Not.EqualTo (Maybe<string>.Nothing));
      Assert.That (_nothing, Is.Not.EqualTo (new Maybe<string> ("Test")));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      Assert.That (_nothing.GetHashCode (), Is.EqualTo (Maybe<string>.Nothing.GetHashCode ()));
      Assert.That (_nonNothing.GetHashCode (), Is.EqualTo (new Maybe<string> ("Test").GetHashCode ()));
    }

    [Test]
    public void FromValue_Null ()
    {
      var value = Maybe.FromValue<string> (null);
      Assert.That (value, Is.EqualTo (_nothing));
    }

    [Test]
    public void FromValue_NonNull ()
    {
      var value = Maybe.FromValue ("Test");
      Assert.That (value, Is.EqualTo (new Maybe<string> ("Test")));
    }

    [Test]
    public void FromNullableValueType_Null ()
    {
      int? nullableInt = null;
      var value = Maybe.FromNullableValueType (nullableInt);

      Assert.That (value, Is.EqualTo (Maybe<int>.Nothing));
    }

    [Test]
    public void FromNullableValueType_NonNull ()
    {
      int? nullableInt = 5;
      var value = Maybe.FromNullableValueType (nullableInt);

      Assert.That (value, Is.EqualTo (new Maybe<int> (5)));
    }

    [Test]
    public void HasValue_Nothing ()
    {
      Assert.That (_nothing.HasValue, Is.False);
    }

    [Test]
    public void HasValue_NonNothing ()
    {
      Assert.That (_nonNothing.HasValue, Is.True);
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
      Assert.That (_nothing.ToString (), Is.EqualTo ("Nothing (String)"));
    }

    [Test]
    public void ToString_NonNothing ()
    {
      Assert.That (_nonNothing.ToString (), Is.EqualTo ("Value: Test (String)"));
    }

    [Test]
    public void Select_Nothing_DoesNotCallSelector ()
    {
      bool selectorCalled = false;
      _nothing.Select (s => { selectorCalled = true; return s.Length; });
      Assert.That (selectorCalled, Is.False);
    }

    [Test]
    public void Select_Nothing_ReturnsNothing ()
    {
      Assert.That (_nothing.Select (s => s.Length), Is.EqualTo (Maybe<int>.Nothing));
    }

    [Test]
    public void Select_NonNothing_NonNull_ReturnsValue ()
    {
      Assert.That (_nonNothing.Select (s => s.Length), Is.EqualTo (Maybe.FromValue (4))); // "Test"
    }

    [Test]
    public void Select_NonNothing_Null_ReturnsNothing ()
    {
      Assert.That (_nonNothing.Select (s => ((object) null)), Is.EqualTo (Maybe<object>.Nothing));
    }

    [Test]
    public void Select_NullableValueType_Nothing_ReturnsNothing ()
    {
      Assert.That (_nothing.Select (s => (int?) s.Length), Is.EqualTo (Maybe<int>.Nothing));
    }

    [Test]
    public void Select_NullableValueType_NonNothing_NonNull_ReturnsValue ()
    {
      Assert.That (_nonNothing.Select (s => (int?) s.Length), Is.EqualTo (Maybe.FromValue (4))); // "Test"
    }

    [Test]
    public void Select_NullableValueType_NonNothing_Null_ReturnsNothing ()
    {
      Assert.That (_nonNothing.Select (s => ((int?) null)), Is.EqualTo (Maybe<int>.Nothing));
    }
  }
}