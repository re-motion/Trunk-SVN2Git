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
using Remotion.Collections;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpander
  {
    private readonly IUserRoleAclAceCombinations _userRoleAclAceCombinations;
    private readonly Infrastructure.AclExpansionEntryCreator _aclExpansionEntryCreator = new Infrastructure.AclExpansionEntryCreator ();

    // IEqualityComparer for value based comparison of AclExpansionEntry|s.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryEqualityComparer =
      new CompoundValueEqualityComparer<AclExpansionEntry> (a => new object[] {
          a.User, a.Role, a.Class, a.AccessControlList is StatefulAccessControlList ? a.StateCombinations : null,
          a.AccessConditions.AbstractRole,
          a.AccessConditions.GroupHierarchyCondition,
          a.AccessConditions.IsOwningUserRequired,
          a.AccessConditions.OwningGroup,
          a.AccessConditions.OwningTenant,
          a.AccessConditions.TenantHierarchyCondition,
          EnumerableEqualsWrapper.New (a.AllowedAccessTypes),
          EnumerableEqualsWrapper.New (a.DeniedAccessTypes)
      }
    );

    public AclExpander (IUserRoleAclAceCombinations userRoleAclAceCombinations)
    {
      ArgumentUtility.CheckNotNull ("userRoleAclAceCombinations", userRoleAclAceCombinations);
      _userRoleAclAceCombinations = userRoleAclAceCombinations;
    }

    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
      : this (new UserRoleAclAceCombinations (userFinder, accessControlListFinder))
    {}

    /// <summary>
    /// Default behavior is to use all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s.
    /// </summary>
    public AclExpander () : this (new AclExpanderUserFinder (), new AclExpanderAclFinder ()) {}


    public Infrastructure.AclExpansionEntryCreator AclExpansionEntryCreator
    {
      get { return _aclExpansionEntryCreator; }
    }


    public List<AclExpansionEntry> GetAclExpansionEntryListSortedAndDistinct ()
    {
      return (from AclExpansionEntry aclExpansionEntry in GetAclExpansionEntryList ()
              orderby aclExpansionEntry.User.DisplayName, aclExpansionEntry.Role.DisplayName
              select aclExpansionEntry).Distinct (_aclExpansionEntryEqualityComparer).ToList ();
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="IEnumerable{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<AclExpansionEntry> GetAclExpansionEntries ()
    {
      foreach (UserRoleAclAceCombination userRoleAclAce in _userRoleAclAceCombinations)
      {
        AclExpansionEntry aclExpansionEntry = AclExpansionEntryCreator.CreateAclExpansionEntry (userRoleAclAce);
        if (aclExpansionEntry != null)
        {
          yield return aclExpansionEntry;
        }
      }
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    /// <returns></returns>
    public List<AclExpansionEntry> GetAclExpansionEntryList ()
    {
      return GetAclExpansionEntries().ToList();
    }
  }
}