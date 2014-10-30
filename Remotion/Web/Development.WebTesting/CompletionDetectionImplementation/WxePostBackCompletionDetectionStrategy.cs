using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Blocks until the WXE postback sequence number (for the current frame) has increased by the given amount.
  /// </summary>
  public class WxePostBackCompletionDetectionStrategy : WxePostBackInCompletionDetectionStrategy
  {
    public WxePostBackCompletionDetectionStrategy (int expectedWxePostBackSequenceNumberIncrease)
        : base (expectedWxePostBackSequenceNumberIncrease)
    {
    }

    public override object PrepareWaitForCompletion (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      
      FrameRootElement = context.FrameRootElement;
      return base.PrepareWaitForCompletion (context);
    }

    public override void WaitForCompletion (TestObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      FrameRootElement = context.FrameRootElement;
      base.WaitForCompletion (context, state);
    }
  }
}