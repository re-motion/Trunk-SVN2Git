// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetTenants : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ObjectID _rootTenantID;
    private ObjectID _childTenantID;
    private ObjectID _grandChildTenantID;


    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      _testHelper = new OrganizationalStructureTestHelper();

      _dbFixtures = new DatabaseFixtures();
      var tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (_testHelper.Transaction);

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant child = _testHelper.CreateTenant ("Child", "UID: Child");
        child.IsAbstract = true;
        child.Parent = tenant;
        Tenant grandChild = _testHelper.CreateTenant ("GrandChild", "UID: GrandChild");
        grandChild.Parent = child;
        ClientTransaction.Current.Commit();

        _rootTenantID = tenant.ID;
        _childTenantID = child.ID;
        _grandChildTenantID = grandChild.ID;
      }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void IncludeAbstractTenants ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, null);

      Assert.That (principal.GetTenants (true).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _childTenantID, _grandChildTenantID }));
    }

    [Test]
    public void ExcludeAbstractTenants ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, null);

      Assert.That (principal.GetTenants (false).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _grandChildTenantID }));
    }
  }
}