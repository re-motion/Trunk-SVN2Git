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
    public ActaNovaBreadCrumbControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the bread crumb's displayed text.
    /// </summary>
    public string Text
    {
      get { return Scope.FindCss ("span.breadCrumbElementText").Text.Trim(); }
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }
  }
}