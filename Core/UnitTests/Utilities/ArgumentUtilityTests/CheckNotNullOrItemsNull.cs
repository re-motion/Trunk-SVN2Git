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

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullOrItemsNull
	{
    [Test]
    public void Succeed_ICollection ()
    {
      ArrayList list = new ArrayList ();
      ArrayList result = ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

	  [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullICollection ()
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", (ICollection) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList ();
      list.Add (null);
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void Fail_zItemNullIEnumerable ()
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", GetEnumerableWithNullValue());
    }

    private IEnumerable GetEnumerableWithNullValue ()
    {
      yield return null;
    }
	}
}
