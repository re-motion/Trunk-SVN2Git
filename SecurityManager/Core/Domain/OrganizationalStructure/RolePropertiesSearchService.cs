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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="Role"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="Role.Group"/> and the <see cref="Role.User"/> properties via the
  /// <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class RolePropertiesSearchService : SecurityManagerSearchServiceBase<Role>
  {
    public RolePropertiesSearchService ()
    {
      AddSearchDelegate ("Group", FindPossibleGroups);
      AddSearchDelegate ("User", FindPossibleUsers);
    }

    private IBusinessObject[] FindPossibleGroups (Role role, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      if (role.User == null || role.User.Tenant == null)
        return new IBusinessObject[0];
      return role.GetPossibleGroups (role.User.Tenant.ID).ToArray();
    }

    private IBusinessObject[] FindPossibleUsers (Role role, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      if (role.Group == null || role.Group.Tenant == null)
        return new IBusinessObject[0];
      return User.FindByTenantID (role.Group.Tenant.ID).ToArray();
    }
  }
}