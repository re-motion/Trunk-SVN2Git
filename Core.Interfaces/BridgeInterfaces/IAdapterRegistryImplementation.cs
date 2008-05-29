using System;
using Remotion.Implementation;

namespace Remotion.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.BridgeImplementations.AdapterRegistryImplementation, Remotion, Version = <version>")]
  public interface IAdapterRegistryImplementation
  {
    void SetAdapter (Type adapterType, IAdapter value);
    T GetAdapter<T>() where T : class, IAdapter;
  }
}