using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web tests which provides various convinience methods:
  /// <list type="table">
  ///   <listheader>
  ///     <term>Step</term>
  ///     <description>Actions</description>
  ///   </listheader>
  ///   <item>
  ///     <term>FixtureSetUp</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Ensures that all browser windows are closed.</item>
  ///         <item>Initializes a new browser session.</item>
  ///         <item>Visits the configured application root.</item>
  ///         <item>Ensures that the cursor is outside the browser window.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>SetUp</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Log output.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>TearDown</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Takes screenshots if the test failed.</item>
  ///         <item>Log output.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>FixtureTearDown</term>
  ///     <description>
  ///       <list type="bullet">
  ///         <item>Disposes the browser session.</item>
  ///         <item>Ensures that all browser windows are closed.</item>
  ///       </list>
  ///     </description>
  ///   </item>
  /// </list>
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
    /// Creates a new <see cref="WebTestHelper"/> from <see cref="WebTestingConfiguration.Current"/>.
    /// </summary>
    public static WebTestHelper CreateFromConfiguration ()
    {
      return new WebTestHelper (WebTestingConfiguration.Current, WebTestingConfiguration.Current, WebTestingConfiguration.Current);
    }

    /// <summary>
    /// SetUp method for each web test fixture.
    /// </summary>
    public void OnFixtureSetUp ()
    {
      // Note: otherwise the Selenium web driver may get confused when searching for windows.
      EnsureAllBrowserWindowsAreClosed();

      MainBrowserSession = CreateNewBrowserSession ();

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
    /// <param name="maximiseWindow">Specified whether the new browser session's window should be maximized.</param>
    /// <returns>The new browser session.</returns>
    public BrowserSession CreateNewBrowserSession (bool maximiseWindow = true)
    {
      using (new PerformanceTimer (s_log, string.Format ("Created new {0} browser session.", _browserConfiguration.BrowserName)))
      {
        var browser = BrowserFactory.CreateBrowser (_browserConfiguration, _coypuConfiguration);
        if (maximiseWindow)
          browser.MaximiseWindow();
        return browser;
      }
    }

    /// <summary>
    /// Returns a new <typeparamref name="TPageObject"/> for the initial page displayed by <see cref="MainBrowserSession"/>.
    /// </summary>
    public TPageObject CreateInitialPageObject<TPageObject> ()
    {
      var context = PageObjectContext.New (MainBrowserSession);
      return (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { context });
    }

    /// <summary>
    /// Accepts a, possibly existent, modal dialog.
    /// </summary>
    public void AcceptPossibleModalDialog ()
    {
      try
      {
        MainBrowserSession.AcceptModalDialogImmediatelyFixed();
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
        screenshotCapturer.TakeDesktopScreenshot (_testName);
        screenshotCapturer.TakeBrowserScreenshot (_testName, MainBrowserSession);
      }

      s_log.InfoFormat ("Finished test: {0} [has succeeded: {1}].", _testName, hasSucceeded);
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