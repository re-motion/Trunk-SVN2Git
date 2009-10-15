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
    /// <summary>
    /// Holds a file path string as well as a flag indicating whether referenced assemblies should be followed or not. Equality comparisons of
    /// instances only check the file name, not the flag - this simplifies the algorithm to exclude file names in 
    /// <see cref="FilePatternRootAssemblyFinder.ConsolidateSpecifications"/>.
    /// </summary>
    private struct FileDescription
    {
      public FileDescription (string file, bool followReferences)
          : this()
      {
        FilePath = file;
        FollowReferences = followReferences;
      }

      public string FilePath { get; private set; }
      public bool FollowReferences { get; private set; }

      public override bool Equals (object obj)
      {
        return obj is FileDescription && Equals (FilePath, ((FileDescription) obj).FilePath);
      }

      public override int GetHashCode ()
      {
        return FilePath.GetHashCode ();
      }
    }

    private readonly string _searchPath;
    private readonly IEnumerable<FilePatternSpecification> _specifications;
    private readonly IFileSearchService _fileSearchService;

    public FilePatternRootAssemblyFinder (string searchPath, IEnumerable<FilePatternSpecification> specifications, IFileSearchService fileSearchService)
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

    public IEnumerable<FilePatternSpecification> Specifications
    {
      get { return _specifications; }
    }

    public IFileSearchService FileSearchService
    {
      get { return _fileSearchService; }
    }

    public RootAssembly[] FindRootAssemblies (IAssemblyLoader loader)
    {
      var fileDescriptions = ConsolidateSpecifications ();

      var rootAssemblies = from fileDescription in fileDescriptions
                           let assembly = loader.TryLoadAssembly (fileDescription.FilePath)
                           where assembly != null
                           select new RootAssembly (assembly, fileDescription.FollowReferences);
      return rootAssemblies.Distinct ().ToArray ();
    }

    private IEnumerable<FileDescription> ConsolidateSpecifications ()
    {
      var fileDescriptions = new HashSet<FileDescription> ();

      foreach (var specification in _specifications)
      {
        switch (specification.Kind)
        {
          case FilePatternSpecificationKind.IncludeNoFollow:
            var filesNotToFollow = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.UnionWith (filesNotToFollow.Select (f => new FileDescription (f, false)));
            break;
          case FilePatternSpecificationKind.IncludeFollowReferences:
            var filesToFollow = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.UnionWith (filesToFollow.Select (f => new FileDescription (f, true)));
            break;
          default:
            var filesToExclude = _fileSearchService.GetFiles (_searchPath, specification.FilePattern, SearchOption.TopDirectoryOnly);
            fileDescriptions.ExceptWith (filesToExclude.Select (f => new FileDescription (f, true))); // the "true" flag is ignored on comparisons
            break;
        }
      }

      return fileDescriptions;
    }
  }
}