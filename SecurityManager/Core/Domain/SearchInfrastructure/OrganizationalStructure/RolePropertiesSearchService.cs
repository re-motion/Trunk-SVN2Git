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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure
{
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
      var defaultSearchArguments = ArgumentUtility.CheckType<DefaultSearchArguments> ("searchArguments", searchArguments);

      if (!SupportsProperty (property))
      {
        throw new ArgumentException (
            string.Format ("The property '{0}' is not supported by the '{1}' type.", property.Identifier, GetType().FullName));
      }

      var positions = GetPositions (defaultSearchArguments);
      var filteredPositions = FilterByAccess (positions, SecurityManagerAccessTypes.AssignRole);
      return filteredPositions.ToArray();
    }

    private GroupType GetGroupType (DefaultSearchArguments searchArguments)
    {
      if (searchArguments == null || string.IsNullOrEmpty (searchArguments.SearchStatement))
        return null;

      var groupID = ObjectID.Parse (searchArguments.SearchStatement);
      var group = Group.GetObject (groupID);
      return group.GroupType;
    }

    private IQueryable<Position> GetPositions (DefaultSearchArguments defaultSearchArguments)
    {
      var positions = Position.FindAll();

      var groupType = GetGroupType (defaultSearchArguments);
      if (groupType == null)
        return positions;

      return positions.Where (p => p.GroupTypes.Any (gtp => gtp.GroupType == groupType));
    }

    private IEnumerable<T> FilterByAccess<T> (IEnumerable<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T: ISecurableObject
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      AccessType[] requiredAccessTypes = Array.ConvertAll (requiredAccessTypeEnums, AccessType.Get);

      return securableObjects.Where (o => securityClient.HasAccess (o, requiredAccessTypes));
    }
  }
}