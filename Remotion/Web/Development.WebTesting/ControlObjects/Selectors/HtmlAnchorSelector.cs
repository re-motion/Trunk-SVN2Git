using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="HtmlAnchorControlObject"/>.
  /// </summary>
  public class HtmlAnchorSelector : HtmlControlSelectorBase<HtmlAnchorControlObject>
  {
    public HtmlAnchorSelector ()
        : base ("a")
    {
    }
  }
}