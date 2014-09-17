using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="TabbedMultiView"/>.
  /// </summary>
  public class TabbedMultiViewControlObject : RemotionControlObject, IControlHost
  {
    public TabbedMultiViewControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public TabStripControlObject GetTabStrip()
    {
      // Todo RM-6297: use constants from TabbedMultiView instead of magic strings? Refactoring safetey, etc. Talk to MK about this.
      var scope = FindChild ("TabStrip");
      return new TabStripControlObject(scope.Id, "_Tab", Context.CloneForScope(scope));
    }

    public ScopeControlObject GetTopControls()
    {
      var scope = FindChild ("TopControl");
      return new ScopeControlObject(scope.Id, Context.CloneForScope(scope));
    }

    public ScopeControlObject GetActiveView()
    {
      var scope = FindChild ("ActiveView");
      return new ScopeControlObject(scope.Id, Context.CloneForScope(scope));
    }

    public ScopeControlObject GetBottomControls()
    {
      var scope = FindChild ("BottomControl");
      return new ScopeControlObject(scope.Id, Context.CloneForScope(scope));
    }

    public ControlObject GetControl<TControlSelectionParameters> (
        IControlSelector<TControlSelectionParameters> selector,
        TControlSelectionParameters selectionParameters) where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindControl (Context, selectionParameters);
    }

    public TControlObject GetControl<TControlObject, TControlSelectionParameters> (
        IControlSelector<TControlObject, TControlSelectionParameters> selector,
        TControlSelectionParameters selectionParameters) where TControlObject : ControlObject
        where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindTypedControl (Context, selectionParameters);
    }
  }
}