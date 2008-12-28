// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
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
  public sealed class SubstitutionPropertiesSearchService : SecurityManagerSearchServiceBase<Substitution>
  {
    public SubstitutionPropertiesSearchService ()
    {
      AddSearchDelegate ("SubstitutingUser", FindPossibleSubstitutingUsers);
      AddSearchDelegate ("SubstitutedRole", FindPossibleSubstitutedRoles);
    }

    private IBusinessObject[] FindPossibleSubstitutingUsers (
        Substitution substitution, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      var defaultSearchArguments = ArgumentUtility.CheckNotNullAndType<DefaultSearchArguments> ("searchArguments", searchArguments);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultSearchArguments.SearchStatement", defaultSearchArguments.SearchStatement);
      ObjectID tenantID = ObjectID.Parse (defaultSearchArguments.SearchStatement);

      return User.FindByTenantID (tenantID).ToArray();
    }

    private IBusinessObject[] FindPossibleSubstitutedRoles (
        Substitution substitution, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("substitution", substitution);

      if (substitution.SubstitutedUser == null)
        return new IBusinessObject[0];
      return substitution.SubstitutedUser.Roles.ToArray();
    }
  }
}
