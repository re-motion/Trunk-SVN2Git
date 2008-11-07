/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using System.Linq;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class TypedSequenceTest
  {
    [Test]
    public void Empty()
    {
      Assert.That (TypedSequence.Empty.ToArray (), Is.Empty);
    }

    [Test]
    public void EnumerableCtor ()
    {
      IEnumerable<Tuple<Type, object>> values = new[] {new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two")};
      Assert.That (new TypedSequence (values).ToArray (), Is.EqualTo (values));
    }

    [Test]
    public void ParamsCtor ()
    {
      Assert.That (new TypedSequence (new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two")).ToArray (), 
          Is.EqualTo (new[] {new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two")}));
    }

    [Test]
    public void Count()
    {
      IEnumerable<Tuple<Type, object>> values = new[] { new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two") };
      Assert.That (new TypedSequence (values).Count, Is.EqualTo (2));
      Assert.That (TypedSequence.Empty.Count, Is.EqualTo (0));
    }

    [Test]
    public void Types()
    {
      IEnumerable<Tuple<Type, object>> values = new[] { new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two") };
      Assert.That (new TypedSequence (values).Types, Is.EqualTo (new[] {typeof (int), typeof (string)}));
    }

    [Test]
    public void Values ()
    {
      IEnumerable<Tuple<Type, object>> values = new[] { new Tuple<Type, object> (typeof (int), 1), new Tuple<Type, object> (typeof (string), "two") };
      Assert.That (new TypedSequence (values).Values, Is.EqualTo (new object[] { 1, "two" }));
    }

    [Test]
    public void Create_1 ()
    {
      var sequence = TypedSequence.Create (1);
      Assert.That (sequence.ToArray(), Is.EqualTo (new[] {new Tuple<Type, object> (typeof (int), 1)}));
    }

    [Test]
    public void Create_10 ()
    {
      var sequence = TypedSequence.Create (1, 2, "three", 4.0, DateTime.MinValue, TimeSpan.Zero, 7, "8", "9", true);
      Assert.That (sequence.ToArray (), Is.EqualTo (new[] { 
          new Tuple<Type, object> (typeof (int), 1), 
          new Tuple<Type, object> (typeof (int), 2), 
          new Tuple<Type, object> (typeof (string), "three"),
          new Tuple<Type, object> (typeof (double), 4.0),
          new Tuple<Type, object> (typeof (DateTime), DateTime.MinValue),
          new Tuple<Type, object> (typeof (TimeSpan), TimeSpan.Zero),
          new Tuple<Type, object> (typeof (int), 7),
          new Tuple<Type, object> (typeof (string), "8"),
          new Tuple<Type, object> (typeof (string), "9"),
          new Tuple<Type, object> (typeof (bool), true),
      }));
    }
  }
}