using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Interface for <see cref="IControlSelector"/> implementations which provide the possibility to select the first control matching their supported
  /// type of <typeparamref name="TControlObject"/>.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IFirstControlSelector<out TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Selects the first control within the given <paramref name="context"/> matching the type <typeparamref name="TControlObject"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If no matching control can be found.</exception>
    TControlObject SelectFirst ([NotNull] WebTestObjectContext context);
  }
}