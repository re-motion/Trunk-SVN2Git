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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions  : IToTextConvertible  
  {
    public bool IsOwningUserRequired { get; set; }

    public bool IsAbstractRoleRequired
    {
      get { return AbstractRole != null; }
    }
    
    public AbstractRoleDefinition AbstractRole { get; set; }


    // Owning Group
    public Group OwningGroup { get; set; }
    public GroupHierarchyCondition GroupHierarchyCondition { get; set; }

    public bool HasOwningGroupCondition
    {
      get { return OwningGroup != null; }
    }


    // Owning Tenant
    public Tenant OwningTenant { get; set; }
    public TenantHierarchyCondition TenantHierarchyCondition { get; set; }
    
    public bool HasOwningTenantCondition 
    {
      get { return OwningTenant != null; }
    }


    public override bool Equals (object obj)
    {
      var ac = obj as AclExpansionAccessConditions;
      if (ac == null)
      {
        return false;
      }

      return (ac.AbstractRole == AbstractRole) &&
        (ac.OwningGroup == OwningGroup) &&
        (ac.GroupHierarchyCondition == GroupHierarchyCondition) &&
        (ac.OwningTenant == OwningTenant) &&
        (ac.TenantHierarchyCondition == TenantHierarchyCondition) &&
        (ac.IsOwningUserRequired == IsOwningUserRequired);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (AbstractRole, OwningGroup, GroupHierarchyCondition, 
        OwningTenant, TenantHierarchyCondition, IsOwningUserRequired);
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AclExpansionAccessConditions>("");
      toTextBuilder.eIfNotEqualTo ("userMustOwn", IsOwningUserRequired, false);
      
      //toTextBuilder.eIfNotEqualTo ("groupMustOwn", HasOwningGroupCondition, false);
      toTextBuilder.eIfNotNull ("owningGroup", OwningGroup);
      toTextBuilder.eIfNotEqualTo ("groupHierarchyCondition", GroupHierarchyCondition, GroupHierarchyCondition.Undefined);

      toTextBuilder.eIfNotEqualTo ("tenantMustOwn", HasOwningTenantCondition, false);
      toTextBuilder.eIfNotEqualTo ("abstractRoleMustMatch", IsAbstractRoleRequired, false).eIfNotNull ("abstractRole", AbstractRole);
      toTextBuilder.ie ();
    }
  }
}