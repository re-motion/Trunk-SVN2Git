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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndTypeIsAssignableFrom
	{
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", null, typeof (string));
    }
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Type ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }
    [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
      Assert.That (result, Is.SameAs (typeof (string)));
    }
	}
}
