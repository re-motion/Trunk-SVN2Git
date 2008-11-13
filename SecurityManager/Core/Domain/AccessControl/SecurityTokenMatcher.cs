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
          return token.OwningTenant != null && token.MatchesUserTenant (token.OwningTenant);

        case TenantCondition.SpecificTenant:
          return token.MatchesUserTenant (_ace.SpecificTenant);

        default:
          return false;
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

        case UserCondition.SpecificPosition:
          return MatchPosition (token);

        default:
          return false;
      }
    }

    private bool MatchPosition (SecurityToken token)
    {
      if (token.User == null)
        return false;

      return GetMatchingUserRoles (token.User).Any();
    }

    private bool MatchesGroupCondition (SecurityToken token)
    {
      Assertion.IsNotNull (token);
      switch (_ace.GroupCondition)
      {
        case GroupCondition.None:
          return true;

        case GroupCondition.OwningGroup:
          return MatchUserAgainstGroup (token.User, token.OwningGroup, _ace.GroupHierarchyCondition);

        case GroupCondition.SpecificGroup:
          return MatchUserAgainstGroup (token.User, _ace.SpecificGroup, _ace.GroupHierarchyCondition);

        case GroupCondition.BranchOfOwningGroup:
          return MatchUserAgainstGroup (token.User, FindBranchRoot (token.OwningGroup), GroupHierarchyCondition.ThisAndChildren);

        case GroupCondition.AnyGroupWithSpecificGroupType:
          return MatchPosition (token);

        default:
          return false;
      }
    }

    private Group FindBranchRoot (Group groupOfTheObject)
    {
      Assertion.IsNotNull (_ace.GroupCondition == GroupCondition.BranchOfOwningGroup);

      return GetParents (groupOfTheObject).Where (g => g.GroupType == _ace.SpecificGroupType).FirstOrDefault();
    }

    private bool MatchUserAgainstGroup (User user, Group groupOfTheObject, GroupHierarchyCondition groupHierarchyCondition)
    {
      if (user == null)
        return false;

      if (groupOfTheObject == null)
        return false;

      IEnumerable<Role> userRoles = GetMatchingUserRoles (user);

      var userGroups = userRoles.Select (r => r.Group);
      var objectGroups = (IEnumerable<Group>) new[] { groupOfTheObject };

      switch (groupHierarchyCondition)
      {
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
          objectGroups = new Group[0];
          userGroups = new Group[0];
          break;
      }

      return userGroups.Intersect (objectGroups).Any();
    }

    private IEnumerable<Role> GetMatchingUserRoles (User user)
    {
      Assertion.IsNotNull (user);

      var userRoles = (IEnumerable<Role>) user.Roles;
      if (_ace.UserCondition == UserCondition.SpecificPosition)
        userRoles = userRoles.Where (r => r.Position == _ace.SpecificPosition);


      bool hasSpecificPositionAndGroupType =
          _ace.UserCondition == UserCondition.SpecificPosition && _ace.GroupCondition == GroupCondition.BranchOfOwningGroup
          || _ace.GroupCondition == GroupCondition.AnyGroupWithSpecificGroupType;

      if (hasSpecificPositionAndGroupType)
        userRoles = userRoles.Where (r => r.Group.GroupType == _ace.SpecificGroupType);

      return userRoles;
    }

    //TODO MK: Move to Linq-Extensions
    private IEnumerable<Group> GetParents (Group group)
    {
      for (Group currentGroup = group; currentGroup != null; currentGroup = currentGroup.Parent)
        yield return currentGroup;
    }
  }
}