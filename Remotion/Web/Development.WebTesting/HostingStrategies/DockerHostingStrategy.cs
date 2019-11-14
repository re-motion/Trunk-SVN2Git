using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using log4net;
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
    private readonly TimeSpan _applicationPingTimeout;
    private static readonly ILog s_log = LogManager.GetLogger (typeof (DockerHostingStrategy));

    public DockerHostingStrategy ([NotNull] string webApplicationPath, int port, [NotNull] string dockerImageName, TimeSpan dockerCommandTimeout, TimeSpan applicationPingTimeout, [CanBeNull] string hostname)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);
      ArgumentUtility.CheckNotNullOrEmpty ("dockerImageName", dockerImageName);

      _applicationPingTimeout = applicationPingTimeout;

      var absoluteWebApplicationPath = Path.GetFullPath (webApplicationPath);
      var is32BitProcess = !Environment.Is64BitProcess;

      var dockerHelper = new DockerHelper (dockerCommandTimeout);
      var configurationParameters = new AspNetDockerContainerWrapperConfigurationParameters (absoluteWebApplicationPath, port, dockerImageName, hostname, is32BitProcess);
      var dockerFileManager = new DockerFileManager ();

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
                    TimeSpan.Parse (properties["applicationPingTimeout"]),
                    properties["hostname"])
    {
    }

    public void DeployAndStartWebApplication ()
    {
      _aspNetDockerContainerWrapper.BuildAndRun();
    }

    public void StopAndUndeployWebApplication ()
    {
      if (_aspNetDockerContainerWrapper != null)
        _aspNetDockerContainerWrapper.Dispose();
    }
  }
}
