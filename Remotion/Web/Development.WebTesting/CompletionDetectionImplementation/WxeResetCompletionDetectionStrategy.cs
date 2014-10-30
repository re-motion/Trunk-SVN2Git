using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Blocks until the WXE function token is different and the WXE post back sequence number (for the current frame) has been reset.
  /// </summary>
  public class WxeResetCompletionDetectionStrategy : WxeResetInCompletionDetectionStrategy
  {
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