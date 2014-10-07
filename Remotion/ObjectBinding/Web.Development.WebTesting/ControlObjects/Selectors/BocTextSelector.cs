using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTextControlObject"/>.
  /// </summary>
  public class BocTextSelector : BocSelectorBase<BocTextControlObject>
  {
    public BocTextSelector ()
        : base ("span", "bocTextValue")
    {
    }
  }
}