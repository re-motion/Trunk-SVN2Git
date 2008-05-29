using System;
using Remotion.Implementation;

namespace Remotion.Security.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Security.BridgeImplementations.AdapterRegistryImplementation, Remotion.Security, Version = <version>")]
  public interface IAdapterRegistryImplementation
  {
    void SetAdapter (Type adapterType, IAdapter value);
    T GetAdapter<T>() where T : class, IAdapter;
  }
}