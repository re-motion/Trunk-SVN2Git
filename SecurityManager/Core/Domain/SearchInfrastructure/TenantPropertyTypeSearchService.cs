﻿// This file is part of re-strict (www.re-motion.org)
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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for properties referencing the <see cref="User"/> type.
  /// </summary>
  /// <remarks>
  /// The service can be applied to any <see cref="User"/>-typed property of a <see cref="BaseSecurityManagerObject"/> 
  /// via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class TenantPropertyTypeSearchService : SecurityManagerPropertyTypeBasedSearchServiceBase<Tenant>
  {
    protected override IQueryable<IBusinessObject> CreateQuery (
        BaseSecurityManagerObject referencingObject,
        IBusinessObjectReferenceProperty property,
        TenantConstraint tenantConstraint,
        DisplayNameConstraint displayNameConstraint)
    {
      return Tenant.FindAll().Apply (displayNameConstraint).Cast<IBusinessObject>();
    }
  }
}