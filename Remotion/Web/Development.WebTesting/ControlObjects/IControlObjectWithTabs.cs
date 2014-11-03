using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a tab strip.
  /// </summary>
  public interface IControlObjectWithTabs
  {
    IControlObjectWithTabs SwitchTo ();
    UnspecifiedPageObject SwitchTo ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);

    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithIndex (int index, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithHtmlID ([NotNull] string htmlID, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}