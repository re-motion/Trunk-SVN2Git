// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
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

    private IBusinessObject[] FindPossibleGroups (Role role, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      if (role.User == null || role.User.Tenant == null)
        return new IBusinessObject[0];
      return role.GetPossibleGroups (role.User.Tenant.ID).ToArray();
    }

    private IBusinessObject[] FindPossibleUsers (Role role, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      if (role.Group == null || role.Group.Tenant == null)
        return new IBusinessObject[0];
      return User.FindByTenantID (role.Group.Tenant.ID).ToArray();
    }
  }
}
