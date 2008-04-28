using Remotion.Collections;
using Remotion.Implementation;

namespace Remotion.Security.BridgeInterfaces
{
  [ConcreteImplementation("Remotion.Security.BridgeImplementations.AccessTypeCacheImplementation, Remotion.Security, Version = <version>")]
  public interface IAccessTypeCacheImplementation
  {
    ICache<EnumWrapper, AccessType> CreateCache ();
  }
}