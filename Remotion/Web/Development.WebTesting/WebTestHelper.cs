using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
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
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WebTestHelper));

    /// <summary>
    /// Browser configuration.
    /// </summary>
    private readonly IBrowserConfiguration _browserConfiguration;

    /// <summary>
    /// Coypu configuration.
    /// </summary>
    private readonly ICoypuConfiguration _coypuConfiguration;

    /// <summary>
    /// Web test configuration.
    /// </summary>
    private readonly IWebTestConfiguration _webTestConfiguration;

    /// <summary>
    /// Name of the current web test.
    /// </summary>
    private string _testName;

    /// <summary>
    /// Coypu main browser session for the web test.
    /// </summary>
    public BrowserSession MainBrowserSession { get; private set; }

    public WebTestHelper (
        [NotNull] IBrowserConfiguration browserConfiguration,
        [NotNull] ICoypuConfiguration coypuConfiguration,
        [NotNull] IWebTestConfiguration webTestConfiguration)
    {
      ArgumentUtility.CheckNotNull ("browserConfiguration", browserConfiguration);
      ArgumentUtility.CheckNotNull ("coypuConfiguration", coypuConfiguration);
      ArgumentUtility.CheckNotNull ("webTestConfiguration", webTestConfiguration);

      _browserConfiguration = browserConfiguration;
      _coypuConfiguration = coypuConfiguration;
      _webTestConfiguration = webTestConfiguration;
    }

    /// <summary>
    /// Creates a new <see cref="WebTestHelper"/> from <see cref="WebTestingFrameworkConfiguration.Current"/>.
    /// </summary>
    public static WebTestHelper CreateFromConfiguration ()
    {
      return new WebTestHelper (
          WebTestingFrameworkConfiguration.Current,
          WebTestingFrameworkConfiguration.Current,
          WebTestingFrameworkConfiguration.Current);
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
      s_log.InfoFormat ("Executing test: {0}.", _testName);
    }

    /// <summary>
    /// Creates a new browser session using the configured settings from the App.config file.
    /// </summary>
    /// <param name="url">The URL to visit.</param>
    /// <param name="maximiseWindow">Specified whether the new browser session's window should be maximized.</param>
    /// <returns>The new browser session.</returns>
    public BrowserSession CreateNewBrowserSession (string url, bool maximiseWindow = true)
    {
      using (new PerformanceTimer (s_log, string.Format ("Created new {0} browser session.", _browserConfiguration.BrowserName)))
      {
        var browser = BrowserFactory.CreateBrowser (_browserConfiguration, _coypuConfiguration);
        if (maximiseWindow)
          browser.MaximiseWindow();
        browser.Visit (url);
        return browser;
      }
    }

    /// <summary>
    /// Returns a new <see cref="TestObjectContext"/> for the <see cref="MainBrowserSession"/>.
    /// </summary>
    public TestObjectContext CreateNewTestObjectContext ()
    {
      return TestObjectContext.New (MainBrowserSession);
    }

    /// <summary>
    /// Accepts a, possibly existent, modal dialog.
    /// </summary>
    public void AcceptPossibleModalDialog ()
    {
      try
      {
        var context = CreateNewTestObjectContext();
        MainBrowserSession.AcceptModalDialogImmediatelyFixed ();
      }
      catch (NoAlertPresentException)
      {
        // It's okay.
      }
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

        var baseFileName = SanitizeFileName (_testName);

        var desktopScreenshotFileName = string.Format ("{0}_Desktop", baseFileName);
        screenshotCapturer.TakeDesktopScreenshot (desktopScreenshotFileName);

        var browserScreenshotFileName = string.Format ("{0}_Browser", baseFileName);
        screenshotCapturer.TakeBrowserScreenshot (browserScreenshotFileName, MainBrowserSession);
      }

      s_log.InfoFormat ("Finished test: {0} [has succeeded: {1}].", _testName, hasSucceeded);
    }

    // Todo RM-6297: SRP: move somewhere...
    private string SanitizeFileName (string fileName)
    {
      foreach (var c in Path.GetInvalidFileNameChars())
        fileName = fileName.Replace (c, '_');

      return fileName;
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
      if (!_browserConfiguration.CloseBrowserWindowsOnSetUpAndTearDown)
        return;

      var browserProcessName = _browserConfiguration.GetBrowserExecutableName();
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