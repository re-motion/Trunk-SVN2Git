using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class TabStripSelector : RemotionControlSelectorBase<TabStripControlObject>
  {
    public TabStripSelector ()
        : base ("div", "tabStrip")
    {
    }
  }
}