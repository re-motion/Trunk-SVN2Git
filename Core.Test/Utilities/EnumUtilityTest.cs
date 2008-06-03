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

namespace Remotion.UnitTests.Utilities
{
	public enum TestEnum
	{
		Negative = -1,
		Zero = 0,
		Positive = 1
	}

	[Flags]
	public enum TestFlags
	{
		Flag1 = 1,
		Flag2 = 2,
		Flag3 = 4,
		Flag4 = 8,
		Flag1and2 = Flag1 | Flag2,
		AllFlags = Flag1and2 | Flag3 | Flag4
	}

	[TestFixture]
	public class EnumUtilityTest
	{
		[Test]
		public void TestIsValidEnumValue1 ()
		{
			Assert.AreEqual (false, EnumUtility.IsValidEnumValue ( (TestEnum) (-3)));
		}
		[Test]
		public void TestIsValidEnumValue2 ()
		{
			Assert.AreEqual (false, EnumUtility.IsValidEnumValue ( (TestEnum) 3));
		}

		[Test]
		public void TestIsValidEnumValue3 ()
		{
			Assert.AreEqual (true, EnumUtility.IsValidEnumValue ( TestEnum.Negative));
		}
		[Test]
		public void TestIsValidEnumValue4 ()
		{
			Assert.AreEqual (true, EnumUtility.IsValidEnumValue ( TestEnum.Zero));
		}
		[Test]
		public void TestIsValidEnumValue5 ()
		{
			Assert.AreEqual (true, EnumUtility.IsValidEnumValue ( TestEnum.Positive));
		}

		[Test]
		public void TestIsValidEnumFlags1 ()
		{
			Assert.AreEqual (false, EnumUtility.IsValidEnumValue ( (TestFlags) (-3) ));
		}
		[Test]
		public void TestIsValidEnumFlags2 ()
		{
			Assert.AreEqual (false, EnumUtility.IsValidEnumValue ( TestFlags.Flag1 | ((TestFlags) 16) ));
		}
		[Test]
		public void TestIsValidEnumFlags3 ()
		{
			 Assert.AreEqual (true, EnumUtility.IsValidEnumValue ( TestFlags.Flag1 | TestFlags.Flag3 ));
		}
		[Test]
		public void TestIsValidEnumFlags4 ()
		{
			Assert.AreEqual (true, EnumUtility.IsValidEnumValue ( TestFlags.Flag1 | TestFlags.Flag2 | TestFlags.Flag3 ));
		}
	}
}
