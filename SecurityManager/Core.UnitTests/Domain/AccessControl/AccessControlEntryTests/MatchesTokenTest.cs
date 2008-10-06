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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class MatchesTokenTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    [Test]
    public void EmptyToken_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForOwningTenant_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningTenant ();
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromOwningGroup_DoesNotMatch ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_AceForPositionFromAllGroups_DoesNotMatch ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithTenant_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      SecurityToken token = _testHelper.CreateTokenWithOwningTenant (null, entry.SpecificTenant);
 
      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenant_AceForOwningTenant_Matches ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningTenant ();
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      SecurityToken token = _testHelper.CreateTokenWithOwningTenant (user, tenant);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenant_AceForOtherOwningTenant_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithOwningTenant ();
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      SecurityToken token = _testHelper.CreateTokenWithOwningTenant (user, _testHelper.CreateTenant ("Other Tenant"));

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenant_AceForSpecificTenant_Matches ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = _testHelper.CreateAceWithSpecficTenant (tenant);
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[0], new AbstractRoleDefinition[0]);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenant_AceForOtherSpecificTenant_DoesNotMatch ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = _testHelper.CreateAceWithSpecficTenant (_testHelper.CreateTenant ("Other Tenant"));
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[0], new AbstractRoleDefinition[0]);

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithAbstractRole_EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole ());

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_AceForAbstractRole_Matches ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithAbstractRole_AceForOtherAbstractRole_DoesNotMatch ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole ());

      Assert.IsFalse (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithRole_AceForPositionFromOwningGroup_Matches ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateTokenWithOwningGroups (user, group);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRole_AceForPositionFromAllGroups_Matches ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.All);
      SecurityToken token = _testHelper.CreateToken (user, null, null, null);

      Assert.IsTrue (entry.MatchesToken (token));
    }


    [Test]
    public void TokenWithRole_AceForPositionFromOwningGroupAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithOwningGroups (user, group);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleAndAbstractRole_AceForPositionFromOwningGroup_Matches ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition, GroupSelection.OwningGroup);
      SecurityToken token = _testHelper.CreateToken (user, null, new Group[] { group }, new AbstractRoleDefinition[] { _testHelper.CreateTestAbstractRole() });

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenant_AceForOwningTenantAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = _testHelper.CreateAceWithOwningTenant ();
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithOwningTenant (user, tenant);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void TokenWithTenantAndAbstractRole_AceForOwningTenant_Matches ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant");
      Position managerPosition = _testHelper.CreatePosition ("Manager");
      Group group = _testHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = _testHelper.CreateAceWithOwningTenant ();
      SecurityToken token = _testHelper.CreateToken (user, tenant, new Group[0], new AbstractRoleDefinition[] { _testHelper.CreateTestAbstractRole () });

      Assert.IsTrue (entry.MatchesToken (token));
    }

    //[Test]
    //public void EmptyToken_AceFilter_Matches ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  SecurityToken token = _testHelper.CreateEmptyToken ();
    //  token.SpecificAce = entry; 
    //  Assert.IsTrue (entry.MatchesToken (token));
    //}

    //[Test]
    //public void EmptyToken_AceFilter_DoesNotMatch ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  AccessControlEntry entryOther = AccessControlEntry.NewObject ();
    //  SecurityToken token = _testHelper.CreateEmptyToken ();
    //  token.SpecificAce = entryOther;
    //  Assert.IsFalse (entry.MatchesToken (token));
    //  Assert.IsTrue (entryOther.MatchesToken (token));
    //}


    private User CreateUser (Tenant tenant, Group group)
    {
      return _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, tenant);
    }
  }
}
