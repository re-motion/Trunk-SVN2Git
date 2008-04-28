using System;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
  /// <summary>
  /// Marker interface, used as type parameter for the <see cref="ISecurityAdapterRegistryImplementation.SetAdapter"/> and 
  /// <see cref="ISecurityAdapterRegistryImplementation.GetAdapter{T}"/> methods of <see cref="SecurityAdapterRegistry"/>.
  /// </summary>
  public interface ISecurityAdapter
  {
  }
}
