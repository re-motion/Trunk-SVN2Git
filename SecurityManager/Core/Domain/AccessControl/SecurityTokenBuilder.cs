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
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class SecurityTokenBuilder : ISecurityTokenBuilder
  {
    public SecurityTokenBuilder ()
    {
    }

    /// <exception cref="AccessControlException">
    ///   A matching <see cref="User"/> is not found for the <paramref name="principal"/>.<br/>- or -<br/>
    ///   A matching <see cref="Group"/> is not found for the <paramref name="context"/>'s <see cref="ISecurityContext.OwnerGroup"/>.<br/>- or -<br/>
    ///   A matching <see cref="AbstractRoleDefinition"/> is not found for all entries in the <paramref name="context"/>'s <see cref="SecurityContext.AbstractRoles"/> collection.
    /// </exception>
    public SecurityToken CreateToken (ClientTransaction transaction, IPrincipal principal, ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("context", context);

      using (transaction.EnterNonDiscardingScope())
      {
        User user = GetUser (principal.Identity.Name);
        Tenant owningTenant = GetTenant (context.OwnerTenant);
        List<Group> owningGroups = GetGroups (context.OwnerGroup);
        List<AbstractRoleDefinition> abstractRoles = GetAbstractRoles (context.AbstractRoles);

        return new SecurityToken (user, owningTenant, owningGroups, abstractRoles);
      }
    }

    private Tenant GetTenant (string tenantUniqueIdentifier)
    {
      if (StringUtility.IsNullOrEmpty (tenantUniqueIdentifier))
        return null;

      Tenant tenant = Tenant.FindByUnqiueIdentifier (tenantUniqueIdentifier);
      if (tenant == null)
        throw CreateAccessControlException ("The tenant '{0}' could not be found.", tenantUniqueIdentifier);

      return tenant;
    }

    private User GetUser (string userName)
    {
      if (StringUtility.IsNullOrEmpty (userName))
        return null;

      User user = User.FindByUserName (userName);
      if (user == null)
        throw CreateAccessControlException ("The user '{0}' could not be found.", userName);

      return user;
    }

    private List<Group> GetGroups (string groupUniqueIdentifier)
    {
      List<Group> groups = new List<Group>();

      if (StringUtility.IsNullOrEmpty (groupUniqueIdentifier))
        return groups;

      Group group = Group.FindByUnqiueIdentifier (groupUniqueIdentifier);
      if (group == null)
        throw CreateAccessControlException ("The group '{0}' could not be found.", groupUniqueIdentifier);

      groups.Add (group);

      while (group.Parent != null)
      {
        group = group.Parent;
        groups.Add (group);
      }

      return groups;
    }

    private List<AbstractRoleDefinition> GetAbstractRoles (EnumWrapper[] abstractRoleNames)
    {
      DomainObjectCollection abstractRolesCollection = AbstractRoleDefinition.Find (abstractRoleNames);

      EnumWrapper? missingAbstractRoleName = FindFirstMissingAbstractRole (abstractRoleNames, abstractRolesCollection);
      if (missingAbstractRoleName != null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", missingAbstractRoleName);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition>();
      foreach (AbstractRoleDefinition abstractRole in abstractRolesCollection)
        abstractRoles.Add (abstractRole);

      return abstractRoles;
    }

    private EnumWrapper? FindFirstMissingAbstractRole (EnumWrapper[] expectedAbstractRoles, DomainObjectCollection actualAbstractRolesCollection)
    {
      if (expectedAbstractRoles.Length == actualAbstractRolesCollection.Count)
        return null;

      AbstractRoleDefinition[] actualAbstractRoles = new AbstractRoleDefinition[actualAbstractRolesCollection.Count];
      actualAbstractRolesCollection.CopyTo (actualAbstractRoles, 0);

      foreach (EnumWrapper expectedAbstractRole in expectedAbstractRoles)
      {
        Predicate<AbstractRoleDefinition> match =
            delegate (AbstractRoleDefinition current) { return current.Name.Equals (expectedAbstractRole.ToString(), StringComparison.Ordinal); };

        if (!Array.Exists (actualAbstractRoles, match))
          return expectedAbstractRole;
      }

      return null;
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
