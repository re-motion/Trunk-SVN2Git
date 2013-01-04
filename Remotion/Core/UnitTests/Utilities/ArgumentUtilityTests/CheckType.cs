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
      Assert.That (result, Is.EqualTo (null));
		}

		[Test]
		public void Succeed_String ()
		{
			string result = ArgumentUtility.CheckType<string> ("arg", "test");
		  Assert.That (result, Is.EqualTo ("test"));
		}

    [Test]
    public void Succeed_BaseType ()
    {
      string result = (string) ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
    }
  }
}
