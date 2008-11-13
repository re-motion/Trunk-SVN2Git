/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
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
    public void Initialize_Empty ()
    {
      SecurityToken token = new SecurityToken (null, null, null, CreateAbstractRoles ());

      Assert.IsNull (token.OwningTenant);
      Assert.IsNull (token.User);
      Assert.IsNull (token.OwningGroup);
      Assert.IsEmpty (token.AbstractRoles);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningTenant ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      User user = null;

      SecurityToken token = new SecurityToken (user, tenant, null, CreateAbstractRoles ());

      Assert.AreSame (tenant, token.OwningTenant);
    }

    [Test]
    public void GetOwningGroups_Empty ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      User user = null;

      SecurityToken token = new SecurityToken (user, null, null, CreateAbstractRoles ());

      Assert.IsNull(token.OwningGroup);
    }

    [Test]
    public void GetOwningGroups ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      Group group = CreateGroup ("Testgroup", null, tenant);
      User user = null;

      SecurityToken token = new SecurityToken (user, null, group, CreateAbstractRoles ());

      Assert.AreSame (group, token.OwningGroup);
    }

    [Test]
    public void GetOwningGroupRoles_Empty ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      Group group = CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser ("test.user", group, tenant);

      SecurityToken token = new SecurityToken (user, null, group, CreateAbstractRoles ());

      Assert.AreEqual (0, token.OwningGroupRoles.Count);
    }

    [Test]
    public void GetOwningGroupRoles_WithoutUser ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      Group group1 = CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser ("test.user", group1, tenant);
      Position officialPosition = CreatePosition ("Official");
      Role officialInGroup1 = CreateRole (user, group1, officialPosition);

      SecurityToken token = new SecurityToken (null, null, group1, CreateAbstractRoles ());

      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningGroupRoles_WithRoles ()
    {
      Tenant tenant = CreateTenant ("Testtenant");
      Group group1 = CreateGroup ("Testgroup", null, tenant);
      Group group2 = CreateGroup ("Other group", null, tenant);
      User user = CreateUser ("test.user", group1, tenant);
      Position officialPosition = CreatePosition ("Official");
      Position managerPosition = CreatePosition ("Manager");
      Role officialInGroup1 = CreateRole (user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, group1, CreateAbstractRoles ());

      Assert.AreEqual (2, token.OwningGroupRoles.Count);
      Assert.Contains (officialInGroup1, token.OwningGroupRoles);
      Assert.Contains (managerInGroup1, token.OwningGroupRoles);
    }

    [Test]
    public void MatchesUserTenant_MatchesUserInTenant ()
    {
      Tenant tenant = CreateTenant ("TestTenant");
      Group group = CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser ("test.user", group, tenant);

      SecurityToken token = new SecurityToken (user, null, null, CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserTenant (tenant));
    }

    [Test]
    public void MatchesUserTenant_MatchesUserInParentTenant ()
    {
      Tenant parentTenant = CreateTenant ("ParentTenant");
      Tenant tenant = CreateTenant ("TestTenant");
      tenant.Parent = parentTenant;
      Group group = CreateGroup ("Testgroup", null, parentTenant);
      User user = CreateUser ("test.user", group, parentTenant);

      SecurityToken token = new SecurityToken (user, null, null, CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserTenant (tenant));
    }

    [Test]
    public void MatchesUserTenant_DoesNotMatchWithoutUser ()
    {
      Tenant tenant = CreateTenant ("Testtenant");

      SecurityToken token = new SecurityToken (null, null, null, CreateAbstractRoles ());

      Assert.IsFalse (token.MatchesUserTenant (tenant));
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

    private Position CreatePosition (string name)
    {
      Position position = _factory.CreatePosition ();
      position.Name = name;

      return position;
    }

    private Role CreateRole (User user, Group group, Position position)
    {
      Role role = Role.NewObject();
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    private List<AbstractRoleDefinition> CreateAbstractRoles ()
    {
      return new List<AbstractRoleDefinition> ();
    }
  }
}
