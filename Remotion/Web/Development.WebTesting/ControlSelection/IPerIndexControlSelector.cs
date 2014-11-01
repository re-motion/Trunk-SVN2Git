using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select their supported
  /// type of <typeparamref name="TControlObject"/> via an index.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IPerIndexControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the control within the given <paramref name="context"/> using the given <paramref name="index"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If the control cannot be found.</exception>
    TControlObject SelectPerIndex ([NotNull] WebTestObjectContext context, int index);
  }
}