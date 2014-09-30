using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// A hosting strategy determines which web server to start and how to deploy/undeploy the web application under test.
  /// </summary>
  public interface IHostingStrategy
  {
    /// <summary>
    /// Deploys the web application and starts the corresponding server.
    /// </summary>
    void DeployAndStartWebApplication ();

    /// <summary>
    /// Stops the corresponding server and undeploys the web application.
    /// </summary>
    void StopAndUndeployWebApplication ();
  }
}