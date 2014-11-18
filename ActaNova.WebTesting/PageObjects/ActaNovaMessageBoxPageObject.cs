using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing the ActaNova message box page.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class ActaNovaMessageBoxPageObject : ActaNovaPageObjectBase
  {
    public ActaNovaMessageBoxPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Confirms the message box by pressing the OK button.
    /// </summary>
    public UnspecifiedPageObject Confirm (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Okay (completionDetection);
    }

    /// <summary>
    /// Cancels the message box by pressing the Cancel button.
    /// </summary>
    public UnspecifiedPageObject Cancel (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Cancel (completionDetection);
    }

    /// <summary>
    /// Confirms the message box by pressing the Yes button.
    /// </summary>
    public UnspecifiedPageObject Yes (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Yes (completionDetection);
    }

    /// <summary>
    /// Cancels the message box by pressing the No button.
    /// </summary>
    public UnspecifiedPageObject No (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.No (completionDetection);
    }

    private ActaNovaMessageBoxControlObject GetActaNovaMessageBoxControlObject ()
    {
      var messageBoxScope = Scope.FindId ("DisplayBoxPopUp_MessagePopupDisplayBoxPopUp");
      if (messageBoxScope.IsVisible())
        return new ActaNovaMessageBoxControlObject (Context.CloneForControl (this, messageBoxScope));

      var detailsArea = GetDetailsArea();
      messageBoxScope = detailsArea.Scope.FindId ("DisplayBoxPopUp_MessagePopupDisplayBoxPopUp");
      return new ActaNovaMessageBoxControlObject (detailsArea.Context.CloneForControl (detailsArea, messageBoxScope));
    }

    private ActaNovaPageObject GetDetailsArea ()
    {
      var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
      var detailsArea = new ActaNovaPageObject (Context.CloneForFrame (detailsAreaScope));
      return detailsArea;
    }
  }
}