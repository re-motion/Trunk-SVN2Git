using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaMessageBoxPageObject : ActaNovaPageObject
  {
    public ActaNovaMessageBoxPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Confirm ()
    {
      var context = DetailsArea.Context;
      var messageBoxScope = context.FrameRootElement.FindId ("DisplayBoxPopUp_MessagePopupDisplayBoxPopUp");
      var actaNovaMessageBox = new ActaNovaMessageBoxControlObject (messageBoxScope.Id, context.CloneForScope (messageBoxScope));
      actaNovaMessageBox.Okay();
      return UnspecifiedPage();
    }

    private ActaNovaDetailsAreaControlObject DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new ActaNovaDetailsAreaControlObject (detailsAreaScope.Id, Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}