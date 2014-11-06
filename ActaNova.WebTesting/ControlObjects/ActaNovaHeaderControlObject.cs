using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova header area.
  /// </summary>
  public class ActaNovaHeaderControlObject : ActaNovaMainFrameControlObject, IControlHost
  {
    public ActaNovaHeaderControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the current ActaNova application context or <see langword="null" /> if no application context is active.
    /// </summary>
    /// <remarks>
    /// ActaNova currently displays the application context as "(Verfahrensbereich BA)", this property returns only "Verfahrensbereich BA".
    /// </remarks>
    public string GetCurrentApplicationContext ()
    {
      var currentApplicationContextLabel = GetControl (
          new PerHtmlIDControlSelectionCommand<LabelControlObject> (new LabelSelector(), "CurrentAppContextLabel"));

      var currentApplicationContext = currentApplicationContextLabel.GetText();

      if (!currentApplicationContext.StartsWith ("("))
        return null;

      return currentApplicationContext.Substring (1, currentApplicationContext.Length - 2);
    }

    /// <summary>
    /// Returns the current ActaNova user name.
    /// </summary>
    public string GetCurrentUser ()
    {
      var userAndGroup = GetUserAndGroup();

      var startIndexOfGroup = userAndGroup.LastIndexOf ('(');
      Assertion.IsTrue (startIndexOfGroup >= 0, "Current user and group label is not correctly formatted.");

      return userAndGroup.Substring (0, startIndexOfGroup - 1);
    }

    /// <summary>
    /// Returns the current ActaNova group name.
    /// </summary>
    public string GetCurrentGroup ()
    {
      var userAndGroup = GetUserAndGroup();

      var startIndexOfGroup = userAndGroup.LastIndexOf ('(') + 1;
      Assertion.IsTrue (startIndexOfGroup >= 0, "Current user and group label is not correctly formatted.");

      return userAndGroup.Substring (startIndexOfGroup, userAndGroup.Length - startIndexOfGroup - 1);
    }

    private string GetUserAndGroup ()
    {
      var currentUserAndGroup = GetControl (
          new PerHtmlIDControlSelectionCommand<BocReferenceValueControlObject> (
              new BocReferenceValueSelector(),
              "SecurityManagerCurrentTenantControl_CurrentUserField"));

      return currentUserAndGroup.GetText();
    }

    /// <summary>
    /// Opens and returns the default group control when the default value is displayed.
    /// </summary>
    public BocReferenceValueControlObject OpenDefaultGroupControlWhenStandardIsDisplayed ()
    {
      return GetDefaultGroupControlUsingOpenButton ("SecurityManagerCurrentTenantControl_NoDefaultGroupButton", false);
    }

    /// <summary>
    /// Opens and returns the default group control when a non-default value is displayed.
    /// </summary>
    public BocReferenceValueControlObject OpenDefaultGroupControl ()
    {
      return GetDefaultGroupControlUsingOpenButton ("SecurityManagerCurrentTenantControl_DefaultGroupField_Command", true);
    }

    private BocReferenceValueControlObject GetDefaultGroupControlUsingOpenButton (string buttonId, bool isCommand)
    {
      if (isCommand)
      {
        var openButton = GetControl (
            new PerHtmlIDControlSelectionCommand<CommandControlObject> (
                new CommandSelector(),
                buttonId));
        openButton.Click();
      }
      else
      {
        var openButtonScope = Scope.FindId (buttonId);
        openButtonScope.Now();

        // Do not use ClickAndWait() here, it uses FocusClick() internally, which fails (at least using Chrome) for unknown reasons.
        openButtonScope.PerformAction (s => s.Click(), Context, Continue.When (Wxe.PostBackCompleted).Build());
      }

      return GetControl (
          new PerHtmlIDControlSelectionCommand<BocReferenceValueControlObject> (
              new BocReferenceValueSelector(),
              "SecurityManagerCurrentTenantControl_DefaultGroupField"));
    }

    /// <summary>
    /// Opens and returns the current tenant control.
    /// </summary>
    public BocReferenceValueControlObject OpenCurrentTenantControl ()
    {
      var openButton = GetControl (
          new PerHtmlIDControlSelectionCommand<CommandControlObject> (
              new CommandSelector(),
              "SecurityManagerCurrentTenantControl_CurrentTenantField_Command"));
      openButton.Click();

      return GetControl (
          new PerHtmlIDControlSelectionCommand<BocReferenceValueControlObject> (
              new BocReferenceValueSelector(),
              "SecurityManagerCurrentTenantControl_CurrentTenantField"));
    }

    /// <summary>
    /// Returns the list of currently displayed ActaNova bread crumbs.
    /// </summary>
    public IReadOnlyList<ActaNovaBreadCrumbControlObject> GetBreadCrumbs ()
    {
      var breadCrumbsScope = Scope.FindId ("BreadCrumbsLabel");
      return RetryUntilTimeout.Run (
          () => breadCrumbsScope.FindAllCss (".breadCrumbLink")
              .Select (s => new ActaNovaBreadCrumbControlObject (Context.CloneForControl (s)))
              .ToList());
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}