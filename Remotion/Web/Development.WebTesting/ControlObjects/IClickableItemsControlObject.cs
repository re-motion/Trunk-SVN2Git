using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of clickable items (e.g. a menu).
  /// </summary>
  public interface IClickableItemsControlObject
  {
    /// <summary>
    /// Click the item given by the <paramref name="itemID"/> parameter.
    /// </summary>
    UnspecifiedPageObject ClickItem ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);

    /// <summary>
    /// Click the nth item, specified by the <paramref name="index"/> parameter.
    /// </summary>
    UnspecifiedPageObject ClickItem (int index, [CanBeNull] ICompletionDetection completionDetection = null);

    /// <summary>
    /// Click the item given by its full <paramref name="htmlID"/>. Use this only as a fallback mechanism if no other way is suitable, as HTML IDs
    /// tend to be brittle in ASP.NET.
    /// </summary>
    UnspecifiedPageObject ClickItemByHtmlID ([NotNull] string htmlID, [CanBeNull] ICompletionDetection completionDetection = null);

    /// <summary>
    /// Click the item given by its <paramref name="text"/>.
    /// </summary>
    UnspecifiedPageObject ClickItemByText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}