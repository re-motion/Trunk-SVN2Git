using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueSelector : BocSelectorBase<BocAutoCompleteReferenceValueControlObject>
  {
    public BocAutoCompleteReferenceValueSelector ()
        : base ("span", "bocAutoCompleteReferenceValue")
    {
    }
  }
}