using System;
using System.ComponentModel;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AccessTypeDefinition : EnumValueDefinition
  {
    public static AccessTypeDefinition NewObject ()
    {
      return NewObject<AccessTypeDefinition> ().With ();
    }

    public static AccessTypeDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AccessTypeDefinition> ().With (metadataItemID, name, value);
    }

    protected AccessTypeDefinition ()
    {
    }

    protected AccessTypeDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation ("AccessType")]
    public abstract ObjectList<AccessTypeReference> References { get; }

    [DBBidirectionalRelation ("AccessType")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected abstract ObjectList<Permission> Permissions { get; }
  }
}
