using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaDetailsAreaControlObject : ActaNovaControlObject, IControlHost
  {
    public ActaNovaDetailsAreaControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string FormPageTitle
    {
      get { return Scope.FindCss (".formPageTitleLabel").Text; }
    }

    public UnspecifiedPageObject Perform (string command, IActionBehavior actionBehavior = null)
    {
      if (actionBehavior == null)
        actionBehavior = Behavior.WaitFor (WaitFor.WxePostBack);

      return GetControl (
          new PerHtmlIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), string.Format ("{0}Button", command)))
          .Click (actionBehavior);
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (Context);
    }
  }
}