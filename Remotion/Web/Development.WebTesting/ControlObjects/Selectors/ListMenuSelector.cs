using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ListMenuControlObject"/>.
  /// </summary>
  public class ListMenuSelector : RemotionControlSelectorBase<ListMenuControlObject>
  {
    public ListMenuSelector ()
        : base ("table", "listMenu")
    {
    }
  }
}