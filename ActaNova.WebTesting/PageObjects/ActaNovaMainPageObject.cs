using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaMainPageObject : ActaNovaPageObject
  {
    public ActaNovaMainPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return DetailsArea.FormPageTitle;
    }

    public UnspecifiedPageObject Refresh (ICompletionDetection completionDetection = null)
    {
      var actualActionBehavior = completionDetection ?? Continue.When (WaitForActaNova.OuterInnerOuterUpdate);

      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "RefreshButton"));
      return webButton.Click (actualActionBehavior);
    }

    public ActaNovaHeaderControlObject Header
    {
      get
      {
        var headerScope = Scope.FindId ("HeaderPanel");
        return new ActaNovaHeaderControlObject (Context.CloneForControl (headerScope));
      }
    }

    public ActaNovaMainMenuControlObject MainMenu
    {
      get
      {
        var mainMenuScope = Scope.FindId ("MainMenu");
        return new ActaNovaMainMenuControlObject (Context.CloneForControl (mainMenuScope));
      }
    }

    public ActaNovaTreeControlObject Tree
    {
      get
      {
        var treeScope = Scope.FindId ("MainTreeView");
        return new ActaNovaTreeControlObject (Context.CloneForControl (treeScope));
      }
    }

    public ActaNovaDetailsAreaControlObject DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new ActaNovaDetailsAreaControlObject (Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}