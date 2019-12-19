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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Builds a DockerImage containing the web application under test and wraps Container Lifecycle.
  /// </summary>
  public class AspNetDockerContainerWrapper : IDisposable
  {
    private readonly IDockerHelper _dockerHelper;
    private readonly IDockerFilePreparer _dockerFilePreparer;
    private readonly AspNetDockerContainerWrapperConfigurationParameters _configurationParameters;
    private readonly string _localImageName;
    private readonly string _containerName;

    public AspNetDockerContainerWrapper (
        [NotNull] IDockerHelper dockerHelper,
        [NotNull] IDockerFilePreparer dockerFilePreparer,
        [NotNull] AspNetDockerContainerWrapperConfigurationParameters configurationParameters)
    {
      ArgumentUtility.CheckNotNull ("dockerHelper", dockerHelper);
      ArgumentUtility.CheckNotNull ("dockerFilePreparer", dockerFilePreparer);
      ArgumentUtility.CheckNotNull ("configurationParameters", configurationParameters);

      _dockerHelper = dockerHelper;
      _dockerFilePreparer = dockerFilePreparer;
      _configurationParameters = configurationParameters;

      _containerName = Guid.NewGuid().ToString();
      _localImageName = Guid.NewGuid() + ":latest";
    }

    public void BuildAndRun ()
    {
      BuildDockerImage();

      _dockerHelper.Run (
          true,
          true,
          new Dictionary<int, int>() { { _configurationParameters.WebApplicationPort, _configurationParameters.WebApplicationPort } },
          _containerName,
          _configurationParameters.Hostname,
          _localImageName);
    }

    public void Dispose ()
    {
      _dockerHelper.Stop (_containerName);
      _dockerHelper.RemoveImage (_localImageName);
    }

    private void BuildDockerImage ()
    {
      var buildArgs = new Dictionary<string, string>()
                      {
                          { "HostingBaseDockerImage", _configurationParameters.DockerImageName },
                          { "WebApplicationPort", _configurationParameters.WebApplicationPort.ToString() }
                      };

      if (_configurationParameters.Is32BitProcess)
        buildArgs.Add ("Is32BitProcess", "true");
      else
        buildArgs.Add ("Is32BitProcess", "false");

      _dockerHelper.Pull (_configurationParameters.DockerImageName);

      var dockerFile = _dockerFilePreparer.Prepare (_configurationParameters.AbsoluteWebApplicationPath);
      try
      {
        _dockerHelper.Build (
            _localImageName,
            buildArgs,
            dockerFile);
      }
      finally
      {
        dockerFile.Dispose();
      }
    }
  }
}