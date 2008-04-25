using System;
using System.Collections.Generic;

namespace Remotion.Security.Metadata
{

  public interface IEnumerationReflector
  {
    Dictionary<Enum, EnumValueInfo> GetValues (Type type, MetadataCache cache);
    EnumValueInfo GetValue (Enum value, MetadataCache cache);
  }

}