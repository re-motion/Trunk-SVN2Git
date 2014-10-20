using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripSelector : RemotionControlSelectorBase<WebTabStripControlObject>
  {
    public WebTabStripSelector ()
        : base ("div", "tabStrip")
    {
    }
  }
}