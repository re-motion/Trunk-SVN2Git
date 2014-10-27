using System;
using Coypu;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  // Todo RM-6297: Improve class design + remove stateful Wxe*InWaitingStrategy implementations.

  /// <summary>
  /// Base class for all WXE-based (sequence counter based) waiting strategies.
  /// </summary>
  public abstract class WxeWaitingStrategyBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (WxeWaitingStrategyBase));

    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    protected int GetWxePostBackSequenceNumber (ElementScope scope)
    {
      return Int32.Parse (scope.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }

    protected void WaitForWxePostBackSequenceNumber (TestObjectContext context, ElementScope scope, int expectedWxePostBackSequenceNumber)
    {
      s_log.DebugFormat ("Performing actual wait on window '{0}' and scope '{1}'.", context.Window.Title, scope.FindCss ("title").InnerHTML.Trim());

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