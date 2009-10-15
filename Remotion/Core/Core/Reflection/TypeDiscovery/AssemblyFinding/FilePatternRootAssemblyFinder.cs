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
using System.IO;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Finds the root assemblies by looking up and loading files matching the given patterns in a specified directory.
  /// </summary>
  public class FilePatternRootAssemblyFinder : IRootAssemblyFinder
  {
    public struct Specification
    {
      public Specification (string filePattern, bool followReferences)
          : this()
      {
        ArgumentUtility.CheckNotNullOrEmpty ("filePattern", filePattern);

        FilePattern = filePattern;
        FollowReferences = followReferences;
      }

      public string FilePattern { get; private set; }
      public bool FollowReferences { get; private set; }
    }

    private readonly string _searchPath;
    private readonly IEnumerable<Specification> _specifications;
    private readonly IFileSearchService _fileSearchService;

    public FilePatternRootAssemblyFinder (string searchPath, IEnumerable<Specification> specifications, IFileSearchService fileSearchService)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("searchPath", searchPath);
      ArgumentUtility.CheckNotNull ("specifications", specifications);
      ArgumentUtility.CheckNotNull ("fileSearchService", fileSearchService);

      _searchPath = searchPath;
      _specifications = specifications;
      _fileSearchService = fileSearchService;
    }

    public string SearchPath
    {
      get { return _searchPath; }
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public IFileSearchService FileSearchService
    {
      get { return _fileSearchService; }
    }

    public RootAssembly[] FindRootAssemblies (IAssemblyLoader loader)
    {
      var rootAssemblies = from specification in _specifications
                           let files = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly)
                           from file in files
                           let assembly = loader.TryLoadAssembly (file)
                           where assembly != null
                           select new RootAssembly (assembly, specification.FollowReferences);
      return rootAssemblies.Distinct ().ToArray ();
    }
  }
}