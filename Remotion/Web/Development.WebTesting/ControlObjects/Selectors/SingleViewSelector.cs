using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="SingleViewControlObject"/>.
  /// </summary>
  public class SingleViewSelector : RemotionControlSelectorBase<SingleViewControlObject>
  {
    public SingleViewSelector ()
        : base ("div", "singleView")
    {
    }
  }
}