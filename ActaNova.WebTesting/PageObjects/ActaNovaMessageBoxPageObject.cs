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

    public UnspecifiedPageObject Confirm (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Okay (completionDetection);
    }

    public UnspecifiedPageObject Cancel (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Cancel (completionDetection);
    }

    public UnspecifiedPageObject Yes (ICompletionDetection completionDetection = null)
    {
      var mesageBox = GetActaNovaMessageBoxControlObject();
      return mesageBox.Yes (completionDetection);
    }

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