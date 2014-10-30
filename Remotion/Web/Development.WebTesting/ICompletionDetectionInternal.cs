using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Internally used interface in order to execute an <see cref="ICompletionDetection"/> properly.
  /// </summary>
  public interface ICompletionDetectionInternal
  {
    /// <summary>
    /// Called immediately before the action has been performed.
    /// </summary>
    /// <returns>An arbitrary state object which is passed to the <see cref="AfterAction"/> method.</returns>
    object BeforeAction ([NotNull] TestObjectContext context);

    /// <summary>
    /// Called immediately after the action has been performed. The object returned by <see cref="BeforeAction"/> is passed as <paramref name="state"/>.
    /// </summary>
    void AfterAction ([NotNull] TestObjectContext context, [NotNull] object state);
  }
}