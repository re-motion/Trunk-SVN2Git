using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="SingleViewControlObject"/>.
  /// </summary>
  public class SingleViewSelector : TypedControlSelectorBase<SingleViewControlObject>
  {
    public SingleViewSelector ()
        : base ("SingleView")
    {
    }
  }
}