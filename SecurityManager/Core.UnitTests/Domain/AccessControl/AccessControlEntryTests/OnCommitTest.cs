// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
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
      var ace = _testHelper.CreateAceWithPositionAndGroupCondition (_testHelper.CreatePosition ("Position"), GroupCondition.None);
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

    [Test]
    public void TouchClass ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      StatelessAccessControlList acl = _testHelper.CreateStatelessAcl (classDefinition);
      var ace = _testHelper.CreateAceWithOwningUser();
      acl.AccessControlEntries.Add (ace);
     
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += delegate { commitOnClassWasCalled = true; };
        ace.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That (commitOnClassWasCalled, Is.True);
      }
    }
  }
}
