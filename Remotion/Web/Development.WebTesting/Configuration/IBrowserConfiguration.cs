using System;
using Coypu.Drivers;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides all the necessary information to initialize an shut down a browser session.
  /// </summary>
  public interface IBrowserConfiguration
  {
    /// <summary>
    /// Browser (as Coypu Browser object).
    /// </summary>
    Browser Browser { get; }

    /// <summary>
    /// Browser in which the web tests are run.
    /// </summary>
    string BrowserName { get; }

    /// <summary>
    /// Absolute or relative path to the logs directory. Some web driver implementations write log files for debugging reasons.
    /// </summary>
    string LogsDirectory { get; }

    /// <summary>
    /// Some Selenium web driver implementations may become confused when searching for windows if there are other browser windows present. Typically
    /// you want to turn this auto-close option on when running web tests, on developer machines, however, this may unexpectedly close important
    /// browser windows, which is why the default value is set to <see langword="false" />.
    /// </summary>
    bool CloseBrowserWindowsOnSetUpAndTearDown { get; }

    /// <summary>
    /// Returns the process name of the configured browser.
    /// </summary>
    /// <returns>The process name without the file extension.</returns>
    string GetBrowserExecutableName ();
  }
}