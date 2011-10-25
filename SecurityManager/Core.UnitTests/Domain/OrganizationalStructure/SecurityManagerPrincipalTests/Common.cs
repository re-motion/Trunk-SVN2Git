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
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Common : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
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
          delegate
          {
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
    public void ActiveSecurityProviderAddsSecurityClientTransactionExtension ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      var bindingTransaction = principal.User.GetBindingTransaction ();
      Assert.That (bindingTransaction.Extensions, Has.Some.InstanceOf<SecurityClientTransactionExtension> ());
    }

    [Test]
    public void NullSecurityProviderDoesNotAddSecurityClientTransactionExtension ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      securityProviderStub.Stub (stub => stub.IsNull).Return (true);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions ().First ();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant, user, substitution);

      var bindingTransaction = principal.User.GetBindingTransaction ();
      Assert.That (bindingTransaction.Extensions, Has.No.InstanceOf<SecurityClientTransactionExtension> ());
    }
  }
}