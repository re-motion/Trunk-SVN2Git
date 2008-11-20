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
  public class MixedTokens : SecurityTokenMatcherTestBase
  {
    [Test]
    public void EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (null, entry.SpecificTenant);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleAndAbstractRole_AceForPositionFromOwningGroup_Matches ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = TestHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = TestHelper.CreateAceWithPositionAndGroupCondition (managerPosition, GroupCondition.OwningGroup);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, null, group, null, new[] { TestHelper.CreateTestAbstractRole () });

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenantAndAbstractRole_AceForOwningTenant_Matches ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, tenant, null, null, new[] { TestHelper.CreateTestAbstractRole () });

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    //[Test]
    //public void EmptyToken_AceFilter_Matches ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  SecurityToken token = TestHelper.CreateEmptyToken ();
    //  token.SpecificAce = entry; 
    //  Assert.IsTrue (entry.MatchesToken (token));
    //}

    //[Test]
    //public void EmptyToken_AceFilter_DoesNotMatch ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  AccessControlEntry entryOther = AccessControlEntry.NewObject ();
    //  SecurityToken token = TestHelper.CreateEmptyToken ();
    //  token.SpecificAce = entryOther;
    //  Assert.IsFalse (entry.MatchesToken (token));
    //  Assert.IsTrue (entryOther.MatchesToken (token));
    //}
  }
}