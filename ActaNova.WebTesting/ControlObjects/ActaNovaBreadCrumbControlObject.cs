using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova bread crumb.
  /// </summary>
  public class ActaNovaBreadCrumbControlObject : ActaNovaMainFrameControlObject, IClickableControlObject
  {
    public ActaNovaBreadCrumbControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the bread crumb's displayed text.
    /// </summary>
    public string Text
    {
      get { return Scope.FindCss ("span.breadCrumbElementText").Text.Trim(); }
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualActionBehavior = DetermineActualCompletionDetection (completionDetection);
      Scope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }
  }
}