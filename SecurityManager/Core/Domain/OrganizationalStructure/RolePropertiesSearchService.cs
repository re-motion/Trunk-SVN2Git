// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
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
  public sealed class RolePropertiesSearchService : SecurityManagerSearchServiceBase<Role>
  {
    public RolePropertiesSearchService ()
    {
      AddSearchDelegate ("Group", FindPossibleGroups);
      AddSearchDelegate ("User", FindPossibleUsers);
    }

    private IQueryable<IBusinessObject> FindPossibleGroups (
        Role role,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("role", role);

      if (role.User == null || role.User.Tenant == null)
        return Enumerable.Empty<IBusinessObject>().AsQueryable();
      return role.GetPossibleGroups (role.User.Tenant.ID).Cast<IBusinessObject>().AsQueryable();
    }

    private IQueryable<IBusinessObject> FindPossibleUsers (
        Role role,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("role", role);

      if (role.Group == null || role.Group.Tenant == null)
        return Enumerable.Empty<IBusinessObject>().AsQueryable();
      return User.FindByTenantID (role.Group.Tenant.ID).Cast<IBusinessObject>().AsQueryable();
    }
  }
}