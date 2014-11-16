using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova message box.
  /// </summary>
  public class ActaNovaMessageBoxControlObject : WebFormsControlObjectWithDiagnosticMetadata
  {
    public ActaNovaMessageBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Confirms the ActaNova message box (presses the "OK" button on an ActaNova message box).
    /// </summary>
    public UnspecifiedPageObject Okay (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      return ClickButton ("OK", completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Cancels the ActaNova message box (presses the "Cancel" button on an ActaNova message box).
    /// </summary>
    public UnspecifiedPageObject Cancel (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      return ClickButton ("Cancel", completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Presses the "Yes" button on an ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Yes (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      return ClickButton ("Yes", completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Presses the "No" button on an ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject No (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      return ClickButton ("No", completionDetection, modalDialogHandler);
    }

    private UnspecifiedPageObject ClickButton (
        string buttonId,
        ICompletionDetection userDefinedCompletionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var itemID = string.Format ("Popup{0}Button", buttonId);
      var webButton = Children.GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), itemID));
      return webButton.Click (userDefinedCompletionDetection, modalDialogHandler);
    }
  }
}