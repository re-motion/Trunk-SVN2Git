using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

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

    public UnspecifiedPageObject Click (IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (waitingStrategy);
      Scope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }
}