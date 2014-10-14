using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebTreeViewControlObject"/>.
  /// </summary>
  public class WebTreeViewSelector : RemotionControlSelectorBase<WebTreeViewControlObject>
  {
    public WebTreeViewSelector ()
        : base ("div", "<not possible>") // Todo RM-6297: do something about <not possible>...e.g. do the re-factoring for data-control-type ;-)
    {
    }
  }
}