using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerDisplayNameControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerDisplayNameControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerDisplayNameControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerDisplayNameControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _displayName;

    public PerDisplayNameControlSelectionCommandBuilder ([NotNull] string displayName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _displayName = displayName;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerDisplayNameControlSelectionCommand<TControlObject> (controlSelector, _displayName);
    }
  }
}