using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaDetailsAreaControlObject : ActaNovaPageObject
  {
    public ActaNovaDetailsAreaControlObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public string FormPageTitle
    {
      get { return Scope.FindCss (".formPageTitleLabel").Text; }
    }

    public UnspecifiedPageObject Perform (string command, ICompletionDetection completionDetection = null)
    {
      if (completionDetection == null)
        completionDetection = Continue.When (Wxe.PostBackCompleted);

      return GetControl (
          new PerHtmlIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), string.Format ("{0}Button", command)))
          .Click (completionDetection);
    }
  }
}