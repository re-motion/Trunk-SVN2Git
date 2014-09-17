using System;
using Coypu;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Interface for all <see cref="TestObject"/>s which host controls within their scope.
  /// </summary>
  public interface IControlHost
  {
    /// <summary>
    /// Tries to find a <see cref="ControlObject"/> within the scope of the control host, using the given <paramref name="selector"/> and the given
    /// <paramref name="selectionParameters"/>.
    /// </summary>
    /// <typeparam name="TControlSelectionParameters">The control selection parameters type required for this control selector.</typeparam>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
    ControlObject GetControl<TControlSelectionParameters> (
        [NotNull] IControlSelector<TControlSelectionParameters> selector,
        [NotNull] TControlSelectionParameters selectionParameters)
        where TControlSelectionParameters : ControlSelectionParameters;

    /// <summary>
    /// Tries to find a <see cref="ControlObject"/> within the scope of the control host, using the given <paramref name="selector"/> and the given
    /// <paramref name="selectionParameters"/>.
    /// </summary>
    /// <typeparam name="TControlObject">The type of the control to be found.</typeparam>
    /// <typeparam name="TControlSelectionParameters">The control selection parameters type required for this control selector.</typeparam>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
    TControlObject GetControl<TControlObject, TControlSelectionParameters> (
        [NotNull] IControlSelector<TControlObject, TControlSelectionParameters> selector,
        [NotNull] TControlSelectionParameters selectionParameters)
        where TControlObject : ControlObject
        where TControlSelectionParameters : ControlSelectionParameters;
  }
}