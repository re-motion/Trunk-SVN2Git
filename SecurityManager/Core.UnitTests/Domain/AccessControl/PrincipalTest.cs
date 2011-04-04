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
using NUnit.Framework;
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
    public void Initialize_WithRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant ("tenant");
      User user = _testHelper.CreateUser ("userName", null, "lastName", null, null, null);
      Role[] roles = new[] { _testHelper.CreateRole (null, null, null), _testHelper.CreateRole (null, null, null) };
      Principal principal = new Principal (tenant, user, roles);

      Assert.That (principal.Tenant, Is.SameAs (tenant));
      Assert.That (principal.User, Is.SameAs (user));
      Assert.That (principal.Roles, Is.Not.SameAs (roles));
      Assert.That (principal.Roles, Is.EquivalentTo (roles));
      Assert.That (principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithoutRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant ("tenant");
      User user = _testHelper.CreateUser ("userName", null, "lastName", null, null, null);
      Principal principal = new Principal (tenant, user, new Role[0]);

      Assert.That (principal.Tenant, Is.SameAs (tenant));
      Assert.That (principal.User, Is.SameAs (user));
      Assert.That (principal.Roles, Is.Empty);
      Assert.That (principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithTenantAndWithoutUserAndWithRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant ("tenant");
      Role[] roles = new[] { _testHelper.CreateRole (null, null, null), _testHelper.CreateRole (null, null, null) };
      Principal principal = new Principal (tenant, null, roles);

      Assert.That (principal.Tenant, Is.SameAs (tenant));
      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Roles, Is.Not.SameAs (roles));
      Assert.That (principal.Roles, Is.EquivalentTo (roles));
      Assert.That (principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithTenantAndWithoutUserAndWithoutRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant ("tenant");
      Principal principal = new Principal (tenant, null, new Role[0]);

      Assert.That (principal.Tenant, Is.SameAs (tenant));
      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Roles, Is.Empty);
      Assert.That (principal.IsNull, Is.False);
    }

    [Test]
    public void GetNullPrincipal ()
    {
      Principal principal = Principal.Null;

      Assert.That (principal.Tenant, Is.Null);
      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Roles, Is.Empty);
      Assert.That (principal.IsNull, Is.True);
    }
  }
}
