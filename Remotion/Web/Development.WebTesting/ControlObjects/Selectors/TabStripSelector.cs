using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebTabStrip"/>.
  /// </summary>
  public class TabStripSelector : RemotionControlSelectorBase<TabStripControlObject>
  {
    public TabStripSelector ()
        : base ("div", "tabStrip")
    {
    }
  }
}