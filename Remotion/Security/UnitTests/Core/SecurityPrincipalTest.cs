// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class SecurityPrincipalTest
  {
    [Test]
    public void Initialize_WithUser ()
    {
      var principal = new SecurityPrincipal ("TheUser", null, null, null);

      Assert.That (principal.User, Is.EqualTo ("TheUser"));
      Assert.That (principal.Role, Is.Null);
      Assert.That (principal.SubstitutedUser, Is.Null);
      Assert.That (principal.SubstitutedRole, Is.Null);
    }

    [Test]
    public void Initialize_WithUserAndRoleAndSubstitutedUserAndSubstitedRole ()
    {
      var role = new SecurityPrincipalRole ("TheGroup", null);
      var substitutedRole = new SecurityPrincipalRole ("SomeGroup", null);
      var principal = new SecurityPrincipal ("TheUser", role, "SomeUser", substitutedRole);

      Assert.That (principal.User, Is.EqualTo ("TheUser"));
      Assert.That (principal.Role, Is.SameAs (role));
      Assert.That (principal.SubstitutedUser, Is.EqualTo ("SomeUser"));
      Assert.That (principal.SubstitutedRole, Is.SameAs (substitutedRole));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: user")]
    public void Initialize_WithoutGroup ()
    {
      new SecurityPrincipal (null, null, null, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException), ExpectedMessage =
        "Parameter 'user' cannot be empty.\r\nParameter name: user")]
    public void Initialize_WithUserEmpty ()
    {
      new SecurityPrincipal (string.Empty, null, null, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentEmptyException), ExpectedMessage =
        "Parameter 'substitutedUser' cannot be empty.\r\nParameter name: substitutedUser")]
    public void Initialize_WithSubstitutedUserEmpty ()
    {
      new SecurityPrincipal ("TheUser", null, string.Empty, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The substituted user must be specified if a substituted role is also specified.\r\nParameter name: substitutedUser")]
    public void Initialize_WithSubstitutedUserMissing ()
    {
      new SecurityPrincipal ("TheUser", null, null, new SecurityPrincipalRole("group", "position"));
    }

    [Test]
    public void Equals_WithEqualUser ()
    {
      var left = CreatePrincipal ("TheUser", null, null, null);
      var right = CreatePrincipal ("TheUser", null, null, null);

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithAllEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithUserNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("OtherUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithRoleNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "OtherGroup", "SomeUser", "SomeGroup");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithSubstitutedUserNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "OtherUser", "SomeGroup");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithSubstitutedRoleNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "OtherGroup");

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = (SecurityPrincipal) null;

      Assert.IsFalse (left.Equals (right));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.IsFalse (left.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.IsFalse (left.Equals (new object()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.GetHashCode(), Is.EqualTo (right.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var principal = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      var deserializedRole = Serializer.SerializeAndDeserialize (principal);

      Assert.AreNotSame (principal, deserializedRole);
      Assert.AreEqual (principal, deserializedRole);
    }

    [Test]
    public void IsNull ()
    {
      var principal = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      
      Assert.That (principal.IsNull, Is.False);
    }

    private SecurityPrincipal CreatePrincipal (string user, string roleGroup, string substitutedUser, string substitutedRoleGroup)
    {
      var role = roleGroup != null ? new SecurityPrincipalRole (roleGroup, null) : null;
      var substitutedRole = substitutedRoleGroup != null ? new SecurityPrincipalRole (substitutedRoleGroup, null) : null;
      return new SecurityPrincipal (user, role, substitutedUser, substitutedRole);
    }
  }
}
