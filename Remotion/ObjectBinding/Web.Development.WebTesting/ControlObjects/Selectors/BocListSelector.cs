using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocListControlObject"/>.
  /// </summary>
  public class BocListSelector : BocSelectorBase<BocListControlObject>
  {
    public BocListSelector ()
        : base ("div", "bocList")
    {
    }
  }
}