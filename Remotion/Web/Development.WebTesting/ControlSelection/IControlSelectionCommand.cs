using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Base interface for all <typeparamref name="TControlObject"/> selection commands. A selection command encapsualtes an implementation
  /// of <see cref="IControlSelector"/> and all necessary selection parameters.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IControlSelectionCommand<out TControlObject>
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Performs the selection within the given <paramref name="context"/>.
    /// </summary>
    /// <returns>The control object.</returns>
    /// <exception cref="AmbiguousException">If the selection command cannot unambiguously identify the control.</exception>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
    TControlObject Select ([NotNull] WebTestObjectContext context);
  }
}