using System;
using Coypu;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
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

  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.InternetExplorer"/>. The default implementation of Coypu does not
  /// set all <see cref="InternetExplorerOptions"/> and does not enable driver-internal logging.
  /// </summary>
  public class CustomInternetExplorerDriver : SeleniumWebDriver
  {
    public CustomInternetExplorerDriver ()
        : base (CreateInternetExplorerDriver(), Browser.InternetExplorer)
    {
    }

    private static IWebDriver CreateInternetExplorerDriver ()
    {
      var driverService = InternetExplorerDriverService.CreateDefaultService();
      driverService.LogFile = "InternetExplorerDriverService.log"; // Todo: obtain log path from App.config?
      driverService.LoggingLevel = InternetExplorerDriverLogLevel.Info;

      return
          new InternetExplorerDriver (
              driverService,
              new InternetExplorerOptions
              {
                  EnableNativeEvents = true,
                  RequireWindowFocus = true,
                  EnablePersistentHover = false,
                  IntroduceInstabilityByIgnoringProtectedModeSettings = true
              });
    }
  }
}