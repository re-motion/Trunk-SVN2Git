using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Builds a DockerImage containing the web application under test and wraps Container Lifecycle.
  /// </summary>
  public class AspNetDockerContainerWrapper : IDisposable
  {
    private readonly IDockerHelper _dockerHelper;
    private readonly IDockerFileManager _dockerFileManager;
    private readonly AspNetDockerContainerWrapperConfigurationParameters _configurationParameters;
    private readonly string _localImageName;
    private readonly string _containerName;

    public AspNetDockerContainerWrapper ([NotNull] IDockerHelper dockerHelper, [NotNull] IDockerFileManager dockerFileManager, [NotNull] AspNetDockerContainerWrapperConfigurationParameters configurationParameters)
    {
      ArgumentUtility.CheckNotNull ("dockerHelper", dockerHelper);
      ArgumentUtility.CheckNotNull ("dockerFileManager", dockerFileManager);
      ArgumentUtility.CheckNotNull ("configurationParameters", configurationParameters);

      _dockerHelper = dockerHelper;
      _dockerFileManager = dockerFileManager;
      _configurationParameters = configurationParameters;

      _containerName = Guid.NewGuid().ToString();
      _localImageName = Guid.NewGuid() + ":latest";
    }

    public void BuildAndRun ()
    {
      BuildDockerImage ();

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

      var dockerFile = _dockerFileManager.Prepare (_configurationParameters.AbsoluteWebApplicationPath);
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