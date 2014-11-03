using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of selectable sub items (e.g. a sub menu).
  /// </summary>
  public interface IControlObjectWithSelectableSubItems
  {
    IControlObjectWithSelectableSubItems SelectSubItem ();
    UnspecifiedPageObject SelectSubItem ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);

    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithIndex (int index, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithHtmlID ([NotNull] string htmlID, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}