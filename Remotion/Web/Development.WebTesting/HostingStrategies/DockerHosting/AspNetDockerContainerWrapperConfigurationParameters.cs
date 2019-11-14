using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.HostingStrategies.DockerHosting
{
  public class AspNetDockerContainerWrapperConfigurationParameters
  {
    [NotNull]
    public string AbsoluteWebApplicationPath { get; }

    public int WebApplicationPort { get; }
    
    [NotNull]
    public string DockerImageName { get; }

    [CanBeNull]
    public string Hostname { get; }

    public bool Is32BitProcess { get; }

    public AspNetDockerContainerWrapperConfigurationParameters (
        [NotNull] string absoluteWebApplicationPath,
        int webApplicationPort,
        [NotNull] string dockerImageName,
        [CanBeNull] string hostname,
        bool is32BitProcess)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("absoluteWebApplicationPath", absoluteWebApplicationPath);
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);

      AbsoluteWebApplicationPath = absoluteWebApplicationPath;
      WebApplicationPort = webApplicationPort;
      DockerImageName = dockerImageName;
      Hostname = hostname;
      Is32BitProcess = is32BitProcess;
    }
  }
}
