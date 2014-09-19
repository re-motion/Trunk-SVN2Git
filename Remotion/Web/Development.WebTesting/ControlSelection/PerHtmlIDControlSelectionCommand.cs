using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given ID within the
  /// given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerHtmlIDControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerHtmlIDControlSelector<TControlObject> _controlSelector;
    private readonly string _htmlID;

    public PerHtmlIDControlSelectionCommand ([NotNull] IPerHtmlIDControlSelector<TControlObject> controlSelector, [NotNull] string htmlID)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      _controlSelector = controlSelector;
      _htmlID = htmlID;
    }

    public TControlObject Select (TestObjectContext context)
    {
      return _controlSelector.SelectPerHtmlID (context, _htmlID);
    }
  }
}