using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

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
    public UnspecifiedPageObject Click (IWebTestActionOptions actionOptions = null)
    {
      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      new ClickAction (this, Scope).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      if (IsLast)
        return Wxe.PostBackCompleted;

      return base.GetDefaultCompletionDetectionStrategy (scope);
    }

    /// <summary>
    /// Identifies the bread crumb as the last one.
    /// </summary>
    internal bool IsLast { private get; set; }
  }
}