using System;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.BridgeInterfaces;

namespace Remotion.BridgeImplementations
{
  public class AdapterRegistryImplementation : IAdapterRegistryImplementation
  {
    private readonly Dictionary<Type, IAdapter> _registry = new Dictionary<Type, IAdapter> ();

    public AdapterRegistryImplementation()
    {
    }

    public void SetAdapter (Type adapterType, IAdapter value)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("adapterType", adapterType, typeof (IAdapter));
      ArgumentUtility.CheckType ("value", value, adapterType);

      _registry[adapterType] = value;
    }

    public T GetAdapter<T>() where T : class, IAdapter
    {
      if (_registry.ContainsKey (typeof (T)))
        return (T) _registry[typeof (T)];
      else
        return null;
    }

  }
}