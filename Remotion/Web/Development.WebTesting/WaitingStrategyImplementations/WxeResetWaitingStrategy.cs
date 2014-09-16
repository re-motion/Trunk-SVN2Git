using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategyImplementations
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxeResetWaitingStrategy : WxeWaitingStrategyBase, IWaitingStrategy
  {
    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      return null;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      const int expectedWxePostBackSequenceNumber = 2;
      WaitForWxePostBackSequenceNumber (context, expectedWxePostBackSequenceNumber);
    }
  }
}