using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Permission : AccessControlObject
  {
    public static Permission NewObject ()
    {
      return NewObject<Permission> ().With ();
    }

    protected Permission ()
    {
    }

    public abstract int Index { get; set; }

    [StorageClassNone]
    public bool BinaryAllowed
    {
      get { return Allowed ?? false; }
      set { Allowed = value ? (bool?) true : null; }
    }

    public abstract bool? Allowed { get; set; }

    [DBBidirectionalRelation ("Permissions")]
    [DBColumn ("AccessTypeDefinitionID")]
    [Mandatory]
    public abstract AccessTypeDefinition AccessType { get; set; }

    [DBBidirectionalRelation ("Permissions")]
    [Mandatory]
    public abstract AccessControlEntry AccessControlEntry { get; }
  }
}
