using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type bearing the given display name
  /// within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerDisplayNameControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerDisplayNameControlSelector<TControlObject> _controlSelector;
    private readonly string _displayName;

    public PerDisplayNameControlSelectionCommand ([NotNull] IPerDisplayNameControlSelector<TControlObject> controlSelector, string displayName)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _controlSelector = controlSelector;
      _displayName = displayName;
    }

    public TControlObject Select (TestObjectContext context)
    {
      return _controlSelector.SelectPerDisplayName (context, _displayName);
    }
  }
}