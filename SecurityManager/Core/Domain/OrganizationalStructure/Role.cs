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
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Role")]
  [PermanentGuid ("23C68C62-5B0F-4857-8DF2-C161C0077745")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Role : OrganizationalStructureObject
  {
    private DomainObjectDeleteHandler _deleteHandler;

    public static Role NewObject ()
    {
      return NewObject<Role>();
    }

    protected Role ()
    {
    }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (RolePropertiesSearchService))]
    public abstract Group Group { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    public abstract Position Position { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (RolePropertiesSearchService))]
    public abstract User User { get; set; }

    [DBBidirectionalRelation ("SubstitutedRole")]
    public abstract ObjectList<Substitution> SubstitutedBy { get; }

    public List<Group> GetPossibleGroups (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      IEnumerable<Group> groups = Group.FindByTenantID (tenantID);

      return FilterByAccess (groups, SecurityManagerAccessTypes.AssignRole);
    }

    public List<Position> GetPossiblePositions (Group group)
    {
      ArgumentUtility.CheckNotNull ("group", group);

      IEnumerable<Position> positions;
      if (group.GroupType == null)
        positions = Position.FindAll();
      else
        positions = group.GroupType.Positions.Select (gtp => gtp.Position).Distinct();

      return FilterByAccess (positions, SecurityManagerAccessTypes.AssignRole);
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (SubstitutedBy);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete ();
    }

    private List<T> FilterByAccess<T> (IEnumerable<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T: ISecurableObject
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      AccessType[] requiredAccessTypes = Array.ConvertAll<Enum, AccessType> (requiredAccessTypeEnums, AccessType.Get);

      return securableObjects.Where (o => securityClient.HasAccess (o, requiredAccessTypes)).ToList();
    }

    protected override string GetOwningTenant ()
    {
      if (User != null)
        return User.Tenant.UniqueIdentifier;

      if (Group != null)
        return Group.Tenant.UniqueIdentifier;

      return null;
    }

    public override string DisplayName
    {
      get
      {
        string positionName = Position != null ? Position.DisplayName : null;
        string groupName = Group != null ? Group.DisplayName : null;

        return string.Format ("{0} / {1}", positionName ?? "?", groupName ?? "?" );
      }
    }
  }
}
