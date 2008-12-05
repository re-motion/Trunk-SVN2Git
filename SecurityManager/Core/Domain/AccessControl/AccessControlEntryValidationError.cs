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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// This enum lists the possible validation errors for an <see cref="AccessControlEntry"/>.
  /// </summary>
  public enum AccessControlEntryValidationError
  {
    [EnumDescription ("The TenantCondition property is set to SpecificTenant, but no SpecificTenant is assigned.")]
    IsSpecificTenantMissing,

    [EnumDescription ("The GroupCondition property is set to SpecificGroup, but no SpecificGroup is assigned.")]
    IsSpecificGroupMissing,
    
    [EnumDescription ("The GroupCondition property is set to BranchOfOwningGroup or AnyGroupWithSpecificGroupType, but no SpecificGroupType is assigned.")]
    IsSpecificGroupTypeMissing,
    
    [EnumDescription ("The UserCondition property is set to SpecificUser, but no SpecificUser is assigned.")]
    IsSpecificUserMissing,

    [EnumDescription ("The UserCondition property is set to SpecificPosition, but no SpecificPosition is assigned.")]
    IsSpecificPositionMissing,
    
    [EnumDescription ("The TenantCondition property is set to SpecificTenant or OwningTenant, but the TenantHierarchyCondition is not set.")]
    IsTenantHierarchyConditionMissing,

    [EnumDescription ("The TenantCondition property is set to SpecificTenant or OwningTenant, but the TenantHierarchyCondition is set to 'Parent', which is not supported.")]
    IsTenantHierarchyConditionOnlyParent,

    [EnumDescription ("The GroupCondition property is set to SpecificGroup or OwningGroup, but the GroupHierarchyCondition is not set.")]
    IsGroupHierarchyConditionMissing,
    
    [EnumDescription ("The GroupCondition property is set to SpecificGroup or OwningGroup, but the GroupHierarchyCondition is set to 'Parent', which is not supported.")]
    IsGroupHierarchyConditionOnlyParent,

    [EnumDescription ("The GroupCondition property is set to SpecificGroup or OwningGroup, but the GroupHierarchyCondition is set to 'Children', which is not supported.")]
    IsGroupHierarchyConditionOnlyChildren,
  }
}
