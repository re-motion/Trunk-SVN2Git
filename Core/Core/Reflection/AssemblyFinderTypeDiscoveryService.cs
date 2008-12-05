// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides an implementation of the <see cref="ITypeDiscoveryService"/> interface that uses an <see cref="AssemblyFinder"/> to
  /// retrieve types. This class can be used to build components that can retrieve types via <see cref="IDesignerHost"/> and
  /// <see cref="AssemblyFinder"/> using the same code for both. For example, it is used by <see cref="ContextAwareTypeDiscoveryUtility"/>
  /// in the standard context.
  /// </summary>
  public class AssemblyFinderTypeDiscoveryService : ITypeDiscoveryService
  {
    private readonly AssemblyFinder _assemblyFinder;

    private _Assembly[] _assemblyCache;

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
      var types = new List<Type>();
      foreach (_Assembly assembly in GetAssemblies (excludeGlobalTypes))
        types.AddRange (GetTypes (assembly, baseType));

      return types;
    }

    private IEnumerable<Type> GetTypes (_Assembly assembly, Type baseType)
    {
      Type[] allTypesInAssembly;
      try
      {
        allTypesInAssembly = assembly.GetTypes ();
      }
      catch (ReflectionTypeLoadException ex)
      {
        string message = string.Format ("The types from assembly '{0}' could not be loaded.{1}{2}", assembly.GetName (), Environment.NewLine, SeparatedStringBuilder.Build (Environment.NewLine, ex.LoaderExceptions, e => e.Message));
        throw new TypeLoadException (message, ex);
      }

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

    private IEnumerable<_Assembly> GetAssemblies (bool excludeGlobalTypes)
    {
      if (_assemblyCache == null)
        _assemblyCache = _assemblyFinder.FindMockableAssemblies();

      foreach (_Assembly assembly in _assemblyCache)
      {
        if (!excludeGlobalTypes || !assembly.GlobalAssemblyCache)
          yield return assembly;
      }
    }
  }
}
