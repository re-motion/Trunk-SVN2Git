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
using System.Linq;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class SecurityTokenMatcher
  {
    private readonly AccessControlEntry _ace;

    public SecurityTokenMatcher (AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);

      _ace = ace;
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      if (!MatchesTenantCondition (token))
        return false;

      if (!MatchesAbstractRole (token))
        return false;

      if (!MatchesUserCondition (token))
        return false;

      if (!MatchesGroupCondition (token))
        return false;

      return true;
    }

    private bool MatchesTenantCondition (SecurityToken token)
    {
      Assertion.IsNotNull (token);
      switch (_ace.TenantCondition)
      {
        case TenantCondition.None:
          return true;

        case TenantCondition.OwningTenant:
          return MatchesUserTenant (token.Principal, token.OwningTenant);

        case TenantCondition.SpecificTenant:
          return MatchesUserTenant (token.Principal, _ace.SpecificTenant);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid enum value for 'TenantCondition'", _ace.TenantCondition);
      }
    }

    private bool MatchesUserTenant (User principal, Tenant referenceTenant)
    {
      if (principal == null)
        return false;

      if (referenceTenant == null)
        return false;

      return GetParents (referenceTenant).Contains (principal.Tenant);
    }

    private bool MatchesAbstractRole (SecurityToken token)
    {
      Assertion.IsNotNull (token);
      if (_ace.SpecificAbstractRole == null)
        return true;

      foreach (var abstractRole in token.AbstractRoles)
      {
        if (abstractRole.ID == _ace.SpecificAbstractRole.ID)
          return true;
      }

      return false;
    }

    private bool MatchesUserCondition (SecurityToken token)
    {
      switch (_ace.UserCondition)
      {
        case UserCondition.None:
          return true;

        case UserCondition.Owner:
          return MatchPrincipalAgainstUser (token.Principal, token.OwningUser);

        case UserCondition.SpecificUser:
          return MatchPrincipalAgainstUser (token.Principal, _ace.SpecificUser);

        case UserCondition.SpecificPosition:
          return MatchPrincipalAgainstPosition (token.Principal);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'UserCondition'.", _ace.UserCondition);
      }
    }

    private bool MatchPrincipalAgainstUser (User principal, User referenceUser)
    {
      if (principal == null)
        return false;

      if (referenceUser == null)
        return false;

      return principal == referenceUser;
    }

    private bool MatchPrincipalAgainstPosition (User principal)
    {
      if (principal == null)
        return false;

      return GetMatchingPrincipalRoles (principal).Any();
    }

    private bool MatchesGroupCondition (SecurityToken token)
    {
      Assertion.IsNotNull (token);
      switch (_ace.GroupCondition)
      {
        case GroupCondition.None:
          return true;

        case GroupCondition.OwningGroup:
          return MatchPrincipalAgainstGroup (token.Principal, token.OwningGroup, _ace.GroupHierarchyCondition);

        case GroupCondition.SpecificGroup:
          return MatchPrincipalAgainstGroup (token.Principal, _ace.SpecificGroup, _ace.GroupHierarchyCondition);

        case GroupCondition.BranchOfOwningGroup:
          return MatchPrincipalAgainstGroup (token.Principal, FindBranchRoot (token.OwningGroup), GroupHierarchyCondition.ThisAndChildren);

        case GroupCondition.AnyGroupWithSpecificGroupType:
          return MatchPrincipalAgainstPosition (token.Principal);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupCondition'.", _ace.GroupCondition);
      }
    }

    private Group FindBranchRoot (Group referenceGroup)
    {
      Assertion.IsNotNull (_ace.GroupCondition == GroupCondition.BranchOfOwningGroup);

      return GetParents (referenceGroup).Where (g => g.GroupType == _ace.SpecificGroupType).FirstOrDefault();
    }

    private bool MatchPrincipalAgainstGroup (User principal, Group referenceGroup, GroupHierarchyCondition groupHierarchyCondition)
    {
      if (principal == null)
        return false;

      if (referenceGroup == null)
        return false;

      IEnumerable<Role> roles = GetMatchingPrincipalRoles (principal);

      var userGroups = roles.Select (r => r.Group);
      var objectGroups = (IEnumerable<Group>) new[] { referenceGroup };

      switch (groupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          objectGroups = new Group[0];
          userGroups = new Group[0];
          break;

        case GroupHierarchyCondition.This:
          break;

        case GroupHierarchyCondition.ThisAndParent:
          objectGroups = objectGroups.SelectMany (g => GetParents (g));
          break;

        case GroupHierarchyCondition.ThisAndChildren:
          userGroups = userGroups.SelectMany (g => GetParents (g));
          break;

        case GroupHierarchyCondition.ThisAndParentAndChildren:
          objectGroups = objectGroups.SelectMany (g => GetParents (g));
          userGroups = userGroups.SelectMany (g => GetParents (g));
          break;

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupHierarchyCondition'.", _ace.GroupHierarchyCondition);
      }

      return userGroups.Intersect (objectGroups).Any();
    }

    private IEnumerable<Role> GetMatchingPrincipalRoles (User principal)
    {
      Assertion.IsNotNull (principal);

      var roles = (IEnumerable<Role>) principal.Roles;
      if (_ace.UserCondition == UserCondition.SpecificPosition)
        roles = roles.Where (r => r.Position == _ace.SpecificPosition);


      bool hasSpecificPositionAndGroupType =
          _ace.UserCondition == UserCondition.SpecificPosition && _ace.GroupCondition == GroupCondition.BranchOfOwningGroup
          || _ace.GroupCondition == GroupCondition.AnyGroupWithSpecificGroupType;

      if (hasSpecificPositionAndGroupType)
        roles = roles.Where (r => r.Group.GroupType == _ace.SpecificGroupType);

      return roles;
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    //TODO MK: Move to Linq-Extensions
    private IEnumerable<Group> GetParents (Group group)
    {
      for (var current = group; current != null; current = current.Parent)
        yield return current;
    }

    private IEnumerable<Tenant> GetParents (Tenant tenant)
    {
      for (var current = tenant; current != null; current = current.Parent)
        yield return current;
    }
  }
}