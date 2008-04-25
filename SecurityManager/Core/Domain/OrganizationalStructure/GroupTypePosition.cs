using System;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupTypePosition")]
  [PermanentGuid ("E2BF5572-DDFF-4319-8824-B41653950860")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupTypePosition : OrganizationalStructureObject
  {
    public static GroupTypePosition NewObject ()
    {
      return NewObject<GroupTypePosition> ().With ();
    }

    protected GroupTypePosition ()
    {
    }

    [DBBidirectionalRelation ("Positions")]
    [Mandatory]
    public abstract GroupType GroupType { get; set; }

    [DBBidirectionalRelation ("GroupTypes")]
    [Mandatory]
    public abstract Position Position { get; set; }

    public override string DisplayName
    {
      get
      {
        string groupTypeName = (GroupType != null) ? GroupType.Name : string.Empty;
        string positionName = (Position != null) ? Position.Name : string.Empty;

        return string.Format ("{0} / {1}", groupTypeName, positionName); 
      }
    }
  }
}
