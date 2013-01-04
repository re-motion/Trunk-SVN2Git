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
      Assert.That (result, Is.EqualTo (1));
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
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableInt_Null ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", null);
      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", 1);
      Assert.That (result, Is.EqualTo (1));
    }

		[Test]
		public void Succeed_String ()
		{
      string result = ArgumentUtility.CheckType<string> ("arg", "test");
		  Assert.That (result, Is.EqualTo ("test"));
		}    
    
    [Test]
    public void Succeed_StringNull ()
    {
      string result = ArgumentUtility.CheckType<string> ("arg", null);
      Assert.That (result, Is.EqualTo (null));
    }

	  private enum TestEnum
	  {
	    TestValue
	  } 

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckType<TestEnum> ("arg", TestEnum.TestValue);
      Assert.That (result, Is.EqualTo (TestEnum.TestValue));
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
      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
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
