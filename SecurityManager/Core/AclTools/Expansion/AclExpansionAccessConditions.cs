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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions  : IToText  
  {
    public bool IsOwningUserRequired { get; set; }
    public bool IsOwningTenantRequired { get; set; }

    public bool IsAbstractRoleRequired
    {
      get { return AbstractRole != null; }
    }
    
    public AbstractRoleDefinition AbstractRole { get; set; }


    public bool HasOwningGroupCondition
    {
      get { return OwningGroup != null; }
    }

    public Group OwningGroup { get; set; }
    public GroupHierarchyCondition GroupHierarchyCondition { get; set; }


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
        (ac.IsOwningTenantRequired == IsOwningTenantRequired) &&
        (ac.IsOwningUserRequired == IsOwningUserRequired);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (AbstractRole, OwningGroup, GroupHierarchyCondition, IsOwningTenantRequired, IsOwningUserRequired);
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AclExpansionAccessConditions>("");
      toTextBuilder.eIfNotEqualTo ("userMustOwn", IsOwningUserRequired, false);
      
      //toTextBuilder.eIfNotEqualTo ("groupMustOwn", HasOwningGroupCondition, false);
      toTextBuilder.eIfNotNull ("owningGroup", OwningGroup);
      toTextBuilder.eIfNotEqualTo ("groupHierarchyCondition", GroupHierarchyCondition, GroupHierarchyCondition.Undefined);

      toTextBuilder.eIfNotEqualTo ("tenantMustOwn", IsOwningTenantRequired, false);
      toTextBuilder.eIfNotEqualTo ("abstractRoleMustMatch", IsAbstractRoleRequired, false).eIfNotNull ("abstractRole", AbstractRole);
      toTextBuilder.ie ();
    }
  }
}