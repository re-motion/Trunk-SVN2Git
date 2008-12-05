// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
