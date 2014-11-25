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
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="BrowserWindow"/> class regaring modal dialog handling.
  /// </summary>
  public static class CoypuBrowserWindowModalDialogExtensions
  {
    /// <summary>
    /// IE-compatible version for Coypu's <see cref="BrowserWindow.AcceptModalDialog"/> method.
    /// </summary>
    /// <param name="window">The <see cref="BrowserWindow"/> on which the action is performed.</param>
    /// <param name="browser">The corresponding <see cref="BrowserSession"/> (internally required for IE-fixes).</param>
    public static void AcceptModalDialogFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        window.AcceptModalDialog();
      else
        HandleModalDialogFixedForInternetExplorer (browser, window, a => a.Accept(), true);
    }

    /// <summary>
    /// See <see cref="AcceptModalDialogFixed"/>, however, the <see cref="WebTestingConfiguration.SearchTimeout"/> and
    /// <see cref="WebTestingConfiguration.RetryInterval"/> do not apply.
    /// </summary>
    public static void AcceptModalDialogImmediatelyFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        window.AcceptModalDialog (Options.NoWait);
      else
        HandleModalDialogFixedForInternetExplorer (browser, window, a => a.Accept(), false);
    }

    /// <summary>
    /// IE-compatible version for Coypu's <see cref="BrowserWindow.CancelModalDialog"/> method.
    /// </summary>
    /// <param name="window">The <see cref="BrowserWindow"/> on which the action is performed.</param>
    /// <param name="browser">The corresponding <see cref="BrowserSession"/> (internally required for IE-fixes).</param>
    public static void CancelModalDialogFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("browser", browser);

      if (!WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        window.CancelModalDialog();
      else
        HandleModalDialogFixedForInternetExplorer (browser, window, a => a.Dismiss(), true);
    }

    private static void HandleModalDialogFixedForInternetExplorer (
        [NotNull] BrowserSession browser,
        [NotNull] BrowserWindow window,
        [NotNull] Action<IAlert> modalDialogAction,
        bool retryUntilTimeout)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("modalDialogAction", modalDialogAction);

      Action handleModalDialogAction = () => modalDialogAction (window.GetNativeFromBrowserWindow().SwitchTo().Alert());

      if (retryUntilTimeout)
        RetryUntilTimeout.Run (handleModalDialogAction);
      else
        handleModalDialogAction();

      // Prevent IE driver crash after window change & possible close with non-Coypu means.
      browser.EnsureWindowIsActive();

      // Unfortunately, we run into a race condition *after* accepting the modal dialog again, so we need to do some waiting here.
      // Todo RM-6297: Try to get rid of waiting. See https://groups.google.com/forum/#!topic/selenium-users/NrtJnq7b678 for more information.
      Thread.Sleep (TimeSpan.FromMilliseconds (250));
    }
  }
}