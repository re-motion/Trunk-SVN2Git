using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Waits for the WXE sequence number to increase by a given amount.
  /// </summary>
  public class WxePostBackWaitingStrategy : WxeWaitingStrategyBase, IWaitingStrategy
  {
    private readonly int _expectedWxePostBackSequenceNumberIncrease;

    public WxePostBackWaitingStrategy (int expectedWxePostBackSequenceNumberIncrease)
    {
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
    }

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (context.FrameRootElement);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + _expectedWxePostBackSequenceNumberIncrease;

      WaitForWxePostBackSequenceNumber (context, context.FrameRootElement, expectedWxePostBackSequenceNumber);
    }
  }
}