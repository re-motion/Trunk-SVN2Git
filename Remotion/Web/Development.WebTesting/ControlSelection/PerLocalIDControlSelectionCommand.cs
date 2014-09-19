using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given local ID within
  /// the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerLocalIDControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerLocalIDControlSelector<TControlObject> _controlSelector;
    private readonly string _localID;

    public PerLocalIDControlSelectionCommand ([NotNull] IPerLocalIDControlSelector<TControlObject> controlSelector, [NotNull] string localID)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      _controlSelector = controlSelector;
      _localID = localID;
    }

    public TControlObject Select (TestObjectContext context)
    {
      return _controlSelector.SelectPerLocalID (context, _localID);
    }
  }
}