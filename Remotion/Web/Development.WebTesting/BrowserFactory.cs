using System;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Factory to create Coypu <see cref="BrowserSession"/> objects from a given <see cref="WebTestConfiguration"/>.
  /// </summary>
  public static class BrowserFactory
  {
    /// <summary>
    /// Creates a Coypu <see cref="BrowserSession"/> using the configuration given by <paramref name="configuration"/>.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <returns>A new Coypu <see cref="BrowserSession"/>.</returns>
    public static BrowserSession CreateBrowser ([NotNull] WebTestConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      var sessionConfiguration = new SessionConfiguration
                                 {
                                     Browser = configuration.Browser,
                                     RetryInterval = configuration.RetryInterval,
                                     Timeout = configuration.SearchTimeout,
                                     ConsiderInvisibleElements = true,
                                     Match = Match.First
                                 };

      if (sessionConfiguration.Browser == Browser.Chrome)
      {
        sessionConfiguration.Driver = typeof (SeleniumWebDriver);
        return new BrowserSession (sessionConfiguration);
      }

      if (sessionConfiguration.Browser == Browser.InternetExplorer)
      {
        sessionConfiguration.Driver = typeof (CustomInternetExplorerDriver);
        return new BrowserSession (sessionConfiguration, new CustomInternetExplorerDriver());
      }

      throw new NotSupportedException (string.Format ("Only browsers '{0}' and '{1}' are supported.", Browser.Chrome, Browser.InternetExplorer));
    }
  }
}