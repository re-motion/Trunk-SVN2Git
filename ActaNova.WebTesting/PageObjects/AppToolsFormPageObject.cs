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
      var itemID = string.Format ("{0}Button", command);
      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), itemID));
      return webButton.Click (completionDetection);
    }
  }
}