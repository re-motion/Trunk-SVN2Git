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
    /// <param name="scope"></param>
    public static void EnsureSingle ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.ElementFinder.Options.Match = Match.Single;
      scope.Now();
      scope.ElementFinder.Options.Match = WebTestingConstants.DefaultMatchStrategy;
    }

    /// <summary>
    /// Returns whether the given <paramref name="scope"/> is currently displayed (visible). The given <paramref name="scope"/> must exist, otherwise
    /// this method will throw an <see cref="NoSuchElementException"/>.
    /// </summary>
    /// <returns>True if the given <paramref name="scope"/> is visible, otherwise false.</returns>
    public static bool IsVisible ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      var webElement = (IWebElement) scope.Native;
      return webElement.Displayed;
    }
  }
}