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
  public class OnCommitTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void ClearSpecificTenant ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.IsNotNull (ace.SpecificTenant);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsNull (ace.SpecificTenant);
      }
    }

    [Test]
    public void ClearTenantHierarchyCondition ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.None;

        Assert.That (ace.TenantHierarchyCondition, Is.Not.EqualTo (TenantHierarchyCondition.Undefined));
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.TenantHierarchyCondition, Is.EqualTo (TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearTenantHierarchyCondition_IfOwningTenant ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.TenantHierarchyCondition, Is.Not.EqualTo (TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearTenantHierarchyCondition_IfSpecificTenant ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.SpecificTenant;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.TenantHierarchyCondition, Is.Not.EqualTo (TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void ClearSpecificGroup ()
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
    public void ClearGroupHierarchyCondition ()
    {
      var group = _testHelper.CreateGroup ("TestGroup", null, _testHelper.CreateTenant ("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup (group);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.None;

        Assert.That (ace.GroupHierarchyCondition, Is.Not.EqualTo (GroupHierarchyCondition.Undefined));
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearGroupHierarchyCondition_IfOwningGroup ()
    {
      var group = _testHelper.CreateGroup ("TestGroup", null, _testHelper.CreateTenant ("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup (group);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.GroupHierarchyCondition, Is.Not.EqualTo (GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearGroupHierarchyCondition_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup ("TestGroup", null, _testHelper.CreateTenant ("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup (group);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.SpecificGroup;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.That (ace.GroupHierarchyCondition, Is.Not.EqualTo (GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void ClearSpecificGroupType ()
    {
      var groupType = GroupType.NewObject ();
      var ace = _testHelper.CreateAceWithSpecificGroupType (groupType);
      ace.GroupCondition = GroupCondition.BranchOfOwningGroup;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.None;

        Assert.IsNotNull (ace.SpecificGroupType);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNull (ace.SpecificGroupType);
      }
    }

    [Test]
    public void DoNotClearSpecificGroupType_IfAnyGroupWithSpecificGroupType ()
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
    public void DoNotClearSpecificGroupType_IfBranchOfOwningGroup ()
    {
      var groupType = GroupType.NewObject ();
      var ace = _testHelper.CreateAceWithSpecificGroupType (groupType);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.GroupCondition = GroupCondition.BranchOfOwningGroup;

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNotNull (ace.SpecificGroupType);
      }
    }

    [Test]
    public void ClearSpecificUser ()
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
    public void ClearSpecificPosition ()
    {
      var ace = _testHelper.CreateAceWithPosition (_testHelper.CreatePosition ("Position"), GroupCondition.None);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.IsNotNull (ace.SpecificPosition);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsNull (ace.SpecificPosition);
      }
    }

    [Test]
    public void DoNotAccessTenantConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant ("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.IsNotNull (ace.SpecificTenant);
        ace.Delete ();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
    }

    [Test]
    public void DoNotAccessGroupConditionWhenObjectIsDeleted_DoesNotThrow ()
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
    public void DoNotAccessUserConditionWhenObjectIsDeleted_DoesNotThrow ()
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