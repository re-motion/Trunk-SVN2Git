using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces as ISecurityAdapterRegistry and InstanceMember.Implementation in SecurityAssembly
  /// <summary>Used to register <see cref="ISecurityAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to the security module to access security information.</remarks>
  public class SecurityAdapterRegistry
  {
    private static readonly SecurityAdapterRegistry s_instance = new SecurityAdapterRegistry();

    public static SecurityAdapterRegistry Instance
    {
      get { return s_instance; }
    }

    private readonly Dictionary<Type, ISecurityAdapter> _registry = new Dictionary<Type, ISecurityAdapter>();

    protected SecurityAdapterRegistry()
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