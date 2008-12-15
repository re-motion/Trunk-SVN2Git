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
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Teh <see cref="SecurityTokenBuilder"/> is responsible for creating a <see cref="SecurityToken"/> from an <see cref="ISecurityContext"/> and an
  /// <see cref="IPrincipal"/>.
  /// </summary>
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
    public SecurityToken CreateToken (ClientTransaction transaction, ISecurityPrincipal principal, ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("context", context);

      using (transaction.EnterNonDiscardingScope())
      {
        Principal principalUser = CreatePrincipal (principal);
        Tenant owningTenant = GetTenant (context.OwnerTenant);
        Group owningGroup = GetGroup (context.OwnerGroup);
        User owningUser = GetUser (context.Owner);
        IList<AbstractRoleDefinition> abstractRoles = GetAbstractRoles (context.AbstractRoles);

        return new SecurityToken (principalUser, owningTenant, owningGroup, owningUser, abstractRoles);
      }
    }

    private Principal CreatePrincipal (ISecurityPrincipal principal)
    {
      User user = GetUser (principal.User);
      if (user == null)
        throw CreateAccessControlException ("No principal was provided.");

      Tenant principalTenant = user.Tenant;

      IEnumerable<Role> principalRoles;
      if (principal.SubstitutedRole != null)
      {
        principalRoles = user.GetActiveSubstitutions()
            .Where (s => IsRoleMatchingPrincipalRole (s.SubstitutedRole, principal.SubstitutedRole))
            .Select (s => s.SubstitutedRole);
      }
      else if (principal.Role != null)
      {
        principalRoles = user.Roles.Where (r => IsRoleMatchingPrincipalRole (r, principal.Role));
      }
      else
      {
        principalRoles = user.Roles;
      }

      User principalUser;
      if (principal.SubstitutedRole != null)
        principalUser = null;
      else
        principalUser = user;

      return new Principal (principalTenant, principalUser, principalRoles.ToArray());
    }

    private bool IsRoleMatchingPrincipalRole (Role role, ISecurityPrincipalRole principalRole)
    {
      if (role == null)
        return false;

      return role.Group.UniqueIdentifier == principalRole.Group && role.Position.UniqueIdentifier == principalRole.Position;
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

    private Group GetGroup (string groupUniqueIdentifier)
    {
      if (StringUtility.IsNullOrEmpty (groupUniqueIdentifier))
        return null;

      Group group = Group.FindByUnqiueIdentifier (groupUniqueIdentifier);
      if (group == null)
        throw CreateAccessControlException ("The group '{0}' could not be found.", groupUniqueIdentifier);

      return group;
    }

    private IList<AbstractRoleDefinition> GetAbstractRoles (EnumWrapper[] abstractRoleNames)
    {
      IList<AbstractRoleDefinition> abstractRolesCollection = AbstractRoleDefinition.Find (abstractRoleNames);

      EnumWrapper? missingAbstractRoleName = FindFirstMissingAbstractRole (abstractRoleNames, abstractRolesCollection);
      if (missingAbstractRoleName != null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", missingAbstractRoleName);

      return abstractRolesCollection;
    }

    private EnumWrapper? FindFirstMissingAbstractRole (
        EnumWrapper[] expectedAbstractRoles, IList<AbstractRoleDefinition> actualAbstractRolesCollection)
    {
      var actualAbstractRoles = from r in actualAbstractRolesCollection select new EnumWrapper (r.Name);
      var result = from expected in expectedAbstractRoles
                   where !actualAbstractRoles.Contains (expected)
                   select (EnumWrapper?) expected;

      return result.FirstOrDefault();
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}