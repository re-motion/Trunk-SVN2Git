using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests. On set up, it ensures that all browser windows are closed, initializes a new browser session and ensures that the
  /// cursor is outside the browser window. On tear down it automatically takes screenshots if the test failed and disposes the browser session.
  /// </summary>
  public class WebTestHelper
  {
    /// <summary>
    /// Web test configuration.
    /// </summary>
    private readonly WebTestConfiguration _webTestConfiguration;

    /// <summary>
    /// Name of the current web test.
    /// </summary>
    private string _testName;

    /// <summary>
    /// Coypu main browser session for the web test.
    /// </summary>
    public BrowserSession MainBrowserSession { get; private set; }

    /// <summary>
    /// Initializes the helper.
    /// </summary>
    public WebTestHelper ()
    {
      _webTestConfiguration = (WebTestConfiguration) ConfigurationManager.GetSection ("webTestConfiguration");
      Assertion.IsNotNull (_webTestConfiguration, "Configuration section 'webTestConfiguration' missing.");
    }

    /// <summary>
    /// Access to the <see cref="WebTestConfiguration"/>.
    /// </summary>
    public WebTestConfiguration Configuration
    {
      get { return _webTestConfiguration; }
    }

    /// <summary>
    /// SetUp method for each web test fixture.
    /// </summary>
    public void OnFixtureSetUp ()
    {
      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      EnsureAllBrowserWindowsAreClosed();

      MainBrowserSession = CreateNewBrowserSession (_webTestConfiguration.WebApplicationRoot);

      // Note: otherwise cursor could interfere with element hovering.
      EnsureCursorIsOutsideBrowserWindow();
    }

    /// <summary>
    /// SetUp method for each web test.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    public void OnSetUp ([NotNull] string testName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("testName", testName);

      _testName = testName;
    }

    /// <summary>
    /// Creates a new browser session using the configured settings from the App.config file.
    /// </summary>
    /// <param name="url">The URL to visit.</param>
    /// <param name="maximiseWindow">Specified whether the new browser session's window should be maximized.</param>
    /// <returns>The new browser session.</returns>
    public BrowserSession CreateNewBrowserSession (string url, bool maximiseWindow = true)
    {
      var browser = BrowserFactory.CreateBrowser (_webTestConfiguration);
      if (maximiseWindow)
        browser.MaximiseWindow();
      browser.Visit (url);
      return browser;
    }

    /// <summary>
    /// TearDown method for each web test.
    /// </summary>
    /// <param name="hasSucceeded">Specifies whether the test has been successful.</param>
    public void OnTearDown (bool hasSucceeded)
    {
      if (!hasSucceeded && ShouldTakeScreenshots())
      {
        var screenshotCapturer = new ScreenshotCapturer (_webTestConfiguration.ScreenshotDirectory);

        var desktopScreenshotFileName = string.Format ("{0}_Desktop", _testName);
        screenshotCapturer.TakeDesktopScreenshot (desktopScreenshotFileName);

        var browserScreenshotFileName = string.Format ("{0}_Browser", _testName);
        screenshotCapturer.TakeBrowserScreenshot (browserScreenshotFileName, MainBrowserSession);
      }
    }

    private bool ShouldTakeScreenshots ()
    {
      return !string.IsNullOrEmpty (_webTestConfiguration.ScreenshotDirectory);
    }

    /// <summary>
    /// TearDown method for each web test fixture.
    /// </summary>
    public void OnFixtureTearDown ()
    {
      if (MainBrowserSession != null)
        MainBrowserSession.Dispose();

      // Note: otherwise the sytem may get clogged, if the Selenium web driver implementation does not properly close all windows in all situations.
      EnsureAllBrowserWindowsAreClosed();
    }

    private void EnsureAllBrowserWindowsAreClosed ()
    {
      if (!_webTestConfiguration.CloseBrowserWindowsOnSetUpAndTearDown)
        return;

      var browserProcessName = _webTestConfiguration.GetBrowserExecutableName();
      if (browserProcessName == null)
        return;

      foreach (var process in Process.GetProcessesByName (browserProcessName))
      {
        try
        {
          process.Kill();
        }
            // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
          // Ignore, process is already closing or we do not have the required privileges anyway.
        }
      }
    }

    private void EnsureCursorIsOutsideBrowserWindow ()
    {
      Cursor.Position = new Point (0, 0);
    }
  }
}