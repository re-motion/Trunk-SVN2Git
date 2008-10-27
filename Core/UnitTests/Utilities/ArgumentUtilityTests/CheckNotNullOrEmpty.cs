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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrEmpty
  {
    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Fail_NullString()
    {
      const string value = null;
      ArgumentUtility.CheckNotNullOrEmpty ("arg", value);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyString()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", "");
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyArray()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[0]);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyCollection()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new ArrayList());
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyIEnumerable ()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", GetEmptyEnumerable());
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (false);
      ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
    }

    [Test]
    public void Succeed_String()
    {
      string result = ArgumentUtility.CheckNotNullOrEmpty ("arg", "Test");
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Succeed_Array()
    {
      var array = new[] {"test"};
      string[] result = ArgumentUtility.CheckNotNullOrEmpty ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection()
    {
      var list = new ArrayList {"test"};
      ArrayList result = ArgumentUtility.CheckNotNullOrEmpty ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable = GetEnumerableWithValue();
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
      Assert.That (result, Is.SameAs (enumerable));
      Assert.That (result.GetEnumerator().MoveNext(), Is.True);
    }

    [Test]
    public void Succeed_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (true);
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
      Assert.That (result, Is.SameAs (enumerable));
      Assert.That (result.GetEnumerator ().MoveNext (), Is.True);
    }

    private IEnumerable GetEnumerableWithValue ()
    {
      yield return "test";
    }

    private IEnumerable GetEmptyEnumerable ()
    {
      yield break;
    }
  }
}
