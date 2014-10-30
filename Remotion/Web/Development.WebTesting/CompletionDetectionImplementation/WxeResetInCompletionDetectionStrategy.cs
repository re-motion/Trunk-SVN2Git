using System;
using Coypu;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Blocks until the WXE function token is different and the WXE post back sequence number (for the given <see cref="ElementScope"/>) has been
  /// reset.
  /// </summary>
  public class WxeResetInCompletionDetectionStrategy : WxeCompletionDetectionStrategyBase
  {
    public WxeResetInCompletionDetectionStrategy ([NotNull] ElementScope frameRootElement)
    {
      ArgumentUtility.CheckNotNull ("frameRootElement", frameRootElement);

      FrameRootElement = frameRootElement;
    }

    protected WxeResetInCompletionDetectionStrategy ()
    {
    }

    public override object PrepareWaitForCompletion (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var wxeFunctionToken = GetWxeFunctionToken (FrameRootElement);
      return wxeFunctionToken;
    }

    public override void WaitForCompletion (TestObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      var oldWxeFunctionToken = (string) state;

      LogManager.GetLogger (GetType()).DebugFormat ("State: previous WXE-FT: {0}.", oldWxeFunctionToken);
      WaitForNewWxeFunctionToken (context, oldWxeFunctionToken);

      const int expectedWxePostBackSequenceNumber = 2;
      WaitForExpectedWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }

    private void WaitForNewWxeFunctionToken (TestObjectContext context, string oldWxeFunctionToken)
    {
      context.Window.Query (() => GetWxeFunctionToken (FrameRootElement) != oldWxeFunctionToken, true);

      Assertion.IsTrue (
          GetWxeFunctionToken (FrameRootElement) != oldWxeFunctionToken,
          string.Format ("Expected WXE-FT to be different to '{0}', but it is equal.", oldWxeFunctionToken));
    }
  }
}