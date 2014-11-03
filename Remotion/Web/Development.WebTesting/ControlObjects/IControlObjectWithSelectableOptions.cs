using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of selectable options.
  /// </summary>
  public interface IControlObjectWithSelectableOptions
  {
    IControlObjectWithSelectableOptions SelectOption ();
    UnspecifiedPageObject SelectOption ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);

    UnspecifiedPageObject WithItemID ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithIndex (int index, [CanBeNull] ICompletionDetection completionDetection = null);
    UnspecifiedPageObject WithText ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}