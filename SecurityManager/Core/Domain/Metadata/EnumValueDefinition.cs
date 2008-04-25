using System;
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [DBTable]
  public abstract class EnumValueDefinition : MetadataObject
  {
    protected EnumValueDefinition ()
    {
    }

    public abstract int Value { get; set; }
  }
}
