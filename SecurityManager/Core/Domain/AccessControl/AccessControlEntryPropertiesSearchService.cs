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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="AccessControlEntry.SpecificTenant"/>, <see cref="AccessControlEntry.SpecificPosition"/>, 
  /// and <see cref="AccessControlEntry.SpecificAbstractRole"/> properties via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class AccessControlEntryPropertiesSearchService : SecurityManagerSearchServiceBase<AccessControlEntry>
  {
    public AccessControlEntryPropertiesSearchService ()
    {
      AddSearchDelegate ("SpecificTenant", delegate { return Tenant.FindAll().Cast<IBusinessObject>().AsQueryable(); });
      AddSearchDelegate ("SpecificGroup", SearchGroups);
      AddSearchDelegate ("SpecificUser", SearchUsers);
      AddSearchDelegate ("SpecificPosition", delegate { return Position.FindAll().Cast<IBusinessObject>().AsQueryable(); });
      AddSearchDelegate ("SpecificGroupType", delegate { return GroupType.FindAll().Cast<IBusinessObject>().AsQueryable(); });
      AddSearchDelegate ("SpecificAbstractRole", delegate { return AbstractRoleDefinition.FindAll().Cast<IBusinessObject>().AsQueryable(); });
    }

    private IQueryable<IBusinessObject> SearchGroups (
        AccessControlEntry referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var securityManagerSearchArguments = ArgumentUtility.CheckNotNullAndType<SecurityManagerSearchArguments> ("searchArguments", searchArguments);
      var tenantConstraint = (ITenantConstraint) securityManagerSearchArguments;

      var groups = Group.FindByTenantID (tenantConstraint.Value);

      var displayNameConstraint = (IDisplayNameConstraint) securityManagerSearchArguments;
      if (!string.IsNullOrEmpty (displayNameConstraint.Text))
        groups = groups.Where (g => g.Name.Contains (displayNameConstraint.Text) || g.ShortName.Contains (displayNameConstraint.Text));

      return groups.Cast<IBusinessObject>();
    }

    private IQueryable<IBusinessObject> SearchUsers (
        AccessControlEntry referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var securityManagerSearchArguments = ArgumentUtility.CheckNotNullAndType<SecurityManagerSearchArguments> ("searchArguments", searchArguments);
      var tenantFilter = (ITenantConstraint) securityManagerSearchArguments;

      return User.FindByTenantID (tenantFilter.Value).Cast<IBusinessObject>().AsQueryable();
    }
  }
}
