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
  public class MixedAces : SecurityTokenMatcherTestBase
  {
    [Test]
    public void AceForOwningTenantAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, tenant);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForPositionFromOwningGroupAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = TestHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = TestHelper.CreateAceWithPosition (managerPosition, GroupCondition.OwningGroup);
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningGroups (user, group);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}