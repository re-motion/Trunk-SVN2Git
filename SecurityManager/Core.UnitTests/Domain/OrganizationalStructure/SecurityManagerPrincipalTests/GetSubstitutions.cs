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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetSubstitutions : DomainTest
  {
    private ObjectID _rootTenantID;
    private ObjectID _childTenantID;
    private ObjectID _grandChildTenantID;
    private ObjectID _userID;


    public override void SetUp ()
    {
      base.SetUp ();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      User user = User.FindByUserName ("substituting.user");
      _userID = user.ID;
      _rootTenantID = user.Tenant.ID;
      _childTenantID = user.Tenant.Children.Single ().ID;
      _grandChildTenantID = user.Tenant.Children.Single ().Children.Single ().ID;
    }

    public override void TearDown ()
    {
      base.TearDown ();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    [Ignore]
    public void IncludeInactiveSubstitutions ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_childTenantID, _userID, null);

      Assert.That (principal.GetTenants (true).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantID, _childTenantID, _grandChildTenantID }));
    }
  }
}