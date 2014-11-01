using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select their supported
  /// type of <typeparamref name="TControlObject"/> via their item ID.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IPerItemIDControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the control within the given <paramref name="context"/> using the given <paramref name="itemID"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="AmbiguousException">If multiple controls with the given <paramref name="itemID"/> are found.</exception>
    /// <exception cref="MissingHtmlException">If the control cannot be found.</exception>
    TControlObject SelectPerItemID ([NotNull] WebTestObjectContext context, [NotNull] string itemID);
  }
}