using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripSelector : TypedControlSelectorBase<WebTabStripControlObject>
  {
    public WebTabStripSelector ()
        : base ("WebTabStrip")
    {
    }

    protected override WebTabStripControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new WebTabStripControlObject (newControlObjectContext);
    }
  }
}