using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTextValueControlObject"/>.
  /// </summary>
  public class BocTextValueSelector : BocSelectorBase<BocTextValueControlObject>
  {
    public BocTextValueSelector ()
        : base ("span", "bocTextValue")
    {
    }
  }
}