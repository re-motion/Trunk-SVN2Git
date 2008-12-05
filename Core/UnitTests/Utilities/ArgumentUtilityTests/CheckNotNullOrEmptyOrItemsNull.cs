// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullOrEmptyOrItemsNull
	{
	  [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullICollection ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", (ICollection) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList ();
      list.Add (null);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyArray ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new string[0]);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new ArrayList ());
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyIEnumerable ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", GetEmptyEnumerable());
    }

    [Test]
    public void Succeed_Array ()
    {
      string[] array = new string[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      ArrayList list = new ArrayList ();
      list.Add ("test");
      ArrayList result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable  = GetEnumerableWithValue();
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", enumerable);
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
