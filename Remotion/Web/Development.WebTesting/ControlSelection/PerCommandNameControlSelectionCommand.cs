using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given command name
  /// within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerCommandNameControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerCommandNameControlSelector<TControlObject> _controlSelector;
    private readonly string _commandName;

    public PerCommandNameControlSelectionCommand (
        [NotNull] IPerCommandNameControlSelector<TControlObject> controlSelector,
        [NotNull] string commandName)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("commandName", commandName);

      _controlSelector = controlSelector;
      _commandName = commandName;
    }

    public TControlObject Select (TestObjectContext context)
    {
      return _controlSelector.SelectPerCommandName (context, _commandName);
    }
  }
}