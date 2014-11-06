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
  public class ActaNovaMessageBoxControlObject : RemotionControlObject
  {
    public ActaNovaMessageBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Confirms the ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Okay ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      return ClickButton ("OK", completionDetection);
    }

    /// <summary>
    /// Cancels the ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Cancel ([CanBeNull] ICompletionDetection completionDetection = null)
    {
      return ClickButton ("Cancel", completionDetection);
    }

    private UnspecifiedPageObject ClickButton (string buttonId, ICompletionDetection userDefinedCompletionDetection = null)
    {
      var itemID = string.Format ("Popup{0}Button", buttonId);
      var webButton = Children.GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), itemID));
      return webButton.Click (userDefinedCompletionDetection);
    }
  }
}