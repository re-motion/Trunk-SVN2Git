using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the nth control of the given <typeparamref name="TControlObject"/> type within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerIndexControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerIndexControlSelector<TControlObject> _controlSelector;
    private readonly int _index;

    public PerIndexControlSelectionCommand ([NotNull] IPerIndexControlSelector<TControlObject> controlSelector, int index)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      _controlSelector = controlSelector;
      _index = index;
    }

    public TControlObject Select (ControlSelectionContext context)
    {
      return _controlSelector.SelectPerIndex (context, _index);
    }
  }
}