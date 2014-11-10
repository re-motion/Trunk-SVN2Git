using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  public static class ActaNovaClickExtensions
  {
    public static UnspecifiedPageObject ClickAndConfirmDataLoss ([NotNull] this ActaNovaBreadCrumbControlObject breadCrumb)
    {
      ArgumentUtility.CheckNotNull ("breadCrumb", breadCrumb);

      var confirmPage = breadCrumb.Click (Continue.When (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      return confirmPage.Confirm (Continue.When (Wxe.PostBackCompletedIn (breadCrumb.Context.PageObject)));
    }

    public static UnspecifiedPageObject ClickAndPreventDataLoss ([NotNull] this ActaNovaBreadCrumbControlObject breadCrumb)
    {
      ArgumentUtility.CheckNotNull ("breadCrumb", breadCrumb);

      var confirmPage = breadCrumb.Click (Continue.When (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      return confirmPage.Cancel();
    }

    public static UnspecifiedPageObject SelectAndConfirmDataLoss ([NotNull] this ActaNovaTreeNodeControlObject treeNode)
    {
      ArgumentUtility.CheckNotNull ("treeNode", treeNode);

      var completionDetection = treeNode.IsSelected() ? null : Continue.When (Wxe.PostBackCompletedIn (treeNode.Context.PageObject));

      var confirmPage = treeNode.Select (Continue.When (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      return confirmPage.Confirm (completionDetection);
    }

    public static UnspecifiedPageObject SelectAndPreventDataLoss ([NotNull] this ActaNovaTreeNodeControlObject treeNode)
    {
      ArgumentUtility.CheckNotNull ("treeNode", treeNode);

      var confirmPage = treeNode.Select (Continue.When (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      return confirmPage.Cancel();
    }

    public static UnspecifiedPageObject PerformAndConfirmDataLoss ([NotNull] this AppToolsFormPageObject formPage, string itemID)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.Perform (
          itemID,
          Continue.When (Wxe.PostBackCompletedInContext (formPage.Context.ParentContext)).AndModalDialogHasBeenAccepted());
    }

    public static UnspecifiedPageObject PerformAndPreventDataLoss ([NotNull] this AppToolsFormPageObject formPage, string itemID)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.Perform (itemID, Continue.Immediately().AndModalDialogHasBeenCanceled());
    }
  }
}