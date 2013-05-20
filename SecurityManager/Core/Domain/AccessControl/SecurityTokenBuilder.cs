// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.FunctionalProgramming;
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

        return SecurityToken.Create(principalUser, owningTenant, owningGroup, owningUser, abstractRoles);
      }
    }

    private Principal CreatePrincipal (ISecurityPrincipal principal)
    {
      if (principal.IsNull)
        return Principal.Null;

      if (string.IsNullOrEmpty (principal.User))
        throw CreateAccessControlException ("No principal was provided.");

      if (string.IsNullOrEmpty (principal.SubstitutedUser) && principal.SubstitutedRole != null)
        throw CreateAccessControlException ("A substituted role was specified without a substituted user.");
     
      User user = GetUser (principal.User);
      Assertion.IsNotNull (user);

      Tenant principalTenant = user.Tenant;
      User principalUser;
      IEnumerable<Role> principalRoles;

      if (principal.SubstitutedUser != null)
      {
        Substitution substitution = GetSubstitution (principal, user);

        if (substitution == null)
        {
          principalUser = null;
          principalRoles = new Role[0];
        }
        else if (principal.SubstitutedRole != null)
        {
          principalUser = null;
          principalRoles = EnumerableUtility.Singleton (substitution.SubstitutedRole);
        }
        else
        {
          principalUser = substitution.SubstitutedUser;
          principalRoles = substitution.SubstitutedUser.Roles;
        }
      }
      else 
      {
        principalUser = user;
        principalRoles = user.Roles;
        
        if (principal.Role != null)
          principalRoles = principalRoles.Where (r => IsRoleMatchingPrincipalRole (r, principal.Role));
      }

      return Principal.Create (principalTenant, principalUser, principalRoles);
    }

    private Substitution GetSubstitution (ISecurityPrincipal principal, User user)
    {
      IEnumerable<Substitution> substitutions = user.GetActiveSubstitutions ();
      
      substitutions = substitutions.Where (s => s.SubstitutedUser.UserName == principal.SubstitutedUser && s.SubstitutedUser.Tenant == user.Tenant);
      
      if (principal.SubstitutedRole != null)
        substitutions = substitutions.Where (s => IsRoleMatchingPrincipalRole (s.SubstitutedRole, principal.SubstitutedRole));
      else
        substitutions = substitutions.Where (s => s.SubstitutedRole == null);

      return substitutions.FirstOrDefault ();
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

    private IList<AbstractRoleDefinition> GetAbstractRoles (IEnumerable<EnumWrapper> abstractRoleNames)
    {
      var abstractRoleNamesCollection = abstractRoleNames.ConvertToCollection();
      IList<AbstractRoleDefinition> abstractRolesCollection = AbstractRoleDefinition.Find (abstractRoleNamesCollection);

      EnumWrapper? missingAbstractRoleName = FindFirstMissingAbstractRole (abstractRoleNamesCollection, abstractRolesCollection);
      if (missingAbstractRoleName != null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", missingAbstractRoleName);

      return abstractRolesCollection;
    }

    private EnumWrapper? FindFirstMissingAbstractRole (
        IEnumerable<EnumWrapper> expectedAbstractRoles, IList<AbstractRoleDefinition> actualAbstractRoleDefinitions)
    {
      var actualAbstractRoles = actualAbstractRoleDefinitions.Select (r => EnumWrapper.Get (r.Name));
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
