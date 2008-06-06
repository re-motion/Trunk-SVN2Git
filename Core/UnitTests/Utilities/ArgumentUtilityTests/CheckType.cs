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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckType
	{
		[Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
		{
			ArgumentUtility.CheckType<string> ("arg", 13);
		}

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType<int> ("arg", (object) null);
    }
    
    [Test]
		public void Succeed_Null ()
		{
			string result = ArgumentUtility.CheckType<string> ("arg", null);
			Assert.AreEqual (null, result);
		}

		[Test]
		public void Succeed_String ()
		{
			string result = ArgumentUtility.CheckType<string> ("arg", "test");
			Assert.AreEqual ("test", result);
		}

    [Test]
    public void Succeed_BaseType ()
    {
      string result = (string) ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.AreEqual ("test", result);
    }
  }
}
