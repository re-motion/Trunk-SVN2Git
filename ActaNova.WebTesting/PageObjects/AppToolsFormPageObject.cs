using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.PageObjects
{
  public class AppToolsFormPageObject : AppToolsPageObject
  {
    public AppToolsFormPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public string FormPageTitle
    {
      get { return Scope.FindCss (".formPageTitleLabel").Text; }
    }

    public UnspecifiedPageObject Perform (string command, ICompletionDetection completionDetection = null)
    {
      var localID = string.Format ("{0}Button", command);
      var webButton = GetControl (new PerLocalIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), localID));
      return webButton.Click (completionDetection);
    }
  }
}