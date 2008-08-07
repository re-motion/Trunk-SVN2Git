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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private readonly User _user;
    private readonly Tenant _owningTenant;
    private readonly ReadOnlyCollection<Group> _owningGroups;
    private ReadOnlyCollection<Group> _userGroups;
    private ReadOnlyCollection<Role> _owningGroupRoles;
    private readonly ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (User user, Tenant owningTenant, IList<Group> owningGroups, IList<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("owningGroups", owningGroups);
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _user = user;
      _owningTenant = owningTenant;
      _owningGroups = new ReadOnlyCollection<Group> (owningGroups);
      _abstractRoles = new ReadOnlyCollection<AbstractRoleDefinition> (abstractRoles);
    }


    public User User
    {
      get { return _user; }
    }

    public ReadOnlyCollection<Group> UserGroups
    {
      get
      {
        if (_userGroups == null)
          _userGroups = GetGroups (_user);
        return _userGroups;
      }
    }

    public Tenant OwningTenant
    {
      get { return _owningTenant; }
    }

    public ReadOnlyCollection<Group> OwningGroups
    {
      get { return _owningGroups; }
    }

    public ReadOnlyCollection<Role> OwningGroupRoles
    {
      get
      {
        if (_owningGroupRoles == null)
          _owningGroupRoles = GetRoles (User, OwningGroups);
        return _owningGroupRoles;
      }
    }

    public ReadOnlyCollection<AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public bool MatchesUserTenant (Tenant tenant)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);

      if (User == null)
        return false;

      while (tenant != null)
      {
        if (User.Tenant.ID == tenant.ID)
          return true;
        tenant = tenant.Parent;
      }
      return false;
    }

    public bool ContainsRoleForOwningGroupAndPosition (Position position)
    {
      ArgumentUtility.CheckNotNull ("position", position);

      return ContainsRoleForPosition (OwningGroupRoles, position);
    }

    public bool ContainsRoleForUserGroupAndPosition (Position position)
    {
      ArgumentUtility.CheckNotNull ("position", position);

      if (User == null)
        return false;

      return ContainsRoleForPosition (User.Roles, position);
    }

    private ReadOnlyCollection<Group> GetGroups (User user)
    {
      List<Group> groups = new List<Group> ();

      if (user != null)
      {
        for (Group group = user.OwningGroup; group != null; group = group.Parent)
          groups.Add (group);
      }

      return groups.AsReadOnly ();
    }

    private ReadOnlyCollection<Role> GetRoles (User user, IList<Group> groups)
    {
      List<Role> roles = new List<Role> ();

      if (user != null)
      {
        foreach (Group group in groups)
          roles.AddRange (user.GetRolesForGroup (group));
      }

      return roles.AsReadOnly ();
    }

    private bool ContainsRoleForPosition (IList roles, Position position)
    {
      foreach (Role role in roles)
      {
        if (role.Position.ID == position.ID)
          return true;
      }

      return false;
    }
  }
}
