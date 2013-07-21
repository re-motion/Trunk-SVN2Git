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

    public override void SetUp ()
    {
      base.SetUp ();
      Tenant tenant = TestHelper.CreateTenant ("TestTenant", "UID: testTenant");
      Group userGroup = TestHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      Group roleGroup = TestHelper.CreateGroup ("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
      _roleGroup2 = TestHelper.CreateGroup ("RoleGroup2", Guid.NewGuid().ToString(), null, tenant);
      _user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
      Position position = TestHelper.CreatePosition ("Position");
      _role = TestHelper.CreateRole (_user, roleGroup, position);
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
      var userDataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).DataContainers[_user.ID];
      Assert.That (userDataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void WithRoleDeleted_RegistersOriginalUserForCommit ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        _role.Delete();
        ClientTransaction.Current.Commit();
      }
      var userDataContainer = DataManagementService.GetDataManager (ClientTransaction.Current).DataContainers[_user.ID];
      Assert.That (userDataContainer.HasBeenMarkedChanged, Is.True);
    }
  }
}