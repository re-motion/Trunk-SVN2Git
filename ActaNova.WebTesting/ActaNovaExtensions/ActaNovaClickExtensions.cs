using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific extension methods for clicking/selecting controls.
  /// </summary>
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

    public static UnspecifiedPageObject PerformAndConfirmDataLoss ([NotNull] this ActaNovaFormPageObject page, [NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return page.Perform (itemID, Continue.When (Wxe.PostBackCompletedInContext (page.Context.ParentContext)), HandleModalDialog.Accept());
    }

    public static UnspecifiedPageObject PerformAndPreventDataLoss ([NotNull] this ActaNovaFormPageObject page, [NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return page.Perform (itemID, Continue.Immediately(), HandleModalDialog.Cancel());
    }

    public static UnspecifiedPageObject PerformAndCloseWindow ([NotNull] this ActaNovaPageObject page, [NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return page.Perform (itemID, Continue.When (Wxe.PostBackCompletedInContext(page.Context.ParentContext)));
    }
  }
}