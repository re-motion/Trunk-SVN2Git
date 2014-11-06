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
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Blocks until the WXE post back sequence number (for the given <see cref="ElementScope"/>) has increased by the given amount.
  /// </summary>
  public class WxePostBackInCompletionDetectionStrategy : WxeCompletionDetectionStrategyBase
  {
    private readonly int _expectedWxePostBackSequenceNumberIncrease;

    public WxePostBackInCompletionDetectionStrategy ([NotNull] PageObjectContext context, int expectedWxePostBackSequenceNumberIncrease)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      PageObjectContext = context;
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    protected WxePostBackInCompletionDetectionStrategy (int expectedWxePostBackSequenceNumberIncrease)
    {
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    public override object PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (PageObjectContext);
      return wxePostBackSequenceNumber;
    }

    public override void WaitForCompletion (PageObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + _expectedWxePostBackSequenceNumberIncrease;

      LogManager.GetLogger (GetType()).DebugFormat (
          "State: previous WXE-PSN: {0}, expected WXE-PSN: {1}.",
          oldWxePostBackSequenceNumber,
          expectedWxePostBackSequenceNumber);

      WaitForExpectedWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }
  }
}