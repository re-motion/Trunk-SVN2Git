using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="SingleViewControlObject"/>.
  /// </summary>
  public class TabbedMultiViewSelector : RemotionControlSelectorBase<TabbedMultiViewControlObject>
  {
    public TabbedMultiViewSelector ()
        : base ("div", "tabbedMultiView")
    {
    }
  }
}