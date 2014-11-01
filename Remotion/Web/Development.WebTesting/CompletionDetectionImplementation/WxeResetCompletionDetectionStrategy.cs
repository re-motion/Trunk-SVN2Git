using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Blocks until the WXE function token is different and the WXE post back sequence number (for the current frame) has been reset.
  /// </summary>
  public class WxeResetCompletionDetectionStrategy : WxeResetInCompletionDetectionStrategy
  {
    public override object PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      
      PageObjectContext = context;
      return base.PrepareWaitForCompletion (context);
    }

    public override void WaitForCompletion (PageObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      PageObjectContext = context;
      base.WaitForCompletion (context, state);
    }
  }
}