using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMultiView"/>.
  /// </summary>
  [UsedImplicitly]
  public class TabbedMultiViewControlObject : RemotionControlObject, IControlHost, ITabStripControlObject
  {
    public TabbedMultiViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public ScopeControlObject GetTopControls ()
    {
      var scope = Scope.FindChild ("TopControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetActiveView ()
    {
      var scope = Scope.FindChild ("ActiveView");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetBottomControls ()
    {
      var scope = Scope.FindChild ("BottomControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public UnspecifiedPageObject SwitchTo (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetTabStrip().SwitchTo (itemID + "_Tab");
    }

    public UnspecifiedPageObject SwitchTo (int index)
    {
      return GetTabStrip().SwitchTo (index);
    }

    public UnspecifiedPageObject SwitchToByHtmlID (string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return GetTabStrip().SwitchToByHtmlID (htmlID);
    }

    public UnspecifiedPageObject SwitchToByText (string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return GetTabStrip().SwitchToByText (text);
    }

    private WebTabStripControlObject GetTabStrip ()
    {
      var scope = Scope.FindChild ("TabStrip");
      return new WebTabStripControlObject (Context.CloneForControl (scope));
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}