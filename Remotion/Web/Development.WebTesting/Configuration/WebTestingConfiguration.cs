// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Coypu.Drivers;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategyImplementation;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Configures the web testing framework.
  /// </summary>
  [UsedImplicitly]
  public class WebTestingConfiguration : ConfigurationSection, IBrowserConfiguration, ICoypuConfiguration, IWebTestConfiguration
  {
    private static readonly Lazy<WebTestingConfiguration> s_current;
    private static readonly Dictionary<string, Type> s_wellKnownHostingStrategyTypes;

    static WebTestingConfiguration ()
    {
      s_current = new Lazy<WebTestingConfiguration> (
          () =>
          {
            var configuration = (WebTestingConfiguration) ConfigurationManager.GetSection ("remotion.webTesting");
            Assertion.IsNotNull (configuration, "Configuration section 'remotion.webTesting' missing.");
            return configuration;
          });

      s_wellKnownHostingStrategyTypes = new Dictionary<string, Type> { { "IisExpress", typeof (IisExpressHostingStrategy) } };
    }

    /// <summary>
    /// Returns the current <see cref="WebTestingConfiguration"/>.
    /// </summary>
    public static WebTestingConfiguration Current
    {
      get { return s_current.Value; }
    }

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
    /// Returns the process name of the configured browser.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    public string GetBrowserExecutableName ()
    {
      if (Browser == Browser.InternetExplorer)
        return "iexplore";
      if (Browser == Browser.Chrome)
        return "chrome";

      throw new NotSupportedException (string.Format ("Only browsers '{0}' and '{1}' are supported.", Browser.Chrome, Browser.InternetExplorer));
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
      get { return Path.GetFullPath ((string) this["screenshotDirectory"]); }
    }

    /// <summary>
    /// Absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons.
    /// </summary>
    [ConfigurationProperty ("logsDirectory", DefaultValue = ".")]
    public string LogsDirectory
    {
      get { return (string) this["logsDirectory"]; }
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
    /// Returns the configured hosting strategy or an instance of <see cref="NullHostingStrategy"/> if the user did not specify one.
    /// </summary>
    public IHostingStrategy GetHostingStrategy ()
    {
      if (string.IsNullOrEmpty (HostingProviderSettings.Type))
        return new NullHostingStrategy();

      var hostingStrategyTypeName = HostingProviderSettings.Type;
      var hostingStrategyType = GetHostingStrategyType (hostingStrategyTypeName);
      Assertion.IsNotNull (hostingStrategyType, string.Format ("Hosting strategy '{0}' could not be loaded.", hostingStrategyTypeName));

      var hostingStrategy = (IHostingStrategy) Activator.CreateInstance (hostingStrategyType, new object[] { HostingProviderSettings.Parameters });
      return hostingStrategy;
    }

    [ConfigurationProperty ("hosting", IsRequired = false)]
    private ProviderSettings HostingProviderSettings
    {
      get { return (ProviderSettings) this["hosting"]; }
    }

    private static Type GetHostingStrategyType (string hostingStrategyTypeName)
    {
      if (s_wellKnownHostingStrategyTypes.ContainsKey (hostingStrategyTypeName))
        return s_wellKnownHostingStrategyTypes[hostingStrategyTypeName];

      return Type.GetType (hostingStrategyTypeName);
    }
  }
}