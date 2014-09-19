using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerLocalIDControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerLocalIDControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerLocalIDControlSelectionCommandBuilder<TControlSelector, TControlObject> : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerLocalIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _localID;

    public PerLocalIDControlSelectionCommandBuilder ([NotNull] string localID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      _localID = localID;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerLocalIDControlSelectionCommand<TControlObject> (controlSelector, _localID);
    }
  }
}