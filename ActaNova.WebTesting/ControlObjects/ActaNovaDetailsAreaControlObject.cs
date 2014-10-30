using System;
using JetBrains.Annotations;
using Remotion.Utilities;
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

    public UnspecifiedPageObject Perform (string command, ICompletionDetection completionDetection = null)
    {
      if (completionDetection == null)
        completionDetection = Behavior.WaitFor (WaitFor.WxePostBack);

      return GetControl (
          new PerHtmlIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), string.Format ("{0}Button", command)))
          .Click (completionDetection);
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}