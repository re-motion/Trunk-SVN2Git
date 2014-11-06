using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova auto complete reference value.
  /// </summary>
  [UsedImplicitly]
  public class ActaNovaAutoCompleteReferenceValueControlObject : BocAutoCompleteReferenceValueControlObject
  {
    public ActaNovaAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Clears the reference value.
    /// </summary>
    public UnspecifiedPageObject Clear ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var additionalButtonsScope = GetAdditionalButtonsScope();
      var clearValueButton =
          additionalButtonsScope.GetControl (
              new PerLocalIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "ClearValueButton"));

      return clearValueButton.Click (completionDetection);
    }

    /// <summary>
    /// Presses the "Edit" button for the reference value.
    /// </summary>
    public UnspecifiedPageObject Edit ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var additionalButtonsScope = GetAdditionalButtonsScope();
      var editButton =
          additionalButtonsScope.GetControl (new PerLocalIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), "EditButton"));

      return editButton.Click (completionDetection);
    }

    /// <summary>
    /// Returns the drop down list representing the search class.
    /// </summary>
    public DropDownListControlObject GetSearchClassDropDown ()
    {
      var scope = GetParentScope();
      return scope.GetControl (new SingleControlSelectionCommand<DropDownListControlObject> (new DropDownListSelector()));
    }

    /// <summary>
    /// Presses the "Search" button for the reference value search class.
    /// </summary>
    public UnspecifiedPageObject Search ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var scope = GetParentScope();
      var searchButton = scope.GetControl (
          new PerLocalIDControlSelectionCommand<WebButtonControlObject> (
              new WebButtonSelector(),
              "SelectRelatedObjectControlSearchReferencedObjectButton"));
      return searchButton.Click (completionDetection);
    }

    /// <summary>
    /// Presses the "New" button for the reference value search class.
    /// </summary>
    public UnspecifiedPageObject New ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var scope = GetParentScope();
      var newButton = scope.GetControl (
          new PerLocalIDControlSelectionCommand<WebButtonControlObject> (
              new WebButtonSelector(),
              "SelectRelatedObjectControlCreateReferencedObjectButton"));
      return newButton.Click (completionDetection);
    }

    /// <summary>
    /// Presses the "New in ÖKOM" button for the reference value search class.
    /// </summary>
    public UnspecifiedPageObject NewInOekom ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetection = completionDetection ?? Continue.When (Wxe.PostBackCompleted);

      var scope = GetNeighbourCellScope();
      var link = scope.Scope.FindLink();
      link.ClickAndWait (Context, actualCompletionDetection.Build());
      return UnspecifiedPage();
    }

    private ScopeControlObject GetAdditionalButtonsScope ()
    {
      var scope = Scope.FindXPath (string.Format ("../span{0}", XPathUtils.CreateHasClassCheck ("additionalButtonContainer")));
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    private ScopeControlObject GetParentScope ()
    {
      var scope = Scope.FindXPath ("..");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    private ScopeControlObject GetNeighbourCellScope ()
    {
      var scope = Scope.FindXPath ("../../../td[2]");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }
  }
}