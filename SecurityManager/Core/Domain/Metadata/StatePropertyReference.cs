using System;
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StatePropertyReference : BaseSecurityManagerObject
  {
    public static StatePropertyReference NewObject ()
    {
      return NewObject<StatePropertyReference> ().With ();
    }

    protected StatePropertyReference ()
    {
    }

    [DBBidirectionalRelation ("StatePropertyReferences")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("References")]
    [Mandatory]
    public abstract StatePropertyDefinition StateProperty { get; set; }

  }
}
