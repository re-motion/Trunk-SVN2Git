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
using System.Collections.Generic;
using System.Linq;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for properties of the <see cref="Role"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="Role.Position"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class RolePropertiesSearchService : ISearchAvailableObjectsService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return property.Identifier == "Position";
    }

    public IBusinessObject[] Search (
        IBusinessObject referencingObject,
        IBusinessObjectReferenceProperty property,
        ISearchAvailableObjectsArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      var rolePropertiesSearchArguments = ArgumentUtility.CheckType<RolePropertiesSearchArguments> ("searchArguments", searchArguments);

      if (!SupportsProperty (property))
      {
        throw new ArgumentException (
            string.Format ("The property '{0}' is not supported by the '{1}' type.", property.Identifier, GetType().FullName));
      }

      var positions = GetPositions (rolePropertiesSearchArguments);
      var filteredPositions = FilterByAccess (positions, SecurityManagerAccessTypes.AssignRole);
      return filteredPositions.ToArray();
    }

    private IQueryable<Position> GetPositions (RolePropertiesSearchArguments defaultSearchArguments)
    {
      var positions = Position.FindAll();

      var groupType = GetGroupType (defaultSearchArguments);
      if (groupType == null)
        return positions;

      return positions.Where (p => p.GroupTypes.Any (gtp => gtp.GroupType == groupType));
    }

    private GroupType GetGroupType (RolePropertiesSearchArguments searchArguments)
    {
      if (searchArguments == null || searchArguments.GroupID == null)
        return null;

      var group = Group.GetObject (searchArguments.GroupID);
      return group.GroupType;
    }

    private IEnumerable<T> FilterByAccess<T> (IEnumerable<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T: ISecurableObject
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      AccessType[] requiredAccessTypes = Array.ConvertAll (requiredAccessTypeEnums, AccessType.Get);

      return securableObjects.Where (o => securityClient.HasAccess (o, requiredAccessTypes));
    }
  }
}