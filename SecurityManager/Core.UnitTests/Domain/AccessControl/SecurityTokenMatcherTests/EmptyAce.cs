// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class EmptyAce:SecurityTokenMatcherTestBase
  {
    [Test]
    public void TokenWithAbstractRole_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (TestHelper.CreateTestAbstractRole ());

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithNullPrincipal_NotMatches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithNullPrincipal ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}
