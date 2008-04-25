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
    private User _user;
    private Tenant _owningTenant;
    private ReadOnlyCollection<Group> _owningGroups;
    private ReadOnlyCollection<Group> _userGroups;
    private ReadOnlyCollection<Role> _owningGroupRoles;
    private ReadOnlyCollection<AbstractRoleDefinition> _abstractRoles;

    public SecurityToken (User user, Tenant owningTenant, List<Group> owningGroups, List<AbstractRoleDefinition> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("owningGroups", owningGroups);
      ArgumentUtility.CheckNotNullOrItemsNull ("abstractRoles", abstractRoles);

      _user = user;
      _owningTenant = owningTenant;
      _owningGroups = owningGroups.AsReadOnly ();
      _abstractRoles = abstractRoles.AsReadOnly ();
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
