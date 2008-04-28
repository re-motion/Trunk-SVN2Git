using System;
using Remotion.Implementation;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
  /// <summary>Used to register <see cref="ISecurityAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to the security module to access security information.</remarks>
  public static class SecurityAdapterRegistry
  {
    public static ISecurityAdapterRegistryImplementation Instance
    {
      get { return VersionDependentImplementationBridge<ISecurityAdapterRegistryImplementation>.Implementation; }
    }
  }
}