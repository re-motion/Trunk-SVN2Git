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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class FindUser : UserTestBase
  {
    private DatabaseFixtures _dbFixtures;
    private ObjectID _expectedTenantID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantID = tenant.ID;
    }

    [Test]
    public void FindByUserName_ValidUser ()
    {
      User foundUser = User.FindByUserName ("test.user");

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName ("not.existing");

      Assert.IsNull (foundUser);
    }

    [Test]
    public void Find_UsersByTenantID ()
    {
      DomainObjectCollection users = User.FindByTenantID (_expectedTenantID);

      Assert.AreEqual (5, users.Count);
    }
  }
}
