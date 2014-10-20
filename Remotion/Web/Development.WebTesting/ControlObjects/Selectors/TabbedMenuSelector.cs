using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="TabbedMenuControlObject"/>.
  /// </summary>
  public class TabbedMenuSelector : RemotionControlSelectorBase<TabbedMenuControlObject>
  {
    public TabbedMenuSelector ()
        : base ("table", "tabbedMenu")
    {
    }
  }
}