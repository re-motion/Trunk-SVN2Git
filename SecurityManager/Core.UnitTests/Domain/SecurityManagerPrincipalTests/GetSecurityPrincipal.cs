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
using Remotion.Collections;
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
  public class GetSecurityPrincipal : DomainTest
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
    public void Test ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().Where (s => s.SubstitutedRole != null).First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();
      Assert.That (securityPrincipal.IsNull, Is.False);
      Assert.That (securityPrincipal.User, Is.EqualTo (user.UserName));
      Assert.That (securityPrincipal.Role, Is.Null);
      Assert.That (securityPrincipal.SubstitutedUser, Is.EqualTo (substitution.SubstitutedUser.UserName));
      Assert.That (securityPrincipal.SubstitutedRole.Group, Is.EqualTo (substitution.SubstitutedRole.Group.UniqueIdentifier));
      Assert.That (securityPrincipal.SubstitutedRole.Position, Is.EqualTo (substitution.SubstitutedRole.Position.UniqueIdentifier));
    }

    [Test]
    public void UsesCache ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, null);

      Assert.That (principal.GetSecurityPrincipal(), Is.SameAs (principal.GetSecurityPrincipal()));
    }

    [Test]
    public void DoesNotCacheDuringSerialization ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, null);

      var deserialized = Serializer.SerializeAndDeserialize (Tuple.Create (principal, principal.GetSecurityPrincipal()));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      ISecurityPrincipal deserialziedSecurityPrincipal = deserialized.Item2;

      Assert.That (deserialziedSecurityManagerPrincipal.GetSecurityPrincipal (), Is.Not.SameAs (deserialziedSecurityPrincipal));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().Where (s => s.SubstitutedRole != null).First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();
      Assert.That (securityPrincipal.IsNull, Is.False);
      Assert.That (securityPrincipal.User, Is.EqualTo (user.UserName));
    }
  }
}