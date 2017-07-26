﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various general extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeExtensions
  {
    /// <summary>
    /// This method ensures that the given <paramref name="scope"/> is existent. Coypu would automatically check for existence whenever we access the
    /// scope, however, we explicitly check the existence when creating new control objects. This ensures that any <see cref="MissingHtmlException"/>
    /// or <see cref="AmbiguousException"/> are thrown when the control object's corresponding <see cref="WebTestObjectContext"/> is created, which
    /// is always near the corresponding <c>parentScope.Find*()</c> method call. Otherwise, exceptions would be thrown when the context's
    /// <see cref="Scope"/> is actually used for the first time, which may be quite some time later and the exception would provide a stack trace
    /// where the <c>parentScope.Find*()</c> call could not be found.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to exist.</param>
    public static void EnsureExistence ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.Now();
    }

    /// <summary>
    /// Ensures that the given <see cref="Scope"/> exists, much like <see cref="EnsureExistence"/>. However, it ensures that Coypu uses the
    /// <see cref="Match.Single"/> matching strategy.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to match only a single DOM element.</param>
    public static void EnsureSingle ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      var matchBackup = scope.ElementFinder.Options.Match;

      scope.ElementFinder.Options.Match = Match.Single;
      scope.Now();
      scope.ElementFinder.Options.Match = matchBackup;
    }

    /// <summary>
    /// Coypu`s <paramref name="scope"/>.<see cref="ElementScope.Exists"/> does not work correctly when working with iframes (see https://www.re-motion.org/jira/projects/RM/issues/RM-6770).
    /// </summary>
    /// <remarks>
    /// As a workaround, we call <paramref name="scope"/>.<see cref="DriverScope.Now"/> and return <see langword="false" /> if an <see cref="MissingHtmlException"/>, <see cref="MissingWindowException"/> or <see cref="StaleElementException"/> is thrown.
    /// These are the same exceptions that Coypu is catching in its <paramref name="scope"/>.<see cref="ElementScope.Exists"/> call.
    /// Should be removed when the issue is fixed in coypu https://www.re-motion.org/jira/browse/RM-6773.
    /// </remarks>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to match only a single DOM element.</param>
    public static bool ExistsWorkaround ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      try
      {
        scope.Now();
        return true;
      }
      catch (MissingHtmlException)
      {
        return false;
      }
      catch (MissingWindowException)
      {
        return false;
      }
      catch (StaleElementException)
      {
        return false;
      }
    }

    /// <summary>
    /// Exists-workaround which also ensures that the element is single (e.g. throws an <see cref="AmbiguousException"/> if the element is not single).
    /// </summary>
    /// <remarks>
    /// This has to be done in its own function, as calling <see cref="EnsureSingle"/> after <see cref="ExistsWorkaround"/> does not throw the expected exception.
    /// This is due to caching of the <paramref name="scope"/>.<see cref="DriverScope.Now"/> call. <see cref="ExistsWorkaround"/> calls <paramref name="scope"/>.<see cref="DriverScope.Now"/> without <see cref="Match.Single"/> matching strategy ->
    /// <see cref="EnsureSingle"/> calls <paramref name="scope"/>.<see cref="DriverScope.Now"/> with <see cref="Match.Single"/> matching strategy.
    /// If .<see cref="ExistsWorkaround"/> is called before <see cref="EnsureSingle"/>, the scope is cached and no evaluation takes place.
    /// Should be removed when the issue is fixed in coypu https://www.re-motion.org/jira/browse/RM-6773.
    /// </remarks>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to match only a single DOM element.</param>
    public static bool ExistsWithEnsureSingleWorkaround ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      var matchBackup = scope.ElementFinder.Options.Match;
      scope.ElementFinder.Options.Match = Match.Single;

      var scopeExists = scope.ExistsWorkaround();

      scope.ElementFinder.Options.Match = matchBackup;

      return scopeExists;
    }

    /// <summary>
    /// Returns the computed background color of the control. This method ignores background images as well as transparencies - the first
    /// non-transparent color set in the node's hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <returns>The background color or <see cref="WebColor.Transparent"/> if no background color is set (not even on any parent node).</returns>
    public static WebColor GetComputedBackgroundColor ([NotNull] this ElementScope scope, [NotNull] ControlObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);

      // Todo RM-6337: Coypu does not support JavaScript executions with arguments by now, simplify as soon as https://github.com/featurist/coypu/issues/128 has been implemented.
      var javaScriptExecutor = (IJavaScriptExecutor) context.Browser.Driver.Native;
      var computedBackgroundColor =
          RetryUntilTimeout.Run (() => (string) javaScriptExecutor.ExecuteScript (CommonJavaScripts.GetComputedBackgroundColor, scope.Native));

      if (IsTransparent (computedBackgroundColor))
        return WebColor.Transparent;

      return ParseColorFromBrowserReturnedString (computedBackgroundColor);
    }

    /// <summary>
    /// Returns the computed text color of the control. This method ignores transparencies - the first non-transparent color set in the node's
    /// DOM hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <returns>The text color or <see cref="WebColor.Transparent"/> if no text color is set (not even on any parent node).</returns>
    public static WebColor GetComputedTextColor ([NotNull] this ElementScope scope, [NotNull] ControlObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);

      // Todo RM-6337: Coypu does not support JavaScript executions with arguments by now, simplify as soon as https://github.com/featurist/coypu/issues/128 has been implemented.
      var javaScriptExecutor = (IJavaScriptExecutor) context.Browser.Driver.Native;
      var computedTextColor =
          RetryUntilTimeout.Run (() => (string) javaScriptExecutor.ExecuteScript (CommonJavaScripts.GetComputedTextColor, scope.Native));

      if (IsTransparent (computedTextColor))
        return WebColor.Transparent;

      return ParseColorFromBrowserReturnedString (computedTextColor);
    }

    private static bool IsTransparent ([NotNull] string color)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("color", color);

      // Chrome
      if (color == "rgba(0, 0, 0, 0)")
        return true;

      // IE11
      if (color == "transparent")
        return true;

      return false;
    }

    private static WebColor ParseColorFromBrowserReturnedString ([NotNull] string color)
    {
      var rgbArgs = color.Split (new[] { '(', ',', ')' });
      var rgb = rgbArgs.Skip (1).Take (3).Select (byte.Parse).ToArray();
      return WebColor.FromRgb (rgb[0], rgb[1], rgb[2]);
    }

    /// <summary>
    /// Returns whether the given <paramref name="scope"/>, which must represent a suitable HTML element (e.g. a checkbox), is currently selected.
    /// </summary>
    /// <returns>True if the HTML element is selected, otherwise false.</returns>
    public static bool IsSelected ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;
            return webElement.Selected;
          });
    }

    /// <summary>
    /// Returns whether the given <paramref name="scope"/> is currently displayed (visible). The given <paramref name="scope"/> must exist, otherwise
    /// this method will throw an <see cref="MissingHtmlException"/>.
    /// </summary>
    /// <returns>True if the given <paramref name="scope"/> is visible, otherwise false.</returns>
    public static bool IsVisible ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return RetryUntilTimeout.Run (
          () =>
          {
            var webElement = (IWebElement) scope.Native;
            return webElement.Displayed;
          });
    }

    /// <summary>
    /// Ensures unhovering of the given <paramref name="scope"/> by placing the cursor back at 0/0 in the top-left corner.
    /// </summary>
    public static void Unhover ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      Cursor.Position = new Point (0, 0);
    }
  }
}