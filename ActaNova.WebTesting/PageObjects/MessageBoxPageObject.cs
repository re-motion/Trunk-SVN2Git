using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing the AppTools-based page which displays a modal message box.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class MessageBoxPageObject : AppToolsPageObject
  {
    public MessageBoxPageObject ([NotNull] PageObjectContext context)
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