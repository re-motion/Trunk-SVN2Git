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
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for properties referencing the <see cref="Group"/> type.
  /// The <see cref="ISearchAvailableObjectsService.Search"/> method filters by <see cref="ITenantConstraint"/>, <see cref="IDisplayNameConstraint"/>,
  /// and <see cref="IResultSizeConstraint"/>.
  /// </summary>
  /// <remarks>
  /// The service can be applied to any <see cref="Group"/>-typed property of a <see cref="BaseSecurityManagerObject"/> 
  /// via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class GroupPropertyTypeSearchService : SecurityManagerPropertyTypeBasedSearchServiceBase<Group>
  {
    protected override IQueryable<IBusinessObject> CreateQuery (
        BaseSecurityManagerObject referencingObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("searchArguments", searchArguments);
      var tenantFilter = (ITenantConstraint) searchArguments;

      return Group.FindByTenantID (tenantFilter.Value).Apply ((IDisplayNameConstraint) searchArguments).Cast<IBusinessObject>().AsQueryable();
    }
  }
}