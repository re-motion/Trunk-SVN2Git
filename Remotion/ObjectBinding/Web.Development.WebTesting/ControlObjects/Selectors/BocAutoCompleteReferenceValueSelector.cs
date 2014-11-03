using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueSelector : BocControlSelectorBase<BocAutoCompleteReferenceValueControlObject>
  {
    public BocAutoCompleteReferenceValueSelector ()
        : base ("BocAutoCompleteReferenceValue")
    {
    }

    protected override BocAutoCompleteReferenceValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocAutoCompleteReferenceValueControlObject (newControlObjectContext);
    }
  }
}