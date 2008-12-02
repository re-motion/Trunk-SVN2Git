// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 

using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionEntryCreator
  {
    public virtual AclExpansionEntry CreateAclExpansionEntry (UserRoleAclAceCombination userRoleAclAce)
    {
      var accessTypesResult = GetAccessTypes (userRoleAclAce); 

      AclExpansionEntry aclExpansionEntry = null;

      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types.
      if (accessTypesResult.AccessTypeStatistics.IsInAccessTypesContributingAces (userRoleAclAce.Ace) && accessTypesResult.AccessInformation.AllowedAccessTypes.Length > 0)
      {
        aclExpansionEntry = new AclExpansionEntry (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, accessTypesResult.AclProbe.AccessConditions,
                                                   accessTypesResult.AccessInformation.AllowedAccessTypes, accessTypesResult.AccessInformation.DeniedAccessTypes);
      }

      return aclExpansionEntry;
    }


    public virtual AclExpansionEntryCreator_GetAccessTypesResult GetAccessTypes (UserRoleAclAceCombination userRoleAclAce) // , out AclProbe aclProbe, out AccessTypeStatistics accessTypeStatistics)
    {
      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var aclProbe = AclProbe.CreateAclProbe (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Ace);

      // Note: The aclProbe created above will NOT always match the ACE it was designed to probe; the reason for this
      // is that its SecurityToken created by the AclProbe is only designed to match the non-decideable access conditions
      // (e.g. abstract role, owning tenant, owning group, etc) of the ACE. If this were not the case, then the AclProbe would need
      // to reproduce code from the SecurityManager, to be able to decide beforehand, whether decideable access condtions
      // (e.g. specific tenant, specific user) will match or not. 
      // 
      // The "non-decideable" here refers to the information context of the AclExpander, which is lacking some information
      // available during normal SecurityManager access rights querying.
      // For decideable access conditons (e.g. specific tenant or specific group), the created SecurityToken
      // is not guaranteed to match, therefore the AccessTypeStatistics returned by Acl.GetAccessTypes are used to filter out these cases.
      //
      // One could also try to remove these entries by removing all AclExpansionEntry|s which are identical to another AclExpansionEntry,
      // apart from having more restrictive AccessConditions; note however that such "double" entries can also come from ACEs which are
      // being shadowed by a 2nd, less restrictive ACE.
      //
      // Note also that it does not suffice to get the access types for the current ACE only, since these rights might be denied
      // by another matching ACE in the current ACL (deny rights always win). 
      var accessTypeStatistics = new AccessTypeStatistics ();

      // Create a discarding sub-transaction so we can change the roles of the current user below without side effects.
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        // Set roles of user to contain only the role we currently probe for.
        // If we don't do that another role of the user can match the ACE.SpecificPosition
        // for case GroupSelection.All or GroupSelection.OwningGroup, giving access rights
        // which the user does not have due to the currently tested role.
        // (Note that the user is in fact always in all roles at the same time, so he will
        // have the access rights returned if the user's roles are not artificially reduced
        // to contain only the role probed for; it's just not the information we want to present in the 
        // ACL-expansion, where we want to distinguish which role gives rise
        // to what access rights).

        // Exchanging the User.Roles-collection with a new one containing only the current Role would not work, even
        // if a public setter would be available, so we empty the collection, then add back the current Role.
        aclProbe.SecurityToken.Principal.Roles.Clear();
        aclProbe.SecurityToken.Principal.Roles.Add (userRoleAclAce.Role);
        AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);
        
        return new AclExpansionEntryCreator_GetAccessTypesResult (accessInformation, aclProbe, accessTypeStatistics);
      }
    }    
  }
}