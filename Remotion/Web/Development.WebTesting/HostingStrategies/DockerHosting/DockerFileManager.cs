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
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class DockerFileManager : IDockerFileManager
  {
    private const string dockerfileName = "dockerfile";

    public IDockerFile Prepare (string absoluteWebApplicationPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("absoluteWebApplicationPath", absoluteWebApplicationPath);

      var dockerfileLocationFullPath = Path.Combine (absoluteWebApplicationPath, dockerfileName);

      if (File.Exists (dockerfileLocationFullPath))
        return new CustomDockerFile (dockerfileLocationFullPath);

      SaveDockerfileFromManifestResource (dockerfileLocationFullPath);

      return new PreparedDockerFile (dockerfileLocationFullPath);
    }

    private void SaveDockerfileFromManifestResource (string dockerfileLocationFullPath)
    {
      var aspNetDockerContainerWrapperType = typeof (AspNetDockerContainerWrapper);
      var assembly = aspNetDockerContainerWrapperType.Assembly;
      var resourceName = $"{aspNetDockerContainerWrapperType.Namespace}.{dockerfileName}";

      SaveManifestToFile (assembly, resourceName, dockerfileLocationFullPath);
    }

    private void SaveManifestToFile (Assembly assembly, string resourceName, string destinationPath)
    {
      using (var manifestResourceStream = assembly.GetManifestResourceStream (resourceName))
      using (var fileStream = File.Create (destinationPath))
      {
        Assertion.IsNotNull (manifestResourceStream, "'{0}' not found as embedded resource in '{1}'.", resourceName, assembly);

        manifestResourceStream.CopyTo (fileStream);
      }
    }
  }
}