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
using System.Collections.Specialized;
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Hosts the WebApplication using a Docker Container.
  /// </summary>
  public class DockerHostingStrategy : IHostingStrategy
  {
    private readonly AspNetDockerContainerWrapper _aspNetDockerContainerWrapper;

    public DockerHostingStrategy ([NotNull] string webApplicationPath, int port, [NotNull] string dockerImageName, TimeSpan dockerCommandTimeout, [CanBeNull] string hostname)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);

      var absoluteWebApplicationPath = Path.GetFullPath (webApplicationPath);
      var is32BitProcess = !Environment.Is64BitProcess;

      var dockerHelper = new DockerHelper (dockerCommandTimeout);
      var configurationParameters = new AspNetDockerContainerWrapperConfigurationParameters (absoluteWebApplicationPath, port, dockerImageName, hostname, is32BitProcess);
      var dockerFileManager = new DockerFileManager();

      _aspNetDockerContainerWrapper = new AspNetDockerContainerWrapper (dockerHelper, dockerFileManager, configurationParameters);
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public DockerHostingStrategy ([NotNull] NameValueCollection properties)
        : this (
            ArgumentUtility.CheckNotNull ("properties", properties)["path"],
            int.Parse (properties["port"]),
            properties["dockerImageName"],
            TimeSpan.Parse (properties["dockerCommandTimeout"]),
            properties["hostname"])
    {
    }

    public void DeployAndStartWebApplication ()
    {
      _aspNetDockerContainerWrapper.BuildAndRun();
    }

    public void StopAndUndeployWebApplication ()
    {
      _aspNetDockerContainerWrapper?.Dispose();
        _aspNetDockerContainerWrapper.Dispose();
    }
  }
}
