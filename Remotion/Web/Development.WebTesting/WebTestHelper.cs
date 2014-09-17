using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests. On set up, it ensures that all browser windows are closed, initializes a new browser session and ensures that the
  /// cursor is outside the browser window. On tear down it automatically takes screenshots if the test failed and disposes the browser session.
  /// </summary>
  public class WebTestHelper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestHelper));

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
    }

    /// <summary>
    /// Access to the <see cref="WebTestConfiguration"/>.
    /// </summary>
    public WebTestConfiguration Configuration
    {
      get { return _webTestConfiguration; }
    }

    /// <summary>
    /// SetUp method for each web test.
    /// </summary>
    /// <param name="testName">Name of the test being performed.</param>
    public void OnSetUp ([NotNull] string testName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("testName", testName);

      _testName = testName;

      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      EnsureAllBrowserWindowsAreClosed();

      MainBrowserSession = CreateNewBrowserSession(_webTestConfiguration.WebApplicationRoot);

      // Note: otherwise cursor could interfere with element hovering.
      EnsureCursorIsOutsideBrowserWindow();
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
      if (!hasSucceeded)
      {
        // Todo: take screenshots.
      }

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