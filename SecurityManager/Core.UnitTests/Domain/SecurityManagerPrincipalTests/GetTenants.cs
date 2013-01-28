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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetTenants : DomainTest
  {
    private IObjectID<Tenant> _rootTenantID;
    private IObjectID<Tenant> _childTenantID;
    private IObjectID<Tenant> _grandChildTenantID;
    private IObjectID<User> _userID;

    public override void SetUp ()
    {
      base.SetUp ();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      User user = User.FindByUserName ("substituting.user");
      _userID = user.GetTypedID();
      _rootTenantID = user.Tenant.GetTypedID();
      _childTenantID = user.Tenant.Children.Single().GetTypedID();
      _grandChildTenantID = user.Tenant.Children.Single().Children.Single().GetTypedID();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void GetTenantHierarchyFromUser ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_childTenantID, _userID, null);

      Assert.That (principal.GetTenants (true).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _childTenantID, _grandChildTenantID }));
    }

    [Test]
    public void IncludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantID, _userID, null);

      Assert.That (principal.GetTenants (true).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _childTenantID, _grandChildTenantID }));
    }

    [Test]
    public void ExcludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantID, _userID, null);

      Assert.That (principal.GetTenants (false).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _grandChildTenantID }));
    }

    [Test]
    public void UsesSecurityFreeSectionToAccessTenantOfUser ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantID, _userID, null);

      Assert.That (principal.GetTenants (true), Is.Empty);
    }
  }
}