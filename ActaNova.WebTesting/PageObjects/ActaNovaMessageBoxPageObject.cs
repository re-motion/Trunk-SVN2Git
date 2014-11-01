using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class ActaNovaMessageBoxPageObject : ActaNovaPageObject
  {
    public ActaNovaMessageBoxPageObject ([NotNull] PageObjectContext context)
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