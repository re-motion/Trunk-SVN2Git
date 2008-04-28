using System;
using Remotion.Implementation;

namespace Remotion.Security.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Security.BridgeImplementations.SecurityAdapterRegistryImplementation, Remotion.Security, Version = <version>")]
  public interface ISecurityAdapterRegistryImplementation
  {
    void SetAdapter (Type adapterType, ISecurityAdapter value);
    T GetAdapter<T>() where T : class, ISecurityAdapter;
  }
}