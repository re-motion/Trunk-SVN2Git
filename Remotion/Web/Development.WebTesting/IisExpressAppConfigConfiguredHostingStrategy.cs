using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// See <see cref="IisExpressHostingStrategy"/>, the web application path and the port are automatically obtained from the App.config file, using
  /// the <see cref="WebTestConfiguration"/> configuration section.
  /// </summary>
  public class IisExpressAppConfigConfiguredHostingStrategy : IHostingStrategy
  {
    private readonly IisExpressHostingStrategy _iisExpressHostingStrategy;

    public IisExpressAppConfigConfiguredHostingStrategy ()
    {
      _iisExpressHostingStrategy = new IisExpressHostingStrategy (
          WebTestConfiguration.Current.WebApplicationPath,
          WebTestConfiguration.Current.WebApplicationPort);
    }

    public void DeployAndStartWebApplication ()
    {
      _iisExpressHostingStrategy.DeployAndStartWebApplication();
    }

    public void StopAndUndeployWebApplication ()
    {
      _iisExpressHostingStrategy.StopAndUndeployWebApplication();
    }
  }
}