using System;
using Remotion.Implementation;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
  //TODO: Move to core and rename to non-security-specific registry
  /// <summary>Used to register <see cref="IAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to the security module to access security information.</remarks>
  public static class AdapterRegistry
  {
    public static IAdapterRegistryImplementation Instance
    {
      get { return VersionDependentImplementationBridge<IAdapterRegistryImplementation>.Implementation; }
    }
  }
}