using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="PerHtmlIDControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IPerHtmlIDControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerHtmlIDControlSelectionCommandBuilder<TControlSelector, TControlObject> : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IPerHtmlIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _htmlID;

    public PerHtmlIDControlSelectionCommandBuilder ([NotNull] string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      _htmlID = htmlID;
    }

    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new PerHtmlIDControlSelectionCommand<TControlObject> (controlSelector, _htmlID);
    }
  }
}