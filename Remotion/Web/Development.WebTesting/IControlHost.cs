using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Interface for all <see cref="TestObject"/>s which host controls within their scope.
  /// </summary>
  public interface IControlHost
  {
    /// <summary>
    /// Tries to find a <see cref="ControlObject"/> within the scope of the control host, using the given <paramref name="controlSelectionCommand"/>.
    /// </summary>
    /// <typeparam name="TControlObject">The type of the control to be found.</typeparam>
    /// <param name="controlSelectionCommand">Encapsulating the <see cref="IControlSelector"/> implementation and all necessary selection parameters.</param>
    /// <returns>The control object.</returns>
    /// <exception cref="AmbiguousException">If the selection command cannot unambiguously identify the control.</exception>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
    TControlObject GetControl<TControlObject> ([NotNull] IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject;
  }
}