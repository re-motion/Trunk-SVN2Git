using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxeResetWaitingStrategy : WxeResetWaitingStrategyBase, IWaitingStrategy
  {
    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxeFunctionToken = GetWxeFunctionToken (context.FrameRootElement);
      return wxeFunctionToken;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxeFunctionToken = (string) state;
      WaitForWxeFunctionTokenToBeDifferent (context, context.FrameRootElement, oldWxeFunctionToken);

      const int expectedWxePostBackSequenceNumber = 2;
      WaitForWxePostBackSequenceNumber (context, context.FrameRootElement, expectedWxePostBackSequenceNumber);
    }
  }
}