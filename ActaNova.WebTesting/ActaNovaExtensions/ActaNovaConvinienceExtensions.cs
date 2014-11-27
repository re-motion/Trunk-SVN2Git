using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific extension methods for more convinience.
  /// </summary>
  public static class ActaNovaConvinienceExtensions
  {
    public static ActaNovaMainPageObject OpenWorkListItem ([NotNull] this BocListRowControlObject workListItem)
    {
      ArgumentUtility.CheckNotNull ("workListItem", workListItem);

      var cell = workListItem.GetCell ("WorkItem");
      return cell.OpenWorkListItem();
    }

    public static ActaNovaMainPageObject OpenWorkListItem ([NotNull] this BocListCellControlObject workListItem)
    {
      ArgumentUtility.CheckNotNull ("workListItem", workListItem);

      return workListItem.ExecuteCommand (Opt.ContinueWhen (Wxe.PostBackCompletedInParent (workListItem.Context.PageObject))).ExpectMainPage();
    }

    public static ActaNovaMainPageObject SwitchTo ([NotNull] this ActaNovaFormPageObject formPage, [NotNull] string tabItemID)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);
      ArgumentUtility.CheckNotNullOrEmpty ("tabItemID", tabItemID);

      return formPage.GetOnlyTabbedMultiView().SwitchTo (tabItemID, Opt.ContinueWhen (Wxe.PostBackCompletedInParent (formPage))).ExpectMainPage();
    }

    public static ActaNovaMainPageObject SetApplicationContextTo ([NotNull] this ActaNovaFormPageObject formPage, [NotNull] string applicationContext)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);
      ArgumentUtility.CheckNotNull ("applicationContext", applicationContext);

      var applicationContextAutoComplete = formPage.GetAutoComplete ("ApplicationContext");
      return
          applicationContextAutoComplete.FillWith (
              applicationContext,
              FinishInput.WithTab,
              Opt.ContinueWhen (Wxe.PostBackCompletedInParent (formPage)))
              .ExpectMainPage();
    }
  }
}