using System;
using System.Collections.Generic;
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
    public static Role NewObject ()
    {
      return NewObject<Role> ().With ();
    }

    protected Role ()
    {
    }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof (RolePropertiesSearchService))]
    public abstract Group Group { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    public abstract Position Position { get; set; }

    [DBBidirectionalRelation ("Roles")]
    [Mandatory]
    [SearchAvailableObjectsServiceType (typeof (RolePropertiesSearchService))]
    public abstract User User { get; set; }


    public List<Group> GetPossibleGroups (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      List<Group> groups = new List<Group> ();
      foreach (Group group in Group.FindByTenantID (tenantID))
        groups.Add (group);

      return FilterByAccess (groups, SecurityManagerAccessTypes.AssignRole);
    }

    public List<Position> GetPossiblePositions (Group group)
    {
      ArgumentUtility.CheckNotNull ("group", group);

      List<Position> positions = new List<Position> ();
      if (group.GroupType == null)
      {
        foreach (Position position in Position.FindAll ())
          positions.Add (position);
      }
      else
      {
        foreach (GroupTypePosition groupTypePosition in group.GroupType.Positions)
        {
          if (!positions.Contains (groupTypePosition.Position))
            positions.Add (groupTypePosition.Position);
        }
      }

      return FilterByAccess (positions, SecurityManagerAccessTypes.AssignRole);
    }

    private List<T> FilterByAccess<T> (List<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T : ISecurableObject
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      AccessType[] requiredAccessTypes = Array.ConvertAll<Enum, AccessType> (requiredAccessTypeEnums, AccessType.Get);
      List<T> filteredObjects = new List<T> ();
      foreach (T securableObject in securableObjects)
      {
        if (securityClient.HasAccess (securableObject, requiredAccessTypes))
          filteredObjects.Add (securableObject);
      }

      return filteredObjects;
    }

    protected override string GetOwningTenant ()
    {
      if (User != null)
        return User.Tenant.UniqueIdentifier;

      if (Group != null)
        return Group.Tenant.UniqueIdentifier;

      return null;
    }
  }
}
