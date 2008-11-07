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


      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types
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
      aclProbe = AclProbe.CreateAclProbe (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Ace);

      // Note: It does not suffice to get the access types for the current ACE only, since these rights might be denied
      // by another matching ACE in the current ACL. Instead to be able to filter non-contributing ACEs below,
      // the contributing ACEs get recorded in the AccessTypeStatistics instance passed to Acl.GetAccessTypes.
      accessTypeStatistics = new AccessTypeStatistics ();
      AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);

      //Assertion.IsTrue (accessTypeStatistics.IsInMatchingAces (ace));

      // Non-contributing-ACE debugging
      //NonContributingAceDebugging (ace, aclProbe, accessTypeStatistics, userRoleAclAce.Acl);

      return accessInformation;
    }


    private void NonContributingAceDebugging (AccessControlEntry ace, AclProbe aclProbe,
      AccessTypeStatistics accessTypeStatistics, AccessControlList acl)
    {
      if (false)
      {
        //To.ConsoleLine.sb().e(accessTypeStatistics.IsInAccessTypesSupplyingAces (ace) ? "Contributing ACE" : ">>> Non-contributing ACE", ace).e().se();
        To.ConsoleLine.e ("========================================================================================================================");
        To.ConsoleLine.sb().s (ace.ToString()).e (() => aclProbe);
        To.ConsoleLine.e (accessTypeStatistics.IsInAccessTypesContributingAces (ace) ? "Contributing ACE" : ">>> Non-contributing ACE", ace);
        To.ConsoleLine.e (accessTypeStatistics.AccessTypesSupplyingAces).se();
      }
      else if (false)
      {
        if (!accessTypeStatistics.IsInAccessTypesContributingAces (ace))
        {
          To.ConsoleLine.e (">>> Non-contributing ACE", ace);
        }
      }
      else if (false)
      {
        bool aceContributed = accessTypeStatistics.IsInAccessTypesContributingAces (ace);
        if (!aceContributed && ace.SpecificAbstractRole != null)
        {
          To.ConsoleLine.e (
              "------------------------------------------------------------------------------------------------------------------------");
          To.ConsoleLine.s (ace.ToString()).nl().e (() => aclProbe);
          To.ConsoleLine.e (">>> Non-contributing ACE with specific abstract role", ace);
          To.ConsoleLine.e (accessTypeStatistics.AccessTypesSupplyingAces);
          To.ConsoleLine.e ("accessTypeStatistics.IsInMatchingAces (ace)", accessTypeStatistics.IsInMatchingAces (ace));

          //Debugger.Break();
          //acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);
          To.ConsoleLine.e (
              "========================================================================================================================");
        }
      }
      else if (false)
      {
        To.ConsoleLine.e ("========================================================================================================================");
        To.ConsoleLine.sb().s (ace.ToString()).nl().e (() => aclProbe);
        bool aceContributed = accessTypeStatistics.IsInAccessTypesContributingAces (ace);
        To.ConsoleLine.e (aceContributed ? "Contributing ACE" : ">>> Non-contributing ACE", ace);
        To.ConsoleLine.e (accessTypeStatistics.AccessTypesSupplyingAces).se();
        if (!aceContributed && ace.SpecificAbstractRole == null && ace.SpecificPosition == null)
        {
          To.ConsoleLine.s (" !!!!!!!!!!!!! ACE has neither abstract role nor position !!!!!!!!!!!!!!!!!!!!!!!! ");
        }
      }
      else if (true)
      {
        if (!accessTypeStatistics.IsInMatchingAces (ace))
        {
          To.ConsoleLine.e (
              "------------------------------------------------------------------------------------------------------------------------");
          To.ConsoleLine.s (ace.ToString()).nl().e (() => aclProbe);
          To.ConsoleLine.e (">>> Non-matching ACE", ace);
          To.ConsoleLine.e (accessTypeStatistics.AccessTypesSupplyingAces);
          To.ConsoleLine.e (accessTypeStatistics.MatchingAces);
          To.ConsoleLine.e (
              "========================================================================================================================");
        }
      }
    }

  }
}