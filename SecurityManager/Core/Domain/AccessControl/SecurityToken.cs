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
using System.Collections.ObjectModel;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public sealed class SecurityToken
  {
    private readonly User _user; // principial, NOT owning user (owning user will be introduced in future)
    private readonly Tenant _owningTenant;
    private readonly Group _owningGroup;
    private readonly ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    private ReadOnlyCollection<Role> _cachedUserRolesForOwningGroups;

    public SecurityToken (User user, Tenant owningTenant, Group owningGroup, IList<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _user = user;
      _owningTenant = owningTenant;
      _owningGroup = owningGroup;
      _abstractRoles = new ReadOnlyCollection<AbstractRoleDefinition> (abstractRoles);
    }


    public User User
    {
      get { return _user; }
    }

    public Tenant OwningTenant
    {
      get { return _owningTenant; }
    }

    public Group OwningGroup
    {
      get { return _owningGroup; }
    }

    public ReadOnlyCollection<Role> OwningGroupRoles
    {
      get
      {
        if (_cachedUserRolesForOwningGroups == null)
          _cachedUserRolesForOwningGroups = GetRoles (User, OwningGroup);
        return _cachedUserRolesForOwningGroups;
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

    private ReadOnlyCollection<Role> GetRoles (User user, Group group)
    {
      List<Role> roles = new List<Role>();

      if (user != null)
      {
        for (Group currentGroup = group; currentGroup != null; currentGroup = currentGroup.Parent)
          roles.AddRange (user.GetRolesForGroup (currentGroup));
      }

      return roles.AsReadOnly();
    }
  }
}