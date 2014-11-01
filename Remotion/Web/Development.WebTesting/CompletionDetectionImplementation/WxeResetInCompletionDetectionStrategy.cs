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
    public WxeResetInCompletionDetectionStrategy ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      PageObjectContext = context;
    }

    protected WxeResetInCompletionDetectionStrategy ()
    {
    }

    public override object PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var wxeFunctionToken = GetWxeFunctionToken (PageObjectContext);
      return wxeFunctionToken;
    }

    public override void WaitForCompletion (PageObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      var oldWxeFunctionToken = (string) state;

      LogManager.GetLogger (GetType()).DebugFormat ("State: previous WXE-FT: {0}.", oldWxeFunctionToken);
      WaitForNewWxeFunctionToken (context, oldWxeFunctionToken);

      const int expectedWxePostBackSequenceNumber = 2;
      WaitForExpectedWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }

    private void WaitForNewWxeFunctionToken (PageObjectContext context, string oldWxeFunctionToken)
    {
      context.Window.Query (() => GetWxeFunctionToken (PageObjectContext) != oldWxeFunctionToken, true);

      Assertion.IsTrue (
          GetWxeFunctionToken (PageObjectContext) != oldWxeFunctionToken,
          string.Format ("Expected WXE-FT to be different to '{0}', but it is equal.", oldWxeFunctionToken));
    }
  }
}