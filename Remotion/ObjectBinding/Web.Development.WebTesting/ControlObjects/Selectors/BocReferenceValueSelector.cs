using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocReferenceValueControlObject"/>.
  /// </summary>
  public class BocReferenceValueSelector : BocSelectorBase<BocReferenceValueControlObject>
  {
    public BocReferenceValueSelector ()
        : base ("span", "bocReferenceValue")
    {
    }
  }
}