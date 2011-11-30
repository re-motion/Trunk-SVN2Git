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
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetMemberFromExpression
  {
    private int _privateTestField = 0;

    [Test]
    public void Property ()
    {
      var member = ReflectionUtility.GetMemberFromExpression ((ClassWithDifferentProperties t) => t.Int32);

      Assert.That (member, Is.Not.Null);
      Assert.That (member, Is.SameAs (typeof (ClassWithDifferentProperties).GetProperty ("Int32")));
    }

    [Test]
    public void Field ()
    {
      var member = ReflectionUtility.GetMemberFromExpression ((ClassWithFields t) => t.Int32Field);

      Assert.That (member, Is.Not.Null);
      Assert.That (member, Is.SameAs (typeof (ClassWithFields).GetField ("Int32Field")));
    }

    [Test]
    public void PrivateMember ()
    {
      var member = ReflectionUtility.GetMemberFromExpression ((GetMemberFromExpression t) => t._privateTestField);

      Assert.That (member, Is.Not.Null);
      Assert.That (member, Is.SameAs (typeof (GetMemberFromExpression).GetField ("_privateTestField", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void InvalidExpression ()
    {
      Assert.That (
          () => ReflectionUtility.GetMemberFromExpression ((GetMemberFromExpression t) => t.ToString()),
          Throws.ArgumentException.With.Message.EqualTo (
              "The memberAccessExpression must be a simple member access expression.\r\nParameter name: memberAccessExpression"));
    }
  }
}