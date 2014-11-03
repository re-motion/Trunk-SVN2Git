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
  /// <summary>
  /// Page object representing the ActaNova main page.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
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
      var actualActionBehavior = completionDetection ?? Continue.When (ActaNovaCompletion.OuterInnerOuterUpdated);

      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "RefreshButton"));
      return webButton.Click (actualActionBehavior);
    }

    public ActaNovaHeaderControlObject Header
    {
      get
      {
        var headerScope = Scope.FindId ("HeaderPanel");
        return new ActaNovaHeaderControlObject (Context.CloneForControl (this, headerScope));
      }
    }

    public ActaNovaMainMenuControlObject MainMenu
    {
      get
      {
        var mainMenuScope = Scope.FindId ("MainMenu");
        return new ActaNovaMainMenuControlObject (Context.CloneForControl (this, mainMenuScope));
      }
    }

    public ActaNovaTreeControlObject Tree
    {
      get
      {
        var treeScope = Scope.FindId ("MainTreeView");
        return new ActaNovaTreeControlObject (Context.CloneForControl (this, treeScope));
      }
    }

    public AppToolsFormPageObject DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new AppToolsFormPageObject (Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}