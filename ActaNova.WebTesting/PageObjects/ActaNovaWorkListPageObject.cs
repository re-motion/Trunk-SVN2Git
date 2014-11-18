using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.ControlObjects.Selectors;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing the ActaNova work list page.
  /// </summary>
  public class ActaNovaWorkListPageObject : ActaNovaPageObject
  {
    public ActaNovaWorkListPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public override string GetTitle ()
    {
      return Scope.FindCss (".formPageTitleLabel").Text;
    }

    public ActaNovaListControlObject GetWorkList ()
    {
      return GetControl (new SingleControlSelectionCommand<ActaNovaListControlObject> (new ActaNovaListSelector()));
    }

    public DropDownMenuControlObject GetWorkStepMenu ()
    {
      return GetControl (new HtmlIDControlSelectionCommand<DropDownMenuControlObject> (new DropDownMenuSelector(), "MultiItemWorkStepMenu"));
    }

    public HtmlPageObject OpenRssFeed ()
    {
      throw new NotSupportedException (
          "OpenRssFeed cannot be implemented at the moment due to technical reasons: the new window does not have a title.");

#pragma warning disable 162 // unreachable code, can be dropped as soon as NotSupportedException has been removed.
      var openRssFeedButton = GetControl (new HtmlIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "OpenRssFeedLink"));
      return openRssFeedButton.Click (Continue.Immediately()).ExpectNewWindow<HtmlPageObject> ("");
#pragma warning restore 162
    }

    public HtmlPageObject OpenHelp ()
    {
      throw new NotSupportedException ("OpenHelp cannot be implemented at the moment due to technical reasons: the new window does not have a title.");

#pragma warning disable 162 // unreachable code, can be dropped as soon as NotSupportedException has been removed.
      var openHelpButton = GetControl (new HtmlIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "OnlineHelpLink"));
      return openHelpButton.Click (Continue.Immediately()).ExpectNewWindow<HtmlPageObject> ("");
#pragma warning restore 162
    }

    public ActaNovaWindowPageObject CreateBugReport ()
    {
      var createBugReportButton =
          GetControl (new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "BugReportButton"));
      return createBugReportButton.Click().ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
    }
  }
}