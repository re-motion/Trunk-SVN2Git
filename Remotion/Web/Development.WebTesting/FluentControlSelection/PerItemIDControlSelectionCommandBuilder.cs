using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerItemIDControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerItemIDControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerItemIDControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerItemIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _itemID;

    public PerItemIDControlSelectionCommandBuilder ([NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      _itemID = itemID;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerItemIDControlSelectionCommand<TControlObject> (controlSelector, _itemID);
    }
  }
}