using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova bread crumb.
  /// </summary>
  public class ActaNovaBreadCrumbControlObject : ActaNovaMainFrameControlObject, IClickableControlObject, IControlObjectWithText
  {
    public ActaNovaBreadCrumbControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.FindCss ("span.breadCrumbElementText").Text.Trim();
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null, IModalDialogHandler modalDialogHandler = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetector, modalDialogHandler);
      return UnspecifiedPage();
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      if(IsLast)
        return Continue.When (Wxe.PostBackCompleted);

      return base.GetDefaultCompletionDetection (scope);
    }

    /// <summary>
    /// Identifies the bread crumb as the last one.
    /// </summary>
    internal bool IsLast { private get; set; }
  }
}