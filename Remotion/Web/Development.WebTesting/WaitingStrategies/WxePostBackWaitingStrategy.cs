using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxePostBackWaitingStrategy : IWaitingStrategy
  {
    private const string c_wxePostBackSequenceNumberFieldId = "#wxePostBackSequenceNumberField";

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (context);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (string) state;
      // Todo RM-6297: Implementation
    }

    private string GetWxePostBackSequenceNumber (TestObjectContext context)
    {
      // Todo RM-6297: make exception safe.
      return context.RootElement.FindId (c_wxePostBackSequenceNumberFieldId).Value;
    }
  }
}