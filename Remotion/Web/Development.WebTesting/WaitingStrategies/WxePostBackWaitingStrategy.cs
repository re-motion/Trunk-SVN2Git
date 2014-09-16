using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class WxePostBackWaitingStrategy : IWaitingStrategy
  {
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      var wxePostBackSequenceNumber = GetWxePostBackSequenceNumber (context);
      return wxePostBackSequenceNumber;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
      var oldWxePostBackSequenceNumber = (int) state;
      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + 1;

      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (context),
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber);
    }

    private int GetWxePostBackSequenceNumber (TestObjectContext context)
    {
      // Todo RM-6297: make exception safe.
      return int.Parse(context.RootElement.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }
  }
}