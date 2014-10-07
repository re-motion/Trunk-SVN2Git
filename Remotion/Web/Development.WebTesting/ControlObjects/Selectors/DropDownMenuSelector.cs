using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="DropDownMenuControlObject"/>.
  /// </summary>
  public class DropDownMenuSelector : RemotionControlSelectorBase<DropDownMenuControlObject>
  {
    public DropDownMenuSelector ()
        : base ("span", "DropDownMenuContainer")
    {
    }
  }
}