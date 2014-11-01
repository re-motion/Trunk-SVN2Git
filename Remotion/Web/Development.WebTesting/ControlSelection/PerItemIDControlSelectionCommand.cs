using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given item ID
  /// within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerItemIDControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerItemIDControlSelector<TControlObject> _controlSelector;
    private readonly string _itemID;

    public PerItemIDControlSelectionCommand (
        [NotNull] IPerItemIDControlSelector<TControlObject> controlSelector,
        [NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      _controlSelector = controlSelector;
      _itemID = itemID;
    }

    public TControlObject Select (WebTestObjectContext context)
    {
      return _controlSelector.SelectPerItemID (context, _itemID);
    }
  }
}