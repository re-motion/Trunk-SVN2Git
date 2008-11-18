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

    [EnumDescription ("The GroupCondition property is set to SpecificGroup or OwningGroup, but the GroupHierarchyCondition is not set.")]
    IsGroupHierarchyConditionMissing,
  }
}