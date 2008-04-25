using System;
using System.Reflection;

namespace Remotion.Security.Metadata
{

  public interface IStatePropertyReflector
  {
    StatePropertyInfo GetMetadata (PropertyInfo property, MetadataCache cache);
  }

}