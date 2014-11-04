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

    public UnspecifiedPageObject Confirm ()
    {
      var context = DetailsArea.Context;
      var messageBoxScope = context.Scope.FindId ("DisplayBoxPopUp_MessagePopupDisplayBoxPopUp");
      var actaNovaMessageBox = new ActaNovaMessageBoxControlObject (context.CloneForControl (this, messageBoxScope));
      return actaNovaMessageBox.Okay();
    }

    public UnspecifiedPageObject Cancel ()
    {
      var context = DetailsArea.Context;
      var messageBoxScope = context.Scope.FindId ("DisplayBoxPopUp_MessagePopupDisplayBoxPopUp");
      var actaNovaMessageBox = new ActaNovaMessageBoxControlObject (context.CloneForControl (this, messageBoxScope));
      return actaNovaMessageBox.Cancel();
    }

    private AppToolsFormPageObject DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new AppToolsFormPageObject (Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}