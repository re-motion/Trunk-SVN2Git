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

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="User"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="User.OwningGroup"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class UserPropertiesSearchService : SecurityManagerSearchServiceBase<User>
  {
    public UserPropertiesSearchService ()
    {
      AddSearchDelegate ("OwningGroup", FindPossibleOwningGroups);
    }

    private IBusinessObject[] FindPossibleOwningGroups (User user, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      if (user.Tenant == null)
        return new IBusinessObject[0];
      return Group.FindByTenantID (user.Tenant.ID).ToArray();
    }
  }
}