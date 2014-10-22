using System;
using System.Configuration;
using Coypu.Drivers;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Configures the web testing framework.
  /// </summary>
  [UsedImplicitly]
  public class WebTestConfiguration : ConfigurationSection
  {
    /// <summary>
    /// Returns the <see cref="WebTestConfiguration"/> loaded by <see cref="WebTestHelper"/>.
    /// </summary>
    public static WebTestConfiguration Current { get; internal set; }

    /// <summary>
    /// Browser in which the web tests are run.
    /// </summary>
    [ConfigurationProperty ("browser", IsRequired = true)]
    public string BrowserName
    {
      get { return (string) this["browser"]; }
    }

    /// <summary>
    /// Browser (as Coypu Browser object).
    /// </summary>
    public Browser Browser
    {
      get { return Browser.Parse (BrowserName); }
    }

    /// <summary>
    /// Returns whether the <see cref="Browser"/> is set to <see cref="Coypu.Drivers.Browser.InternetExplorer"/>.
    /// </summary>
    public bool BrowserIsInternetExplorer ()
    {
      return Browser == Browser.InternetExplorer;
    }

    /// <summary>
    /// Specifies how long the Coypu engine should maximally search for a web element or try to interact with a web element before it fails.
    /// </summary>
    [ConfigurationProperty ("searchTimeout", IsRequired = true)]
    public TimeSpan SearchTimeout
    {
      get { return (TimeSpan) this["searchTimeout"]; }
    }

    /// <summary>
    /// Whenever the element to be interacted with is not ready, visible or otherwise not present, the Coypu engine automatically retries the action
    /// after the given <see cref="RetryInterval"/> until the <see cref="SearchTimeout"/> has been reached.
    /// </summary>
    [ConfigurationProperty ("retryInterval", IsRequired = true)]
    public TimeSpan RetryInterval
    {
      get { return (TimeSpan) this["retryInterval"]; }
    }

    /// <summary>
    /// Absolute file path to the web application under test. The framework will automatically host an IIS Express instance for this web application.
    /// </summary>
    [ConfigurationProperty ("webApplicationPath", IsRequired = false)]
    public string WebApplicationPath
    {
      get { return (string) this["webApplicationPath"]; }
    }

    /// <summary>
    /// Port to be used for hosting the web application given by <see cref="WebApplicationPath"/>.
    /// </summary>
    [ConfigurationProperty ("webApplicationPort", IsRequired = false, DefaultValue = 60401)]
    [IntegerValidator(MinValue = 49152, MaxValue = 65534)]
    public int WebApplicationPort
    {
      get { return (int) this["webApplicationPort"]; }
    }

    /// <summary>
    /// URL to which the web application under test has been published.
    /// </summary>
    [ConfigurationProperty ("webApplicationRoot", IsRequired = true)]
    public string WebApplicationRoot
    {
      get { return (string) this["webApplicationRoot"]; }
    }

    /// <summary>
    /// Absolute or relative path to the screenshot directory. The web testing framework automatically takes two screenshots (one of the whole desktop
    /// and one of the browser window) in case a web test failed.
    /// </summary>
    [ConfigurationProperty ("screenshotDirectory", IsRequired = false)]
    public string ScreenshotDirectory
    {
      get { return (string) this["screenshotDirectory"]; }
    }

    /// <summary>
    /// Some Selenium web driver implementations may become confused when searching for windows if there are other browser windows present. Typically
    /// you want to turn this auto-close option on when running web tests, on developer machines, however, this may unexpectedly close important
    /// browser windows, which is why the default value is set to <see langword="false" />.
    /// </summary>
    [ConfigurationProperty ("closeBrowserWindowsOnSetUpAndTearDown", DefaultValue = false)]
    public bool CloseBrowserWindowsOnSetUpAndTearDown
    {
      get { return (bool) this["closeBrowserWindowsOnSetUpAndTearDown"]; }
    }

    /// <summary>
    /// Returns the process name of the configured browser.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    public string GetBrowserExecutableName ()
    {
      if (Browser == Browser.InternetExplorer)
        return "iexplore";
      if (Browser == Browser.Chrome)
        return "chrome";

      throw new NotSupportedException ("Only Chrome and InternetExplorer are supported browsers.");
    }
  }
}