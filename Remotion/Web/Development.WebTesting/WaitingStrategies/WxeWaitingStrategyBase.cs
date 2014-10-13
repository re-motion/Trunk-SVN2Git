using System;
using Coypu;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// Base class for all WXE-based (sequence counter based) waiting strategies.
  /// </summary>
  public abstract class WxeWaitingStrategyBase
  {
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    protected int GetWxePostBackSequenceNumber (ElementScope scope)
    {
      // Todo RM-6297: make exception safe.
      return Int32.Parse (scope.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }

    protected void WaitForWxePostBackSequenceNumber (TestObjectContext context, ElementScope scope, int expectedWxePostBackSequenceNumber)
    {
      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (scope),
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