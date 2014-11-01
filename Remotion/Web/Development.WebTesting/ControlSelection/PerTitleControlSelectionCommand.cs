using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given title within the
  /// given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerTitleControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerTitleControlSelector<TControlObject> _controlSelector;
    private readonly string _title;

    public PerTitleControlSelectionCommand ([NotNull] IPerTitleControlSelector<TControlObject> controlSelector, [NotNull] string title)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("title", title);

      _controlSelector = controlSelector;
      _title = title;
    }

    public TControlObject Select (WebTestObjectContext context)
    {
      return _controlSelector.SelectPerTitle (context, _title);
    }
  }
}