using System;
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="BrowserSession"/> class.
  /// </summary>
  public static class CoypuBrowserSessionExtensions
  {
    /// <summary>
    /// IE-compatible version for Selenium's <see cref="BrowserWindow.AcceptModalDialog"/> method.
    /// </summary>
    /// <remarks>
    /// We require this method to be called on a <see cref="BrowserSession"/> instance, as the fixed implementation needs access to it. We don't want
    /// to hide this fact by using <paramref name="context"/>.<see cref="TestObjectContext.Browser"/>.
    /// </remarks>
    /// <param name="browser">The <see cref="BrowserSession"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    public static void AcceptModalDialogFixed ([NotNull] this BrowserSession browser, [NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("context", context);

      if (context.Configuration.Browser != Browser.InternetExplorer)
        browser.AcceptModalDialog();
      else
        browser.AcceptModalDialogFixedInternetExplorer (context);
    }

    /// <summary>
    /// See <see cref="AcceptModalDialogFixed"/>, however, the <see cref="WebTestConfiguration.SearchTimeout"/> and
    /// <see cref="WebTestConfiguration.RetryInterval"/> do not apply.
    /// </summary>
    public static void AcceptModalDialogImmediatelyFixed([NotNull] this BrowserSession browser, [NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (context.Configuration.Browser != Browser.InternetExplorer)
        browser.AcceptModalDialog (Options.NoWait);
      else
        browser.AcceptModalDialogImmediatelyFixedInternetExplorer();
    }

    /// <summary>
    /// IE-compatible version for Selenium's <see cref="BrowserWindow.CancelModalDialog"/> method.
    /// </summary>
    /// <remarks>
    /// We require this method to be called on a <see cref="BrowserSession"/> instance, as the fixed implementation needs access to it. We don't want
    /// to hide this fact by using <paramref name="context"/>.<see cref="TestObjectContext.Browser"/>.
    /// </remarks>
    /// <param name="browser">The <see cref="BrowserSession"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    public static void CancelModalDialogFixed ([NotNull] this BrowserSession browser, [NotNull] TestObjectContext context)
    {
      // Todo RM-6297: get rid of code duplication in Accept/CancelModalDialogFixed and Accept/CancelModalDialogFixedInternetExplorer.

      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("context", context);

      if (context.Configuration.Browser != Browser.InternetExplorer)
        browser.CancelModalDialog();
      else
        browser.CancelModalDialogFixedInternetExplorer (context);
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) runs into a race condition when accepting modal browser
    /// dialogs.
    /// </summary>
    private static void AcceptModalDialogFixedInternetExplorer ([NotNull] this BrowserSession browser, [NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("context", context);

      RetryUntilTimeout.Run (
          browser.AcceptModalDialogImmediatelyFixedInternetExplorer,
          context.Configuration.SearchTimeout,
          context.Configuration.RetryInterval);
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
    private static void CancelModalDialogFixedInternetExplorer ([NotNull] this BrowserSession browser, [NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("context", context);

      RetryUntilTimeout.Run (
          browser.CancelModalDialogImmediatelyFixedInternetExplorer,
          context.Configuration.SearchTimeout,
          context.Configuration.RetryInterval);
    }

    private static void CancelModalDialogImmediatelyFixedInternetExplorer([NotNull] this BrowserSession browser)
    {
      var webDriver = (IWebDriver) browser.Native;
      webDriver.SwitchTo().Alert().Dismiss();
    }
  }
}