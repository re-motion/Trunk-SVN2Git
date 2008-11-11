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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class SecurityTokenMatcher
  {
    private readonly AccessControlEntry _accessControlEntry;

    public SecurityTokenMatcher (AccessControlEntry accessControlEntry)
    {
      ArgumentUtility.CheckNotNull ("accessControlEntry", accessControlEntry);

      _accessControlEntry = accessControlEntry;
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

       //To.ConsoleLine.s ("AccessControList.FindMatchingEntries: ").e ("ACE", entry);
      //To.ConsoleLine.sb().s ("AccessControEntry.MatchesToken: ").e (ToString ()).e ("ACE", this).se();

      if (!MatchesTenant (token))
        return false;

      //To.ConsoleLine.s ("AccessControEntry.MatchesToken: matched Tenant");

      if (!MatchesAbstractRole (token))
        return false;

      //To.ConsoleLine.s ("AccessControEntry.MatchesToken: matched Role");

      if (!MatchesUserOrPosition (token))
        return false;

      //To.ConsoleLine.s ("AccessControEntry.MatchesToken: matched UserOrPosition");

      return true;
    }

    private bool MatchesTenant (SecurityToken token)
    {
      switch (_accessControlEntry.TenantSelection)
      {
        case TenantSelection.All:
          return true;

        case TenantSelection.OwningTenant:
          return token.OwningTenant != null && token.MatchesUserTenant (token.OwningTenant);

        case TenantSelection.SpecificTenant:
          return token.MatchesUserTenant (_accessControlEntry.SpecificTenant);

        default:
          return false;
      }
    }

    private bool MatchesAbstractRole (SecurityToken token)
    {
      if (_accessControlEntry.SpecificAbstractRole == null)
        return true;

      foreach (var abstractRole in token.AbstractRoles)
      {
        if (abstractRole.ID == _accessControlEntry.SpecificAbstractRole.ID)
          return true;
      }

      return false;
    }

    private bool MatchesUserOrPosition (SecurityToken token)
    {
      switch (_accessControlEntry.UserSelection)
      {
        case UserSelection.All:
          return true;

        case UserSelection.SpecificPosition:
          return MatchPosition (token);

        default:
          return false;
      }
    }

    private bool MatchPosition (SecurityToken token)
    {
      switch (_accessControlEntry.GroupSelection)
      {
        case GroupSelection.All:
          return token.ContainsRoleForUserGroupAndPosition (_accessControlEntry.SpecificPosition);

        case GroupSelection.OwningGroup:
          return token.ContainsRoleForOwningGroupAndPosition (_accessControlEntry.SpecificPosition);

        default:
          return false;
      }
    }
  }
}