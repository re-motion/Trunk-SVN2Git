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

    public WxePostBackInCompletionDetectionStrategy ([NotNull] ElementScope frameRootElement, int expectedWxePostBackSequenceNumberIncrease)
    {
      ArgumentUtility.CheckNotNull ("frameRootElement", frameRootElement);

      FrameRootElement = frameRootElement;
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    protected WxePostBackInCompletionDetectionStrategy (int expectedWxePostBackSequenceNumberIncrease)
    {
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    public override object PrepareWaitForCompletion (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (FrameRootElement);
      return wxePostBackSequenceNumber;
    }

    public override void WaitForCompletion (TestObjectContext context, object state)
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