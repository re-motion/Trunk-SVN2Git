using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides an implementation of the <see cref="ITypeDiscoveryService"/> interface that uses an <see cref="AssemblyFinder"/> to
  /// retrieve types. This class can be used to build components that can retrieve types via <see cref="IDesignerHost"/> and
  /// <see cref="AssemblyFinder"/> using the same code for both. For example, it is used by <see cref="ContextAwareTypeDiscoveryService"/>
  /// in the standard context.
  /// </summary>
  public class AssemblyFinderTypeDiscoveryService : ITypeDiscoveryService
  {
    private readonly AssemblyFinder _assemblyFinder;

    private Assembly[] _assemblyCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFinderTypeDiscoveryService"/> class with a specific <see cref="AssemblyFinder"/>
    /// instance.
    /// </summary>
    /// <param name="assemblyFinder">The assembly finder used by this service instance to retrieve types.</param>
    public AssemblyFinderTypeDiscoveryService (AssemblyFinder assemblyFinder)
    {
      ArgumentUtility.CheckNotNull ("assemblyFinder", assemblyFinder);
      _assemblyFinder = assemblyFinder;
    }

    /// <summary>
    /// Gets the assembly finder used by this service to discover types. The service simply returns the types returned by the
    /// <see cref="Assembly.GetTypes"/> method for the assemblies found by this object.
    /// </summary>
    /// <value>The assembly finder used for type discovery.</value>
    public AssemblyFinder AssemblyFinder
    {
      get { return _assemblyFinder; }
    }

    /// <summary>
    /// Retrieves the list of types available in the assemblies found by the <see cref="AssemblyFinder"/> specified in the constructor.
    /// </summary>
    /// <param name="baseType">The base type to match. Can be null.</param>
    /// <param name="excludeGlobalTypes">Indicates whether types from all referenced assemblies should be checked.</param>
    /// <returns>
    /// A collection of types that match the criteria specified by baseType and excludeGlobalTypes.
    /// </returns>
    public ICollection GetTypes (Type baseType, bool excludeGlobalTypes)
    {
      List<Type> types = new List<Type>();
      foreach (Assembly assembly in GetAssemblies (excludeGlobalTypes))
        types.AddRange (GetTypes (assembly, baseType));

      return types;
    }

    private IEnumerable<Type> GetTypes (Assembly assembly, Type baseType)
    {
      Type[] allTypesInAssembly = assembly.GetTypes ();
      if (baseType == null)
        return allTypesInAssembly;
      else
        return GetFilteredTypes (allTypesInAssembly, baseType);
    }

    private IEnumerable<Type> GetFilteredTypes (IEnumerable<Type> types, Type baseType)
    {
      foreach (Type type in types)
      {
        if (baseType.IsAssignableFrom (type))
          yield return type;
      }
    }

    private IEnumerable<Assembly> GetAssemblies (bool excludeGlobalTypes)
    {
      if (_assemblyCache == null)
        _assemblyCache = _assemblyFinder.FindAssemblies();

      foreach (Assembly assembly in _assemblyCache)
      {
        if (!excludeGlobalTypes || !assembly.GlobalAssemblyCache)
          yield return assembly;
      }
    }
  }
}