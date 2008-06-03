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

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="Role"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="Role.Group"/> and the <see cref="Role.User"/> properties via the
  /// <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class RolePropertiesSearchService : ISearchAvailableObjectsService
  {
    private const string c_groupName = "Group";
    private const string c_userName = "User";

    public RolePropertiesSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (property.Identifier == c_groupName || property.Identifier == c_userName)
        return true;
      else
        return false;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      Role role = ArgumentUtility.CheckNotNullAndType<Role> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case c_groupName:
          if (role.User == null || role.User.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (role.GetPossibleGroups (role.User.Tenant.ID), typeof (IBusinessObject));
        case c_userName:
          if (role.Group == null || role.Group.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (User.FindByTenantID (role.Group.Tenant.ID), typeof (IBusinessObject));
        default:
          throw new ArgumentException (
              string.Format (
                  "The property '{0}' is not supported by the '{1}' type.", property.DisplayName, typeof (RolePropertiesSearchService).FullName));
      }
    }
  }
}
