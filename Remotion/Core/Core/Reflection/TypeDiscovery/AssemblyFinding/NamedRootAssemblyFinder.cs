// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using System.Reflection;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Loads root assemblies by assembly names.
  /// </summary>
  public class NamedRootAssemblyFinder : IRootAssemblyFinder
  {
    /// <summary>
    /// Holds an <see cref="System.Reflection.AssemblyName"/> and a flag indicating whether to include referenced assemblies.
    /// </summary>
    public struct Specification
    {
      public Specification (AssemblyName assemblyName, bool followReferences)
          : this()
      {
        ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);

        AssemblyName = assemblyName;
        FollowReferences = followReferences;
      }

      public AssemblyName AssemblyName { get; private set; }
      public bool FollowReferences { get; private set; }

      public override string ToString ()
      {
        return "Specification: " + AssemblyName;
      }
    }

    private readonly IEnumerable<Specification> _assemblySpecifications;

    public NamedRootAssemblyFinder (IEnumerable<Specification> assemblySpecifications)
    {
      ArgumentUtility.CheckNotNull ("assemblySpecifications", assemblySpecifications);
      _assemblySpecifications = assemblySpecifications;
    }

    public RootAssembly[] FindRootAssemblies (IAssemblyLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      var rootAssemblies = from specification in _assemblySpecifications
                           let assembly = loader.TryLoadAssembly (specification.AssemblyName, specification.ToString())
                           where assembly != null
                           select new RootAssembly(assembly, specification.FollowReferences);
      return rootAssemblies.Distinct().ToArray ();
    }
  }
}