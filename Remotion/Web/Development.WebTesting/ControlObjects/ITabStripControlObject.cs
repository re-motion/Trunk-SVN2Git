using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a tab strip.
  /// </summary>
  public interface ITabStripControlObject
  {
    /// <summary>
    /// Switch to another tab using the tab's <paramref name="itemID"/>.
    /// </summary>
    /// <returns>An unspecified page object, it may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="AmbiguousException">If multiple tabs with the given <paramref name="itemID"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchTo (string itemID);

    /// <summary>
    /// Switch to the nth tab, specified by the <paramref name="index"/> parameter.
    /// </summary>
    /// <returns>An unspecified page object, it may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchTo (int index);

    /// <summary>
    /// Switch to another tab using the tab's full <paramref name="htmlID"/>. Use this only as a fallback mechanism if no other way is suitable, as
    /// HTML IDs tend to be brittle in ASP.NET.
    /// </summary>
    /// <returns>An unspecified page object, it may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchToByHtmlID (string htmlID);

    /// <summary>
    /// Switch to another tab using the tab's <paramref name="text"/>.
    /// </summary>
    /// <returns>An unspecified page object, it may be used in case a new page is expected after switching the tab.</returns>
    /// <exception cref="AmbiguousException">If multiple tabs with the given <paramref name="text"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If the tab cannot be found.</exception>
    UnspecifiedPageObject SwitchToByText (string text);
  }
}