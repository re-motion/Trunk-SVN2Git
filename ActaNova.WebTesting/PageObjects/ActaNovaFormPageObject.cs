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
  /// Represents the ActaNova form page.
  /// </summary>
  public class ActaNovaFormPageObject : ActaNovaPageObject
  {
    public ActaNovaFormPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the form page's title.
    /// </summary>
    public override string GetTitle ()
    {
      return Scope.FindCss (".formPageTitleLabel").Text;
    }

    /// <summary>
    /// Presses the begin activity button.
    /// </summary>
    public UnspecifiedPageObject BeginActivity ()
    {
      var beginActivityButton =
          GetControl (new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "BeginActivityButton"));
      return beginActivityButton.Click();
    }

    /// <summary>
    /// Returns the name of the current activity as displayed on the form page.
    /// </summary>
    public string GetCurrentActivityName ()
    {
      var label = GetControl (new HtmlIDControlSelectionCommand<LabelControlObject> (new LabelSelector(), "NavigationControl_CurrentItemLabel"));
      return label.GetText();
    }

    /// <summary>
    /// Presses the previous activity button.
    /// </summary>
    public UnspecifiedPageObject PreviousActivity ()
    {
      var gotoPreviousButton =
          GetControl (
              new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "NavigationControl_GotoPreviousButton"));
      return gotoPreviousButton.Click (Continue.When (Wxe.PostBackCompletedInParent (this)));
    }

    /// <summary>
    /// Presses the next activity button.
    /// </summary>
    public UnspecifiedPageObject NextActivity ()
    {
      var gotoNextButton =
          GetControl (
              new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "NavigationControl_GotoNextButton"));
      return gotoNextButton.Click (Continue.When (Wxe.PostBackCompletedInParent (this)));
    }

    /// <summary>
    /// Hovers the work steps control on the form page and returns the appearing <see cref="ActaNovaTreePopupListControlObject"/>.
    /// </summary>
    public ActaNovaTreePopupListControlObject HoverWorkStepsControl ()
    {
      var workStepsControlScope = GetControl (new HtmlIDControlSelectionCommand<ScopeControlObject> (new ScopeSelector(), "WorkStepsControl"));
      workStepsControlScope.Scope.Hover();

      return workStepsControlScope.GetControl (
          new SingleControlSelectionCommand<ActaNovaTreePopupListControlObject> (new ActaNovaTreePopupListSelector()));
    }

    /// <summary>
    /// Presses the pin (add to/remove from favorites) button.
    /// </summary>
    public UnspecifiedPageObject PressPinButton ()
    {
      var pinButton = GetControl (new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "PinButton"));
      return pinButton.Click (Continue.When (Wxe.PostBackCompletedInParent (this)));
    }

    /// <summary>
    /// Retrieves the permalink for the current form page.
    /// </summary>
    public string GetPermalink ()
    {
      var permalinkButton = GetControl (new HtmlIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "PermalinkButton"));
      permalinkButton.Click (Continue.Immediately(), HandleModalDialog.Accept());

      var permalink = permalinkButton.Scope["href"].Replace ("/?", "/Main.wxe?");
      return permalink;
    }

    /// <summary>
    /// Presses the print button.
    /// </summary>
    public /*ActaNovaPrintPageObject*/ ActaNovaPageObject Print ()
    {
      var printButton = GetControl (new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "PrintButton"));
      return printButton.Click().ExpectNewWindow< /*ActaNovaPrintPageObject*/ ActaNovaPageObject> ("Untitled Page");
    }

    /// <summary>
    /// Presses the open help button.
    /// </summary>
    public HtmlPageObject OpenHelp ()
    {
      throw new NotSupportedException ("OpenHelp cannot be implemented at the moment due to technical reasons: the new window does not have a title.");

#pragma warning disable 162 // unreachable code, can be dropped as soon as NotSupportedException has been removed.
      var openHelpButton = GetControl (new HtmlIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "OnlineHelpLink"));
      return openHelpButton.Click (Continue.Immediately()).ExpectNewWindow<HtmlPageObject> ("");
#pragma warning restore 162
    }

    /// <summary>
    /// Presses the create bug report button.
    /// </summary>
    public ActaNovaWindowPageObject CreateBugReport ()
    {
      var bugReportButton = GetControl (new HtmlIDControlSelectionCommand<ImageButtonControlObject> (new ImageButtonSelector(), "BugReportButton"));
      return bugReportButton.Click().ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
    }

    /// <summary>
    /// Allows to interact with the form page's additional commands menu.
    /// </summary>
    public DropDownMenuControlObject GetAdditionalCommandsMenu ()
    {
      var additionalCommandsMenu =
          GetControl (
              new HtmlIDControlSelectionCommand<DropDownMenuControlObject> (new DropDownMenuSelector(), "AdditionalCommandsMenu_Boc_DropDownMenu"));
      return additionalCommandsMenu;
    }

    /// <inheritdoc/>
    protected override ICompletionDetection GetDefaultCompletionDetectionForPerform ()
    {
      return Continue.When (Wxe.PostBackCompletedInParent (this));
    }
  }
}