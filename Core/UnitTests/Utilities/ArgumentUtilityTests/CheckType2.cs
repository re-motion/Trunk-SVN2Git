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
	public class CheckType2
	{
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]

    [Test]
    public void Succeed_Int ()
    {
      int result = ArgumentUtility.CheckType<int> ("arg", 1);
      Assert.AreEqual (1, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))] 
    public void Fail_Int_Null ()
    {
      ArgumentUtility.CheckType<int> ("arg", null);
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckType<int> ("arg", (int?) 1);
      Assert.AreEqual (1, result);
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", (int?) 1);
      Assert.AreEqual (1, result);
    }

    [Test]
    public void Succeed_NullableInt_Null ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", null);
      Assert.AreEqual (null, result);
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", 1);
      Assert.AreEqual (1, result);
    }

		[Test]
		public void Succeed_String ()
		{
      string result = ArgumentUtility.CheckType<string> ("arg", "test");
			Assert.AreEqual ("test", result);
		}    
    
    [Test]
    public void Succeed_StringNull ()
    {
      string result = ArgumentUtility.CheckType<string> ("arg", null);
      Assert.AreEqual (null, result);
    }

	  private enum TestEnum
	  {
	    TestValue
	  } 

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckType<TestEnum> ("arg", TestEnum.TestValue);
      Assert.AreEqual (TestEnum.TestValue, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))] 
    public void Fail_Enum_Null ()
    {
      ArgumentUtility.CheckType<TestEnum> ("arg", null);
    }

    [Test]
    public void Succeed_NullableEnum_Null ()
    {
      TestEnum? result = ArgumentUtility.CheckType<TestEnum?> ("arg", null);
      Assert.AreEqual (null, result);
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.AreEqual ("test", result);
    }
    
    [Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_String_Int ()
		{
      ArgumentUtility.CheckType<string> ("arg", 1);
		}

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Long_Int ()
    {
      ArgumentUtility.CheckType<long> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Int_String ()
    {
      ArgumentUtility.CheckType<int> ("arg", "test");
    }
  }
}
