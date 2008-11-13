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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class TokenWithAbstractRole : SecurityTokenMatcherTestBase
  {
    [Test]
    public void EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (TestHelper.CreateTestAbstractRole ());

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForAbstractRole_Matches ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForOtherAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithAbstractRole (TestHelper.CreateTestAbstractRole ());

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}