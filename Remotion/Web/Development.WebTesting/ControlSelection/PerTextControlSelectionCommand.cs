using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given text within the
  /// given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerTextControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerTextControlSelector<TControlObject> _controlSelector;
    private readonly string _text;

    public PerTextControlSelectionCommand (
        [NotNull] IPerTextControlSelector<TControlObject> controlSelector,
        [NotNull] string text)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", text);
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      _controlSelector = controlSelector;
      _text = text;
    }

    public TControlObject Select (ControlSelectionContext context)
    {
      return _controlSelector.SelectPerText (context, _text);
    }
  }
}