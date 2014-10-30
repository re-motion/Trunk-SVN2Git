using System;
using Coypu;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Base class for all WXE-based <see cref="ICompletionDetectionStrategy"/> implementations.
  /// </summary>
  public abstract class WxeCompletionDetectionStrategyBase : ICompletionDetectionStrategy
  {
    private const string c_wxeFunctionToken = "WxeFunctionToken";
    private const string c_wxePostBackSequenceNumberFieldId = "wxePostBackSequenceNumberField";

    public abstract object PrepareWaitForCompletion (TestObjectContext context);
    public abstract void WaitForCompletion (TestObjectContext context, object state);

    protected ElementScope FrameRootElement { get; set; }

    protected string GetWxeFunctionToken ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return scope.FindId (c_wxeFunctionToken).Value;
    }

    protected int GetWxePostBackSequenceNumber ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return int.Parse (scope.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }

    protected void WaitForExpectedWxePostBackSequenceNumber ([NotNull] TestObjectContext context, int expectedWxePostBackSequenceNumber)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      LogManager.GetLogger (GetType())
          .DebugFormat ("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetScopeTitle (context, FrameRootElement));

      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (FrameRootElement),
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format ("Expected WXE-PSN to be '{0}', but it actually is '{1}'", expectedWxePostBackSequenceNumber, newWxePostBackSequenceNumber));
    }

    private string GetScopeTitle (TestObjectContext context, ElementScope frameRootElement)
    {
      if (context.RootElement == frameRootElement)
        return "root";

      return frameRootElement.FindCss ("title").InnerHTML.Trim();
    }
  }
}