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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Refresh : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }
    
    [Test]
    public void SameRevisionDoesNotChangeTransaction ()
    {
      var user = User.FindByUserName ("substituting.user");
      var tenant = user.Tenant;

      var principal = new SecurityManagerPrincipal (tenant.GetHandle(), user.GetHandle(), null);
      
      var oldTenant = principal.Tenant;
      var oldUser = principal.User;
      var oldSubstitution = principal.Substitution;

      principal.Refresh ();

      Assert.That (principal.Tenant, Is.SameAs (oldTenant));
      Assert.That (principal.User, Is.SameAs (oldUser));
      Assert.That (principal.Substitution, Is.SameAs (oldSubstitution));
    }

    [Test]
    public void NewRevisionResetsTransaction ()
    {
      var user = User.FindByUserName ("substituting.user");
      var helper = new OrganizationalStructure.OrganizationalStructureTestHelper (ClientTransaction.Current);
      var user2 = helper.CreateUser ("RevisionTestUser", "FN", "LN", null, user.OwningGroup, user.Tenant);
      ClientTransaction.Current.Commit();
      var tenant = user2.Tenant;

      var principal = new SecurityManagerPrincipal (tenant.GetHandle(), user2.GetHandle(), null);

      var oldUser = principal.User;
    
      user2.LastName = "New LN";
      ClientTransaction.Current.Commit ();
      IncrementRevision ();

      principal.Refresh();

      Assert.That (principal.User.ID, Is.EqualTo (oldUser.ID));
      Assert.That (principal.User.DisplayName, Is.Not.EqualTo (oldUser.DisplayName));
    }

    private void IncrementRevision ()
    {
      ClientTransaction.Current.QueryManager.GetScalar (Revision.GetIncrementRevisionQuery());
    }
  }
}