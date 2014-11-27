using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova message box.
  /// </summary>
  public class ActaNovaMessageBoxControlObject : WebFormsControlObjectWithDiagnosticMetadata
  {
    public ActaNovaMessageBoxControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Confirms the ActaNova message box (presses the "OK" button on an ActaNova message box).
    /// </summary>
    public UnspecifiedPageObject Okay ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return ClickButton ("OK", actionOptions);
    }

    /// <summary>
    /// Cancels the ActaNova message box (presses the "Cancel" button on an ActaNova message box).
    /// </summary>
    public UnspecifiedPageObject Cancel ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return ClickButton ("Cancel", actionOptions);
    }

    /// <summary>
    /// Presses the "Yes" button on an ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject Yes ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return ClickButton ("Yes", actionOptions);
    }

    /// <summary>
    /// Presses the "No" button on an ActaNova message box.
    /// </summary>
    public UnspecifiedPageObject No ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return ClickButton ("No", actionOptions);
    }

    private UnspecifiedPageObject ClickButton (string buttonId, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var itemID = string.Format ("Popup{0}Button", buttonId);
      var webButton = Children.GetControl (new ItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), itemID));
      return webButton.Click (actionOptions);
    }
  }
}