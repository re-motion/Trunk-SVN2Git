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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetAllowedAccessTypes(), Is.EquivalentTo (new[] { readAccessType }));
    }

    [Test]
    public void GetDeniedAccessTypes_DeleteDenied ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetDeniedAccessTypes(), Is.EquivalentTo (new[] { deleteAccessType }));
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    public void DenyAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.DenyAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That (allowedAccessTypes, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType")]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AllowAccess (accessType);
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);

      ace.RemoveAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (0, allowedAccessTypes.Length);
    }

    [Test]
    public void AttachAccessType_TwoNewAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType0 = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Access Type 0", 0);
      AccessTypeDefinition accessType1 = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Access Type 1", 1);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, ace.State);

        ace.AttachAccessType (accessType0);
        ace.AttachAccessType (accessType1);

        Assert.AreEqual (2, ace.Permissions.Count);
        Permission permission0 = ace.Permissions[0];
        Assert.AreSame (accessType0, permission0.AccessType);
        Assert.AreEqual (0, permission0.Index);
        Permission permission1 = ace.Permissions[1];
        Assert.AreSame (accessType1, permission1.AccessType);
        Assert.AreEqual (1, permission1.Index);
        Assert.AreEqual (StateType.Changed, ace.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The access type 'Test' has already been attached to this access control entry.\r\nParameter name: accessType")]
    public void AttachAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AttachAccessType (accessType);
      ace.AttachAccessType (accessType);
    }

    [Test]
    public void Get_PermissionsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAndCommitAccessControlEntryWithPermissions (10, ClientTransaction.CreateRootTransaction());

      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);

      Assert.AreEqual (10, ace.Permissions.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (string.Format ("Access Type {0}", i), (ace.Permissions[i]).AccessType.Name);
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreEqual (StateType.New, ace.State);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreEqual (StateType.New, ace.State);

      ace.Touch();

      Assert.AreEqual (StateType.New, ace.State);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ace.Index = 1;
      Assert.AreEqual (1, ace.Index);
    }

    [Test]
    public void ClearSpecificTenantOnCommit ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.IsNotNull (ace.SpecificTenant);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsNull (ace.SpecificTenant);
      }
    }

    [Test]
    public void ClearSpecificGroupOnCommit ()
    {
      var group = _testHelper.CreateGroup ("TestGroup", null, _testHelper.CreateTenant ("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup (group);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        Assert.IsNotNull (ace.SpecificGroup);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsNull (ace.SpecificGroup);
      }
    }

    [Test]
    public void ClearSpecificGroupTypeOnCommit ()
    {
      var groupType = GroupType.NewObject ();
      var ace = _testHelper.CreateAceWithSpecificGroupType (groupType);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.AnyGroupWithSpecificGroupType;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNotNull (ace.SpecificGroupType);
      }
    }

    [Test]
    public void DoNotClearSpecificGroupTypeOnCommit_IfAnyGroupWithSpecificGroupType ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType (groupType);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.BranchOfOwningGroup;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNotNull (ace.SpecificGroupType);
      }
    }

    [Test]
    public void DoNotClearSpecificGroupTypeOnCommit_IfBranchOfOwningGroup ()
    {
      var groupType = GroupType.NewObject ();
      var ace = _testHelper.CreateAceWithSpecificGroupType (groupType);
      ace.GroupCondition = GroupCondition.BranchOfOwningGroup;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        Assert.IsNotNull (ace.SpecificGroupType);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNull (ace.SpecificGroupType);
      }
    }

    [Test]
    public void ClearSpecificUserOnCommit ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var user = _testHelper.CreateUser ("TestUser", "user", "user", null, _testHelper.CreateGroup ("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser (user);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.IsNotNull (ace.SpecificUser);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNull (ace.SpecificUser);
      }
    }

    [Test]
    public void ClearSpecificPositionOnCommit ()
    {
      var ace = _testHelper.CreateAceWithOwningUser ();
      ace.UserCondition = UserCondition.SpecificUser;
      ace.SpecificPosition = _testHelper.CreatePosition ("Position");
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.IsNotNull (ace.SpecificPosition);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNull (ace.SpecificPosition);
      }
    }

    [Test]
    public void DoNotAccessTenantConditionOnCommitWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.IsNotNull (ace.SpecificTenant);
        ace.Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
    }

    [Test]
    public void DoNotAccessGroupConditionOnCommitWhenObjectIsDeleted_DoesNotThrow ()
    {
      var group = _testHelper.CreateGroup ("TestGroup", null, _testHelper.CreateTenant ("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup (group);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        Assert.IsNotNull (ace.SpecificGroup);
        ace.Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
    }

    [Test]
    public void DoNotAccessUserConditionOnCommitWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var user = _testHelper.CreateUser ("TestUser", "user", "user", null, _testHelper.CreateGroup ("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser (user);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.IsNotNull (ace.SpecificUser);
        ace.Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
    }
  }
}