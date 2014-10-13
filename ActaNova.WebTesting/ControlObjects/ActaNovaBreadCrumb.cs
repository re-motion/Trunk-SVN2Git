using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaBreadCrumb : ActaNovaMainFrameControlObject, IClickableControlObject
  {
    public ActaNovaBreadCrumb ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public string Text
    {
      get { return Scope.FindCss ("span.breadCrumbElementText").Text; }
    }

    public UnspecifiedPageObject Click (IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      Scope.ClickAndWait (Context, actualActionBehavior);
      return UnspecifiedPage();
    }
  }
}