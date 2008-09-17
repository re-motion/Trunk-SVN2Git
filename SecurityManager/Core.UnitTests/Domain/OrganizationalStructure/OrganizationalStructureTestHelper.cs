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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganizationalStructureTestHelper
  {
    private readonly ClientTransaction _transaction;
    private readonly OrganizationalStructureFactory _factory;

    public OrganizationalStructureTestHelper ()
    {
      _transaction = ClientTransaction.CreateRootTransaction();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Tenant CreateTenant (string name, string uniqueIdentifier)
    {
      return CreateTenant (_transaction, name, uniqueIdentifier);
    }

    public Tenant CreateTenant (ClientTransaction transaction, string name, string uniqueIdentifier)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = _factory.CreateTenant ();
        tenant.UniqueIdentifier = uniqueIdentifier;
        tenant.Name = name;

        return tenant;
      }
    }

    public Group CreateGroup (string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      return CreateGroup (_transaction, name, uniqueIdentifier, parent, tenant);
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      using (transaction.EnterNonDiscardingScope())
      {
        Group group = _factory.CreateGroup ();
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;
        group.UniqueIdentifier = uniqueIdentifier;

        return group;
      }
    }

    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Tenant tenant)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        User user = _factory.CreateUser ();
        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Title = title;
        user.Tenant = tenant;
        user.OwningGroup = owningGroup;

        return user;
      }
    }

    public Position CreatePosition (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Position position = _factory.CreatePosition ();
        position.Name = name;

        return position;
      }
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        Role role = Role.NewObject();
        role.User = user;
        role.Group = group;
        role.Position = position;

        return role;
      }
    }

    public GroupType CreateGroupType (string name)
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = GroupType.NewObject();
        groupType.Name = name;

        return groupType;
      }
    }

    public GroupTypePosition CreateGroupTypePosition (GroupType groupType, Position position)
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        GroupTypePosition concretePosition = GroupTypePosition.NewObject();
        concretePosition.GroupType = groupType;
        concretePosition.Position = position;

        return concretePosition;
      }
    }
  }
}
