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
using System.Diagnostics;
using System.Linq;
using Remotion.Data.DomainObjects;
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
      //To.ConsoleLine.s ("~~~~~ AddAclExpansionEntry ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      AclProbe aclProbe;
      AccessTypeStatistics accessTypeStatistics;
      AccessInformation accessInformation = GetAccessTypes(userRoleAclAce, out aclProbe, out accessTypeStatistics);


      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types.
      if (accessTypeStatistics.IsInAccessTypesContributingAces (userRoleAclAce.Ace) && accessInformation.AllowedAccessTypes.Length > 0)
      {
        var aclExpansionEntry = new AclExpansionEntry (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, aclProbe.AccessConditions,
          accessInformation.AllowedAccessTypes, accessInformation.DeniedAccessTypes);
        aclExpansionEntries.Add (aclExpansionEntry);
      }
    }



    public AccessInformation GetAccessTypes (UserRoleAclAceCombination userRoleAclAce, 
      out AclProbe aclProbe, out AccessTypeStatistics accessTypeStatistics)
    {
      const bool probeForCurrentRoleOnly = true;

      aclProbe = AclProbe.CreateAclProbe (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Ace);

      // Note: The aclProbe created above will NOT always match the ACE it was designed to probe; the reason for this
      // is that its SecurityToken is only designed to match the non-decideable access conditions
      // (abstract role and owning tenant, group, etc) of the ACE
      // (the "non-decideable" refers to the information context of the AclExpander, which is lacking some information).
      // For decideable access conditons (e.g. specific tenant or specific group), the created SecurityToken
      // is not guaranteed to match. The AccessTypeStatistics returned by Acl.GetAccessTypes are used to filter out these cases.
      //
      // Note also that it does not suffice to get the access types for the current ACE only, since these rights might be denied
      // by another matching ACE in the current ACL. 
      accessTypeStatistics = new AccessTypeStatistics ();

      // Create a discarding sub-transaction so we can change the roles of the current user below without side effects.
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        // Set roles of user to contain only the role we currently probe for.
        // If we don't do that another role of the user can match the ACE.SpecificPosition
        // for case GroupSelection.All or GroupSelection.OwningGroup, giving access rights
        // which the user does not have due to the currently tested role.
        // (Note that the user is in fact always in all roles at the same time, so he will
        // have the access rights returned without reducing the user's roles to
        // the one probed for; it's just not the information we want to present in the 
        // ACL-expansion, where we want to distinguish which role gives rise
        // to which access rights).

        if (probeForCurrentRoleOnly)
        {
          // Exchanging the User.Roles-collection with a new one containing only the current Role would not work (MK),
          // so we empty the collection, then add back the current Role.
          aclProbe.SecurityToken.User.Roles.Clear();
          aclProbe.SecurityToken.User.Roles.Add (userRoleAclAce.Role);
        }
        AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);
        
        // Non-contributing-ACE debugging
        //NonContributingAceDebugging (ace, aclProbe, accessTypeStatistics, userRoleAclAce.Acl);

       return accessInformation;
      }
    }

  }
}