using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategyImplementations
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxePostBackWaitingStrategy : WxeWaitingStrategyBase, IWaitingStrategy
  {
    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (context);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + 1;

      WaitForWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }
  }
}