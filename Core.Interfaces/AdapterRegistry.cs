using System;
using Remotion.Implementation;
using Remotion.BridgeInterfaces;

namespace Remotion
{
  /// <summary>Used to register <see cref="IAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to another module to access information from this module.</remarks>
  /// <seealso cref="T:Remotion.Security.ISecurityAdapter"/>
  public static class AdapterRegistry
  {
    public static IAdapterRegistryImplementation Instance
    {
      get { return VersionDependentImplementationBridge<IAdapterRegistryImplementation>.Implementation; }
    }
  }
}