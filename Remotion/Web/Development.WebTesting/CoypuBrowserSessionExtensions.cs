using System;
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="BrowserSession"/> class.
  /// </summary>
  public static class CoypuBrowserSessionExtensions
  {
    /// <summary>
    /// Some WebDriver implementations hang, when trying to query within an <see cref="ElementScope"/> which is not on the "active" window.
    /// </summary>
    public static void EnsureWindowIsActive (this BrowserWindow window)
    {
      // ReSharper disable once UnusedVariable
      var temp = window.Title;
    }

    public static ElementScope GetRootScope (this BrowserWindow window)
    {
      return window.FindCss ("html");
    }

    /// <summary>
    /// IE-compatible version for Selenium's <see cref="BrowserWindow.AcceptModalDialog"/> method.
    /// </summary>
    /// <param name="browser">The <see cref="BrowserSession"/> on which the action is performed.</param>
    public static void AcceptModalDialogFixed ([NotNull] this BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        browser.AcceptModalDialog();
      else
        browser.AcceptModalDialogFixedInternetExplorer ();
    }

    /// <summary>
    /// See <see cref="AcceptModalDialogFixed"/>, however, the <see cref="WebTestingConfiguration.SearchTimeout"/> and
    /// <see cref="WebTestingConfiguration.RetryInterval"/> do not apply.
    /// </summary>
    public static void AcceptModalDialogImmediatelyFixed([NotNull] this BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        browser.AcceptModalDialog (Options.NoWait);
      else
        browser.AcceptModalDialogImmediatelyFixedInternetExplorer();
    }

    /// <summary>
    /// IE-compatible version for Selenium's <see cref="BrowserWindow.CancelModalDialog"/> method.
    /// </summary>
    /// <param name="browser">The <see cref="BrowserSession"/> on which the action is performed.</param>
    public static void CancelModalDialogFixed ([NotNull] this BrowserSession browser)
    {
      // Todo RM-6297: get rid of code duplication in Accept/CancelModalDialogFixed and Accept/CancelModalDialogFixedInternetExplorer.

      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        browser.CancelModalDialog();
      else
        browser.CancelModalDialogFixedInternetExplorer ();
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) runs into a race condition when accepting modal browser
    /// dialogs.
    /// </summary>
    private static void AcceptModalDialogFixedInternetExplorer ([NotNull] this BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      RetryUntilTimeout.Run (browser.AcceptModalDialogImmediatelyFixedInternetExplorer);
    }

    private static void AcceptModalDialogImmediatelyFixedInternetExplorer([NotNull] this BrowserSession browser)
    {
      var webDriver = (IWebDriver) browser.Native;
      webDriver.SwitchTo().Alert().Accept();
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) runs into a race condition when canceling modal browser
    /// dialogs.
    /// </summary>
    private static void CancelModalDialogFixedInternetExplorer ([NotNull] this BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      RetryUntilTimeout.Run (browser.CancelModalDialogImmediatelyFixedInternetExplorer);
    }

    private static void CancelModalDialogImmediatelyFixedInternetExplorer([NotNull] this BrowserSession browser)
    {
      var webDriver = (IWebDriver) browser.Native;
      webDriver.SwitchTo().Alert().Dismiss();
    }
  }
}