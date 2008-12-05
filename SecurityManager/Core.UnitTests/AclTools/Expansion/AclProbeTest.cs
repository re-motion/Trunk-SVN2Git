// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclProbeTest : AclToolsTestBase
  {
    [Test]
    public void CreateAclProbe_User_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.Principal, Is.EqualTo (User));
    }


    [Test]
    public void CreateAclProbe_OwningGroup_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningGroup, Is.SameAs (Role.Group));

      var accessConditionsExpected = new AclExpansionAccessConditions
                                     {
                                         OwningGroup = Group,
                                         GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren
                                     };
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_GroupSelectionAll_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithoutGroupCondition ();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningGroup, Is.Null);

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "ace.GroupSelection=2147483647 is currently not supported by this method. Please extend method to handle the new GroupSelection state.")]
    public void CreateAclProbe_UnsupportedGroupSelection_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithoutGroupCondition ();
      FleshOutAccessControlEntryForTest (ace);
      ace.GroupCondition = (GroupCondition) int.MaxValue;
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
    }


    [Test]
    public void CreateAclProbe_SpecificTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithSpecificTenant (Tenant);
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.Null);

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_OwningTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.EqualTo (User.Tenant));

      var accessConditionsExpected = new AclExpansionAccessConditions
      {
        OwningTenant = Tenant,
        TenantHierarchyCondition = TenantHierarchyCondition.This
      };
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "ace.TenantSelection=-1 is currently not supported by this method. Please extend method to handle the new TenantSelection state.")]
    public void CreateAclProbe_UnsupportedTenantSelection_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      FleshOutAccessControlEntryForTest (ace);
      ace.TenantCondition = (TenantCondition) (object) -1;
      AclProbe.CreateAclProbe (User, Role, ace); 
    }


    [Test]
    public void CreateAclProbe_SpecificAbstractRole_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest (ace);
      Assert.That (ace.SpecificAbstractRole, Is.Not.Null);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.AbstractRoles, List.Contains (ace.SpecificAbstractRole));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      accessConditionsExpected.AbstractRole = ace.SpecificAbstractRole;
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }


    private void FleshOutAccessControlEntryForTest (AccessControlEntry ace)
    {
      ace.SpecificGroup = TestHelper.CreateGroup ("Specific Group for an ACE", null, Tenant);
      ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
    }
  }
}
