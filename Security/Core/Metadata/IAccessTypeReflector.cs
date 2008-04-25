using System;
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Security.Metadata
{
  public interface IAccessTypeReflector
  {
    List<EnumValueInfo> GetAccessTypesFromType (Type type, MetadataCache cache);
    List<EnumValueInfo> GetAccessTypesFromAssembly (Assembly assembly, MetadataCache cache);
  }
}
