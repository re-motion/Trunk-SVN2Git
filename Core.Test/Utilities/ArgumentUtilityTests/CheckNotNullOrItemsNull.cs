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
	}
}
