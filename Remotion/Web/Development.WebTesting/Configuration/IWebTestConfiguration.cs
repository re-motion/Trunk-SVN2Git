using System;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides all the necessary information to run a web test.
  /// </summary>
  public interface IWebTestConfiguration
  {
    /// <summary>
    /// URL to which the web application under test has been published.
    /// </summary>
    string WebApplicationRoot { get; }

    /// <summary>
    /// Absolute or relative path to the screenshot directory. The web testing framework automatically takes two screenshots (one of the whole desktop
    /// and one of the browser window) in case a web test failed.
    /// </summary>
    string ScreenshotDirectory { get; }
  }
}