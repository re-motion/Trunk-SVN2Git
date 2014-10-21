using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing something selectable.
  /// </summary>
  public interface ISelectableControlObject
  {
    /// <summary>
    /// Selects an option of the control using the given <paramref name="itemID"/>.
    /// </summary>
    /// <param name="itemID">The item ID of the option to select.</param>
    /// <param name="actionBehavior">Required <see cref="IActionBehavior"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject SelectOption ([NotNull] string itemID, [CanBeNull] IActionBehavior actionBehavior = null);

    /// <summary>
    /// Selects an option of the control using the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the option to select.</param>
    /// <param name="actionBehavior">Required <see cref="IActionBehavior"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject SelectOption (int index, [CanBeNull] IActionBehavior actionBehavior = null);

    /// <summary>
    /// Selects an option of the control using the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text of the option to select.</param>
    /// <param name="actionBehavior">Required <see cref="IActionBehavior"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject SelectOptionByText ([NotNull] string text, [CanBeNull] IActionBehavior actionBehavior = null);
  }
}