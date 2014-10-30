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
    public ActaNovaMainPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return DetailsArea.FormPageTitle;
    }

    public UnspecifiedPageObject Refresh (ICompletionDetection completionDetection = null)
    {
      var actualActionBehavior = completionDetection ?? Behavior.WaitFor (WaitForActaNova.OuterInnerOuterUpdate);

      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "RefreshButton"));
      webButton.Click (actualActionBehavior);

      return UnspecifiedPage();
    }

    public ActaNovaHeaderControlObject Header
    {
      get
      {
        var headerScope = Scope.FindId ("HeaderPanel");
        return new ActaNovaHeaderControlObject (headerScope.Id, Context.CloneForScope (headerScope));
      }
    }

    public ActaNovaMainMenuControlObject MainMenu
    {
      get
      {
        var mainMenuScope = Scope.FindId ("MainMenu");
        return new ActaNovaMainMenuControlObject (mainMenuScope.Id, Context.CloneForScope (mainMenuScope));
      }
    }

    public ActaNovaTreeControlObject Tree
    {
      get
      {
        var treeScope = Scope.FindId ("MainTreeView");
        return new ActaNovaTreeControlObject (treeScope.Id, Context.CloneForScope (treeScope));
      }
    }

    public ActaNovaDetailsAreaControlObject DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new ActaNovaDetailsAreaControlObject (detailsAreaScope.Id, Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}