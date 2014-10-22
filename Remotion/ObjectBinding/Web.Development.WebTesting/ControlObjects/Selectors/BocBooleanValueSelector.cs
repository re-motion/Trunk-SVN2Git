using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocBooleanValueControlObject"/>.
  /// </summary>
  public class BocBooleanValueSelector : BocSelectorBase<BocBooleanValueControlObject>
  {
    public BocBooleanValueSelector ()
        : base ("span", "bocBooleanValue")
    {
    }
  }
}