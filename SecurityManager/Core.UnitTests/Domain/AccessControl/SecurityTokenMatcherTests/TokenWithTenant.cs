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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class TokenWithTenant : SecurityTokenMatcherTestBase
  {
    [Test]
    public void EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (null, entry.SpecificTenant);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForOwningTenant_Matches ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, tenant);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForOtherOwningTenant_DoesNotMatch ()
    {
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, TestHelper.CreateTenant ("Other Tenant"));

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForSpecificTenant_Matches ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithSpecficTenant (tenant);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, null, null, new AbstractRoleDefinition[0]);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForOtherSpecificTenant_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithSpecficTenant (TestHelper.CreateTenant ("Other Tenant"));
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, null, null, new AbstractRoleDefinition[0]);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

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
  }
}