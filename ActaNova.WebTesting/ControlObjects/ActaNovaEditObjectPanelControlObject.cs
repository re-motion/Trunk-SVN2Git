using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova edit object panel.
  /// </summary>
  public class ActaNovaEditObjectPanelControlObject : ScopeControlObject, IWebTestObjectWithWebButtons
  {
    public ActaNovaEditObjectPanelControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Perform (string itemID, ICompletionDetection completionDetection = null)
    {
      var fullItemID = string.Format ("{0}Button", itemID);
      var webButton = GetControl (new PerItemIDControlSelectionCommand<WebButtonControlObject> (new WebButtonSelector(), fullItemID));
      return webButton.Click (completionDetection);
    }
  }
}