// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class PrincipalTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      _testHelper = testHelper;
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Initialize_WithUserAndRoles ()
    {
      User user = _testHelper.CreateUser ("userName", null, "lastName", null, null, null);
      Role[] roles = new[] { _testHelper.CreateRole (null, null, null), _testHelper.CreateRole (null, null, null) };
      Principal principal = new Principal (user, roles);

      Assert.That (principal.User, Is.SameAs (user));
      Assert.That (principal.Roles, Is.Not.SameAs (roles));
      Assert.That (principal.Roles, Is.EquivalentTo (roles));
    }

    [Test]
    public void Initialize_WithUserAndWithoutRoles ()
    {
      User user = _testHelper.CreateUser ("userName", null, "lastName", null, null, null);
      Principal principal = new Principal (user, new Role[0]);

      Assert.That (principal.User, Is.SameAs (user));
      Assert.That (principal.Roles, Is.Empty);
    }

    [Test]
    public void Initialize_WithoutUserAndWithRoles ()
    {
      Role[] roles = new[] { _testHelper.CreateRole (null, null, null), _testHelper.CreateRole (null, null, null) };
      Principal principal = new Principal (null, roles);

      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Roles, Is.Not.SameAs (roles));
      Assert.That (principal.Roles, Is.EquivalentTo (roles));
    }

    [Test]
    public void Initialize_WithoutUserAndWithoutRoles ()
    {
      Principal principal = new Principal (null, new Role[0]);

      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Roles, Is.Empty);
    }
  }
}