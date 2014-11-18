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
  /// Base class for all WXE-based <see cref="ICompletionDetectionStrategy"/> implementations.
  /// </summary>
  public abstract class WxeCompletionDetectionStrategyBase : ICompletionDetectionStrategy
  {
    private const string c_wxeFunctionToken = "WxeFunctionToken";
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    public abstract object PrepareWaitForCompletion (PageObjectContext context);
    public abstract void WaitForCompletion (PageObjectContext context, object state);

    protected PageObjectContext PageObjectContext { get; set; }

    protected string GetWxeFunctionToken ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run(() => context.Scope.FindId (c_wxeFunctionToken).Value);
    }

    protected int GetWxePostBackSequenceNumber ()
    {
      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run(() => int.Parse (PageObjectContext.Scope.FindId (c_wxePostBackSequenceNumberFieldId).Value));
    }

    protected void WaitForExpectedWxePostBackSequenceNumber (int expectedWxePostBackSequenceNumber)
    {

      LogManager.GetLogger (GetType())
          .DebugFormat ("Parameters: window: '{0}' scope: '{1}'.", PageObjectContext.Window.Title, GetPageTitle (PageObjectContext));

      var newWxePostBackSequenceNumber = PageObjectContext.Window.Query (
          GetWxePostBackSequenceNumber,
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format ("Expected WXE-PSN to be '{0}', but it actually is '{1}'", expectedWxePostBackSequenceNumber, newWxePostBackSequenceNumber));
    }

    private string GetPageTitle (PageObjectContext page)
    {
      // Note: use RetryUntilTimeout until Coypu secured its attribute access (https://github.com/featurist/coypu/issues/117).
      return RetryUntilTimeout.Run(() => page.Scope.FindCss ("title").InnerHTML);
    }
  }
}