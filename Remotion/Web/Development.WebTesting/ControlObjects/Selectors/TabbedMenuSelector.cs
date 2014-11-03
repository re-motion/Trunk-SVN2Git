using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="TabbedMenuControlObject"/>.
  /// </summary>
  public class TabbedMenuSelector : TypedControlSelectorBase<TabbedMenuControlObject>
  {
    public TabbedMenuSelector ()
        : base ("TabbedMenu")
    {
    }

    protected override TabbedMenuControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new TabbedMenuControlObject (newControlObjectContext);
    }
  }
}