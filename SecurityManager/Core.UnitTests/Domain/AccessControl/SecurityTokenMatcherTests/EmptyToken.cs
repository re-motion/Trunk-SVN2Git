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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class EmptyToken : SecurityTokenMatcherTestBase
  {
    [Test]
    public void EmptyToken_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForOwningTenant_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromOwningGroup_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      AccessControlEntry entry = TestHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromAllGroups_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      AccessControlEntry entry = TestHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateEmptyToken ();

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}