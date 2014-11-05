using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> or <see cref="PageObject"/> implementations hosting a collection of
  /// <see cref="WebButtonControlObject"/>s.
  /// </summary>
  public interface IWebTestObjectWithWebButtons
  {
     /// <summary>
    /// Presses the button given by <paramref name="itemID"/>, using a given <paramref name="completionDetection"/> to wait for the triggered
    /// action's results.
    /// </summary>
    /// <param name="itemID">The button's item ID without the trailing "Button", e.g. "Save" for "SaveButton".</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject Perform ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}