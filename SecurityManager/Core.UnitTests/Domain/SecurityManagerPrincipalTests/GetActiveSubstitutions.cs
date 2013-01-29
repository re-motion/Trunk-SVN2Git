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
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetActiveSubstitutions : DomainTest
  {
    private IDomainObjectHandle<Tenant> _tenantID;
    private IDomainObjectHandle<User> _userID;
    private IDomainObjectHandle<Substitution>[] _substitutionHandles;

    public override void SetUp ()
    {
      base.SetUp ();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      User user = User.FindByUserName ("substituting.user");
      _userID = user.GetHandle();
      _tenantID = user.Tenant.GetHandle();
      _substitutionHandles = user.GetActiveSubstitutions().Select (s => s.GetHandle()).ToArray();
      Assert.That (_substitutionHandles.Length, Is.EqualTo (2));
    }

    public override void TearDown ()
    {
      base.TearDown ();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void ExcludeInactiveSubstitutions ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_tenantID, _userID, null);

      Assert.That (principal.GetActiveSubstitutions().Select (t => t.ID), Is.EquivalentTo (_substitutionHandles.Select (h => h.ObjectID)));
    }
  }
}