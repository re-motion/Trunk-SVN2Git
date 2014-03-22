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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Finds assemblies using an <see cref="IRootAssemblyFinder"/> and an <see cref="IAssemblyLoader"/>. The <see cref="IRootAssemblyFinder"/> is
  /// used to find a set of root assemblies, the <see cref="AssemblyFinder"/> automatically traverses the assembly references to (transitively)
  /// find all referenced assemblies as well. The root assemblies and referenced assemblies are loaded with the <see cref="IAssemblyLoader"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class AssemblyFinder : IAssemblyFinder
  {
    private static readonly Lazy<ILog> s_log = new Lazy<ILog> (() => LogManager.GetLogger (typeof (AssemblyFinder)));

    private readonly IRootAssemblyFinder _rootAssemblyFinder;
    private readonly IAssemblyLoader _assemblyLoader;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFinder"/> class.
    /// </summary>
    /// <param name="rootAssemblyFinder">The <see cref="IRootAssemblyFinder"/> to use for finding the root assemblies.</param>
    /// <param name="assemblyLoader">The <see cref="IAssemblyLoader"/> to use for loading the assemblies found.</param>
    public AssemblyFinder (IRootAssemblyFinder rootAssemblyFinder, IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull ("rootAssemblyFinder", rootAssemblyFinder);
      ArgumentUtility.CheckNotNull ("assemblyLoader", assemblyLoader);

      _rootAssemblyFinder = rootAssemblyFinder;
      _assemblyLoader = assemblyLoader;
    }

    public IRootAssemblyFinder RootAssemblyFinder
    {
      get { return _rootAssemblyFinder; }
    }

    public IAssemblyLoader AssemblyLoader
    {
      get { return _assemblyLoader; }
    }

    /// <summary>
    /// Uses the <see cref="RootAssemblyFinder"/> to find root assemblies and returns them together with all directly or indirectly referenced 
    /// assemblies. The assemblies are loaded via the <see cref="AssemblyLoader"/>.
    /// </summary>
    /// <returns>The root assemblies and their referenced assemblies.</returns>
    public virtual IEnumerable<Assembly> FindAssemblies ()
    {
      s_log.Value.Debug ("Finding assemblies...");
      using (StopwatchScope.CreateScope (s_log.Value, LogLevel.Info, "Time spent for finding and loading assemblies: {elapsed}."))
      {
        var rootAssemblies = FindRootAssemblies();
        var resultSet = new HashSet<Assembly> (rootAssemblies.Select (root => root.Assembly));

        resultSet.UnionWith (FindReferencedAssemblies (rootAssemblies));

        // Forcing the enumeration at this point does not have a measurable impact on performance.
        // Instead, decoupling the assembly loading from the rest of the system is actually helpful for concurrency.
        return resultSet.LogAndReturnItems (s_log.Value, LogLevel.Info, count => string.Format ("Found {0} assemblies.", count))
            .ToList();
      }
    }

    private ICollection<RootAssembly> FindRootAssemblies ()
    {
      s_log.Value.Debug ("Finding root assemblies...");
      using (StopwatchScope.CreateScope (s_log.Value, LogLevel.Debug, "Time spent for finding and loading root assemblies: {elapsed}."))
      {
        return _rootAssemblyFinder.FindRootAssemblies()
            .LogAndReturnItems (s_log.Value, LogLevel.Debug, count => string.Format ("Found {0} root assemblies.", count))
            .ToList();
      }
    }

    private IEnumerable<Assembly> FindReferencedAssemblies (IEnumerable<RootAssembly> rootAssemblies)
    {
      s_log.Value.Debug ("Finding referenced assemblies...");
      using (StopwatchScope.CreateScope (s_log.Value, LogLevel.Debug, "Time spent for finding and loading referenced assemblies: {elapsed}."))
      {
        var processedAssemblyNames = new HashSet<string>(); // used to avoid loading assemblies twice
        var referenceRoots = new HashSet<RootAssembly> (rootAssemblies); // referenced assemblies added later in order to get their references as well

        while (referenceRoots.Count > 0)
        {
          var currentRoot = referenceRoots.First(); // take any reference
          referenceRoots.Remove (currentRoot); // don't handle again

          if (currentRoot.FollowReferences)
          {
            foreach (var referencedAssemblyName in currentRoot.Assembly.GetReferencedAssemblies())
            {
              if (!processedAssemblyNames.Contains (referencedAssemblyName.FullName)) // don't process an assembly name twice
              {
                processedAssemblyNames.Add (referencedAssemblyName.FullName);

                var referencedAssembly = _assemblyLoader.TryLoadAssembly (referencedAssemblyName, currentRoot.Assembly.FullName);
                if (referencedAssembly != null) // might return null if filtered by the loader
                {
                  referenceRoots.Add (new RootAssembly (referencedAssembly, true)); // store as a root in order to process references transitively
                  yield return referencedAssembly;
                }
              }
            }
          }
        }
      }
    }
  }
}
