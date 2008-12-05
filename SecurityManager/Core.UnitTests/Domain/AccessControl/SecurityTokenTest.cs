// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new OrganizationalStructureFactory ();

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Initialize_Values ()
    {
      Tenant principalTenant = CreateTenant ("principalTenant");
      User principal = CreateUser ("principal", CreateGroup ("principalGroup", null, principalTenant), principalTenant);
      Tenant owningTenant = CreateTenant ("owningTenant");
      Group owningGroup = CreateGroup ("owningGroup", null, owningTenant);
      User owningUser = CreateUser ("owningUser", CreateGroup ("owningUserGroup", null, owningTenant), owningTenant);
      AbstractRoleDefinition abstractRole1 = AbstractRoleDefinition.NewObject (Guid.NewGuid (), "role1", 0);
      AbstractRoleDefinition abstractRole2 = AbstractRoleDefinition.NewObject (Guid.NewGuid (), "role2", 1);
      SecurityToken token = new SecurityToken (principal, owningTenant, owningGroup, owningUser, new [] { abstractRole1,abstractRole2});

      Assert.That (token.Principal, Is.SameAs (principal));
      Assert.That (token.OwningTenant, Is.SameAs (owningTenant));
      Assert.That (token.OwningGroup, Is.SameAs (owningGroup));
      Assert.That (token.OwningUser, Is.SameAs (owningUser));
      Assert.That (token.AbstractRoles, Is.EquivalentTo (new[] { abstractRole1, abstractRole2 }));
    }

    [Test]
    public void Initialize_Empty ()
    {
      SecurityToken token = new SecurityToken (null, null, null, null, new List<AbstractRoleDefinition> ());

      Assert.IsNull (token.Principal);
      Assert.IsNull (token.OwningTenant);
      Assert.IsNull (token.OwningGroup);
      Assert.IsNull (token.OwningUser);
      Assert.IsEmpty (token.AbstractRoles);
    }

    private Tenant CreateTenant (string name)
    {
      Tenant tenant = _factory.CreateTenant ();
      tenant.Name = name;

      return tenant;
    }

    private Group CreateGroup (string name, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup ();
      group.Name = name;
      group.Parent = parent;
      group.Tenant = tenant;

      return group;
    }

    private User CreateUser (string userName, Group owningGroup, Tenant tenant)
    {
      User user = _factory.CreateUser ();
      user.UserName = userName;
      user.FirstName = "First Name";
      user.LastName = "Last Name";
      user.Title = "Title";
      user.Tenant = tenant;
      user.OwningGroup = owningGroup;

      return user;
    }
  }
}
