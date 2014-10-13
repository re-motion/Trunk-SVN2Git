using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing something clickable.
  /// </summary>
  public interface IClickableControlObject
  {
    /// <summary>
    /// Clicks the link, using a given <paramref name="actionBehavior"/> to wait for the triggered action's results.
    /// </summary>
    /// <param name="actionBehavior">Required <see cref="IActionBehavior"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject Click (IActionBehavior actionBehavior = null);
  }
}