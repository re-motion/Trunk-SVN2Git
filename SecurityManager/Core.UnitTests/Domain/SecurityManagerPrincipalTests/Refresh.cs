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
using System.Linq;
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
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      var oldTenant = principal.Tenant;
      var oldUser = principal.User;
      var oldSubstitution = principal.Substitution;

      ClientTransactionScope.ResetActiveScope ();

      principal.Refresh ();

      Assert.Ignore();
      Assert.That (principal.Tenant, Is.SameAs (oldTenant));
      Assert.That (principal.User, Is.SameAs (oldUser));
      Assert.That (principal.Substitution, Is.SameAs (oldSubstitution));
    }

    [Test]
    public void NewRevisionResetsTransaction ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      var oldTenant = principal.Tenant;
      var oldUser = principal.User;
      var oldSubstitution = principal.Substitution;

      Revision.IncrementRevision ();

      ClientTransactionScope.ResetActiveScope ();

      principal.Refresh ();

      Assert.Ignore ();
      Assert.That (principal.Tenant.ID, Is.EqualTo (oldTenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (oldTenant));

      Assert.That (principal.User.ID, Is.EqualTo (oldUser.ID));
      Assert.That (principal.User, Is.Not.SameAs (oldUser));

      Assert.That (principal.Substitution.ID, Is.EqualTo (oldSubstitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (oldSubstitution));
    }
  }
}