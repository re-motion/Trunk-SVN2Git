using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing something fillable.
  /// </summary>
  public interface IFillableControlObject
  {
    /// <summary>
    /// Fills the control with the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to fill in.</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);

    /// <summary>
    /// Fills the control with the given <paramref name="text"/> and afterwards executes the given <paramref name="finishInputWith"/> action.
    /// </summary>
    /// <param name="text">The text to fill in.</param>
    /// <param name="finishInputWith">What to do after the text has been filled in (see <see cref="FinishInput"/> for supported default options).</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject FillWith ([NotNull] string text, [NotNull] FinishInputWithAction finishInputWith, [CanBeNull] ICompletionDetection completionDetection = null);
  }
}