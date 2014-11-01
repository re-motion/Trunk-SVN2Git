using System;

namespace Remotion.Web.Development.WebTesting.HostingStrategyImplementation
{
  /// <summary>
  /// Does not host any web application. E.g. used for web tests on already deployed and running web applications.
  /// </summary>
  public class NullHostingStrategy : IHostingStrategy
  {
    public void DeployAndStartWebApplication ()
    {
    }

    public void StopAndUndeployWebApplication ()
    {
    }
  }
}