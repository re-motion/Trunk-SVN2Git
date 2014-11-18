using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing an arbitrary, "non-special" ActaNova-based page.
  /// </summary>
  public class ActaNovaPageObject : ActaNovaPageObjectBase, IWebTestObjectWithWebButtons
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public ActaNovaPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Perform (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      var fullItemID = string.Format ("{0}Button", itemID);
      var webButton = GetControl (new ItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), fullItemID));
      return webButton.Click (completionDetection, modalDialogHandler);
    }
  }
}