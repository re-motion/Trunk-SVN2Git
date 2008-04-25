using System;

namespace Remotion.Security.Metadata
{
  public interface IClassReflector
  {
    SecurableClassInfo GetMetadata (Type type, MetadataCache cache);
  }
}
