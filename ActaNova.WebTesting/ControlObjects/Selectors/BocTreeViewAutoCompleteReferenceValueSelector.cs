using System;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTreeViewAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class BocTreeViewAutoCompleteReferenceValueSelector : BocControlSelectorBase<BocTreeViewAutoCompleteReferenceValueControlObject>
  {
    public BocTreeViewAutoCompleteReferenceValueSelector ()
        : base ("BocAutoCompleteReferenceValue")
    {
    }

    protected override BocTreeViewAutoCompleteReferenceValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocTreeViewAutoCompleteReferenceValueControlObject (newControlObjectContext);
    }
  }
}