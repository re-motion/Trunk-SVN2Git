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

    public abstract object PrepareWaitForCompletion (PageObjectContext context);
    public abstract void WaitForCompletion (PageObjectContext context, object state);

    protected PageObjectContext PageObjectContext { get; set; }

    protected string GetWxeFunctionToken ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return context.Scope.FindId (c_wxeFunctionToken).Value;
    }

    protected int GetWxePostBackSequenceNumber ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return int.Parse (context.Scope.FindId (c_wxePostBackSequenceNumberFieldId).Value);
    }

    protected void WaitForExpectedWxePostBackSequenceNumber ([NotNull] PageObjectContext context, int expectedWxePostBackSequenceNumber)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      LogManager.GetLogger (GetType())
          .DebugFormat ("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle (PageObjectContext));

      var newWxePostBackSequenceNumber = context.Window.Query (
          () => GetWxePostBackSequenceNumber (PageObjectContext),
          expectedWxePostBackSequenceNumber);

      Assertion.IsTrue (
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format ("Expected WXE-PSN to be '{0}', but it actually is '{1}'", expectedWxePostBackSequenceNumber, newWxePostBackSequenceNumber));
    }

    private string GetPageTitle (PageObjectContext page)
    {
      return page.Scope.FindCss ("title").InnerHTML;
    }
  }
}