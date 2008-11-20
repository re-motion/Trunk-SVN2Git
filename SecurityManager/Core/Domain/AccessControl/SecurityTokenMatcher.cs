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
          return MatchPrincipalAgainstTenant (token.Principal, token.OwningTenant);

        case TenantCondition.SpecificTenant:
          return MatchPrincipalAgainstTenant (token.Principal, _ace.SpecificTenant);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'TenantCondition'.", _ace.TenantCondition);
      }
    }

    private bool MatchPrincipalAgainstTenant (User principal, Tenant referenceTenant)
    {
      if (principal == null)
        return false;

      if (referenceTenant == null)
        return false;

      switch (_ace.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw CreateInvalidOperationException ("The value 'Undefined' is not a valid value for matching the 'TenantHierarchyCondition'.");

        case TenantHierarchyCondition.This:
          return referenceTenant == principal.Tenant;

        case TenantHierarchyCondition.Parent:
          throw CreateInvalidOperationException ("The value 'Parent' is not a valid value for matching the 'TenantHierarchyCondition'.");

        case TenantHierarchyCondition.ThisAndParent:
          return GetThisAndParents (referenceTenant).Contains (principal.Tenant);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'TenantHierarchyCondition'.", _ace.TenantHierarchyCondition);
      }
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

        // BranchOfOwningGroup matches the ACE iff: The (security token) owning group or any of its parent-groups has the 
        // group type stored in the ACE - and - the principal is a member of any of the groups which come below the first
        // group in the group hierarchy including and above the owning group whose group type is equal to the ACE-group-type.
        // Algorithmically the matcher first walks upward the group hierarchy of the owning group (including the owning group itself),
        // returning the first group whose group type is equal to the group type stored in the ACE. 
        // Starting with this group, it then traverses the group hierarchy downward and checks if any of the groups is a member
        // of the principal's groups; if yes, the ACE matches, otherwise it does not.
        case GroupCondition.BranchOfOwningGroup:
          return MatchPrincipalAgainstGroup (token.Principal, FindBranchRoot (token.OwningGroup), GroupHierarchyCondition.ThisAndChildren);

        case GroupCondition.AnyGroupWithSpecificGroupType:
          return MatchPrincipalAgainstPosition (token.Principal);

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupCondition'.", _ace.GroupCondition);
      }
    }

    // Starting at the passed group, returns the first group in the parent hierarchy of the passed group whose
    // GroupType is equal to the ACE specific GroupType.
    private Group FindBranchRoot (Group referenceGroup)
    {
      Assertion.IsNotNull (_ace.GroupCondition == GroupCondition.BranchOfOwningGroup);

      return GetThisAndParents (referenceGroup).Where (g => g.GroupType == _ace.SpecificGroupType).FirstOrDefault ();
    }

    private bool MatchPrincipalAgainstGroup (User principal, Group referenceGroup, GroupHierarchyCondition groupHierarchyCondition)
    {
      if (principal == null)
        return false;

      if (referenceGroup == null)
        return false;

      var roles = GetMatchingPrincipalRoles (principal);
      var principalGroups = roles.Select (r => r.Group);

      Func<bool> isPrincipalMatchingReferenceGroupOrParents = () => principalGroups.Intersect (GetThisAndParents (referenceGroup)).Any();
      Func<bool> isPrincipalMatchingReferenceGroupOrChildren = () => principalGroups.SelectMany (g => GetThisAndParents (g)).Contains (referenceGroup);

      switch (groupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw CreateInvalidOperationException ("The value 'Undefined' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.This:
          return principalGroups.Contains (referenceGroup);

        case GroupHierarchyCondition.Parent:
          throw CreateInvalidOperationException ("The value 'Parent' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.Children:
          throw CreateInvalidOperationException ("The value 'Children' is not a valid value for matching the 'GroupHierarchyCondition'.");

        case GroupHierarchyCondition.ThisAndParent:
          return isPrincipalMatchingReferenceGroupOrParents();

        case GroupHierarchyCondition.ThisAndChildren:
          return isPrincipalMatchingReferenceGroupOrChildren();

        case GroupHierarchyCondition.ThisAndParentAndChildren:
          return isPrincipalMatchingReferenceGroupOrParents() || isPrincipalMatchingReferenceGroupOrChildren();

        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupHierarchyCondition'.", groupHierarchyCondition);
      }
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
    private IEnumerable<Group> GetThisAndParents (Group group)
    {
      for (var current = group; current != null; current = current.Parent)
        yield return current;
    }

    private IEnumerable<Tenant> GetThisAndParents (Tenant tenant)
    {
      for (var current = tenant; current != null; current = current.Parent)
        yield return current;
    }
  }
}