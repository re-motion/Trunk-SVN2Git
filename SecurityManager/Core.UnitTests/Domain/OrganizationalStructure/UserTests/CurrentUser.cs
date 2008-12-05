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
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class CurrentUser : UserTestBase
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
    public void Get_Current_NotInitialized ()
    {
      Assert.IsNull (User.Current);
    }

    [Test]
    public void SetAndGet_Current ()
    {
      DomainObjectCollection users = User.FindByTenantID (_expectedTenantID);
      Assert.Greater (users.Count, 0);
      User user = (User) users[0];

      User.Current = user;
      Assert.AreSame (user, User.Current);

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.AreEqual (user.ID, User.Current.ID);
        Assert.AreNotSame (user, User.Current);
      }

      User.Current = null;
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      DomainObjectCollection users = User.FindByTenantID (_expectedTenantID);
      Assert.Greater (users.Count, 0);
      User user = (User) users[0];

      User.Current = user;
      Assert.AreSame (user, User.Current);

      ThreadRunner.Run (
          delegate ()
          {
            User otherUser = CreateUser();

            Assert.IsNull (User.Current);
            User.Current = otherUser;
            using (TestHelper.Transaction.EnterNonDiscardingScope())
            {
              Assert.AreSame (otherUser, User.Current);
            }
          });

      Assert.AreSame (user, User.Current);
    }
  }
}
