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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Remotion.Web.Configuration;

namespace OBWTest
{
  public class ResourcePathMapping
  {
    private readonly FileSystemInfo _physicalPath;
    private readonly string _virtualPath;

    public ResourcePathMapping (string virtualPath, FileSystemInfo physicalPath)
    {
      _virtualPath = virtualPath;
      _physicalPath = physicalPath;
    }

    public FileSystemInfo PhysicalPath
    {
      get { return _physicalPath; }
    }

    public string VirtualPath
    {
      get { return _virtualPath; }
    }
  }

  public class ResourceVirtualFile : VirtualFile
  {
    private readonly FileInfo _physicalFile;

    public ResourceVirtualFile (string virtualPath, FileInfo physicalFile)
        : base (virtualPath)
    {
      _physicalFile = physicalFile;
    }


    public bool Exists
    {
      get { return _physicalFile != null && _physicalFile.Exists; }
    }

    public override Stream Open ()
    {
      return _physicalFile.OpenRead();
    }
  }

  public class ResourceVirtualDirectory : VirtualDirectory
  {
    public ResourceVirtualDirectory (string virtualPath)
        : base (virtualPath)
    {
    }

    public override IEnumerable Directories
    {
      get { return Enumerable.Empty<VirtualDirectory>(); }
    }

    public override IEnumerable Files
    {
      get { return Enumerable.Empty<VirtualFile>(); }
    }

    public override IEnumerable Children
    {
      get { return Enumerable.Empty<VirtualFileBase>(); }
    }
  }

  public class ResourceVirtualPathProvider : VirtualPathProvider
  {
    private ResourcePathMapping[] _mappings;
    private readonly string _resourceRoot;

    public ResourceVirtualPathProvider ()
    {
      _resourceRoot = VirtualPathUtility.AppendTrailingSlash (VirtualPathUtility.Combine ("~/", WebConfiguration.Current.Resources.Root));
      var offset = @"..\..\..\";
      var projectRoot = HttpContext.Current.Server.MapPath ("~/");
      var solutionRoot = Path.Combine (projectRoot, offset);
      _mappings =
          new[]
          {
              new ResourcePathMapping (
                  VirtualPathUtility.AppendTrailingSlash (VirtualPathUtility.Combine (_resourceRoot, "Remotion.Web")),
                  new DirectoryInfo (Path.Combine (solutionRoot, @"Remotion\Web\Core\res"))),
              new ResourcePathMapping (
                  VirtualPathUtility.AppendTrailingSlash (VirtualPathUtility.Combine (_resourceRoot, "Remotion.ObjectBinding.Web")),
                  new DirectoryInfo (Path.Combine (solutionRoot, @"Remotion\ObjectBinding\Web\res")))
          };
    }

    private bool IsResourcePath (string virtualPath)
    {
      String checkPath = VirtualPathUtility.ToAppRelative (virtualPath);
      return checkPath.StartsWith (_resourceRoot, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool FileExists (string virtualPath)
    {
      if (IsResourcePath (virtualPath))
      {
        var file = GetResourceVirtualFile (virtualPath);
        return file.Exists;
      }
      else
        return Previous.FileExists (virtualPath);
    }

    public override bool DirectoryExists (string virtualDir)
    {
      if (IsResourcePath (virtualDir))
        return false;
      else
        return Previous.DirectoryExists (virtualDir);
    }

    public override VirtualFile GetFile (string virtualPath)
    {
      if (IsResourcePath (virtualPath))
        return GetResourceVirtualFile (virtualPath);
      else
        return Previous.GetFile (virtualPath);
    }

    public override VirtualDirectory GetDirectory (string virtualDir)
    {
      if (IsResourcePath (virtualDir))
        return GetResourceVirtualDirectory (virtualDir);
      else
        return Previous.GetDirectory (virtualDir);
    }

    public override CacheDependency GetCacheDependency (string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
    {
      if (IsResourcePath (virtualPath))
        return null;
      else
        return Previous.GetCacheDependency (virtualPath, virtualPathDependencies, utcStart);
    }

    private ResourceVirtualFile GetResourceVirtualFile (string virtualPath)
    {
      var appRelativeVirtualPath = VirtualPathUtility.ToAppRelative (virtualPath);
      var mapping = _mappings.SingleOrDefault (m => appRelativeVirtualPath.StartsWith (m.VirtualPath, StringComparison.InvariantCultureIgnoreCase));
      FileInfo physicalFile = null;
      if (mapping != null)
      {
        var relativeVirtualPath = VirtualPathUtility.MakeRelative (mapping.VirtualPath, appRelativeVirtualPath);
        var relativePhysicalPath = relativeVirtualPath.Replace ('/', '\\');
        physicalFile = new FileInfo (Path.Combine (mapping.PhysicalPath.FullName, relativePhysicalPath));
      }
      return new ResourceVirtualFile (virtualPath, physicalFile);
    }

    private ResourceVirtualDirectory GetResourceVirtualDirectory (string virtualDir)
    {
      return new ResourceVirtualDirectory (virtualDir);
    }
  }
}