using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaBreadCrumb : ActaNovaControlObject, IClickable
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
      // TODO RM-62978: Use a different waiting strategy - wait for second increase of main wxePostSequenceCounter!

      Scope.ClickAndWait (Context, waitingStrategy);
      return UnspecifiedPage();
    }
  }
}