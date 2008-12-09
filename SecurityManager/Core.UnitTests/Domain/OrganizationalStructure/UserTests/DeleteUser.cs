// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class DeleteUser : UserTestBase
  {
    [Test]
    public void DeleteUser_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        var user = testHelper.CreateUser ("user", null, "user", null, null, testHelper.CreateTenant ("tenant"));
        var ace = testHelper.CreateAceWithSpecificUser (user);

        user.Delete();

        Assert.IsTrue (ace.IsDiscarded);
      }
    }

    [Test]
    public void DeleteUser_WithRole ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = testHelper.CreateTenant ("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
        Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
        User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition ("Position");
        Role role = testHelper.CreateRole (user, roleGroup, position);

        user.Delete();

        Assert.IsTrue (role.IsDiscarded);
      }
    }

    [Test]
    public void DeleteUser_WithSubstitutionAsSubstitutingUser ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        User substitutingUser = testHelper.CreateUser ("user", null, "Lastname", null, null, null);
        Substitution substitution = substitutingUser.CreateSubstitution();

        substitutingUser.Delete ();

        Assert.IsTrue (substitution.IsDiscarded);
      }
    }

    [Test]
    public void DeleteUser_WithSubstitutionAsSubstitutedUser ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        User substitutingUser = testHelper.CreateUser ("user", null, "Lastname", null, null, null);
        User substitutedUser = testHelper.CreateUser ("user", null, "Lastname", null, null, null);
        Substitution substitution = substitutingUser.CreateSubstitution ();
        substitution.SubstitutedUser = substitutedUser;

        substitutedUser.Delete ();

        Assert.IsTrue (substitution.IsDiscarded);
      }
    }
  }
}
