// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerPrincipalTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Initialize_WithObjects ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      Assert.That (principal.Tenant.ID, Is.EqualTo (tenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (tenant));

      Assert.That (principal.User.ID, Is.EqualTo (user.ID));
      Assert.That (principal.User, Is.Not.SameAs (user));

      Assert.That (principal.Substitution.ID, Is.EqualTo (substitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (substitution));
    }

    [Test]
    public void Initialize_WithObjectIDs ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      Assert.That (principal.Tenant.ID, Is.EqualTo (tenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (tenant));

      Assert.That (principal.User.ID, Is.EqualTo (user.ID));
      Assert.That (principal.User, Is.Not.SameAs (user));

      Assert.That (principal.Substitution.ID, Is.EqualTo (substitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (substitution));
    }

    [Test]
    public void Refresh ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      Tenant oldTenant = principal.Tenant;
      User oldUser = principal.User;
      Substitution oldSubstitution = principal.Substitution;

      principal.Refresh();

      Assert.That (principal.Tenant.ID, Is.EqualTo (oldTenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (oldTenant));

      Assert.That (principal.User.ID, Is.EqualTo (oldUser.ID));
      Assert.That (principal.User, Is.Not.SameAs (oldUser));

      Assert.That (principal.Substitution.ID, Is.EqualTo (oldSubstitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (oldSubstitution));
    }

    [Test]
    public void Get_Current_NotInitialized ()
    {
      Assert.That (SecurityManagerPrincipal.Current.IsNull, Is.True);
    }

    [Test]
    public void SetAndGet_Current ()
    {
      User user = User.FindByUserName ("substituting.user");

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (user.Tenant, user, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (principal));
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      User user = User.FindByUserName ("substituting.user");

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (user.Tenant, user, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (principal));

      ThreadRunner.Run (
          delegate {
            using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
            {
              User otherUser = User.FindByUserName ("group1/user1");
              SecurityManagerPrincipal otherPrincipal = new SecurityManagerPrincipal (otherUser.Tenant, otherUser, null);

              Assert.That (SecurityManagerPrincipal.Current.IsNull, Is.True);
              SecurityManagerPrincipal.Current = otherPrincipal;
              Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (otherPrincipal));
            }
          });

      Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (principal));
    }

    [Test]
    public void GetValuesInNewTransaction ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That (principal.Tenant.ID, Is.EqualTo (tenant.ID));
        Assert.That (principal.Tenant, Is.Not.SameAs (tenant));

        Assert.That (principal.User.ID, Is.EqualTo (user.ID));
        Assert.That (principal.User, Is.Not.SameAs (user));

        Assert.That (principal.Substitution.ID, Is.EqualTo (substitution.ID));
        Assert.That (principal.Substitution, Is.Not.SameAs (substitution));
      }
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().Where (s => s.SubstitutedRole != null).First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();
      Assert.That (securityPrincipal.IsNull, Is.False);
      Assert.That (securityPrincipal.User, Is.EqualTo (user.UserName));
      Assert.That (securityPrincipal.Role, Is.Null);
      Assert.That (securityPrincipal.SubstitutedUser, Is.EqualTo (substitution.SubstitutedUser.UserName));
      Assert.That (securityPrincipal.SubstitutedRole.Group, Is.EqualTo (substitution.SubstitutedRole.Group.UniqueIdentifier));
      Assert.That (securityPrincipal.SubstitutedRole.Position, Is.EqualTo (substitution.SubstitutedRole.Position.UniqueIdentifier));
    }

    [Test]
    public void Serialization ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      var principal = new SecurityManagerPrincipal (tenant, user, substitution);
      var deserializedPrincipal = Serializer.SerializeAndDeserialize (principal);

      Assert.That (deserializedPrincipal.Tenant.ID, Is.EqualTo (principal.Tenant.ID));
      Assert.That (deserializedPrincipal.Tenant, Is.Not.SameAs (principal.Tenant));

      Assert.That (deserializedPrincipal.User.ID, Is.EqualTo (principal.User.ID));
      Assert.That (deserializedPrincipal.User, Is.Not.SameAs (principal.User));

      Assert.That (deserializedPrincipal.Substitution.ID, Is.EqualTo (principal.Substitution.ID));
      Assert.That (deserializedPrincipal.Substitution, Is.Not.SameAs (principal.Substitution));     
    }

    [Test]
    public void Get_IsNull ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      
      ISecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, null);

      Assert.That (principal.IsNull, Is.False);
    }
  }
}
