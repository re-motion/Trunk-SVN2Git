using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the only control of the given <typeparamref name="TControlObject"/> type within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class SingleControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly ISingleControlSelector<TControlObject> _controlSelector;

    public SingleControlSelectionCommand ([NotNull] ISingleControlSelector<TControlObject> controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      _controlSelector = controlSelector;
    }

    public TControlObject Select (ControlSelectionContext context)
    {
      return _controlSelector.SelectSingle (context);
    }
  }
}