using System;
using System.Configuration;
using Remotion.Utilities;

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
      var webTestConfiguration = (WebTestConfiguration) ConfigurationManager.GetSection ("webTestConfiguration");
      Assertion.IsNotNull (webTestConfiguration, "Configuration section 'webTestConfiguration' missing.");

      _iisExpressHostingStrategy = new IisExpressHostingStrategy (webTestConfiguration.WebApplicationPath, webTestConfiguration.WebApplicationPort);
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