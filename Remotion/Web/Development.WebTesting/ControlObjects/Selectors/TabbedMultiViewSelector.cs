using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="TabbedMultiViewControlObject"/>.
  /// </summary>
  public class TabbedMultiViewSelector : TypedControlSelectorBase<TabbedMultiViewControlObject>
  {
    public TabbedMultiViewSelector ()
        : base ("TabbedMultiView")
    {
    }

    protected override TabbedMultiViewControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new TabbedMultiViewControlObject (newControlObjectContext);
    }
  }
}