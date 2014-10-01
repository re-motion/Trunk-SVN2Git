using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerCommandNameControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerCommandNameControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerCommandNameControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerCommandNameControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _commandName;

    public PerCommandNameControlSelectionCommandBuilder ([NotNull] string commandName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandName", commandName);

      _commandName = commandName;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerCommandNameControlSelectionCommand<TControlObject> (controlSelector, _commandName);
    }
  }
}