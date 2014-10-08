using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerTextControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerTextControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerTextControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerTextControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _text;

    public PerTextControlSelectionCommandBuilder ([NotNull] string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      _text = text;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerTextControlSelectionCommand<TControlObject> (controlSelector, _text);
    }
  }
}