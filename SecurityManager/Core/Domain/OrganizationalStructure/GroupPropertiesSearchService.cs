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
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="Group"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="Group.Parent"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class GroupPropertiesSearchService : SecurityManagerSearchServiceBase<Group>
  {
    public GroupPropertiesSearchService ()
    {
      AddSearchDelegate ("Parent", FindPossibleParentGroups);
    }

    private IBusinessObject[] FindPossibleParentGroups (Group group, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      if (group.Tenant == null)
        return new IBusinessObject[0];
      return group.GetPossibleParentGroups (group.Tenant.ID).ToArray();
    }
  }
}