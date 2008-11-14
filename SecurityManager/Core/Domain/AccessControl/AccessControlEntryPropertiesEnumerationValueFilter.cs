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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryPropertiesEnumerationValueFilter : IEnumerationValueFilter
  {
    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullAndType<AccessControlEntry> ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("property", property);

      AccessControlEntry ace = (AccessControlEntry) businessObject;
      bool isStateless = ace.AccessControlList is StatelessAccessControlList;

      switch (property.Identifier)
      {
        case "TenantCondition":
          return value.IsEnabled && IsTenantConditionEnabled ((TenantCondition) value.Value, isStateless);
        case "GroupCondition":
          return value.IsEnabled && IsGroupConditionEnabled ((GroupCondition) value.Value, isStateless);
        case "UserCondition":
          return value.IsEnabled && IsUserConditionEnabled ((UserCondition) value.Value, isStateless);
        default:
          throw CreateInvalidOperationException (property.Identifier);
      }
    }

    private bool IsTenantConditionEnabled (TenantCondition value, bool isStateless)
    {
      switch (value)
      {
        case TenantCondition.None:
          return true;
        case TenantCondition.OwningTenant:
          return !isStateless;
        case TenantCondition.SpecificTenant:
          return true;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'TenantCondition'.", value);
      }
    }

    private bool IsGroupConditionEnabled (GroupCondition value, bool isStateless)
    {
      switch (value)
      {
        case GroupCondition.None:
          return true;
        case GroupCondition.OwningGroup:
          return !isStateless;
        case GroupCondition.SpecificGroup:
          return true;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          return true;
        case GroupCondition.BranchOfOwningGroup:
          return !isStateless;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupCondition'.", value);
      }
    }

    private bool IsUserConditionEnabled (UserCondition value, bool isStateless)
    {
      switch (value)
      {
        case UserCondition.None:
          return true;
        case UserCondition.Owner:
          return !isStateless;
        case UserCondition.SpecificUser:
          return true;
        case UserCondition.SpecificPosition:
          return true;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'UserCondition'.", value);
      }
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }
  }
}