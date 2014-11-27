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
    public ActaNovaPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject Perform (string itemID, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      var actualActionOptions = MergeWithDefaultActionOptions (actionOptions);

      var fullItemID = string.Format ("{0}Button", itemID);
      var webButton = GetControl (new ItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), fullItemID));
      return webButton.Click (actualActionOptions);
    }

    /// <summary>
    /// Overrides the default <see cref="ICompletionDetectionStrategy"/> used by <see cref="Perform"/>. Return <see langword="null" /> if you want to
    /// keep the <see cref="WebButtonControlObject"/>'s default completion detection.
    /// </summary>
    protected virtual ICompletionDetectionStrategy GetDefaultCompletionDetectionForPerform ()
    {
      return null;
    }

    private IWebTestActionOptions MergeWithDefaultActionOptions (IWebTestActionOptions actionOptions)
    {
      if (actionOptions == null)
        actionOptions = new WebTestActionOptions();
      if (actionOptions.CompletionDetectionStrategy == null)
        actionOptions.CompletionDetectionStrategy = GetDefaultCompletionDetectionForPerform();

      return actionOptions;
    }
  }
}