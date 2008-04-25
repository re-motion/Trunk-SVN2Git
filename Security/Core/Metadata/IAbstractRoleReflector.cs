using System;
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Security.Metadata
{
  public interface IAbstractRoleReflector
  {
    List<EnumValueInfo> GetAbstractRoles (Assembly assembly, MetadataCache cache);
  }
}
