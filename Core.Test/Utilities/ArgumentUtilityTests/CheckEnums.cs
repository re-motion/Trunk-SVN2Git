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
  public enum TestEnum
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3
  }

  [Flags]
  public enum TestFlags
  {
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
    Flag13 = Flag1 | Flag3
  }

	[TestFixture]
	public class CheckValidEnumValue
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValue ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValue ("arg", (TestFlags) (1 | 8));
		}

    [Test]
		public void Succeed_SingleValue ()
    {
      Enum result = ArgumentUtility.CheckValidEnumValue ("arg", TestEnum.Value1);
      Assert.That (result, Is.EqualTo (TestEnum.Value1));
    }

	  [Test]
		public void Succeed_Flags ()
	  {
      Enum result = ArgumentUtility.CheckValidEnumValue ("arg", TestFlags.Flag1 | TestFlags.Flag2);
      Assert.That (result, Is.EqualTo (TestFlags.Flag1 | TestFlags.Flag2));
	  }
	}

	[TestFixture]
	public class CheckValidEnumValueAndTypeAndNotNull
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentNullException))]
		public void Fail_Null ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", null);
		}
    
    [Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", (TestFlags) (1 | 8));
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentTypeException))]
    public void Fail_WrongType ()
    {
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", TestEnum.Value1);
    }

    [Test]
		public void Succeed_SingleValue ()
		{
      TestEnum result = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", TestEnum.Value1);
      Assert.AreEqual (TestEnum.Value1, result);
		}
		[Test]
		public void Succeed_Flags ()
		{
      TestFlags result = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", TestFlags.Flag1 | TestFlags.Flag2);
      Assert.AreEqual (TestFlags.Flag1 | TestFlags.Flag2, result);
		}
	}

	[TestFixture]
	public class CheckValidEnumValueAndType
	{
		[Test]
		public void Succeed_Null ()
		{
      TestEnum? result = ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", null);
      Assert.IsNull (result);
		}
    
    [Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", (TestFlags) (1 | 8));
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentTypeException))]
    public void Fail_WrongType ()
    {
      ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", TestEnum.Value1);
    }

    [Test]
		public void Succeed_SingleValue ()
		{
      TestEnum? result = ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", TestEnum.Value1);
      Assert.AreEqual (TestEnum.Value1, result);
		}
		[Test]
		public void Succeed_Flags ()
		{
      TestFlags? result = ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", TestFlags.Flag1 | TestFlags.Flag2);
      Assert.AreEqual (TestFlags.Flag1 | TestFlags.Flag2, result);
		}
	}
}
