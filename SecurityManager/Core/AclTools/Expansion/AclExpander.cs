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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {
    private readonly IUserRoleAclAceCombinations _userRoleAclAceCombinations;

    public AclExpander (IUserRoleAclAceCombinations userRoleAclAceCombinations)
    {
      _userRoleAclAceCombinations = userRoleAclAceCombinations;
    }

    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      _userRoleAclAceCombinations = new UserRoleAclAceCombinations (userFinder, accessControlListFinder);
    }

    /// <summary>
    /// Default behavior is to use all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s.
    /// </summary>
    public AclExpander () : this (new AclExpanderUserFinder (), new AclExpanderAclFinder ()) {}


    ///// <summary>
    ///// Filter  <see cref="User"/>|s by the passed first-, last- and fully qualified user name. Pass <c>null</c> to not filter
    ///// for the respective name. Uses all <see cref="AccessControlList"/>|s.
    ///// </summary>
    //public AclExpander (string userFirstName, string userLastName, string userName) : 
    //  this (new AclExpanderUserFinder (userFirstName, userLastName, userName), new AclExpanderAclFinder ()) 
    //{ }


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
      //To.ConsoleLine.s ("\t\t\t").e (() => aclProbe);

      // NOTTODO: Check if we already queried with an identical token.
      // Problem: If the same token was already used by an AclProbe which has conditions then it would be incorrect
      // to skip it if it would now be coming from an AclProbe without (or with different) conditions.
      // This problem stems from the fact that due to the current SecurityManager logic which uses priorities
      // the SecurityToken of an AclProbe for an ACE can be "shadowed" by a different ACE (i.e. the ACE which is used for 
      // deciding the access rights is not the one we are probing for). This could be solved if we extend the SecurityManager
      // to support a mode where only a specific ACE shall be used for deciding the access rights and if that ACE is not
      // in the set of ACEs used then no access rights shall be returned. MK is reluctant to allow these (small) changes due
      // to code purity concerns; since it is planned to remove priorities from the SecurityManager 
      // in the near future (to be replaced by a deny concept), which transforms the problem, since any matching ACE
      // will contribute to the access rights result (deny rights can still lead to it not having any impact, though)
      // it was therefore decided to ignore these (up to 9 = 2^4+1) "double entries" for now.
      // See below for a solution which does not change the access right logic but only records the contributing ACEs.
      
      // AccessTypeDefinition[] accessTypeDefinitions = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken);

      // Call extended AccessControlList.GetAccessTypes-method which returns information about the ACEs which contributed to
      // the resulting AccessType|s.
      var accessTypeStatistics = new AccessTypeStatistics();
      AccessTypeDefinition[] accessTypeDefinitions = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);



      // We only create an AclExpansionEntry if the current probe ACE contributed to the returned AccessTypes
      if (accessTypeStatistics.IsInAccessTypesSupplyingAces (ace) && accessTypeDefinitions.Length > 0)
      //if (accessTypeDefinitions.Length > 0)
      {
        var aclExpansionEntry = new AclExpansionEntry (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, aclProbe.AccessConditions, accessTypeDefinitions);
        //To.ConsoleLine.s ("\t\t\t").e (() => aclExpansionEntry);
        aclExpansionEntries.Add (aclExpansionEntry);
      }
    }


  }
}