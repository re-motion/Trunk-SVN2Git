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
      Assert.That (value, Is.EqualTo (_nothing));
    }

    [Test]
    public void Initialization_Null ()
    {
      var value = new Maybe<string> (null);
      Assert.That (value, Is.EqualTo (_nothing));
    }

    [Test]
    public void Initialization_NonNull ()
    {
      var value = new Maybe<string> ("Test");
      Assert.That (value, Is.Not.EqualTo (_nothing));
      Assert.That (value, Is.EqualTo (new Maybe<string> ("Test")));
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
      Assert.That (_nothing.ToString (), Is.EqualTo ("Nothing"));
    }

    [Test]
    public void ToString_NonNothing ()
    {
      Assert.That (_nonNothing.ToString (), Is.EqualTo ("Value: Test"));
    }
  }
}