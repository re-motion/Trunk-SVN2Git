using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// This control object represents controls (or areas within a control) which do not have any additional features than hosting other
  /// controls (<see cref="IControlHost"/>). Typcially this control object is returned by other control objects in order to scope into a specific
  /// area (e.g. top controls or bottom controls in <see cref="TabbedMultiViewControlObject"/>.
  /// </summary>
  public class ScopeControlObject : RemotionControlObject, IControlHost
  {
    public ScopeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    // Todo RM-6297: ControlHostingRemotionControlObject to remove code duplication with other IControlHost implementations?
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return controlSelectionCommand.Select (Context);
    }
  }
}