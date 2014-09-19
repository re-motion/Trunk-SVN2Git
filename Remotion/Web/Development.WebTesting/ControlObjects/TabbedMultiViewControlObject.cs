using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="TabbedMultiView"/>.
  /// </summary>
  public class TabbedMultiViewControlObject : RemotionControlObject, IControlHost, ITabStrip
  {
    public TabbedMultiViewControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public ScopeControlObject GetTopControls ()
    {
      var scope = FindChild ("TopControl");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    public ScopeControlObject GetActiveView ()
    {
      var scope = FindChild ("ActiveView");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    public ScopeControlObject GetBottomControls ()
    {
      var scope = FindChild ("BottomControl");
      return new ScopeControlObject (scope.Id, Context.CloneForScope (scope));
    }

    public UnspecifiedPageObject SwitchTo (string localID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      return GetTabStrip().SwitchTo (localID + "_Tab");
    }

    public UnspecifiedPageObject SwitchToByLabel (string label)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("label", label);

      return GetTabStrip().SwitchToByLabel (label);
    }

    private TabStripControlObject GetTabStrip ()
    {
      var scope = FindChild ("TabStrip");
      return new TabStripControlObject (scope.Id, Context.CloneForScope (scope));
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