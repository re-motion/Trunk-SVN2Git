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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {
    private readonly IUserRoleAclAceCombinations _userRoleAclAceCombinations;

    public AclExpander (IUserRoleAclAceCombinations userRoleAclAceCombinations)
    {
      ArgumentUtility.CheckNotNull ("userRoleAclAceCombinations", userRoleAclAceCombinations);
      _userRoleAclAceCombinations = userRoleAclAceCombinations;
    }

    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      ArgumentUtility.CheckNotNull ("userFinder", userFinder);
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      _userRoleAclAceCombinations = new UserRoleAclAceCombinations (userFinder, accessControlListFinder);
    }

    /// <summary>
    /// Default behavior is to use all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s.
    /// </summary>
    public AclExpander () : this (new AclExpanderUserFinder (), new AclExpanderAclFinder ()) {}


    public List<AclExpansionEntry> GetAclExpansionEntryListSortedAndDistinct ()
    {
      return (from AclExpansionEntry aclExpansionEntry in GetAclExpansionEntryList ()
              orderby aclExpansionEntry.User.DisplayName, aclExpansionEntry.Role.DisplayName
              select aclExpansionEntry ).Distinct().ToList();
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    /// <returns></returns>
    public List<AclExpansionEntry> GetAclExpansionEntryList ()
    {
      var aclExpansionEntries = new List<AclExpansionEntry> ();

      foreach (UserRoleAclAceCombination userRoleAclAce in _userRoleAclAceCombinations)
      {
        AddAclExpansionEntry (aclExpansionEntries, userRoleAclAce);
      }

      return aclExpansionEntries;
    }


    private void AddAclExpansionEntry (List<AclExpansionEntry> aclExpansionEntries, UserRoleAclAceCombination userRoleAclAce)
    {
      AccessControlEntry ace = userRoleAclAce.Ace;
      AclProbe aclProbe = AclProbe.CreateAclProbe (userRoleAclAce.User, userRoleAclAce.Role, ace);


      var accessTypeStatistics = new AccessTypeStatistics();
      AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);

      //To.ConsoleLine.e (accessTypeStatistics.IsInAccessTypesSupplyingAces (ace) ? "Contributing ACE" : "Non-contributing ACE", ace);
      
      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types
      if (accessTypeStatistics.IsInAccessTypesSupplyingAces (ace) && accessInformation.AllowedAccessTypes.Length > 0)
      {
        var aclExpansionEntry = new AclExpansionEntry (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, aclProbe.AccessConditions,
          accessInformation.AllowedAccessTypes, accessInformation.DeniedAccessTypes);
        aclExpansionEntries.Add (aclExpansionEntry);
      }
    }


  }
}