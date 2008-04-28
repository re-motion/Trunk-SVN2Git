using System;
using Remotion.Collections;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security.BridgeImplementations
{
  public class AccessTypeCacheImplementation : IAccessTypeCacheImplementation
  {
    public ICache<EnumWrapper, AccessType> CreateCache ()
    {
      return new InterlockedCache<EnumWrapper, AccessType> ();
    }
  }
}