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
  public class ActaNovaMainPageObject : ActaNovaPageObjectBase
  {
    public ActaNovaMainPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
      // Ensure that inner frame has loaded before actual work starts (otherwise IE driver sometimes loads right frame in new window, don't know why).
      // ActaNovaBreadCrumbControlObjectTest.Test often failed that way.
      FormPage.Scope.EnsureExistence();
    }

    public override string GetTitle ()
    {
      return FormPage.GetTitle();
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

    public ActaNovaFormPageObject FormPage
    {
      get { return new ActaNovaFormPageObject (GetContextForFrame()); }
    }

    public ActaNovaWorkListPageObject WorkList
    {
      get { return new ActaNovaWorkListPageObject (GetContextForFrame()); }
    }

    private PageObjectContext GetContextForFrame ()
    {
      var frameScope = Scope.FindFrame ("RightFrameContent");
      return Context.CloneForFrame (frameScope);
    }
  }
}