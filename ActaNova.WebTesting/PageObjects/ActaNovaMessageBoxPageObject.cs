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
      var actaNovaMessageBox = new ActaNovaMessageBox (messageBoxScope.Id, context.CloneForScope (messageBoxScope));
      actaNovaMessageBox.Okay();
      return UnspecifiedPage();
    }

    // Todo RM-6297: Code duplication with ANMainPageObject.
    private ActaNovaDetailsArea DetailsArea
    {
      get
      {
        var detailsAreaScope = Scope.FindFrame ("RightFrameContent");
        return new ActaNovaDetailsArea (detailsAreaScope.Id, Context.CloneForFrame (detailsAreaScope));
      }
    }
  }
}