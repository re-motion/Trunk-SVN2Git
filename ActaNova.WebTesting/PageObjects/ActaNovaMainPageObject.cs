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

    /// <summary>
    /// Returns the main page's title (fails if the inner frame does not provide a form grid title).
    /// </summary>
    public override string GetTitle ()
    {
      return FormPage.GetTitle();
    }

    /// <summary>
    /// Presses the refresh button.
    /// </summary>
    public UnspecifiedPageObject Refresh ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualActionBehavior = MergeWithDefaultActionOptions (actionOptions);

      var webButton = GetControl (new ItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "RefreshButton"));
      return webButton.Click (actualActionBehavior);
    }

    /// <summary>
    /// Gives access to the header area by returning the <see cref="ActaNovaHeaderControlObject"/>.
    /// </summary>
    public ActaNovaHeaderControlObject Header
    {
      get
      {
        var headerScope = Scope.FindId ("HeaderPanel");
        return new ActaNovaHeaderControlObject (Context.CloneForControl (this, headerScope));
      }
    }

    /// <summary>
    /// Gives access to the main menu by returning the <see cref="ActaNovaMainMenuControlObject"/>.
    /// </summary>
    public ActaNovaMainMenuControlObject MainMenu
    {
      get
      {
        var mainMenuScope = Scope.FindId ("MainMenu");
        return new ActaNovaMainMenuControlObject (Context.CloneForControl (this, mainMenuScope));
      }
    }

    /// <summary>
    /// Gives access to the main tree by returning the <see cref="ActaNovaTreeControlObject"/>.
    /// </summary>
    public ActaNovaTreeControlObject Tree
    {
      get
      {
        var treeScope = Scope.FindId ("MainTreeView");
        return new ActaNovaTreeControlObject (Context.CloneForControl (this, treeScope));
      }
    }

    /// <summary>
    /// Gives access to the inner frame which should display a form page (<see cref="ActaNovaFormPageObject"/>) at the time of accessing this property.
    /// </summary>
    public ActaNovaFormPageObject FormPage
    {
      get { return new ActaNovaFormPageObject (GetContextForFrame()); }
    }

    /// <summary>
    /// Gives access to the inner frame which should display a work list page (<see cref="ActaNovaWorkListPageObject"/>) at the time of accessing this
    /// property.
    /// </summary>
    public ActaNovaWorkListPageObject WorkListPage
    {
      get { return new ActaNovaWorkListPageObject (GetContextForFrame()); }
    }

    /// <summary>
    /// Gives access to the inner frame which should display a <typeparamref name="TPageObject"/> at the time of calling this method.
    /// </summary>
    public TPageObject GetFrame<TPageObject> ()
        where TPageObject : PageObject
    {
      return (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { GetContextForFrame() });
    }

    private PageObjectContext GetContextForFrame ()
    {
      var frameScope = Scope.FindFrame ("RightFrameContent");
      return Context.CloneForFrame (frameScope);
    }

    private IWebTestActionOptions MergeWithDefaultActionOptions (IWebTestActionOptions actionOptions)
    {
      if (actionOptions == null)
        actionOptions = new WebTestActionOptions();
      if (actionOptions.CompletionDetectionStrategy == null)
        actionOptions.CompletionDetectionStrategy = ActaNovaCompletion.OuterInnerOuterUpdated;

      return actionOptions;
    }
  }
}