using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTreeViewControlObject"/>.
  /// </summary>
  public class BocTreeViewSelector : BocSelectorBase<BocTreeViewControlObject>
  {
    public BocTreeViewSelector ()
        : base ("span", "bocTreeView")
    {
    }
  }
}