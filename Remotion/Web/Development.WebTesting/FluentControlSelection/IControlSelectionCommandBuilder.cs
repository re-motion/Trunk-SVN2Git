using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Interface for all control selection command builders.
  /// </summary>
  /// <typeparam name="TControlSelector">The control selector to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public interface IControlSelectionCommandBuilder<in TControlSelector, out TControlObject>
      where TControlSelector : IControlSelector
      where TControlObject : ControlObject
  {
    IControlSelectionCommand<TControlObject> Using ([NotNull] TControlSelector strategy);
  }
}