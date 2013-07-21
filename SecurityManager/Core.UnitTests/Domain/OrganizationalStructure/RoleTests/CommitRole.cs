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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests
{
  [TestFixture]
  public class CommitRole : RoleTestBase
  {
    private User _user;
    private Role _role;
    private Group _roleGroup2;
    private Substitution _substitution;

    public override void SetUp ()
    {
      base.SetUp ();
      var tenant = TestHelper.CreateTenant ("TestTenant", "UID: testTenant");
      var userGroup = TestHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      var roleGroup = TestHelper.CreateGroup ("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
      _roleGroup2 = TestHelper.CreateGroup ("RoleGroup2", Guid.NewGuid().ToString(), null, tenant);
      _user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
      var position = TestHelper.CreatePosition ("Position");
      _role = TestHelper.CreateRole (_user, roleGroup, position);

       var substitutingUser = TestHelper.CreateUser ("substitutingUser", "Firstname", "Lastname", "Title", userGroup, tenant);
      _substitution = Substitution.NewObject();
      _substitution.SubstitutedUser = _user;
      _substitution.SubstitutedRole = _role;
      _substitution.SubstitutingUser = substitutingUser;

      ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope();
    }

    [Test]
    public void WithUser_RegistersUserForCommit ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _role.Group = _roleGroup2;
        ClientTransaction.Current.Commit();
      }
      var dataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).GetDataContainerWithLazyLoad (_user.ID, true);
      Assert.That (dataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void WithRoleDeleted_RegistersOriginalUserForCommit ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _role.Delete();
        ClientTransaction.Current.Commit();
      }
      var dataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).GetDataContainerWithLazyLoad (_user.ID, true);
      Assert.That (dataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void WithSubstition_RegistersSubstitutionForCommit ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _role.Group = _roleGroup2;
        ClientTransaction.Current.Commit();
      }
      var dataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).GetDataContainerWithLazyLoad (_substitution.ID, true);
      Assert.That (dataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void WithRoleDeleted_DeletesSubstitution ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _role.Delete();
        ClientTransaction.Current.Commit();
      }
      var dataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).GetDataContainerWithLazyLoad (_substitution.ID, true);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));
    }
  }
}