using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocCheckBoxControlObject"/>.
  /// </summary>
  public class BocCheckBoxSelector : BocSelectorBase<BocCheckBoxControlObject>
  {
    public BocCheckBoxSelector ()
        : base ("span", "bocCheckBox")
    {
    }
  }
}