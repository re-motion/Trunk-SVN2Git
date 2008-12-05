// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
    private readonly IUserRoleAclAceCombinationFinder _userRoleAclAceCombinationFinder;
    private readonly Infrastructure.AclExpansionEntryCreator _aclExpansionEntryCreator = new Infrastructure.AclExpansionEntryCreator ();

    // IEqualityComparer for value based comparison of AclExpansionEntry|s.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryEqualityComparer =
      new CompoundValueEqualityComparer<AclExpansionEntry> (a => new object[] {
          a.User, a.Role, a.Class, a.AccessControlList is StatefulAccessControlList ? a.GetStateCombinations() : null,
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

    public AclExpander (IUserRoleAclAceCombinationFinder userRoleAclAceCombinationFinder)
    {
      ArgumentUtility.CheckNotNull ("userRoleAclAceCombinationFinder", userRoleAclAceCombinationFinder);
      _userRoleAclAceCombinationFinder = userRoleAclAceCombinationFinder;
    }

    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
      : this (new UserRoleAclAceCombinationFinder (userFinder, accessControlListFinder))
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
      foreach (UserRoleAclAceCombination userRoleAclAce in _userRoleAclAceCombinationFinder)
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
