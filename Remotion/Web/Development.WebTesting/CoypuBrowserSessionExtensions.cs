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
using Coypu;
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
    /// WebDriver implementations hang when trying to find an element within an <see cref="ElementScope"/> which is on an already closed window.
    /// Therefore, after closing a window, it is important to ensure that a non-closed window is active.
    /// </summary>
    public static void EnsureWindowIsActive ([NotNull] this BrowserWindow window)
    {
      ArgumentUtility.CheckNotNull ("window", window);

      // ReSharper disable once UnusedVariable
      var temp = window.Title;
    }

    /// <summary>
    /// Returns the scope of the HTML element on the given window.
    /// </summary>
    public static ElementScope GetRootScope ([NotNull] this BrowserWindow window)
    {
      ArgumentUtility.CheckNotNull ("window", window);

      return window.FindCss ("html");
    }

    /// <summary>
    /// Closes the window specified in the context.
    /// </summary>
    public static void CloseWindow ([NotNull] this PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.Window.ExecuteScript (CommonJavaScripts.SelfClose);
      context.ParentContext.Window.EnsureWindowIsActive();
    }

    /// <summary>
    /// Clears all cookies for the current domain.
    /// </summary>
    public static void ClearCookies ([NotNull] this BrowserSession browser)
    {
      var webDriver = browser.Native as IWebDriver;
      webDriver.Manage().Cookies.DeleteAllCookies();
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
        browser.AcceptModalDialogFixedInternetExplorer();
    }

    /// <summary>
    /// See <see cref="AcceptModalDialogFixed"/>, however, the <see cref="WebTestingConfiguration.SearchTimeout"/> and
    /// <see cref="WebTestingConfiguration.RetryInterval"/> do not apply.
    /// </summary>
    public static void AcceptModalDialogImmediatelyFixed ([NotNull] this BrowserSession browser)
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
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        browser.CancelModalDialog();
      else
        browser.CancelModalDialogFixedInternetExplorer();
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

    private static void AcceptModalDialogImmediatelyFixedInternetExplorer ([NotNull] this BrowserSession browser)
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

    private static void CancelModalDialogImmediatelyFixedInternetExplorer ([NotNull] this BrowserSession browser)
    {
      var webDriver = (IWebDriver) browser.Native;
      webDriver.SwitchTo().Alert().Dismiss();
    }
  }
}