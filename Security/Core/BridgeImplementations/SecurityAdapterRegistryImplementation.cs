using System;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security.BridgeImplementations
{
  public class SecurityAdapterRegistryImplementation : ISecurityAdapterRegistryImplementation
  {
    private readonly Dictionary<Type, ISecurityAdapter> _registry = new Dictionary<Type, ISecurityAdapter> ();

    public SecurityAdapterRegistryImplementation()
    {
    }

    public void SetAdapter (Type adapterType, ISecurityAdapter value)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("adapterType", adapterType, typeof (ISecurityAdapter));
      ArgumentUtility.CheckType ("value", value, adapterType);

      _registry[adapterType] = value;
    }

    public T GetAdapter<T>() where T : class, ISecurityAdapter
    {
      if (_registry.ContainsKey (typeof (T)))
        return (T) _registry[typeof (T)];
      else
        return null;
    }

  }
}