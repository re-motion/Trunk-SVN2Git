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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Common : DomainTest
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
    public void Get_Current_NotInitialized ()
    {
      Assert.That (SecurityManagerPrincipal.Current.IsNull, Is.True);
    }

    [Test]
    public void SetAndGet_Current ()
    {
      User user = User.FindByUserName ("substituting.user");

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (user.Tenant.ID, user.ID, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (principal));
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      User user = User.FindByUserName ("substituting.user");

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (user.Tenant.ID, user.ID, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That (SecurityManagerPrincipal.Current, Is.SameAs (principal));

      ThreadRunner.Run (
          delegate
          {
            using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
            {
              User otherUser = User.FindByUserName ("group1/user1");
              SecurityManagerPrincipal otherPrincipal = new SecurityManagerPrincipal (otherUser.Tenant.ID, otherUser.ID, null);

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

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

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
    public void Serialization ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      var principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);
      var deserializedPrincipal = Serializer.SerializeAndDeserialize (principal);

      Assert.That (deserializedPrincipal.Tenant.ID, Is.EqualTo (principal.Tenant.ID));
      Assert.That (deserializedPrincipal.Tenant, Is.Not.SameAs (principal.Tenant));

      Assert.That (deserializedPrincipal.User.ID, Is.EqualTo (principal.User.ID));
      Assert.That (deserializedPrincipal.User, Is.Not.SameAs (principal.User));

      Assert.That (deserializedPrincipal.Substitution.ID, Is.EqualTo (principal.Substitution.ID));
      Assert.That (deserializedPrincipal.Substitution, Is.Not.SameAs (principal.Substitution));
    }

    [Test]
    public void Test_IsNull ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;

      ISecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, null);

      Assert.That (principal.IsNull, Is.False);
    }

    [Test]
    public void ActiveSecurityProviderAddsSecurityClientTransactionExtension ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      //Must test for observable effect
      Assert.That (principal.GetActiveSubstitutions(), Is.Empty);
    }

    [Test]
    public void NullSecurityProviderDoesNotAddSecurityClientTransactionExtension ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (true);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      //Must test for observable effect
      Assert.That (principal.GetActiveSubstitutions(), Is.Not.Empty);
    }
  }
}