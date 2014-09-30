using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
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
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (context);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + _expectedWxePostBackSequenceNumberIncrease;

      WaitForWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }
  }
}