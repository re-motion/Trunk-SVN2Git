using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Interface for <see cref="FluentControlSelector{TControlSelector,TControlObject}"/>, whose only job is to allow <see cref="GetControl"/> to
  /// be implemented explicitly and therefore prevent IntelliSense polluting.
  /// </summary>
  public interface IFluentControlSelector<out TControlSelector, TControlObject>
      where TControlSelector : IControlSelector
      where TControlObject : ControlObject
  {
    TControlObject GetControl ([NotNull] IControlSelectionCommandBuilder<TControlSelector, TControlObject> selectionCommandBuilder);
  }
}