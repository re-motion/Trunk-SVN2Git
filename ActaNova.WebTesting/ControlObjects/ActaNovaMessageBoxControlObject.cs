using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

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
    public UnspecifiedPageObject Okay (ICompletionDetection completionDetection = null)
    {
      return ClickButton ("OK", completionDetection);
    }

    /// <summary>
    /// Cancels the ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Cancel (ICompletionDetection completionDetection = null)
    {
      return ClickButton ("Cancel", completionDetection);
    }

    private UnspecifiedPageObject ClickButton (string buttonId, ICompletionDetection userDefinedCompletionDetection = null)
    {
      var id = string.Format ("DisplayBoxPopUp_MessageBoxControl_Popup{0}Button", buttonId);
      var buttonScope = Scope.FindId (id);
      var actualCompletionDetector = GetActualCompletionDetector (buttonScope, userDefinedCompletionDetection);
      buttonScope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }
  }
}