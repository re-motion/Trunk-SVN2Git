using Coypu;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a tab strip.
  /// </summary>
  public interface ITabStrip
  {
    /// <summary>
    /// Switch to another tab using the tab's local ID.
    /// </summary>
    /// <param name="localID">The local ID of the tab.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="AmbiguousException">If multiple tabs with the given <paramref name="localID"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchTo (string localID);

    /// <summary>
    /// Switch to another tab using the tab's label.
    /// </summary>
    /// <param name="label">The label of the tab.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="AmbiguousException">If multiple tabs with the given <paramref name="label"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchToByLabel (string label);
  }
}