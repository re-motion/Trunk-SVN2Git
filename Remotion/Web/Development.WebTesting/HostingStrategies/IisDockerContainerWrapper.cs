﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Wraps an IIS Docker container and its lifecycle.
  /// </summary>
  public class IisDockerContainerWrapper : IDisposable
  {
    private readonly IDockerClient _docker;
    private readonly IisDockerContainerWrapperConfigurationParameters _configurationParameters;
    private string _containerName;

    public IisDockerContainerWrapper (
        [NotNull] IDockerClient docker,
        [NotNull] IisDockerContainerWrapperConfigurationParameters configurationParameters)
    {
      ArgumentUtility.CheckNotNull ("docker", docker);
      ArgumentUtility.CheckNotNull ("configurationParameters", configurationParameters);

      _docker = docker;
      _configurationParameters = configurationParameters;
    }

    /// <summary>
    /// Pulls and starts the configured docker image as a container with the settings specified in the App.config.
    /// </summary>
    public void Run ()
    {
      _docker.Pull (_configurationParameters.DockerImageName);

      var mounts = GetMounts (_configurationParameters.Mounts);
      var portMappings = new Dictionary<int, int> { { _configurationParameters.WebApplicationPort, _configurationParameters.WebApplicationPort } };

      _containerName = _docker.Run (
          portMappings,
          mounts,
          _configurationParameters.DockerImageName,
          _configurationParameters.Hostname,
          true,
          true,
          "powershell",
          $@"-Command ""
C:\Windows\System32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32bitapponwin64:{_configurationParameters.Is32BitProcess};
C:\Windows\System32\inetsrv\appcmd.exe add site /name:WebTestingSite /physicalPath:""{_configurationParameters.AbsoluteWebApplicationPath}"" /bindings:""http://*:{_configurationParameters.WebApplicationPort}"";
C:\ServiceMonitor.exe w3svc;
""");
    }

    /// <inheritdoc />
    public void Dispose ()
    {
      _docker.Stop (_containerName);
      var containerRemovedAfterStop = WaitForContainerRemoval();

      if (containerRemovedAfterStop)
        return;

      _docker.Remove (_containerName, true);
      var containerRemovedAfterForceRemove = WaitForContainerRemoval();

      if (containerRemovedAfterForceRemove)
        return;

      throw new InvalidOperationException ($"The container with the id '{_containerName}' could not be removed.");
    }

    /// <summary>
    /// Polls the container for existence.
    /// </summary>
    /// <returns>True if the container no longer exists, false otherwise.</returns>
    private bool WaitForContainerRemoval ()
    {
      for (var i = 0; i < 15; i++)
      {
        if (!_docker.ContainerExists (_containerName))
          return true;

        Thread.Sleep (100);
      }

      return false;
    }

    private Dictionary<string, string> GetMounts (IEnumerable<string> additionalMounts)
    {
      var mounts = new Dictionary<string, string>
                   {
                       {
                           _configurationParameters.AbsoluteWebApplicationPath,
                           _configurationParameters.AbsoluteWebApplicationPath
                       }
                   };

      foreach (var mount in additionalMounts)
      {
        var absoluteMountPath = Path.Combine (_configurationParameters.AbsoluteWebApplicationPath, mount);
        mounts.Add (absoluteMountPath, absoluteMountPath);
      }

      return mounts;
    }
  }
}