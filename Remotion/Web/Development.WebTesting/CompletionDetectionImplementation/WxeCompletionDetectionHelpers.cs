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
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Various helper methods for all WXE-based <see cref="ICompletionDetectionStrategy"/> implementations.
  /// </summary>
  internal static class WxeCompletionDetectionHelpers
  {
    private const string c_wxeFunctionToken = "WxeFunctionToken";
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    public static int GetWxePostBackSequenceNumber ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run (() => int.Parse (context.Scope.FindId (c_wxePostBackSequenceNumberFieldId).Value));
    }

    public static string GetWxeFunctionToken ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run (() => context.Scope.FindId (c_wxeFunctionToken).Value);
    }

    public static void WaitForExpectedWxePostBackSequenceNumber (
        [NotNull] ILog log,
        [NotNull] PageObjectContext context,
        int expectedWxePostBackSequenceNumber)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("context", context);

      log.DebugFormat ("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle (context));

      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (context),
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format ("Expected WXE-PSN to be '{0}', but it actually is '{1}'", expectedWxePostBackSequenceNumber, newWxePostBackSequenceNumber));
    }

    public static void WaitForExpectedWxePostBackSequenceNumber (
        [NotNull] ILog log,
        [NotNull] PageObjectContext context,
        int oldWxePostBackSequenceNumber,
        int expectedWxePostBackSequenceNumberIncrease)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("context", context);

      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + expectedWxePostBackSequenceNumberIncrease;

      log.DebugFormat ("State: previous WXE-PSN: {0}, expected WXE-PSN: {1}.", oldWxePostBackSequenceNumber, expectedWxePostBackSequenceNumber);

      WaitForExpectedWxePostBackSequenceNumber (log, context, expectedWxePostBackSequenceNumber);
    }

    public static void WaitForNewWxeFunctionToken ([NotNull] ILog log, [NotNull] PageObjectContext context, [NotNull] string oldWxeFunctionToken)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("oldWxeFunctionToken", oldWxeFunctionToken);

      log.DebugFormat ("State: previous WXE-FT: {0}.", oldWxeFunctionToken);
      log.DebugFormat ("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle (context));

      context.Window.Query (() => GetWxeFunctionToken (context) != oldWxeFunctionToken, true);

      Assertion.IsTrue (
          GetWxeFunctionToken (context) != oldWxeFunctionToken,
          string.Format ("Expected WXE-FT to be different to '{0}', but it is equal.", oldWxeFunctionToken));
    }

    private static string GetPageTitle ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run (() => context.Scope.FindCss ("title").InnerHTML);
    }
  }
}