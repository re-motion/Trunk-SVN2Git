using System;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Factory to create Coypu <see cref="BrowserSession"/> objects from a given <see cref="WebTestingConfiguration"/>.
  /// </summary>
  public static class BrowserFactory
  {
    /// <summary>
    /// Creates a Coypu <see cref="BrowserSession"/> using the configuration given by <paramref name="browserConfiguration"/>.
    /// </summary>
    /// <param name="browserConfiguration">The browser configuration to use.</param>
    /// /// <param name="coypuConfiguration">The Coypu configuration to use.</param>
    /// <returns>A new Coypu <see cref="BrowserSession"/>.</returns>
    public static BrowserSession CreateBrowser (
        [NotNull] IBrowserConfiguration browserConfiguration,
        [NotNull] ICoypuConfiguration coypuConfiguration)
    {
      ArgumentUtility.CheckNotNull ("browserConfiguration", browserConfiguration);
      ArgumentUtility.CheckNotNull ("coypuConfiguration", coypuConfiguration);

      var sessionConfiguration = new SessionConfiguration
                                 {
                                     Browser = browserConfiguration.Browser,
                                     RetryInterval = coypuConfiguration.RetryInterval,
                                     Timeout = coypuConfiguration.SearchTimeout,
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