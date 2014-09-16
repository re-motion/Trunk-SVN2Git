using System;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategyImplementations
{
  /// <summary>
  /// Base class for all WXE-based (sequence counter based) waiting strategies.
  /// </summary>
  public abstract class WxeWaitingStrategyBase
  {
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    protected int GetWxePostBackSequenceNumber (TestObjectContext context)
    {
      // Todo RM-6297: make exception safe.
      return Int32.Parse (context.RootElement.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }

    protected void WaitForWxePostBackSequenceNumber (TestObjectContext context, int expectedWxePostBackSequenceNumber)
    {
      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (context),
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format (
              "Expected wxePostBackSequenceNumberField to be '{0}', but it actually is '{1}'",
              expectedWxePostBackSequenceNumber,
              newWxePostBackSequenceNumber));
    }
  }
}