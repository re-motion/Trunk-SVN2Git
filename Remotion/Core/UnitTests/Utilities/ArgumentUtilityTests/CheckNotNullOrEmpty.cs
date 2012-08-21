// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  //Remotion.Linq.UnitTests.Utilities.ArgumentUtilityTests.CheckNotNullOrEmpty
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
