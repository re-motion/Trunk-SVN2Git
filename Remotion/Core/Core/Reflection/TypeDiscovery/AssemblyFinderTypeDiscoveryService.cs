// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  /// <summary>
  /// Provides an implementation of the <see cref="ITypeDiscoveryService"/> interface that uses an <see cref="AssemblyFinder"/> to
  /// retrieve types. This class is created by <see cref="TypeDiscoveryConfiguration.CreateCustomService"/> in the default configuration and
  /// is therefore the default <see cref="ITypeDiscoveryService"/> provided by <see cref="ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService"/>
  /// in the standard context.
  /// </summary>
  public sealed class AssemblyFinderTypeDiscoveryService : ITypeDiscoveryService
  {
    // This class holds lazy, readonly static fields. It relies on the fact that the .NET runtime will reliably initialize fields in a nested static
    // class with a static constructor as lazily as possible on first access of the static field.
    // Singleton implementations with nested classes are documented here: http://csharpindepth.com/Articles/General/Singleton.aspx.
    private static class LazyStaticFields
    {
      public static readonly ILog s_log = LogManager.GetLogger (typeof (AssemblyFinderTypeDiscoveryService));

      // ReSharper disable EmptyConstructor
      // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit; this will make the static fields as lazy as possible.
      static LazyStaticFields ()
      {
      }

      // ReSharper restore EmptyConstructor
    }

    private readonly IAssemblyFinder _assemblyFinder;
    private readonly Lazy<BaseTypeCache> _baseTypeCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFinderTypeDiscoveryService"/> class with a specific <see cref="AssemblyFinder"/>
    /// instance.
    /// </summary>
    /// <param name="assemblyFinder">The assembly finder used by this service instance to retrieve types.</param>
    public AssemblyFinderTypeDiscoveryService (IAssemblyFinder assemblyFinder)
    {
      ArgumentUtility.CheckNotNull ("assemblyFinder", assemblyFinder);
      _assemblyFinder = assemblyFinder;
      _baseTypeCache = new Lazy<BaseTypeCache> (() => BaseTypeCache.Create (GetTypesFromAllAssemblies (null, true)));
    }

    /// <summary>
    /// Gets the assembly finder used by this service to discover types. The service simply returns the types returned by the
    /// <see cref="Assembly.GetTypes"/> method for the assemblies found by this object.
    /// </summary>
    /// <value>The assembly finder used for type discovery.</value>
    public IAssemblyFinder AssemblyFinder
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
      using (StopwatchScope.CreateScope (
          LazyStaticFields.s_log,
          LogLevel.Debug,
          string.Format ("Total time needed to discover types derived from '{0}': {{elapsed}}.", baseType ?? typeof (object))))
      {
        if (baseType != null && (baseType.IsSealed || baseType.IsValueType))
          return new[] { baseType };

        if (!excludeGlobalTypes)
          return GetTypesFromAllAssemblies (baseType, false).ToArray();

        return _baseTypeCache.Value.GetFromCache (baseType ?? typeof (object));
      }
    }

    private IEnumerable<Type> GetTypesFromAllAssemblies (Type baseType, bool excludeGlobalTypes)
    {
      return GetAssemblies (excludeGlobalTypes).AsParallel().SelectMany (a => GetTypesFromBaseType (a, baseType));
    }

    private IEnumerable<Assembly> GetAssemblies (bool excludeGlobalTypes)
    {
      var assemblies = _assemblyFinder.FindAssemblies();
      return assemblies.Where (assembly => !excludeGlobalTypes || !assembly.GlobalAssemblyCache);
    }

    private IEnumerable<Type> GetTypesFromBaseType (_Assembly assembly, Type baseType)
    {
      ReadOnlyCollection<Type> allTypesInAssembly;

      try
      {
        allTypesInAssembly = AssemblyTypeCache.GetTypes (assembly);
      }
      catch (ReflectionTypeLoadException ex)
      {
        string message = string.Format (
            "The types from assembly '{0}' could not be loaded.{1}{2}",
            assembly.GetName(),
            Environment.NewLine,
            string.Join (Environment.NewLine, ex.LoaderExceptions.Select (e => e.Message)));
        throw new TypeLoadException (message, ex);
      }

      if (baseType == null)
        return allTypesInAssembly;

      return GetFilteredTypes (allTypesInAssembly, baseType);
    }

    private IEnumerable<Type> GetFilteredTypes (IEnumerable<Type> types, Type baseType)
    {
      return types.Where (baseType.IsAssignableFrom);
    }
  }
}